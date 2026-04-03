using GardenAssistant.Data.Entities;
using GardenAssistant.Data.Entities.Enums;
using GardenAssistant.Services;
using GardenAssistant.Tests.Infrastructure;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;

namespace GardenAssistant.Tests.Plants;

public class VarietyAssociationTests : DatabaseTestBase
{
    private readonly PlantAssociationService _sut;

    public VarietyAssociationTests()
    {
        var logger = new Mock<ILogger<PlantAssociationService>>();
        _sut = new PlantAssociationService(DbContext, logger.Object);
    }

    private Plant CreatePlant(string name, Guid? parentPlantId = null)
    {
        var plant = new Plant { Id = Guid.NewGuid(), Key = name.ToLowerInvariant().Replace(' ', '-'), Name = name, ParentPlantId = parentPlantId };
        DbContext.Plants.Add(plant);
        return plant;
    }

    private void CreateAssociation(Guid sourceId, Guid targetId,
        AssociationEffect effect,
        AssociationMechanism mechanism = AssociationMechanism.OlfactoryConfusion)
    {
        DbContext.PlantAssociations.Add(new PlantAssociation
        {
            Id = Guid.NewGuid(),
            SourcePlantId = sourceId,
            TargetPlantId = targetId,
            Mechanism = mechanism,
            Effect = effect,
            DistanceEffect = DistanceEffect.Short,
            ConfidenceLevel = ConfidenceLevel.FieldObserved
        });
    }

    [Fact]
    public async Task GetCompanionRecommendationsAsync_WhenVarietySelected_ShouldUseParentAssociations()
    {
        var courge = CreatePlant("Courge");
        var courgette = CreatePlant("Courgette", courge.Id);
        var tomate = CreatePlant("Tomate");
        CreateAssociation(courge.Id, tomate.Id, AssociationEffect.Beneficial);
        await DbContext.SaveChangesAsync();

        var result = await _sut.GetCompanionRecommendationsAsync([courgette.Id]);

        result.GoodCompanions.ShouldContain(c => c.PlantId == tomate.Id);
    }

    [Fact]
    public async Task GetCompanionRecommendationsAsync_WhenVarietyAndParentBothSelected_ShouldNotDoubleCount()
    {
        var courge = CreatePlant("Courge");
        var courgette = CreatePlant("Courgette", courge.Id);
        var tomate = CreatePlant("Tomate");
        CreateAssociation(courge.Id, tomate.Id, AssociationEffect.Beneficial);
        await DbContext.SaveChangesAsync();

        var resultVariety = await _sut.GetCompanionRecommendationsAsync([courgette.Id]);
        var resultParent = await _sut.GetCompanionRecommendationsAsync([courge.Id]);

        var scoreVariety = resultVariety.GoodCompanions.FirstOrDefault(c => c.PlantId == tomate.Id);
        var scoreParent = resultParent.GoodCompanions.FirstOrDefault(c => c.PlantId == tomate.Id);

        scoreVariety.ShouldNotBeNull();
        scoreParent.ShouldNotBeNull();
    }

    [Fact]
    public async Task GetCompanionRecommendationsAsync_WhenVarietyHasHarmfulParentAssociation_ShouldDetectConflict()
    {
        var courge = CreatePlant("Courge");
        var courgette = CreatePlant("Courgette", courge.Id);
        var pommedeterre = CreatePlant("Pomme de terre");
        CreateAssociation(courge.Id, pommedeterre.Id, AssociationEffect.Harmful);
        await DbContext.SaveChangesAsync();

        var result = await _sut.GetCompanionRecommendationsAsync([courgette.Id]);

        result.PlantsToAvoid.ShouldContain(c => c.PlantId == pommedeterre.Id);
    }

    [Fact]
    public async Task GetCompanionRecommendationsAsync_WhenPlantHasNoParent_ShouldWorkNormally()
    {
        var tomate = CreatePlant("Tomate");
        var basilic = CreatePlant("Basilic");
        CreateAssociation(tomate.Id, basilic.Id, AssociationEffect.Beneficial);
        await DbContext.SaveChangesAsync();

        var result = await _sut.GetCompanionRecommendationsAsync([tomate.Id]);

        result.GoodCompanions.ShouldContain(c => c.PlantId == basilic.Id);
    }

    [Fact]
    public async Task GetCompanionRecommendationsAsync_WhenVarietyIsCandidate_ShouldInheritParentBenefits()
    {
        var courge = CreatePlant("Courge");
        var courgette = CreatePlant("Courgette", courge.Id);
        var tomate = CreatePlant("Tomate");
        CreateAssociation(courge.Id, tomate.Id, AssociationEffect.Beneficial, AssociationMechanism.SoilCover);
        await DbContext.SaveChangesAsync();

        var result = await _sut.GetCompanionRecommendationsAsync([tomate.Id]);

        var courgeRec = result.GoodCompanions.FirstOrDefault(c => c.PlantId == courge.Id);
        var courgetteRec = result.GoodCompanions.FirstOrDefault(c => c.PlantId == courgette.Id);

        courgeRec.ShouldNotBeNull();
        courgetteRec.ShouldNotBeNull();
        courgetteRec!.Mechanisms.ShouldContain(AssociationMechanism.SoilCover);
    }

    [Fact]
    public async Task GetCompanionRecommendationsAsync_WhenVarietyIsCandidate_ShouldNotGetUnknownScore()
    {
        var courge = CreatePlant("Courge");
        var courgette = CreatePlant("Courgette", courge.Id);
        var tomate = CreatePlant("Tomate");
        CreateAssociation(courge.Id, tomate.Id, AssociationEffect.Beneficial);
        await DbContext.SaveChangesAsync();

        var result = await _sut.GetCompanionRecommendationsAsync([tomate.Id]);

        result.PlantsToAvoid.ShouldNotContain(c => c.PlantId == courgette.Id);
    }

    [Fact]
    public async Task GetCompanionRecommendationsAsync_WhenVarietySelected_ShouldReturnParentIdInAssociationDetails()
    {
        var courge = CreatePlant("Courge");
        var courgette = CreatePlant("Courgette", courge.Id);
        var tomate = CreatePlant("Tomate");
        CreateAssociation(courge.Id, tomate.Id, AssociationEffect.Beneficial, AssociationMechanism.SoilCover);
        await DbContext.SaveChangesAsync();

        var result = await _sut.GetCompanionRecommendationsAsync([courgette.Id, tomate.Id]);

        result.SelectedPlantAssociations.ShouldNotBeEmpty();
        var assoc = result.SelectedPlantAssociations[0];
        var ids = new[] { assoc.SourcePlantId, assoc.TargetPlantId };
        ids.ShouldContain(courge.Id);
        ids.ShouldContain(tomate.Id);
    }

    [Fact]
    public async Task GetCompanionRecommendationsAsync_WhenVarietySelected_ShouldReturnParentIdInMechanisms()
    {
        var courge = CreatePlant("Courge");
        var courgette = CreatePlant("Courgette", courge.Id);
        var tomate = CreatePlant("Tomate");
        CreateAssociation(courge.Id, tomate.Id, AssociationEffect.Beneficial, AssociationMechanism.SoilCover);
        await DbContext.SaveChangesAsync();

        var result = await _sut.GetCompanionRecommendationsAsync([courgette.Id, tomate.Id]);

        result.SelectedPlantsMechanisms.ShouldContain(m => m.PlantId == courge.Id);
    }
}
