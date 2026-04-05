using GardenAssistant.Data.Entities.Enums;
using GardenAssistant.Services.Watering;
using Shouldly;

namespace GardenAssistant.Tests.Watering;

public class WateringCalculatorTests
{
    private readonly WateringCalculator _sut = new();

    // --- Matrice de base ---

    [Theory]
    [InlineData(1,  1)]
    [InlineData(4,  1)]
    [InlineData(23, 1)]
    [InlineData(24, 1)]
    public void CalculateFrequency_WhenLowAndWinter_ShouldReturn1(int halfMonth, int expected)
    {
        var result = _sut.CalculateFrequency(WaterNeeds.Low, halfMonth);
        result.TimesPerWeek.ShouldBe(expected);
    }

    [Theory]
    [InlineData(5,  1)]
    [InlineData(10, 1)]
    public void CalculateFrequency_WhenLowAndSpring_ShouldReturn1(int halfMonth, int expected)
    {
        var result = _sut.CalculateFrequency(WaterNeeds.Low, halfMonth);
        result.TimesPerWeek.ShouldBe(expected);
    }

    [Theory]
    [InlineData(11, 2)]
    [InlineData(16, 2)]
    public void CalculateFrequency_WhenLowAndSummer_ShouldReturn2(int halfMonth, int expected)
    {
        var result = _sut.CalculateFrequency(WaterNeeds.Low, halfMonth);
        result.TimesPerWeek.ShouldBe(expected);
    }

    [Theory]
    [InlineData(11, 4)]
    [InlineData(16, 4)]
    public void CalculateFrequency_WhenMediumAndSummer_ShouldReturn4(int halfMonth, int expected)
    {
        var result = _sut.CalculateFrequency(WaterNeeds.Medium, halfMonth);
        result.TimesPerWeek.ShouldBe(expected);
    }

    [Theory]
    [InlineData(11, 5)]
    [InlineData(16, 5)]
    public void CalculateFrequency_WhenHighAndSummer_ShouldReturn5(int halfMonth, int expected)
    {
        var result = _sut.CalculateFrequency(WaterNeeds.High, halfMonth);
        result.TimesPerWeek.ShouldBe(expected);
    }

    [Theory]
    [InlineData(5,  3)]
    [InlineData(17, 3)]
    public void CalculateFrequency_WhenHighAndSpringOrAutumn_ShouldReturn3(int halfMonth, int expected)
    {
        var result = _sut.CalculateFrequency(WaterNeeds.High, halfMonth);
        result.TimesPerWeek.ShouldBe(expected);
    }

    [Theory]
    [InlineData(1,  2)]
    [InlineData(24, 2)]
    public void CalculateFrequency_WhenHighAndWinter_ShouldReturn2(int halfMonth, int expected)
    {
        var result = _sut.CalculateFrequency(WaterNeeds.High, halfMonth);
        result.TimesPerWeek.ShouldBe(expected);
    }

    // --- RecommendedDays ---

    [Fact]
    public void CalculateFrequency_RecommendedDaysLength_ShouldMatchTimesPerWeek()
    {
        var result = _sut.CalculateFrequency(WaterNeeds.Low, halfMonth: 13);
        result.RecommendedDays.Length.ShouldBe(result.TimesPerWeek);
    }

    [Fact]
    public void CalculateFrequency_WhenOncePerWeek_ShouldRecommendSaturday()
    {
        var result = _sut.CalculateFrequency(WaterNeeds.Low, halfMonth: 7);
        result.RecommendedDays.ShouldBe([DayOfWeek.Saturday]);
    }

    [Fact]
    public void CalculateFrequency_WhenTwicePerWeek_ShouldRecommendWedSat()
    {
        var result = _sut.CalculateFrequency(WaterNeeds.Low, halfMonth: 13);
        result.RecommendedDays.ShouldBe([DayOfWeek.Wednesday, DayOfWeek.Saturday]);
    }

    [Fact]
    public void CalculateFrequency_WhenFourPerWeek_ShouldRecommendTueThuSatSun()
    {
        var result = _sut.CalculateFrequency(WaterNeeds.Medium, halfMonth: 13);
        result.RecommendedDays.ShouldBe([DayOfWeek.Tuesday, DayOfWeek.Thursday, DayOfWeek.Saturday, DayOfWeek.Sunday]);
    }
}
