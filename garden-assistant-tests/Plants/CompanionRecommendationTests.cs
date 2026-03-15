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

    private Guild CreateGuild(string name, string description, params Plant[] plants)
    {
        var guild = new Guild { Id = Guid.NewGuid(), Name = name, Description = description };
        DbContext.Guilds.Add(guild);
        foreach (var plant in plants)
            DbContext.GuildPlants.Add(new GuildPlant { GuildId = guild.Id, PlantId = plant.Id });
        return guild;
    }

    [Fact]
    public async Task GetCompanionRecommendationsAsync_WhenNoCandidates_ShouldReturnEmpty()
    {
        var plant = CreatePlant("Tomato");
        await DbContext.SaveChangesAsync();

        var result = await _sut.GetCompanionRecommendationsAsync([plant.Id]);

        result.GoodCompanions.ShouldBeEmpty();
        result.PlantsToAvoid.ShouldBeEmpty();
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

        result.GoodCompanions.Count.ShouldBe(2);
        result.GoodCompanions[0].PlantId.ShouldBe(basil.Id);
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

        var neutralScore = result.GoodCompanions.First(r => r.PlantId == neutral.Id).Score;
        var unknownScore = result.GoodCompanions.First(r => r.PlantId == unknown.Id).Score;
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

        result.GoodCompanions.Count.ShouldBe(10);
    }

    [Fact]
    public async Task GetCompanionRecommendationsAsync_ShouldExcludeSelectedPlants()
    {
        var tomato = CreatePlant("Tomato");
        var basil = CreatePlant("Basil");
        var carrot = CreatePlant("Carrot");
        await DbContext.SaveChangesAsync();

        var result = await _sut.GetCompanionRecommendationsAsync([tomato.Id, basil.Id]);

        result.GoodCompanions.Select(r => r.PlantId).ShouldNotContain(tomato.Id);
        result.GoodCompanions.Select(r => r.PlantId).ShouldNotContain(basil.Id);
        result.GoodCompanions.ShouldContain(r => r.PlantId == carrot.Id);
    }

    [Fact]
    public async Task GetCompanionRecommendationsAsync_ShouldReturnPlantNameAndScientificName()
    {
        var tomato = CreatePlant("Tomato");
        var basil = CreatePlant("Basil", "Ocimum basilicum");
        CreateAssociation(tomato.Id, basil.Id, AssociationEffect.Beneficial);
        await DbContext.SaveChangesAsync();

        var result = await _sut.GetCompanionRecommendationsAsync([tomato.Id]);

        result.GoodCompanions[0].PlantName.ShouldBe("Basil");
        result.GoodCompanions[0].ScientificName.ShouldBe("Ocimum basilicum");
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
        var good = result.GoodCompanions;

        var teamAIndex = good.FindIndex(r => r.PlantId == teamA.Id);
        var teamBIndex = good.FindIndex(r => r.PlantId == teamB.Id);
        var goodAloneIndex = good.FindIndex(r => r.PlantId == goodAlone.Id);

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

        result.GoodCompanions[0].Score.ShouldBeGreaterThan(1.0);
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
        result.GoodCompanions[0].Score.ShouldBeGreaterThan(0);
    }

    [Fact]
    public async Task GetCompanionRecommendationsAsync_WhenFewerCandidatesThanMax_ShouldReturnAll()
    {
        var selected = CreatePlant("Selected");
        CreatePlant("A");
        CreatePlant("B");
        await DbContext.SaveChangesAsync();

        var result = await _sut.GetCompanionRecommendationsAsync([selected.Id]);

        result.GoodCompanions.Count.ShouldBe(2);
    }

    [Fact]
    public async Task GetCompanionRecommendationsAsync_WhenHarmfulAssociationExists_ShouldReturnPlantsToAvoid()
    {
        var tomato = CreatePlant("Tomato");
        var fennel = CreatePlant("Fennel");
        CreateAssociation(tomato.Id, fennel.Id, AssociationEffect.Harmful, AssociationMechanism.RootAllelopathy);
        await DbContext.SaveChangesAsync();

        var result = await _sut.GetCompanionRecommendationsAsync([tomato.Id]);

        result.PlantsToAvoid.Count.ShouldBe(1);
        result.PlantsToAvoid[0].PlantId.ShouldBe(fennel.Id);
        result.PlantsToAvoid[0].PlantName.ShouldBe("Fennel");
        result.PlantsToAvoid[0].Mechanisms.ShouldContain(AssociationMechanism.RootAllelopathy);
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
    public async Task GetCompanionRecommendationsAsync_WhenGuildExists_ShouldIncludeGuildInfo()
    {
        var tomato = CreatePlant("Tomato");
        var basil = CreatePlant("Basil");
        CreateAssociation(tomato.Id, basil.Id, AssociationEffect.Beneficial);
        CreateGuild("Tomato Guild", "Tomato and basil work great together", tomato, basil);
        await DbContext.SaveChangesAsync();

        var result = await _sut.GetCompanionRecommendationsAsync([tomato.Id]);

        result.GoodCompanions[0].PlantId.ShouldBe(basil.Id);
        result.GoodCompanions[0].Guilds.Count.ShouldBe(1);
        result.GoodCompanions[0].Guilds[0].Name.ShouldBe("Tomato Guild");
    }

    [Fact]
    public async Task GetCompanionRecommendationsAsync_WhenNoGuild_ShouldReturnEmptyGuilds()
    {
        var tomato = CreatePlant("Tomato");
        var basil = CreatePlant("Basil");
        CreateAssociation(tomato.Id, basil.Id, AssociationEffect.Beneficial);
        await DbContext.SaveChangesAsync();

        var result = await _sut.GetCompanionRecommendationsAsync([tomato.Id]);

        result.GoodCompanions[0].Guilds.ShouldBeEmpty();
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
        CreateGuild("Tomato Guild", "Classic guild", tomato, basil);
        await DbContext.SaveChangesAsync();

        var result = await _sut.GetCompanionRecommendationsAsync([tomato.Id]);

        result.GoodCompanions[0].PlantId.ShouldBe(carrot.Id);
        result.GoodCompanions[0].Score.ShouldBeGreaterThan(result.GoodCompanions[1].Score);
        result.GoodCompanions[1].PlantId.ShouldBe(basil.Id);
        result.GoodCompanions[1].Guilds.Count.ShouldBeGreaterThan(0);
    }

    [Fact]
    public async Task GetCompanionRecommendationsAsync_WhenHarmfulWithMultipleMechanisms_ShouldListAll()
    {
        var tomato = CreatePlant("Tomato");
        var fennel = CreatePlant("Fennel");
        CreateAssociation(tomato.Id, fennel.Id, AssociationEffect.Harmful, AssociationMechanism.RootAllelopathy);
        CreateAssociation(tomato.Id, fennel.Id, AssociationEffect.Harmful, AssociationMechanism.AerialRepulsion);
        await DbContext.SaveChangesAsync();

        var result = await _sut.GetCompanionRecommendationsAsync([tomato.Id]);

        result.PlantsToAvoid[0].Mechanisms.Count.ShouldBe(2);
        result.PlantsToAvoid[0].Mechanisms.ShouldContain(AssociationMechanism.RootAllelopathy);
        result.PlantsToAvoid[0].Mechanisms.ShouldContain(AssociationMechanism.AerialRepulsion);
    }
}
