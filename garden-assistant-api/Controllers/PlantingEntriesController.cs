using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using GardenAssistant.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GardenAssistant.Controllers;

[ApiController]
[Authorize]
[Route("api/planting-entries")]
public class PlantingEntriesController(IPlantingEntryService plantingEntryService) : ControllerBase
{
    private Guid CallerId =>
        Guid.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub)!);

    [HttpDelete("{entryId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveEntry(Guid entryId)
    {
        var removed = await plantingEntryService.RemoveEntryAsync(entryId, CallerId);
        return removed ? NoContent() : NotFound();
    }
}
