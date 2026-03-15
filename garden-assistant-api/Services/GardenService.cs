using GardenAssistant.Data;
using GardenAssistant.Data.Entities;
using GardenAssistant.DTOs;
using Microsoft.EntityFrameworkCore;

using GardenAssistant.Services.Interfaces;

namespace GardenAssistant.Services;

public class GardenService(AppDbContext dbContext) : IGardenService
{
    public async Task<IEnumerable<GardenDto>> GetAllAsync(Guid userId)
    {
        return await dbContext.Gardens
            .Where(g => g.UserId == userId)
            .Select(g => new GardenDto(g.Id, g.Name, g.Description))
            .ToListAsync();
    }

    public async Task<GardenDto> CreateAsync(CreateGardenRequest request, Guid userId)
    {
        var garden = new Garden
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            UserId = userId
        };

        dbContext.Gardens.Add(garden);
        await dbContext.SaveChangesAsync();

        return new GardenDto(garden.Id, garden.Name, garden.Description);
    }

    public async Task<GardenDto?> UpdateAsync(Guid id, UpdateGardenRequest request, Guid userId)
    {
        var garden = await dbContext.Gardens
            .FirstOrDefaultAsync(g => g.Id == id && g.UserId == userId);
        if (garden is null)
        {
            return null;
        }

        garden.Name = request.Name;
        garden.Description = request.Description;
        await dbContext.SaveChangesAsync();

        return new GardenDto(garden.Id, garden.Name, garden.Description);
    }

    public async Task<bool> DeleteAsync(Guid id, Guid userId)
    {
        var garden = await dbContext.Gardens
            .FirstOrDefaultAsync(g => g.Id == id && g.UserId == userId);
        if (garden is null)
        {
            return false;
        }

        dbContext.Gardens.Remove(garden);
        await dbContext.SaveChangesAsync();

        return true;
    }
}
