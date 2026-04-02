using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using GardenAssistant.Controllers;
using GardenAssistant.Data.Entities.Enums;
using GardenAssistant.DTOs.Calendar;
using GardenAssistant.DTOs.Plants;
using GardenAssistant.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Shouldly;

namespace GardenAssistant.Tests.PlantActions;

public class CalendarControllerTests
{
    private readonly Mock<IUserPlantService> _userPlantServiceMock = new();
    private readonly Mock<IPlantActionService> _plantActionServiceMock = new();
    private readonly CalendarController _sut;
    private static readonly Guid DefaultUserId = new("00000000-0000-0000-0000-000000000001");

    public CalendarControllerTests()
    {
        _sut = new CalendarController(_userPlantServiceMock.Object, _plantActionServiceMock.Object);
        _sut.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(
                [
                    new Claim(JwtRegisteredClaimNames.Sub, DefaultUserId.ToString())
                ], "TestAuth"))
            }
        };
    }

    [Fact]
    public async Task GetMyPlantsCalendar_WhenUserHasPlants_ShouldReturnCalendarWithActions()
    {
        // Arrange
        var tomatoId = Guid.NewGuid();
        var basilId = Guid.NewGuid();

        var userPlants = new List<PlantDto>
        {
            new(tomatoId, "Tomate", "Solanum lycopersicum", null, "Solanaceae", "Solanum",
                LifeCycle.Annual, 150, RootDepth.Medium, SunRequirement.FullSun,
                WaterNeeds.Medium, PropagationMethod.Seed, null, true, [], false, null, null, []),
            new(basilId, "Basilic", "Ocimum basilicum", null, "Lamiaceae", "Ocimum",
                LifeCycle.Annual, 40, RootDepth.Shallow, SunRequirement.FullSun,
                WaterNeeds.Medium, PropagationMethod.Seed, null, true, [], false, null, null, [])
        };

        var tomatoActions = new List<PlantActionDto>
        {
            new(Guid.NewGuid(), PlantActionType.IndoorSowing, 3, 6, "Semer en godets"),
            new(Guid.NewGuid(), PlantActionType.Transplanting, 9, 10, null),
            new(Guid.NewGuid(), PlantActionType.Harvest, 14, 20, null)
        };

        var basilActions = new List<PlantActionDto>
        {
            new(Guid.NewGuid(), PlantActionType.DirectSowing, 9, 12, null)
        };

        _userPlantServiceMock
            .Setup(s => s.GetAllAsync(DefaultUserId))
            .ReturnsAsync(userPlants);

        var actionsByPlant = new Dictionary<Guid, List<PlantActionDto>>
        {
            { tomatoId, tomatoActions },
            { basilId, basilActions }
        };

        _plantActionServiceMock
            .Setup(s => s.GetByPlantIdsAsync(It.IsAny<IEnumerable<Guid>>()))
            .ReturnsAsync(actionsByPlant);

        // Act
        var result = await _sut.GetMyPlantsCalendar();

        // Assert
        var okResult = result.ShouldBeOfType<OkObjectResult>();
        var calendar = okResult.Value.ShouldBeOfType<CalendarDto>();
        calendar.Plants.Count.ShouldBe(2);

        var tomato = calendar.Plants[0];
        tomato.PlantId.ShouldBe(tomatoId);
        tomato.Actions.Count.ShouldBe(3);
        tomato.Actions[0].ActionType.ShouldBe(PlantActionType.IndoorSowing);

        var basil = calendar.Plants[1];
        basil.PlantId.ShouldBe(basilId);
        basil.Actions.Count.ShouldBe(1);

        _userPlantServiceMock.Verify(s => s.GetAllAsync(DefaultUserId), Times.Once);
        _plantActionServiceMock.Verify(s => s.GetByPlantIdsAsync(It.IsAny<IEnumerable<Guid>>()), Times.Once);
    }

    [Fact]
    public async Task GetMyPlantsCalendar_WhenUserHasNoPlants_ShouldReturnEmptyList()
    {
        // Arrange
        _userPlantServiceMock
            .Setup(s => s.GetAllAsync(DefaultUserId))
            .ReturnsAsync(new List<PlantDto>());

        // Act
        var result = await _sut.GetMyPlantsCalendar();

        // Assert
        var okResult = result.ShouldBeOfType<OkObjectResult>();
        var calendar = okResult.Value.ShouldBeOfType<CalendarDto>();
        calendar.Plants.ShouldBeEmpty();

        _userPlantServiceMock.Verify(s => s.GetAllAsync(DefaultUserId), Times.Once);
        _plantActionServiceMock.Verify(
            s => s.GetByPlantIdsAsync(It.IsAny<IEnumerable<Guid>>()), Times.Once);
    }
}

public class PlantsControllerActionTests
{
    private readonly Mock<IPlantService> _plantServiceMock = new();
    private readonly Mock<IPlantActionService> _plantActionServiceMock = new();
    private readonly Mock<IHarvestReadinessService> _harvestReadinessServiceMock = new();
    private readonly PlantsController _sut;

