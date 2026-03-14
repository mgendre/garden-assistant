namespace GardenAssistant.DTOs;

public record GardenDto(Guid Id, string Name, string? Description, Guid UserId);
