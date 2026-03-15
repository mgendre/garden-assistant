using GardenAssistant.Data.Entities;
using GardenAssistant.Data.Entities.Enums;
using GardenAssistant.Services;
using GardenAssistant.Tests.Infrastructure;
using Shouldly;

namespace GardenAssistant.Tests.Plants;

public class CompanionRecommendationTests : DatabaseTestBase
{
    private readonly PlantAssociationService _sut;

    public CompanionRecommendationTests()
    {
        _sut = new PlantAssociationService(DbContext);
    }

    private Plant CreatePlant(string name, string? scientificName = null)
    {
        var plant = new Plant { Id = Guid.NewGuid(), Name = name, ScientificName = scientificName };
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
    public async Task GetCompanionRecommendationsAsync_WhenNoCandidates_ShouldReturnEmpty()
    {
        var plant = CreatePlant("Tomato");
        await DbContext.SaveChangesAsync();

        var result = await _sut.GetCompanionRecommendationsAsync([plant.Id]);

        result.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetCompanionRecommendationsAsync_WhenBeneficialAssociationExists_ShouldRankHigher()
    {
        var tomato = CreatePlant("Tomato");
        var basil = CreatePlant("Basil");
        var fennel = CreatePlant("Fennel");
        CreateAssociation(tomato.Id, basil.Id, AssociationEffect.Beneficial);
        CreateAssociation(tomato.Id, fennel.Id, AssociationEffect.Harmful);
        await DbContext.SaveChangesAsync();

        var result = await _sut.GetCompanionRecommendationsAsync([tomato.Id]);

        result.Count.ShouldBe(2);
        result[0].PlantId.ShouldBe(basil.Id);
        result[1].PlantId.ShouldBe(fennel.Id);
    }

    [Fact]
    public async Task GetCompanionRecommendationsAsync_WhenNoAssociation_ShouldScoreSlightlyBelowNeutral()
    {
        var tomato = CreatePlant("Tomato");
        var unknown = CreatePlant("Unknown Plant");
        var neutral = CreatePlant("Neutral Plant");
        CreateAssociation(tomato.Id, neutral.Id, AssociationEffect.Neutral);
        await DbContext.SaveChangesAsync();

        var result = await _sut.GetCompanionRecommendationsAsync([tomato.Id]);

        var neutralScore = result.First(r => r.PlantId == neutral.Id).Score;
        var unknownScore = result.First(r => r.PlantId == unknown.Id).Score;
        neutralScore.ShouldBeGreaterThan(unknownScore);
    }

    [Fact]
    public async Task GetCompanionRecommendationsAsync_ShouldReturnAtMost10()
    {
        var selected = CreatePlant("Selected");
        for (var i = 0; i < 15; i++)
            CreatePlant($"Candidate {i}");
        await DbContext.SaveChangesAsync();

        var result = await _sut.GetCompanionRecommendationsAsync([selected.Id]);

        result.Count.ShouldBe(10);
    }

    [Fact]
    public async Task GetCompanionRecommendationsAsync_ShouldExcludeSelectedPlants()
    {
        var tomato = CreatePlant("Tomato");
        var basil = CreatePlant("Basil");
        var carrot = CreatePlant("Carrot");
        await DbContext.SaveChangesAsync();

        var result = await _sut.GetCompanionRecommendationsAsync([tomato.Id, basil.Id]);

        result.Select(r => r.PlantId).ShouldNotContain(tomato.Id);
        result.Select(r => r.PlantId).ShouldNotContain(basil.Id);
        result.ShouldContain(r => r.PlantId == carrot.Id);
    }

    [Fact]
    public async Task GetCompanionRecommendationsAsync_ShouldReturnPlantNameAndScientificName()
    {
        var tomato = CreatePlant("Tomato");
        var basil = CreatePlant("Basil", "Ocimum basilicum");
        CreateAssociation(tomato.Id, basil.Id, AssociationEffect.Beneficial);
        await DbContext.SaveChangesAsync();

        var result = await _sut.GetCompanionRecommendationsAsync([tomato.Id]);

        result[0].PlantName.ShouldBe("Basil");
        result[0].ScientificName.ShouldBe("Ocimum basilicum");
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

        result[0].PlantId.ShouldBe(basil.Id);
    }

    [Fact]
    public async Task GetCompanionRecommendationsAsync_ShouldFavorCandidatesCompatibleWithEachOther()
    {
        var selected = CreatePlant("Selected");
        var goodAlone = CreatePlant("Good Alone");
        var teamA = CreatePlant("Team A");
        var teamB = CreatePlant("Team B");

        CreateAssociation(selected.Id, goodAlone.Id, AssociationEffect.Beneficial);
        CreateAssociation(selected.Id, teamA.Id, AssociationEffect.Beneficial);
        CreateAssociation(selected.Id, teamB.Id, AssociationEffect.Beneficial);

        CreateAssociation(goodAlone.Id, teamA.Id, AssociationEffect.Harmful);
        CreateAssociation(goodAlone.Id, teamB.Id, AssociationEffect.Harmful);

        CreateAssociation(teamA.Id, teamB.Id, AssociationEffect.Beneficial);
        await DbContext.SaveChangesAsync();

        var result = await _sut.GetCompanionRecommendationsAsync([selected.Id]);

        var teamAIndex = result.FindIndex(r => r.PlantId == teamA.Id);
        var teamBIndex = result.FindIndex(r => r.PlantId == teamB.Id);
        var goodAloneIndex = result.FindIndex(r => r.PlantId == goodAlone.Id);

        teamAIndex.ShouldBeLessThan(goodAloneIndex);
        teamBIndex.ShouldBeLessThan(goodAloneIndex);
    }

    [Fact]
    public async Task GetCompanionRecommendationsAsync_WhenMultipleAssociationsForSamePair_ShouldSumAll()
    {
        var tomato = CreatePlant("Tomato");
        var basil = CreatePlant("Basil");
        CreateAssociation(tomato.Id, basil.Id, AssociationEffect.Beneficial,
            AssociationMechanism.OlfactoryConfusion);
        CreateAssociation(tomato.Id, basil.Id, AssociationEffect.Beneficial,
            AssociationMechanism.PredatorAttraction);
        await DbContext.SaveChangesAsync();

        var result = await _sut.GetCompanionRecommendationsAsync([tomato.Id]);

        result[0].Score.ShouldBeGreaterThan(1.0);
    }

    [Fact]
    public async Task GetCompanionRecommendationsAsync_ShouldHandleBidirectionalAssociations()
    {
        var tomato = CreatePlant("Tomato");
        var basil = CreatePlant("Basil");
        CreateAssociation(basil.Id, tomato.Id, AssociationEffect.Beneficial);
        await DbContext.SaveChangesAsync();

        var result = await _sut.GetCompanionRecommendationsAsync([tomato.Id]);

        result[0].PlantId.ShouldBe(basil.Id);
        result[0].Score.ShouldBeGreaterThan(0);
    }

    [Fact]
    public async Task GetCompanionRecommendationsAsync_WhenBeneficialHarmfulNeutral_ShouldProduceExpectedScoreOrder()
    {
        var selected = CreatePlant("Selected");
        var beneficial = CreatePlant("Beneficial");
        var neutral = CreatePlant("Neutral");
        var harmful = CreatePlant("Harmful");
        CreateAssociation(selected.Id, beneficial.Id, AssociationEffect.Beneficial);
        CreateAssociation(selected.Id, neutral.Id, AssociationEffect.Neutral);
        CreateAssociation(selected.Id, harmful.Id, AssociationEffect.Harmful);
        await DbContext.SaveChangesAsync();

        var result = await _sut.GetCompanionRecommendationsAsync([selected.Id]);

        result[0].PlantId.ShouldBe(beneficial.Id);
        result[0].Score.ShouldBeGreaterThan(0);
        result[2].PlantId.ShouldBe(harmful.Id);
        result[2].Score.ShouldBeLessThan(0);
    }

    [Fact]
    public async Task GetCompanionRecommendationsAsync_WhenFewerCandidatesThanMax_ShouldReturnAll()
    {
        var selected = CreatePlant("Selected");
        var candidateA = CreatePlant("A");
        var candidateB = CreatePlant("B");
        await DbContext.SaveChangesAsync();

        var result = await _sut.GetCompanionRecommendationsAsync([selected.Id]);

        result.Count.ShouldBe(2);
    }
}
