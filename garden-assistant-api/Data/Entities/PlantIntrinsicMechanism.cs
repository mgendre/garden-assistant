using GardenAssistant.Data.Entities.Enums;

namespace GardenAssistant.Data.Entities;

public class PlantIntrinsicMechanism
{
    public Guid PlantId { get; set; }
    public AssociationMechanism Mechanism { get; set; }
}
