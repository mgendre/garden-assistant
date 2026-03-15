using GardenAssistant.DTOs;

namespace GardenAssistant.Services.Interfaces;

public interface IPlantingEntryService
{
    Task<IEnumerable<PlantingEntryDto>?> GetForPlantingAsync(Guid plantingId, Guid userId);
    Task<PlantingEntryDto?> AddEntryAsync(Guid plantingId, CreatePlantingEntryRequest request, Guid userId);
    Task<bool> RemoveEntryAsync(Guid entryId, Guid userId);
}
