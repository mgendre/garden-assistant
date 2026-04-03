using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using GardenAssistant.Data.Entities;
using GardenAssistant.Data.Entities.Enums;
using GardenAssistant.Data.Seeders.Records;
using Microsoft.EntityFrameworkCore;

namespace GardenAssistant.Data.Seeders;

public class AssociationSeeder(AppDbContext db, IWebHostEnvironment env, ILogger<AssociationSeeder> logger) : ISeeder
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public async Task SeedAsync()
    {
        logger.LogInformation("Seeding associations...");
        var sw = Stopwatch.StartNew();

        var records = await LoadSeedRecords();
        var lockedPlantIds = await LoadLockedPlantIds();
        var plantsByKey = await LoadPlantsByKey();
        var existingLookup = await LoadExistingAssociations();

        foreach (var record in records)
        {
            if (!plantsByKey.TryGetValue(record.SourcePlantKey, out var source) ||
                !plantsByKey.TryGetValue(record.TargetPlantKey, out var target))
            {
                continue;
            }

            if (lockedPlantIds.Contains(source.Id) || lockedPlantIds.Contains(target.Id))
            {
                if (logger.IsEnabled(LogLevel.Debug))
                {
                    logger.LogDebug("Association {Source} → {Target} skipped — plant is customized",
                        record.SourcePlantKey, record.TargetPlantKey);
                }
                continue;
            }

            UpsertAssociation(record, source, target, existingLookup);
        }

        await db.SaveChangesAsync();

        logger.LogInformation("Associations seeded in {ElapsedMs}ms", sw.ElapsedMilliseconds);
    }

    private async Task<List<AssociationSeedRecord>> LoadSeedRecords()
    {
        var path = Path.Combine(env.ContentRootPath, "Data", "Seeds", "associations.json");
        var json = await File.ReadAllTextAsync(path);
        return JsonSerializer.Deserialize<List<AssociationSeedRecord>>(json, JsonOptions)
            ?? throw new InvalidOperationException("Failed to deserialize association seed data.");
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

    private async Task<Dictionary<(Guid, Guid, AssociationMechanism), PlantAssociation>> LoadExistingAssociations()
    {
        var associations = await db.PlantAssociations.ToListAsync();
        return associations.ToDictionary(a => (a.SourcePlantId, a.TargetPlantId, a.Mechanism), a => a);
    }

    private void UpsertAssociation(
        AssociationSeedRecord record,
        Plant source,
        Plant target,
        Dictionary<(Guid, Guid, AssociationMechanism), PlantAssociation> existingLookup)
    {
        var key = (source.Id, target.Id, record.Mechanism);

        if (existingLookup.TryGetValue(key, out var existing))
        {
            var changes = DetectAssociationFieldChanges(existing, record);
            if (changes.Count > 0)
            {
                logger.LogInformation("Association {Source} → {Target} ({Mechanism}) updated — {Changes}",
                    record.SourcePlantKey, record.TargetPlantKey, record.Mechanism, string.Join(", ", changes));
            }
            return;
        }

        var association = new PlantAssociation
        {
            Id = Guid.NewGuid(),
            SourcePlantId = source.Id,
            TargetPlantId = target.Id,
            Mechanism = record.Mechanism,
            Effect = record.Effect,
            DistanceEffect = record.DistanceEffect,
            ConfidenceLevel = record.ConfidenceLevel,
            Notes = record.Notes
        };

        db.PlantAssociations.Add(association);
        existingLookup[key] = association;
    }

    private static List<string> DetectAssociationFieldChanges(PlantAssociation existing, AssociationSeedRecord record)
    {
        var changes = new List<string>();

        if (existing.Effect != record.Effect)
        {
            changes.Add($"Effect: {existing.Effect} → {record.Effect}");
            existing.Effect = record.Effect;
        }

        if (existing.DistanceEffect != record.DistanceEffect)
        {
            changes.Add($"DistanceEffect: {existing.DistanceEffect} → {record.DistanceEffect}");
            existing.DistanceEffect = record.DistanceEffect;
        }

        if (existing.ConfidenceLevel != record.ConfidenceLevel)
        {
            changes.Add($"ConfidenceLevel: {existing.ConfidenceLevel} → {record.ConfidenceLevel}");
            existing.ConfidenceLevel = record.ConfidenceLevel;
        }

        if (existing.Notes != record.Notes)
        {
            changes.Add($"Notes: {existing.Notes} → {record.Notes}");
            existing.Notes = record.Notes;
        }

        return changes;
    }
}
