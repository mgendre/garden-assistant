using GardenAssistant.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace GardenAssistant.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Garden> Gardens => Set<Garden>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Plant> Plants => Set<Plant>();
    public DbSet<PlantAssociation> PlantAssociations => Set<PlantAssociation>();
    public DbSet<Planting> Plantings => Set<Planting>();
    public DbSet<PlantingEntry> PlantingEntries => Set<PlantingEntry>();
    public DbSet<Guild> Guilds => Set<Guild>();
    public DbSet<GuildPlant> GuildPlants => Set<GuildPlant>();
    public DbSet<UserPlant> UserPlants => Set<UserPlant>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Email).IsRequired().HasMaxLength(256);
            entity.HasMany<Garden>()
                  .WithOne()
                  .HasForeignKey(g => g.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Garden>(entity =>
        {
            entity.HasKey(g => g.Id);
            entity.Property(g => g.Name).IsRequired().HasMaxLength(256);
            entity.Property(g => g.Description).HasMaxLength(2000);
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(rt => rt.Id);
            entity.HasOne<User>()
                  .WithMany()
                  .HasForeignKey(rt => rt.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(rt => rt.Token).IsUnique();
        });

        modelBuilder.Entity<Plant>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Name).IsRequired().HasMaxLength(256);
            entity.Property(p => p.ScientificName).HasMaxLength(256);
            entity.Property(p => p.Description).HasMaxLength(10000);
            entity.Property(p => p.Family).HasMaxLength(128);
            entity.Property(p => p.Genus).HasMaxLength(128);
        });

        modelBuilder.Entity<PlantAssociation>(entity =>
        {
            entity.HasKey(pa => pa.Id);
            entity.HasOne<Plant>().WithMany().HasForeignKey(pa => pa.SourcePlantId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne<Plant>().WithMany().HasForeignKey(pa => pa.TargetPlantId).OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(pa => new { pa.SourcePlantId, pa.TargetPlantId, pa.Mechanism }).IsUnique();
            entity.Property(pa => pa.Notes).HasMaxLength(10000);
        });

        modelBuilder.Entity<Planting>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Name).IsRequired().HasMaxLength(256);
            entity.HasOne<Garden>().WithMany().HasForeignKey(p => p.GardenId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne<User>().WithMany().HasForeignKey(p => p.UserId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<PlantingEntry>(entity =>
        {
            entity.HasKey(pe => pe.Id);
            entity.HasOne<Planting>().WithMany().HasForeignKey(pe => pe.PlantingId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne<Plant>().WithMany().HasForeignKey(pe => pe.PlantId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Guild>(entity =>
        {
            entity.HasKey(g => g.Id);
            entity.Property(g => g.Name).IsRequired().HasMaxLength(256);
            entity.Property(g => g.Description).HasMaxLength(2000);
            entity.HasOne<User>().WithMany().HasForeignKey(g => g.UserId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<GuildPlant>(entity =>
        {
            entity.HasKey(gp => new { gp.GuildId, gp.PlantId });
            entity.HasOne<Guild>().WithMany().HasForeignKey(gp => gp.GuildId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne<Plant>().WithMany().HasForeignKey(gp => gp.PlantId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<UserPlant>(entity =>
        {
            entity.HasKey(up => new { up.UserId, up.PlantId });
            entity.HasOne<User>().WithMany().HasForeignKey(up => up.UserId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne<Plant>().WithMany().HasForeignKey(up => up.PlantId).OnDelete(DeleteBehavior.Restrict);
            entity.Property(up => up.AddedAtUtc).HasDefaultValueSql("now() at time zone 'utc'");
        });

        modelBuilder.Entity<User>().HasData(new User
        {
            Id = new Guid("00000000-0000-0000-0000-000000000001"),
            Email = "admin@gardenassistant.local"
        });
    }
}
