using GardenAssistant.Data.Entities.Enums;

namespace GardenAssistant.DTOs;

public record PlantToAvoidDto(
    Guid PlantId,
    string PlantName,
    string? ScientificName,
    List<AssociationMechanism> Mechanisms
);
