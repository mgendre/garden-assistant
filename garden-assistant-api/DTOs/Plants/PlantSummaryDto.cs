namespace GardenAssistant.DTOs.Plants;

public record PlantSummaryDto(
    Guid Id,
    string Name,
    string? ScientificName
);
