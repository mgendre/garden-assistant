namespace GardenAssistant.DTOs.Companions;

public record CompatibilityScoreDto(
    int Beneficial,
    int Harmful,
    int Neutral,
    int Total
);
