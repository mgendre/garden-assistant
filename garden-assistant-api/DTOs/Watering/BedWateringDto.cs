using GardenAssistant.Data.Entities.Enums;

namespace GardenAssistant.DTOs.Watering;

public record BedWateringDto(Guid? BedId, string BedName, bool IsPersonalPlants, SoilType? SoilType, bool HasMulch, List<PlantWateringDto> Plants);
