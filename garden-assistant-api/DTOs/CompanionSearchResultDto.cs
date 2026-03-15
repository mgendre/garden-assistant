namespace GardenAssistant.DTOs;

public record CompanionSearchResultDto(
    List<CompanionRecommendationDto> GoodCompanions,
    List<PlantToAvoidDto> PlantsToAvoid,
    List<SelectedPlantConflictDto> SelectedPlantConflicts
);
