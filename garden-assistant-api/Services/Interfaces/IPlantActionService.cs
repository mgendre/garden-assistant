    using GardenAssistant.DTOs.Plants;

namespace GardenAssistant.Services.Interfaces;

public interface IPlantActionService
{
    Task<List<PlantActionDto>> GetByPlantIdAsync(Guid plantId);
    Task<Dictionary<Guid, List<PlantActionDto>>> GetByPlantIdsAsync(IEnumerable<Guid> plantIds);
}
