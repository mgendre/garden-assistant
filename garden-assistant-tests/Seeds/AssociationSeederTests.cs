using GardenAssistant.Data.Entities;
using GardenAssistant.Data.Entities.Enums;
using GardenAssistant.Data.Seeders;
using GardenAssistant.Tests.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;

namespace GardenAssistant.Tests.Seeds;

public class AssociationSeederTests : DatabaseTestBase
{
    private readonly string _tempRoot;
    private readonly Mock<IWebHostEnvironment> _envMock;
    private readonly Mock<ILogger<AssociationSeeder>> _loggerMock;

    public AssociationSeederTests()
    {
        _tempRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(Path.Combine(_tempRoot, "Data", "Seeds"));

        _envMock = new Mock<IWebHostEnvironment>();
        _envMock.Setup(e => e.ContentRootPath).Returns(_tempRoot);

        _loggerMock = new Mock<ILogger<AssociationSeeder>>();
    }

    private void WriteAssociationsJson(string json) =>
        File.WriteAllText(Path.Combine(_tempRoot, "Data", "Seeds", "associations.json"), json);

    private AssociationSeeder CreateSeeder() => new(DbContext, _envMock.Object, _loggerMock.Object);

    private async Task<(Plant tomate, Plant basilic)> SeedTwoPlantsAsync()
    {
        var tomate = new Plant { Id = Guid.NewGuid(), Key = "tomate", Name = "Tomate" };
        var basilic = new Plant { Id = Guid.NewGuid(), Key = "basilic", Name = "Basilic" };
        DbContext.Plants.AddRange(tomate, basilic);
        await DbContext.SaveChangesAsync();
        return (tomate, basilic);
    }

    [Fact]
    public async Task SeedAsync_WhenNoAssociationsExist_ShouldInsertValidAssociations()
    {
        await SeedTwoPlantsAsync();

        WriteAssociationsJson("""
        [
          {
            "sourcePlantKey": "tomate",
            "targetPlantKey": "basilic",
            "mechanism": "OlfactoryConfusion",
            "effect": "Beneficial",
            "distanceEffect": "Contact",
            "confidenceLevel": "FieldObserved",
            "notes": "Classique tomate-basilic"
          }
        ]
        """);

        await CreateSeeder().SeedAsync();

        DbContext.PlantAssociations.Count().ShouldBe(1);
    }

    [Fact]
    public async Task SeedAsync_WhenNoAssociationsExist_ShouldMapFieldsCorrectly()
    {
        var (tomate, basilic) = await SeedTwoPlantsAsync();

        WriteAssociationsJson("""
        [
          {
            "sourcePlantKey": "tomate",
            "targetPlantKey": "basilic",
            "mechanism": "PollinatorAttraction",
            "effect": "Beneficial",
            "distanceEffect": "Short",
            "confidenceLevel": "PeerReviewed",
            "notes": "Note de test"
          }
        ]
        """);

        await CreateSeeder().SeedAsync();

        var assoc = DbContext.PlantAssociations.Single();
        assoc.SourcePlantId.ShouldBe(tomate.Id);
        assoc.TargetPlantId.ShouldBe(basilic.Id);
        assoc.Mechanism.ShouldBe(AssociationMechanism.PollinatorAttraction);
        assoc.Effect.ShouldBe(AssociationEffect.Beneficial);
        assoc.ConfidenceLevel.ShouldBe(ConfidenceLevel.PeerReviewed);
        assoc.Notes.ShouldBe("Note de test");
    }

    [Fact]
    public async Task SeedAsync_WhenPlantKeyNotFound_ShouldSkipThatAssociation()
    {
        await SeedTwoPlantsAsync();

        WriteAssociationsJson("""
        [
          {
            "sourcePlantKey": "tomate",
            "targetPlantKey": "inconnue",
            "mechanism": "OlfactoryConfusion",
            "effect": "Beneficial",
            "distanceEffect": "Contact",
            "confidenceLevel": "Anecdotal",
            "notes": null
          },
          {
            "sourcePlantKey": "tomate",
            "targetPlantKey": "basilic",
            "mechanism": "OlfactoryConfusion",
            "effect": "Beneficial",
            "distanceEffect": "Contact",
            "confidenceLevel": "Anecdotal",
            "notes": null
          }
        ]
        """);

        await CreateSeeder().SeedAsync();

        DbContext.PlantAssociations.Count().ShouldBe(1);
    }

    [Fact]
    public async Task SeedAsync_WhenJsonIsEmpty_ShouldInsertNothing()
    {
        await SeedTwoPlantsAsync();

        WriteAssociationsJson("[]");
        await CreateSeeder().SeedAsync();

        DbContext.PlantAssociations.Count().ShouldBe(0);
    }

