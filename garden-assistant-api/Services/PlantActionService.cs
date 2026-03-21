using GardenAssistant.Data;
using GardenAssistant.DTOs.Plants;
using GardenAssistant.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GardenAssistant.Services;

public class PlantActionService(AppDbContext dbContext) : IPlantActionService
{
    public async Task<List<PlantActionDto>> GetByPlantIdAsync(Guid plantId)
    {
        return await dbContext.PlantActions
            .Where(pa => pa.PlantId == plantId)
            .OrderBy(pa => pa.ActionType)
            .ThenBy(pa => pa.HalfMonthStart)
            .Select(pa => new PlantActionDto(
                pa.Id,
                pa.ActionType,
                pa.HalfMonthStart,
                pa.HalfMonthEnd,
                pa.Notes
            ))
            .ToListAsync();
    }

    public async Task<Dictionary<Guid, List<PlantActionDto>>> GetByPlantIdsAsync(IEnumerable<Guid> plantIds)
    {
        var plantIdSet = plantIds.ToHashSet();

        var actions = await dbContext.PlantActions
            .Where(pa => plantIdSet.Contains(pa.PlantId))
            .OrderBy(pa => pa.ActionType)
            .ThenBy(pa => pa.HalfMonthStart)
            .Select(pa => new { pa.PlantId, Dto = new PlantActionDto(
                pa.Id,
                pa.ActionType,
                pa.HalfMonthStart,
                pa.HalfMonthEnd,
                pa.Notes
            )})
            .ToListAsync();

        return actions
            .GroupBy(a => a.PlantId)
            .ToDictionary(g => g.Key, g => g.Select(a => a.Dto).ToList());
    }
}
