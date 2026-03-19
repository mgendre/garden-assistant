using GardenAssistant.Data.Entities;
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
}
