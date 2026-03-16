namespace GardenAssistant.Data.Entities;

public class UserPlant
{
    public Guid UserId { get; set; }
    public Guid PlantId { get; set; }
    public DateTime AddedAtUtc { get; set; }
}
