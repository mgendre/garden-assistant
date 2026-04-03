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

public class PlantSeederTests : DatabaseTestBase
{
    private readonly string _tempRoot;
    private readonly Mock<IWebHostEnvironment> _envMock;
    private readonly Mock<ILogger<PlantSeeder>> _loggerMock;

    public PlantSeederTests()
    {
        _tempRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(Path.Combine(_tempRoot, "Data", "Seeds"));

        _envMock = new Mock<IWebHostEnvironment>();
        _envMock.Setup(e => e.ContentRootPath).Returns(_tempRoot);

        _loggerMock = new Mock<ILogger<PlantSeeder>>();
    }

    private void WritePlantsJson(string json) =>
        File.WriteAllText(Path.Combine(_tempRoot, "Data", "Seeds", "plants.json"), json);

    private PlantSeeder CreateSeeder() => new(DbContext, _envMock.Object, _loggerMock.Object);

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
        plant.Key.ShouldBe("trefle-blanc");
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

    [Fact]
    public async Task SeedAsync_WhenPropagationMethodSpecified_ShouldMapCorrectly()
    {
        WritePlantsJson("""
        [
          {
            "key": "fraise",
            "name": "Fraise",
            "lifeCycle": "Perennial",
            "rootDepth": "Shallow",
            "sunRequirement": "FullSun",
            "waterNeeds": "Medium",
            "propagationMethod": "Division"
          }
        ]
        """);

        await CreateSeeder().SeedAsync();

        var plant = DbContext.Plants.Single();
        plant.PropagationMethod.ShouldBe(PropagationMethod.Division);
    }

    [Fact]
    public async Task SeedAsync_WhenPropagationMethodNotSpecified_ShouldDefaultToSeed()
    {
        WritePlantsJson("""
        [
          {
            "key": "tomate",
            "name": "Tomate",
            "lifeCycle": "Annual",
            "rootDepth": "Medium",
            "sunRequirement": "FullSun",
            "waterNeeds": "Medium"
          }
        ]
        """);

        await CreateSeeder().SeedAsync();

        var plant = DbContext.Plants.Single();
        plant.PropagationMethod.ShouldBe(PropagationMethod.Seed);
    }

    [Fact]
    public async Task SeedAsync_WhenFrostSensitiveSpecified_ShouldMapCorrectly()
    {
        WritePlantsJson("""
        [
          {
            "key": "tomate",
            "name": "Tomate",
            "lifeCycle": "Annual",
            "rootDepth": "Medium",
            "sunRequirement": "FullSun",
            "waterNeeds": "Medium",
            "frostSensitive": true
          }
        ]
        """);

        await CreateSeeder().SeedAsync();

        var plant = DbContext.Plants.Single();
        plant.FrostSensitive.ShouldBeTrue();
    }

    [Fact]
    public async Task SeedAsync_WhenFrostSensitiveNotSpecified_ShouldDefaultToFalse()
    {
        WritePlantsJson("""
        [
          {
            "key": "tomate",
            "name": "Tomate",
            "lifeCycle": "Annual",
            "rootDepth": "Medium",
            "sunRequirement": "FullSun",
            "waterNeeds": "Medium"
          }
        ]
        """);

        await CreateSeeder().SeedAsync();

        var plant = DbContext.Plants.Single();
        plant.FrostSensitive.ShouldBeFalse();
    }

