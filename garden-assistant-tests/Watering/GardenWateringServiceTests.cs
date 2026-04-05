using GardenAssistant.Data.Entities;
using GardenAssistant.Data.Entities.Enums;
using GardenAssistant.Services.Watering;
using GardenAssistant.Tests.Infrastructure;
using Shouldly;

namespace GardenAssistant.Tests.Watering;

public class GardenWateringServiceTests : DatabaseTestBase
{
    private readonly GardenWateringService _sut;
    private readonly Guid _userId = Guid.NewGuid();
    private readonly Guid _gardenId = Guid.NewGuid();

    public GardenWateringServiceTests()
    {
        DbContext.Users.Add(new User { Id = _userId, Email = "test@example.com" });
        DbContext.Gardens.Add(new Garden { Id = _gardenId, Name = "Test", UserId = _userId, CreatedAtUtc = DateTime.UtcNow });
        DbContext.SaveChanges();
        _sut = new GardenWateringService(DbContext, new WateringCalculator());
    }

    [Fact]
    public async Task GetScheduleAsync_WhenNoBeds_ShouldReturnEmptyBeds()
    {
        var result = await _sut.GetScheduleAsync(_userId, _gardenId, halfMonth: 13);
        result.Beds.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetScheduleAsync_WhenBedHasPlants_ShouldReturnBedWithFrequency()
    {
        var plant = new Plant { Id = Guid.NewGuid(), Key = "tomate", Name = "Tomate", WaterNeeds = WaterNeeds.High };
        var guild = new Guild { Id = Guid.NewGuid(), UserId = _userId, Name = "G" };
        var bed = new Planting { Id = Guid.NewGuid(), GardenId = _gardenId, UserId = _userId, Name = "Planche", GuildId = guild.Id, CreatedAtUtc = DateTime.UtcNow };
        DbContext.Plants.Add(plant);
        DbContext.Guilds.Add(guild);
        DbContext.Plantings.Add(bed);
        DbContext.GuildPlants.Add(new GuildPlant { GuildId = guild.Id, PlantId = plant.Id });
        await DbContext.SaveChangesAsync();

        var result = await _sut.GetScheduleAsync(_userId, _gardenId, halfMonth: 13);

        result.Beds.Count.ShouldBe(1);
        result.Beds[0].BedId.ShouldBe(bed.Id);
        result.Beds[0].Plants.Count.ShouldBe(1);
        const int expectedTimesPerWeek = 5;
        result.Beds[0].Plants[0].TimesPerWeek.ShouldBe(expectedTimesPerWeek);
    }

    [Fact]
    public async Task GetScheduleAsync_WhenOtherGarden_ShouldReturnEmptyBeds()
    {
        var otherGardenId = Guid.NewGuid();
        DbContext.Gardens.Add(new Garden { Id = otherGardenId, Name = "Other", UserId = _userId, CreatedAtUtc = DateTime.UtcNow });
        var plant = new Plant { Id = Guid.NewGuid(), Key = "tomate-g", Name = "Tomate", WaterNeeds = WaterNeeds.High };
        var guild = new Guild { Id = Guid.NewGuid(), UserId = _userId, Name = "G" };
        var bed = new Planting { Id = Guid.NewGuid(), GardenId = otherGardenId, UserId = _userId, Name = "Planche", GuildId = guild.Id, CreatedAtUtc = DateTime.UtcNow };
        DbContext.Plants.Add(plant);
        DbContext.Guilds.Add(guild);
        DbContext.Plantings.Add(bed);
        DbContext.GuildPlants.Add(new GuildPlant { GuildId = guild.Id, PlantId = plant.Id });
        await DbContext.SaveChangesAsync();

        var result = await _sut.GetScheduleAsync(_userId, _gardenId, halfMonth: 13);
        result.Beds.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetScheduleAsync_WhenOtherUser_ShouldReturnEmptyBeds()
    {
        var otherUser = Guid.NewGuid();
        DbContext.Users.Add(new User { Id = otherUser, Email = "other@test.com" });
        DbContext.SaveChanges();

        var result = await _sut.GetScheduleAsync(otherUser, _gardenId, halfMonth: 13);
        result.Beds.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetScheduleAsync_WhenBedHasMulch_ShouldReduceFrequency()
    {
        var plant = new Plant { Id = Guid.NewGuid(), Key = "tomate-m", Name = "Tomate", WaterNeeds = WaterNeeds.High };
        var guild = new Guild { Id = Guid.NewGuid(), UserId = _userId, Name = "G" };
        var bed = new Planting { Id = Guid.NewGuid(), GardenId = _gardenId, UserId = _userId, Name = "Planche", GuildId = guild.Id, HasMulch = true, CreatedAtUtc = DateTime.UtcNow };
        DbContext.Plants.Add(plant);
        DbContext.Guilds.Add(guild);
        DbContext.Plantings.Add(bed);
        DbContext.GuildPlants.Add(new GuildPlant { GuildId = guild.Id, PlantId = plant.Id });
        await DbContext.SaveChangesAsync();

        var result = await _sut.GetScheduleAsync(_userId, _gardenId, halfMonth: 13);

        const int expectedTimesPerWeek = 3;
        result.Beds[0].Plants[0].TimesPerWeek.ShouldBe(expectedTimesPerWeek);
    }

    [Fact]
    public async Task GetScheduleAsync_WhenBedHasNoGuild_ShouldReturnEmptyBeds()
    {
        var bed = new Planting { Id = Guid.NewGuid(), GardenId = _gardenId, UserId = _userId, Name = "Planche", GuildId = null, CreatedAtUtc = DateTime.UtcNow };
        DbContext.Plantings.Add(bed);
        await DbContext.SaveChangesAsync();

        var result = await _sut.GetScheduleAsync(_userId, _gardenId, halfMonth: 13);
        result.Beds.ShouldBeEmpty();
    }
}
