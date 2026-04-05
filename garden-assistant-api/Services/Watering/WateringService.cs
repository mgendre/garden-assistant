using GardenAssistant.Data;
using GardenAssistant.Data.Entities;
using GardenAssistant.DTOs.Watering;
using Microsoft.EntityFrameworkCore;

namespace GardenAssistant.Services.Watering;

public class WateringService(AppDbContext dbContext, IWateringCalculator calculator) : IWateringService
{
    public async Task<WateringTodayDto> GetWateringTodayAsync(Guid userId, DateOnly today)
    {
        var (plantings, plantsByGuild) = await LoadGardenDataAsync(userId);
        var halfMonth = GetHalfMonth(today);

        var beds = plantings
            .Where(p => p.GuildId.HasValue)
            .Select(p => BuildBedTodayDto(p, plantsByGuild, halfMonth, today))
            .Where(b => b.Plants.Count > 0)
            .ToList();

        return new WateringTodayDto(beds);
    }

    public Task<WateringScheduleDto> GetWateringScheduleAsync(Guid userId, int halfMonth, string source)
        => Task.FromResult(new WateringScheduleDto([]));

    private BedWateringTodayDto BuildBedTodayDto(
        Planting planting,
        Dictionary<Guid, List<Plant>> plantsByGuild,
        int halfMonth,
        DateOnly today)
    {
        var plants = plantsByGuild.GetValueOrDefault(planting.GuildId!.Value, []);
        var plantStatuses = plants
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

    private async Task<(List<Planting> plantings, Dictionary<Guid, List<Plant>> plantsByGuild)> LoadGardenDataAsync(Guid userId)
    {
        var plantings = await dbContext.Plantings
            .Where(p => p.UserId == userId && p.GuildId.HasValue)
            .ToListAsync();

        var guildIds = plantings.Select(p => p.GuildId!.Value).ToList();

        var guildPlantPairs = await dbContext.GuildPlants
            .Where(gp => guildIds.Contains(gp.GuildId))
            .ToListAsync();

        var plantIds = guildPlantPairs.Select(gp => gp.PlantId).Distinct().ToList();

        var plantsById = await dbContext.Plants
            .Where(p => plantIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id);

        var plantsByGuild = guildPlantPairs
            .GroupBy(gp => gp.GuildId)
            .ToDictionary(
                g => g.Key,
                g => g.Where(gp => plantsById.ContainsKey(gp.PlantId))
                      .Select(gp => plantsById[gp.PlantId])
                      .ToList());

        return (plantings, plantsByGuild);
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
