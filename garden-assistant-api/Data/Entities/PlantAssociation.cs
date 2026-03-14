using GardenAssistant.Data.Entities.Enums;

namespace GardenAssistant.Data.Entities;

public class PlantAssociation
{
    public Guid Id { get; set; }
    public Guid SourcePlantId { get; set; }
    public Guid TargetPlantId { get; set; }
    public AssociationMechanism Mechanism { get; set; }
    public AssociationEffect Effect { get; set; }
    public DistanceEffect DistanceEffect { get; set; }
    public ConfidenceLevel ConfidenceLevel { get; set; }
    public string? Notes { get; set; }
}
