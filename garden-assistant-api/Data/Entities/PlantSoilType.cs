using GardenAssistant.Data.Entities.Enums;

namespace GardenAssistant.Data.Entities;

public class PlantSoilType
{
    public Guid PlantId { get; set; }
    public SoilType SoilType { get; set; }
}
