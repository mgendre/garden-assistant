using GardenAssistant.DTOs.Plants;
using GardenAssistant.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GardenAssistant.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class PlantsController(
    IPlantService plantService,
    IPlantActionService plantActionService,
    IHarvestReadinessService harvestReadinessService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(List<PlantDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll() =>
        Ok(await plantService.GetAllAsync());

    [HttpGet("{id:guid}/actions")]
    [ProducesResponseType(typeof(List<PlantActionDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetActions(Guid id) =>
        Ok(await plantActionService.GetByPlantIdAsync(id));

    [HttpGet("{id:guid}/harvest-readiness")]
    [ProducesResponseType(typeof(HarvestReadinessDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<HarvestReadinessDto>> GetHarvestReadiness(Guid id)
    {
        var result = await harvestReadinessService.GetByPlantIdAsync(id);
        if (result is null)
        {
            return NotFound();
        }
        return Ok(result);
    }
}
