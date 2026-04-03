using GardenAssistant.Data.Entities;
using GardenAssistant.Data.Entities.Enums;
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
    public DbSet<PlantIntrinsicMechanism> PlantIntrinsicMechanisms => Set<PlantIntrinsicMechanism>();
    public DbSet<PlantSoilType> PlantSoilTypes => Set<PlantSoilType>();
    public DbSet<PlantAction> PlantActions => Set<PlantAction>();
    public DbSet<HarvestReadiness> HarvestReadiness => Set<HarvestReadiness>();
    public DbSet<HarvestReadinessCriterion> HarvestReadinessCriteria => Set<HarvestReadinessCriterion>();

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
            entity.Property(p => p.Key).IsRequired().HasMaxLength(256);
            entity.Property(p => p.Name).IsRequired().HasMaxLength(256);
            entity.Property(p => p.ScientificName).HasMaxLength(256);
            entity.Property(p => p.Description).HasMaxLength(10000);
            entity.Property(p => p.Family).HasMaxLength(128);
            entity.Property(p => p.Genus).HasMaxLength(128);
            entity.Property(p => p.IsCustomized).HasDefaultValue(false);
            entity.HasOne<User>()
                  .WithMany()
                  .HasForeignKey(p => p.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(p => p.ParentPlant)
                  .WithMany(p => p.Varieties)
                  .HasForeignKey(p => p.ParentPlantId)
                  .OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(p => p.Key).IsUnique().HasFilter("user_id IS NULL");
            entity.Property(p => p.OptimalPhMin).HasColumnType("decimal(3,1)");
            entity.Property(p => p.OptimalPhMax).HasColumnType("decimal(3,1)");
            entity.HasIndex(p => p.UserId);
            entity.HasIndex(p => p.ParentPlantId);
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
            entity.HasOne<Guild>().WithMany().HasForeignKey(p => p.GuildId).OnDelete(DeleteBehavior.SetNull);
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
            entity.Property(gp => gp.Role).HasDefaultValue(GuildPlantRole.Companion);
        });

        modelBuilder.Entity<UserPlant>(entity =>
        {
            entity.HasKey(up => new { up.UserId, up.PlantId });
            entity.HasOne<User>().WithMany().HasForeignKey(up => up.UserId).OnDelete(DeleteBehavior.Cascade);
            entity.HasOne<Plant>().WithMany().HasForeignKey(up => up.PlantId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<PlantIntrinsicMechanism>(entity =>
        {
            entity.HasKey(pim => new { pim.PlantId, pim.Mechanism });
            entity.HasOne<Plant>()
                  .WithMany(p => p.IntrinsicMechanisms)
                  .HasForeignKey(pim => pim.PlantId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<PlantSoilType>(entity =>
        {
            entity.HasKey(pst => new { pst.PlantId, pst.SoilType });
            entity.HasOne<Plant>()
                  .WithMany(p => p.SoilTypes)
                  .HasForeignKey(pst => pst.PlantId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<PlantAction>(entity =>
        {
            entity.HasKey(pa => pa.Id);
            entity.Property(pa => pa.Notes).HasMaxLength(1000);
            entity.HasOne(pa => pa.Plant)
                  .WithMany(p => p.Actions)
                  .HasForeignKey(pa => pa.PlantId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<HarvestReadiness>(entity =>
        {
            entity.HasKey(hr => hr.Id);
            entity.Property(hr => hr.Description).IsRequired().HasMaxLength(2000);
            entity.HasOne(hr => hr.Plant)
                  .WithOne(p => p.HarvestReadiness)
                  .HasForeignKey<HarvestReadiness>(hr => hr.PlantId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(hr => hr.PlantId).IsUnique();
        });

        modelBuilder.Entity<HarvestReadinessCriterion>(entity =>
        {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Description).IsRequired().HasMaxLength(1000);
            entity.HasOne(c => c.HarvestReadiness)
                  .WithMany(hr => hr.Criteria)
                  .HasForeignKey(c => c.HarvestReadinessId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<User>().HasData(new User
        {
            Id = new Guid("00000000-0000-0000-0000-000000000001"),
            Email = "admin@gardenassistant.local"
        });
    }
}
