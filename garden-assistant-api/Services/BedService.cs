using GardenAssistant.Data;
using GardenAssistant.Data.Entities;
using GardenAssistant.DTOs.Beds;
using GardenAssistant.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GardenAssistant.Services;

public class BedService(AppDbContext dbContext) : IBedService
{
    public async Task<IEnumerable<BedDto>> GetByGardenIdAsync(Guid gardenId, Guid userId)
    {
        var gardenOwned = await dbContext.Gardens
            .AnyAsync(g => g.Id == gardenId && g.UserId == userId);

        if (!gardenOwned)
        {
            return [];
        }

        var beds = await dbContext.Plantings
            .Where(p => p.GardenId == gardenId)
            .OrderBy(p => p.CreatedAtUtc)
            .ToListAsync();

        var bedGuildIds = beds
            .Where(b => b.GuildId.HasValue)
            .Select(b => b.GuildId!.Value)
            .ToList();

        var plantIdsByGuild = await dbContext.GuildPlants
            .Where(gp => bedGuildIds.Contains(gp.GuildId))
            .GroupBy(gp => gp.GuildId)
            .ToDictionaryAsync(
                g => g.Key,
                g => g.Select(gp => gp.PlantId).ToList());

        return beds.Select(b => new BedDto(
            b.Id,
            b.Name,
            b.GuildId,
            b.GuildId.HasValue
                ? plantIdsByGuild.GetValueOrDefault(b.GuildId.Value, [])
                : []
        ));
    }

    public async Task<BedDto?> CreateAsync(Guid gardenId, CreateBedRequest request, Guid userId)
    {
        var gardenOwned = await dbContext.Gardens
            .AnyAsync(g => g.Id == gardenId && g.UserId == userId);

        if (!gardenOwned)
        {
            return null;
        }

        var bedName = request.Name ?? "";

        var guild = new Guild
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Name = bedName
        };
        dbContext.Guilds.Add(guild);

        var bed = new Planting
        {
            Id = Guid.NewGuid(),
            GardenId = gardenId,
            UserId = userId,
            Name = bedName,
            GuildId = guild.Id,
            CreatedAtUtc = DateTime.UtcNow
        };
        dbContext.Plantings.Add(bed);

        await dbContext.SaveChangesAsync();

        return new BedDto(bed.Id, bed.Name, guild.Id, []);
    }

    public async Task<BedDto?> UpdateAsync(Guid gardenId, Guid bedId, UpdateBedRequest request, Guid userId)
    {
        var bed = await dbContext.Plantings
            .Where(p => p.Id == bedId && p.GardenId == gardenId && p.UserId == userId)
            .FirstOrDefaultAsync();

        if (bed is null)
        {
            return null;
        }

        var newName = request.Name ?? "";
        bed.Name = newName;

        var guild = bed.GuildId.HasValue ? await dbContext.Guilds.FindAsync(bed.GuildId.Value) : null;
        if (guild is not null)
        {
            guild.Name = newName;
        }

        await dbContext.SaveChangesAsync();

        var plantIds = bed.GuildId.HasValue
            ? await dbContext.GuildPlants
                .Where(gp => gp.GuildId == bed.GuildId.Value)
                .Select(gp => gp.PlantId)
                .ToListAsync()
            : [];

        return new BedDto(bed.Id, bed.Name, bed.GuildId, plantIds);
    }

    public async Task<bool> DeleteAsync(Guid gardenId, Guid bedId, Guid userId)
    {
        var bed = await dbContext.Plantings
            .Where(p => p.Id == bedId && p.GardenId == gardenId && p.UserId == userId)
            .FirstOrDefaultAsync();

        if (bed is null)
        {
            return false;
        }

        var guild = bed.GuildId.HasValue ? await dbContext.Guilds.FindAsync(bed.GuildId.Value) : null;
        dbContext.Plantings.Remove(bed);

        if (guild is not null)
        {
            dbContext.Guilds.Remove(guild);
        }

        await dbContext.SaveChangesAsync();
        return true;
    }
}
