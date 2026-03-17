using GardenAssistant.Data.Entities.Enums;
using GardenAssistant.DTOs.Guilds;

namespace GardenAssistant.DTOs.Companions;

public record CompanionRecommendationDto(
    Guid PlantId,
    string PlantName,
    string? ScientificName,
    double Score,
    List<AssociationMechanism> Mechanisms,
    List<GuildInfoDto> Guilds
);
