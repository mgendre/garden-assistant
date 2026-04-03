using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using GardenAssistant.Data.Entities;
using GardenAssistant.Data.Entities.Enums;
using GardenAssistant.Data.Seeders.Records;
using Microsoft.EntityFrameworkCore;

namespace GardenAssistant.Data.Seeders;

public class GuildSeeder(AppDbContext db, IWebHostEnvironment env, ILogger<GuildSeeder> logger) : ISeeder
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public async Task SeedAsync()
    {
        logger.LogInformation("Seeding guilds...");
        var sw = Stopwatch.StartNew();

        var records = await LoadSeedRecords();
        var lockedPlantIds = await LoadLockedPlantIds();
        var plantsByKey = await LoadPlantsByKey();
        var existingGuilds = await LoadExistingGuilds();
        var existingGuildPlants = await LoadExistingGuildPlants();

        foreach (var record in records)
        {
            var guild = UpsertGuild(record, existingGuilds);
            var currentLinks = existingGuildPlants[guild.Id].ToList();
            UpsertGuildPlantLinks(guild, record.Plants, plantsByKey, lockedPlantIds, currentLinks);
        }

        await db.SaveChangesAsync();

        logger.LogInformation("Guilds seeded in {ElapsedMs}ms", sw.ElapsedMilliseconds);
    }

    private async Task<List<GuildSeedRecord>> LoadSeedRecords()
    {
        var path = Path.Combine(env.ContentRootPath, "Data", "Seeds", "guilds.json");
        var json = await File.ReadAllTextAsync(path);
        return JsonSerializer.Deserialize<List<GuildSeedRecord>>(json, JsonOptions)
            ?? throw new InvalidOperationException("Failed to deserialize guild seed data.");
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

    private async Task<Dictionary<string, Guild>> LoadExistingGuilds()
    {
        var guilds = await db.Guilds.Where(g => g.UserId == null).ToListAsync();
        var result = new Dictionary<string, Guild>();
        foreach (var guild in guilds)
        {
            result.TryAdd(guild.Name, guild);
        }
        return result;
    }

    private async Task<ILookup<Guid, GuildPlant>> LoadExistingGuildPlants()
    {
        var guildPlants = await db.GuildPlants.ToListAsync();
        return guildPlants.ToLookup(gp => gp.GuildId);
    }

    private Guild UpsertGuild(GuildSeedRecord record, Dictionary<string, Guild> existingGuilds)
    {
        if (existingGuilds.TryGetValue(record.Name, out var existing))
        {
            if (existing.Description != record.Description)
            {
                logger.LogInformation("Guild \"{Name}\" updated — Description: {Old} → {New}",
                    record.Name, existing.Description, record.Description);
                existing.Description = record.Description;
            }
            return existing;
        }

        var guild = new Guild
        {
            Id = Guid.NewGuid(),
            Name = record.Name,
            Description = record.Description
        };
        db.Guilds.Add(guild);
        existingGuilds[record.Name] = guild;
        return guild;
    }

    private void UpsertGuildPlantLinks(
        Guild guild,
        List<GuildPlantEntry> plantEntries,
        Dictionary<string, Plant> plantsByKey,
        HashSet<Guid> lockedPlantIds,
        List<GuildPlant> currentLinks)
    {
        foreach (var entry in plantEntries)
        {
            if (!plantsByKey.TryGetValue(entry.Key, out var plant))
            {
                continue;
            }

            if (lockedPlantIds.Contains(plant.Id))
            {
                if (logger.IsEnabled(LogLevel.Debug))
                {
                    logger.LogDebug("GuildPlant link {Guild} → {Plant} skipped — plant is customized",
                        guild.Name, entry.Key);
                }
                continue;
            }

            var role = entry.Role ?? GuildPlantRole.Companion;
            var existingLink = currentLinks.FirstOrDefault(l => l.PlantId == plant.Id);

            if (existingLink is not null)
            {
                if (existingLink.Role != role)
                {
                    logger.LogInformation("GuildPlant link {Guild} → {Plant} updated — Role: {Old} → {New}",
                        guild.Name, entry.Key, existingLink.Role, role);
                    existingLink.Role = role;
                }
                continue;
            }

            db.GuildPlants.Add(new GuildPlant
            {
                GuildId = guild.Id,
                PlantId = plant.Id,
                Role = role
            });
        }
    }
}
