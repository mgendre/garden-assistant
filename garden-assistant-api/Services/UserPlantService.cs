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
        var plants = await dbContext.UserPlants
            .Where(up => up.UserId == userId)
            .OrderByDescending(up => up.AddedAtUtc)
            .Join(dbContext.Plants
                    .Include(p => p.IntrinsicMechanisms)
                    .Include(p => p.ParentPlant)
                        .ThenInclude(pp => pp!.IntrinsicMechanisms)
                    .Include(p => p.Varieties)
                    .AsSplitQuery(),
                up => up.PlantId, p => p.Id, (up, p) => p)
            .ToListAsync();

        return plants.Select(PlantService.ToDto);
    }

    public async Task<PlantDto?> AddAsync(Guid plantId, Guid userId)
    {
        var plant = await dbContext.Plants
            .Include(p => p.IntrinsicMechanisms)
            .Include(p => p.ParentPlant)
                .ThenInclude(pp => pp!.IntrinsicMechanisms)
            .Include(p => p.Varieties)
            .AsSplitQuery()
            .FirstOrDefaultAsync(p => p.Id == plantId);
        if (plant is null)
        {
            return null;
        }

        var existing = await dbContext.UserPlants
            .FirstOrDefaultAsync(up => up.UserId == userId && up.PlantId == plantId);

        if (existing is not null)
        {
            return PlantService.ToDto(plant);
        }

        dbContext.UserPlants.Add(new UserPlant
        {
            UserId = userId,
            PlantId = plantId,
            AddedAtUtc = DateTime.UtcNow
        });

        await dbContext.SaveChangesAsync();

        return PlantService.ToDto(plant);
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

}
