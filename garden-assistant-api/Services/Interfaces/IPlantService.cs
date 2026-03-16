using GardenAssistant.DTOs;

namespace GardenAssistant.Services.Interfaces;

public interface IPlantService
{
    Task<PaginatedResult<PlantDto>> GetAllAsync(string? search = null);
    Task<PlantDto?> GetByIdAsync(Guid id);
    Task<PlantDto> CreateAsync(CreatePlantRequest request);
    Task<bool> DeleteAsync(Guid id);
}
