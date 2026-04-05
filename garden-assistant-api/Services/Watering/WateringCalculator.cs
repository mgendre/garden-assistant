using GardenAssistant.Data.Entities.Enums;
using GardenAssistant.Models;

namespace GardenAssistant.Services.Watering;

public class WateringCalculator : IWateringCalculator
{
    private static readonly Dictionary<int, DayOfWeek[]> RecommendedDaysMap = new()
    {
        [1] = [DayOfWeek.Saturday],
        [2] = [DayOfWeek.Wednesday, DayOfWeek.Saturday],
        [3] = [DayOfWeek.Wednesday, DayOfWeek.Saturday, DayOfWeek.Sunday],
        [4] = [DayOfWeek.Tuesday, DayOfWeek.Thursday, DayOfWeek.Saturday, DayOfWeek.Sunday],
        [5] = [DayOfWeek.Monday, DayOfWeek.Wednesday, DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday],
        [6] = [DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday],
        [7] = [DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday],
    };

    public WateringFrequency CalculateFrequency(
        WaterNeeds waterNeeds,
        int halfMonth,
        SoilType? soilType = null,
        bool hasMulch = false)
    {
        var season = GetSeason(halfMonth);
        var baseFrequency = GetBaseFrequency(waterNeeds, season);
        var adjusted = ApplyCoefficients(baseFrequency, soilType, hasMulch, season);
        var days = RecommendedDaysMap.TryGetValue(adjusted, out var d) ? d : RecommendedDaysMap[7];
        return new WateringFrequency(adjusted, days);
    }

    private static int GetBaseFrequency(WaterNeeds waterNeeds, Season season) => (waterNeeds, season) switch
    {
        (WaterNeeds.Low,    Season.Winter) => 1,
        (WaterNeeds.Low,    Season.Spring) => 1,
        (WaterNeeds.Low,    Season.Summer) => 2,
        (WaterNeeds.Low,    Season.Autumn) => 1,
        (WaterNeeds.Medium, Season.Winter) => 1,
        (WaterNeeds.Medium, Season.Spring) => 2,
        (WaterNeeds.Medium, Season.Summer) => 4,
        (WaterNeeds.Medium, Season.Autumn) => 2,
        (WaterNeeds.High,   Season.Winter) => 2,
        (WaterNeeds.High,   Season.Spring) => 3,
        (WaterNeeds.High,   Season.Summer) => 5,
        (WaterNeeds.High,   Season.Autumn) => 3,
        _ => 1
    };

    private static Season GetSeason(int halfMonth) => halfMonth switch
    {
        >= 1  and <= 4  => Season.Winter,
        >= 5  and <= 10 => Season.Spring,
        >= 11 and <= 16 => Season.Summer,
        >= 17 and <= 22 => Season.Autumn,
        >= 23 and <= 24 => Season.Winter,
        _ => throw new ArgumentOutOfRangeException(nameof(halfMonth))
    };

    private static int ApplyCoefficients(int baseFrequency, SoilType? soilType, bool hasMulch, Season season)
    {
        var frequency = baseFrequency * GetSoilCoefficient(soilType);
        if (hasMulch) { frequency *= 0.6; }
        var rounded = (int)Math.Round(frequency);
        var minimum = season == Season.Winter ? 0 : 1;
        return Math.Max(rounded, minimum);
    }

    private static double GetSoilCoefficient(SoilType? soilType) => soilType switch
    {
        SoilType.Sandy  => 1.3,
        SoilType.Loam   => 1.0,
        SoilType.Clay   => 0.7,
        SoilType.Silty  => 0.9,
        SoilType.Chalky => 1.2,
        SoilType.Peaty  => 0.8,
        SoilType.Rocky  => 1.3,
        _ => 1.0
    };

}
