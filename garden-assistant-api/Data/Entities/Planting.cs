using GardenAssistant.Data.Entities.Enums;

namespace GardenAssistant.Data.Entities;

public class Planting
{
    public Guid Id { get; set; }
    public Guid GardenId { get; set; }
    public Guid UserId { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public DateOnly? PlannedDate { get; set; }
    public Guid? GuildId { get; set; }
    public SoilType? SoilType { get; set; }
    public bool HasMulch { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
