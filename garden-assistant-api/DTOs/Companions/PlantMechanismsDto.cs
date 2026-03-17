using GardenAssistant.Data.Entities.Enums;

namespace GardenAssistant.DTOs.Companions;

public record PlantMechanismsDto(Guid PlantId, List<AssociationMechanism> Mechanisms);
