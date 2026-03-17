using GardenAssistant.Data;
using GardenAssistant.Data.Entities;
using GardenAssistant.DTOs.Guilds;
using GardenAssistant.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GardenAssistant.Services;

public class GuildService(AppDbContext dbContext) : IGuildService
{
    public async Task<IEnumerable<GuildSummaryDto>> GetAllAsync(Guid userId)
    {
        var guilds = await dbContext.Guilds
            .Where(g => g.UserId == null || g.UserId == userId)
            .OrderBy(g => g.Name)
            .ToListAsync();

        var guildIds = guilds.Select(g => g.Id).ToList();

        var guildPlants = await dbContext.GuildPlants
            .Where(gp => guildIds.Contains(gp.GuildId))
            .Join(dbContext.Plants, gp => gp.PlantId, p => p.Id, (gp, p) => new { gp.GuildId, p.Id, p.Name, p.ScientificName })
            .OrderBy(x => x.Name)
            .ToListAsync();

        var plantsByGuild = guildPlants
            .GroupBy(x => x.GuildId)
            .ToDictionary(g => g.Key, g => g.Select(x => new GuildPlantMemberDto(x.Id, x.Name, x.ScientificName)).ToList());

        return guilds.Select(g => new GuildSummaryDto(
            g.Id,
            g.Name,
            g.Description,
            plantsByGuild.GetValueOrDefault(g.Id, []),
            g.UserId == null
        ));
    }

    public async Task<GuildDetailDto?> GetByIdAsync(Guid id, Guid userId)
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
            .Join(dbContext.Plants, gp => gp.PlantId, p => p.Id, (gp, p) => p)
            .OrderBy(p => p.Name)
            .Select(p => new GuildPlantMemberDto(p.Id, p.Name, p.ScientificName))
            .ToListAsync();

        return new GuildDetailDto(
            guild.Id,
            guild.Name,
            guild.Description,
            plants,
            guild.UserId == null,
            guild.UserId == userId
        );
    }

    public async Task<GuildDetailDto> CreateAsync(CreateGuildRequest request, Guid userId)
    {
        var validPlantIds = await dbContext.Plants
            .Where(p => request.PlantIds.Contains(p.Id))
            .Select(p => p.Id)
            .ToListAsync();

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
                PlantId = plantId
            });
        }

        await dbContext.SaveChangesAsync();

        var plants = await dbContext.GuildPlants
            .Where(gp => gp.GuildId == guild.Id)
            .Join(dbContext.Plants, gp => gp.PlantId, p => p.Id, (gp, p) => p)
            .OrderBy(p => p.Name)
            .Select(p => new GuildPlantMemberDto(p.Id, p.Name, p.ScientificName))
            .ToListAsync();

        return new GuildDetailDto(
            guild.Id,
            guild.Name,
            guild.Description,
            plants,
            false,
            true
        );
    }

    public async Task<GuildDetailDto?> UpdateAsync(Guid id, UpdateGuildRequest request, Guid userId)
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

        var validPlantIds = await dbContext.Plants
            .Where(p => request.PlantIds.Contains(p.Id))
            .Select(p => p.Id)
            .ToListAsync();

        foreach (var plantId in validPlantIds)
        {
            dbContext.GuildPlants.Add(new GuildPlant
            {
                GuildId = guild.Id,
                PlantId = plantId
            });
        }

        await dbContext.SaveChangesAsync();

        var plants = await dbContext.GuildPlants
            .Where(gp => gp.GuildId == guild.Id)
            .Join(dbContext.Plants, gp => gp.PlantId, p => p.Id, (gp, p) => p)
            .OrderBy(p => p.Name)
            .Select(p => new GuildPlantMemberDto(p.Id, p.Name, p.ScientificName))
            .ToListAsync();

        return new GuildDetailDto(
            guild.Id,
            guild.Name,
            guild.Description,
            plants,
            false,
            true
        );
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
