using GardenAssistant.Data.Entities;
using GardenAssistant.Data.Entities.Enums;
using GardenAssistant.DTOs;
using GardenAssistant.Services;
using GardenAssistant.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace GardenAssistant.Tests.Plants;

public class PlantServiceTests : DatabaseTestBase
{
    private readonly PlantService _sut;

    public PlantServiceTests()
    {
        _sut = new PlantService(DbContext);
    }

    [Fact]
    public async Task GetAllAsync_WhenNoPlantsExist_ShouldReturnEmptyWithZeroCount()
    {
        var result = await _sut.GetAllAsync();

        result.Items.ShouldBeEmpty();
        result.TotalCount.ShouldBe(0);
    }

    [Fact]
    public async Task GetAllAsync_WhenPlantsExist_ShouldReturnAllWithCount()
    {
        DbContext.Plants.Add(new Plant { Id = Guid.NewGuid(), Name = "Tomato" });
        DbContext.Plants.Add(new Plant { Id = Guid.NewGuid(), Name = "Basil" });
        await DbContext.SaveChangesAsync();

        var result = await _sut.GetAllAsync();

        result.Items.Count().ShouldBe(2);
        result.TotalCount.ShouldBe(2);
    }

    [Fact]
    public async Task GetByIdAsync_WhenPlantExists_ShouldReturnDto()
    {
        var plantId = Guid.NewGuid();
        DbContext.Plants.Add(new Plant { Id = plantId, Name = "Mint" });
        await DbContext.SaveChangesAsync();

        var result = await _sut.GetByIdAsync(plantId);

        result.ShouldNotBeNull();
        result.Id.ShouldBe(plantId);
        result.Name.ShouldBe("Mint");
    }

    [Fact]
    public async Task GetByIdAsync_WhenPlantDoesNotExist_ShouldReturnNull()
    {
        var result = await _sut.GetByIdAsync(Guid.NewGuid());

        result.ShouldBeNull();
    }

    [Fact]
    public async Task CreateAsync_WhenValidRequest_ShouldPersistAndReturnDto()
    {
        var request = new CreatePlantRequest(
            "Rosemary",
            "Salvia rosmarinus",
            "Aromatic herb",
            "Lamiaceae",
            "Salvia",
            LifeCycle.Perennial,
            100,
            RootDepth.Medium,
            SunRequirement.FullSun,
            WaterNeeds.Low,
            false,
            false,
            true
        );

        var result = await _sut.CreateAsync(request);

        result.Id.ShouldNotBe(Guid.Empty);
        result.Name.ShouldBe("Rosemary");
        result.ScientificName.ShouldBe("Salvia rosmarinus");
        result.LifeCycle.ShouldBe(LifeCycle.Perennial);
        result.PollinatorPlant.ShouldBeTrue();
        (await DbContext.Plants.CountAsync()).ShouldBe(1);
    }

    [Fact]
    public async Task DeleteAsync_WhenPlantExists_ShouldRemoveAndReturnTrue()
    {
        var plantId = Guid.NewGuid();
        DbContext.Plants.Add(new Plant { Id = plantId, Name = "Lavender" });
        await DbContext.SaveChangesAsync();

        var result = await _sut.DeleteAsync(plantId);

        result.ShouldBeTrue();
        (await DbContext.Plants.CountAsync()).ShouldBe(0);
    }

    [Fact]
    public async Task DeleteAsync_WhenPlantDoesNotExist_ShouldReturnFalse()
    {
        var result = await _sut.DeleteAsync(Guid.NewGuid());

        result.ShouldBeFalse();
    }

    [Fact]
    public async Task GetAllAsync_WhenSearchMatchesName_ShouldReturnMatch()
    {
        DbContext.Plants.Add(new Plant { Id = Guid.NewGuid(), Name = "Tomate" });
        DbContext.Plants.Add(new Plant { Id = Guid.NewGuid(), Name = "Basilic" });
        await DbContext.SaveChangesAsync();

        var result = await _sut.GetAllAsync("tom");

        result.Items.Count().ShouldBe(1);
        result.Items.First().Name.ShouldBe("Tomate");
        result.TotalCount.ShouldBe(1);
    }

    [Fact]
    public async Task GetAllAsync_WhenSearchMatchesScientificName_ShouldReturnMatch()
    {
        DbContext.Plants.Add(new Plant { Id = Guid.NewGuid(), Name = "Tomate", ScientificName = "Solanum lycopersicum" });
        await DbContext.SaveChangesAsync();

        var result = await _sut.GetAllAsync("solanum");

        result.Items.Count().ShouldBe(1);
        result.Items.First().Name.ShouldBe("Tomate");
        result.TotalCount.ShouldBe(1);
    }

    [Fact]
    public async Task GetAllAsync_WhenSearchIsCaseInsensitive_ShouldReturnMatch()
    {
        DbContext.Plants.Add(new Plant { Id = Guid.NewGuid(), Name = "Tomate" });
        await DbContext.SaveChangesAsync();

        var result = await _sut.GetAllAsync("TOMATE");

        result.Items.Count().ShouldBe(1);
        result.TotalCount.ShouldBe(1);
    }

    [Fact]
    public async Task GetAllAsync_WhenSearchHasNoMatch_ShouldReturnEmptyWithZeroCount()
    {
        DbContext.Plants.Add(new Plant { Id = Guid.NewGuid(), Name = "Tomate" });
        await DbContext.SaveChangesAsync();

        var result = await _sut.GetAllAsync("xyz");

        result.Items.ShouldBeEmpty();
        result.TotalCount.ShouldBe(0);
    }

    [Fact]
    public async Task GetAllAsync_WhenSearchExceedsLimit_ShouldReturnTotalCountGreaterThanItems()
    {
        for (var i = 0; i < 25; i++)
        {
            DbContext.Plants.Add(new Plant { Id = Guid.NewGuid(), Name = $"Plant {i}" });
        }
        await DbContext.SaveChangesAsync();

        var result = await _sut.GetAllAsync("Plant");

        result.Items.Count().ShouldBe(20);
        result.TotalCount.ShouldBe(25);
    }
}
