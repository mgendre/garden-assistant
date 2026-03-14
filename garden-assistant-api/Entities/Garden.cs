namespace GardenAssistant.Entities;

public class Garden
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
}
