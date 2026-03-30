using GardenAssistant.Data.Entities.Enums;

namespace GardenAssistant.DTOs.Companions;

public record CompanionRecommendationDto(
    Guid PlantId,
    List<AssociationMechanism> Mechanisms,
    List<Guid> LinkedPlantIds
);
