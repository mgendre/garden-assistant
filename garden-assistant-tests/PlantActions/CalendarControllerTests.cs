using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using GardenAssistant.Controllers;
using GardenAssistant.Data.Entities.Enums;
using GardenAssistant.DTOs.Calendar;
using GardenAssistant.DTOs.Plants;
using GardenAssistant.Services.Interfaces;
using GardenAssistant.Services.Watering;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Shouldly;

namespace GardenAssistant.Tests.PlantActions;

public class CalendarControllerTests
{
    private readonly Mock<IUserPlantService> _userPlantServiceMock = new();
    private readonly Mock<IPlantActionService> _plantActionServiceMock = new();
    private readonly Mock<IWateringService> _wateringServiceMock = new();
    private readonly CalendarController _sut;
    private static readonly Guid DefaultUserId = new("00000000-0000-0000-0000-000000000001");

    public CalendarControllerTests()
    {
        _sut = new CalendarController(_userPlantServiceMock.Object, _plantActionServiceMock.Object, _wateringServiceMock.Object);
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
                WaterNeeds.Medium, PropagationMethod.Seed, null, true, [], [], null, null, false, null, null, [], null, []),
            new(basilId, "Basilic", "Ocimum basilicum", null, "Lamiaceae", "Ocimum",
                LifeCycle.Annual, 40, RootDepth.Shallow, SunRequirement.FullSun,
                WaterNeeds.Medium, PropagationMethod.Seed, null, true, [], [], null, null, false, null, null, [], null, [])
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
