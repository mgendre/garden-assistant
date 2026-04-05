using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using GardenAssistant.DTOs.Calendar;
using GardenAssistant.DTOs.Watering;
using GardenAssistant.Services.Interfaces;
using GardenAssistant.Services.Watering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GardenAssistant.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class CalendarController(
    IUserPlantService userPlantService,
    IPlantActionService plantActionService,
    IWateringService wateringService) : ControllerBase
{
    private Guid CallerId =>
        Guid.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub)!);

    [HttpGet("my-plants")]
    [ProducesResponseType(typeof(CalendarDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyPlantsCalendar()
    {
        var userPlants = (await userPlantService.GetAllAsync(CallerId)).ToList();
        var plantIds = userPlants.Select(p => p.Id).ToList();
        var actionsByPlant = await plantActionService.GetByPlantIdsAsync(plantIds);

        var calendarPlants = plantIds.Select(id => new CalendarPlantDto(
            id,
            actionsByPlant.GetValueOrDefault(id, [])
        )).ToList();

        return Ok(new CalendarDto(calendarPlants));
    }

    [HttpGet("watering/today")]
    [ProducesResponseType(typeof(WateringTodayDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetWateringToday()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var result = await wateringService.GetWateringTodayAsync(CallerId, today);
        return Ok(result);
    }

    [HttpGet("watering/schedule")]
    [ProducesResponseType(typeof(WateringScheduleDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetWateringSchedule(
        [FromQuery][Range(1, 24)] int halfMonth,
        [FromQuery] string source = "all")
    {
        var result = await wateringService.GetWateringScheduleAsync(CallerId, halfMonth, source);
        return Ok(result);
    }
}
