using System.Text.Json;
using System.Text.Json.Serialization;
using GardenAssistant.Data.Entities;
using GardenAssistant.Data.Entities.Enums;
using Microsoft.EntityFrameworkCore;

namespace GardenAssistant.Data.Seeders;

public class PlantActionSeeder(AppDbContext db, IWebHostEnvironment env) : ISeeder
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public async Task SeedAsync()
    {
        if (await db.PlantActions.AnyAsync())
        {
            return;
        }

        var path = Path.Combine(env.ContentRootPath, "Data", "Seeds", "plant-actions.json");
        var json = await File.ReadAllTextAsync(path);
        var records = JsonSerializer.Deserialize<List<PlantActionSeedRecord>>(json, JsonOptions)
            ?? throw new InvalidOperationException("Failed to deserialize plant action seed data.");

        var plantsPath = Path.Combine(env.ContentRootPath, "Data", "Seeds", "plants.json");
        var plantsJson = await File.ReadAllTextAsync(plantsPath);
        var plantRecords = JsonSerializer.Deserialize<List<PlantKeySeedRecord>>(plantsJson, JsonOptions)
            ?? throw new InvalidOperationException("Failed to deserialize plant seed data.");

        var plantNameToId = await db.Plants.ToDictionaryAsync(p => p.Name, p => p.Id);

        foreach (var r in records)
        {
            var plantName = plantRecords.FirstOrDefault(p => p.Key == r.PlantKey)?.Name;
            if (plantName is null || !plantNameToId.TryGetValue(plantName, out var plantId))
            {
                continue;
            }

            foreach (var a in r.Actions)
            {
                db.PlantActions.Add(new PlantAction
                {
                    Id = Guid.NewGuid(),
                    PlantId = plantId,
                    ActionType = a.ActionType,
                    HalfMonthStart = a.HalfMonthStart,
                    HalfMonthEnd = a.HalfMonthEnd,
                    Notes = a.Notes
                });
            }
        }

        await db.SaveChangesAsync();
    }

    private record PlantActionSeedRecord(string PlantKey, List<ActionRecord> Actions);
    private record ActionRecord(PlantActionType ActionType, int HalfMonthStart, int HalfMonthEnd, string? Notes);
    private record PlantKeySeedRecord(string Key, string Name);
}
