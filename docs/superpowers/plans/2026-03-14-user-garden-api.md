# User & Garden API Implementation Plan

> **For agentic workers:** REQUIRED: Use superpowers:subagent-driven-development (if subagents available) or superpowers:executing-plans to implement this plan. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Add a seeded `User` entity, a `Garden` entity linked to `User`, full CRUD REST API for Gardens, and a reusable SQLite in-memory test infrastructure.

**Architecture:** Flat folder structure — `Entities/`, `DTOs/`, `Services/`, `Controllers/` under the API project. A thin `GardensController` delegates to `GardenService` which directly uses `AppDbContext`. Tests extend a shared `DatabaseTestBase` that spins up a SQLite in-memory database, so future test classes only need to inherit it.

**Tech Stack:** .NET 10 · ASP.NET Core Controllers · EF Core 10 + Npgsql (prod) / SQLite (tests) · xUnit · Shouldly · EFCore.NamingConventions

---

## File Map

### Created
| File | Responsibility |
|------|---------------|
| `garden-assistant-api/Entities/User.cs` | `User` POCO with `Id`, `Email`, navigation to `Gardens` |
| `garden-assistant-api/Entities/Garden.cs` | `Garden` POCO with `Id`, `Name`, `Description`, `UserId` FK |
| `garden-assistant-api/DTOs/GardenDto.cs` | Read-model record returned from API |
| `garden-assistant-api/DTOs/CreateGardenRequest.cs` | Write-model for POST |
| `garden-assistant-api/DTOs/UpdateGardenRequest.cs` | Write-model for PUT |
| `garden-assistant-api/Services/GardenService.cs` | All garden business logic against `AppDbContext` |
| `garden-assistant-api/Controllers/GardensController.cs` | Thin controller — validate → call service → return |
| `garden-assistant-tests/Infrastructure/DatabaseTestBase.cs` | Abstract base: opens SQLite connection, creates schema, disposes |

### Modified
| File | Change |
|------|--------|
| `garden-assistant-api/Data/AppDbContext.cs` | Add `DbSet<User>`, `DbSet<Garden>`, Fluent API config, seed default user |
| `garden-assistant-api/Program.cs` | Register controllers, `GardenService`, apply pending migrations |
| `garden-assistant-api/garden-assistant-api.csproj` | Fix `RootNamespace` to `GardenAssistant` |
| `garden-assistant-tests/garden-assistant-tests.csproj` | Fix `RootNamespace` to `GardenAssistant.Tests`; add `Microsoft.EntityFrameworkCore.Sqlite` |
| `CLAUDE.md` | Fix tech stack header (.NET 9 → .NET 10); add `UserId` FK convention |

### Migration (generated, not hand-edited)
| File | Change |
|------|--------|
| `garden-assistant-api/Data/Migrations/` | `AddUserAndGarden` migration files produced by EF tooling |

---

## Chunk 1: Entities, DTOs, DbContext, and Migration

### Task 1: Fix csproj RootNamespace and update CLAUDE.md

**Files:**
- Modify: `garden-assistant-api/garden-assistant-api.csproj`
- Modify: `garden-assistant-tests/garden-assistant-tests.csproj`
- Modify: `CLAUDE.md`

- [ ] **Step 1: Fix `RootNamespace` in `garden-assistant-api/garden-assistant-api.csproj`**

Change `<RootNamespace>garden_assistant_api</RootNamespace>` to:

```xml
<RootNamespace>GardenAssistant</RootNamespace>
```

- [ ] **Step 2: Fix `RootNamespace` in `garden-assistant-tests/garden-assistant-tests.csproj`**

Change `<RootNamespace>garden_assistant_tests</RootNamespace>` to:

```xml
<RootNamespace>GardenAssistant.Tests</RootNamespace>
```

- [ ] **Step 3: In `CLAUDE.md`, fix the tech stack header (first line after the title)**

Change:
```
Garden Assistant · Angular (frontend) · .NET 9 / ASP.NET Core (backend) · PostgreSQL 17
```
To:
```
Garden Assistant · Angular (frontend) · .NET 10 / ASP.NET Core (backend) · PostgreSQL 17
```

- [ ] **Step 4: In `CLAUDE.md`, add the following under the `### Database` section, after the snake_case rule:**

