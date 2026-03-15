using GardenAssistant.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GardenAssistant.Tests.Infrastructure;

public class WebAppFixture : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _connection = new("DataSource=:memory:");

    private DbContextOptions<AppDbContext> SqliteOptions =>
        new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .UseSnakeCaseNamingConvention()
            .Options;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        _connection.Open();

        builder.ConfigureAppConfiguration(config =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "test-secret-key-minimum-32-characters-long",
                ["Jwt:Issuer"] = "garden-assistant",
                ["Jwt:Audience"] = "garden-assistant",
                ["Jwt:AccessTokenMinutes"] = "15",
                ["Jwt:RefreshTokenDays"] = "30"
            });
        });

        builder.ConfigureServices(services =>
        {
            var toRemove = services
                .Where(d => d.ServiceType == typeof(AppDbContext) ||
                            d.ServiceType == typeof(DbContextOptions<AppDbContext>))
                .ToList();
            foreach (var d in toRemove)
            {
                services.Remove(d);
            }

            services.AddScoped<AppDbContext>(_ => new AppDbContext(SqliteOptions));
        });

        builder.UseEnvironment("Testing");
    }

    public AppDbContext CreateDbContext()
    {
        var db = new AppDbContext(SqliteOptions);
        db.Database.EnsureCreated();
        return db;
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        if (disposing)
        {
            _connection.Dispose();
        }
    }
}
