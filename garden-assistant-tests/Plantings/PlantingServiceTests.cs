using GardenAssistant.Data.Entities;
using GardenAssistant.Data.Entities.Enums;
using GardenAssistant.DTOs.Plantings;
using GardenAssistant.Services;
using GardenAssistant.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace GardenAssistant.Tests.Plantings;

public class PlantingServiceTests : DatabaseTestBase
{
    private readonly PlantingService _sut;
    private static readonly Guid DefaultUserId = new("00000000-0000-0000-0000-000000000001");
    private static readonly Guid OtherUserId = Guid.NewGuid();

    public PlantingServiceTests()
    {
        _sut = new PlantingService(DbContext);
    }

    private async Task<Guid> SeedGardenAsync()
    {
        var gardenId = Guid.NewGuid();
        DbContext.Gardens.Add(new Garden { Id = gardenId, Name = "Test Garden", UserId = DefaultUserId });
        await DbContext.SaveChangesAsync();
        return gardenId;
    }

    private async Task<Guid> SeedPlantingAsync(Guid gardenId, Guid userId)
    {
        var plantingId = Guid.NewGuid();
        DbContext.Plantings.Add(new Planting
        {
            Id = plantingId,
            GardenId = gardenId,
            UserId = userId,
            Name = "Spring Bed"
        });
        await DbContext.SaveChangesAsync();
        return plantingId;
    }

