using GardenAssistant.Data;
using GardenAssistant.Data.Entities;
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
            .Select(p => ToDto(p))
            .ToListAsync();
    }

    public async Task<PlantDto?> GetByIdAsync(Guid id)
    {
        var plant = await dbContext.Plants
            .Include(p => p.IntrinsicMechanisms)
            .FirstOrDefaultAsync(p => p.Id == id);
        return plant is null ? null : ToDto(plant);
    }

    public async Task<PlantDto> CreateAsync(CreatePlantRequest request)
    {
        var plant = new Plant
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            ScientificName = request.ScientificName,
            Description = request.Description,
            Family = request.Family,
            Genus = request.Genus,
            LifeCycle = request.LifeCycle,
            HeightAtMaturityCm = request.HeightAtMaturityCm,
            RootDepth = request.RootDepth,
            SunRequirement = request.SunRequirement,
            WaterNeeds = request.WaterNeeds
        };

        dbContext.Plants.Add(plant);

        foreach (var mechanism in request.IntrinsicMechanisms ?? [])
        {
            dbContext.PlantIntrinsicMechanisms.Add(new PlantIntrinsicMechanism
            {
                PlantId = plant.Id,
                Mechanism = mechanism
            });
        }

        await dbContext.SaveChangesAsync();

        plant.IntrinsicMechanisms = await dbContext.PlantIntrinsicMechanisms
            .Where(pim => pim.PlantId == plant.Id)
            .ToListAsync();

        return ToDto(plant);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var plant = await dbContext.Plants.FindAsync(id);
        if (plant is null)
        {
            return false;
        }

        dbContext.Plants.Remove(plant);
        await dbContext.SaveChangesAsync();

        return true;
    }

    private static PlantDto ToDto(Plant p) => new(
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
        p.IntrinsicMechanisms.Select(im => im.Mechanism).ToList()
    );
}
