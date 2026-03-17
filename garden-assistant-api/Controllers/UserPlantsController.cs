using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using GardenAssistant.DTOs.Plants;
using GardenAssistant.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GardenAssistant.Controllers;

[ApiController]
[Authorize]
[Route("api/user-plants")]
public class UserPlantsController(IUserPlantService userPlantService) : ControllerBase
{
    private Guid CallerId =>
        Guid.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub)!);

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PlantDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll() =>
        Ok(await userPlantService.GetAllAsync(CallerId));

    [HttpPost("{plantId:guid}")]
    [ProducesResponseType(typeof(PlantDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Add(Guid plantId)
    {
        var plant = await userPlantService.AddAsync(plantId, CallerId);
        return plant is null ? NotFound() : CreatedAtAction(nameof(GetAll), plant);
    }

    [HttpDelete("{plantId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Remove(Guid plantId)
    {
        var removed = await userPlantService.RemoveAsync(plantId, CallerId);
        return removed ? NoContent() : NotFound();
    }
}
