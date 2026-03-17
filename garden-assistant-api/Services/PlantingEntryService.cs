using GardenAssistant.Data;
using GardenAssistant.Data.Entities;
using GardenAssistant.DTOs.Plantings;
using Microsoft.EntityFrameworkCore;

using GardenAssistant.Services.Interfaces;

namespace GardenAssistant.Services;

public class PlantingEntryService(AppDbContext dbContext) : IPlantingEntryService
{
    public async Task<IEnumerable<PlantingEntryDto>?> GetForPlantingAsync(Guid plantingId, Guid userId)
    {
        var plantingExists = await dbContext.Plantings
            .AnyAsync(p => p.Id == plantingId && p.UserId == userId);

        if (!plantingExists)
        {
            return null;
        }

        return await dbContext.PlantingEntries
            .Where(pe => pe.PlantingId == plantingId)
            .Select(pe => ToDto(pe))
            .ToListAsync();
    }

    public async Task<PlantingEntryDto?> AddEntryAsync(Guid plantingId, CreatePlantingEntryRequest request, Guid userId)
    {
        var plantingExists = await dbContext.Plantings
            .AnyAsync(p => p.Id == plantingId && p.UserId == userId);

        if (!plantingExists)
        {
            return null;
        }

        var entry = new PlantingEntry
        {
            Id = Guid.NewGuid(),
            PlantingId = plantingId,
            PlantId = request.PlantId,
            Quantity = request.Quantity,
            PositionX = request.PositionX,
            PositionY = request.PositionY,
            Layer = request.Layer,
            PlannedSowDate = request.PlannedSowDate,
            PlannedHarvestDate = request.PlannedHarvestDate,
            Notes = request.Notes
        };

        dbContext.PlantingEntries.Add(entry);
        await dbContext.SaveChangesAsync();

        return ToDto(entry);
    }

    public async Task<bool> RemoveEntryAsync(Guid entryId, Guid userId)
    {
        var entry = await dbContext.PlantingEntries.FindAsync(entryId);
        if (entry is null)
        {
            return false;
        }

        var plantingBelongsToUser = await dbContext.Plantings
            .AnyAsync(p => p.Id == entry.PlantingId && p.UserId == userId);

        if (!plantingBelongsToUser)
        {
            return false;
        }

        dbContext.PlantingEntries.Remove(entry);
        await dbContext.SaveChangesAsync();

        return true;
    }

    private static PlantingEntryDto ToDto(PlantingEntry pe) => new(
        pe.Id,
        pe.PlantingId,
        pe.PlantId,
        pe.Quantity,
        pe.PositionX,
        pe.PositionY,
        pe.Layer,
        pe.PlannedSowDate,
        pe.PlannedHarvestDate,
        pe.ActualHarvestDate,
        pe.Notes
    );
}
