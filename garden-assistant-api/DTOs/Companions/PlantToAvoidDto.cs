using GardenAssistant.Data.Entities.Enums;

namespace GardenAssistant.DTOs.Companions;

public record PlantToAvoidDto(
    Guid PlantId,
    string PlantName,
    string? ScientificName,
    List<AssociationMechanism> Mechanisms
);
