using GardenAssistant.Data.Entities;
using GardenAssistant.Data.Entities.Enums;
using GardenAssistant.Services;
using GardenAssistant.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace GardenAssistant.Tests.Plants;

public class VarietyInheritanceTests : DatabaseTestBase
{
    private readonly PlantService _plantService;
    private readonly PlantActionService _actionService;
    private readonly HarvestReadinessService _harvestService;

    public VarietyInheritanceTests()
    {
        _plantService = new PlantService(DbContext);
        _actionService = new PlantActionService(DbContext);
        _harvestService = new HarvestReadinessService(DbContext);
    }

    [Fact]
    public async Task Plant_WhenVarietyWithParent_ShouldPersistRelationship()
    {
        var parent = new Plant { Id = Guid.NewGuid(), Key = "courge", Name = "Courge" };
        var variety = new Plant { Id = Guid.NewGuid(), Key = "courgette", Name = "Courgette", ParentPlantId = parent.Id };
        DbContext.Plants.AddRange(parent, variety);
        await DbContext.SaveChangesAsync();

        var loaded = await DbContext.Plants
            .Include(p => p.ParentPlant)
            .FirstAsync(p => p.Id == variety.Id);

        loaded.ParentPlantId.ShouldBe(parent.Id);
        loaded.ParentPlant.ShouldNotBeNull();
        loaded.ParentPlant!.Name.ShouldBe("Courge");
    }

    [Fact]
    public async Task Plant_WhenSpeciesWithVarieties_ShouldLoadVarietiesCollection()
    {
        var parent = new Plant { Id = Guid.NewGuid(), Key = "courge", Name = "Courge" };
        var v1 = new Plant { Id = Guid.NewGuid(), Key = "courgette", Name = "Courgette", ParentPlantId = parent.Id };
        var v2 = new Plant { Id = Guid.NewGuid(), Key = "patisson", Name = "Pâtisson", ParentPlantId = parent.Id };
        DbContext.Plants.AddRange(parent, v1, v2);
        await DbContext.SaveChangesAsync();

        var loaded = await DbContext.Plants
            .Include(p => p.Varieties)
            .FirstAsync(p => p.Id == parent.Id);

        loaded.Varieties.Count.ShouldBe(2);
    }

    [Fact]
    public async Task Plant_WhenNoParent_ShouldHaveNullParentPlantId()
    {
        var species = new Plant { Id = Guid.NewGuid(), Key = "tomate", Name = "Tomate" };
        DbContext.Plants.Add(species);
        await DbContext.SaveChangesAsync();

        var loaded = await DbContext.Plants.FirstAsync(p => p.Id == species.Id);

        loaded.ParentPlantId.ShouldBeNull();
    }

    [Fact]
    public async Task GetAllAsync_WhenVarietyHasNoOwnMechanisms_ShouldInheritFromParent()
    {
        var parent = new Plant { Id = Guid.NewGuid(), Key = "courge", Name = "Courge" };
        DbContext.Plants.Add(parent);
        DbContext.PlantIntrinsicMechanisms.Add(new PlantIntrinsicMechanism
        {
            PlantId = parent.Id,
            Mechanism = AssociationMechanism.WeedSuppression
        });
        var variety = new Plant { Id = Guid.NewGuid(), Key = "courgette", Name = "Courgette", ParentPlantId = parent.Id };
        DbContext.Plants.Add(variety);
        await DbContext.SaveChangesAsync();

        var result = await _plantService.GetAllAsync();
        var courgette = result.Single(p => p.Name == "Courgette");

        courgette.IntrinsicMechanisms.ShouldContain(AssociationMechanism.WeedSuppression);
        courgette.IsVariety.ShouldBeTrue();
        courgette.ParentPlantId.ShouldBe(parent.Id);
        courgette.ParentPlantName.ShouldBe("Courge");
    }

    [Fact]
    public async Task GetAllAsync_WhenVarietyOverridesCulturalProperty_ShouldUseVarietyValue()
    {
        var parent = new Plant
        {
            Id = Guid.NewGuid(),
            Key = "courge",
            Name = "Courge",
            HeightAtMaturityCm = 30,
            WaterNeeds = WaterNeeds.Medium
        };
        var variety = new Plant
        {
            Id = Guid.NewGuid(),
            Key = "courgette",
            Name = "Courgette",
            ParentPlantId = parent.Id,
            HeightAtMaturityCm = 80
        };
        DbContext.Plants.AddRange(parent, variety);
        await DbContext.SaveChangesAsync();

        var result = await _plantService.GetAllAsync();
        var courgette = result.Single(p => p.Name == "Courgette");

        courgette.HeightAtMaturityCm.ShouldBe(80);
    }

