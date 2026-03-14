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

        var path = Path.Combine(env.ContentRootPath, "Data", "Seeds", "associations.json");
        var json = await File.ReadAllTextAsync(path);
        var records = JsonSerializer.Deserialize<List<AssociationSeedRecord>>(json, JsonOptions)
            ?? throw new InvalidOperationException("Failed to deserialize association seed data.");

        var plantsByName = await db.Plants
            .ToDictionaryAsync(p => p.Name, p => p.Id);

        var associations = new List<PlantAssociation>();

        foreach (var r in records)
        {
            if (!plantsByName.TryGetValue(r.SourcePlantName, out var sourceId) ||
                !plantsByName.TryGetValue(r.TargetPlantName, out var targetId))
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

    private record AssociationSeedRecord(
        string SourcePlantName,
        string TargetPlantName,
        AssociationMechanism Mechanism,
        AssociationEffect Effect,
        DistanceEffect DistanceEffect,
        ConfidenceLevel ConfidenceLevel,
        string? Notes
    );
}
