using GardenAssistant.DTOs.Plants;

namespace GardenAssistant.Services.Interfaces;

public interface IPlantService
{
    Task<List<PlantDto>> GetAllAsync();
    Task<PlantDto?> GetByIdAsync(Guid id);
    Task<PlantDto> CreateAsync(CreatePlantRequest request);
    Task<bool> DeleteAsync(Guid id);
}
