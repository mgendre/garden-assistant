using GardenAssistant.Data.Entities.Enums;

namespace GardenAssistant.DTOs.Guilds;

public record GuildPlantMemberDto(Guid Id, string Name, string? ScientificName, GuildPlantRole Role);
