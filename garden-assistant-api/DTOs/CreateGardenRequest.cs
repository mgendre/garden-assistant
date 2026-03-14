namespace GardenAssistant.DTOs;

public record CreateGardenRequest(string Name, string? Description, Guid UserId);
