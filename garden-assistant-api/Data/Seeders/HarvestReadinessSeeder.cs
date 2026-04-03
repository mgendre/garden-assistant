using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using GardenAssistant.Data.Entities;
using GardenAssistant.Data.Entities.Enums;
using GardenAssistant.Data.Seeders.Records;
using Microsoft.EntityFrameworkCore;

namespace GardenAssistant.Data.Seeders;

public class HarvestReadinessSeeder(AppDbContext db, IWebHostEnvironment env, ILogger<HarvestReadinessSeeder> logger) : ISeeder
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public async Task SeedAsync()
    {
        logger.LogInformation("Seeding harvest readiness...");
        var sw = Stopwatch.StartNew();

        var records = await LoadSeedRecords();
        var lockedPlantIds = await LoadLockedPlantIds();
        var plantsByKey = await LoadPlantsByKey();
        var existingReadiness = await LoadExistingReadiness();

        foreach (var record in records)
        {
            if (!plantsByKey.TryGetValue(record.PlantKey, out var plant))
            {
                continue;
            }

            if (lockedPlantIds.Contains(plant.Id))
            {
                if (logger.IsEnabled(LogLevel.Debug))
                {
                    logger.LogDebug("Harvest readiness for \"{Name}\" (key: {Key}) skipped — IsCustomized",
                        plant.Name, record.PlantKey);
                }
                continue;
            }

            UpsertReadiness(plant, record, existingReadiness);
        }

        await db.SaveChangesAsync();

        logger.LogInformation("Harvest readiness seeded in {ElapsedMs}ms", sw.ElapsedMilliseconds);
    }

    private async Task<List<HarvestReadinessSeedRecord>> LoadSeedRecords()
    {
        var path = Path.Combine(env.ContentRootPath, "Data", "Seeds", "harvest-readiness.json");
        var json = await File.ReadAllTextAsync(path);
        return JsonSerializer.Deserialize<List<HarvestReadinessSeedRecord>>(json, JsonOptions)
            ?? throw new InvalidOperationException("Failed to deserialize harvest readiness seed data.");
    }

    private async Task<HashSet<Guid>> LoadLockedPlantIds()
    {
        var ids = await db.Plants
            .Where(p => p.IsCustomized)
            .Select(p => p.Id)
            .ToListAsync();
        return ids.ToHashSet();
    }

    private async Task<Dictionary<string, Plant>> LoadPlantsByKey()
    {
        return await db.Plants.ToDictionaryAsync(p => p.Key, p => p);
    }

    private async Task<Dictionary<Guid, HarvestReadiness>> LoadExistingReadiness()
    {
        return await db.HarvestReadiness
            .Include(h => h.Criteria)
            .ToDictionaryAsync(h => h.PlantId, h => h);
    }

    private void UpsertReadiness(
        Plant plant,
        HarvestReadinessSeedRecord record,
        Dictionary<Guid, HarvestReadiness> existingReadiness)
    {
        if (existingReadiness.TryGetValue(plant.Id, out var existing))
        {
            var changes = DetectReadinessFieldChanges(existing, record);
            var criteriaChanges = ReplaceCriteria(existing, record.Criteria);
            changes.AddRange(criteriaChanges);

            if (changes.Count > 0)
            {
                logger.LogInformation("HarvestReadiness for \"{Name}\" updated — {Changes}",
                    plant.Name, string.Join(", ", changes));
            }
            return;
        }

        var harvestReadiness = new HarvestReadiness
        {
            Id = Guid.NewGuid(),
            PlantId = plant.Id,
            Description = record.Description,
            DaysFromTransplant = record.DaysFromTransplant,
            DaysFromSowing = record.DaysFromSowing
        };

        foreach (var criterion in record.Criteria)
        {
            harvestReadiness.Criteria.Add(new HarvestReadinessCriterion
            {
                Id = Guid.NewGuid(),
                CriterionType = criterion.CriterionType,
                Description = criterion.Description
            });
        }

        db.HarvestReadiness.Add(harvestReadiness);
        existingReadiness[plant.Id] = harvestReadiness;
    }

    private static List<string> DetectReadinessFieldChanges(HarvestReadiness existing, HarvestReadinessSeedRecord record)
    {
        var changes = new List<string>();

        if (existing.Description != record.Description)
        {
            changes.Add($"Description: {existing.Description} → {record.Description}");
            existing.Description = record.Description;
        }

        if (existing.DaysFromTransplant != record.DaysFromTransplant)
        {
            changes.Add($"DaysFromTransplant: {existing.DaysFromTransplant} → {record.DaysFromTransplant}");
            existing.DaysFromTransplant = record.DaysFromTransplant;
        }

        if (existing.DaysFromSowing != record.DaysFromSowing)
        {
            changes.Add($"DaysFromSowing: {existing.DaysFromSowing} → {record.DaysFromSowing}");
            existing.DaysFromSowing = record.DaysFromSowing;
        }

        return changes;
    }

    private List<string> ReplaceCriteria(HarvestReadiness existing, List<CriterionRecord> seedCriteria)
    {
        var changes = new List<string>();
        var existingByType = existing.Criteria.ToDictionary(c => c.CriterionType, c => c);
        var desiredTypes = seedCriteria.Select(c => c.CriterionType).ToHashSet();

        foreach (var seed in seedCriteria)
        {
            if (existingByType.TryGetValue(seed.CriterionType, out var match))
            {
                if (match.Description != seed.Description)
                {
                    changes.Add($"Criterion {seed.CriterionType}: {match.Description} → {seed.Description}");
                    match.Description = seed.Description;
                }
            }
            else
            {
                changes.Add($"Criterion added: {seed.CriterionType}");
                db.HarvestReadinessCriteria.Add(new HarvestReadinessCriterion
                {
                    Id = Guid.NewGuid(),
                    HarvestReadinessId = existing.Id,
                    CriterionType = seed.CriterionType,
                    Description = seed.Description
                });
            }
        }

        foreach (var type in existingByType.Keys.Except(desiredTypes))
        {
            changes.Add($"Criterion removed: {type}");
            db.HarvestReadinessCriteria.Remove(existingByType[type]);
        }

        return changes;
    }
}
