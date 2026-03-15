using GardenAssistant.Data.Entities;
using GardenAssistant.Data.Entities.Enums;

namespace GardenAssistant.DTOs;

public record CreatePlantAssociationRequest(
    Guid SourcePlantId,
    Guid TargetPlantId,
    AssociationMechanism Mechanism,
    AssociationEffect Effect,
    DistanceEffect DistanceEffect,
    ConfidenceLevel ConfidenceLevel,
    string? Notes
);
