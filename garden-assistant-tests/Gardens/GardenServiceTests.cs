using GardenAssistant.DTOs;
using GardenAssistant.Data.Entities;
using GardenAssistant.Services;
using GardenAssistant.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace GardenAssistant.Tests.Gardens;

public class GardenServiceTests : DatabaseTestBase
{
    private readonly GardenService _sut;
    private static readonly Guid DefaultUserId = new("00000000-0000-0000-0000-000000000001");
    private static readonly Guid OtherUserId = Guid.NewGuid();

    public GardenServiceTests()
    {
        _sut = new GardenService(DbContext);
    }

    [Fact]
    public async Task GetAllAsync_WhenNoGardens_ShouldReturnEmpty()
    {
        var result = await _sut.GetAllAsync(DefaultUserId);

        result.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetAllAsync_WhenGardensExist_ShouldReturnOnlyCallerGardens()
    {
        DbContext.Users.Add(new User { Id = OtherUserId, Email = "other@example.com" });
        DbContext.Gardens.Add(new Garden
        {
            Id = Guid.NewGuid(),
            Name = "My Garden",
            Description = "Lovely",
            UserId = DefaultUserId
        });
        DbContext.Gardens.Add(new Garden
        {
            Id = Guid.NewGuid(),
            Name = "Other Garden",
            UserId = OtherUserId
        });
        await DbContext.SaveChangesAsync();

        var result = await _sut.GetAllAsync(DefaultUserId);
        var gardenDtos = result as GardenDto[] ?? result.ToArray();
        gardenDtos.Length.ShouldBe(1);
        gardenDtos.First().Name.ShouldBe("My Garden");
    }

    [Fact]
    public async Task CreateAsync_WhenValidRequest_ShouldPersistAndReturnGarden()
    {
        var request = new CreateGardenRequest("Rose Garden", "Full of roses");

        var result = await _sut.CreateAsync(request, DefaultUserId);

        result.Id.ShouldNotBe(Guid.Empty);
        result.Name.ShouldBe("Rose Garden");
        result.Description.ShouldBe("Full of roses");
        (await DbContext.Gardens.CountAsync()).ShouldBe(1);
        (await DbContext.Gardens.SingleAsync()).UserId.ShouldBe(DefaultUserId);
    }

    [Fact]
    public async Task UpdateAsync_WhenGardenExistsAndBelongsToCaller_ShouldUpdateAndReturn()
    {
        var gardenId = Guid.NewGuid();
        DbContext.Gardens.Add(new Garden { Id = gardenId, Name = "Old Name", UserId = DefaultUserId });
        await DbContext.SaveChangesAsync();

        var result = await _sut.UpdateAsync(gardenId, new UpdateGardenRequest("New Name", "New Desc"), DefaultUserId);

        result.ShouldNotBeNull();
        result.Name.ShouldBe("New Name");
        result.Description.ShouldBe("New Desc");
    }

    [Fact]
    public async Task UpdateAsync_WhenGardenBelongsToAnotherUser_ShouldReturnNull()
    {
        var gardenId = Guid.NewGuid();
        DbContext.Users.Add(new User { Id = OtherUserId, Email = "other@example.com" });
        DbContext.Gardens.Add(new Garden { Id = gardenId, Name = "Other Garden", UserId = OtherUserId });
        await DbContext.SaveChangesAsync();

        var result = await _sut.UpdateAsync(gardenId, new UpdateGardenRequest("New Name", null), DefaultUserId);

        result.ShouldBeNull();
    }

    [Fact]
    public async Task UpdateAsync_WhenGardenNotFound_ShouldReturnNull()
    {
        var result = await _sut.UpdateAsync(Guid.NewGuid(), new UpdateGardenRequest("Name", null), DefaultUserId);

        result.ShouldBeNull();
    }

    [Fact]
    public async Task DeleteAsync_WhenGardenExistsAndBelongsToCaller_ShouldRemoveAndReturnTrue()
    {
        var gardenId = Guid.NewGuid();
        DbContext.Gardens.Add(new Garden { Id = gardenId, Name = "Garden", UserId = DefaultUserId });
        await DbContext.SaveChangesAsync();

        var result = await _sut.DeleteAsync(gardenId, DefaultUserId);

        result.ShouldBeTrue();
        (await DbContext.Gardens.CountAsync()).ShouldBe(0);
    }

    [Fact]
    public async Task DeleteAsync_WhenGardenBelongsToAnotherUser_ShouldReturnFalse()
    {
        var gardenId = Guid.NewGuid();
        DbContext.Users.Add(new User { Id = OtherUserId, Email = "other@example.com" });
        DbContext.Gardens.Add(new Garden { Id = gardenId, Name = "Garden", UserId = OtherUserId });
        await DbContext.SaveChangesAsync();

        var result = await _sut.DeleteAsync(gardenId, DefaultUserId);

        result.ShouldBeFalse();
        (await DbContext.Gardens.CountAsync()).ShouldBe(1);
    }

    [Fact]
    public async Task DeleteAsync_WhenGardenNotFound_ShouldReturnFalse()
    {
        var result = await _sut.DeleteAsync(Guid.NewGuid(), DefaultUserId);

        result.ShouldBeFalse();
    }
}
