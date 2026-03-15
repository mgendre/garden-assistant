using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using GardenAssistant.DTOs;
using GardenAssistant.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GardenAssistant.Controllers;

[ApiController]
[Authorize]
[Route("api")]
public class PlantingEntriesController(IPlantingEntryService plantingEntryService) : ControllerBase
{
    private Guid CallerId =>
        Guid.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub)!);

    [HttpGet("plantings/{plantingId:guid}/entries")]
    [ProducesResponseType(typeof(IEnumerable<PlantingEntryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetForPlanting(Guid plantingId)
    {
        var entries = await plantingEntryService.GetForPlantingAsync(plantingId, CallerId);
        return entries is null ? NotFound() : Ok(entries);
    }

    [HttpPost("plantings/{plantingId:guid}/entries")]
    [ProducesResponseType(typeof(PlantingEntryDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AddEntry(Guid plantingId, CreatePlantingEntryRequest request)
    {
        var entry = await plantingEntryService.AddEntryAsync(plantingId, request, CallerId);
        if (entry is null) return NotFound();
        return CreatedAtAction(nameof(GetForPlanting), new { plantingId }, entry);
    }

    [HttpDelete("plantingentries/{entryId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveEntry(Guid entryId)
    {
        var removed = await plantingEntryService.RemoveEntryAsync(entryId, CallerId);
        return removed ? NoContent() : NotFound();
    }
}
