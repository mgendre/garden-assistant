using GardenAssistant.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace GardenAssistant.Tests.Infrastructure;

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
