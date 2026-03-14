using GardenAssistant.Data;
using GardenAssistant.Data.Entities;
using GardenAssistant.DTOs;
using Microsoft.EntityFrameworkCore;

namespace GardenAssistant.Services;

public class PlantAssociationService(AppDbContext dbContext)
{
    public async Task<IEnumerable<PlantAssociationDto>> GetForPlantAsync(Guid plantId)
    {
        return await dbContext.PlantAssociations
            .Where(pa => pa.SourcePlantId == plantId || pa.TargetPlantId == plantId)
            .Select(pa => ToDto(pa))
            .ToListAsync();
    }

    public async Task<PlantAssociationDto> CreateAsync(CreatePlantAssociationRequest request)
    {
        var association = new PlantAssociation
        {
            Id = Guid.NewGuid(),
            SourcePlantId = request.SourcePlantId,
            TargetPlantId = request.TargetPlantId,
            Mechanism = request.Mechanism,
            Effect = request.Effect,
            DistanceEffect = request.DistanceEffect,
            ConfidenceLevel = request.ConfidenceLevel,
            Notes = request.Notes
        };

        dbContext.PlantAssociations.Add(association);
        await dbContext.SaveChangesAsync();

        return ToDto(association);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var association = await dbContext.PlantAssociations.FindAsync(id);
        if (association is null) return false;

        dbContext.PlantAssociations.Remove(association);
        await dbContext.SaveChangesAsync();

        return true;
    }

    private static PlantAssociationDto ToDto(PlantAssociation pa) => new(
        pa.Id,
        pa.SourcePlantId,
        pa.TargetPlantId,
        pa.Mechanism,
        pa.Effect,
        pa.DistanceEffect,
        pa.ConfidenceLevel,
        pa.Notes
    );
}
