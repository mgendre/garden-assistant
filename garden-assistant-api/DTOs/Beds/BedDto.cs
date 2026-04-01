namespace GardenAssistant.DTOs.Beds;

public record BedDto(Guid Id, string Name, Guid? GuildId, List<Guid> PlantIds);
