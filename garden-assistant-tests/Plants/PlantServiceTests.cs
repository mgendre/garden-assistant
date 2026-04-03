using GardenAssistant.Data.Entities;
using GardenAssistant.Data.Entities.Enums;
using GardenAssistant.Services;
using GardenAssistant.Tests.Infrastructure;
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
        DbContext.Plants.Add(new Plant { Id = Guid.NewGuid(), Key = "tomato", Name = "Tomato" });
        DbContext.Plants.Add(new Plant { Id = Guid.NewGuid(), Key = "basil", Name = "Basil" });
        await DbContext.SaveChangesAsync();

        var result = await _sut.GetAllAsync();

        result.Count.ShouldBe(2);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnPlantsOrderedByName()
    {
        DbContext.Plants.Add(new Plant { Id = Guid.NewGuid(), Key = "zucchini", Name = "Zucchini" });
        DbContext.Plants.Add(new Plant { Id = Guid.NewGuid(), Key = "artichoke", Name = "Artichoke" });
        DbContext.Plants.Add(new Plant { Id = Guid.NewGuid(), Key = "basil", Name = "Basil" });
        await DbContext.SaveChangesAsync();

        var result = await _sut.GetAllAsync();

        result[0].Name.ShouldBe("Artichoke");
        result[1].Name.ShouldBe("Basil");
        result[2].Name.ShouldBe("Zucchini");
    }

    [Fact]
    public async Task GetAllAsync_WhenPlantHasSoilTypes_ShouldMapToDto()
    {
        var plantId = Guid.NewGuid();
        DbContext.Plants.Add(new Plant { Id = plantId, Key = "tomato", Name = "Tomato" });
        DbContext.PlantSoilTypes.Add(new PlantSoilType { PlantId = plantId, SoilType = SoilType.Loam });
        DbContext.PlantSoilTypes.Add(new PlantSoilType { PlantId = plantId, SoilType = SoilType.Sandy });
        await DbContext.SaveChangesAsync();

        var result = await _sut.GetAllAsync();

        var dto = result.Single();
        dto.SoilTypes.Count.ShouldBe(2);
        dto.SoilTypes.ShouldContain("Loam");
        dto.SoilTypes.ShouldContain("Sandy");
    }

    [Fact]
    public async Task GetAllAsync_WhenPlantHasPhRange_ShouldMapToDto()
    {
        DbContext.Plants.Add(new Plant
        {
            Id = Guid.NewGuid(),
            Key = "tomato",
            Name = "Tomato",
            OptimalPhMin = 5.5m,
            OptimalPhMax = 6.5m
        });
        await DbContext.SaveChangesAsync();

        var result = await _sut.GetAllAsync();

        var dto = result.Single();
        dto.OptimalPhMin.ShouldBe(5.5m);
        dto.OptimalPhMax.ShouldBe(6.5m);
    }

    [Fact]
    public async Task GetAllAsync_WhenVarietyHasNoSoilTypes_ShouldInheritFromParent()
    {
        var parentId = Guid.NewGuid();
        var varietyId = Guid.NewGuid();
        DbContext.Plants.Add(new Plant
        {
            Id = parentId,
            Key = "courge",
            Name = "Courge",
            OptimalPhMin = 6.0m,
            OptimalPhMax = 6.8m
        });
        DbContext.PlantSoilTypes.Add(new PlantSoilType { PlantId = parentId, SoilType = SoilType.Loam });
        DbContext.PlantSoilTypes.Add(new PlantSoilType { PlantId = parentId, SoilType = SoilType.Sandy });
        DbContext.Plants.Add(new Plant
        {
            Id = varietyId,
            Key = "courgette",
            Name = "Courgette",
            ParentPlantId = parentId
        });
        await DbContext.SaveChangesAsync();

        var result = await _sut.GetAllAsync();

        var variety = result.Single(p => p.Name == "Courgette");
        variety.SoilTypes.Count.ShouldBe(2);
        variety.SoilTypes.ShouldContain("Loam");
        variety.SoilTypes.ShouldContain("Sandy");
        variety.OptimalPhMin.ShouldBe(6.0m);
        variety.OptimalPhMax.ShouldBe(6.8m);
    }
}
