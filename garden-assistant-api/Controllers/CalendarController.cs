using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using GardenAssistant.DTOs.Calendar;
using GardenAssistant.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GardenAssistant.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class CalendarController(
    IUserPlantService userPlantService,
    IPlantActionService plantActionService) : ControllerBase
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
}
