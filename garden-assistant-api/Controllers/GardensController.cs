using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using GardenAssistant.DTOs.Gardens;
using GardenAssistant.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GardenAssistant.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class GardensController(IGardenService gardenService) : ControllerBase
{
    private Guid CallerId =>
        Guid.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub)!);

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<GardenDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll() =>
        Ok(await gardenService.GetAllAsync(CallerId));

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(GardenDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var garden = await gardenService.GetByIdAsync(id, CallerId);
        return garden is null ? NotFound() : Ok(garden);
    }

    [HttpPost]
    [ProducesResponseType(typeof(GardenDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(CreateGardenRequest request)
    {
        var garden = await gardenService.CreateAsync(request, CallerId);
        return CreatedAtAction(nameof(GetById), new { id = garden.Id }, garden);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(GardenDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(Guid id, UpdateGardenRequest request)
    {
        var garden = await gardenService.UpdateAsync(id, request, CallerId);
        return garden is null ? NotFound() : Ok(garden);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await gardenService.DeleteAsync(id, CallerId);
        return deleted ? NoContent() : NotFound();
    }
}
