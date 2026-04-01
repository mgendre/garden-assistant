using GardenAssistant.Data;
using GardenAssistant.Data.Entities;
using GardenAssistant.DTOs.Gardens;
using GardenAssistant.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GardenAssistant.Services;

public class GardenService(AppDbContext dbContext) : IGardenService
{
    public async Task<IEnumerable<GardenDto>> GetAllAsync(Guid userId)
    {
        var gardens = await dbContext.Gardens
            .Where(g => g.UserId == userId)
            .OrderByDescending(g => g.CreatedAtUtc)
            .ToListAsync();

        var gardenIds = gardens.Select(g => g.Id).ToList();

        var bedCounts = await dbContext.Plantings
            .Where(p => gardenIds.Contains(p.GardenId))
            .GroupBy(p => p.GardenId)
            .Select(g => new { GardenId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.GardenId, x => x.Count);

        return gardens.Select(g => new GardenDto(
            g.Id,
            g.Name,
            g.Description,
            bedCounts.GetValueOrDefault(g.Id, 0),
            g.CreatedAtUtc
        ));
    }

    public async Task<GardenDto?> GetByIdAsync(Guid id, Guid userId)
    {
        var garden = await dbContext.Gardens
            .Where(g => g.Id == id && g.UserId == userId)
            .FirstOrDefaultAsync();

        if (garden is null)
        {
            return null;
        }

        var bedCount = await dbContext.Plantings
            .Where(p => p.GardenId == id)
            .CountAsync();

        return new GardenDto(garden.Id, garden.Name, garden.Description, bedCount, garden.CreatedAtUtc);
    }

    public async Task<GardenDto> CreateAsync(CreateGardenRequest request, Guid userId)
    {
        var garden = new Garden
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            UserId = userId,
            CreatedAtUtc = DateTime.UtcNow
        };

        dbContext.Gardens.Add(garden);
        await dbContext.SaveChangesAsync();

        return new GardenDto(garden.Id, garden.Name, garden.Description, 0, garden.CreatedAtUtc);
    }

    public async Task<GardenDto?> UpdateAsync(Guid id, UpdateGardenRequest request, Guid userId)
    {
        var garden = await dbContext.Gardens
            .Where(g => g.Id == id && g.UserId == userId)
            .FirstOrDefaultAsync();

        if (garden is null)
        {
            return null;
        }

        garden.Name = request.Name;
        garden.Description = request.Description;
        await dbContext.SaveChangesAsync();

        return await GetByIdAsync(id, userId);
    }

    public async Task<bool> DeleteAsync(Guid id, Guid userId)
    {
        var garden = await dbContext.Gardens
            .Where(g => g.Id == id && g.UserId == userId)
            .FirstOrDefaultAsync();

        if (garden is null)
        {
            return false;
        }

        var bedGuilds = await dbContext.Plantings
            .Where(p => p.GardenId == id && p.GuildId != null)
            .Join(dbContext.Guilds, p => p.GuildId, g => g.Id, (p, g) => g)
            .ToListAsync();

        dbContext.Guilds.RemoveRange(bedGuilds);

        dbContext.Gardens.Remove(garden);
        await dbContext.SaveChangesAsync();
        return true;
    }
}
