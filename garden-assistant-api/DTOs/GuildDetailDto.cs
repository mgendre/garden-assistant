namespace GardenAssistant.DTOs;

public record GuildDetailDto(Guid Id, string Name, string? Description, List<GuildPlantMemberDto> Plants);
