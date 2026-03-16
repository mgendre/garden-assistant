using GardenAssistant.Data;
using GardenAssistant.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace GardenAssistant.Tests.Infrastructure;

public class TestAppDbContext(DbContextOptions<AppDbContext> options) : AppDbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserPlant>(entity =>
        {
            entity.Property(up => up.AddedAtUtc).HasDefaultValueSql("datetime('now')");
        });
    }
}
