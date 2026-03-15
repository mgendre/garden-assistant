using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using GardenAssistant.DTOs;
using GardenAssistant.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GardenAssistant.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class PlantingsController(IPlantingService plantingService) : ControllerBase
{
    private Guid CallerId =>
        Guid.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub)!);

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PlantingDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll() =>
        Ok(await plantingService.GetAllAsync(CallerId));

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(PlantingDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var planting = await plantingService.GetByIdAsync(id, CallerId);
        return planting is null ? NotFound() : Ok(planting);
    }

    [HttpPost]
    [ProducesResponseType(typeof(PlantingDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(CreatePlantingRequest request)
    {
        var planting = await plantingService.CreateAsync(request, CallerId);
        return CreatedAtAction(nameof(GetById), new { id = planting.Id }, planting);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await plantingService.DeleteAsync(id, CallerId);
        return deleted ? NoContent() : NotFound();
    }

    [HttpGet("{id:guid}/compatibility")]
    [ProducesResponseType(typeof(CompatibilityScoreDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCompatibilityScore(Guid id) =>
        Ok(await plantingService.GetCompatibilityScoreAsync(id, CallerId));
}
