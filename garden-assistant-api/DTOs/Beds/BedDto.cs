using GardenAssistant.Data.Entities.Enums;

namespace GardenAssistant.DTOs.Beds;

public record BedDto(Guid Id, string Name, Guid? GuildId, List<Guid> PlantIds, SoilType? SoilType, bool HasMulch);
