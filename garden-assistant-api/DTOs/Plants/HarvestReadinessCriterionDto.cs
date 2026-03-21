using GardenAssistant.Data.Entities.Enums;

namespace GardenAssistant.DTOs.Plants;

public record HarvestReadinessCriterionDto(
    HarvestCriterionType CriterionType,
    string Description
);
