using GardenAssistant.Data;
using GardenAssistant.DTOs.Plants;
using Microsoft.EntityFrameworkCore;

using GardenAssistant.Services.Interfaces;

namespace GardenAssistant.Services;

public class PlantService(AppDbContext dbContext) : IPlantService
{
    public async Task<List<PlantDto>> GetAllAsync()
    {
        return await dbContext.Plants
            .Include(p => p.IntrinsicMechanisms)
            .OrderBy(p => p.Name)
            .Select(p => new PlantDto(
                p.Id,
                p.Name,
                p.ScientificName,
                p.Description,
                p.Family,
                p.Genus,
                p.LifeCycle,
                p.HeightAtMaturityCm,
                p.RootDepth,
                p.SunRequirement,
                p.WaterNeeds,
                p.PropagationMethod,
                p.FrostSensitive,
                p.IntrinsicMechanisms.Select(im => im.Mechanism).ToList()
            ))
            .ToListAsync();
    }
}
