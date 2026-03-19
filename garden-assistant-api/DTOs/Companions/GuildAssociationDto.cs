using GardenAssistant.Data.Entities.Enums;

namespace GardenAssistant.DTOs.Companions;

public record GuildAssociationDto(
    Guid SourcePlantId,
    Guid TargetPlantId,
    AssociationMechanism Mechanism,
    AssociationEffect Effect,
    string? Notes
);
