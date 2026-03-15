using GardenAssistant.Data.Entities.Enums;

namespace GardenAssistant.Data.Entities;

public class PlantingEntry
{
    public Guid Id { get; set; }
    public Guid PlantingId { get; set; }
    public Guid PlantId { get; set; }
    public int? Quantity { get; set; }
    public float? PositionX { get; set; }
    public float? PositionY { get; set; }
    public PlantingLayer? Layer { get; set; }
    public DateOnly? PlannedSowDate { get; set; }
    public DateOnly? PlannedHarvestDate { get; set; }
    public DateOnly? ActualHarvestDate { get; set; }
    public string? Notes { get; set; }
}
