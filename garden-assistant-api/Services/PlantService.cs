using GardenAssistant.Data;
using GardenAssistant.Data.Entities;
using GardenAssistant.DTOs;
using Microsoft.EntityFrameworkCore;

using GardenAssistant.Services.Interfaces;

namespace GardenAssistant.Services;

public class PlantService(AppDbContext dbContext) : IPlantService
{
    public async Task<IEnumerable<PlantDto>> GetAllAsync()
    {
        return await dbContext.Plants
            .OrderBy(p => p.Name)
            .Take(20)
            .Select(p => ToDto(p))
            .ToListAsync();
    }

    public async Task<IEnumerable<PlantDto>> SearchAsync(string query)
    {
        var q = query.ToLower();
        return await dbContext.Plants
            .Where(p => p.Name.ToLower().Contains(q)
                     || (p.ScientificName != null && p.ScientificName.ToLower().Contains(q)))
            .OrderBy(p => p.Name)
            .Take(20)
            .Select(p => ToDto(p))
            .ToListAsync();
    }

    public async Task<PlantDto?> GetByIdAsync(Guid id)
    {
        var plant = await dbContext.Plants.FindAsync(id);
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
            WaterNeeds = request.WaterNeeds,
            NitrogenFixer = request.NitrogenFixer,
            AllelopathicRisk = request.AllelopathicRisk,
            PollinatorPlant = request.PollinatorPlant
        };

        dbContext.Plants.Add(plant);
        await dbContext.SaveChangesAsync();

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
        p.NitrogenFixer,
        p.AllelopathicRisk,
        p.PollinatorPlant
    );
}
