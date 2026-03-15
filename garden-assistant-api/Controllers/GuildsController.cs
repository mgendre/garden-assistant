using GardenAssistant.DTOs;
using GardenAssistant.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GardenAssistant.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class GuildsController(IGuildService guildService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<GuildSummaryDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll() =>
        Ok(await guildService.GetAllAsync());

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(GuildDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var guild = await guildService.GetByIdAsync(id);
        return guild is null ? NotFound() : Ok(guild);
    }
}