    [Fact]
    public async Task SeedAsync_WhenMultipleVarietiesShareParent_ShouldResolveAll()
    {
        WritePlantsJson("""
        [
          {
            "key": "courge",
            "name": "Courge",
            "lifeCycle": "Annual",
            "rootDepth": "Medium",
            "sunRequirement": "FullSun",
            "waterNeeds": "Medium"
          },
          {
            "key": "courgette",
            "name": "Courgette",
            "lifeCycle": "Annual",
            "rootDepth": "Medium",
            "sunRequirement": "FullSun",
            "waterNeeds": "Medium",
            "parentKey": "courge"
          },
          {
            "key": "potimarron",
            "name": "Potimarron",
            "lifeCycle": "Annual",
            "rootDepth": "Deep",
            "sunRequirement": "FullSun",
            "waterNeeds": "Medium",
            "parentKey": "courge"
          }
        ]
        """);

        await CreateSeeder().SeedAsync();

        var plants = DbContext.Plants.ToList();
        plants.Count.ShouldBe(3);
        var courge = plants.Single(p => p.Name == "Courge");
        plants.Single(p => p.Name == "Courgette").ParentPlantId.ShouldBe(courge.Id);
        plants.Single(p => p.Name == "Potimarron").ParentPlantId.ShouldBe(courge.Id);
    }

    [Fact]
    public async Task SeedAsync_WhenPlantExistsAndNotCustomized_ShouldUpdateChangedFields()
    {
        DbContext.Plants.Add(new Plant
        {
            Id = Guid.NewGuid(),
            Key = "tomate",
            Name = "Tomate",
            HeightAtMaturityCm = 150,
            RootDepth = RootDepth.Medium,
            SunRequirement = SunRequirement.FullSun,
            WaterNeeds = WaterNeeds.Medium,
            LifeCycle = LifeCycle.Annual
        });
        await DbContext.SaveChangesAsync();

        WritePlantsJson("""
        [
          {
            "key": "tomate",
            "name": "Tomate",
            "lifeCycle": "Annual",
            "heightAtMaturityCm": 180,
            "rootDepth": "Medium",
            "sunRequirement": "FullSun",
            "waterNeeds": "Medium"
          }
        ]
        """);

        await CreateSeeder().SeedAsync();

        var plant = DbContext.Plants.Single();
        plant.HeightAtMaturityCm.ShouldBe(180);
    }

    [Fact]
    public async Task SeedAsync_WhenPlantExistsAndIsCustomized_ShouldNotModify()
    {
        DbContext.Plants.Add(new Plant
        {
            Id = Guid.NewGuid(),
            Key = "tomate",
            Name = "Tomate Perso",
            HeightAtMaturityCm = 150,
            RootDepth = RootDepth.Medium,
            SunRequirement = SunRequirement.FullSun,
            WaterNeeds = WaterNeeds.Medium,
            LifeCycle = LifeCycle.Annual,
            IsCustomized = true
        });
        await DbContext.SaveChangesAsync();

        WritePlantsJson("""
        [
          {
            "key": "tomate",
            "name": "Tomate",
            "lifeCycle": "Annual",
            "heightAtMaturityCm": 180,
            "rootDepth": "Medium",
            "sunRequirement": "FullSun",
            "waterNeeds": "Medium"
          }
        ]
        """);

        await CreateSeeder().SeedAsync();

        var plant = DbContext.Plants.Single();
        plant.Name.ShouldBe("Tomate Perso");
        plant.HeightAtMaturityCm.ShouldBe(150);
    }

    [Fact]
    public async Task SeedAsync_WhenPlantExistsWithNoChanges_ShouldNotModify()
    {
        DbContext.Plants.Add(new Plant
        {
            Id = Guid.NewGuid(),
            Key = "tomate",
            Name = "Tomate",
            HeightAtMaturityCm = 180,
            RootDepth = RootDepth.Medium,
            SunRequirement = SunRequirement.FullSun,
            WaterNeeds = WaterNeeds.Medium,
            LifeCycle = LifeCycle.Annual
        });
        await DbContext.SaveChangesAsync();

        WritePlantsJson("""
        [
          {
            "key": "tomate",
            "name": "Tomate",
            "lifeCycle": "Annual",
            "heightAtMaturityCm": 180,
            "rootDepth": "Medium",
            "sunRequirement": "FullSun",
            "waterNeeds": "Medium"
          }
        ]
        """);

        await CreateSeeder().SeedAsync();

        DbContext.Plants.Count().ShouldBe(1);
        DbContext.Plants.Single().Name.ShouldBe("Tomate");
    }

