using GardenAssistant.DTOs.Beds;

namespace GardenAssistant.Services.Interfaces;

public interface IBedService
{
    Task<IEnumerable<BedDto>> GetByGardenIdAsync(Guid gardenId, Guid userId);
    Task<BedDto?> CreateAsync(Guid gardenId, CreateBedRequest request, Guid userId);
    Task<BedDto?> UpdateAsync(Guid gardenId, Guid bedId, UpdateBedRequest request, Guid userId);
    Task<bool> DeleteAsync(Guid gardenId, Guid bedId, Guid userId);
}
