using GardenAssistant.Data.Entities;
using GardenAssistant.Data.Entities.Enums;

namespace GardenAssistant.DTOs.Plantings;

public record CreatePlantingEntryRequest(
    Guid PlantId,
    int? Quantity,
    float? PositionX,
    float? PositionY,
    PlantingLayer? Layer,
    DateOnly? PlannedSowDate,
    DateOnly? PlannedHarvestDate,
    string? Notes
);
