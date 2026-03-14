using GardenAssistant.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace GardenAssistant.Tests.Infrastructure;

/// <summary>
/// Base class for tests that need a real database.
/// Creates an in-memory SQLite database per test class, applies the EF schema,
/// and disposes both the context and the connection after each test run.
///
/// Usage: inherit this class and inject <see cref="DbContext"/> into your system under test.
/// </summary>
public abstract class DatabaseTestBase : IDisposable
{
    private readonly SqliteConnection _connection;
    protected AppDbContext DbContext { get; }

    protected DatabaseTestBase()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .UseSnakeCaseNamingConvention()
            .Options;

        DbContext = new AppDbContext(options);
        DbContext.Database.EnsureCreated();
    }

    public void Dispose()
    {
        DbContext.Dispose();
        _connection.Dispose();
        GC.SuppressFinalize(this);
    }
}
