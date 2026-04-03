using GardenAssistant.Data.Entities;
using GardenAssistant.Data.Entities.Enums;
using GardenAssistant.Data.Seeders;
using GardenAssistant.Tests.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;

namespace GardenAssistant.Tests.Seeds;

public class PlantActionSeederTests : DatabaseTestBase
{
    private readonly string _tempRoot;
    private readonly Mock<IWebHostEnvironment> _envMock;
    private readonly Mock<ILogger<PlantActionSeeder>> _loggerMock;

    public PlantActionSeederTests()
    {
        _tempRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(Path.Combine(_tempRoot, "Data", "Seeds"));

        _envMock = new Mock<IWebHostEnvironment>();
        _envMock.Setup(e => e.ContentRootPath).Returns(_tempRoot);

        _loggerMock = new Mock<ILogger<PlantActionSeeder>>();
    }

    private void WriteActionsJson(string json) =>
        File.WriteAllText(Path.Combine(_tempRoot, "Data", "Seeds", "plant-actions.json"), json);

    private PlantActionSeeder CreateSeeder() => new(DbContext, _envMock.Object, _loggerMock.Object);

    private async Task<Plant> SeedPlantAsync(string key = "tomate", string name = "Tomate", bool isCustomized = false)
    {
        var plant = new Plant { Id = Guid.NewGuid(), Key = key, Name = name, IsCustomized = isCustomized };
        DbContext.Plants.Add(plant);
        await DbContext.SaveChangesAsync();
        return plant;
    }

    [Fact]
    public async Task SeedAsync_WhenNoActionsExist_ShouldInsertAll()
    {
        await SeedPlantAsync();

        WriteActionsJson("""
        [
          {
            "plantKey": "tomate",
            "actions": [
              {
                "actionType": "IndoorSowing",
                "halfMonthStart": 3,
                "halfMonthEnd": 6,
                "notes": null
              },
              {
                "actionType": "Transplanting",
                "halfMonthStart": 9,
                "halfMonthEnd": 10,
                "notes": "Après les gelées"
              }
            ]
          }
        ]
        """);

        await CreateSeeder().SeedAsync();

        DbContext.PlantActions.Count().ShouldBe(2);
    }

    [Fact]
    public async Task SeedAsync_WhenActionExistsAndNotLocked_ShouldUpdateNotes()
    {
        var plant = await SeedPlantAsync();
        DbContext.PlantActions.Add(new PlantAction
        {
            Id = Guid.NewGuid(),
            PlantId = plant.Id,
            ActionType = PlantActionType.IndoorSowing,
            HalfMonthStart = 3,
            HalfMonthEnd = 6,
            Notes = "Old notes"
        });
        await DbContext.SaveChangesAsync();

        WriteActionsJson("""
        [
          {
            "plantKey": "tomate",
            "actions": [
              {
                "actionType": "IndoorSowing",
                "halfMonthStart": 3,
                "halfMonthEnd": 6,
                "notes": "Updated notes"
              }
            ]
          }
        ]
        """);

        await CreateSeeder().SeedAsync();

        DbContext.PlantActions.Count().ShouldBe(1);
        DbContext.PlantActions.Single().Notes.ShouldBe("Updated notes");
    }

    [Fact]
    public async Task SeedAsync_WhenPlantIsCustomized_ShouldSkipActions()
    {
        await SeedPlantAsync(isCustomized: true);

        WriteActionsJson("""
        [
          {
            "plantKey": "tomate",
            "actions": [
              {
                "actionType": "IndoorSowing",
                "halfMonthStart": 3,
                "halfMonthEnd": 6,
                "notes": null
              }
            ]
          }
        ]
        """);

        await CreateSeeder().SeedAsync();

        DbContext.PlantActions.Count().ShouldBe(0);
    }

    [Fact]
    public async Task SeedAsync_WhenPlantKeyNotFound_ShouldSkip()
    {
        WriteActionsJson("""
        [
          {
            "plantKey": "tomate",
            "actions": [
              {
                "actionType": "IndoorSowing",
                "halfMonthStart": 3,
                "halfMonthEnd": 6,
                "notes": null
              }
            ]
          }
        ]
        """);

        await CreateSeeder().SeedAsync();

        DbContext.PlantActions.Count().ShouldBe(0);
    }

    [Fact]
    public async Task SeedAsync_WhenRunTwice_ShouldBeIdempotent()
    {
        await SeedPlantAsync();

        WriteActionsJson("""
        [
          {
            "plantKey": "tomate",
            "actions": [
              {
                "actionType": "IndoorSowing",
                "halfMonthStart": 3,
                "halfMonthEnd": 6,
                "notes": "Semis en godet"
              }
            ]
          }
        ]
        """);

        await CreateSeeder().SeedAsync();
        await CreateSeeder().SeedAsync();

        DbContext.PlantActions.Count().ShouldBe(1);
        DbContext.PlantActions.Single().Notes.ShouldBe("Semis en godet");
    }

    ~PlantActionSeederTests()
    {
        if (Directory.Exists(_tempRoot))
        {
            Directory.Delete(_tempRoot, recursive: true);
        }
    }
}
