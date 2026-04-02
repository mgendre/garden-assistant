using GardenAssistant.Data.Entities;
using GardenAssistant.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Shouldly;

namespace GardenAssistant.Tests.Plants;

public class VarietyInheritanceTests : DatabaseTestBase
{
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
}
