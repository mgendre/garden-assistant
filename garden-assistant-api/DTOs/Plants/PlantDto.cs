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
    int? MaxAltitudeM,
    bool FrostSensitive,
    List<AssociationMechanism> IntrinsicMechanisms,
    List<string> SoilTypes,
    decimal? OptimalPhMin,
    decimal? OptimalPhMax,
    bool IsVariety,
    Guid? ParentPlantId,
    string? ParentPlantName,
    List<PlantSummaryDto> Varieties,
    HarvestReadinessDto? HarvestReadiness,
    List<PlantActionDto> Actions,
    int? WaterAmountMl
);
