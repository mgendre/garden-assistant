namespace GardenAssistant.DTOs;

public record GuildSummaryDto(Guid Id, string Name, string? Description, int PlantCount);

public record GuildDetailDto(Guid Id, string Name, string? Description, List<GuildPlantMemberDto> Plants);

public record GuildPlantMemberDto(Guid Id, string Name, string? ScientificName);
