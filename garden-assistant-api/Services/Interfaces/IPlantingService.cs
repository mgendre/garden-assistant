using GardenAssistant.DTOs;

namespace GardenAssistant.Services.Interfaces;

public interface IPlantingService
{
    Task<IEnumerable<PlantingDto>> GetAllAsync(Guid userId);
    Task<PlantingDto?> GetByIdAsync(Guid id, Guid userId);
    Task<PlantingDto> CreateAsync(CreatePlantingRequest request, Guid userId);
    Task<bool> DeleteAsync(Guid id, Guid userId);
    Task<CompatibilityScoreDto> GetCompatibilityScoreAsync(Guid plantingId, Guid userId);
}
