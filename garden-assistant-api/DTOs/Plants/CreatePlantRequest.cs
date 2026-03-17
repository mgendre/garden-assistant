using System.ComponentModel.DataAnnotations;
using GardenAssistant.Data.Entities;
using GardenAssistant.Data.Entities.Enums;

namespace GardenAssistant.DTOs.Plants;

public record CreatePlantRequest(
    [Required][MaxLength(256)] string Name,
    [MaxLength(256)] string? ScientificName,
    [MaxLength(2000)] string? Description,
    [MaxLength(128)] string? Family,
    [MaxLength(128)] string? Genus,
    LifeCycle LifeCycle,
    int? HeightAtMaturityCm,
    RootDepth RootDepth,
    SunRequirement SunRequirement,
    WaterNeeds WaterNeeds,
    bool NitrogenFixer,
    bool AllelopathicRisk,
    bool PollinatorPlant
);
