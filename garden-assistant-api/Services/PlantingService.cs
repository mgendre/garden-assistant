using GardenAssistant.Data;
using GardenAssistant.Data.Entities;
using GardenAssistant.Data.Entities.Enums;
using GardenAssistant.DTOs;
using Microsoft.EntityFrameworkCore;

namespace GardenAssistant.Services;

public class PlantingService(AppDbContext dbContext)
{
    public async Task<IEnumerable<PlantingDto>> GetAllAsync(Guid userId)
    {
        return await dbContext.Plantings
            .Where(p => p.UserId == userId)
            .Select(p => ToDto(p))
            .ToListAsync();
    }

    public async Task<PlantingDto?> GetByIdAsync(Guid id, Guid userId)
    {
        var planting = await dbContext.Plantings
            .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);
        return planting is null ? null : ToDto(planting);
    }

    public async Task<PlantingDto> CreateAsync(CreatePlantingRequest request, Guid userId)
    {
        var planting = new Planting
        {
            Id = Guid.NewGuid(),
            GardenId = request.GardenId,
            UserId = userId,
            Name = request.Name,
            Description = request.Description,
            PlannedDate = request.PlannedDate
        };

        dbContext.Plantings.Add(planting);
        await dbContext.SaveChangesAsync();

        return ToDto(planting);
    }

    public async Task<bool> DeleteAsync(Guid id, Guid userId)
    {
        var planting = await dbContext.Plantings
            .FirstOrDefaultAsync(p => p.Id == id && p.UserId == userId);
        if (planting is null) return false;

        dbContext.Plantings.Remove(planting);
        await dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<CompatibilityScoreDto> GetCompatibilityScoreAsync(Guid plantingId, Guid userId)
    {
        var plantingExists = await dbContext.Plantings
            .AnyAsync(p => p.Id == plantingId && p.UserId == userId);

        if (!plantingExists)
            return new CompatibilityScoreDto(0, 0, 0, 0);

        var plantIds = await dbContext.PlantingEntries
            .Where(pe => pe.PlantingId == plantingId)
            .Select(pe => pe.PlantId)
            .ToListAsync();

        var associations = await dbContext.PlantAssociations
            .Where(pa => plantIds.Contains(pa.SourcePlantId) && plantIds.Contains(pa.TargetPlantId))
            .ToListAsync();

        var beneficial = associations.Count(a => a.Effect == AssociationEffect.Beneficial);
        var harmful = associations.Count(a => a.Effect == AssociationEffect.Harmful);
        var neutral = associations.Count(a => a.Effect == AssociationEffect.Neutral);

        return new CompatibilityScoreDto(beneficial, harmful, neutral, beneficial + harmful + neutral);
    }

    private static PlantingDto ToDto(Planting p) => new(
        p.Id,
        p.GardenId,
        p.Name,
        p.Description,
        p.PlannedDate
    );
}
