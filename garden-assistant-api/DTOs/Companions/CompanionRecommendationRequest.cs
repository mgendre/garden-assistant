using System.ComponentModel.DataAnnotations;

namespace GardenAssistant.DTOs.Companions;

public record CompanionRecommendationRequest(
    [Required] [MinLength(1)] List<Guid> PlantIds,
    double? MinScore = null
);
