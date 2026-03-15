using GardenAssistant.DTOs;

namespace GardenAssistant.Services.Interfaces;

public interface IPlantService
{
    Task<IEnumerable<PlantDto>> GetAllAsync();
    Task<IEnumerable<PlantDto>> SearchAsync(string query);
    Task<PlantDto?> GetByIdAsync(Guid id);
    Task<PlantDto> CreateAsync(CreatePlantRequest request);
    Task<bool> DeleteAsync(Guid id);
}
