using GardenAssistant.Data.Entities;
using GardenAssistant.Data.Entities.Enums;

namespace GardenAssistant.DTOs.Plantings;

public record PlantingEntryDto(
    Guid Id,
    Guid PlantingId,
    Guid PlantId,
    int? Quantity,
    float? PositionX,
    float? PositionY,
    PlantingLayer? Layer,
    DateOnly? PlannedSowDate,
    DateOnly? PlannedHarvestDate,
    DateOnly? ActualHarvestDate,
    string? Notes
);
