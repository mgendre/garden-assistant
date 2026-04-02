using System.Text.Json;
using System.Text.Json.Serialization;
using GardenAssistant.Data.Entities;
using GardenAssistant.Data.Entities.Enums;
using Microsoft.EntityFrameworkCore;

namespace GardenAssistant.Data.Seeders;

public class PlantSeeder(AppDbContext db, IWebHostEnvironment env) : ISeeder
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public async Task SeedAsync()
    {
        if (await db.Plants.AnyAsync())
        {
            return;
        }

        var path = Path.Combine(env.ContentRootPath, "Data", "Seeds", "plants.json");
        var json = await File.ReadAllTextAsync(path);
        var records = JsonSerializer.Deserialize<List<PlantSeedRecord>>(json, JsonOptions)
            ?? throw new InvalidOperationException("Failed to deserialize plant seed data.");

        var species = records.Where(r => r.ParentKey is null).ToList();
        var varieties = records.Where(r => r.ParentKey is not null).ToList();

        var plantsByKey = new Dictionary<string, Plant>();

        foreach (var r in species)
        {
            var plant = CreatePlantFromRecord(r);
            plantsByKey[r.Key] = plant;
            db.Plants.Add(plant);
            AddIntrinsicMechanisms(plant.Id, r.IntrinsicMechanisms);
        }

        await db.SaveChangesAsync();

        foreach (var r in varieties)
        {
            if (!plantsByKey.TryGetValue(r.ParentKey!, out var parent))
            {
                throw new InvalidOperationException(
                    $"Variety '{r.Key}' references parent '{r.ParentKey}' which does not exist in seed data.");
            }

            var plant = CreatePlantFromRecord(r);
            plant.ParentPlantId = parent.Id;
            plantsByKey[r.Key] = plant;
            db.Plants.Add(plant);
            AddIntrinsicMechanisms(plant.Id, r.IntrinsicMechanisms);
        }

        await db.SaveChangesAsync();
    }

    private static Plant CreatePlantFromRecord(PlantSeedRecord r) => new()
    {
        Id = Guid.NewGuid(),
        Name = r.Name,
        ScientificName = r.ScientificName,
        Description = r.Description,
        Family = r.Family,
        Genus = r.Genus,
        LifeCycle = r.LifeCycle,
        HeightAtMaturityCm = r.HeightAtMaturityCm,
        RootDepth = r.RootDepth,
        SunRequirement = r.SunRequirement,
        WaterNeeds = r.WaterNeeds,
        MaxAltitudeM = r.MaxAltitudeM,
        PropagationMethod = r.PropagationMethod ?? PropagationMethod.Seed,
        FrostSensitive = r.FrostSensitive ?? false
    };

    private void AddIntrinsicMechanisms(Guid plantId, List<AssociationMechanism>? mechanisms)
    {
        foreach (var mechanism in mechanisms ?? [])
        {
            db.PlantIntrinsicMechanisms.Add(new PlantIntrinsicMechanism
            {
                PlantId = plantId,
                Mechanism = mechanism
            });
        }
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
}
