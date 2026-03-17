namespace GardenAssistant.DTOs.Plantings;

public record PlantingDto(
    Guid Id,
    Guid GardenId,
    string Name,
    string? Description,
    DateOnly? PlannedDate
);