```markdown
- Every table that stores user-scoped data **must** include a `UserId` column (Guid) with a foreign key referencing the `users` table. Add this at entity creation time — do not retrofit later.
```

- [ ] **Step 5: Commit**

```bash
git add garden-assistant-api/garden-assistant-api.csproj garden-assistant-tests/garden-assistant-tests.csproj CLAUDE.md
git commit -m "docs: fix RootNamespace in csproj files, update CLAUDE.md tech stack and UserId FK convention"
```

---

### Task 2: Create User and Garden entities

**Files:**
- Create: `garden-assistant-api/Entities/User.cs`
- Create: `garden-assistant-api/Entities/Garden.cs`

- [ ] **Step 1: Create `garden-assistant-api/Entities/User.cs`**

```csharp
namespace GardenAssistant.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;

    public ICollection<Garden> Gardens { get; set; } = [];
}
```

- [ ] **Step 2: Create `garden-assistant-api/Entities/Garden.cs`**

```csharp
namespace GardenAssistant.Entities;

public class Garden
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    public Guid UserId { get; set; }
    public User User { get; set; } = null!;
}
```

- [ ] **Step 3: Verify the project still builds**

```bash
cd garden-assistant-api && dotnet build
```

Expected: `Build succeeded.`

- [ ] **Step 4: Commit**

```bash
git add garden-assistant-api/Entities/
git commit -m "feat: add User and Garden entities"
```

---

### Task 3: Update AppDbContext — DbSets, Fluent API, seed

**Files:**
- Modify: `garden-assistant-api/Data/AppDbContext.cs`

- [ ] **Step 1: Replace `AppDbContext.cs` with the following:**

```csharp
using GardenAssistant.Entities;
using Microsoft.EntityFrameworkCore;

namespace GardenAssistant.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Garden> Gardens => Set<Garden>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Email).IsRequired().HasMaxLength(256);
            entity.HasMany(u => u.Gardens)
                  .WithOne(g => g.User)
                  .HasForeignKey(g => g.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Garden>(entity =>
        {
            entity.HasKey(g => g.Id);
            entity.Property(g => g.Name).IsRequired().HasMaxLength(256);
            entity.Property(g => g.Description).HasMaxLength(2000);
        });

        // Default user — used during development before auth is implemented
        modelBuilder.Entity<User>().HasData(new User
        {
            Id = new Guid("00000000-0000-0000-0000-000000000001"),
            Email = "admin@gardenassistant.local"
        });
    }
}
```

- [ ] **Step 2: Build to confirm no compilation errors**

```bash
cd garden-assistant-api && dotnet build
```

Expected: `Build succeeded.`

- [ ] **Step 3: Commit**

```bash
git add garden-assistant-api/Data/AppDbContext.cs
git commit -m "feat: configure User and Garden in AppDbContext with seed data"
```

---

### Task 4: Create DTOs

**Files:**
- Create: `garden-assistant-api/DTOs/GardenDto.cs`
- Create: `garden-assistant-api/DTOs/CreateGardenRequest.cs`
- Create: `garden-assistant-api/DTOs/UpdateGardenRequest.cs`

- [ ] **Step 1: Create `garden-assistant-api/DTOs/GardenDto.cs`**

```csharp
namespace GardenAssistant.DTOs;

public record GardenDto(Guid Id, string Name, string? Description, Guid UserId);
```

- [ ] **Step 2: Create `garden-assistant-api/DTOs/CreateGardenRequest.cs`**

```csharp
namespace GardenAssistant.DTOs;

public record CreateGardenRequest(string Name, string? Description, Guid UserId);
```

- [ ] **Step 3: Create `garden-assistant-api/DTOs/UpdateGardenRequest.cs`**

```csharp
namespace GardenAssistant.DTOs;

public record UpdateGardenRequest(string Name, string? Description);
```

- [ ] **Step 4: Build to confirm**

```bash
cd garden-assistant-api && dotnet build
```

Expected: `Build succeeded.`

- [ ] **Step 5: Commit**

```bash
git add garden-assistant-api/DTOs/
git commit -m "feat: add Garden DTOs"
```

---

### Task 5: Install EF tools locally and create migration

