using GardenAssistant.Data.Entities.Enums;

namespace GardenAssistant.Data.Entities;

public class GuildPlant
{
    public Guid GuildId { get; set; }
    public Guid PlantId { get; set; }
    public GuildPlantRole Role { get; set; } = GuildPlantRole.Companion;
}
