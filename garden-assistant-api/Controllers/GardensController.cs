using GardenAssistant.DTOs;
using GardenAssistant.Services;
using Microsoft.AspNetCore.Mvc;

namespace GardenAssistant.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GardensController(GardenService gardenService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await gardenService.GetAllAsync());

    [HttpPost]
    public async Task<IActionResult> Create(CreateGardenRequest request)
    {
        var garden = await gardenService.CreateAsync(request);
        return CreatedAtAction(nameof(GetAll), new { id = garden.Id }, garden);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateGardenRequest request)
    {
        var garden = await gardenService.UpdateAsync(id, request);
        return garden is null ? NotFound() : Ok(garden);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await gardenService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}
