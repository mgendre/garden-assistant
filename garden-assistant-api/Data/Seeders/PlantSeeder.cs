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

        foreach (var r in records)
        {
            var plant = new Plant
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

            db.Plants.Add(plant);

            foreach (var mechanism in r.IntrinsicMechanisms ?? [])
            {
                db.PlantIntrinsicMechanisms.Add(new PlantIntrinsicMechanism
                {
                    PlantId = plant.Id,
                    Mechanism = mechanism
                });
            }
        }

        await db.SaveChangesAsync();
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
        bool? FrostSensitive
    );
}
