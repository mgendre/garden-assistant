using GardenAssistant.Data.Entities;
using GardenAssistant.Data.Entities.Enums;

namespace GardenAssistant.DTOs.Companions;

public record CreatePlantAssociationRequest(
    Guid SourcePlantId,
    Guid TargetPlantId,
    AssociationMechanism Mechanism,
    AssociationEffect Effect,
    DistanceEffect DistanceEffect,
    ConfidenceLevel ConfidenceLevel,
    string? Notes
);
