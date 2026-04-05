using GardenAssistant.Data;
using GardenAssistant.DTOs.Watering;
using Microsoft.EntityFrameworkCore;

namespace GardenAssistant.Services.Watering;

public class WateringService(AppDbContext dbContext, IWateringCalculator calculator) : IWateringService
{
    public async Task<WateringTodayDto> GetWateringTodayAsync(Guid userId, DateOnly today)
    {
        var (plantings, plantsById) = await LoadGardenDataAsync(userId);
        var halfMonth = GetHalfMonth(today);

        var beds = plantings
            .Where(p => p.GuildId.HasValue)
            .Select(p => BuildBedTodayDto(p, plantsById, halfMonth, today))
            .Where(b => b.Plants.Count > 0)
            .ToList();

        return new WateringTodayDto(beds);
    }

    public Task<WateringScheduleDto> GetWateringScheduleAsync(Guid userId, int halfMonth, string source)
        => Task.FromResult(new WateringScheduleDto([]));

    private BedWateringTodayDto BuildBedTodayDto(
        Data.Entities.Planting planting,
        Dictionary<Guid, Data.Entities.Plant> plantsById,
        int halfMonth,
        DateOnly today)
    {
        var plantStatuses = GetPlantsForGuild(planting.GuildId!.Value, plantsById)
            .Select(plant =>
            {
                var freq = calculator.CalculateFrequency(plant.WaterNeeds, halfMonth);
                var isToday = freq.RecommendedDays.Contains(today.DayOfWeek);
                var next = isToday ? (DayOfWeek?)null : FindNextDay(freq.RecommendedDays, today.DayOfWeek);
                return new PlantWateringStatusDto(plant.Id, plant.Name, isToday, next);
            })
            .ToList();

        return new BedWateringTodayDto(planting.Id, planting.Name, false, plantStatuses);
    }

    private IEnumerable<Data.Entities.Plant> GetPlantsForGuild(Guid guildId, Dictionary<Guid, Data.Entities.Plant> plantsById)
    {
        return dbContext.GuildPlants
            .Where(gp => gp.GuildId == guildId)
            .Select(gp => gp.PlantId)
            .AsEnumerable()
            .Where(plantsById.ContainsKey)
            .Select(id => plantsById[id]);
    }

    private async Task<(List<Data.Entities.Planting> plantings, Dictionary<Guid, Data.Entities.Plant> plantsById)> LoadGardenDataAsync(Guid userId)
    {
        var plantings = await dbContext.Plantings
            .Where(p => p.UserId == userId && p.GuildId.HasValue)
            .ToListAsync();

        var guildIds = plantings.Select(p => p.GuildId!.Value).ToList();

        var plantIds = await dbContext.GuildPlants
            .Where(gp => guildIds.Contains(gp.GuildId))
            .Select(gp => gp.PlantId)
            .Distinct()
            .ToListAsync();

        var plantsById = await dbContext.Plants
            .Where(p => plantIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id);

        return (plantings, plantsById);
    }

    private static int GetHalfMonth(DateOnly date)
        => (date.Month - 1) * 2 + (date.Day <= 15 ? 1 : 2);

    private static DayOfWeek? FindNextDay(DayOfWeek[] days, DayOfWeek today)
    {
        var todayInt = (int)today;
        return days
            .Select(d => ((int)d - todayInt + 7) % 7)
            .Where(diff => diff > 0)
            .OrderBy(diff => diff)
            .Select<int, DayOfWeek?>(diff => (DayOfWeek)((todayInt + diff) % 7))
            .FirstOrDefault()
            ?? days.MinBy(d => (int)d);
    }
}
