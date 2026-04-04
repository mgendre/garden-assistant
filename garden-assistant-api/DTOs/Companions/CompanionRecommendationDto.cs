using GardenAssistant.Data.Entities.Enums;

namespace GardenAssistant.DTOs.Companions;

public record CompanionRecommendationDto(
    Guid PlantId,
    List<AssociationMechanism> Mechanisms,
    List<AssociationMechanism> HarmfulMechanisms,
    List<Guid> LinkedPlantIds,
    int Rating,
    double Score,
    bool HasRootDepthBonus,
    bool HasSameFamilyMalus,
    bool HasWaterIncompatibility
);
