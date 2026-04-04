using GardenAssistant.DTOs.Companions;

namespace GardenAssistant.Services.Interfaces;

public interface IPlantAssociationService
{
    Task<CompanionSearchResultDto> GetCompanionRecommendationsAsync(List<Guid> selectedPlantIds, List<Guid>? centralPlantIds = null, double? minScore = null);
}
