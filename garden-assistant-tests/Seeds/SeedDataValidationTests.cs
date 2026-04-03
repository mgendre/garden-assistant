using System.Text.Json;
using System.Text.Json.Serialization;
using GardenAssistant.Data.Entities.Enums;
using GardenAssistant.Data.Seeders.Records;
using Shouldly;

namespace GardenAssistant.Tests.Seeds;

public class SeedDataValidationTests
{
    private static readonly string SeedsPath = Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "garden-assistant-api", "Data", "Seeds");

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
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
}
