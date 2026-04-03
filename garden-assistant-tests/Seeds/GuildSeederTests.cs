using GardenAssistant.Data.Entities;
using GardenAssistant.Data.Entities.Enums;
using GardenAssistant.Data.Seeders;
using GardenAssistant.Tests.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;

namespace GardenAssistant.Tests.Seeds;

public class GuildSeederTests : DatabaseTestBase
{
    private readonly string _tempRoot;
    private readonly Mock<IWebHostEnvironment> _envMock;
    private readonly Mock<ILogger<GuildSeeder>> _loggerMock;

    public GuildSeederTests()
    {
        _tempRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(Path.Combine(_tempRoot, "Data", "Seeds"));

        _envMock = new Mock<IWebHostEnvironment>();
        _envMock.Setup(e => e.ContentRootPath).Returns(_tempRoot);

        _loggerMock = new Mock<ILogger<GuildSeeder>>();
    }

    private void WriteGuildsJson(string json) =>
        File.WriteAllText(Path.Combine(_tempRoot, "Data", "Seeds", "guilds.json"), json);

    private GuildSeeder CreateSeeder() => new(DbContext, _envMock.Object, _loggerMock.Object);

    private async Task SeedPlantsAsync()
    {
        DbContext.Plants.AddRange(
            new Plant { Id = Guid.NewGuid(), Key = "tomate", Name = "Tomate" },
            new Plant { Id = Guid.NewGuid(), Key = "basilic", Name = "Basilic" },
            new Plant { Id = Guid.NewGuid(), Key = "carotte", Name = "Carotte" }
        );
        await DbContext.SaveChangesAsync();
    }

    [Fact]
    public async Task SeedAsync_WhenNoGuildsExist_ShouldInsertGuildAndLinks()
    {
        await SeedPlantsAsync();

        WriteGuildsJson("""
        [
          {
            "name": "Guilde Tomate",
            "description": "Guilde autour de la tomate",
            "plants": [
              { "key": "tomate", "role": "Central" },
              "basilic",
              "carotte"
            ]
          }
        ]
        """);

        await CreateSeeder().SeedAsync();

        DbContext.Guilds.Count().ShouldBe(1);
        var guild = DbContext.Guilds.Single();
        guild.Name.ShouldBe("Guilde Tomate");
        guild.Description.ShouldBe("Guilde autour de la tomate");

        var links = DbContext.GuildPlants.Where(gp => gp.GuildId == guild.Id).ToList();
        links.Count.ShouldBe(3);
        links.ShouldContain(gp => gp.Role == GuildPlantRole.Central);
        links.Count(gp => gp.Role == GuildPlantRole.Companion).ShouldBe(2);
    }

    [Fact]
    public async Task SeedAsync_WhenGuildExists_ShouldUpdateDescription()
    {
        await SeedPlantsAsync();
        DbContext.Guilds.Add(new Guild
        {
            Id = Guid.NewGuid(),
            Name = "Guilde Tomate",
            Description = "Old description"
        });
        await DbContext.SaveChangesAsync();

        WriteGuildsJson("""
        [
          {
            "name": "Guilde Tomate",
            "description": "Updated description",
            "plants": []
          }
        ]
        """);

        await CreateSeeder().SeedAsync();

        DbContext.Guilds.Count().ShouldBe(1);
        DbContext.Guilds.Single().Description.ShouldBe("Updated description");
    }

    [Fact]
    public async Task SeedAsync_WhenPlantIsLocked_ShouldSkipGuildPlantLink()
    {
        var tomate = new Plant { Id = Guid.NewGuid(), Key = "tomate", Name = "Tomate", IsCustomized = true };
        var basilic = new Plant { Id = Guid.NewGuid(), Key = "basilic", Name = "Basilic" };
        DbContext.Plants.AddRange(tomate, basilic);
        await DbContext.SaveChangesAsync();

        WriteGuildsJson("""
        [
          {
            "name": "Guilde Tomate",
            "description": "Guilde",
            "plants": [
              { "key": "tomate", "role": "Central" },
              "basilic"
            ]
          }
        ]
        """);

        await CreateSeeder().SeedAsync();

        var guild = DbContext.Guilds.Single();
        var links = DbContext.GuildPlants.Where(gp => gp.GuildId == guild.Id).ToList();
        links.Count.ShouldBe(1);
        links.Single().PlantId.ShouldBe(basilic.Id);
    }

