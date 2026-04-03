using GardenAssistant.Data.Entities.Enums;

namespace GardenAssistant.Data.Seeders.Records;

public record ActionRecord(PlantActionType ActionType, int HalfMonthStart, int HalfMonthEnd, string? Notes);
