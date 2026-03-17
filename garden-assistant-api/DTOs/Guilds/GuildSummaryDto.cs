namespace GardenAssistant.DTOs.Guilds;

public record GuildSummaryDto(Guid Id, string Name, string? Description, List<GuildPlantMemberDto> Plants, bool IsOfficial);
