using GardenAssistant.Data.Entities;
using GardenAssistant.Data.Entities.Enums;
using GardenAssistant.DTOs;
using GardenAssistant.Services;
using GardenAssistant.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace GardenAssistant.Tests.Plants;

public class PlantAssociationServiceTests : DatabaseTestBase
{
    private readonly PlantAssociationService _sut;

    public PlantAssociationServiceTests()
    {
        _sut = new PlantAssociationService(DbContext);
    }

    private async Task<(Guid sourceId, Guid targetId)> SeedTwoPlantsAsync()
    {
        var sourceId = Guid.NewGuid();
        var targetId = Guid.NewGuid();
        DbContext.Plants.Add(new Plant { Id = sourceId, Name = "Carrot" });
        DbContext.Plants.Add(new Plant { Id = targetId, Name = "Onion" });
        await DbContext.SaveChangesAsync();
        return (sourceId, targetId);
    }

    [Fact]
    public async Task GetForPlantAsync_WhenNoAssociations_ShouldReturnEmpty()
    {
        var (sourceId, _) = await SeedTwoPlantsAsync();

        var result = await _sut.GetForPlantAsync(sourceId);

        result.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetForPlantAsync_WhenPlantIsSource_ShouldReturnAssociation()
    {
        var (sourceId, targetId) = await SeedTwoPlantsAsync();
        DbContext.PlantAssociations.Add(new PlantAssociation
        {
            Id = Guid.NewGuid(),
            SourcePlantId = sourceId,
            TargetPlantId = targetId,
            Mechanism = AssociationMechanism.OlfactoryConfusion,
            Effect = AssociationEffect.Beneficial,
            DistanceEffect = DistanceEffect.Contact,
            ConfidenceLevel = ConfidenceLevel.FieldObserved
        });
        await DbContext.SaveChangesAsync();

        var result = await _sut.GetForPlantAsync(sourceId);

        result.Count().ShouldBe(1);
        result.First().SourcePlantId.ShouldBe(sourceId);
    }

    [Fact]
    public async Task GetForPlantAsync_WhenPlantIsTarget_ShouldReturnAssociation()
    {
        var (sourceId, targetId) = await SeedTwoPlantsAsync();
        DbContext.PlantAssociations.Add(new PlantAssociation
        {
            Id = Guid.NewGuid(),
            SourcePlantId = sourceId,
            TargetPlantId = targetId,
            Mechanism = AssociationMechanism.RootAllelopathy,
            Effect = AssociationEffect.Harmful,
            DistanceEffect = DistanceEffect.Short,
            ConfidenceLevel = ConfidenceLevel.Anecdotal
        });
        await DbContext.SaveChangesAsync();

        var result = await _sut.GetForPlantAsync(targetId);

        result.Count().ShouldBe(1);
        result.First().TargetPlantId.ShouldBe(targetId);
    }

    [Fact]
    public async Task GetForPlantAsync_WhenPlantIsUnrelated_ShouldReturnEmpty()
    {
        var (sourceId, targetId) = await SeedTwoPlantsAsync();
        var unrelatedId = Guid.NewGuid();
        DbContext.Plants.Add(new Plant { Id = unrelatedId, Name = "Garlic" });
        DbContext.PlantAssociations.Add(new PlantAssociation
        {
            Id = Guid.NewGuid(),
            SourcePlantId = sourceId,
            TargetPlantId = targetId,
            Mechanism = AssociationMechanism.PollinatorAttraction,
            Effect = AssociationEffect.Neutral,
            DistanceEffect = DistanceEffect.Medium,
            ConfidenceLevel = ConfidenceLevel.PeerReviewed
        });
        await DbContext.SaveChangesAsync();

        var result = await _sut.GetForPlantAsync(unrelatedId);

        result.ShouldBeEmpty();
    }

    [Fact]
    public async Task CreateAsync_WhenValidRequest_ShouldPersistAllFields()
    {
        var (sourceId, targetId) = await SeedTwoPlantsAsync();
        var request = new CreatePlantAssociationRequest(
            sourceId,
            targetId,
            AssociationMechanism.NitrogenFixation,
            AssociationEffect.Beneficial,
            DistanceEffect.Field,
            ConfidenceLevel.PeerReviewed,
            "Fixes nitrogen in the soil"
        );

        var result = await _sut.CreateAsync(request);

        result.Id.ShouldNotBe(Guid.Empty);
        result.SourcePlantId.ShouldBe(sourceId);
        result.TargetPlantId.ShouldBe(targetId);
        result.Mechanism.ShouldBe(AssociationMechanism.NitrogenFixation);
        result.Effect.ShouldBe(AssociationEffect.Beneficial);
        result.DistanceEffect.ShouldBe(DistanceEffect.Field);
        result.ConfidenceLevel.ShouldBe(ConfidenceLevel.PeerReviewed);
        result.Notes.ShouldBe("Fixes nitrogen in the soil");
        (await DbContext.PlantAssociations.CountAsync()).ShouldBe(1);
    }

    [Fact]
    public async Task DeleteAsync_WhenAssociationExists_ShouldRemoveAndReturnTrue()
    {
        var (sourceId, targetId) = await SeedTwoPlantsAsync();
        var associationId = Guid.NewGuid();
        DbContext.PlantAssociations.Add(new PlantAssociation
        {
            Id = associationId,
            SourcePlantId = sourceId,
            TargetPlantId = targetId,
            Mechanism = AssociationMechanism.TrapCrop,
            Effect = AssociationEffect.Beneficial,
            DistanceEffect = DistanceEffect.Contact,
            ConfidenceLevel = ConfidenceLevel.Anecdotal
        });
        await DbContext.SaveChangesAsync();

        var result = await _sut.DeleteAsync(associationId);

        result.ShouldBeTrue();
        (await DbContext.PlantAssociations.CountAsync()).ShouldBe(0);
    }

    [Fact]
    public async Task DeleteAsync_WhenAssociationDoesNotExist_ShouldReturnFalse()
    {
        var result = await _sut.DeleteAsync(Guid.NewGuid());

        result.ShouldBeFalse();
    }
}
