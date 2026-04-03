using GardenAssistant.Data.Entities;
using GardenAssistant.Data.Entities.Enums;
using GardenAssistant.DTOs.Guilds;
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
        DbContext.Users.Add(new User { Id = _userId, Email = "test@example.com" });
        DbContext.SaveChanges();
        _sut = new GuildService(DbContext);
    }

    [Fact]
    public async Task GetAllAsync_WhenNoGuilds_ShouldReturnEmpty()
    {
        var result = await _sut.GetAllAsync(_userId);

        result.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetAllAsync_WhenGuildsExist_ShouldReturnAllWithPlants()
    {
        // Arrange
        var guildWithPlants = new Guild { Id = Guid.NewGuid(), Name = "Three Sisters" };
        var guildWithoutPlants = new Guild { Id = Guid.NewGuid(), Name = "Herb Spiral" };

        var corn = new Plant { Id = Guid.NewGuid(), Key = "corn", Name = "Corn" };
        var beans = new Plant { Id = Guid.NewGuid(), Key = "beans", Name = "Beans" };
        var squash = new Plant { Id = Guid.NewGuid(), Key = "squash", Name = "Squash" };

        DbContext.Guilds.AddRange(guildWithPlants, guildWithoutPlants);
        DbContext.Plants.AddRange(corn, beans, squash);
        DbContext.GuildPlants.AddRange(
            new GuildPlant { GuildId = guildWithPlants.Id, PlantId = corn.Id, Role = GuildPlantRole.Central },
            new GuildPlant { GuildId = guildWithPlants.Id, PlantId = beans.Id, Role = GuildPlantRole.Central },
            new GuildPlant { GuildId = guildWithPlants.Id, PlantId = squash.Id, Role = GuildPlantRole.Central }
        );
        await DbContext.SaveChangesAsync();

        // Act
        var result = (await _sut.GetAllAsync(_userId)).ToList();

        // Assert
        result.Count.ShouldBe(2);

        var threeSisters = result.Single(g => g.Name == "Three Sisters");
        threeSisters.Plants.Count.ShouldBe(3);
        threeSisters.Plants.ShouldContain(p => p.Name == "Corn");
        threeSisters.Plants.ShouldAllBe(p => p.Role == GuildPlantRole.Central);
        threeSisters.Id.ShouldBe(guildWithPlants.Id);
        threeSisters.IsOfficial.ShouldBeTrue();

        var herbSpiral = result.Single(g => g.Name == "Herb Spiral");
        herbSpiral.Plants.ShouldBeEmpty();
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
        // Arrange
        var guild = new Guild
        {
            Id = Guid.NewGuid(),
            Name = "Three Sisters",
            Description = "Classic companion planting guild"
        };
        var corn = new Plant { Id = Guid.NewGuid(), Key = "corn", Name = "Corn", ScientificName = "Zea mays" };
        var beans = new Plant { Id = Guid.NewGuid(), Key = "beans", Name = "Beans", ScientificName = "Phaseolus vulgaris" };

        DbContext.Guilds.Add(guild);
        DbContext.Plants.AddRange(corn, beans);
        DbContext.GuildPlants.AddRange(
            new GuildPlant { GuildId = guild.Id, PlantId = corn.Id },
            new GuildPlant { GuildId = guild.Id, PlantId = beans.Id }
        );
        await DbContext.SaveChangesAsync();

        // Act
        var result = await _sut.GetByIdAsync(guild.Id, _userId);

        // Assert
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
        // Arrange
        var guild = new Guild { Id = Guid.NewGuid(), Name = "Test Guild" };

        var squash = new Plant { Id = Guid.NewGuid(), Key = "squash", Name = "Squash" };
        var beans = new Plant { Id = Guid.NewGuid(), Key = "beans", Name = "Beans" };
        var corn = new Plant { Id = Guid.NewGuid(), Key = "corn", Name = "Corn" };

        DbContext.Guilds.Add(guild);
        DbContext.Plants.AddRange(squash, beans, corn);
        DbContext.GuildPlants.AddRange(
            new GuildPlant { GuildId = guild.Id, PlantId = squash.Id },
            new GuildPlant { GuildId = guild.Id, PlantId = beans.Id },
            new GuildPlant { GuildId = guild.Id, PlantId = corn.Id }
        );
        await DbContext.SaveChangesAsync();

        // Act
        var result = await _sut.GetByIdAsync(guild.Id, _userId);

        // Assert
        result.ShouldNotBeNull();
        result.Plants.Count.ShouldBe(3);
        result.Plants[0].Name.ShouldBe("Beans");
        result.Plants[1].Name.ShouldBe("Corn");
        result.Plants[2].Name.ShouldBe("Squash");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldSortCentralPlantsFirst()
    {
        // Arrange
        var guild = new Guild { Id = Guid.NewGuid(), Name = "Mixed Role Guild" };

        var apple = new Plant { Id = Guid.NewGuid(), Key = "apple", Name = "Apple" };
        var comfrey = new Plant { Id = Guid.NewGuid(), Key = "comfrey", Name = "Comfrey" };
        var daffodil = new Plant { Id = Guid.NewGuid(), Key = "daffodil", Name = "Daffodil" };
        var basil = new Plant { Id = Guid.NewGuid(), Key = "basil", Name = "Basil" };

        DbContext.Guilds.Add(guild);
        DbContext.Plants.AddRange(apple, comfrey, daffodil, basil);
        DbContext.GuildPlants.AddRange(
            new GuildPlant { GuildId = guild.Id, PlantId = comfrey.Id, Role = GuildPlantRole.Companion },
            new GuildPlant { GuildId = guild.Id, PlantId = daffodil.Id, Role = GuildPlantRole.Central },
            new GuildPlant { GuildId = guild.Id, PlantId = apple.Id, Role = GuildPlantRole.Central },
            new GuildPlant { GuildId = guild.Id, PlantId = basil.Id, Role = GuildPlantRole.Companion }
        );
        await DbContext.SaveChangesAsync();

        // Act
        var result = await _sut.GetByIdAsync(guild.Id, _userId);

        // Assert
        result.ShouldNotBeNull();
        result.Plants.Count.ShouldBe(4);

        result.Plants[0].Name.ShouldBe("Apple");
        result.Plants[0].Role.ShouldBe(GuildPlantRole.Central);

        result.Plants[1].Name.ShouldBe("Daffodil");
        result.Plants[1].Role.ShouldBe(GuildPlantRole.Central);

        result.Plants[2].Name.ShouldBe("Basil");
        result.Plants[2].Role.ShouldBe(GuildPlantRole.Companion);

        result.Plants[3].Name.ShouldBe("Comfrey");
        result.Plants[3].Role.ShouldBe(GuildPlantRole.Companion);
    }

    [Fact]
    public async Task CreateAsync_ShouldPersistPlantRoles()
    {
        // Arrange
        var apple = new Plant { Id = Guid.NewGuid(), Key = "apple", Name = "Apple" };
        var comfrey = new Plant { Id = Guid.NewGuid(), Key = "comfrey", Name = "Comfrey" };
        var clover = new Plant { Id = Guid.NewGuid(), Key = "clover", Name = "Clover" };

        DbContext.Plants.AddRange(apple, comfrey, clover);
        await DbContext.SaveChangesAsync();

        var request = new CreateGuildRequest(
            "Apple Guild",
            "An apple-centered guild",
            [
                new GuildPlantRequest(apple.Id, GuildPlantRole.Central),
                new GuildPlantRequest(comfrey.Id, GuildPlantRole.Companion),
                new GuildPlantRequest(clover.Id)
            ]);

        // Act
        var result = await _sut.CreateAsync(request, _userId);

        // Assert
        result.ShouldNotBeNull();
        result.Name.ShouldBe("Apple Guild");
        result.Plants.Count.ShouldBe(3);

        var applePlant = result.Plants.Single(p => p.Name == "Apple");
        applePlant.Role.ShouldBe(GuildPlantRole.Central);

        var comfreyPlant = result.Plants.Single(p => p.Name == "Comfrey");
        comfreyPlant.Role.ShouldBe(GuildPlantRole.Companion);

        var cloverPlant = result.Plants.Single(p => p.Name == "Clover");
        cloverPlant.Role.ShouldBe(GuildPlantRole.Companion);
    }

    [Fact]
    public async Task CreateAsync_WhenNoRoleSpecified_ShouldDefaultToCompanion()
    {
        // Arrange
        var mint = new Plant { Id = Guid.NewGuid(), Key = "mint", Name = "Mint" };
        var basil = new Plant { Id = Guid.NewGuid(), Key = "basil", Name = "Basil" };

        DbContext.Plants.AddRange(mint, basil);
        await DbContext.SaveChangesAsync();

        var request = new CreateGuildRequest(
            "Herb Guild",
            null,
            [
                new GuildPlantRequest(mint.Id),
                new GuildPlantRequest(basil.Id)
            ]);

        // Act
        var result = await _sut.CreateAsync(request, _userId);

        // Assert
        result.ShouldNotBeNull();
        result.Plants.Count.ShouldBe(2);
        result.Plants.ShouldAllBe(p => p.Role == GuildPlantRole.Companion);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdatePlantRoles()
    {
        // Arrange
        var apple = new Plant { Id = Guid.NewGuid(), Key = "apple", Name = "Apple" };
        var comfrey = new Plant { Id = Guid.NewGuid(), Key = "comfrey", Name = "Comfrey" };

        DbContext.Plants.AddRange(apple, comfrey);
        await DbContext.SaveChangesAsync();

        var createRequest = new CreateGuildRequest(
            "Apple Guild",
            null,
            [
                new GuildPlantRequest(apple.Id, GuildPlantRole.Companion),
                new GuildPlantRequest(comfrey.Id, GuildPlantRole.Companion)
            ]);
        var created = await _sut.CreateAsync(createRequest, _userId);

        created.Plants.Single(p => p.Name == "Apple").Role.ShouldBe(GuildPlantRole.Companion);
        created.Plants.Single(p => p.Name == "Comfrey").Role.ShouldBe(GuildPlantRole.Companion);

        var updateRequest = new UpdateGuildRequest(
            "Apple Guild",
            null,
            [
                new GuildPlantRequest(apple.Id, GuildPlantRole.Central),
                new GuildPlantRequest(comfrey.Id, GuildPlantRole.Central)
            ]);

        // Act
        var result = await _sut.UpdateAsync(created.Id, updateRequest, _userId);

        // Assert
        result.ShouldNotBeNull();
        result.Plants.Count.ShouldBe(2);

        var applePlant = result.Plants.Single(p => p.Name == "Apple");
        applePlant.Role.ShouldBe(GuildPlantRole.Central);

        var comfreyPlant = result.Plants.Single(p => p.Name == "Comfrey");
        comfreyPlant.Role.ShouldBe(GuildPlantRole.Central);
    }
}
