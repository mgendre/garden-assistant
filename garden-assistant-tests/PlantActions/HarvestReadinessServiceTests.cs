using GardenAssistant.Data.Entities;
using GardenAssistant.Data.Entities.Enums;
using GardenAssistant.Services;
using GardenAssistant.Tests.Infrastructure;
using Shouldly;

namespace GardenAssistant.Tests.PlantActions;

public class HarvestReadinessServiceTests : DatabaseTestBase
{
    private readonly HarvestReadinessService _sut;

    public HarvestReadinessServiceTests()
    {
        _sut = new HarvestReadinessService(DbContext);
    }

    [Fact]
    public async Task GetByPlantIdAsync_WhenPlantHasNoReadiness_ShouldReturnNull()
    {
        var plant = new Plant { Id = Guid.NewGuid(), Key = "basil", Name = "Basil" };
        DbContext.Plants.Add(plant);
        await DbContext.SaveChangesAsync();

        var result = await _sut.GetByPlantIdAsync(plant.Id);

        result.ShouldBeNull();
    }

    [Fact]
    public async Task GetByPlantIdAsync_WhenPlantHasReadiness_ShouldReturnWithCriteriaOrdered()
    {
        var plant = new Plant { Id = Guid.NewGuid(), Key = "tomato", Name = "Tomato" };
        DbContext.Plants.Add(plant);

        var harvestReadiness = new HarvestReadiness
        {
            Id = Guid.NewGuid(),
            PlantId = plant.Id,
            Description = "Harvest when fruits are fully colored",
            DaysFromTransplant = 70,
            DaysFromSowing = 120,
            Criteria =
            [
                new HarvestReadinessCriterion
                {
                    Id = Guid.NewGuid(),
                    CriterionType = HarvestCriterionType.Technique,
                    Description = "Twist gently to detach"
                },
                new HarvestReadinessCriterion
                {
                    Id = Guid.NewGuid(),
                    CriterionType = HarvestCriterionType.Visual,
                    Description = "Uniform red color"
                }
            ]
        };
        DbContext.HarvestReadiness.Add(harvestReadiness);
        await DbContext.SaveChangesAsync();

        var result = await _sut.GetByPlantIdAsync(plant.Id);

        result.ShouldNotBeNull();
        result!.Description.ShouldBe("Harvest when fruits are fully colored");
        result.DaysFromTransplant.ShouldBe(70);
        result.DaysFromSowing.ShouldBe(120);
        result.Criteria.Count.ShouldBe(2);
        result.Criteria[0].CriterionType.ShouldBe(HarvestCriterionType.Visual);
        result.Criteria[1].CriterionType.ShouldBe(HarvestCriterionType.Technique);
    }
}
