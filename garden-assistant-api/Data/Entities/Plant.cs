using GardenAssistant.Data.Entities.Enums;

namespace GardenAssistant.Data.Entities;

public class Plant
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? ScientificName { get; set; }
    public string? Description { get; set; }
    public string? Family { get; set; }
    public string? Genus { get; set; }
    public LifeCycle LifeCycle { get; set; }
    public int? HeightAtMaturityCm { get; set; }
    public RootDepth RootDepth { get; set; }
    public SunRequirement SunRequirement { get; set; }
    public WaterNeeds WaterNeeds { get; set; }
    public int? MaxAltitudeM { get; set; }
    public PropagationMethod PropagationMethod { get; set; }
    public bool FrostSensitive { get; set; }
    public List<PlantIntrinsicMechanism> IntrinsicMechanisms { get; set; } = [];
    public List<PlantAction> Actions { get; set; } = [];
    public HarvestReadiness? HarvestReadiness { get; set; }
    public Guid? ParentPlantId { get; set; }
    public Plant? ParentPlant { get; set; }
    public List<Plant> Varieties { get; set; } = [];
}
