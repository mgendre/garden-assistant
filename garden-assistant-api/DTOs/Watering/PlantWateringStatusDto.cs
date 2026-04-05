namespace GardenAssistant.DTOs.Watering;

public record PlantWateringStatusDto(Guid PlantId, string PlantName, bool IsToday, DayOfWeek? NextWateringDay);
