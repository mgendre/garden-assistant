using GardenAssistant.DTOs.Plants;

namespace GardenAssistant.Services.Interfaces;

public interface IUserPlantService
{
    Task<IEnumerable<PlantDto>> GetAllAsync(Guid userId);
    Task<PlantDto?> AddAsync(Guid plantId, Guid userId);
    Task<bool> RemoveAsync(Guid plantId, Guid userId);
}
