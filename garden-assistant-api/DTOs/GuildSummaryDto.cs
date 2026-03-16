namespace GardenAssistant.DTOs;

public record GuildSummaryDto(Guid Id, string Name, string? Description, int PlantCount, bool IsOfficial);
