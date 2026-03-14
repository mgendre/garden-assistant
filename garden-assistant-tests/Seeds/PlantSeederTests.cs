using GardenAssistant.Data;
using GardenAssistant.Data.Entities;
using GardenAssistant.Data.Entities.Enums;
using GardenAssistant.Tests.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Moq;
using Shouldly;

namespace GardenAssistant.Tests.Seeds;

public class PlantSeederTests : DatabaseTestBase
{
    private readonly string _tempRoot;
    private readonly Mock<IWebHostEnvironment> _envMock;

    public PlantSeederTests()
    {
        _tempRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(Path.Combine(_tempRoot, "Data", "Seeds"));

        _envMock = new Mock<IWebHostEnvironment>();
        _envMock.Setup(e => e.ContentRootPath).Returns(_tempRoot);
    }

    private void WritePlantsJson(string json) =>
        File.WriteAllText(Path.Combine(_tempRoot, "Data", "Seeds", "plants.json"), json);

    private PlantSeeder CreateSeeder() => new(DbContext, _envMock.Object);

    [Fact]
    public async Task SeedAsync_WhenPlantsAlreadyExist_ShouldSkip()
    {
        DbContext.Plants.Add(new Plant { Id = Guid.NewGuid(), Name = "Existing" });
        await DbContext.SaveChangesAsync();

        WritePlantsJson("[]");
        await CreateSeeder().SeedAsync();

        DbContext.Plants.Count().ShouldBe(1);
    }

    [Fact]
    public async Task SeedAsync_WhenNoPlantsExist_ShouldInsertAllRecords()
    {
        WritePlantsJson("""
        [
          {
            "name": "Tomate",
            "scientificName": "Solanum lycopersicum",
            "description": null,
            "family": "Solanaceae",
            "genus": "Solanum",
            "lifeCycle": "Annual",
            "heightAtMaturityCm": 180,
            "rootDepth": "Medium",
            "sunRequirement": "FullSun",
            "waterNeeds": "Medium",
            "nitrogenFixer": false,
            "allelopathicRisk": false,
            "pollinatorPlant": false
          },
          {
            "name": "Basilic",
            "scientificName": "Ocimum basilicum",
            "description": null,
            "family": "Lamiaceae",
            "genus": "Ocimum",
            "lifeCycle": "Annual",
            "heightAtMaturityCm": 50,
            "rootDepth": "Shallow",
            "sunRequirement": "FullSun",
            "waterNeeds": "Medium",
            "nitrogenFixer": false,
            "allelopathicRisk": false,
            "pollinatorPlant": true
          }
        ]
        """);

        await CreateSeeder().SeedAsync();

        DbContext.Plants.Count().ShouldBe(2);
    }

    [Fact]
    public async Task SeedAsync_WhenNoPlantsExist_ShouldMapFieldsCorrectly()
    {
        WritePlantsJson("""
        [
          {
            "name": "Trèfle blanc",
            "scientificName": "Trifolium repens",
            "description": "Fixateur d'azote",
            "family": "Fabaceae",
            "genus": "Trifolium",
            "lifeCycle": "Perennial",
            "heightAtMaturityCm": 15,
            "rootDepth": "Shallow",
            "sunRequirement": "FullSun",
            "waterNeeds": "Medium",
            "nitrogenFixer": true,
            "allelopathicRisk": false,
            "pollinatorPlant": true
          }
        ]
        """);

        await CreateSeeder().SeedAsync();

        var plant = DbContext.Plants.Single();
        plant.Name.ShouldBe("Trèfle blanc");
        plant.ScientificName.ShouldBe("Trifolium repens");
        plant.NitrogenFixer.ShouldBeTrue();
        plant.PollinatorPlant.ShouldBeTrue();
        plant.LifeCycle.ShouldBe(LifeCycle.Perennial);
    }

    ~PlantSeederTests()
    {
        if (Directory.Exists(_tempRoot))
            Directory.Delete(_tempRoot, recursive: true);
    }
}
