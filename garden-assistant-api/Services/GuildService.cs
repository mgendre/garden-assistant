using GardenAssistant.Data;
using GardenAssistant.DTOs;
using Microsoft.EntityFrameworkCore;

namespace GardenAssistant.Services;

public interface IGuildService
{
    Task<IEnumerable<GuildSummaryDto>> GetAllAsync();
    Task<GuildDetailDto?> GetByIdAsync(Guid id);
}

public class GuildService(AppDbContext dbContext) : IGuildService
{
    public async Task<IEnumerable<GuildSummaryDto>> GetAllAsync()
    {
        var guilds = await dbContext.Guilds.OrderBy(g => g.Name).ToListAsync();
        var plantCounts = await dbContext.GuildPlants
            .GroupBy(gp => gp.GuildId)
            .Select(g => new { GuildId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.GuildId, x => x.Count);

        return guilds.Select(g => new GuildSummaryDto(
            g.Id,
            g.Name,
            g.Description,
            plantCounts.GetValueOrDefault(g.Id, 0)
        ));
    }

    public async Task<GuildDetailDto?> GetByIdAsync(Guid id)
    {
        var guild = await dbContext.Guilds.FindAsync(id);
        if (guild is null) return null;

        var plants = await dbContext.GuildPlants
            .Where(gp => gp.GuildId == id)
            .Join(dbContext.Plants, gp => gp.PlantId, p => p.Id,
                (gp, p) => p)
            .OrderBy(p => p.Name)
            .Select(p => new GuildPlantMemberDto(p.Id, p.Name, p.ScientificName))
            .ToListAsync();

        return new GuildDetailDto(guild.Id, guild.Name, guild.Description, plants);
    }
}
