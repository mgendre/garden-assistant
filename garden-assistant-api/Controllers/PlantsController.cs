using GardenAssistant.DTOs.Plants;
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
    [ProducesResponseType(typeof(List<PlantDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll() =>
        Ok(await plantService.GetAllAsync());
}
