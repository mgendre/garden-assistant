using GardenAssistant.Data.Entities.Enums;

namespace GardenAssistant.DTOs.Companions;

public record SelectedPlantConflictDto(
    Guid PlantAId,
    string PlantAName,
    Guid PlantBId,
    string PlantBName,
    List<AssociationMechanism> Mechanisms
);
