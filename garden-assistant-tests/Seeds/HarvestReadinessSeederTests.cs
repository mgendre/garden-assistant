using GardenAssistant.Data.Entities;
using GardenAssistant.Data.Entities.Enums;
using GardenAssistant.Data.Seeders;
using GardenAssistant.Tests.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;

namespace GardenAssistant.Tests.Seeds;

public class HarvestReadinessSeederTests : DatabaseTestBase
{
    private readonly string _tempRoot;
    private readonly Mock<IWebHostEnvironment> _envMock;
    private readonly Mock<ILogger<HarvestReadinessSeeder>> _loggerMock;

    public HarvestReadinessSeederTests()
    {
        _tempRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(Path.Combine(_tempRoot, "Data", "Seeds"));

        _envMock = new Mock<IWebHostEnvironment>();
        _envMock.Setup(e => e.ContentRootPath).Returns(_tempRoot);

        _loggerMock = new Mock<ILogger<HarvestReadinessSeeder>>();
    }

    private void WriteHarvestReadinessJson(string json) =>
        File.WriteAllText(Path.Combine(_tempRoot, "Data", "Seeds", "harvest-readiness.json"), json);

    private HarvestReadinessSeeder CreateSeeder() => new(DbContext, _envMock.Object, _loggerMock.Object);

    private async Task<Plant> SeedPlantAsync(string key = "tomate", string name = "Tomate", bool isCustomized = false)
    {
        var plant = new Plant { Id = Guid.NewGuid(), Key = key, Name = name, IsCustomized = isCustomized };
        DbContext.Plants.Add(plant);
        await DbContext.SaveChangesAsync();
        return plant;
    }

    [Fact]
    public async Task SeedAsync_WhenNoReadinessExists_ShouldInsertWithCriteria()
    {
        var plant = await SeedPlantAsync();

        WriteHarvestReadinessJson("""
        [
          {
            "plantKey": "tomate",
            "description": "Recolter quand le fruit est bien colore",
            "daysFromTransplant": 70,
            "daysFromSowing": 120,
            "criteria": [
              {
                "criterionType": "Visual",
                "description": "Fruit bien colore et ferme"
              },
              {
                "criterionType": "Touch",
                "description": "Leger ramollissement"
              }
            ]
          }
        ]
        """);

        await CreateSeeder().SeedAsync();

        var readiness = DbContext.HarvestReadiness.Include(h => h.Criteria).Single();
        readiness.PlantId.ShouldBe(plant.Id);
        readiness.Description.ShouldBe("Recolter quand le fruit est bien colore");
        readiness.DaysFromTransplant.ShouldBe(70);
        readiness.DaysFromSowing.ShouldBe(120);
        readiness.Criteria.Count.ShouldBe(2);
        readiness.Criteria.ShouldContain(c => c.CriterionType == HarvestCriterionType.Visual);
        readiness.Criteria.ShouldContain(c => c.CriterionType == HarvestCriterionType.Touch);
    }

    [Fact]
    public async Task SeedAsync_WhenReadinessExistsAndNotLocked_ShouldUpdateFieldsAndReplaceCriteria()
    {
        var plant = await SeedPlantAsync();
        DbContext.HarvestReadiness.Add(new HarvestReadiness
        {
            Id = Guid.NewGuid(),
            PlantId = plant.Id,
            Description = "Old description",
            DaysFromTransplant = 60,
            DaysFromSowing = 100,
        });
        await DbContext.SaveChangesAsync();
        DbContext.ChangeTracker.Clear();

        WriteHarvestReadinessJson("""
        [
          {
            "plantKey": "tomate",
            "description": "Updated description",
            "daysFromTransplant": 70,
            "daysFromSowing": 120,
            "criteria": [
              {
                "criterionType": "Touch",
                "description": "New touch criterion"
              },
              {
                "criterionType": "Visual",
                "description": "New visual criterion"
              }
            ]
          }
        ]
        """);
        await CreateSeeder().SeedAsync();

        DbContext.ChangeTracker.Clear();
        var readiness = DbContext.HarvestReadiness.Include(h => h.Criteria).Single();
        readiness.Description.ShouldBe("Updated description");
        readiness.DaysFromTransplant.ShouldBe(70);
        readiness.DaysFromSowing.ShouldBe(120);
        readiness.Criteria.Count.ShouldBe(2);
        readiness.Criteria.ShouldContain(c => c.Description == "New touch criterion");
        readiness.Criteria.ShouldContain(c => c.Description == "New visual criterion");
    }

    [Fact]
    public async Task SeedAsync_WhenPlantIsCustomized_ShouldSkip()
    {
        await SeedPlantAsync(isCustomized: true);

        WriteHarvestReadinessJson("""
        [
          {
            "plantKey": "tomate",
            "description": "Some description",
            "daysFromTransplant": 70,
            "daysFromSowing": 120,
            "criteria": []
          }
        ]
        """);

        await CreateSeeder().SeedAsync();

        DbContext.HarvestReadiness.Count().ShouldBe(0);
    }

    [Fact]
    public async Task SeedAsync_WhenPlantKeyNotFound_ShouldSkip()
    {
        WriteHarvestReadinessJson("""
        [
          {
            "plantKey": "tomate",
            "description": "Some description",
            "daysFromTransplant": 70,
            "daysFromSowing": 120,
            "criteria": []
          }
        ]
        """);

        await CreateSeeder().SeedAsync();

        DbContext.HarvestReadiness.Count().ShouldBe(0);
    }

    [Fact]
    public async Task SeedAsync_WhenRunTwice_ShouldBeIdempotent()
    {
        await SeedPlantAsync();

        WriteHarvestReadinessJson("""
        [
          {
            "plantKey": "tomate",
            "description": "Recolter quand le fruit est bien colore",
            "daysFromTransplant": 70,
            "daysFromSowing": 120,
            "criteria": [
              {
                "criterionType": "Visual",
                "description": "Fruit bien colore"
              }
            ]
          }
        ]
        """);

        await CreateSeeder().SeedAsync();
        await CreateSeeder().SeedAsync();

        DbContext.HarvestReadiness.Count().ShouldBe(1);
        DbContext.HarvestReadinessCriteria.Count().ShouldBe(1);
    }

    ~HarvestReadinessSeederTests()
    {
        if (Directory.Exists(_tempRoot))
        {
            Directory.Delete(_tempRoot, recursive: true);
        }
    }
}
