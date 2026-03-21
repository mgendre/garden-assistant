using GardenAssistant.DTOs.Plants;

namespace GardenAssistant.Services.Interfaces;

public interface IHarvestReadinessService
{
    Task<HarvestReadinessDto?> GetByPlantIdAsync(Guid plantId);
}
