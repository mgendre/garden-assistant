using GardenAssistant.Data.Entities.Enums;

namespace GardenAssistant.Data.Entities;

public class PlantAction
{
    public Guid Id { get; set; }
    public Guid PlantId { get; set; }
    public PlantActionType ActionType { get; set; }
    public int HalfMonthStart { get; set; }
    public int HalfMonthEnd { get; set; }
    public string? Notes { get; set; }
    public Plant Plant { get; set; } = null!;
}
