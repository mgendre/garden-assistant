using GardenAssistant.Data.Entities;
using GardenAssistant.Data.Entities.Enums;
using GardenAssistant.DTOs;
using GardenAssistant.Services;
using GardenAssistant.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace GardenAssistant.Tests.Plantings;

public class PlantingEntryServiceTests : DatabaseTestBase
{
    private readonly PlantingEntryService _sut;
    private static readonly Guid DefaultUserId = new("00000000-0000-0000-0000-000000000001");
    private static readonly Guid OtherUserId = Guid.NewGuid();

    public PlantingEntryServiceTests()
    {
        _sut = new PlantingEntryService(DbContext);
    }

    private async Task<Guid> SeedPlantingAsync(Guid userId)
    {
        var gardenId = Guid.NewGuid();
        DbContext.Gardens.Add(new Garden { Id = gardenId, Name = "Test Garden", UserId = userId });
        var plantingId = Guid.NewGuid();
        DbContext.Plantings.Add(new Planting
        {
            Id = plantingId,
            GardenId = gardenId,
            UserId = userId,
            Name = "Test Planting"
        });
        await DbContext.SaveChangesAsync();
        return plantingId;
    }

    private async Task<Guid> SeedPlantAsync()
    {
        var plantId = Guid.NewGuid();
        DbContext.Plants.Add(new Plant { Id = plantId, Name = "Sage" });
        await DbContext.SaveChangesAsync();
        return plantId;
    }

    private async Task<Guid> SeedEntryAsync(Guid plantingId, Guid plantId)
    {
        var entryId = Guid.NewGuid();
        DbContext.PlantingEntries.Add(new PlantingEntry
        {
            Id = entryId,
            PlantingId = plantingId,
            PlantId = plantId,
            Quantity = 3
        });
        await DbContext.SaveChangesAsync();
        return entryId;
    }

    [Fact]
    public async Task GetForPlantingAsync_WhenEntriesExist_ShouldReturnAll()
    {
        var plantingId = await SeedPlantingAsync(DefaultUserId);
        var plantId = await SeedPlantAsync();
        await SeedEntryAsync(plantingId, plantId);

        var result = await _sut.GetForPlantingAsync(plantingId, DefaultUserId);

        var plantingEntryDtos = result as PlantingEntryDto[] ?? result?.ToArray();
        plantingEntryDtos.ShouldNotBeNull();
        plantingEntryDtos.Length.ShouldBe(1);
        plantingEntryDtos.First().PlantingId.ShouldBe(plantingId);
    }

    [Fact]
    public async Task GetForPlantingAsync_WhenPlantingBelongsToDifferentUser_ShouldReturnNull()
    {
        DbContext.Users.Add(new User { Id = OtherUserId, Email = "other@example.com" });
        await DbContext.SaveChangesAsync();
        var plantingId = await SeedPlantingAsync(OtherUserId);

        var result = await _sut.GetForPlantingAsync(plantingId, DefaultUserId);

        result.ShouldBeNull();
    }

    [Fact]
    public async Task AddEntryAsync_WhenValidRequest_ShouldPersistAndReturnDto()
    {
        var plantingId = await SeedPlantingAsync(DefaultUserId);
        var plantId = await SeedPlantAsync();
        var request = new CreatePlantingEntryRequest(
            plantId,
            5,
            1.0f,
            2.0f,
            PlantingLayer.Herbaceous,
            new DateOnly(2026, 3, 15),
            new DateOnly(2026, 7, 1),
            "Sow early"
        );

        var result = await _sut.AddEntryAsync(plantingId, request, DefaultUserId);

        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(Guid.Empty);
        result.PlantingId.ShouldBe(plantingId);
        result.PlantId.ShouldBe(plantId);
        result.Quantity.ShouldBe(5);
        result.Layer.ShouldBe(PlantingLayer.Herbaceous);
        result.Notes.ShouldBe("Sow early");
        (await DbContext.PlantingEntries.CountAsync()).ShouldBe(1);
    }

    [Fact]
    public async Task AddEntryAsync_WhenPlantingBelongsToDifferentUser_ShouldReturnNull()
    {
        DbContext.Users.Add(new User { Id = OtherUserId, Email = "other@example.com" });
        await DbContext.SaveChangesAsync();
        var plantingId = await SeedPlantingAsync(OtherUserId);
        var plantId = await SeedPlantAsync();
        var request = new CreatePlantingEntryRequest(plantId, null, null, null, null, null, null, null);

        var result = await _sut.AddEntryAsync(plantingId, request, DefaultUserId);

        result.ShouldBeNull();
        (await DbContext.PlantingEntries.CountAsync()).ShouldBe(0);
    }

    [Fact]
    public async Task RemoveEntryAsync_WhenEntryExists_ShouldRemoveAndReturnTrue()
    {
        var plantingId = await SeedPlantingAsync(DefaultUserId);
        var plantId = await SeedPlantAsync();
        var entryId = await SeedEntryAsync(plantingId, plantId);

        var result = await _sut.RemoveEntryAsync(entryId, DefaultUserId);

        result.ShouldBeTrue();
        (await DbContext.PlantingEntries.CountAsync()).ShouldBe(0);
    }

    [Fact]
    public async Task RemoveEntryAsync_WhenEntryBelongsToDifferentUser_ShouldReturnFalse()
    {
        DbContext.Users.Add(new User { Id = OtherUserId, Email = "other@example.com" });
        await DbContext.SaveChangesAsync();
        var plantingId = await SeedPlantingAsync(OtherUserId);
        var plantId = await SeedPlantAsync();
        var entryId = await SeedEntryAsync(plantingId, plantId);

        var result = await _sut.RemoveEntryAsync(entryId, DefaultUserId);

        result.ShouldBeFalse();
        (await DbContext.PlantingEntries.CountAsync()).ShouldBe(1);
    }

    [Fact]
    public async Task RemoveEntryAsync_WhenEntryDoesNotExist_ShouldReturnFalse()
    {
        var result = await _sut.RemoveEntryAsync(Guid.NewGuid(), DefaultUserId);

        result.ShouldBeFalse();
    }
}
