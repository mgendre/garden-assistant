using GardenAssistant.Data.Entities.Enums;

namespace GardenAssistant.Data.Seeders.Records;

public record AssociationSeedRecord(
    string SourcePlantKey,
    string TargetPlantKey,
    AssociationMechanism Mechanism,
    AssociationEffect Effect,
    DistanceEffect DistanceEffect,
    ConfidenceLevel ConfidenceLevel,
    string? Notes
);
