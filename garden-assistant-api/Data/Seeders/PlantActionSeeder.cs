using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using GardenAssistant.Data.Entities;
using GardenAssistant.Data.Entities.Enums;
using GardenAssistant.Data.Seeders.Records;
using Microsoft.EntityFrameworkCore;

namespace GardenAssistant.Data.Seeders;

public class PlantActionSeeder(AppDbContext db, IWebHostEnvironment env, ILogger<PlantActionSeeder> logger) : ISeeder
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public async Task SeedAsync()
    {
        logger.LogInformation("Seeding plant actions...");
        var sw = Stopwatch.StartNew();

        var records = await LoadSeedRecords();
        var lockedPlantIds = await LoadLockedPlantIds();
        var plantsByKey = await LoadPlantsByKey();
        var existingActions = await LoadExistingActionsByPlant();

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
                    logger.LogDebug("Plant actions for \"{Name}\" (key: {Key}) skipped — IsCustomized",
                        plant.Name, record.PlantKey);
                }
                continue;
            }

            UpsertActionsForPlant(plant, record.Actions, existingActions);
        }

        await db.SaveChangesAsync();

        logger.LogInformation("Plant actions seeded in {ElapsedMs}ms", sw.ElapsedMilliseconds);
    }

    private async Task<List<PlantActionSeedRecord>> LoadSeedRecords()
    {
        var path = Path.Combine(env.ContentRootPath, "Data", "Seeds", "plant-actions.json");
        var json = await File.ReadAllTextAsync(path);
        return JsonSerializer.Deserialize<List<PlantActionSeedRecord>>(json, JsonOptions)
            ?? throw new InvalidOperationException("Failed to deserialize plant action seed data.");
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

    private async Task<ILookup<Guid, PlantAction>> LoadExistingActionsByPlant()
    {
        var actions = await db.PlantActions.ToListAsync();
        return actions.ToLookup(a => a.PlantId);
    }

    private void UpsertActionsForPlant(
        Plant plant,
        List<ActionRecord> seedActions,
        ILookup<Guid, PlantAction> existingActions)
    {
        var currentActions = existingActions[plant.Id].ToList();

        foreach (var action in seedActions)
        {
            var existing = currentActions.FirstOrDefault(a =>
                a.ActionType == action.ActionType &&
                a.HalfMonthStart == action.HalfMonthStart &&
                a.HalfMonthEnd == action.HalfMonthEnd);

            if (existing is not null)
            {
                if (existing.Notes != action.Notes)
                {
                    logger.LogInformation("PlantAction for \"{Name}\" ({ActionType} {Start}-{End}) updated — Notes: {Old} → {New}",
                        plant.Name, action.ActionType, action.HalfMonthStart, action.HalfMonthEnd,
                        existing.Notes, action.Notes);
                    existing.Notes = action.Notes;
                }
                continue;
            }

            db.PlantActions.Add(new PlantAction
            {
                Id = Guid.NewGuid(),
                PlantId = plant.Id,
                ActionType = action.ActionType,
                HalfMonthStart = action.HalfMonthStart,
                HalfMonthEnd = action.HalfMonthEnd,
                Notes = action.Notes
            });
        }
    }
}
