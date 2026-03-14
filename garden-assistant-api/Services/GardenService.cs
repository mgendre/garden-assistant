using GardenAssistant.Data;
using GardenAssistant.DTOs;
using GardenAssistant.Entities;
using Microsoft.EntityFrameworkCore;

namespace GardenAssistant.Services;

public class GardenService(AppDbContext dbContext)
{
    // TODO(auth): Filter by the authenticated user's ID from HttpContext.User claims.
    public async Task<IEnumerable<GardenDto>> GetAllAsync()
    {
        return await dbContext.Gardens
            .Select(g => new GardenDto(g.Id, g.Name, g.Description, g.UserId))
            .ToListAsync();
    }

    // TODO(auth): UserId should come from HttpContext.User claims, not the request body.
    public async Task<GardenDto> CreateAsync(CreateGardenRequest request)
    {
        var garden = new Garden
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            UserId = request.UserId
        };

        dbContext.Gardens.Add(garden);
        await dbContext.SaveChangesAsync();

        return new GardenDto(garden.Id, garden.Name, garden.Description, garden.UserId);
    }

    public async Task<GardenDto?> UpdateAsync(Guid id, UpdateGardenRequest request)
    {
        var garden = await dbContext.Gardens.FindAsync(id);
        if (garden is null) return null;

        garden.Name = request.Name;
        garden.Description = request.Description;
        await dbContext.SaveChangesAsync();

        return new GardenDto(garden.Id, garden.Name, garden.Description, garden.UserId);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var garden = await dbContext.Gardens.FindAsync(id);
        if (garden is null) return false;

        dbContext.Gardens.Remove(garden);
        await dbContext.SaveChangesAsync();

        return true;
    }
}
