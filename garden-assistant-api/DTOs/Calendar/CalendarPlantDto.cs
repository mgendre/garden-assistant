using GardenAssistant.DTOs.Plants;

namespace GardenAssistant.DTOs.Calendar;

public record CalendarPlantDto(
    Guid PlantId,
    List<PlantActionDto> Actions
);
