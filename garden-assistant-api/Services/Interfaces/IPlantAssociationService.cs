using GardenAssistant.DTOs;

namespace GardenAssistant.Services.Interfaces;

public interface IPlantAssociationService
{
    Task<IEnumerable<PlantAssociationDto>> GetForPlantAsync(Guid plantId);
    Task<CompanionSearchResultDto> GetCompanionRecommendationsAsync(List<Guid> selectedPlantIds);
    Task<PlantAssociationDto> CreateAsync(CreatePlantAssociationRequest request);
    Task<bool> DeleteAsync(Guid id);
}
