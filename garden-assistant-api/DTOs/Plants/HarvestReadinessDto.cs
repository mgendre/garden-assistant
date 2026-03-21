namespace GardenAssistant.DTOs.Plants;

public record HarvestReadinessDto(
    string Description,
    int? DaysFromTransplant,
    int? DaysFromSowing,
    List<HarvestReadinessCriterionDto> Criteria
);
