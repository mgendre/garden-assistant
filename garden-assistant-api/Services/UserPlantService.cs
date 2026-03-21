using GardenAssistant.Data;
using GardenAssistant.Data.Entities;
using GardenAssistant.DTOs.Plants;
using GardenAssistant.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GardenAssistant.Services;

public class UserPlantService(AppDbContext dbContext) : IUserPlantService
{
    public async Task<IEnumerable<PlantDto>> GetAllAsync(Guid userId)
    {
        var userPlantIds = await dbContext.UserPlants
            .Where(up => up.UserId == userId)
            .OrderByDescending(up => up.AddedAtUtc)
            .Select(up => up.PlantId)
            .ToListAsync();

        var plants = await dbContext.Plants
            .Include(p => p.IntrinsicMechanisms)
            .Where(p => userPlantIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id);

        return userPlantIds
            .Where(id => plants.ContainsKey(id))
            .Select(id => ToDto(plants[id]));
    }

    public async Task<PlantDto?> AddAsync(Guid plantId, Guid userId)
    {
        var plant = await dbContext.Plants
            .Include(p => p.IntrinsicMechanisms)
            .FirstOrDefaultAsync(p => p.Id == plantId);
        if (plant is null)
        {
            return null;
        }

        var existing = await dbContext.UserPlants
            .FirstOrDefaultAsync(up => up.UserId == userId && up.PlantId == plantId);

        if (existing is not null)
        {
            return ToDto(plant);
        }

        dbContext.UserPlants.Add(new UserPlant
        {
            UserId = userId,
            PlantId = plantId,
            AddedAtUtc = DateTime.UtcNow
        });

        await dbContext.SaveChangesAsync();

        return ToDto(plant);
    }

    public async Task<bool> RemoveAsync(Guid plantId, Guid userId)
    {
        var userPlant = await dbContext.UserPlants
            .FirstOrDefaultAsync(up => up.UserId == userId && up.PlantId == plantId);

        if (userPlant is null)
        {
            return false;
        }

        dbContext.UserPlants.Remove(userPlant);
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
        p.PropagationMethod,
        p.FrostSensitive,
        p.IntrinsicMechanisms.Select(im => im.Mechanism).ToList()
    );
}
