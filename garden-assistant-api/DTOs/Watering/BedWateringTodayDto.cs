namespace GardenAssistant.DTOs.Watering;

public record BedWateringTodayDto(Guid? BedId, string BedName, bool IsPersonalPlants, List<PlantWateringStatusDto> Plants);
