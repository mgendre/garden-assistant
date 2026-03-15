using GardenAssistant.DTOs;
using GardenAssistant.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GardenAssistant.Controllers;

[ApiController]
[Authorize]
[Route("api")]
public class PlantAssociationsController(IPlantAssociationService plantAssociationService) : ControllerBase
{
    [HttpGet("plants/{plantId:guid}/associations")]
    [ProducesResponseType(typeof(IEnumerable<PlantAssociationDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetForPlant(Guid plantId) =>
        Ok(await plantAssociationService.GetForPlantAsync(plantId));

    [HttpPost("plantassociations")]
    [ProducesResponseType(typeof(PlantAssociationDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(CreatePlantAssociationRequest request)
    {
        var association = await plantAssociationService.CreateAsync(request);
        return CreatedAtAction(nameof(GetForPlant), new { plantId = association.SourcePlantId }, association);
    }

    [HttpPost("plants/companions")]
    [ProducesResponseType(typeof(CompanionSearchResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetCompanionRecommendations(CompanionRecommendationRequest request) =>
        Ok(await plantAssociationService.GetCompanionRecommendationsAsync(request.PlantIds));

    [HttpDelete("plantassociations/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await plantAssociationService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}
