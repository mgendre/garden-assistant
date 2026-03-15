namespace GardenAssistant.DTOs;

public record CompatibilityScoreDto(
    int Beneficial,
    int Harmful,
    int Neutral,
    int Total
);
