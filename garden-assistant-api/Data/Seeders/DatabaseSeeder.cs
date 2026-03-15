namespace GardenAssistant.Data.Seeders;

public interface IDatabaseSeeder
{
    Task SeedAllAsync();
}

public class DatabaseSeeder(IEnumerable<ISeeder> seeders) : IDatabaseSeeder
{
    public async Task SeedAllAsync()
    {
        foreach (var seeder in seeders)
        {
            await seeder.SeedAsync();
        }
    }
}
