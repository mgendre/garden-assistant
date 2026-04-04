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
    [HttpPost("companions")]
    [ProducesResponseType(typeof(CompanionSearchResultDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetCompanionRecommendations(CompanionRecommendationRequest request) =>
        Ok(await plantAssociationService.GetCompanionRecommendationsAsync(request.PlantIds, request.CentralPlantIds, request.MinScore));
}
