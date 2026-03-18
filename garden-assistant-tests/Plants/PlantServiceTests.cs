using GardenAssistant.Data.Entities;
using GardenAssistant.Data.Entities.Enums;
using GardenAssistant.DTOs.Plants;
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
    public async Task GetAllAsync_WhenNoPlantsExist_ShouldReturnEmptyList()
    {
        var result = await _sut.GetAllAsync();

        result.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetAllAsync_WhenPlantsExist_ShouldReturnAll()
    {
        DbContext.Plants.Add(new Plant { Id = Guid.NewGuid(), Name = "Tomato" });
        DbContext.Plants.Add(new Plant { Id = Guid.NewGuid(), Name = "Basil" });
        await DbContext.SaveChangesAsync();

        var result = await _sut.GetAllAsync();

        result.Count.ShouldBe(2);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnPlantsOrderedByName()
    {
        DbContext.Plants.Add(new Plant { Id = Guid.NewGuid(), Name = "Zucchini" });
        DbContext.Plants.Add(new Plant { Id = Guid.NewGuid(), Name = "Artichoke" });
        DbContext.Plants.Add(new Plant { Id = Guid.NewGuid(), Name = "Basil" });
        await DbContext.SaveChangesAsync();

        var result = await _sut.GetAllAsync();

        result[0].Name.ShouldBe("Artichoke");
        result[1].Name.ShouldBe("Basil");
        result[2].Name.ShouldBe("Zucchini");
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
            [AssociationMechanism.PollinatorAttraction]
        );

        var result = await _sut.CreateAsync(request);

        result.Id.ShouldNotBe(Guid.Empty);
        result.Name.ShouldBe("Rosemary");
        result.ScientificName.ShouldBe("Salvia rosmarinus");
        result.LifeCycle.ShouldBe(LifeCycle.Perennial);
        result.IntrinsicMechanisms.ShouldContain(AssociationMechanism.PollinatorAttraction);
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
}