    [Fact]
    public async Task SeedAsync_WhenPlantExistsAndNotCustomized_ShouldUpdateMechanisms()
    {
        var plantId = Guid.NewGuid();
        DbContext.Plants.Add(new Plant
        {
            Id = plantId,
            Key = "basilic",
            Name = "Basilic",
            RootDepth = RootDepth.Shallow,
            SunRequirement = SunRequirement.FullSun,
            WaterNeeds = WaterNeeds.Medium,
            LifeCycle = LifeCycle.Annual
        });
        DbContext.PlantIntrinsicMechanisms.Add(new PlantIntrinsicMechanism
        {
            PlantId = plantId,
            Mechanism = AssociationMechanism.OlfactoryConfusion
        });
        await DbContext.SaveChangesAsync();

        WritePlantsJson("""
        [
          {
            "key": "basilic",
            "name": "Basilic",
            "lifeCycle": "Annual",
            "rootDepth": "Shallow",
            "sunRequirement": "FullSun",
            "waterNeeds": "Medium",
            "intrinsicMechanisms": ["PollinatorAttraction", "NitrogenFixation"]
          }
        ]
        """);

        await CreateSeeder().SeedAsync();

        var mechanisms = DbContext.PlantIntrinsicMechanisms.Where(m => m.PlantId == plantId).ToList();
        mechanisms.Count.ShouldBe(2);
        mechanisms.ShouldContain(m => m.Mechanism == AssociationMechanism.PollinatorAttraction);
        mechanisms.ShouldContain(m => m.Mechanism == AssociationMechanism.NitrogenFixation);
        mechanisms.ShouldNotContain(m => m.Mechanism == AssociationMechanism.OlfactoryConfusion);
    }

    [Fact]
    public async Task SeedAsync_WhenNewPlantInSeed_ShouldInsertAlongsideExisting()
    {
        DbContext.Plants.Add(new Plant
        {
            Id = Guid.NewGuid(),
            Key = "tomate",
            Name = "Tomate",
            RootDepth = RootDepth.Medium,
            SunRequirement = SunRequirement.FullSun,
            WaterNeeds = WaterNeeds.Medium,
            LifeCycle = LifeCycle.Annual
        });
        await DbContext.SaveChangesAsync();

        WritePlantsJson("""
        [
          {
            "key": "tomate",
            "name": "Tomate",
            "lifeCycle": "Annual",
            "rootDepth": "Medium",
            "sunRequirement": "FullSun",
            "waterNeeds": "Medium"
          },
          {
            "key": "basilic",
            "name": "Basilic",
            "lifeCycle": "Annual",
            "rootDepth": "Shallow",
            "sunRequirement": "FullSun",
            "waterNeeds": "Medium"
          }
        ]
        """);

        await CreateSeeder().SeedAsync();

        DbContext.Plants.Count().ShouldBe(2);
    }

    [Fact]
    public async Task SeedAsync_WhenRunTwice_ShouldBeIdempotent()
    {
        WritePlantsJson("""
        [
          {
            "key": "tomate",
            "name": "Tomate",
            "scientificName": "Solanum lycopersicum",
            "family": "Solanaceae",
            "genus": "Solanum",
            "lifeCycle": "Annual",
            "heightAtMaturityCm": 180,
            "rootDepth": "Medium",
            "sunRequirement": "FullSun",
            "waterNeeds": "Medium",
            "intrinsicMechanisms": ["PollinatorAttraction"]
          }
        ]
        """);

        await CreateSeeder().SeedAsync();
        await CreateSeeder().SeedAsync();

        DbContext.Plants.Count().ShouldBe(1);
        DbContext.PlantIntrinsicMechanisms.Count().ShouldBe(1);
    }

    ~PlantSeederTests()
    {
        if (Directory.Exists(_tempRoot))
        {
            Directory.Delete(_tempRoot, recursive: true);
        }
    }
}
