using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using GardenAssistant.DTOs.Beds;
using GardenAssistant.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GardenAssistant.Controllers;

[ApiController]
[Authorize]
[Route("api/gardens/{gardenId:guid}/beds")]
public class BedsController(IBedService bedService) : ControllerBase
{
    private Guid CallerId =>
        Guid.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub)!);

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<BedDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(Guid gardenId) =>
        Ok(await bedService.GetByGardenIdAsync(gardenId, CallerId));

    [HttpPost]
    [ProducesResponseType(typeof(BedDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Create(Guid gardenId, CreateBedRequest request)
    {
        var bed = await bedService.CreateAsync(gardenId, request, CallerId);
        return bed is null ? NotFound() : Created($"api/gardens/{gardenId}/beds/{bed.Id}", bed);
    }

    [HttpPut("{bedId:guid}")]
    [ProducesResponseType(typeof(BedDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid gardenId, Guid bedId, UpdateBedRequest request)
    {
        var bed = await bedService.UpdateAsync(gardenId, bedId, request, CallerId);
        return bed is null ? NotFound() : Ok(bed);
    }

    [HttpDelete("{bedId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid gardenId, Guid bedId)
    {
        var deleted = await bedService.DeleteAsync(gardenId, bedId, CallerId);
        return deleted ? NoContent() : NotFound();
    }
}
