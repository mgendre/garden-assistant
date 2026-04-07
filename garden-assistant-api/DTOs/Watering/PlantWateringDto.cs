using GardenAssistant.Data.Entities.Enums;

namespace GardenAssistant.DTOs.Watering;

public record PlantWateringDto(Guid PlantId, string PlantName, WaterNeeds WaterNeeds, int TimesPerWeek, DayOfWeek[] RecommendedDays);
