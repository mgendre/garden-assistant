using GardenAssistant.Data.Entities.Enums;

namespace GardenAssistant.Data.Entities;

public class HarvestReadinessCriterion
{
    public Guid Id { get; set; }
    public Guid HarvestReadinessId { get; set; }
    public HarvestCriterionType CriterionType { get; set; }
    public required string Description { get; set; }
    public HarvestReadiness HarvestReadiness { get; set; } = null!;
}
