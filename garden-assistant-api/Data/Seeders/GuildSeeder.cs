using System.Text.Json;
using System.Text.Json.Serialization;
using GardenAssistant.Data.Entities;
using GardenAssistant.Data.Entities.Enums;
using Microsoft.EntityFrameworkCore;

namespace GardenAssistant.Data.Seeders;

public class GuildSeeder(AppDbContext db, IWebHostEnvironment env) : ISeeder
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public async Task SeedAsync()
    {
        if (await db.Guilds.AnyAsync())
        {
            return;
        }

        var plantsPath = Path.Combine(env.ContentRootPath, "Data", "Seeds", "plants.json");
        var plantsJson = await File.ReadAllTextAsync(plantsPath);
        var plantRecords = JsonSerializer.Deserialize<List<PlantKeyRecord>>(plantsJson, JsonOptions)
            ?? throw new InvalidOperationException("Failed to deserialize plant key data.");
        var keyToName = plantRecords.ToDictionary(p => p.Key, p => p.Name);

        var path = Path.Combine(env.ContentRootPath, "Data", "Seeds", "guilds.json");
        var json = await File.ReadAllTextAsync(path);
        var records = JsonSerializer.Deserialize<List<GuildSeedRecord>>(json, JsonOptions)
            ?? throw new InvalidOperationException("Failed to deserialize guild seed data.");

        var plantsByName = await db.Plants
            .ToDictionaryAsync(p => p.Name, p => p.Id);

        foreach (var r in records)
        {
            var guild = new Guild
            {
                Id = Guid.NewGuid(),
                Name = r.Name,
                Description = r.Description
            };
            db.Guilds.Add(guild);

            foreach (var entry in r.Plants)
            {
                if (!keyToName.TryGetValue(entry.Key, out var plantName) ||
                    !plantsByName.TryGetValue(plantName, out var plantId))
                {
                    continue;
                }

                db.GuildPlants.Add(new GuildPlant
                {
                    GuildId = guild.Id,
                    PlantId = plantId,
                    Role = entry.Role ?? GuildPlantRole.Companion
                });
            }
        }

        await db.SaveChangesAsync();
    }

    private record PlantKeyRecord(string Key, string Name);

    private record GuildSeedRecord(
        string Name,
        string? Description,
        List<PlantEntry> Plants
    );

    [JsonConverter(typeof(PlantEntryJsonConverter))]
    private record PlantEntry(string Key, GuildPlantRole? Role);

    private sealed class PlantEntryJsonConverter : JsonConverter<PlantEntry>
    {
        public override PlantEntry Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                var key = reader.GetString()!;
                return new PlantEntry(key, null);
            }

            if (reader.TokenType == JsonTokenType.StartObject)
            {
                string? key = null;
                GuildPlantRole? role = null;

                while (reader.Read())
                {
                    if (reader.TokenType == JsonTokenType.EndObject)
                    {
                        break;
                    }

                    if (reader.TokenType == JsonTokenType.PropertyName)
                    {
                        var propertyName = reader.GetString()!;
                        reader.Read();

                        if (string.Equals(propertyName, "key", StringComparison.OrdinalIgnoreCase))
                        {
                            key = reader.GetString();
                        }
                        else if (string.Equals(propertyName, "role", StringComparison.OrdinalIgnoreCase))
                        {
                            var roleStr = reader.GetString();
                            if (roleStr is not null && Enum.TryParse<GuildPlantRole>(roleStr, true, out var parsed))
                            {
                                role = parsed;
                            }
                        }
                    }
                }

                return new PlantEntry(
                    key ?? throw new JsonException("PlantEntry object must have a 'key' property."),
                    role
                );
            }

            throw new JsonException($"Unexpected token type '{reader.TokenType}' when parsing PlantEntry.");
        }

        public override void Write(Utf8JsonWriter writer, PlantEntry value, JsonSerializerOptions options)
        {
            if (value.Role is null)
            {
                writer.WriteStringValue(value.Key);
            }
            else
            {
                writer.WriteStartObject();
                writer.WriteString("key", value.Key);
                writer.WriteString("role", value.Role.Value.ToString());
                writer.WriteEndObject();
            }
        }
    }
}
