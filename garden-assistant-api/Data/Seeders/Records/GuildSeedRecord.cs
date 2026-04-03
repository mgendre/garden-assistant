namespace GardenAssistant.Data.Seeders.Records;

public record GuildSeedRecord(
    string Name,
    string? Description,
    List<GuildPlantEntry> Plants
);
