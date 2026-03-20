namespace GardenAssistant.Data.Entities;

public class HarvestReadiness
{
    public Guid Id { get; set; }
    public Guid PlantId { get; set; }
    public required string Description { get; set; }
    public int? DaysFromTransplant { get; set; }
    public int? DaysFromSowing { get; set; }
    public Plant Plant { get; set; } = null!;
    public List<HarvestReadinessCriterion> Criteria { get; set; } = [];
}
