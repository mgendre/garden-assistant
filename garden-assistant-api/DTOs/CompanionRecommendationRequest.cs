using System.ComponentModel.DataAnnotations;

namespace GardenAssistant.DTOs;

public record CompanionRecommendationRequest(
    [Required] [MinLength(1)] List<Guid> PlantIds
);
