namespace GardenAssistant.Data.Entities;

public class Garden
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public Guid UserId { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
