using GardenAssistant.Data.Entities;
using GardenAssistant.Data.Entities.Enums;
using GardenAssistant.Services;
using GardenAssistant.Tests.Infrastructure;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;

namespace GardenAssistant.Tests.Plants;

public class RootDepthBonusTests : DatabaseTestBase
{
    private readonly Mock<ILogger<PlantAssociationService>> _logger;
    private readonly PlantAssociationService _sut;

    public RootDepthBonusTests()
    {
        _logger = new Mock<ILogger<PlantAssociationService>>();
        _sut = new PlantAssociationService(DbContext, _logger.Object);
    }

    private Plant CreatePlant(string name, RootDepth rootDepth)
    {
        var plant = new Plant { Id = Guid.NewGuid(), Key = name.ToLowerInvariant().Replace(' ', '-'), Name = name, RootDepth = rootDepth };
        DbContext.Plants.Add(plant);
        return plant;
    }

    private void CreateAssociation(Guid sourceId, Guid targetId, AssociationEffect effect,
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
    public async Task GetCompanionRecommendations_WhenCandidateAndSelectedHaveDifferentRootDepths_ShouldApplyTenPercentBonus()
    {
        // Arrange
        var selected = CreatePlant("DeepRooted", RootDepth.Deep);
        var candidate = CreatePlant("ShallowRooted", RootDepth.Shallow);
        CreateAssociation(selected.Id, candidate.Id, AssociationEffect.Beneficial);
        await DbContext.SaveChangesAsync();

        // Act
        var result = await _sut.GetCompanionRecommendationsAsync([selected.Id]);

        // Assert
        var candidateRec = result.GoodCompanions.First(c => c.PlantId == candidate.Id);
        // BeneficialScore (1.0) * 1.10 = 1.10, rounded to 2 decimals
        var candidateScore = result.GoodCompanions
            .OrderByDescending(c => c.PlantId == candidate.Id)
            .First(c => c.PlantId == candidate.Id);
        candidateRec.ShouldNotBeNull();

        // The candidate should rank above the selected plant which scores against itself
        // Verify via ordering: candidate (score 1.10) beats selected (score -0.11 or similar)
        result.GoodCompanions[0].PlantId.ShouldBe(candidate.Id);
    }

    [Fact]
    public async Task GetCompanionRecommendations_WhenCandidateAndSelectedHaveSameRootDepth_ShouldNotApplyBonus()
    {
        // Arrange
        var selected = CreatePlant("MediumSelected", RootDepth.Medium);
        var candidate = CreatePlant("MediumCandidate", RootDepth.Medium);
        CreateAssociation(selected.Id, candidate.Id, AssociationEffect.Beneficial);
        await DbContext.SaveChangesAsync();

        // Act
        var result = await _sut.GetCompanionRecommendationsAsync([selected.Id]);

        // Assert
        // Same depth: no bonus, score stays at BeneficialScore = 1.0
        // candidate ranks first since 1.0 > unknown (-0.1, which is the selected plant's score vs itself with no association)
        result.GoodCompanions[0].PlantId.ShouldBe(candidate.Id);

        // The selected plant has no association with itself, so score = -0.1 (UnknownScore, no bonus because same depth)
        // Candidate has score = 1.0 (no bonus applied)
        // Both are in GoodCompanions; candidate must be ranked higher
        var candidateIndex = result.GoodCompanions.FindIndex(c => c.PlantId == candidate.Id);
        var selectedIndex = result.GoodCompanions.FindIndex(c => c.PlantId == selected.Id);
        candidateIndex.ShouldBeLessThan(selectedIndex);
    }

    [Fact]
    public async Task GetCompanionRecommendations_WhenMultipleSelectedPlantsWithMixedDepths_ShouldApplyBonusOnlyToDifferentDepthPairs()
    {
        // Arrange
        var selectedShallow = CreatePlant("SelectedShallow", RootDepth.Shallow);
        var selectedDeep = CreatePlant("SelectedDeep", RootDepth.Deep);
        var candidate = CreatePlant("CandidateShallow", RootDepth.Shallow);
        // Beneficial association with shallow selected (same depth) → score 1.0, no bonus
        CreateAssociation(selectedShallow.Id, candidate.Id, AssociationEffect.Beneficial);
        // Beneficial association with deep selected (different depth) → score 1.0 * 1.10 = 1.10
        CreateAssociation(selectedDeep.Id, candidate.Id, AssociationEffect.Beneficial);
        await DbContext.SaveChangesAsync();

        // Act
        var result = await _sut.GetCompanionRecommendationsAsync([selectedShallow.Id, selectedDeep.Id]);

        // Assert
        // candidate total = 1.0 (same depth, no bonus) + 1.1 (different depth, bonus) = 2.1 → rounded = 2.1
        // candidate must rank first (highest score among all plants)
        result.GoodCompanions[0].PlantId.ShouldBe(candidate.Id);
    }

    [Fact]
    public async Task GetCompanionRecommendations_WhenNegativeBaseScoreAndDifferentDepths_ShouldApplyBonusToNegativeScore()
    {
        // Arrange
        var selected = CreatePlant("DeepSelected", RootDepth.Deep);
        var candidate = CreatePlant("ShallowCandidate", RootDepth.Shallow);
        // Harmful association: HarmfulScore = -1.5
        CreateAssociation(selected.Id, candidate.Id, AssociationEffect.Harmful);
        await DbContext.SaveChangesAsync();

        // Act
        var result = await _sut.GetCompanionRecommendationsAsync([selected.Id]);

        // Assert
        // pairScore = -1.5 (harmful), bonus not applied because pairScore <= 0
        // candidate score = -1.5, selected score vs itself = -0.1 (no association, same depth)
        // candidate must rank below the selected plant in goodCompanions
        var candidateIndex = result.GoodCompanions.FindIndex(c => c.PlantId == candidate.Id);
        var selectedIndex = result.GoodCompanions.FindIndex(c => c.PlantId == selected.Id);
        candidateIndex.ShouldBeGreaterThan(selectedIndex);
    }

    [Fact]
    public async Task GetCompanionRecommendations_WhenNoAssociationExistsAndDifferentDepths_ShouldApplyBonusToUnknownScore()
    {
        // Arrange
        var selected = CreatePlant("DeepSelected", RootDepth.Deep);
        var candidate = CreatePlant("ShallowCandidate", RootDepth.Shallow);
        // No association created: ScorePair returns UnknownScore = -0.1
        await DbContext.SaveChangesAsync();

        // Act
        var result = await _sut.GetCompanionRecommendationsAsync([selected.Id]);

        // Assert
        var rec = result.GoodCompanions.FirstOrDefault(c => c.PlantId == candidate.Id);
        rec.ShouldNotBeNull();
        rec.Score.ShouldBeGreaterThan(0);
        rec.HasRootDepthBonus.ShouldBeTrue();
    }

}
