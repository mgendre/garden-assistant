namespace GardenAssistant.DTOs;

public record CompanionRecommendationDto(
    Guid PlantId,
    string PlantName,
    string? ScientificName,
    double Score
);
