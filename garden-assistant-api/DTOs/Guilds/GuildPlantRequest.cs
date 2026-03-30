using GardenAssistant.Data.Entities.Enums;

namespace GardenAssistant.DTOs.Guilds;

public record GuildPlantRequest(Guid PlantId, GuildPlantRole? Role = null);
