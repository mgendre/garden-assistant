using GardenAssistant.DTOs.Companions;
using GardenAssistant.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GardenAssistant.Controllers;

[ApiController]
[Authorize]
[Route("api/plant-associations")]
public class PlantAssociationsController(IPlantAssociationService plantAssociationService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<PlantAssociationDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetForPlant([FromQuery] Guid plantId) =>
        Ok(await plantAssociationService.GetForPlantAsync(plantId));

    [HttpPost]
    [ProducesResponseType(typeof(PlantAssociationDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(CreatePlantAssociationRequest request)
    {
        var association = await plantAssociationService.CreateAsync(request);
        return Created($"/api/plant-associations/{association.Id}", association);
    }

    [HttpPost("companions")]
    [ProducesResponseType(typeof(CompanionSearchResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetCompanionRecommendations(CompanionRecommendationRequest request) =>
        Ok(await plantAssociationService.GetCompanionRecommendationsAsync(request.PlantIds, request.MinScore));

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await plantAssociationService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}