**Files:**
- Create: `.config/dotnet-tools.json` (generated by dotnet)
- Create: `garden-assistant-api/Data/Migrations/` (generated by EF)

- [ ] **Step 1: Create a local tools manifest (if one doesn't already exist)**

```bash
cd /home/mg/dev/garden-assistant && dotnet new tool-manifest
```

Expected: `The template "Dotnet local tool manifest file" was created successfully.`

- [ ] **Step 2: Install the EF Core tools locally**

```bash
dotnet tool install dotnet-ef --version 10.0.5
```

Expected: `dotnet-ef (10.0.5) was successfully installed.`

- [ ] **Step 3: Create the migration**

```bash
cd garden-assistant-api && dotnet ef migrations add AddUserAndGarden --output-dir Data/Migrations
```

Expected: output ending with `Done. To undo this action, use 'ef migrations remove'`

- [ ] **Step 4: Verify the generated migration files look correct**

Open `garden-assistant-api/Data/Migrations/<timestamp>_AddUserAndGarden.cs` and confirm:
- `users` table created with `id` (uuid) and `email` columns
- `gardens` table created with `id`, `name`, `description`, `user_id` columns
- Foreign key from `gardens.user_id` → `users.id`
- Seed insert for the default user

- [ ] **Step 5: Commit**

```bash
git add .config/dotnet-tools.json garden-assistant-api/Data/Migrations/
git commit -m "feat: add EF migration AddUserAndGarden"
```

---

## Chunk 2: Test Infrastructure and GardenService (TDD)

### Task 6: Add SQLite package and create DatabaseTestBase

**Files:**
- Modify: `garden-assistant-tests/garden-assistant-tests.csproj`
- Create: `garden-assistant-tests/Infrastructure/DatabaseTestBase.cs`

- [ ] **Step 1: Add the SQLite EF Core provider to the test project**

```bash
cd garden-assistant-tests && dotnet add package Microsoft.EntityFrameworkCore.Sqlite --version 10.0.5
```

Expected: package added, version `10.0.5`.

- [ ] **Step 2: Create `garden-assistant-tests/Infrastructure/DatabaseTestBase.cs`**

```csharp
using GardenAssistant.Data;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace GardenAssistant.Tests.Infrastructure;

/// <summary>
/// Base class for tests that need a real database.
/// Creates an in-memory SQLite database per test class, applies the EF schema,
/// and disposes both the context and the connection after each test run.
///
/// Usage: inherit this class and inject <see cref="DbContext"/> into your system under test.
/// </summary>
public abstract class DatabaseTestBase : IDisposable
{
    private readonly SqliteConnection _connection;
    protected AppDbContext DbContext { get; }

    protected DatabaseTestBase()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(_connection)
            .UseSnakeCaseNamingConvention()
            .Options;

        DbContext = new AppDbContext(options);
        DbContext.Database.EnsureCreated();
    }

    public void Dispose()
    {
        DbContext.Dispose();
        _connection.Dispose();
    }
}
```

> **Why keep the connection open?** SQLite in-memory databases are tied to the connection lifetime. If EF Core closes and reopens the connection between queries, the data is lost. Holding the connection open in `DatabaseTestBase` prevents that.

- [ ] **Step 3: Build the test project to confirm everything compiles**

```bash
cd garden-assistant-tests && dotnet build
```

Expected: `Build succeeded.`

- [ ] **Step 4: Commit**

```bash
git add garden-assistant-tests/garden-assistant-tests.csproj garden-assistant-tests/Infrastructure/DatabaseTestBase.cs
git commit -m "test: add reusable SQLite in-memory DatabaseTestBase"
```

---

### Task 7: Implement GardenService using TDD

**Files:**
- Create: `garden-assistant-tests/Gardens/GardenServiceTests.cs`  ← write first
- Create: `garden-assistant-api/Services/GardenService.cs`  ← write after tests

- [ ] **Step 1: Write the failing tests — create `garden-assistant-tests/Gardens/GardenServiceTests.cs`**

```csharp
using GardenAssistant.DTOs;
using GardenAssistant.Entities;
using GardenAssistant.Services;
using GardenAssistant.Tests.Infrastructure;
using Shouldly;

namespace GardenAssistant.Tests.Gardens;

public class GardenServiceTests : DatabaseTestBase
{
    private readonly GardenService _sut;
    private static readonly Guid DefaultUserId = new("00000000-0000-0000-0000-000000000001");

    public GardenServiceTests()
    {
        // The default user (DefaultUserId) is already present because
        // DatabaseTestBase.EnsureCreated() applies HasData from AppDbContext.
        // No explicit seeding needed here.
        _sut = new GardenService(DbContext);
    }

    [Fact]
    public async Task GetAllAsync_WhenNoGardens_ShouldReturnEmpty()
    {
        var result = await _sut.GetAllAsync();

        result.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetAllAsync_WhenGardensExist_ShouldReturnAll()
    {
        DbContext.Gardens.Add(new Garden
        {
            Id = Guid.NewGuid(),
            Name = "My Garden",
            Description = "Lovely",
            UserId = DefaultUserId
        });
        await DbContext.SaveChangesAsync();

        var result = await _sut.GetAllAsync();

        result.Count().ShouldBe(1);
        result.First().Name.ShouldBe("My Garden");
    }

    [Fact]
    public async Task CreateAsync_WhenValidRequest_ShouldPersistAndReturnGarden()
    {
        var request = new CreateGardenRequest("Rose Garden", "Full of roses", DefaultUserId);

        var result = await _sut.CreateAsync(request);

        result.Id.ShouldNotBe(Guid.Empty);
        result.Name.ShouldBe("Rose Garden");
        result.Description.ShouldBe("Full of roses");
        result.UserId.ShouldBe(DefaultUserId);
        DbContext.Gardens.Count().ShouldBe(1);
    }

    [Fact]
    public async Task UpdateAsync_WhenGardenExists_ShouldUpdateAndReturn()
    {
        var gardenId = Guid.NewGuid();
        DbContext.Gardens.Add(new Garden { Id = gardenId, Name = "Old Name", UserId = DefaultUserId });
        await DbContext.SaveChangesAsync();

        var result = await _sut.UpdateAsync(gardenId, new UpdateGardenRequest("New Name", "New Desc"));

        result.ShouldNotBeNull();
        result.Name.ShouldBe("New Name");
        result.Description.ShouldBe("New Desc");
    }

    [Fact]
    public async Task UpdateAsync_WhenGardenNotFound_ShouldReturnNull()
    {
        var result = await _sut.UpdateAsync(Guid.NewGuid(), new UpdateGardenRequest("Name", null));

        result.ShouldBeNull();
    }

    [Fact]
    public async Task DeleteAsync_WhenGardenExists_ShouldRemoveAndReturnTrue()
    {
        var gardenId = Guid.NewGuid();
        DbContext.Gardens.Add(new Garden { Id = gardenId, Name = "Garden", UserId = DefaultUserId });
        await DbContext.SaveChangesAsync();

        var result = await _sut.DeleteAsync(gardenId);

        result.ShouldBeTrue();
        DbContext.Gardens.Count().ShouldBe(0);
    }

    [Fact]
    public async Task DeleteAsync_WhenGardenNotFound_ShouldReturnFalse()
    {
        var result = await _sut.DeleteAsync(Guid.NewGuid());

        result.ShouldBeFalse();
    }
}
```

- [ ] **Step 2: Run the tests — expect them to fail (GardenService doesn't exist yet)**

```bash
cd garden-assistant-tests && dotnet test --filter "FullyQualifiedName~GardenServiceTests" -v minimal 2>&1 | tail -20
```

Expected: build error or test failures — `GardenService` type not found.

- [ ] **Step 3: Create `garden-assistant-api/Services/GardenService.cs`**

```csharp
using GardenAssistant.Data;
using GardenAssistant.DTOs;
using GardenAssistant.Entities;
using Microsoft.EntityFrameworkCore;

namespace GardenAssistant.Services;

public class GardenService(AppDbContext dbContext)
{
    public async Task<IEnumerable<GardenDto>> GetAllAsync()
    {
        return await dbContext.Gardens
            .Select(g => new GardenDto(g.Id, g.Name, g.Description, g.UserId))
            .ToListAsync();
    }

    public async Task<GardenDto> CreateAsync(CreateGardenRequest request)
    {
        var garden = new Garden
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            UserId = request.UserId
        };

        dbContext.Gardens.Add(garden);
        await dbContext.SaveChangesAsync();

        return new GardenDto(garden.Id, garden.Name, garden.Description, garden.UserId);
    }

    public async Task<GardenDto?> UpdateAsync(Guid id, UpdateGardenRequest request)
    {
        var garden = await dbContext.Gardens.FindAsync(id);
        if (garden is null) return null;

        garden.Name = request.Name;
        garden.Description = request.Description;
        await dbContext.SaveChangesAsync();

        return new GardenDto(garden.Id, garden.Name, garden.Description, garden.UserId);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var garden = await dbContext.Gardens.FindAsync(id);
        if (garden is null) return false;

        dbContext.Gardens.Remove(garden);
        await dbContext.SaveChangesAsync();

        return true;
    }
}
```

- [ ] **Step 4: Run the tests again — expect them all to pass**

```bash
cd garden-assistant-tests && dotnet test --filter "FullyQualifiedName~GardenServiceTests" -v minimal 2>&1 | tail -20
```

Expected: `Passed! - Failed: 0, Passed: 6, Skipped: 0`

- [ ] **Step 5: Commit**

```bash
git add garden-assistant-tests/Gardens/GardenServiceTests.cs garden-assistant-api/Services/GardenService.cs
git commit -m "feat: implement GardenService with full unit test coverage"
```

---

## Chunk 3: Controller, Registration, and Final Verification

### Task 8: Create GardensController

**Files:**
- Create: `garden-assistant-api/Controllers/GardensController.cs`

- [ ] **Step 1: Create `garden-assistant-api/Controllers/GardensController.cs`**

```csharp
using GardenAssistant.DTOs;
using GardenAssistant.Services;
using Microsoft.AspNetCore.Mvc;

namespace GardenAssistant.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GardensController(GardenService gardenService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll() =>
        Ok(await gardenService.GetAllAsync());

    [HttpPost]
    public async Task<IActionResult> Create(CreateGardenRequest request)
    {
        var garden = await gardenService.CreateAsync(request);
        return CreatedAtAction(nameof(GetAll), new { id = garden.Id }, garden);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateGardenRequest request)
    {
        var garden = await gardenService.UpdateAsync(id, request);
        return garden is null ? NotFound() : Ok(garden);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await gardenService.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }
}
```

- [ ] **Step 2: Build to confirm**

```bash
cd garden-assistant-api && dotnet build
```

Expected: `Build succeeded.`

- [ ] **Step 3: Commit**

```bash
git add garden-assistant-api/Controllers/GardensController.cs
git commit -m "feat: add GardensController with CRUD endpoints"
```

---

### Task 9: Wire up services and controllers in Program.cs

**Files:**
- Modify: `garden-assistant-api/Program.cs`

- [ ] **Step 1: Replace `Program.cs` with the following:**

```csharp
using EFCore.NamingConventions;
using GardenAssistant.Data;
using GardenAssistant.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Default"))
           .UseSnakeCaseNamingConvention());

builder.Services.AddScoped<GardenService>();
builder.Services.AddControllers();

var app = builder.Build();

// Apply pending migrations on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));
app.MapControllers();

app.Run();
```

- [ ] **Step 2: Build to confirm everything compiles**

```bash
cd garden-assistant-api && dotnet build
```

Expected: `Build succeeded.`

- [ ] **Step 3: Run the full test suite to confirm nothing is broken**

```bash
cd garden-assistant-tests && dotnet test -v minimal 2>&1 | tail -10
```

Expected: `Passed! - Failed: 0`

- [ ] **Step 4: Commit**

```bash
git add garden-assistant-api/Program.cs
git commit -m "feat: register GardenService and controllers, apply migrations on startup"
```

---

## Done

All tasks complete. The result is:

- `User` table with seeded default user (`00000000-0000-0000-0000-000000000001 / admin@gardenassistant.local`)
- `Garden` table linked to `User` via `user_id` FK
- `GET /api/gardens`, `POST /api/gardens`, `PUT /api/gardens/{id}`, `DELETE /api/gardens/{id}`
- 6 unit tests covering all `GardenService` methods, running against SQLite in-memory
- `DatabaseTestBase` ready for future test classes to inherit
- CLAUDE.md updated with the `UserId` FK convention
