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

    public async Task<WateringScheduleDto> GetWateringScheduleAsync(Guid userId, int halfMonth, string source)
    {
        var beds = new List<BedWateringDto>();

        if (source is "gardenPlants" or "all")
        {
            beds.AddRange(await BuildGardenBedsAsync(userId, halfMonth));
        }

        if (source is "myPlants" or "all")
        {
            var personalBed = await BuildPersonalPlantsBedAsync(userId, halfMonth);
            if (personalBed.Plants.Count > 0) { beds.Add(personalBed); }
        }

        return new WateringScheduleDto(beds);
    }

    private async Task<List<BedWateringDto>> BuildGardenBedsAsync(Guid userId, int halfMonth)
    {
        var (plantings, plantsByGuild) = await LoadGardenDataAsync(userId);

        return plantings
            .Where(p => p.GuildId.HasValue)
            .Select(p =>
            {
                var plants = plantsByGuild.GetValueOrDefault(p.GuildId!.Value, [])
                    .Select(plant => BuildPlantWateringDto(plant, halfMonth, null, false))
                    .ToList();
                return new BedWateringDto(p.Id, p.Name, false, null, false, plants);
            })
            .ToList();
    }

    private async Task<BedWateringDto> BuildPersonalPlantsBedAsync(Guid userId, int halfMonth)
    {
        var userPlantIds = await dbContext.UserPlants
            .Where(up => up.UserId == userId)
            .Select(up => up.PlantId)
            .ToListAsync();

        var plants = await dbContext.Plants
            .Where(p => userPlantIds.Contains(p.Id))
            .ToListAsync();

        var plantDtos = plants
            .Select(p => BuildPlantWateringDto(p, halfMonth, null, false))
            .ToList();

        return new BedWateringDto(null, "MyPlants", true, null, false, plantDtos);
    }

    private PlantWateringDto BuildPlantWateringDto(Plant plant, int halfMonth, Data.Entities.Enums.SoilType? soilType, bool hasMulch)
    {
        var freq = calculator.CalculateFrequency(plant.WaterNeeds, halfMonth, soilType, hasMulch);
        return new PlantWateringDto(plant.Id, plant.Name, plant.WaterNeeds, freq.TimesPerWeek, freq.RecommendedDays, null);
    }

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
