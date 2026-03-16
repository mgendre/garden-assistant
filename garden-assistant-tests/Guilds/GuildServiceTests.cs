using GardenAssistant.Data.Entities;
using GardenAssistant.Services;
using GardenAssistant.Tests.Infrastructure;
using Shouldly;

namespace GardenAssistant.Tests.Guilds;

public class GuildServiceTests : DatabaseTestBase
{
    private readonly GuildService _sut;
    private readonly Guid _userId = Guid.NewGuid();

    public GuildServiceTests()
    {
        _sut = new GuildService(DbContext);
    }

    [Fact]
    public async Task GetAllAsync_WhenNoGuilds_ShouldReturnEmpty()
    {
        var result = await _sut.GetAllAsync(_userId);

        result.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetAllAsync_WhenGuildsExist_ShouldReturnAllWithPlantCounts()
    {
        var guildWithPlants = new Guild { Id = Guid.NewGuid(), Name = "Three Sisters" };
        var guildWithoutPlants = new Guild { Id = Guid.NewGuid(), Name = "Herb Spiral" };

        var corn = new Plant { Id = Guid.NewGuid(), Name = "Corn" };
        var beans = new Plant { Id = Guid.NewGuid(), Name = "Beans" };
        var squash = new Plant { Id = Guid.NewGuid(), Name = "Squash" };

        DbContext.Guilds.AddRange(guildWithPlants, guildWithoutPlants);
        DbContext.Plants.AddRange(corn, beans, squash);
        DbContext.GuildPlants.AddRange(
            new GuildPlant { GuildId = guildWithPlants.Id, PlantId = corn.Id },
            new GuildPlant { GuildId = guildWithPlants.Id, PlantId = beans.Id },
            new GuildPlant { GuildId = guildWithPlants.Id, PlantId = squash.Id }
        );
        await DbContext.SaveChangesAsync();

        var result = (await _sut.GetAllAsync(_userId)).ToList();

        result.Count.ShouldBe(2);

        var threeSisters = result.Single(g => g.Name == "Three Sisters");
        threeSisters.PlantCount.ShouldBe(3);
        threeSisters.Id.ShouldBe(guildWithPlants.Id);
        threeSisters.IsOfficial.ShouldBeTrue();

        var herbSpiral = result.Single(g => g.Name == "Herb Spiral");
        herbSpiral.PlantCount.ShouldBe(0);
        herbSpiral.IsOfficial.ShouldBeTrue();
    }

    [Fact]
    public async Task GetAllAsync_ShouldSortByName()
    {
        DbContext.Guilds.AddRange(
            new Guild { Id = Guid.NewGuid(), Name = "Walnut Guild" },
            new Guild { Id = Guid.NewGuid(), Name = "Apple Guild" },
            new Guild { Id = Guid.NewGuid(), Name = "Mulberry Guild" }
        );
        await DbContext.SaveChangesAsync();

        var result = (await _sut.GetAllAsync(_userId)).ToList();

        result.Count.ShouldBe(3);
        result[0].Name.ShouldBe("Apple Guild");
        result[1].Name.ShouldBe("Mulberry Guild");
        result[2].Name.ShouldBe("Walnut Guild");
    }

    [Fact]
    public async Task GetByIdAsync_WhenNotFound_ShouldReturnNull()
    {
        var result = await _sut.GetByIdAsync(Guid.NewGuid(), _userId);

        result.ShouldBeNull();
    }

    [Fact]
    public async Task GetByIdAsync_WhenFound_ShouldReturnGuildWithPlants()
    {
        var guild = new Guild
        {
            Id = Guid.NewGuid(),
            Name = "Three Sisters",
            Description = "Classic companion planting guild"
        };
        var corn = new Plant { Id = Guid.NewGuid(), Name = "Corn", ScientificName = "Zea mays" };
        var beans = new Plant { Id = Guid.NewGuid(), Name = "Beans", ScientificName = "Phaseolus vulgaris" };

        DbContext.Guilds.Add(guild);
        DbContext.Plants.AddRange(corn, beans);
        DbContext.GuildPlants.AddRange(
            new GuildPlant { GuildId = guild.Id, PlantId = corn.Id },
            new GuildPlant { GuildId = guild.Id, PlantId = beans.Id }
        );
        await DbContext.SaveChangesAsync();

        var result = await _sut.GetByIdAsync(guild.Id, _userId);

        result.ShouldNotBeNull();
        result.Id.ShouldBe(guild.Id);
        result.Name.ShouldBe("Three Sisters");
        result.Description.ShouldBe("Classic companion planting guild");
        result.Plants.Count.ShouldBe(2);
        result.Plants.ShouldContain(p => p.Id == corn.Id && p.Name == "Corn" && p.ScientificName == "Zea mays");
        result.Plants.ShouldContain(p => p.Id == beans.Id && p.Name == "Beans" && p.ScientificName == "Phaseolus vulgaris");
        result.IsOfficial.ShouldBeTrue();
        result.IsOwner.ShouldBeFalse();
    }

    [Fact]
    public async Task GetByIdAsync_ShouldSortPlantsByName()
    {
        var guild = new Guild { Id = Guid.NewGuid(), Name = "Test Guild" };

        var squash = new Plant { Id = Guid.NewGuid(), Name = "Squash" };
        var beans = new Plant { Id = Guid.NewGuid(), Name = "Beans" };
        var corn = new Plant { Id = Guid.NewGuid(), Name = "Corn" };

        DbContext.Guilds.Add(guild);
        DbContext.Plants.AddRange(squash, beans, corn);
        DbContext.GuildPlants.AddRange(
            new GuildPlant { GuildId = guild.Id, PlantId = squash.Id },
            new GuildPlant { GuildId = guild.Id, PlantId = beans.Id },
            new GuildPlant { GuildId = guild.Id, PlantId = corn.Id }
        );
        await DbContext.SaveChangesAsync();

        var result = await _sut.GetByIdAsync(guild.Id, _userId);

        result.ShouldNotBeNull();
        result.Plants.Count.ShouldBe(3);
        result.Plants[0].Name.ShouldBe("Beans");
        result.Plants[1].Name.ShouldBe("Corn");
        result.Plants[2].Name.ShouldBe("Squash");
    }
}
