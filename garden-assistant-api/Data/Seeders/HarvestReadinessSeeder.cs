using System.Text.Json;
using System.Text.Json.Serialization;
using GardenAssistant.Data.Entities;
using GardenAssistant.Data.Entities.Enums;
using Microsoft.EntityFrameworkCore;

namespace GardenAssistant.Data.Seeders;

public class HarvestReadinessSeeder(AppDbContext db, IWebHostEnvironment env) : ISeeder
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public async Task SeedAsync()
    {
        if (await db.HarvestReadiness.AnyAsync())
        {
            return;
        }

        var path = Path.Combine(env.ContentRootPath, "Data", "Seeds", "harvest-readiness.json");
        var json = await File.ReadAllTextAsync(path);
        var records = JsonSerializer.Deserialize<List<HarvestReadinessSeedRecord>>(json, JsonOptions)
            ?? throw new InvalidOperationException("Failed to deserialize harvest readiness seed data.");

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

            var harvestReadiness = new HarvestReadiness
            {
                Id = Guid.NewGuid(),
                PlantId = plantId,
                Description = r.Description,
                DaysFromTransplant = r.DaysFromTransplant,
                DaysFromSowing = r.DaysFromSowing
            };

            foreach (var c in r.Criteria)
            {
                harvestReadiness.Criteria.Add(new HarvestReadinessCriterion
                {
                    Id = Guid.NewGuid(),
                    CriterionType = c.CriterionType,
                    Description = c.Description
                });
            }

            db.HarvestReadiness.Add(harvestReadiness);
        }

        await db.SaveChangesAsync();
    }

    private record HarvestReadinessSeedRecord(
        string PlantKey,
        string Description,
        int? DaysFromTransplant,
        int? DaysFromSowing,
        List<CriterionRecord> Criteria);

    private record CriterionRecord(HarvestCriterionType CriterionType, string Description);

    private record PlantKeySeedRecord(string Key, string Name);
}