    public PlantsControllerActionTests()
    {
        _sut = new PlantsController(
            _plantServiceMock.Object,
            _plantActionServiceMock.Object,
            _harvestReadinessServiceMock.Object);
    }

    [Fact]
    public async Task GetActions_WhenPlantHasActions_ShouldReturnActions()
    {
        // Arrange
        var plantId = Guid.NewGuid();
        var actions = new List<PlantActionDto>
        {
            new(Guid.NewGuid(), PlantActionType.IndoorSowing, 3, 6, "Semer en godets"),
            new(Guid.NewGuid(), PlantActionType.Transplanting, 9, 10, null),
            new(Guid.NewGuid(), PlantActionType.Harvest, 14, 20, null)
        };

        _plantActionServiceMock
            .Setup(s => s.GetByPlantIdAsync(plantId))
            .ReturnsAsync(actions);

        // Act
        var result = await _sut.GetActions(plantId);

        // Assert
        var okResult = result.ShouldBeOfType<OkObjectResult>();
        var returnedActions = okResult.Value.ShouldBeOfType<List<PlantActionDto>>();
        returnedActions.Count.ShouldBe(3);
        returnedActions[0].ActionType.ShouldBe(PlantActionType.IndoorSowing);
        returnedActions[0].HalfMonthStart.ShouldBe(3);
        returnedActions[0].HalfMonthEnd.ShouldBe(6);
        returnedActions[0].Notes.ShouldBe("Semer en godets");
        returnedActions[1].ActionType.ShouldBe(PlantActionType.Transplanting);
        returnedActions[2].ActionType.ShouldBe(PlantActionType.Harvest);

        _plantActionServiceMock.Verify(s => s.GetByPlantIdAsync(plantId), Times.Once);
    }

    [Fact]
    public async Task GetActions_WhenPlantHasNoActions_ShouldReturnEmptyList()
    {
        // Arrange
        var plantId = Guid.NewGuid();

        _plantActionServiceMock
            .Setup(s => s.GetByPlantIdAsync(plantId))
            .ReturnsAsync(new List<PlantActionDto>());

        // Act
        var result = await _sut.GetActions(plantId);

        // Assert
        var okResult = result.ShouldBeOfType<OkObjectResult>();
        var returnedActions = okResult.Value.ShouldBeOfType<List<PlantActionDto>>();
        returnedActions.ShouldBeEmpty();

        _plantActionServiceMock.Verify(s => s.GetByPlantIdAsync(plantId), Times.Once);
    }

    [Fact]
    public async Task GetHarvestReadiness_WhenPlantHasReadiness_ShouldReturnDto()
    {
        // Arrange
        var plantId = Guid.NewGuid();
        var readinessDto = new HarvestReadinessDto(
            "La tomate est prete quand elle est rouge",
            70,
            120,
            [
                new HarvestReadinessCriterionDto(HarvestCriterionType.Visual, "Couleur uniforme"),
                new HarvestReadinessCriterionDto(HarvestCriterionType.Touch, "Legere souplesse")
            ]);

        _harvestReadinessServiceMock
            .Setup(s => s.GetByPlantIdAsync(plantId))
            .ReturnsAsync(readinessDto);

        // Act
        var result = await _sut.GetHarvestReadiness(plantId);

        // Assert
        var okResult = result.Result.ShouldBeOfType<OkObjectResult>();
        var returned = okResult.Value.ShouldBeOfType<HarvestReadinessDto>();
        returned.Description.ShouldBe("La tomate est prete quand elle est rouge");
        returned.DaysFromTransplant.ShouldBe(70);
        returned.DaysFromSowing.ShouldBe(120);
        returned.Criteria.Count.ShouldBe(2);
        returned.Criteria[0].CriterionType.ShouldBe(HarvestCriterionType.Visual);
        returned.Criteria[0].Description.ShouldBe("Couleur uniforme");
        returned.Criteria[1].CriterionType.ShouldBe(HarvestCriterionType.Touch);
        returned.Criteria[1].Description.ShouldBe("Legere souplesse");

        _harvestReadinessServiceMock.Verify(s => s.GetByPlantIdAsync(plantId), Times.Once);
    }

    [Fact]
    public async Task GetHarvestReadiness_WhenPlantHasNoReadiness_ShouldReturn404()
    {
        // Arrange
        var plantId = Guid.NewGuid();

        _harvestReadinessServiceMock
            .Setup(s => s.GetByPlantIdAsync(plantId))
            .ReturnsAsync((HarvestReadinessDto?)null);

        // Act
        var result = await _sut.GetHarvestReadiness(plantId);

        // Assert
        result.Result.ShouldBeOfType<NotFoundResult>();

        _harvestReadinessServiceMock.Verify(s => s.GetByPlantIdAsync(plantId), Times.Once);
    }
}
