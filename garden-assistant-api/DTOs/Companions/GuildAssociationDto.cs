using GardenAssistant.Data.Entities.Enums;

namespace GardenAssistant.DTOs.Companions;

public record GuildAssociationDto(
    Guid SourcePlantId,
    string SourcePlantName,
    Guid TargetPlantId,
    string TargetPlantName,
    AssociationMechanism Mechanism,
    AssociationEffect Effect,
    string? Notes
);
