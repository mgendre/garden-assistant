using GardenAssistant.Data;
using GardenAssistant.Data.Entities;
using GardenAssistant.Data.Entities.Enums;
using GardenAssistant.DTOs.Watering;
using Microsoft.EntityFrameworkCore;

namespace GardenAssistant.Services.Watering;

public class GardenWateringService(AppDbContext dbContext, IWateringCalculator calculator) : IGardenWateringService
{
    public async Task<WateringScheduleDto> GetScheduleAsync(Guid userId, Guid gardenId, int halfMonth)
    {
        var plantings = await dbContext.Plantings
            .Where(p => p.UserId == userId && p.GardenId == gardenId && p.GuildId.HasValue)
            .ToListAsync();

        if (plantings.Count == 0)
        {
            return new WateringScheduleDto([]);
        }

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

        var beds = plantings.Select(p => BuildBedDto(p, plantsByGuild, halfMonth)).ToList();

        return new WateringScheduleDto(beds);
    }

    private BedWateringDto BuildBedDto(
        Planting planting,
        Dictionary<Guid, List<Plant>> plantsByGuild,
        int halfMonth)
    {
        var plants = plantsByGuild.GetValueOrDefault(planting.GuildId!.Value, [])
            .Select(plant => BuildPlantDto(plant, halfMonth, planting.SoilType, planting.HasMulch))
            .ToList();

        return new BedWateringDto(planting.Id, planting.Name, false, planting.SoilType, planting.HasMulch, plants);
    }

    private PlantWateringDto BuildPlantDto(Plant plant, int halfMonth, SoilType? soilType, bool hasMulch)
    {
        var freq = calculator.CalculateFrequency(plant.WaterNeeds, halfMonth, soilType, hasMulch);
        return new PlantWateringDto(plant.Id, plant.Name, plant.WaterNeeds, freq.TimesPerWeek, freq.RecommendedDays);
    }
}
