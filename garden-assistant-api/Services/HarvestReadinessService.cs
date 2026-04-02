using GardenAssistant.Data;
using GardenAssistant.DTOs.Plants;
using GardenAssistant.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GardenAssistant.Services;

public class HarvestReadinessService(AppDbContext dbContext) : IHarvestReadinessService
{
    public async Task<HarvestReadinessDto?> GetByPlantIdAsync(Guid plantId)
    {
        var harvestReadiness = await dbContext.HarvestReadiness
            .Include(hr => hr.Criteria)
            .FirstOrDefaultAsync(hr => hr.PlantId == plantId);

        if (harvestReadiness is null)
        {
            var parentId = await dbContext.Plants
                .Where(p => p.Id == plantId)
                .Select(p => p.ParentPlantId)
                .FirstOrDefaultAsync();

            if (parentId.HasValue)
            {
                harvestReadiness = await dbContext.HarvestReadiness
                    .Include(hr => hr.Criteria)
                    .FirstOrDefaultAsync(hr => hr.PlantId == parentId.Value);
            }
        }

        if (harvestReadiness is null)
        {
            return null;
        }

        return new HarvestReadinessDto(
            harvestReadiness.Description,
            harvestReadiness.DaysFromTransplant,
            harvestReadiness.DaysFromSowing,
            harvestReadiness.Criteria
                .OrderBy(c => c.CriterionType)
                .Select(c => new HarvestReadinessCriterionDto(c.CriterionType, c.Description))
                .ToList()
        );
    }
}
