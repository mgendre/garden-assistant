using GardenAssistant.DTOs.Gardens;

namespace GardenAssistant.Services.Interfaces;

public interface IGardenService
{
    Task<IEnumerable<GardenDto>> GetAllAsync(Guid userId);
    Task<GardenDto> CreateAsync(CreateGardenRequest request, Guid userId);
    Task<GardenDto?> UpdateAsync(Guid id, UpdateGardenRequest request, Guid userId);
    Task<bool> DeleteAsync(Guid id, Guid userId);
}
