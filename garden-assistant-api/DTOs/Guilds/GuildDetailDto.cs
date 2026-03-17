namespace GardenAssistant.DTOs.Guilds;

public record GuildDetailDto(Guid Id, string Name, string? Description, List<GuildPlantMemberDto> Plants, bool IsOfficial, bool IsOwner);