    [Fact]
    public async Task GetAllAsync_WhenNoPlantings_ShouldReturnEmpty()
    {
        var result = await _sut.GetAllAsync(DefaultUserId);

        result.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetAllAsync_WhenPlantingsBelongToDifferentUser_ShouldReturnEmpty()
    {
        var gardenId = Guid.NewGuid();
        DbContext.Users.Add(new User { Id = OtherUserId, Email = "other@example.com" });
        DbContext.Gardens.Add(new Garden { Id = gardenId, Name = "Other Garden", UserId = OtherUserId });
        DbContext.Plantings.Add(new Planting
        {
            Id = Guid.NewGuid(),
            GardenId = gardenId,
            UserId = OtherUserId,
            Name = "Other Bed"
        });
        await DbContext.SaveChangesAsync();

        var result = await _sut.GetAllAsync(DefaultUserId);

        result.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetAllAsync_WhenPlantingsExist_ShouldReturnOnlyCallerPlantings()
    {
        var gardenId = await SeedGardenAsync();
        var otherGardenId = Guid.NewGuid();
        DbContext.Users.Add(new User { Id = OtherUserId, Email = "other@example.com" });
        DbContext.Gardens.Add(new Garden { Id = otherGardenId, Name = "Other Garden", UserId = OtherUserId });
        DbContext.Plantings.Add(new Planting
        {
            Id = Guid.NewGuid(),
            GardenId = gardenId,
            UserId = DefaultUserId,
            Name = "My Bed"
        });
        DbContext.Plantings.Add(new Planting
        {
            Id = Guid.NewGuid(),
            GardenId = otherGardenId,
            UserId = OtherUserId,
            Name = "Other Bed"
        });
        await DbContext.SaveChangesAsync();

        var result = await _sut.GetAllAsync(DefaultUserId);

        var plantingDtos = result as PlantingDto[] ?? result.ToArray();
        plantingDtos.Length.ShouldBe(1);
        plantingDtos.First().Name.ShouldBe("My Bed");
    }

    [Fact]
    public async Task GetByIdAsync_WhenPlantingExists_ShouldReturnDto()
    {
        var gardenId = await SeedGardenAsync();
        var plantingId = await SeedPlantingAsync(gardenId, DefaultUserId);

        var result = await _sut.GetByIdAsync(plantingId, DefaultUserId);

        result.ShouldNotBeNull();
        result.Id.ShouldBe(plantingId);
        result.Name.ShouldBe("Spring Bed");
    }

    [Fact]
    public async Task GetByIdAsync_WhenPlantingBelongsToDifferentUser_ShouldReturnNull()
    {
        var gardenId = Guid.NewGuid();
        DbContext.Users.Add(new User { Id = OtherUserId, Email = "other@example.com" });
        DbContext.Gardens.Add(new Garden { Id = gardenId, Name = "Other Garden", UserId = OtherUserId });
        var plantingId = await SeedPlantingAsync(gardenId, OtherUserId);

        var result = await _sut.GetByIdAsync(plantingId, DefaultUserId);

        result.ShouldBeNull();
    }

    [Fact]
    public async Task GetByIdAsync_WhenPlantingDoesNotExist_ShouldReturnNull()
    {
        var result = await _sut.GetByIdAsync(Guid.NewGuid(), DefaultUserId);

        result.ShouldBeNull();
    }

    [Fact]
    public async Task CreateAsync_WhenValidRequest_ShouldPersistAndReturnDto()
    {
        var gardenId = await SeedGardenAsync();
        var request = new CreatePlantingRequest(gardenId, "Summer Bed", "Warm season crops", new DateOnly(2026, 5, 1));

        var result = await _sut.CreateAsync(request, DefaultUserId);

        result.Id.ShouldNotBe(Guid.Empty);
        result.Name.ShouldBe("Summer Bed");
        result.Description.ShouldBe("Warm season crops");
        result.PlannedDate.ShouldBe(new DateOnly(2026, 5, 1));
        (await DbContext.Plantings.CountAsync()).ShouldBe(1);
        (await DbContext.Plantings.SingleAsync()).UserId.ShouldBe(DefaultUserId);
    }

    [Fact]
    public async Task DeleteAsync_WhenPlantingExists_ShouldRemoveAndReturnTrue()
    {
        var gardenId = await SeedGardenAsync();
        var plantingId = await SeedPlantingAsync(gardenId, DefaultUserId);

        var result = await _sut.DeleteAsync(plantingId, DefaultUserId);

        result.ShouldBeTrue();
        (await DbContext.Plantings.CountAsync()).ShouldBe(0);
    }

    [Fact]
    public async Task DeleteAsync_WhenPlantingBelongsToDifferentUser_ShouldReturnFalse()
    {
        var gardenId = Guid.NewGuid();
        DbContext.Users.Add(new User { Id = OtherUserId, Email = "other@example.com" });
        DbContext.Gardens.Add(new Garden { Id = gardenId, Name = "Other Garden", UserId = OtherUserId });
        var plantingId = await SeedPlantingAsync(gardenId, OtherUserId);

        var result = await _sut.DeleteAsync(plantingId, DefaultUserId);

        result.ShouldBeFalse();
        (await DbContext.Plantings.CountAsync()).ShouldBe(1);
    }

    [Fact]
    public async Task GetCompatibilityScoreAsync_WhenNoEntries_ShouldReturnAllZero()
    {
        var gardenId = await SeedGardenAsync();
        var plantingId = await SeedPlantingAsync(gardenId, DefaultUserId);

        var result = await _sut.GetCompatibilityScoreAsync(plantingId, DefaultUserId);

        result.Beneficial.ShouldBe(0);
        result.Harmful.ShouldBe(0);
        result.Neutral.ShouldBe(0);
        result.Total.ShouldBe(0);
    }

    [Fact]
    public async Task GetCompatibilityScoreAsync_WhenEntriesHaveBeneficialAssociation_ShouldCountCorrectly()
    {
        var gardenId = await SeedGardenAsync();
        var plantingId = await SeedPlantingAsync(gardenId, DefaultUserId);

        var plantAId = Guid.NewGuid();
        var plantBId = Guid.NewGuid();
        DbContext.Plants.Add(new Plant { Id = plantAId, Name = "Tomato" });
        DbContext.Plants.Add(new Plant { Id = plantBId, Name = "Basil" });
        DbContext.PlantingEntries.Add(new PlantingEntry { Id = Guid.NewGuid(), PlantingId = plantingId, PlantId = plantAId });
        DbContext.PlantingEntries.Add(new PlantingEntry { Id = Guid.NewGuid(), PlantingId = plantingId, PlantId = plantBId });
        DbContext.PlantAssociations.Add(new PlantAssociation
        {
            Id = Guid.NewGuid(),
            SourcePlantId = plantAId,
            TargetPlantId = plantBId,
            Mechanism = AssociationMechanism.OlfactoryConfusion,
            Effect = AssociationEffect.Beneficial,
            DistanceEffect = DistanceEffect.Contact,
            ConfidenceLevel = ConfidenceLevel.FieldObserved
        });
        await DbContext.SaveChangesAsync();

        var result = await _sut.GetCompatibilityScoreAsync(plantingId, DefaultUserId);

        result.Beneficial.ShouldBe(1);
        result.Harmful.ShouldBe(0);
        result.Neutral.ShouldBe(0);
        result.Total.ShouldBe(1);
    }

    [Fact]
    public async Task GetCompatibilityScoreAsync_WhenEntriesHaveHarmfulAssociation_ShouldCountCorrectly()
    {
        var gardenId = await SeedGardenAsync();
        var plantingId = await SeedPlantingAsync(gardenId, DefaultUserId);

        var plantAId = Guid.NewGuid();
        var plantBId = Guid.NewGuid();
        DbContext.Plants.Add(new Plant { Id = plantAId, Name = "Fennel" });
        DbContext.Plants.Add(new Plant { Id = plantBId, Name = "Tomato" });
        DbContext.PlantingEntries.Add(new PlantingEntry { Id = Guid.NewGuid(), PlantingId = plantingId, PlantId = plantAId });
        DbContext.PlantingEntries.Add(new PlantingEntry { Id = Guid.NewGuid(), PlantingId = plantingId, PlantId = plantBId });
        DbContext.PlantAssociations.Add(new PlantAssociation
        {
            Id = Guid.NewGuid(),
            SourcePlantId = plantAId,
            TargetPlantId = plantBId,
            Mechanism = AssociationMechanism.RootAllelopathy,
            Effect = AssociationEffect.Harmful,
            DistanceEffect = DistanceEffect.Short,
            ConfidenceLevel = ConfidenceLevel.PeerReviewed
        });
        await DbContext.SaveChangesAsync();

        var result = await _sut.GetCompatibilityScoreAsync(plantingId, DefaultUserId);

        result.Beneficial.ShouldBe(0);
        result.Harmful.ShouldBe(1);
        result.Neutral.ShouldBe(0);
        result.Total.ShouldBe(1);
    }

    [Fact]
    public async Task GetCompatibilityScoreAsync_WhenPlantingBelongsToDifferentUser_ShouldReturnAllZero()
    {
        var gardenId = Guid.NewGuid();
        DbContext.Users.Add(new User { Id = OtherUserId, Email = "other@example.com" });
        DbContext.Gardens.Add(new Garden { Id = gardenId, Name = "Other Garden", UserId = OtherUserId });
        var plantingId = await SeedPlantingAsync(gardenId, OtherUserId);

        var result = await _sut.GetCompatibilityScoreAsync(plantingId, DefaultUserId);

        result.Beneficial.ShouldBe(0);
        result.Harmful.ShouldBe(0);
        result.Neutral.ShouldBe(0);
        result.Total.ShouldBe(0);
    }
}
