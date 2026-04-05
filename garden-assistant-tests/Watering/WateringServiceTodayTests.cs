using GardenAssistant.Data.Entities;
using GardenAssistant.Data.Entities.Enums;
using GardenAssistant.Services.Watering;
using GardenAssistant.Tests.Infrastructure;
using Shouldly;

namespace GardenAssistant.Tests.Watering;

public class WateringServiceTodayTests : DatabaseTestBase
{
    private readonly WateringService _sut;
    private readonly Guid _userId = Guid.NewGuid();
    private readonly Guid _gardenId = Guid.NewGuid();

    public WateringServiceTodayTests()
    {
        DbContext.Users.Add(new User { Id = _userId, Email = "test@example.com" });
        DbContext.Gardens.Add(new Garden { Id = _gardenId, Name = "Test", UserId = _userId, CreatedAtUtc = DateTime.UtcNow });
        DbContext.SaveChanges();
        _sut = new WateringService(DbContext, new WateringCalculator());
    }

    [Fact]
    public async Task GetWateringTodayAsync_WhenNoBeds_ShouldReturnEmptyBeds()
    {
        var result = await _sut.GetWateringTodayAsync(_userId, DateOnly.FromDateTime(DateTime.UtcNow));
        result.Beds.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetWateringTodayAsync_WhenBedHasPlants_ShouldReturnBedWithPlantStatus()
    {
        var (plant, _) = SeedBedWithPlant(WaterNeeds.Low);

        var result = await _sut.GetWateringTodayAsync(_userId, DateOnly.FromDateTime(DateTime.UtcNow));

        result.Beds.Count.ShouldBe(1);
        result.Beds[0].Plants.Count.ShouldBe(1);
        result.Beds[0].Plants[0].PlantId.ShouldBe(plant.Id);
        result.Beds[0].IsPersonalPlants.ShouldBeFalse();
    }

    [Fact]
    public async Task GetWateringTodayAsync_WhenPlantIsToday_IsToday_ShouldBeTrue()
    {
        SeedBedWithPlant(WaterNeeds.High);
        var sunday = NextDayOfWeek(DayOfWeek.Sunday);

        var result = await _sut.GetWateringTodayAsync(_userId, sunday);

        result.Beds[0].Plants[0].IsToday.ShouldBeTrue();
    }

    [Fact]
    public async Task GetWateringTodayAsync_WhenOtherUser_ShouldReturnEmptyBeds()
    {
        SeedBedWithPlant(WaterNeeds.Low);
        var otherUser = Guid.NewGuid();
        DbContext.Users.Add(new User { Id = otherUser, Email = "other@example.com" });
        DbContext.SaveChanges();

        var result = await _sut.GetWateringTodayAsync(otherUser, DateOnly.FromDateTime(DateTime.UtcNow));

        result.Beds.ShouldBeEmpty();
    }

    private (Plant plant, Planting bed) SeedBedWithPlant(WaterNeeds waterNeeds)
    {
        var plant = new Plant { Id = Guid.NewGuid(), Key = $"plant-{Guid.NewGuid()}", Name = "Tomate", WaterNeeds = waterNeeds };
        var guild = new Guild { Id = Guid.NewGuid(), UserId = _userId, Name = "Guilde" };
        var bed = new Planting { Id = Guid.NewGuid(), GardenId = _gardenId, UserId = _userId, Name = "Planche", GuildId = guild.Id, CreatedAtUtc = DateTime.UtcNow };
        DbContext.Plants.Add(plant);
        DbContext.Guilds.Add(guild);
        DbContext.Plantings.Add(bed);
        DbContext.GuildPlants.Add(new GuildPlant { GuildId = guild.Id, PlantId = plant.Id });
        DbContext.SaveChanges();
        return (plant, bed);
    }

    private static DateOnly NextDayOfWeek(DayOfWeek day)
    {
        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        while (date.DayOfWeek != day) { date = date.AddDays(1); }
        return date;
    }
}
