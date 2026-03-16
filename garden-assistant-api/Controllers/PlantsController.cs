using GardenAssistant.DTOs;
using GardenAssistant.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GardenAssistant.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class PlantsController(IPlantService plantService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResult<PlantDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll([FromQuery] string? q = null) =>
        Ok(await plantService.GetAllAsync(q));

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(PlantDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var plant = await plantService.GetByIdAsync(id);
        return plant is null ? NotFound() : Ok(plant);
    }

    [HttpPost]
    [ProducesResponseType(typeof(PlantDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(CreatePlantRequest request)
    {
        var plant = await plantService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = plant.Id }, plant);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await plantService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }

}
