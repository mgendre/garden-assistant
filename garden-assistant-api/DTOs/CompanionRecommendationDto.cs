using GardenAssistant.Data.Entities.Enums;

namespace GardenAssistant.DTOs;

public record CompanionRecommendationDto(
    Guid PlantId,
    string PlantName,
    string? ScientificName,
    double Score,
    List<AssociationMechanism> Mechanisms,
    List<GuildInfoDto> Guilds
);
