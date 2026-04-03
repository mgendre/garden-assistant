using System.Diagnostics;

namespace GardenAssistant.Data.Seeders;

public interface IDatabaseSeeder
{
    Task SeedAllAsync();
}

public class DatabaseSeeder(IEnumerable<ISeeder> seeders, ILogger<DatabaseSeeder> logger) : IDatabaseSeeder
{
    public async Task SeedAllAsync()
    {
        logger.LogInformation("Database seeding started...");
        var sw = Stopwatch.StartNew();

        foreach (var seeder in seeders)
        {
            await seeder.SeedAsync();
        }

        logger.LogInformation("Database seeding completed in {ElapsedMs}ms", sw.ElapsedMilliseconds);
    }
}
