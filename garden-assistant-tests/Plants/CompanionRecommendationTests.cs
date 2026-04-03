using GardenAssistant.Data.Entities;
using GardenAssistant.Data.Entities.Enums;
using GardenAssistant.Services;
using GardenAssistant.Tests.Infrastructure;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;

namespace GardenAssistant.Tests.Plants;

public class CompanionRecommendationTests : DatabaseTestBase
{
    private readonly PlantAssociationService _sut;

    public CompanionRecommendationTests()
    {
        var logger = new Mock<ILogger<PlantAssociationService>>();
        _sut = new PlantAssociationService(DbContext, logger.Object);
    }

    private Plant CreatePlant(string name, string? scientificName = null)
    {
        var plant = new Plant { Id = Guid.NewGuid(), Key = name.ToLowerInvariant().Replace(' ', '-'), Name = name, ScientificName = scientificName };
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
    public async Task GetCompanionRecommendationsAsync_WhenOnlySelectedPlant_ShouldReturnItInResults()
    {
        var plant = CreatePlant("Tomato");
        await DbContext.SaveChangesAsync();

        var result = await _sut.GetCompanionRecommendationsAsync([plant.Id]);

        result.GoodCompanions.ShouldContain(c => c.PlantId == plant.Id);
        result.PlantsToAvoid.ShouldBeEmpty();
        result.SelectedPlantConflicts.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetCompanionRecommendationsAsync_WhenBeneficialAssociationExists_ShouldRankHigher()
    {
        var tomato = CreatePlant("Tomato");
        var basil = CreatePlant("Basil");
        var neutral = CreatePlant("Neutral Plant");
        CreateAssociation(tomato.Id, basil.Id, AssociationEffect.Beneficial);
        CreateAssociation(tomato.Id, neutral.Id, AssociationEffect.Neutral);
        await DbContext.SaveChangesAsync();

        var result = await _sut.GetCompanionRecommendationsAsync([tomato.Id]);

        result.GoodCompanions.Count.ShouldBe(3);
        var nonSelected = result.GoodCompanions.Where(c => c.PlantId != tomato.Id).ToList();
        nonSelected[0].PlantId.ShouldBe(basil.Id);
    }

    [Fact]
    public async Task GetCompanionRecommendationsAsync_WhenNoAssociation_ShouldRankBelowNeutral()
    {
        var tomato = CreatePlant("Tomato");
        var unknown = CreatePlant("Unknown Plant");
        var neutral = CreatePlant("Neutral Plant");
        CreateAssociation(tomato.Id, neutral.Id, AssociationEffect.Neutral);
        await DbContext.SaveChangesAsync();

        var result = await _sut.GetCompanionRecommendationsAsync([tomato.Id]);

        var neutralIndex = result.GoodCompanions.FindIndex(r => r.PlantId == neutral.Id);
        var unknownIndex = result.GoodCompanions.FindIndex(r => r.PlantId == unknown.Id);
        neutralIndex.ShouldBeLessThan(unknownIndex);
    }

    [Fact]
    public async Task GetCompanionRecommendationsAsync_ShouldReturnAllPlants()
    {
        var selected = CreatePlant("Selected");
        for (var i = 0; i < 15; i++)
        {
            CreatePlant($"Candidate {i}");
        }
        await DbContext.SaveChangesAsync();

        var result = await _sut.GetCompanionRecommendationsAsync([selected.Id]);

        result.GoodCompanions.Count.ShouldBe(16);
    }

    [Fact]
    public async Task GetCompanionRecommendationsAsync_ShouldIncludeSelectedPlants()
    {
        var tomato = CreatePlant("Tomato");
        var basil = CreatePlant("Basil");
        var carrot = CreatePlant("Carrot");
        await DbContext.SaveChangesAsync();

        var result = await _sut.GetCompanionRecommendationsAsync([tomato.Id, basil.Id]);

        result.GoodCompanions.ShouldContain(r => r.PlantId == tomato.Id);
        result.GoodCompanions.ShouldContain(r => r.PlantId == basil.Id);
        result.GoodCompanions.ShouldContain(r => r.PlantId == carrot.Id);
    }

    [Fact]
    public async Task GetCompanionRecommendationsAsync_WhenBeneficialAssociation_ShouldReturnPlantId()
    {
        var tomato = CreatePlant("Tomato");
        var basil = CreatePlant("Basil", "Ocimum basilicum");
        CreateAssociation(tomato.Id, basil.Id, AssociationEffect.Beneficial);
        await DbContext.SaveChangesAsync();

        var result = await _sut.GetCompanionRecommendationsAsync([tomato.Id]);

        result.GoodCompanions[0].PlantId.ShouldBe(basil.Id);
    }

    [Fact]
    public async Task GetCompanionRecommendationsAsync_WithMultipleSelected_ShouldAggregateScores()
    {
        var tomato = CreatePlant("Tomato");
        var carrot = CreatePlant("Carrot");
        var basil = CreatePlant("Basil");
        var fennel = CreatePlant("Fennel");
        CreateAssociation(tomato.Id, basil.Id, AssociationEffect.Beneficial);
        CreateAssociation(carrot.Id, basil.Id, AssociationEffect.Beneficial);
        CreateAssociation(tomato.Id, fennel.Id, AssociationEffect.Beneficial);
        CreateAssociation(carrot.Id, fennel.Id, AssociationEffect.Harmful);
        await DbContext.SaveChangesAsync();

        var result = await _sut.GetCompanionRecommendationsAsync([tomato.Id, carrot.Id]);

        result.GoodCompanions[0].PlantId.ShouldBe(basil.Id);
    }

    [Fact]
    public async Task GetCompanionRecommendationsAsync_WhenMultipleAssociationsForSamePair_ShouldRankFirst()
    {
        var tomato = CreatePlant("Tomato");
        var basil = CreatePlant("Basil");
        var carrot = CreatePlant("Carrot");
        CreateAssociation(tomato.Id, basil.Id, AssociationEffect.Beneficial,
            AssociationMechanism.OlfactoryConfusion);
        CreateAssociation(tomato.Id, basil.Id, AssociationEffect.Beneficial,
            AssociationMechanism.PredatorAttraction);
        CreateAssociation(tomato.Id, carrot.Id, AssociationEffect.Beneficial,
            AssociationMechanism.OlfactoryConfusion);
        await DbContext.SaveChangesAsync();

        var result = await _sut.GetCompanionRecommendationsAsync([tomato.Id]);

        result.GoodCompanions[0].PlantId.ShouldBe(basil.Id);
    }

    [Fact]
    public async Task GetCompanionRecommendationsAsync_ShouldHandleBidirectionalAssociations()
    {
        var tomato = CreatePlant("Tomato");
        var basil = CreatePlant("Basil");
        CreateAssociation(basil.Id, tomato.Id, AssociationEffect.Beneficial);
        await DbContext.SaveChangesAsync();

        var result = await _sut.GetCompanionRecommendationsAsync([tomato.Id]);

        result.GoodCompanions[0].PlantId.ShouldBe(basil.Id);
        result.GoodCompanions[0].Mechanisms.ShouldNotBeEmpty();
    }

    [Fact]
    public async Task GetCompanionRecommendationsAsync_WhenFewPlants_ShouldReturnAll()
    {
        var selected = CreatePlant("Selected");
        CreatePlant("A");
        CreatePlant("B");
        await DbContext.SaveChangesAsync();

        var result = await _sut.GetCompanionRecommendationsAsync([selected.Id]);

        result.GoodCompanions.Count.ShouldBe(3);
    }

    [Fact]
    public async Task GetCompanionRecommendationsAsync_WhenHarmfulAssociationExists_ShouldIncludeInBothLists()
    {
        var tomato = CreatePlant("Tomato");
        var fennel = CreatePlant("Fennel");
        var basil = CreatePlant("Basil");
        CreateAssociation(tomato.Id, fennel.Id, AssociationEffect.Harmful, AssociationMechanism.RootAllelopathy);
        CreateAssociation(tomato.Id, basil.Id, AssociationEffect.Beneficial);
        await DbContext.SaveChangesAsync();

        var result = await _sut.GetCompanionRecommendationsAsync([tomato.Id]);

        result.GoodCompanions.ShouldContain(c => c.PlantId == fennel.Id);
        result.GoodCompanions.ShouldContain(c => c.PlantId == basil.Id);
        result.PlantsToAvoid.ShouldContain(p => p.PlantId == fennel.Id);
    }

    [Fact]
    public async Task GetCompanionRecommendationsAsync_WhenNoHarmfulAssociations_ShouldReturnEmptyPlantsToAvoid()
    {
        var tomato = CreatePlant("Tomato");
        var basil = CreatePlant("Basil");
        CreateAssociation(tomato.Id, basil.Id, AssociationEffect.Beneficial);
        await DbContext.SaveChangesAsync();

        var result = await _sut.GetCompanionRecommendationsAsync([tomato.Id]);

        result.PlantsToAvoid.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetCompanionRecommendationsAsync_ShouldSortByScoreDescending()
    {
        var tomato = CreatePlant("Tomato");
        var basil = CreatePlant("Basil");
        var carrot = CreatePlant("Carrot");
        CreateAssociation(tomato.Id, basil.Id, AssociationEffect.Beneficial);
        CreateAssociation(tomato.Id, carrot.Id, AssociationEffect.Beneficial);
        CreateAssociation(tomato.Id, carrot.Id, AssociationEffect.Beneficial,
            AssociationMechanism.PredatorAttraction);
        await DbContext.SaveChangesAsync();

        var result = await _sut.GetCompanionRecommendationsAsync([tomato.Id]);

        result.GoodCompanions[0].PlantId.ShouldBe(carrot.Id);
        result.GoodCompanions[1].PlantId.ShouldBe(basil.Id);
    }

    [Fact]
    public async Task GetCompanionRecommendationsAsync_WhenHarmfulWithMultipleMechanisms_ShouldIncludeInBothLists()
    {
        var tomato = CreatePlant("Tomato");
        var fennel = CreatePlant("Fennel");
        var basil = CreatePlant("Basil");
        CreateAssociation(tomato.Id, fennel.Id, AssociationEffect.Harmful, AssociationMechanism.RootAllelopathy);
        CreateAssociation(tomato.Id, fennel.Id, AssociationEffect.Harmful, AssociationMechanism.AerialRepulsion);
        CreateAssociation(tomato.Id, basil.Id, AssociationEffect.Beneficial);
        await DbContext.SaveChangesAsync();

        var result = await _sut.GetCompanionRecommendationsAsync([tomato.Id]);

        result.GoodCompanions.ShouldContain(c => c.PlantId == fennel.Id);
        result.GoodCompanions.ShouldContain(c => c.PlantId == basil.Id);
        result.PlantsToAvoid.ShouldContain(p => p.PlantId == fennel.Id);
        result.PlantsToAvoid.First(p => p.PlantId == fennel.Id).Mechanisms.Count.ShouldBe(2);
    }

    [Fact]
    public async Task GetCompanionRecommendationsAsync_WhenHarmfulWithAnySelectedPlant_ShouldIncludeInBothLists()
    {
        var tomato = CreatePlant("Tomato");
        var carrot = CreatePlant("Carrot");
        var fennel = CreatePlant("Fennel");
        var basil = CreatePlant("Basil");
        CreateAssociation(tomato.Id, fennel.Id, AssociationEffect.Beneficial);
        CreateAssociation(carrot.Id, fennel.Id, AssociationEffect.Harmful, AssociationMechanism.RootAllelopathy);
        CreateAssociation(tomato.Id, basil.Id, AssociationEffect.Beneficial);
        CreateAssociation(carrot.Id, basil.Id, AssociationEffect.Beneficial);
        await DbContext.SaveChangesAsync();

        var result = await _sut.GetCompanionRecommendationsAsync([tomato.Id, carrot.Id]);

        result.GoodCompanions.ShouldContain(c => c.PlantId == fennel.Id);
        result.GoodCompanions.ShouldContain(c => c.PlantId == basil.Id);
        result.PlantsToAvoid.ShouldContain(p => p.PlantId == fennel.Id);
    }

    [Fact]
    public async Task GetCompanionRecommendationsAsync_WhenBeneficialMechanismsExist_ShouldIncludeMechanisms()
    {
        var tomato = CreatePlant("Tomato");
        var basil = CreatePlant("Basil");
        CreateAssociation(tomato.Id, basil.Id, AssociationEffect.Beneficial,
            AssociationMechanism.OlfactoryConfusion);
        CreateAssociation(tomato.Id, basil.Id, AssociationEffect.Beneficial,
            AssociationMechanism.PredatorAttraction);
        await DbContext.SaveChangesAsync();

        var result = await _sut.GetCompanionRecommendationsAsync([tomato.Id]);

        result.GoodCompanions[0].Mechanisms.Count.ShouldBe(2);
        result.GoodCompanions[0].Mechanisms.ShouldContain(AssociationMechanism.OlfactoryConfusion);
        result.GoodCompanions[0].Mechanisms.ShouldContain(AssociationMechanism.PredatorAttraction);
    }

    [Fact]
    public async Task GetCompanionRecommendationsAsync_WhenNoBeneficialMechanisms_ShouldReturnEmptyMechanisms()
    {
        var tomato = CreatePlant("Tomato");
        var basil = CreatePlant("Basil");
        CreateAssociation(tomato.Id, basil.Id, AssociationEffect.Neutral);
        await DbContext.SaveChangesAsync();

        var result = await _sut.GetCompanionRecommendationsAsync([tomato.Id]);

        result.GoodCompanions[0].Mechanisms.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetCompanionRecommendationsAsync_WhenMixedEffects_ShouldOnlyCollectBeneficialMechanisms()
    {
        var tomato = CreatePlant("Tomato");
        var carrot = CreatePlant("Carrot");
        var basil = CreatePlant("Basil");
        CreateAssociation(tomato.Id, basil.Id, AssociationEffect.Beneficial,
            AssociationMechanism.OlfactoryConfusion);
        CreateAssociation(carrot.Id, basil.Id, AssociationEffect.Neutral,
            AssociationMechanism.NitrogenFixation);
        await DbContext.SaveChangesAsync();

        var result = await _sut.GetCompanionRecommendationsAsync([tomato.Id, carrot.Id]);

        var basilRec = result.GoodCompanions.First(c => c.PlantId == basil.Id);
        basilRec.Mechanisms.Count.ShouldBe(1);
        basilRec.Mechanisms.ShouldContain(AssociationMechanism.OlfactoryConfusion);
    }

    [Fact]
    public async Task GetCompanionRecommendationsAsync_WhenBeneficialFromMultipleSelected_ShouldCollectAllMechanisms()
    {
        var tomato = CreatePlant("Tomato");
        var carrot = CreatePlant("Carrot");
        var basil = CreatePlant("Basil");
        CreateAssociation(tomato.Id, basil.Id, AssociationEffect.Beneficial,
            AssociationMechanism.OlfactoryConfusion);
        CreateAssociation(carrot.Id, basil.Id, AssociationEffect.Beneficial,
            AssociationMechanism.NitrogenFixation);
        await DbContext.SaveChangesAsync();

        var result = await _sut.GetCompanionRecommendationsAsync([tomato.Id, carrot.Id]);

        var basilRec = result.GoodCompanions.First(c => c.PlantId == basil.Id);
        basilRec.Mechanisms.Count.ShouldBe(2);
        basilRec.Mechanisms.ShouldContain(AssociationMechanism.OlfactoryConfusion);
        basilRec.Mechanisms.ShouldContain(AssociationMechanism.NitrogenFixation);
    }

    [Fact]
    public async Task GetCompanionRecommendationsAsync_WhenDuplicateBeneficialMechanism_ShouldDeduplicateMechanisms()
    {
        var tomato = CreatePlant("Tomato");
        var carrot = CreatePlant("Carrot");
        var basil = CreatePlant("Basil");
        CreateAssociation(tomato.Id, basil.Id, AssociationEffect.Beneficial,
            AssociationMechanism.OlfactoryConfusion);
        CreateAssociation(carrot.Id, basil.Id, AssociationEffect.Beneficial,
            AssociationMechanism.OlfactoryConfusion);
        await DbContext.SaveChangesAsync();

        var result = await _sut.GetCompanionRecommendationsAsync([tomato.Id, carrot.Id]);

        var basilRec = result.GoodCompanions.First(c => c.PlantId == basil.Id);
        basilRec.Mechanisms.Count.ShouldBe(1);
    }

    [Fact]
    public async Task GetCompanionRecommendationsAsync_WhenSelectedPlantsConflict_ShouldReturnSelectedPlantConflicts()
    {
        var tomato = CreatePlant("Tomato");
        var fennel = CreatePlant("Fennel");
        CreatePlant("Bystander");
        CreateAssociation(tomato.Id, fennel.Id, AssociationEffect.Harmful, AssociationMechanism.RootAllelopathy);
        await DbContext.SaveChangesAsync();

        var result = await _sut.GetCompanionRecommendationsAsync([tomato.Id, fennel.Id]);

        result.SelectedPlantConflicts.Count.ShouldBe(1);
        result.SelectedPlantConflicts[0].Mechanisms.ShouldContain(AssociationMechanism.RootAllelopathy);
    }

    [Fact]
    public async Task GetCompanionRecommendationsAsync_WhenSelectedPlantsDoNotConflict_ShouldReturnEmptyConflicts()
    {
        var tomato = CreatePlant("Tomato");
        var basil = CreatePlant("Basil");
        CreatePlant("Bystander");
        CreateAssociation(tomato.Id, basil.Id, AssociationEffect.Beneficial);
        await DbContext.SaveChangesAsync();

        var result = await _sut.GetCompanionRecommendationsAsync([tomato.Id, basil.Id]);

        result.SelectedPlantConflicts.ShouldBeEmpty();
    }
}
