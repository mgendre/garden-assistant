using GardenAssistant.Data.Entities;
using GardenAssistant.DTOs.Beds;
using GardenAssistant.Services;
using GardenAssistant.Tests.Infrastructure;
using Shouldly;

namespace GardenAssistant.Tests.Beds;

public class BedServiceTests : DatabaseTestBase
{
    private readonly BedService _sut;
    private readonly Guid _userId = Guid.NewGuid();
    private readonly Guid _gardenId = Guid.NewGuid();

    public BedServiceTests()
    {
        DbContext.Users.Add(new User { Id = _userId, Email = "test@example.com" });
        DbContext.Gardens.Add(new Garden
        {
            Id = _gardenId, Name = "Test Garden", UserId = _userId, CreatedAtUtc = DateTime.UtcNow
        });
        DbContext.SaveChanges();
        _sut = new BedService(DbContext);
    }

    [Fact]
    public async Task GetByGardenIdAsync_WhenNoBeds_ShouldReturnEmpty()
    {
        var result = await _sut.GetByGardenIdAsync(_gardenId, _userId);
        result.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetByGardenIdAsync_ShouldReturnBedsWithPlantIds()
    {
        var guild = new Guild { Id = Guid.NewGuid(), UserId = _userId, Name = "Bed Guild" };
        var plant = new Plant { Id = Guid.NewGuid(), Key = "tomato", Name = "Tomato" };
        DbContext.Guilds.Add(guild);
        DbContext.Plants.Add(plant);
        DbContext.GuildPlants.Add(new GuildPlant { GuildId = guild.Id, PlantId = plant.Id });
        DbContext.Plantings.Add(new Planting
        {
            Id = Guid.NewGuid(), GardenId = _gardenId, UserId = _userId,
            Name = "Planche 1", GuildId = guild.Id, CreatedAtUtc = DateTime.UtcNow
        });
        await DbContext.SaveChangesAsync();

        var result = (await _sut.GetByGardenIdAsync(_gardenId, _userId)).ToList();
        result.Count.ShouldBe(1);
        result[0].Name.ShouldBe("Planche 1");
        result[0].GuildId.ShouldBe(guild.Id);
        result[0].PlantIds.Count.ShouldBe(1);
        result[0].PlantIds[0].ShouldBe(plant.Id);
    }

    [Fact]
    public async Task GetByGardenIdAsync_WhenNotOwner_ShouldReturnEmpty()
    {
        var otherUserId = Guid.NewGuid();
        DbContext.Users.Add(new User { Id = otherUserId, Email = "other@example.com" });
        var result = await _sut.GetByGardenIdAsync(_gardenId, otherUserId);
        result.ShouldBeEmpty();
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateBedAndGuild()
    {
        var result = await _sut.CreateAsync(_gardenId, new CreateBedRequest("Planche tomates"), _userId);

        result.ShouldNotBeNull();
        result.Name.ShouldBe("Planche tomates");
        result.GuildId.ShouldNotBeNull();
        result.PlantIds.ShouldBeEmpty();
        DbContext.Plantings.Count().ShouldBe(1);
        DbContext.Guilds.Count().ShouldBe(1);
        var guild = DbContext.Guilds.First();
        guild.Name.ShouldBe("Planche tomates");
        guild.UserId.ShouldBe(_userId);
    }

    [Fact]
    public async Task CreateAsync_WhenNoName_ShouldUseEmptyString()
    {
        var result = await _sut.CreateAsync(_gardenId, new CreateBedRequest(null), _userId);
        result.ShouldNotBeNull();
        result.Name.ShouldBe("");
    }

    [Fact]
    public async Task CreateAsync_WhenGardenNotOwned_ShouldReturnNull()
    {
        var otherUserId = Guid.NewGuid();
        DbContext.Users.Add(new User { Id = otherUserId, Email = "other@example.com" });
        var result = await _sut.CreateAsync(_gardenId, new CreateBedRequest("Hacked"), otherUserId);
        result.ShouldBeNull();
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateBedAndGuildName()
    {
        var guild = new Guild { Id = Guid.NewGuid(), UserId = _userId, Name = "Old" };
        var bed = new Planting
        {
            Id = Guid.NewGuid(), GardenId = _gardenId, UserId = _userId,
            Name = "Old", GuildId = guild.Id, CreatedAtUtc = DateTime.UtcNow
        };
        DbContext.Guilds.Add(guild);
        DbContext.Plantings.Add(bed);
        await DbContext.SaveChangesAsync();

        var result = await _sut.UpdateAsync(_gardenId, bed.Id, new UpdateBedRequest("New"), _userId);
        result.ShouldNotBeNull();
        result.Name.ShouldBe("New");
        var updatedGuild = DbContext.Guilds.First();
        updatedGuild.Name.ShouldBe("New");
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteBedAndGuild()
    {
        var guild = new Guild { Id = Guid.NewGuid(), UserId = _userId, Name = "Bed Guild" };
        var bed = new Planting
        {
            Id = Guid.NewGuid(), GardenId = _gardenId, UserId = _userId,
            Name = "ToDelete", GuildId = guild.Id, CreatedAtUtc = DateTime.UtcNow
        };
        DbContext.Guilds.Add(guild);
        DbContext.Plantings.Add(bed);
        await DbContext.SaveChangesAsync();

        var result = await _sut.DeleteAsync(_gardenId, bed.Id, _userId);
        result.ShouldBeTrue();
        DbContext.Plantings.Count().ShouldBe(0);
        DbContext.Guilds.Count().ShouldBe(0);
    }

    [Fact]
    public async Task DeleteAsync_WhenNotOwner_ShouldReturnFalse()
    {
        var otherUserId = Guid.NewGuid();
        DbContext.Users.Add(new User { Id = otherUserId, Email = "other@example.com" });
        var bed = new Planting
        {
            Id = Guid.NewGuid(), GardenId = _gardenId, UserId = _userId,
            Name = "Protected", CreatedAtUtc = DateTime.UtcNow
        };
        DbContext.Plantings.Add(bed);
        await DbContext.SaveChangesAsync();

        var result = await _sut.DeleteAsync(_gardenId, bed.Id, otherUserId);
        result.ShouldBeFalse();
    }
}
