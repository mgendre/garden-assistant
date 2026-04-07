using GardenAssistant.Data.Entities.Enums;

namespace GardenAssistant.Data.Seeders.Records;

public record PlantSeedRecord(
    string Key,
    string Name,
    string? ScientificName,
    string? Description,
    string? Family,
    string? Genus,
    LifeCycle LifeCycle,
    int? HeightAtMaturityCm,
    RootDepth RootDepth,
    SunRequirement SunRequirement,
    WaterNeeds WaterNeeds,
    int? MaxAltitudeM,
    List<AssociationMechanism>? IntrinsicMechanisms,
    PropagationMethod? PropagationMethod,
    bool? FrostSensitive,
    List<SoilType>? SoilTypes,
    decimal? OptimalPhMin,
    decimal? OptimalPhMax,
    string? ParentKey
);
