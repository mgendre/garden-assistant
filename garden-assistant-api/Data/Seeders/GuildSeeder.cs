using System.Text.Json;
using GardenAssistant.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace GardenAssistant.Data.Seeders;

public class GuildSeeder(AppDbContext db, IWebHostEnvironment env) : ISeeder
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task SeedAsync()
    {
        if (await db.Guilds.AnyAsync())
        {
            return;
        }

        var plantsPath = Path.Combine(env.ContentRootPath, "Data", "Seeds", "plants.json");
        var plantsJson = await File.ReadAllTextAsync(plantsPath);
        var plantRecords = JsonSerializer.Deserialize<List<PlantKeyRecord>>(plantsJson, JsonOptions)
            ?? throw new InvalidOperationException("Failed to deserialize plant key data.");
        var keyToName = plantRecords.ToDictionary(p => p.Key, p => p.Name);

        var path = Path.Combine(env.ContentRootPath, "Data", "Seeds", "guilds.json");
        var json = await File.ReadAllTextAsync(path);
        var records = JsonSerializer.Deserialize<List<GuildSeedRecord>>(json, JsonOptions)
            ?? throw new InvalidOperationException("Failed to deserialize guild seed data.");

        var plantsByName = await db.Plants
            .ToDictionaryAsync(p => p.Name, p => p.Id);

        foreach (var r in records)
        {
            var guild = new Guild
            {
                Id = Guid.NewGuid(),
                Name = r.Name,
                Description = r.Description
            };
            db.Guilds.Add(guild);

            foreach (var plantKey in r.PlantKeys)
            {
                if (!keyToName.TryGetValue(plantKey, out var plantName) ||
                    !plantsByName.TryGetValue(plantName, out var plantId))
                {
                    continue;
                }

                db.GuildPlants.Add(new GuildPlant
                {
                    GuildId = guild.Id,
                    PlantId = plantId
                });
            }
        }

        await db.SaveChangesAsync();
    }

    private record PlantKeyRecord(string Key, string Name);

    private record GuildSeedRecord(
        string Name,
        string? Description,
        List<string> PlantKeys
    );
}
