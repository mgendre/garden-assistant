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
    public List<PlantIntrinsicMechanism> IntrinsicMechanisms { get; set; } = [];
}