    [Fact]
    public async Task SeedAsync_WhenGuildPlantRoleChanged_ShouldUpdate()
    {
        await SeedPlantsAsync();
        var guild = new Guild { Id = Guid.NewGuid(), Name = "Guilde Tomate", Description = "Guilde" };
        DbContext.Guilds.Add(guild);

        var tomate = DbContext.Plants.Single(p => p.Key == "tomate");
        DbContext.GuildPlants.Add(new GuildPlant
        {
            GuildId = guild.Id,
            PlantId = tomate.Id,
            Role = GuildPlantRole.Companion
        });
        await DbContext.SaveChangesAsync();

        WriteGuildsJson("""
        [
          {
            "name": "Guilde Tomate",
            "description": "Guilde",
            "plants": [
              { "key": "tomate", "role": "Central" }
            ]
          }
        ]
        """);

        await CreateSeeder().SeedAsync();

        var link = DbContext.GuildPlants.Single(gp => gp.GuildId == guild.Id && gp.PlantId == tomate.Id);
        link.Role.ShouldBe(GuildPlantRole.Central);
    }

    [Fact]
    public async Task SeedAsync_WhenRunTwice_ShouldBeIdempotent()
    {
        await SeedPlantsAsync();

        WriteGuildsJson("""
        [
          {
            "name": "Guilde Tomate",
            "description": "Guilde autour de la tomate",
            "plants": [
              { "key": "tomate", "role": "Central" },
              "basilic"
            ]
          }
        ]
        """);

        await CreateSeeder().SeedAsync();
        await CreateSeeder().SeedAsync();

        DbContext.Guilds.Count().ShouldBe(1);
        DbContext.GuildPlants.Count().ShouldBe(2);
    }

    [Fact]
    public async Task SeedAsync_WhenPlantKeyNotFound_ShouldSkipThatLink()
    {
        await SeedPlantsAsync();

        WriteGuildsJson("""
        [
          {
            "name": "Guilde Tomate",
            "description": "Guilde",
            "plants": [
              { "key": "tomate", "role": "Central" },
              "inconnue"
            ]
          }
        ]
        """);

        await CreateSeeder().SeedAsync();

        var guild = DbContext.Guilds.Single();
        var links = DbContext.GuildPlants.Where(gp => gp.GuildId == guild.Id).ToList();
        links.Count.ShouldBe(1);
    }

    [Fact]
    public async Task SeedAsync_WhenJsonIsEmpty_ShouldInsertNothing()
    {
        await SeedPlantsAsync();

        WriteGuildsJson("[]");

        await CreateSeeder().SeedAsync();

        DbContext.Guilds.Count().ShouldBe(0);
    }

    [Fact]
    public async Task SeedAsync_WhenPlantEntryIsString_ShouldDefaultToCompanion()
    {
        await SeedPlantsAsync();

        WriteGuildsJson("""
        [
          {
            "name": "Guilde Tomate",
            "description": "Guilde",
            "plants": ["basilic"]
          }
        ]
        """);

        await CreateSeeder().SeedAsync();

        var link = DbContext.GuildPlants.Single();
        link.Role.ShouldBe(GuildPlantRole.Companion);
    }

    [Fact]
    public async Task SeedAsync_WhenPlantEntryHasRole_ShouldMapRole()
    {
        await SeedPlantsAsync();

        WriteGuildsJson("""
        [
          {
            "name": "Guilde Tomate",
            "description": "Guilde",
            "plants": [
              { "key": "tomate", "role": "Central" }
            ]
          }
        ]
        """);

        await CreateSeeder().SeedAsync();

        var link = DbContext.GuildPlants.Single();
        link.Role.ShouldBe(GuildPlantRole.Central);
    }

    [Fact]
    public async Task SeedAsync_WhenMultipleGuilds_ShouldInsertAll()
    {
        await SeedPlantsAsync();

        WriteGuildsJson("""
        [
          {
            "name": "Guilde Tomate",
            "description": "Guilde tomate",
            "plants": [{ "key": "tomate", "role": "Central" }, "basilic"]
          },
          {
            "name": "Guilde Carotte",
            "description": "Guilde carotte",
            "plants": [{ "key": "carotte", "role": "Central" }]
          }
        ]
        """);

        await CreateSeeder().SeedAsync();

        DbContext.Guilds.Count().ShouldBe(2);
        DbContext.GuildPlants.Count().ShouldBe(3);
    }

    [Fact]
    public async Task SeedAsync_WhenGuildHasNoPlants_ShouldInsertGuildOnly()
    {
        await SeedPlantsAsync();

        WriteGuildsJson("""
        [
          {
            "name": "Guilde Vide",
            "description": "Guilde sans plantes",
            "plants": []
          }
        ]
        """);

        await CreateSeeder().SeedAsync();

        DbContext.Guilds.Count().ShouldBe(1);
        DbContext.GuildPlants.Count().ShouldBe(0);
    }

    ~GuildSeederTests()
    {
        if (Directory.Exists(_tempRoot))
        {
            Directory.Delete(_tempRoot, recursive: true);
        }
    }
}
