using GardenAssistant.Data.Entities.Enums;

namespace GardenAssistant.DTOs.Plants;

public record PlantDto(
    Guid Id,
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
    PropagationMethod PropagationMethod,
    bool FrostSensitive,
    List<AssociationMechanism> IntrinsicMechanisms
);
