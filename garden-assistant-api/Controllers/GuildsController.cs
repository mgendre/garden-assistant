using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using GardenAssistant.DTOs.Guilds;
using GardenAssistant.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GardenAssistant.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class GuildsController(IGuildService guildService) : ControllerBase
{
    private Guid CallerId =>
        Guid.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub)!);

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<GuildDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll() =>
        Ok(await guildService.GetAllAsync(CallerId));

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(GuildDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var guild = await guildService.GetByIdAsync(id, CallerId);
        return guild is null ? NotFound() : Ok(guild);
    }

    [HttpPost]
    [ProducesResponseType(typeof(GuildDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(CreateGuildRequest request)
    {
        var guild = await guildService.CreateAsync(request, CallerId);
        return CreatedAtAction(nameof(GetById), new { id = guild.Id }, guild);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(GuildDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(Guid id, UpdateGuildRequest request)
    {
        var guild = await guildService.UpdateAsync(id, request, CallerId);
        return guild is null ? NotFound() : Ok(guild);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await guildService.DeleteAsync(id, CallerId);
        return deleted ? NoContent() : NotFound();
    }
}
