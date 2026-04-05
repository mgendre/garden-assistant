using GardenAssistant.Data.Entities;
using GardenAssistant.Data.Entities.Enums;
using GardenAssistant.Services.Watering;
using GardenAssistant.Tests.Infrastructure;
using Shouldly;

namespace GardenAssistant.Tests.Watering;

public class WateringServiceScheduleTests : DatabaseTestBase
{
    private readonly WateringService _sut;
    private readonly Guid _userId = Guid.NewGuid();
    private readonly Guid _gardenId = Guid.NewGuid();

    public WateringServiceScheduleTests()
    {
        DbContext.Users.Add(new User { Id = _userId, Email = "test@example.com" });
        DbContext.Gardens.Add(new Garden { Id = _gardenId, Name = "Test", UserId = _userId, CreatedAtUtc = DateTime.UtcNow });
        DbContext.SaveChanges();
        _sut = new WateringService(DbContext, new WateringCalculator());
    }

    [Fact]
    public async Task GetWateringScheduleAsync_WhenNoBeds_ShouldReturnEmptyBeds()
    {
        var result = await _sut.GetWateringScheduleAsync(_userId, halfMonth: 13, source: "gardenPlants");
        result.Beds.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetWateringScheduleAsync_WhenBedHasPlants_ShouldReturnBedWithFrequency()
    {
        var plant = new Plant { Id = Guid.NewGuid(), Key = "tomate", Name = "Tomate", WaterNeeds = WaterNeeds.High };
        var guild = new Guild { Id = Guid.NewGuid(), UserId = _userId, Name = "G" };
        var bed = new Planting { Id = Guid.NewGuid(), GardenId = _gardenId, UserId = _userId, Name = "Planche", GuildId = guild.Id, CreatedAtUtc = DateTime.UtcNow };
        DbContext.Plants.Add(plant);
        DbContext.Guilds.Add(guild);
        DbContext.Plantings.Add(bed);
        DbContext.GuildPlants.Add(new GuildPlant { GuildId = guild.Id, PlantId = plant.Id });
        await DbContext.SaveChangesAsync();

        var result = await _sut.GetWateringScheduleAsync(_userId, halfMonth: 13, source: "gardenPlants");

        result.Beds.Count.ShouldBe(1);
        result.Beds[0].BedId.ShouldBe(bed.Id);
        result.Beds[0].Plants.Count.ShouldBe(1);
        result.Beds[0].Plants[0].TimesPerWeek.ShouldBe(5);
        result.Beds[0].Plants[0].RecommendedDays.Length.ShouldBe(5);
    }

    [Fact]
    public async Task GetWateringScheduleAsync_WhenSourceIsMyPlants_ShouldReturnPersonalPlantsBed()
    {
        var plant = new Plant { Id = Guid.NewGuid(), Key = "laitue", Name = "Laitue", WaterNeeds = WaterNeeds.Medium };
        DbContext.Plants.Add(plant);
        DbContext.UserPlants.Add(new UserPlant { UserId = _userId, PlantId = plant.Id });
        await DbContext.SaveChangesAsync();

        var result = await _sut.GetWateringScheduleAsync(_userId, halfMonth: 13, source: "myPlants");

        result.Beds.Count.ShouldBe(1);
        result.Beds[0].IsPersonalPlants.ShouldBeTrue();
        result.Beds[0].BedId.ShouldBeNull();
        result.Beds[0].Plants.Count.ShouldBe(1);
    }

    [Fact]
    public async Task GetWateringScheduleAsync_WhenSourceIsAll_ShouldReturnGardenAndPersonalBeds()
    {
        var plant1 = new Plant { Id = Guid.NewGuid(), Key = "tomate-all", Name = "Tomate", WaterNeeds = WaterNeeds.High };
        var guild = new Guild { Id = Guid.NewGuid(), UserId = _userId, Name = "G" };
        var bed = new Planting { Id = Guid.NewGuid(), GardenId = _gardenId, UserId = _userId, Name = "Planche", GuildId = guild.Id, CreatedAtUtc = DateTime.UtcNow };
        var plant2 = new Plant { Id = Guid.NewGuid(), Key = "laitue-all", Name = "Laitue", WaterNeeds = WaterNeeds.Low };
        DbContext.Plants.AddRange(plant1, plant2);
        DbContext.Guilds.Add(guild);
        DbContext.Plantings.Add(bed);
        DbContext.GuildPlants.Add(new GuildPlant { GuildId = guild.Id, PlantId = plant1.Id });
        DbContext.UserPlants.Add(new UserPlant { UserId = _userId, PlantId = plant2.Id });
        await DbContext.SaveChangesAsync();

        var result = await _sut.GetWateringScheduleAsync(_userId, halfMonth: 13, source: "all");

        result.Beds.Count.ShouldBe(2);
        result.Beds.Any(b => !b.IsPersonalPlants).ShouldBeTrue();
        result.Beds.Any(b => b.IsPersonalPlants).ShouldBeTrue();
    }

    [Fact]
    public async Task GetWateringScheduleAsync_WhenOtherUser_ShouldReturnEmptyBeds()
    {
        var otherUser = Guid.NewGuid();
        DbContext.Users.Add(new User { Id = otherUser, Email = "other@test.com" });
        DbContext.SaveChanges();

        var result = await _sut.GetWateringScheduleAsync(otherUser, halfMonth: 13, source: "gardenPlants");
        result.Beds.ShouldBeEmpty();
    }
}
