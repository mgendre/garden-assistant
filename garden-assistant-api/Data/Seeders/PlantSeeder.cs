using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using GardenAssistant.Data.Entities;
using GardenAssistant.Data.Entities.Enums;
using GardenAssistant.Data.Seeders.Records;
using Microsoft.EntityFrameworkCore;

namespace GardenAssistant.Data.Seeders;

public class PlantSeeder(AppDbContext db, IWebHostEnvironment env, ILogger<PlantSeeder> logger) : ISeeder
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public async Task SeedAsync()
    {
        logger.LogInformation("Seeding plants...");
        var sw = Stopwatch.StartNew();

        var records = await LoadSeedRecords();
        var existingPlants = await LoadExistingPlantsWithMechanisms();
        var plantsByKey = new Dictionary<string, Plant>(existingPlants);

        UpsertSpecies(records, existingPlants, plantsByKey);
        await db.SaveChangesAsync();

        UpsertVarieties(records, existingPlants, plantsByKey);
        await db.SaveChangesAsync();

        logger.LogInformation("Plants seeded in {ElapsedMs}ms", sw.ElapsedMilliseconds);
    }

    private async Task<List<PlantSeedRecord>> LoadSeedRecords()
    {
        var path = Path.Combine(env.ContentRootPath, "Data", "Seeds", "plants.json");
        var json = await File.ReadAllTextAsync(path);
        return JsonSerializer.Deserialize<List<PlantSeedRecord>>(json, JsonOptions)
            ?? throw new InvalidOperationException("Failed to deserialize plant seed data.");
    }

    private async Task<Dictionary<string, Plant>> LoadExistingPlantsWithMechanisms()
    {
        return await db.Plants
            .Include(p => p.IntrinsicMechanisms)
            .Include(p => p.SoilTypes)
            .Where(p => p.UserId == null)
            .AsSplitQuery()
            .ToDictionaryAsync(p => p.Key, p => p);
    }

    private void UpsertSpecies(
        List<PlantSeedRecord> records,
        Dictionary<string, Plant> existingPlants,
        Dictionary<string, Plant> plantsByKey)
    {
        var species = records.Where(r => r.ParentKey is null);
        foreach (var record in species)
        {
            UpsertPlant(record, null, existingPlants, plantsByKey);
        }
    }

    private void UpsertVarieties(
        List<PlantSeedRecord> records,
        Dictionary<string, Plant> existingPlants,
        Dictionary<string, Plant> plantsByKey)
    {
        var varieties = records.Where(r => r.ParentKey is not null);
        foreach (var record in varieties)
        {
            if (!plantsByKey.TryGetValue(record.ParentKey!, out var parent))
            {
                throw new InvalidOperationException(
                    $"Variety '{record.Key}' references parent '{record.ParentKey}' which does not exist in seed data.");
            }

            UpsertPlant(record, parent.Id, existingPlants, plantsByKey);
        }
    }

    private void UpsertPlant(
        PlantSeedRecord record,
        Guid? parentPlantId,
        Dictionary<string, Plant> existingPlants,
        Dictionary<string, Plant> plantsByKey)
    {
        if (existingPlants.TryGetValue(record.Key, out var existing))
        {
            if (existing.IsCustomized)
            {
                if (logger.IsEnabled(LogLevel.Debug))
                {
                    logger.LogDebug("Plant \"{Name}\" (key: {Key}) skipped — IsCustomized", existing.Name, record.Key);
                }
                return;
            }

            var changes = DetectPlantFieldChanges(existing, record, parentPlantId);
            UpsertMechanisms(existing, record.IntrinsicMechanisms);
            UpsertSoilTypes(existing, record.SoilTypes);

            if (changes.Count > 0)
            {
                logger.LogInformation("Plant \"{Name}\" (key: {Key}) updated — {Changes}",
                    existing.Name, record.Key, string.Join(", ", changes));
            }

            return;
        }

        var plant = CreatePlantFromRecord(record, parentPlantId);
        plantsByKey[record.Key] = plant;
        db.Plants.Add(plant);
        AddIntrinsicMechanisms(plant.Id, record.IntrinsicMechanisms);
        AddSoilTypes(plant.Id, record.SoilTypes);
    }

    private static List<string> DetectPlantFieldChanges(Plant plant, PlantSeedRecord record, Guid? parentPlantId)
    {
        var changes = new List<string>();

        if (plant.Name != record.Name)
        {
            changes.Add($"Name: {plant.Name} → {record.Name}");
            plant.Name = record.Name;
        }

        if (plant.ScientificName != record.ScientificName)
        {
            changes.Add($"ScientificName: {plant.ScientificName} → {record.ScientificName}");
            plant.ScientificName = record.ScientificName;
        }

        if (plant.Description != record.Description)
        {
            changes.Add($"Description: {plant.Description} → {record.Description}");
            plant.Description = record.Description;
        }

        if (plant.Family != record.Family)
        {
            changes.Add($"Family: {plant.Family} → {record.Family}");
            plant.Family = record.Family;
        }

        if (plant.Genus != record.Genus)
        {
            changes.Add($"Genus: {plant.Genus} → {record.Genus}");
            plant.Genus = record.Genus;
        }

        if (plant.LifeCycle != record.LifeCycle)
        {
            changes.Add($"LifeCycle: {plant.LifeCycle} → {record.LifeCycle}");
            plant.LifeCycle = record.LifeCycle;
        }

        if (plant.HeightAtMaturityCm != record.HeightAtMaturityCm)
        {
            changes.Add($"HeightAtMaturityCm: {plant.HeightAtMaturityCm} → {record.HeightAtMaturityCm}");
            plant.HeightAtMaturityCm = record.HeightAtMaturityCm;
        }

        if (plant.RootDepth != record.RootDepth)
        {
            changes.Add($"RootDepth: {plant.RootDepth} → {record.RootDepth}");
            plant.RootDepth = record.RootDepth;
        }

        if (plant.SunRequirement != record.SunRequirement)
        {
            changes.Add($"SunRequirement: {plant.SunRequirement} → {record.SunRequirement}");
            plant.SunRequirement = record.SunRequirement;
        }

        if (plant.WaterNeeds != record.WaterNeeds)
        {
            changes.Add($"WaterNeeds: {plant.WaterNeeds} → {record.WaterNeeds}");
            plant.WaterNeeds = record.WaterNeeds;
        }

        if (plant.MaxAltitudeM != record.MaxAltitudeM)
        {
            changes.Add($"MaxAltitudeM: {plant.MaxAltitudeM} → {record.MaxAltitudeM}");
            plant.MaxAltitudeM = record.MaxAltitudeM;
        }

        var seedPropagation = record.PropagationMethod ?? PropagationMethod.Seed;
        if (plant.PropagationMethod != seedPropagation)
        {
            changes.Add($"PropagationMethod: {plant.PropagationMethod} → {seedPropagation}");
            plant.PropagationMethod = seedPropagation;
        }

        var seedFrostSensitive = record.FrostSensitive ?? false;
        if (plant.FrostSensitive != seedFrostSensitive)
        {
            changes.Add($"FrostSensitive: {plant.FrostSensitive} → {seedFrostSensitive}");
            plant.FrostSensitive = seedFrostSensitive;
        }

        if (plant.OptimalPhMin != record.OptimalPhMin)
        {
            changes.Add($"OptimalPhMin: {plant.OptimalPhMin} → {record.OptimalPhMin}");
            plant.OptimalPhMin = record.OptimalPhMin;
        }

        if (plant.OptimalPhMax != record.OptimalPhMax)
        {
            changes.Add($"OptimalPhMax: {plant.OptimalPhMax} → {record.OptimalPhMax}");
            plant.OptimalPhMax = record.OptimalPhMax;
        }

        if (plant.ParentPlantId != parentPlantId)
        {
            changes.Add($"ParentPlantId: {plant.ParentPlantId} → {parentPlantId}");
            plant.ParentPlantId = parentPlantId;
        }

        if (plant.WaterAmountMl != record.WaterAmountMl)
        {
            changes.Add($"WaterAmountMl: {plant.WaterAmountMl} → {record.WaterAmountMl}");
            plant.WaterAmountMl = record.WaterAmountMl;
        }

        return changes;
    }

    private void UpsertMechanisms(Plant plant, List<AssociationMechanism>? seedMechanisms)
    {
        var desired = seedMechanisms ?? [];
        var current = plant.IntrinsicMechanisms.Select(m => m.Mechanism).ToHashSet();
        var desiredSet = desired.ToHashSet();

        var toRemove = plant.IntrinsicMechanisms.Where(m => !desiredSet.Contains(m.Mechanism)).ToList();
        foreach (var mechanism in toRemove)
        {
            db.PlantIntrinsicMechanisms.Remove(mechanism);
        }

        var toAdd = desired.Where(m => !current.Contains(m)).ToList();
        foreach (var mechanism in toAdd)
        {
            db.PlantIntrinsicMechanisms.Add(new PlantIntrinsicMechanism
            {
                PlantId = plant.Id,
                Mechanism = mechanism
            });
        }
    }

    private static Plant CreatePlantFromRecord(PlantSeedRecord record, Guid? parentPlantId) => new()
    {
        Id = Guid.NewGuid(),
        Key = record.Key,
        Name = record.Name,
        ScientificName = record.ScientificName,
        Description = record.Description,
        Family = record.Family,
        Genus = record.Genus,
        LifeCycle = record.LifeCycle,
        HeightAtMaturityCm = record.HeightAtMaturityCm,
        RootDepth = record.RootDepth,
        SunRequirement = record.SunRequirement,
        WaterNeeds = record.WaterNeeds,
        MaxAltitudeM = record.MaxAltitudeM,
        PropagationMethod = record.PropagationMethod ?? PropagationMethod.Seed,
        FrostSensitive = record.FrostSensitive ?? false,
        OptimalPhMin = record.OptimalPhMin,
        OptimalPhMax = record.OptimalPhMax,
        ParentPlantId = parentPlantId,
        WaterAmountMl = record.WaterAmountMl
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

    private void UpsertSoilTypes(Plant plant, List<SoilType>? seedSoilTypes)
    {
        var desired = seedSoilTypes ?? [];
        var current = plant.SoilTypes.Select(st => st.SoilType).ToHashSet();
        var desiredSet = desired.ToHashSet();

        var toRemove = plant.SoilTypes.Where(st => !desiredSet.Contains(st.SoilType)).ToList();
        foreach (var soilType in toRemove)
        {
            db.PlantSoilTypes.Remove(soilType);
        }

        var toAdd = desired.Where(st => !current.Contains(st)).ToList();
        foreach (var soilType in toAdd)
        {
            db.PlantSoilTypes.Add(new PlantSoilType
            {
                PlantId = plant.Id,
                SoilType = soilType
            });
        }
    }

    private void AddSoilTypes(Guid plantId, List<SoilType>? soilTypes)
    {
        foreach (var soilType in soilTypes ?? [])
        {
            db.PlantSoilTypes.Add(new PlantSoilType
            {
                PlantId = plantId,
                SoilType = soilType
            });
        }
    }
}
