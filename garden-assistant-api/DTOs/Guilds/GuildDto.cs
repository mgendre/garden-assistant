namespace GardenAssistant.DTOs.Guilds;

public record GuildDto(Guid Id, string Name, string? Description, List<GuildPlantMemberDto> Plants, bool IsOfficial, bool IsOwner);
