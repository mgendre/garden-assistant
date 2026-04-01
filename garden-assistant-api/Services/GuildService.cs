using GardenAssistant.Data;
using GardenAssistant.Data.Entities;
using GardenAssistant.Data.Entities.Enums;
using GardenAssistant.DTOs.Guilds;
using GardenAssistant.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GardenAssistant.Services;

public class GuildService(AppDbContext dbContext) : IGuildService
{
    public async Task<IEnumerable<GuildDto>> GetAllAsync(Guid userId)
    {
        var bedGuildIds = await dbContext.Plantings
            .Where(p => p.GuildId != null)
            .Select(p => p.GuildId!.Value)
            .Distinct()
            .ToListAsync();

        var guilds = await dbContext.Guilds
            .Where(g => (g.UserId == null || g.UserId == userId) && !bedGuildIds.Contains(g.Id))
            .OrderBy(g => g.Name)
            .ToListAsync();

        var guildIds = guilds.Select(g => g.Id).ToList();

        var guildPlants = await dbContext.GuildPlants
            .Where(gp => guildIds.Contains(gp.GuildId))
            .Join(dbContext.Plants, gp => gp.PlantId, p => p.Id, (gp, p) => new { gp.GuildId, p.Id, p.Name, p.ScientificName, gp.Role })
            .OrderByDescending(x => x.Role)
            .ThenBy(x => x.Name)
            .ToListAsync();

        var plantsByGuild = guildPlants
            .GroupBy(x => x.GuildId)
            .ToDictionary(g => g.Key, g => g.Select(x => new GuildPlantMemberDto(x.Id, x.Name, x.ScientificName, x.Role)).ToList());

        return guilds.Select(g => new GuildDto(
            g.Id,
            g.Name,
            g.Description,
            plantsByGuild.GetValueOrDefault(g.Id, []),
            g.UserId == null,
            g.UserId == userId
        ));
    }

    public async Task<GuildDto?> GetByIdAsync(Guid id, Guid userId)
    {
        var guild = await dbContext.Guilds
            .Where(g => g.Id == id && (g.UserId == null || g.UserId == userId))
            .FirstOrDefaultAsync();

        if (guild is null)
        {
            return null;
        }

        var plants = await dbContext.GuildPlants
            .Where(gp => gp.GuildId == id)
            .Join(dbContext.Plants, gp => gp.PlantId, p => p.Id, (gp, p) => new { p.Id, p.Name, p.ScientificName, gp.Role })
            .OrderByDescending(x => x.Role)
            .ThenBy(x => x.Name)
            .Select(x => new GuildPlantMemberDto(x.Id, x.Name, x.ScientificName, x.Role))
            .ToListAsync();

        return new GuildDto(
            guild.Id,
            guild.Name,
            guild.Description,
            plants,
            guild.UserId == null,
            guild.UserId == userId
        );
    }

    public async Task<GuildDto> CreateAsync(CreateGuildRequest request, Guid userId)
    {
        var plantIds = request.Plants.Select(p => p.PlantId).ToList();
        var validPlantIds = await dbContext.Plants
            .Where(p => plantIds.Contains(p.Id))
            .Select(p => p.Id)
            .ToListAsync();

        var roleByPlantId = request.Plants.ToDictionary(p => p.PlantId, p => p.Role ?? GuildPlantRole.Companion);

        var guild = new Guild
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Name = request.Name,
            Description = request.Description
        };

        dbContext.Guilds.Add(guild);

        foreach (var plantId in validPlantIds)
        {
            dbContext.GuildPlants.Add(new GuildPlant
            {
                GuildId = guild.Id,
                PlantId = plantId,
                Role = roleByPlantId.GetValueOrDefault(plantId, GuildPlantRole.Companion)
            });
        }

        await dbContext.SaveChangesAsync();

        return await GetByIdAsync(guild.Id, userId) ?? throw new InvalidOperationException("Guild not found after creation.");
    }

    public async Task<GuildDto?> UpdateAsync(Guid id, UpdateGuildRequest request, Guid userId)
    {
        var guild = await dbContext.Guilds
            .Where(g => g.Id == id && g.UserId == userId)
            .FirstOrDefaultAsync();

        if (guild is null)
        {
            return null;
        }

        guild.Name = request.Name;
        guild.Description = request.Description;

        var existingPlants = await dbContext.GuildPlants
            .Where(gp => gp.GuildId == id)
            .ToListAsync();

        dbContext.GuildPlants.RemoveRange(existingPlants);

        var plantIds = request.Plants.Select(p => p.PlantId).ToList();
        var validPlantIds = await dbContext.Plants
            .Where(p => plantIds.Contains(p.Id))
            .Select(p => p.Id)
            .ToListAsync();

        var roleByPlantId = request.Plants.ToDictionary(p => p.PlantId, p => p.Role ?? GuildPlantRole.Companion);

        foreach (var plantId in validPlantIds)
        {
            dbContext.GuildPlants.Add(new GuildPlant
            {
                GuildId = guild.Id,
                PlantId = plantId,
                Role = roleByPlantId.GetValueOrDefault(plantId, GuildPlantRole.Companion)
            });
        }

        await dbContext.SaveChangesAsync();

        return await GetByIdAsync(guild.Id, userId);
    }

    public async Task<bool> DeleteAsync(Guid id, Guid userId)
    {
        var guild = await dbContext.Guilds
            .Where(g => g.Id == id && g.UserId == userId)
            .FirstOrDefaultAsync();

        if (guild is null)
        {
            return false;
        }

        dbContext.Guilds.Remove(guild);
        await dbContext.SaveChangesAsync();
        return true;
    }
}
