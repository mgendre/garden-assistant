namespace GardenAssistant.Data.Seeders.Records;

public record HarvestReadinessSeedRecord(
    string PlantKey,
    string Description,
    int? DaysFromTransplant,
    int? DaysFromSowing,
    List<CriterionRecord> Criteria
);
