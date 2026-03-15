using GardenAssistant.Data.Entities;
using GardenAssistant.Data.Entities.Enums;
using GardenAssistant.Data.Seeders;
using GardenAssistant.Tests.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Moq;
using Shouldly;

namespace GardenAssistant.Tests.Seeds;

public class AssociationSeederTests : DatabaseTestBase
{
    private readonly string _tempRoot;
    private readonly Mock<IWebHostEnvironment> _envMock;

    public AssociationSeederTests()
    {
        _tempRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(Path.Combine(_tempRoot, "Data", "Seeds"));

        _envMock = new Mock<IWebHostEnvironment>();
        _envMock.Setup(e => e.ContentRootPath).Returns(_tempRoot);
    }

    private void WritePlantsJson(string json) =>
        File.WriteAllText(Path.Combine(_tempRoot, "Data", "Seeds", "plants.json"), json);

    private void WriteAssociationsJson(string json) =>
        File.WriteAllText(Path.Combine(_tempRoot, "Data", "Seeds", "associations.json"), json);

    private AssociationSeeder CreateSeeder() => new(DbContext, _envMock.Object);

    private static readonly string TwoPlantsJson = """
        [
          { "key": "tomate", "name": "Tomate" },
          { "key": "basilic", "name": "Basilic" }
        ]
        """;

    private async Task<(Plant tomate, Plant basilic)> SeedTwoPlantsAsync()
    {
        var tomate = new Plant { Id = Guid.NewGuid(), Name = "Tomate" };
        var basilic = new Plant { Id = Guid.NewGuid(), Name = "Basilic" };
        DbContext.Plants.AddRange(tomate, basilic);
        await DbContext.SaveChangesAsync();
        return (tomate, basilic);
    }

    [Fact]
    public async Task SeedAsync_WhenAssociationsAlreadyExist_ShouldSkip()
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
            ConfidenceLevel = ConfidenceLevel.FieldObserved
        });
        await DbContext.SaveChangesAsync();

        WritePlantsJson(TwoPlantsJson);
        WriteAssociationsJson("[]");
        await CreateSeeder().SeedAsync();

        DbContext.PlantAssociations.Count().ShouldBe(1);
    }

    [Fact]
    public async Task SeedAsync_WhenNoAssociationsExist_ShouldInsertValidAssociations()
    {
        await SeedTwoPlantsAsync();

        WritePlantsJson(TwoPlantsJson);
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

        WritePlantsJson(TwoPlantsJson);
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

        WritePlantsJson(TwoPlantsJson);
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

        WritePlantsJson(TwoPlantsJson);
        WriteAssociationsJson("[]");
        await CreateSeeder().SeedAsync();

        DbContext.PlantAssociations.Count().ShouldBe(0);
    }

    ~AssociationSeederTests()
    {
        if (Directory.Exists(_tempRoot))
        {
            Directory.Delete(_tempRoot, recursive: true);
        }
    }
}
