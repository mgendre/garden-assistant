using System.Text.Json;
using System.Text.Json.Serialization;
using GardenAssistant.Data.Entities.Enums;
using Shouldly;

namespace GardenAssistant.Tests.Seeds;

public class SeedDataValidationTests
{
    private static readonly string SeedsPath = Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "garden-assistant-api", "Data", "Seeds");

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter(), new GuildPlantEntryConverter() }
    };

    [Fact]
    public void PlantsJson_ShouldDeserializeWithoutErrors()
    {
        var json = File.ReadAllText(Path.Combine(SeedsPath, "plants.json"));

        var plants = JsonSerializer.Deserialize<List<PlantSeedRecord>>(json, JsonOptions);

        plants.ShouldNotBeNull();
        plants.Count.ShouldBeGreaterThan(0);
    }

    [Fact]
    public void PlantsJson_AllKeysShouldBeUnique()
    {
        var plants = LoadPlants();

        var duplicates = plants
            .GroupBy(p => p.Key)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        duplicates.ShouldBeEmpty($"Duplicate keys found: {string.Join(", ", duplicates)}");
    }

    [Fact]
    public void PlantsJson_AllParentKeysShouldReferenceExistingPlants()
    {
        var plants = LoadPlants();
        var keys = plants.Select(p => p.Key).ToHashSet();

        var broken = plants
            .Where(p => p.ParentKey is not null && !keys.Contains(p.ParentKey))
            .Select(p => $"{p.Key} -> {p.ParentKey}")
            .ToList();

        broken.ShouldBeEmpty($"Broken parentKey references: {string.Join(", ", broken)}");
    }

    [Fact]
    public void AssociationsJson_ShouldDeserializeWithoutErrors()
    {
        var json = File.ReadAllText(Path.Combine(SeedsPath, "associations.json"));

        var associations = JsonSerializer.Deserialize<List<AssociationSeedRecord>>(json, JsonOptions);

        associations.ShouldNotBeNull();
        associations.Count.ShouldBeGreaterThan(0);
    }

    [Fact]
    public void AssociationsJson_AllPlantKeysShouldExistInPlantsJson()
    {
        var plantKeys = LoadPlants().Select(p => p.Key).ToHashSet();
        var associations = LoadAssociations();

        var missingSource = associations
            .Where(a => !plantKeys.Contains(a.SourcePlantKey))
            .Select(a => a.SourcePlantKey)
            .Distinct()
            .ToList();

        var missingTarget = associations
            .Where(a => !plantKeys.Contains(a.TargetPlantKey))
            .Select(a => a.TargetPlantKey)
            .Distinct()
            .ToList();

        var missing = missingSource.Concat(missingTarget).Distinct().ToList();
        missing.ShouldBeEmpty($"Association references to non-existent plants: {string.Join(", ", missing)}");
    }

    [Fact]
    public void GuildsJson_ShouldDeserializeWithoutErrors()
    {
        var json = File.ReadAllText(Path.Combine(SeedsPath, "guilds.json"));

        var guilds = JsonSerializer.Deserialize<List<GuildSeedRecord>>(json, JsonOptions);

        guilds.ShouldNotBeNull();
        guilds.Count.ShouldBeGreaterThan(0);
    }

    [Fact]
    public void GuildsJson_AllPlantKeysShouldExistInPlantsJson()
    {
        var plantKeys = LoadPlants().Select(p => p.Key).ToHashSet();
        var guilds = LoadGuilds();

        var missing = guilds
            .SelectMany(g => g.Plants)
            .Select(p => p.Key)
            .Where(k => !plantKeys.Contains(k))
            .Distinct()
            .ToList();

        missing.ShouldBeEmpty($"Guild references to non-existent plants: {string.Join(", ", missing)}");
    }

    private List<PlantSeedRecord> LoadPlants()
    {
        var json = File.ReadAllText(Path.Combine(SeedsPath, "plants.json"));
        return JsonSerializer.Deserialize<List<PlantSeedRecord>>(json, JsonOptions)!;
    }

    private List<AssociationSeedRecord> LoadAssociations()
    {
        var json = File.ReadAllText(Path.Combine(SeedsPath, "associations.json"));
        return JsonSerializer.Deserialize<List<AssociationSeedRecord>>(json, JsonOptions)!;
    }

    private List<GuildSeedRecord> LoadGuilds()
    {
        var json = File.ReadAllText(Path.Combine(SeedsPath, "guilds.json"));
        return JsonSerializer.Deserialize<List<GuildSeedRecord>>(json, JsonOptions)!;
    }

    private record PlantSeedRecord(
        string Key,
        string Name,
        string? ScientificName,
        string? Description,
        string? Family,
        string? Genus,
        LifeCycle LifeCycle,
        int? HeightAtMaturityCm,
        RootDepth RootDepth,
        SunRequirement SunRequirement,
        WaterNeeds WaterNeeds,
        int? MaxAltitudeM,
        List<AssociationMechanism>? IntrinsicMechanisms,
        PropagationMethod? PropagationMethod,
        bool? FrostSensitive,
        string? ParentKey
    );

    private record AssociationSeedRecord(
        string SourcePlantKey,
        string TargetPlantKey,
        AssociationMechanism Mechanism,
        AssociationEffect Effect,
        DistanceEffect DistanceEffect,
        ConfidenceLevel ConfidenceLevel,
        string? Notes
    );

    private record GuildSeedRecord(
        string Name,
        string? Description,
        List<GuildPlantEntry> Plants
    );

    [JsonConverter(typeof(GuildPlantEntryConverter))]
    private record GuildPlantEntry(string Key, GuildPlantRole? Role);

    private sealed class GuildPlantEntryConverter : JsonConverter<GuildPlantEntry>
    {
        public override GuildPlantEntry Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                return new GuildPlantEntry(reader.GetString()!, null);
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

                return new GuildPlantEntry(key!, role);
            }

            throw new JsonException($"Unexpected token {reader.TokenType}");
        }

        public override void Write(Utf8JsonWriter writer, GuildPlantEntry value, JsonSerializerOptions options)
        {
            throw new NotSupportedException();
        }
    }
}
