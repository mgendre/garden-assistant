using GardenAssistant.Data.Entities.Enums;

namespace GardenAssistant.DTOs.Companions;

public record SelectedPlantConflictDto(
    Guid PlantAId,
    Guid PlantBId,
    List<AssociationMechanism> Mechanisms
);