    [Fact]
    public async Task GetAllAsync_WhenPlantHasNoParent_ShouldNotBeVariety()
    {
        DbContext.Plants.Add(new Plant { Id = Guid.NewGuid(), Key = "tomate", Name = "Tomate" });
        await DbContext.SaveChangesAsync();

        var result = await _plantService.GetAllAsync();
        var tomate = result.Single();

        tomate.IsVariety.ShouldBeFalse();
        tomate.ParentPlantId.ShouldBeNull();
        tomate.ParentPlantName.ShouldBeNull();
        tomate.Varieties.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetAllAsync_WhenSpeciesHasVarieties_ShouldPopulateVarietiesList()
    {
        var parent = new Plant { Id = Guid.NewGuid(), Key = "courge", Name = "Courge" };
        var v1 = new Plant { Id = Guid.NewGuid(), Key = "courgette", Name = "Courgette", ScientificName = "C. pepo var. cylindrica", ParentPlantId = parent.Id };
        var v2 = new Plant { Id = Guid.NewGuid(), Key = "patisson", Name = "Patisson", ScientificName = "C. pepo var. clypeata", ParentPlantId = parent.Id };
        DbContext.Plants.AddRange(parent, v1, v2);
        await DbContext.SaveChangesAsync();

        var result = await _plantService.GetAllAsync();
        var courge = result.Single(p => p.Name == "Courge");

        courge.Varieties.Count.ShouldBe(2);
        courge.Varieties.ShouldContain(v => v.Name == "Courgette");
        courge.Varieties.ShouldContain(v => v.Name == "Patisson");
    }

    [Fact]
    public async Task GetByPlantIdAsync_WhenVarietyHasNoActions_ShouldInheritFromParent()
    {
        var parent = new Plant { Id = Guid.NewGuid(), Key = "courge", Name = "Courge" };
        var variety = new Plant { Id = Guid.NewGuid(), Key = "courgette", Name = "Courgette", ParentPlantId = parent.Id };
        DbContext.Plants.AddRange(parent, variety);
        DbContext.PlantActions.Add(new PlantAction
        {
            Id = Guid.NewGuid(),
            PlantId = parent.Id,
            ActionType = PlantActionType.DirectSowing,
            HalfMonthStart = 9,
            HalfMonthEnd = 12
        });
        await DbContext.SaveChangesAsync();

        var result = await _actionService.GetByPlantIdAsync(variety.Id);

        result.Count.ShouldBe(1);
        result[0].ActionType.ShouldBe(PlantActionType.DirectSowing);
    }

    [Fact]
    public async Task GetByPlantIdAsync_WhenVarietyHasOwnActions_ShouldUseVarietyActions()
    {
        var parent = new Plant { Id = Guid.NewGuid(), Key = "courge", Name = "Courge" };
        var variety = new Plant { Id = Guid.NewGuid(), Key = "courgette", Name = "Courgette", ParentPlantId = parent.Id };
        DbContext.Plants.AddRange(parent, variety);
        DbContext.PlantActions.Add(new PlantAction
        {
            Id = Guid.NewGuid(),
            PlantId = parent.Id,
            ActionType = PlantActionType.DirectSowing,
            HalfMonthStart = 9,
            HalfMonthEnd = 12
        });
        DbContext.PlantActions.Add(new PlantAction
        {
            Id = Guid.NewGuid(),
            PlantId = variety.Id,
            ActionType = PlantActionType.Transplanting,
            HalfMonthStart = 10,
            HalfMonthEnd = 14
        });
        await DbContext.SaveChangesAsync();

        var result = await _actionService.GetByPlantIdAsync(variety.Id);

        result.Count.ShouldBe(1);
        result[0].ActionType.ShouldBe(PlantActionType.Transplanting);
    }

    [Fact]
    public async Task GetHarvestReadiness_WhenVarietyHasNone_ShouldInheritFromParent()
    {
        var parent = new Plant { Id = Guid.NewGuid(), Key = "courge", Name = "Courge" };
        var variety = new Plant { Id = Guid.NewGuid(), Key = "courgette", Name = "Courgette", ParentPlantId = parent.Id };
        DbContext.Plants.AddRange(parent, variety);
        DbContext.HarvestReadiness.Add(new HarvestReadiness
        {
            Id = Guid.NewGuid(),
            PlantId = parent.Id,
            Description = "Récolter quand le fruit est ferme"
        });
        await DbContext.SaveChangesAsync();

        var result = await _harvestService.GetByPlantIdAsync(variety.Id);

        result.ShouldNotBeNull();
        result!.Description.ShouldBe("Récolter quand le fruit est ferme");
    }
}
