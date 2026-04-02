using GardenAssistant.Data;
using GardenAssistant.DTOs.Plants;
using GardenAssistant.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GardenAssistant.Services;

public class PlantActionService(AppDbContext dbContext) : IPlantActionService
{
    public async Task<List<PlantActionDto>> GetByPlantIdAsync(Guid plantId)
    {
        var actions = await QueryActions(plantId);

        if (actions.Count == 0)
        {
            var parentId = await dbContext.Plants
                .Where(p => p.Id == plantId)
                .Select(p => p.ParentPlantId)
                .FirstOrDefaultAsync();

            if (parentId.HasValue)
            {
                actions = await QueryActions(parentId.Value);
            }
        }

        return actions;
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

        var result = actions
            .GroupBy(a => a.PlantId)
            .ToDictionary(g => g.Key, g => g.Select(a => a.Dto).ToList());

        var missingIds = plantIdSet.Except(result.Keys).ToList();
        if (missingIds.Count > 0)
        {
            var parentMap = await dbContext.Plants
                .Where(p => missingIds.Contains(p.Id) && p.ParentPlantId.HasValue)
                .Select(p => new { p.Id, p.ParentPlantId })
                .ToListAsync();

            var parentIds = parentMap.Select(pm => pm.ParentPlantId!.Value).Distinct().ToHashSet();
            var parentActions = await dbContext.PlantActions
                .Where(pa => parentIds.Contains(pa.PlantId))
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

            var parentActionsLookup = parentActions
                .GroupBy(a => a.PlantId)
                .ToDictionary(g => g.Key, g => g.Select(a => a.Dto).ToList());

            foreach (var pm in parentMap)
            {
                if (parentActionsLookup.TryGetValue(pm.ParentPlantId!.Value, out var inherited))
                {
                    result[pm.Id] = inherited;
                }
            }
        }

        return result;
    }

    private async Task<List<PlantActionDto>> QueryActions(Guid plantId)
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
}
