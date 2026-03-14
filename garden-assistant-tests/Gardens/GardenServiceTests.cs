using GardenAssistant.DTOs;
using GardenAssistant.Entities;
using GardenAssistant.Services;
using GardenAssistant.Tests.Infrastructure;
using Shouldly;

namespace GardenAssistant.Tests.Gardens;

public class GardenServiceTests : DatabaseTestBase
{
    private readonly GardenService _sut;
    private static readonly Guid DefaultUserId = new("00000000-0000-0000-0000-000000000001");

    public GardenServiceTests()
    {
        // The default user (DefaultUserId) is already present because
        // DatabaseTestBase.EnsureCreated() applies HasData from AppDbContext.
        // No explicit seeding needed here.
        _sut = new GardenService(DbContext);
    }

    [Fact]
    public async Task GetAllAsync_WhenNoGardens_ShouldReturnEmpty()
    {
        var result = await _sut.GetAllAsync();

        result.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetAllAsync_WhenGardensExist_ShouldReturnAll()
    {
        DbContext.Gardens.Add(new Garden
        {
            Id = Guid.NewGuid(),
            Name = "My Garden",
            Description = "Lovely",
            UserId = DefaultUserId
        });
        await DbContext.SaveChangesAsync();

        var result = await _sut.GetAllAsync();

        result.Count().ShouldBe(1);
        result.First().Name.ShouldBe("My Garden");
    }

    [Fact]
    public async Task CreateAsync_WhenValidRequest_ShouldPersistAndReturnGarden()
    {
        var request = new CreateGardenRequest("Rose Garden", "Full of roses", DefaultUserId);

        var result = await _sut.CreateAsync(request);

        result.Id.ShouldNotBe(Guid.Empty);
        result.Name.ShouldBe("Rose Garden");
        result.Description.ShouldBe("Full of roses");
        result.UserId.ShouldBe(DefaultUserId);
        DbContext.Gardens.Count().ShouldBe(1);
    }

    [Fact]
    public async Task UpdateAsync_WhenGardenExists_ShouldUpdateAndReturn()
    {
        var gardenId = Guid.NewGuid();
        DbContext.Gardens.Add(new Garden { Id = gardenId, Name = "Old Name", UserId = DefaultUserId });
        await DbContext.SaveChangesAsync();

        var result = await _sut.UpdateAsync(gardenId, new UpdateGardenRequest("New Name", "New Desc"));

        result.ShouldNotBeNull();
        result.Name.ShouldBe("New Name");
        result.Description.ShouldBe("New Desc");
    }

    [Fact]
    public async Task UpdateAsync_WhenGardenNotFound_ShouldReturnNull()
    {
        var result = await _sut.UpdateAsync(Guid.NewGuid(), new UpdateGardenRequest("Name", null));

        result.ShouldBeNull();
    }

    [Fact]
    public async Task DeleteAsync_WhenGardenExists_ShouldRemoveAndReturnTrue()
    {
        var gardenId = Guid.NewGuid();
        DbContext.Gardens.Add(new Garden { Id = gardenId, Name = "Garden", UserId = DefaultUserId });
        await DbContext.SaveChangesAsync();

        var result = await _sut.DeleteAsync(gardenId);

        result.ShouldBeTrue();
        DbContext.Gardens.Count().ShouldBe(0);
    }

    [Fact]
    public async Task DeleteAsync_WhenGardenNotFound_ShouldReturnFalse()
    {
        var result = await _sut.DeleteAsync(Guid.NewGuid());

        result.ShouldBeFalse();
    }
}
