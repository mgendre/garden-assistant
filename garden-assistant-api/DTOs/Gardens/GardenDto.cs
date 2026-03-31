namespace GardenAssistant.DTOs.Gardens;

public record GardenDto(Guid Id, string Name, string? Description, int BedCount, DateTime CreatedAtUtc);
