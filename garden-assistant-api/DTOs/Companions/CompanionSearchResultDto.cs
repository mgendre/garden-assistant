using GardenAssistant.Data.Entities.Enums;

namespace GardenAssistant.DTOs.Companions;

public record CompanionSearchResultDto(
    List<CompanionRecommendationDto> GoodCompanions,
    List<CompanionRecommendationDto> PlantsToAvoid,
    List<SelectedPlantConflictDto> SelectedPlantConflicts,
    List<AssociationMechanism> SelectedPlantMechanisms,
    List<PlantMechanismsDto> SelectedPlantsMechanisms,
    List<PlantMechanismsDto> IntrinsicMechanismsByPlant,
    List<GuildAssociationDto> SelectedPlantAssociations
);
