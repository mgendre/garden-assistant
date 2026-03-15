using System.Text.Json;
using System.Text.Json.Serialization;
using GardenAssistant.Data.Entities;
using GardenAssistant.Data.Entities.Enums;
using Microsoft.EntityFrameworkCore;

namespace GardenAssistant.Data;

public class AssociationSeeder(AppDbContext db, IWebHostEnvironment env)
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public async Task SeedAsync()
    {
        if (await db.PlantAssociations.AnyAsync()) return;

        var plantsPath = Path.Combine(env.ContentRootPath, "Data", "Seeds", "plants.json");
        var plantsJson = await File.ReadAllTextAsync(plantsPath);
        var plantRecords = JsonSerializer.Deserialize<List<PlantKeyRecord>>(plantsJson, JsonOptions)
            ?? throw new InvalidOperationException("Failed to deserialize plant key data.");
        var keyToName = plantRecords.ToDictionary(p => p.Key, p => p.Name);

        var path = Path.Combine(env.ContentRootPath, "Data", "Seeds", "associations.json");
        var json = await File.ReadAllTextAsync(path);
        var records = JsonSerializer.Deserialize<List<AssociationSeedRecord>>(json, JsonOptions)
            ?? throw new InvalidOperationException("Failed to deserialize association seed data.");

        var plantsByName = await db.Plants
            .ToDictionaryAsync(p => p.Name, p => p.Id);

        var associations = new List<PlantAssociation>();

        foreach (var r in records)
        {
            if (!keyToName.TryGetValue(r.SourcePlantKey, out var sourceName) ||
                !keyToName.TryGetValue(r.TargetPlantKey, out var targetName) ||
                !plantsByName.TryGetValue(sourceName, out var sourceId) ||
                !plantsByName.TryGetValue(targetName, out var targetId))
                continue;

            associations.Add(new PlantAssociation
            {
                Id = Guid.NewGuid(),
                SourcePlantId = sourceId,
                TargetPlantId = targetId,
                Mechanism = r.Mechanism,
                Effect = r.Effect,
                DistanceEffect = r.DistanceEffect,
                ConfidenceLevel = r.ConfidenceLevel,
                Notes = r.Notes
            });
        }

        db.PlantAssociations.AddRange(associations);
        await db.SaveChangesAsync();
    }

    private record PlantKeyRecord(string Key, string Name);

    private record AssociationSeedRecord(
        string SourcePlantKey,
        string TargetPlantKey,
        AssociationMechanism Mechanism,
        AssociationEffect Effect,
        DistanceEffect DistanceEffect,
        ConfidenceLevel ConfidenceLevel,
        string? Notes
    );
}
