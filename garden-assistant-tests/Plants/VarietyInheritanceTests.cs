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

    public VarietyInheritanceTests()
    {
        _plantService = new PlantService(DbContext);
    }

    [Fact]
    public async Task Plant_WhenVarietyWithParent_ShouldPersistRelationship()
    {
        var parent = new Plant { Id = Guid.NewGuid(), Name = "Courge" };
        var variety = new Plant { Id = Guid.NewGuid(), Name = "Courgette", ParentPlantId = parent.Id };
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
        var parent = new Plant { Id = Guid.NewGuid(), Name = "Courge" };
        var v1 = new Plant { Id = Guid.NewGuid(), Name = "Courgette", ParentPlantId = parent.Id };
        var v2 = new Plant { Id = Guid.NewGuid(), Name = "Pâtisson", ParentPlantId = parent.Id };
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
        var species = new Plant { Id = Guid.NewGuid(), Name = "Tomate" };
        DbContext.Plants.Add(species);
        await DbContext.SaveChangesAsync();

        var loaded = await DbContext.Plants.FirstAsync(p => p.Id == species.Id);

        loaded.ParentPlantId.ShouldBeNull();
    }

    [Fact]
    public async Task GetAllAsync_WhenVarietyHasNoOwnMechanisms_ShouldInheritFromParent()
    {
        var parent = new Plant { Id = Guid.NewGuid(), Name = "Courge" };
        DbContext.Plants.Add(parent);
        DbContext.PlantIntrinsicMechanisms.Add(new PlantIntrinsicMechanism
        {
            PlantId = parent.Id,
            Mechanism = AssociationMechanism.WeedSuppression
        });
        var variety = new Plant { Id = Guid.NewGuid(), Name = "Courgette", ParentPlantId = parent.Id };
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
            Name = "Courge",
            HeightAtMaturityCm = 30,
            WaterNeeds = WaterNeeds.Medium
        };
        var variety = new Plant
        {
            Id = Guid.NewGuid(),
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
        DbContext.Plants.Add(new Plant { Id = Guid.NewGuid(), Name = "Tomate" });
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
        var parent = new Plant { Id = Guid.NewGuid(), Name = "Courge" };
        var v1 = new Plant { Id = Guid.NewGuid(), Name = "Courgette", ScientificName = "C. pepo var. cylindrica", ParentPlantId = parent.Id };
        var v2 = new Plant { Id = Guid.NewGuid(), Name = "Patisson", ScientificName = "C. pepo var. clypeata", ParentPlantId = parent.Id };
        DbContext.Plants.AddRange(parent, v1, v2);
        await DbContext.SaveChangesAsync();

        var result = await _plantService.GetAllAsync();
        var courge = result.Single(p => p.Name == "Courge");

        courge.Varieties.Count.ShouldBe(2);
        courge.Varieties.ShouldContain(v => v.Name == "Courgette");
        courge.Varieties.ShouldContain(v => v.Name == "Patisson");
    }
}
