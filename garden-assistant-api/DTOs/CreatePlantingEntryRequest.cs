using GardenAssistant.Data.Entities;
using GardenAssistant.Data.Entities.Enums;

namespace GardenAssistant.DTOs;

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
