using GardenAssistant.Data.Entities;
using GardenAssistant.Data.Entities.Enums;

namespace GardenAssistant.DTOs;

public record PlantAssociationDto(
    Guid Id,
    Guid SourcePlantId,
    Guid TargetPlantId,
    AssociationMechanism Mechanism,
    AssociationEffect Effect,
    DistanceEffect DistanceEffect,
    ConfidenceLevel ConfidenceLevel,
    string? Notes
);
