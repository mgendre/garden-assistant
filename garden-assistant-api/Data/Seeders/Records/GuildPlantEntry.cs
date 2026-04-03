using System.Text.Json.Serialization;
using GardenAssistant.Data.Entities.Enums;
using GardenAssistant.Data.Seeders.Converters;

namespace GardenAssistant.Data.Seeders.Records;

[JsonConverter(typeof(GuildPlantEntryJsonConverter))]
public record GuildPlantEntry(string Key, GuildPlantRole? Role);
