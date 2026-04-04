using System.ComponentModel.DataAnnotations;

namespace GardenAssistant.DTOs.Companions;

public record CompanionRecommendationRequest(
    [Required] [MinLength(1)] List<Guid> PlantIds,
    List<Guid>? CentralPlantIds = null,
    double? MinScore = null
);
