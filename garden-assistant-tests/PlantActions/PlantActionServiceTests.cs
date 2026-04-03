using GardenAssistant.Data.Entities;
using GardenAssistant.Data.Entities.Enums;
using GardenAssistant.Services;
using GardenAssistant.Tests.Infrastructure;
using Shouldly;

namespace GardenAssistant.Tests.PlantActions;

public class PlantActionServiceTests : DatabaseTestBase
{
    private readonly PlantActionService _sut;

    public PlantActionServiceTests()
    {
        _sut = new PlantActionService(DbContext);
    }

    [Fact]
    public async Task GetByPlantIdAsync_WhenPlantHasNoActions_ShouldReturnEmpty()
    {
        var plant = new Plant { Id = Guid.NewGuid(), Key = "tomato", Name = "Tomato" };
        DbContext.Plants.Add(plant);
        await DbContext.SaveChangesAsync();

        var result = await _sut.GetByPlantIdAsync(plant.Id);

        result.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetByPlantIdAsync_WhenPlantHasActions_ShouldReturnAllActionsOrdered()
    {
        var plant = new Plant { Id = Guid.NewGuid(), Key = "tomato-2", Name = "Tomato" };
        DbContext.Plants.Add(plant);

        DbContext.PlantActions.AddRange(
            new PlantAction
            {
                Id = Guid.NewGuid(),
                PlantId = plant.Id,
                ActionType = PlantActionType.Harvest,
                HalfMonthStart = 14,
                HalfMonthEnd = 20
            },
            new PlantAction
            {
                Id = Guid.NewGuid(),
                PlantId = plant.Id,
                ActionType = PlantActionType.IndoorSowing,
                HalfMonthStart = 5,
                HalfMonthEnd = 7
            },
            new PlantAction
            {
                Id = Guid.NewGuid(),
                PlantId = plant.Id,
                ActionType = PlantActionType.IndoorSowing,
                HalfMonthStart = 3,
                HalfMonthEnd = 4,
                Notes = "Early start"
            }
        );
        await DbContext.SaveChangesAsync();

        var result = await _sut.GetByPlantIdAsync(plant.Id);

        result.Count.ShouldBe(3);
        result[0].ActionType.ShouldBe(PlantActionType.IndoorSowing);
        result[0].HalfMonthStart.ShouldBe(3);
        result[0].Notes.ShouldBe("Early start");
        result[1].ActionType.ShouldBe(PlantActionType.IndoorSowing);
        result[1].HalfMonthStart.ShouldBe(5);
        result[2].ActionType.ShouldBe(PlantActionType.Harvest);
    }

    [Fact]
    public async Task GetByPlantIdAsync_WhenPlantDoesNotExist_ShouldReturnEmpty()
    {
        var result = await _sut.GetByPlantIdAsync(Guid.NewGuid());

        result.ShouldBeEmpty();
    }
}
