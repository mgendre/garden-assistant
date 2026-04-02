using GardenAssistant.Data.Entities;
using GardenAssistant.Data.Entities.Enums;
using GardenAssistant.Data.Seeders;
using GardenAssistant.Tests.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
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
            "key": "tomate",
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
            "maxAltitudeM": 1000
          },
          {
            "key": "basilic",
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
            "maxAltitudeM": 800,
            "intrinsicMechanisms": ["PollinatorAttraction"]
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
            "key": "trefle-blanc",
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
            "maxAltitudeM": 2500,
            "intrinsicMechanisms": ["NitrogenFixation", "PollinatorAttraction"]
          }
        ]
        """);

        await CreateSeeder().SeedAsync();

        var plant = DbContext.Plants.Include(p => p.IntrinsicMechanisms).Single();
        plant.Name.ShouldBe("Trèfle blanc");
        plant.ScientificName.ShouldBe("Trifolium repens");
        plant.IntrinsicMechanisms.ShouldContain(im => im.Mechanism == AssociationMechanism.NitrogenFixation);
        plant.IntrinsicMechanisms.ShouldContain(im => im.Mechanism == AssociationMechanism.PollinatorAttraction);
        plant.LifeCycle.ShouldBe(LifeCycle.Perennial);
        plant.MaxAltitudeM.ShouldBe(2500);
    }

    [Fact]
    public async Task SeedAsync_WhenVarietyHasParentKey_ShouldSetParentPlantId()
    {
        WritePlantsJson("""
        [
          {
            "key": "courge",
            "name": "Courge",
            "scientificName": "Cucurbita pepo",
            "family": "Cucurbitaceae",
            "genus": "Cucurbita",
            "lifeCycle": "Annual",
            "rootDepth": "Medium",
            "sunRequirement": "FullSun",
            "waterNeeds": "Medium"
          },
          {
            "key": "courgette",
            "name": "Courgette",
            "scientificName": "Cucurbita pepo var. cylindrica",
            "family": "Cucurbitaceae",
            "genus": "Cucurbita",
            "lifeCycle": "Annual",
            "rootDepth": "Medium",
            "sunRequirement": "FullSun",
            "waterNeeds": "Medium",
            "parentKey": "courge"
          }
        ]
        """);

        await CreateSeeder().SeedAsync();

        var plants = DbContext.Plants.ToList();
        plants.Count.ShouldBe(2);
        var courge = plants.Single(p => p.Name == "Courge");
        var courgette = plants.Single(p => p.Name == "Courgette");
        courgette.ParentPlantId.ShouldBe(courge.Id);
        courge.ParentPlantId.ShouldBeNull();
    }

    [Fact]
    public async Task SeedAsync_WhenParentKeyReferencesNonExistentPlant_ShouldThrow()
    {
        WritePlantsJson("""
        [
          {
            "key": "courgette",
            "name": "Courgette",
            "scientificName": "Cucurbita pepo var. cylindrica",
            "family": "Cucurbitaceae",
            "genus": "Cucurbita",
            "lifeCycle": "Annual",
            "rootDepth": "Medium",
            "sunRequirement": "FullSun",
            "waterNeeds": "Medium",
            "parentKey": "courge"
          }
        ]
        """);

        await Should.ThrowAsync<InvalidOperationException>(CreateSeeder().SeedAsync());
    }

    [Fact]
    public async Task SeedAsync_WhenVarietiesListedBeforeParent_ShouldStillResolve()
    {
        WritePlantsJson("""
        [
          {
            "key": "courgette",
            "name": "Courgette",
            "scientificName": "Cucurbita pepo var. cylindrica",
            "family": "Cucurbitaceae",
            "genus": "Cucurbita",
            "lifeCycle": "Annual",
            "rootDepth": "Medium",
            "sunRequirement": "FullSun",
            "waterNeeds": "Medium",
            "parentKey": "courge"
          },
          {
            "key": "courge",
            "name": "Courge",
            "scientificName": "Cucurbita pepo",
            "family": "Cucurbitaceae",
            "genus": "Cucurbita",
            "lifeCycle": "Annual",
            "rootDepth": "Medium",
            "sunRequirement": "FullSun",
            "waterNeeds": "Medium"
          }
        ]
        """);

        await CreateSeeder().SeedAsync();

        var courgette = DbContext.Plants.Single(p => p.Name == "Courgette");
        courgette.ParentPlantId.ShouldNotBeNull();
    }

    ~PlantSeederTests()
    {
        if (Directory.Exists(_tempRoot))
        {
            Directory.Delete(_tempRoot, recursive: true);
        }
    }
}