    [Fact]
    public async Task SeedAsync_WhenMultipleAssociations_ShouldInsertAll()
    {
        await SeedTwoPlantsAsync();

        WriteAssociationsJson("""
        [
          {
            "sourcePlantKey": "tomate",
            "targetPlantKey": "basilic",
            "mechanism": "OlfactoryConfusion",
            "effect": "Beneficial",
            "distanceEffect": "Contact",
            "confidenceLevel": "FieldObserved",
            "notes": null
          },
          {
            "sourcePlantKey": "basilic",
            "targetPlantKey": "tomate",
            "mechanism": "PollinatorAttraction",
            "effect": "Beneficial",
            "distanceEffect": "Short",
            "confidenceLevel": "PeerReviewed",
            "notes": "Attire les pollinisateurs"
          }
        ]
        """);

        await CreateSeeder().SeedAsync();

        DbContext.PlantAssociations.Count().ShouldBe(2);
    }

    [Fact]
    public async Task SeedAsync_WhenAssociationExistsAndNotLocked_ShouldUpdateFields()
    {
        var (tomate, basilic) = await SeedTwoPlantsAsync();
        DbContext.PlantAssociations.Add(new PlantAssociation
        {
            Id = Guid.NewGuid(),
            SourcePlantId = tomate.Id,
            TargetPlantId = basilic.Id,
            Mechanism = AssociationMechanism.OlfactoryConfusion,
            Effect = AssociationEffect.Beneficial,
            DistanceEffect = DistanceEffect.Contact,
            ConfidenceLevel = ConfidenceLevel.Anecdotal,
            Notes = "Old notes"
        });
        await DbContext.SaveChangesAsync();

        WriteAssociationsJson("""
        [
          {
            "sourcePlantKey": "tomate",
            "targetPlantKey": "basilic",
            "mechanism": "OlfactoryConfusion",
            "effect": "Beneficial",
            "distanceEffect": "Short",
            "confidenceLevel": "PeerReviewed",
            "notes": "Updated notes"
          }
        ]
        """);

        await CreateSeeder().SeedAsync();

        var assoc = DbContext.PlantAssociations.Single();
        assoc.DistanceEffect.ShouldBe(DistanceEffect.Short);
        assoc.ConfidenceLevel.ShouldBe(ConfidenceLevel.PeerReviewed);
        assoc.Notes.ShouldBe("Updated notes");
    }

    [Fact]
    public async Task SeedAsync_WhenSourcePlantIsCustomized_ShouldSkipAssociation()
    {
        var tomate = new Plant { Id = Guid.NewGuid(), Key = "tomate", Name = "Tomate", IsCustomized = true };
        var basilic = new Plant { Id = Guid.NewGuid(), Key = "basilic", Name = "Basilic" };
        DbContext.Plants.AddRange(tomate, basilic);
        await DbContext.SaveChangesAsync();

        WriteAssociationsJson("""
        [
          {
            "sourcePlantKey": "tomate",
            "targetPlantKey": "basilic",
            "mechanism": "OlfactoryConfusion",
            "effect": "Beneficial",
            "distanceEffect": "Contact",
            "confidenceLevel": "FieldObserved",
            "notes": null
          }
        ]
        """);

        await CreateSeeder().SeedAsync();

        DbContext.PlantAssociations.Count().ShouldBe(0);
    }

    [Fact]
    public async Task SeedAsync_WhenTargetPlantIsCustomized_ShouldSkipAssociation()
    {
        var tomate = new Plant { Id = Guid.NewGuid(), Key = "tomate", Name = "Tomate" };
        var basilic = new Plant { Id = Guid.NewGuid(), Key = "basilic", Name = "Basilic", IsCustomized = true };
        DbContext.Plants.AddRange(tomate, basilic);
        await DbContext.SaveChangesAsync();

        WriteAssociationsJson("""
        [
          {
            "sourcePlantKey": "tomate",
            "targetPlantKey": "basilic",
            "mechanism": "OlfactoryConfusion",
            "effect": "Beneficial",
            "distanceEffect": "Contact",
            "confidenceLevel": "FieldObserved",
            "notes": null
          }
        ]
        """);

        await CreateSeeder().SeedAsync();

        DbContext.PlantAssociations.Count().ShouldBe(0);
    }

    [Fact]
    public async Task SeedAsync_WhenRunTwice_ShouldBeIdempotent()
    {
        await SeedTwoPlantsAsync();

        WriteAssociationsJson("""
        [
          {
            "sourcePlantKey": "tomate",
            "targetPlantKey": "basilic",
            "mechanism": "OlfactoryConfusion",
            "effect": "Beneficial",
            "distanceEffect": "Contact",
            "confidenceLevel": "FieldObserved",
            "notes": null
          }
        ]
        """);

        await CreateSeeder().SeedAsync();
        await CreateSeeder().SeedAsync();

        DbContext.PlantAssociations.Count().ShouldBe(1);
    }

    ~AssociationSeederTests()
    {
        if (Directory.Exists(_tempRoot))
        {
            Directory.Delete(_tempRoot, recursive: true);
        }
    }
}
