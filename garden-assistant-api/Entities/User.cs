namespace GardenAssistant.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;

    public ICollection<Garden> Gardens { get; set; } = [];
}
