using GardenAssistant.Data.Entities.Enums;

namespace GardenAssistant.DTOs.Plants;

public record PlantActionDto(
    Guid Id,
    PlantActionType ActionType,
    int HalfMonthStart,
    int HalfMonthEnd,
    string? Notes
);
