using GardenAssistant.Data.Entities;
using GardenAssistant.DTOs.Gardens;
using GardenAssistant.Services;
using GardenAssistant.Tests.Infrastructure;
using Shouldly;

namespace GardenAssistant.Tests.Gardens;

public class GardenServiceTests : DatabaseTestBase
{
    private readonly GardenService _sut;
    private readonly Guid _userId = Guid.NewGuid();

    public GardenServiceTests()
    {
        DbContext.Users.Add(new User { Id = _userId, Email = "test@example.com" });
        DbContext.SaveChanges();
        _sut = new GardenService(DbContext);
    }

    [Fact]
    public async Task GetAllAsync_WhenNoGardens_ShouldReturnEmpty()
    {
        var result = await _sut.GetAllAsync(_userId);
        result.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetAllAsync_WhenGardensExist_ShouldReturnWithBedCount()
    {
        var garden = new Garden
        {
            Id = Guid.NewGuid(), Name = "Potager", Description = "Mon potager",
            UserId = _userId, CreatedAtUtc = DateTime.UtcNow
        };
        DbContext.Gardens.Add(garden);
        DbContext.Plantings.Add(new Planting
        {
            Id = Guid.NewGuid(), GardenId = garden.Id, UserId = _userId,
            Name = "Planche 1", CreatedAtUtc = DateTime.UtcNow
        });
        await DbContext.SaveChangesAsync();

        var result = (await _sut.GetAllAsync(_userId)).ToList();
        result.Count.ShouldBe(1);
        result[0].Name.ShouldBe("Potager");
        result[0].BedCount.ShouldBe(1);
    }

    [Fact]
    public async Task GetAllAsync_ShouldOnlyReturnUserGardens()
    {
        var otherUserId = Guid.NewGuid();
        DbContext.Users.Add(new User { Id = otherUserId, Email = "other@example.com" });
        DbContext.Gardens.AddRange(
            new Garden { Id = Guid.NewGuid(), Name = "Mine", UserId = _userId, CreatedAtUtc = DateTime.UtcNow },
            new Garden { Id = Guid.NewGuid(), Name = "Theirs", UserId = otherUserId, CreatedAtUtc = DateTime.UtcNow }
        );
        await DbContext.SaveChangesAsync();

        var result = (await _sut.GetAllAsync(_userId)).ToList();
        result.Count.ShouldBe(1);
        result[0].Name.ShouldBe("Mine");
    }

    [Fact]
    public async Task GetAllAsync_ShouldSortByCreatedAtDescending()
    {
        DbContext.Gardens.AddRange(
            new Garden { Id = Guid.NewGuid(), Name = "Old", UserId = _userId, CreatedAtUtc = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
            new Garden { Id = Guid.NewGuid(), Name = "New", UserId = _userId, CreatedAtUtc = new DateTime(2026, 3, 1, 0, 0, 0, DateTimeKind.Utc) }
        );
        await DbContext.SaveChangesAsync();

        var result = (await _sut.GetAllAsync(_userId)).ToList();
        result[0].Name.ShouldBe("New");
        result[1].Name.ShouldBe("Old");
    }

    [Fact]
    public async Task CreateAsync_ShouldPersistAndReturnGarden()
    {
        var request = new CreateGardenRequest("Potager", "Description");
        var result = await _sut.CreateAsync(request, _userId);

        result.Name.ShouldBe("Potager");
        result.Description.ShouldBe("Description");
        result.BedCount.ShouldBe(0);
        result.Id.ShouldNotBe(Guid.Empty);
    }

    [Fact]
    public async Task UpdateAsync_WhenOwner_ShouldUpdateAndReturn()
    {
        var garden = new Garden { Id = Guid.NewGuid(), Name = "Old", UserId = _userId, CreatedAtUtc = DateTime.UtcNow };
        DbContext.Gardens.Add(garden);
        await DbContext.SaveChangesAsync();

        var result = await _sut.UpdateAsync(garden.Id, new UpdateGardenRequest("New", "Desc"), _userId);
        result.ShouldNotBeNull();
        result.Name.ShouldBe("New");
        result.Description.ShouldBe("Desc");
    }

    [Fact]
    public async Task UpdateAsync_WhenNotOwner_ShouldReturnNull()
    {
        var otherUserId = Guid.NewGuid();
        DbContext.Users.Add(new User { Id = otherUserId, Email = "other@example.com" });
        var garden = new Garden { Id = Guid.NewGuid(), Name = "Theirs", UserId = otherUserId, CreatedAtUtc = DateTime.UtcNow };
        DbContext.Gardens.Add(garden);
        await DbContext.SaveChangesAsync();

        var result = await _sut.UpdateAsync(garden.Id, new UpdateGardenRequest("Hacked", null), _userId);
        result.ShouldBeNull();
    }

    [Fact]
    public async Task DeleteAsync_WhenOwner_ShouldDeleteAndReturnTrue()
    {
        var garden = new Garden { Id = Guid.NewGuid(), Name = "ToDelete", UserId = _userId, CreatedAtUtc = DateTime.UtcNow };
        DbContext.Gardens.Add(garden);
        await DbContext.SaveChangesAsync();

        var result = await _sut.DeleteAsync(garden.Id, _userId);
        result.ShouldBeTrue();
        DbContext.Gardens.Count().ShouldBe(0);
    }

    [Fact]
    public async Task DeleteAsync_ShouldCleanUpBedGuilds()
    {
        var garden = new Garden { Id = Guid.NewGuid(), Name = "WithBeds", UserId = _userId, CreatedAtUtc = DateTime.UtcNow };
        var guild = new Guild { Id = Guid.NewGuid(), UserId = _userId, Name = "Bed Guild" };
        var bed = new Planting
        {
            Id = Guid.NewGuid(), GardenId = garden.Id, UserId = _userId,
            Name = "Planche", GuildId = guild.Id, CreatedAtUtc = DateTime.UtcNow
        };
        DbContext.Gardens.Add(garden);
        DbContext.Guilds.Add(guild);
        DbContext.Plantings.Add(bed);
        await DbContext.SaveChangesAsync();

        await _sut.DeleteAsync(garden.Id, _userId);
        DbContext.Gardens.Count().ShouldBe(0);
        DbContext.Guilds.Count().ShouldBe(0);
    }

    [Fact]
    public async Task DeleteAsync_WhenNotOwner_ShouldReturnFalse()
    {
        var otherUserId = Guid.NewGuid();
        DbContext.Users.Add(new User { Id = otherUserId, Email = "other@example.com" });
        var garden = new Garden { Id = Guid.NewGuid(), Name = "Theirs", UserId = otherUserId, CreatedAtUtc = DateTime.UtcNow };
        DbContext.Gardens.Add(garden);
        await DbContext.SaveChangesAsync();

        var result = await _sut.DeleteAsync(garden.Id, _userId);
        result.ShouldBeFalse();
    }
}
