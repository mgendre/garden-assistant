using GardenAssistant.Data.Entities;
using GardenAssistant.Data.Entities.Enums;
using GardenAssistant.Services;
using GardenAssistant.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace GardenAssistant.Tests.UserPlants;

public class UserPlantServiceTests : DatabaseTestBase
{
    private readonly UserPlantService _sut;
    private static readonly Guid DefaultUserId = new("00000000-0000-0000-0000-000000000001");

    public UserPlantServiceTests()
    {
        _sut = new UserPlantService(DbContext);
    }

    private static Plant CreatePlant(Guid? id = null, string name = "Tomato") => new()
    {
        Id = id ?? Guid.NewGuid(),
        Name = name,
        ScientificName = "Solanum lycopersicum",
        Family = "Solanaceae",
        Genus = "Solanum",
        LifeCycle = LifeCycle.Annual,
        RootDepth = RootDepth.Medium,
        SunRequirement = SunRequirement.FullSun,
        WaterNeeds = WaterNeeds.Medium,
        NitrogenFixer = false,
        AllelopathicRisk = false,
        PollinatorPlant = false
    };

    private async Task SeedUserPlantAsync(Guid plantId, Guid userId, DateTime? addedAtUtc = null)
    {
        DbContext.UserPlants.Add(new UserPlant
        {
            UserId = userId,
            PlantId = plantId,
            AddedAtUtc = addedAtUtc ?? DateTime.UtcNow
        });
        await DbContext.SaveChangesAsync();
    }

    [Fact]
    public async Task GetAllAsync_WhenUserHasPlants_ShouldReturnPlantDtos()
    {
        var plantA = CreatePlant(name: "Tomato");
        var plantB = CreatePlant(name: "Basil");
        DbContext.Plants.AddRange(plantA, plantB);
        await DbContext.SaveChangesAsync();
        await SeedUserPlantAsync(plantA.Id, DefaultUserId, DateTime.UtcNow.AddMinutes(-10));
        await SeedUserPlantAsync(plantB.Id, DefaultUserId, DateTime.UtcNow);

        var result = (await _sut.GetAllAsync(DefaultUserId)).ToList();

        result.Count.ShouldBe(2);
        result[0].Name.ShouldBe("Basil");
        result[1].Name.ShouldBe("Tomato");
    }

    [Fact]
    public async Task GetAllAsync_WhenUserHasNoPlants_ShouldReturnEmpty()
    {
        var plant = CreatePlant();
        DbContext.Plants.Add(plant);
        await DbContext.SaveChangesAsync();

        var result = await _sut.GetAllAsync(DefaultUserId);

        result.ShouldBeEmpty();
    }

    [Fact]
    public async Task AddAsync_WhenPlantExists_ShouldReturnPlantDto()
    {
        var plant = CreatePlant(name: "Mint");
        DbContext.Plants.Add(plant);
        await DbContext.SaveChangesAsync();

        var result = await _sut.AddAsync(plant.Id, DefaultUserId);

        result.ShouldNotBeNull();
        result.Id.ShouldBe(plant.Id);
        result.Name.ShouldBe("Mint");
        (await DbContext.UserPlants.CountAsync()).ShouldBe(1);
        var saved = await DbContext.UserPlants.SingleAsync();
        saved.UserId.ShouldBe(DefaultUserId);
        saved.PlantId.ShouldBe(plant.Id);
    }

    [Fact]
    public async Task AddAsync_WhenPlantAlreadySaved_ShouldReturnExistingWithoutDuplicate()
    {
        var plant = CreatePlant(name: "Rosemary");
        DbContext.Plants.Add(plant);
        await DbContext.SaveChangesAsync();
        await SeedUserPlantAsync(plant.Id, DefaultUserId);

        var result = await _sut.AddAsync(plant.Id, DefaultUserId);

        result.ShouldNotBeNull();
        result.Id.ShouldBe(plant.Id);
        result.Name.ShouldBe("Rosemary");
        (await DbContext.UserPlants.CountAsync()).ShouldBe(1);
    }

    [Fact]
    public async Task AddAsync_WhenPlantNotFound_ShouldReturnNull()
    {
        var result = await _sut.AddAsync(Guid.NewGuid(), DefaultUserId);

        result.ShouldBeNull();
        (await DbContext.UserPlants.CountAsync()).ShouldBe(0);
    }

    [Fact]
    public async Task RemoveAsync_WhenPlantSaved_ShouldReturnTrue()
    {
        var plant = CreatePlant();
        DbContext.Plants.Add(plant);
        await DbContext.SaveChangesAsync();
        await SeedUserPlantAsync(plant.Id, DefaultUserId);

        var result = await _sut.RemoveAsync(plant.Id, DefaultUserId);

        result.ShouldBeTrue();
        (await DbContext.UserPlants.CountAsync()).ShouldBe(0);
    }

    [Fact]
    public async Task RemoveAsync_WhenPlantNotSaved_ShouldReturnFalse()
    {
        var result = await _sut.RemoveAsync(Guid.NewGuid(), DefaultUserId);

        result.ShouldBeFalse();
    }
}
