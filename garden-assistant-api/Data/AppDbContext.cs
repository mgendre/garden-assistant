using GardenAssistant.Entities;
using Microsoft.EntityFrameworkCore;

namespace GardenAssistant.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Garden> Gardens => Set<Garden>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Email).IsRequired().HasMaxLength(256);
            entity.HasMany(u => u.Gardens)
                  .WithOne(g => g.User)
                  .HasForeignKey(g => g.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Garden>(entity =>
        {
            entity.HasKey(g => g.Id);
            entity.Property(g => g.Name).IsRequired().HasMaxLength(256);
            entity.Property(g => g.Description).HasMaxLength(2000);
        });

        // Default user — used during development before auth is implemented
        modelBuilder.Entity<User>().HasData(new User
        {
            Id = new Guid("00000000-0000-0000-0000-000000000001"),
            Email = "admin@gardenassistant.local"
        });
    }
}
