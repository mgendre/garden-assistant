# Watering Calendar — Move to Garden View

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Move the watering calendar out of `/calendar` and into each garden's view as two mutually-exclusive collapsible panels (cultural calendar + watering), both collapsed by default.

**Architecture:** Replace the global `IWateringService`/`WateringService` (multi-source) with a lean `IGardenWateringService`/`GardenWateringService` scoped to one garden. Frontend: a new `GardenWatering` component with local state replaces the global `WateringStore`. The `Collapsible` component gains a controlled mode (`open` input + `toggled` output) to let `GardenView` coordinate mutual exclusivity.

**Tech Stack:** .NET 10 / ASP.NET Core, EF Core, xUnit + Moq + Shouldly, Angular 19 signals, ngx-translate

---

## File map

### Backend — delete
- `garden-assistant-api/Services/Watering/IWateringService.cs`
- `garden-assistant-api/Services/Watering/WateringService.cs`
- `garden-assistant-api/DTOs/Watering/WateringTodayDto.cs`
- `garden-assistant-api/DTOs/Watering/BedWateringTodayDto.cs`
- `garden-assistant-api/DTOs/Watering/PlantWateringStatusDto.cs`
- `garden-assistant-tests/Watering/WateringServiceTodayTests.cs`
- `garden-assistant-tests/Watering/WateringServiceScheduleTests.cs`

### Backend — create
- `garden-assistant-api/Services/Watering/IGardenWateringService.cs`
- `garden-assistant-api/Services/Watering/GardenWateringService.cs`
- `garden-assistant-tests/Watering/GardenWateringServiceTests.cs`

### Backend — modify
- `garden-assistant-api/Controllers/GardensController.cs` — add watering endpoint
- `garden-assistant-api/Controllers/CalendarController.cs` — remove watering endpoints + injection
- `garden-assistant-api/ServiceCollectionExtensions.cs` — swap service registration

### Frontend — delete
- `garden-assistant-app/src/app/shared/services/watering.store.ts`
- `garden-assistant-app/src/app/features/calendar/calendar-watering/` (entire folder)
- `garden-assistant-app/src/app/features/calendar/calendar-watering-today/` (entire folder)

### Frontend — create
- `garden-assistant-app/src/app/features/garden/garden-watering/garden-watering.ts`
- `garden-assistant-app/src/app/features/garden/garden-watering/garden-watering.html`
- `garden-assistant-app/src/app/features/garden/garden-watering/garden-watering.scss`

### Frontend — modify
- `garden-assistant-app/src/app/shared/ui/collapsible/collapsible.ts` — add `open` input + `toggled` output
- `garden-assistant-app/src/app/features/garden/garden-calendar/garden-calendar.ts` — add `open`, `toggled`
- `garden-assistant-app/src/app/features/garden/garden-calendar/garden-calendar.html` — use controlled collapsible
- `garden-assistant-app/src/app/features/garden/garden-view/garden-view.ts` — add `activePanel` signal
- `garden-assistant-app/src/app/features/garden/garden-view/garden-view.html` — wire both panels
- `garden-assistant-app/src/app/shared/services/watering.service.ts` — replace methods
- `garden-assistant-app/src/app/api/watering.api.ts` — remove Today types
- `garden-assistant-app/src/app/features/calendar/calendar.ts` — remove watering
- `garden-assistant-app/src/app/features/calendar/calendar.html` — remove watering
- `garden-assistant-app/public/i18n/fr.json` — cleanup + add GardenWatering keys
- `garden-assistant-app/public/i18n/en.json` — cleanup + add GardenWatering keys

---

### Task 1: Backend — GardenWateringService (TDD)

**Files:**
- Create: `garden-assistant-api/Services/Watering/IGardenWateringService.cs`
- Create: `garden-assistant-api/Services/Watering/GardenWateringService.cs`
- Create: `garden-assistant-tests/Watering/GardenWateringServiceTests.cs`

- [ ] **Step 1: Write the failing tests**

Create `garden-assistant-tests/Watering/GardenWateringServiceTests.cs`:

```csharp
using GardenAssistant.Data.Entities;
using GardenAssistant.Data.Entities.Enums;
using GardenAssistant.Services.Watering;
using GardenAssistant.Tests.Infrastructure;
using Shouldly;

namespace GardenAssistant.Tests.Watering;

public class GardenWateringServiceTests : DatabaseTestBase
{
    private readonly GardenWateringService _sut;
    private readonly Guid _userId = Guid.NewGuid();
    private readonly Guid _gardenId = Guid.NewGuid();

    public GardenWateringServiceTests()
    {
        DbContext.Users.Add(new User { Id = _userId, Email = "test@example.com" });
        DbContext.Gardens.Add(new Garden { Id = _gardenId, Name = "Test", UserId = _userId, CreatedAtUtc = DateTime.UtcNow });
        DbContext.SaveChanges();
        _sut = new GardenWateringService(DbContext, new WateringCalculator());
    }

    [Fact]
    public async Task GetScheduleAsync_WhenNoBeds_ShouldReturnEmptyBeds()
    {
        var result = await _sut.GetScheduleAsync(_userId, _gardenId, halfMonth: 13);
        result.Beds.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetScheduleAsync_WhenBedHasPlants_ShouldReturnBedWithFrequency()
    {
        var plant = new Plant { Id = Guid.NewGuid(), Key = "tomate", Name = "Tomate", WaterNeeds = WaterNeeds.High };
        var guild = new Guild { Id = Guid.NewGuid(), UserId = _userId, Name = "G" };
        var bed = new Planting { Id = Guid.NewGuid(), GardenId = _gardenId, UserId = _userId, Name = "Planche", GuildId = guild.Id, CreatedAtUtc = DateTime.UtcNow };
        DbContext.Plants.Add(plant);
        DbContext.Guilds.Add(guild);
        DbContext.Plantings.Add(bed);
        DbContext.GuildPlants.Add(new GuildPlant { GuildId = guild.Id, PlantId = plant.Id });
        await DbContext.SaveChangesAsync();

        var result = await _sut.GetScheduleAsync(_userId, _gardenId, halfMonth: 13);

        result.Beds.Count.ShouldBe(1);
        result.Beds[0].BedId.ShouldBe(bed.Id);
        result.Beds[0].Plants.Count.ShouldBe(1);
        result.Beds[0].Plants[0].TimesPerWeek.ShouldBe(5); // High/été
    }

    [Fact]
    public async Task GetScheduleAsync_WhenOtherGarden_ShouldReturnEmptyBeds()
    {
        var otherGardenId = Guid.NewGuid();
        DbContext.Gardens.Add(new Garden { Id = otherGardenId, Name = "Other", UserId = _userId, CreatedAtUtc = DateTime.UtcNow });
        var plant = new Plant { Id = Guid.NewGuid(), Key = "tomate-g", Name = "Tomate", WaterNeeds = WaterNeeds.High };
        var guild = new Guild { Id = Guid.NewGuid(), UserId = _userId, Name = "G" };
        var bed = new Planting { Id = Guid.NewGuid(), GardenId = otherGardenId, UserId = _userId, Name = "Planche", GuildId = guild.Id, CreatedAtUtc = DateTime.UtcNow };
        DbContext.Plants.Add(plant);
        DbContext.Guilds.Add(guild);
        DbContext.Plantings.Add(bed);
        DbContext.GuildPlants.Add(new GuildPlant { GuildId = guild.Id, PlantId = plant.Id });
        await DbContext.SaveChangesAsync();

        var result = await _sut.GetScheduleAsync(_userId, _gardenId, halfMonth: 13);
        result.Beds.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetScheduleAsync_WhenOtherUser_ShouldReturnEmptyBeds()
    {
        var otherUser = Guid.NewGuid();
        DbContext.Users.Add(new User { Id = otherUser, Email = "other@test.com" });
        DbContext.SaveChanges();

        var result = await _sut.GetScheduleAsync(otherUser, _gardenId, halfMonth: 13);
        result.Beds.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetScheduleAsync_WhenBedHasMulch_ShouldReduceFrequency()
    {
        var plant = new Plant { Id = Guid.NewGuid(), Key = "tomate-m", Name = "Tomate", WaterNeeds = WaterNeeds.High };
        var guild = new Guild { Id = Guid.NewGuid(), UserId = _userId, Name = "G" };
        var bed = new Planting { Id = Guid.NewGuid(), GardenId = _gardenId, UserId = _userId, Name = "Planche", GuildId = guild.Id, HasMulch = true, CreatedAtUtc = DateTime.UtcNow };
        DbContext.Plants.Add(plant);
        DbContext.Guilds.Add(guild);
        DbContext.Plantings.Add(bed);
        DbContext.GuildPlants.Add(new GuildPlant { GuildId = guild.Id, PlantId = plant.Id });
        await DbContext.SaveChangesAsync();

        var result = await _sut.GetScheduleAsync(_userId, _gardenId, halfMonth: 13);

        // High/été=5, mulch ×0.6 → Math.Round(3) = 3
        result.Beds[0].Plants[0].TimesPerWeek.ShouldBe(3);
    }
}
```

- [ ] **Step 2: Run tests — expect FAIL (type not found)**

```bash
dotnet test garden-assistant-tests --filter "GardenWateringServiceTests" 2>&1 | tail -10
```

Expected: build error — `GardenWateringService` does not exist.

- [ ] **Step 3: Create the interface**

Create `garden-assistant-api/Services/Watering/IGardenWateringService.cs`:

```csharp
using GardenAssistant.DTOs.Watering;

namespace GardenAssistant.Services.Watering;

public interface IGardenWateringService
{
    Task<WateringScheduleDto> GetScheduleAsync(Guid userId, Guid gardenId, int halfMonth);
}
```

- [ ] **Step 4: Create the service**

Create `garden-assistant-api/Services/Watering/GardenWateringService.cs`:

```csharp
using GardenAssistant.Data;
using GardenAssistant.Data.Entities;
using GardenAssistant.Data.Entities.Enums;
using GardenAssistant.DTOs.Watering;
using Microsoft.EntityFrameworkCore;

namespace GardenAssistant.Services.Watering;

public class GardenWateringService(AppDbContext dbContext, IWateringCalculator calculator) : IGardenWateringService
{
    public async Task<WateringScheduleDto> GetScheduleAsync(Guid userId, Guid gardenId, int halfMonth)
    {
        var plantings = await dbContext.Plantings
            .Where(p => p.UserId == userId && p.GardenId == gardenId && p.GuildId.HasValue)
            .ToListAsync();

        if (plantings.Count == 0)
        {
            return new WateringScheduleDto([]);
        }

        var guildIds = plantings.Select(p => p.GuildId!.Value).ToList();

        var guildPlantPairs = await dbContext.GuildPlants
            .Where(gp => guildIds.Contains(gp.GuildId))
            .ToListAsync();

        var plantIds = guildPlantPairs.Select(gp => gp.PlantId).Distinct().ToList();

        var plantsById = await dbContext.Plants
            .Where(p => plantIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id);

        var plantsByGuild = guildPlantPairs
            .GroupBy(gp => gp.GuildId)
            .ToDictionary(
                g => g.Key,
                g => g.Where(gp => plantsById.ContainsKey(gp.PlantId))
                      .Select(gp => plantsById[gp.PlantId])
                      .ToList());

        var beds = plantings.Select(p => BuildBedDto(p, plantsByGuild, halfMonth)).ToList();

        return new WateringScheduleDto(beds);
    }

    private BedWateringDto BuildBedDto(
        Planting planting,
        Dictionary<Guid, List<Plant>> plantsByGuild,
        int halfMonth)
    {
        var plants = plantsByGuild.GetValueOrDefault(planting.GuildId!.Value, [])
            .Select(plant => BuildPlantDto(plant, halfMonth, planting.SoilType, planting.HasMulch))
            .ToList();

        return new BedWateringDto(planting.Id, planting.Name, false, planting.SoilType, planting.HasMulch, plants);
    }

    private PlantWateringDto BuildPlantDto(Plant plant, int halfMonth, SoilType? soilType, bool hasMulch)
    {
        var freq = calculator.CalculateFrequency(plant.WaterNeeds, halfMonth, soilType, hasMulch);
        return new PlantWateringDto(plant.Id, plant.Name, plant.WaterNeeds, freq.TimesPerWeek, freq.RecommendedDays, plant.WaterAmountMl);
    }
}
```

- [ ] **Step 5: Run tests — expect PASS**

```bash
dotnet test garden-assistant-tests --filter "GardenWateringServiceTests" 2>&1 | tail -10
```

Expected: all 5 tests pass.

- [ ] **Step 6: Commit**

```bash
git add garden-assistant-api/Services/Watering/IGardenWateringService.cs \
        garden-assistant-api/Services/Watering/GardenWateringService.cs \
        garden-assistant-tests/Watering/GardenWateringServiceTests.cs
git commit -m "feat: add GardenWateringService scoped to one garden"
```

---

### Task 2: Backend — New endpoint + cleanup

**Files:**
- Modify: `garden-assistant-api/Controllers/GardensController.cs`
- Modify: `garden-assistant-api/Controllers/CalendarController.cs`
- Modify: `garden-assistant-api/ServiceCollectionExtensions.cs`
- Delete: `IWateringService.cs`, `WateringService.cs`, `WateringTodayDto.cs`, `BedWateringTodayDto.cs`, `PlantWateringStatusDto.cs`
- Delete: `WateringServiceTodayTests.cs`, `WateringServiceScheduleTests.cs`

- [ ] **Step 1: Add watering endpoint to GardensController**

Replace content of `garden-assistant-api/Controllers/GardensController.cs`:

```csharp
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using GardenAssistant.DTOs.Gardens;
using GardenAssistant.DTOs.Watering;
using GardenAssistant.Services.Interfaces;
using GardenAssistant.Services.Watering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GardenAssistant.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class GardensController(
    IGardenService gardenService,
    IGardenWateringService gardenWateringService) : ControllerBase
{
    private Guid CallerId =>
        Guid.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub)!);

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<GardenDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll() =>
        Ok(await gardenService.GetAllAsync(CallerId));

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(GardenDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var garden = await gardenService.GetByIdAsync(id, CallerId);
        return garden is null ? NotFound() : Ok(garden);
    }

    [HttpPost]
    [ProducesResponseType(typeof(GardenDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create(CreateGardenRequest request)
    {
        var garden = await gardenService.CreateAsync(request, CallerId);
        return CreatedAtAction(nameof(GetById), new { id = garden.Id }, garden);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(GardenDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(Guid id, UpdateGardenRequest request)
    {
        var garden = await gardenService.UpdateAsync(id, request, CallerId);
        return garden is null ? NotFound() : Ok(garden);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await gardenService.DeleteAsync(id, CallerId);
        return deleted ? NoContent() : NotFound();
    }

    [HttpGet("{gardenId:guid}/watering/schedule")]
    [ProducesResponseType(typeof(WateringScheduleDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetWateringSchedule(
        Guid gardenId,
        [FromQuery][Range(1, 24)] int halfMonth)
    {
        var result = await gardenWateringService.GetScheduleAsync(CallerId, gardenId, halfMonth);
        return Ok(result);
    }
}
```

- [ ] **Step 2: Remove watering from CalendarController**

Replace content of `garden-assistant-api/Controllers/CalendarController.cs`:

```csharp
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using GardenAssistant.DTOs.Calendar;
using GardenAssistant.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GardenAssistant.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class CalendarController(
    IUserPlantService userPlantService,
    IPlantActionService plantActionService) : ControllerBase
{
    private Guid CallerId =>
        Guid.Parse(User.FindFirstValue(JwtRegisteredClaimNames.Sub)!);

    [HttpGet("my-plants")]
    [ProducesResponseType(typeof(CalendarDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyPlantsCalendar()
    {
        var userPlants = (await userPlantService.GetAllAsync(CallerId)).ToList();
        var plantIds = userPlants.Select(p => p.Id).ToList();
        var actionsByPlant = await plantActionService.GetByPlantIdsAsync(plantIds);

        var calendarPlants = plantIds.Select(id => new CalendarPlantDto(
            id,
            actionsByPlant.GetValueOrDefault(id, [])
        )).ToList();

        return Ok(new CalendarDto(calendarPlants));
    }
}
```

- [ ] **Step 3: Update DI registrations**

Replace content of `garden-assistant-api/ServiceCollectionExtensions.cs`:

```csharp
using GardenAssistant.Data.Seeders;
using GardenAssistant.Services;
using GardenAssistant.Services.Interfaces;
using GardenAssistant.Services.Watering;

namespace GardenAssistant;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<ISeeder, PlantSeeder>();
        services.AddScoped<ISeeder, AssociationSeeder>();
        services.AddScoped<ISeeder, GuildSeeder>();
        services.AddScoped<ISeeder, PlantActionSeeder>();
        services.AddScoped<ISeeder, HarvestReadinessSeeder>();
        services.AddScoped<IDatabaseSeeder, DatabaseSeeder>();

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IPlantService, PlantService>();
        services.AddScoped<IPlantAssociationService, PlantAssociationService>();
        services.AddScoped<IGuildService, GuildService>();
        services.AddScoped<IUserPlantService, UserPlantService>();
        services.AddScoped<IPlantActionService, PlantActionService>();
        services.AddScoped<IHarvestReadinessService, HarvestReadinessService>();
        services.AddScoped<IGardenService, GardenService>();
        services.AddScoped<IBedService, BedService>();
        services.AddSingleton<IWateringCalculator, WateringCalculator>();
        services.AddScoped<IGardenWateringService, GardenWateringService>();

        return services;
    }
}
```

- [ ] **Step 4: Delete obsolete backend files**

```bash
rm garden-assistant-api/Services/Watering/IWateringService.cs
rm garden-assistant-api/Services/Watering/WateringService.cs
rm garden-assistant-api/DTOs/Watering/WateringTodayDto.cs
rm garden-assistant-api/DTOs/Watering/BedWateringTodayDto.cs
rm garden-assistant-api/DTOs/Watering/PlantWateringStatusDto.cs
rm garden-assistant-tests/Watering/WateringServiceTodayTests.cs
rm garden-assistant-tests/Watering/WateringServiceScheduleTests.cs
```

- [ ] **Step 5: Build and run all tests — expect PASS**

```bash
dotnet build garden-assistant-api/garden-assistant-api.csproj 2>&1 | tail -5
dotnet test garden-assistant-tests 2>&1 | tail -10
```

Expected: build succeeds, all tests pass.

- [ ] **Step 6: Commit**

```bash
git add -A
git commit -m "refactor: replace WateringService with GardenWateringService, add GET /api/gardens/{id}/watering/schedule"
```

---

### Task 3: Frontend — Collapsible controlled mode

**Files:**
- Modify: `garden-assistant-app/src/app/shared/ui/collapsible/collapsible.ts`

The `Collapsible` currently uses only internal state. We add an `open` input (when non-null, overrides internal state) and a `toggled` output (emitted on click when in controlled mode).

- [ ] **Step 1: Update Collapsible**

Replace content of `garden-assistant-app/src/app/shared/ui/collapsible/collapsible.ts`:

```typescript
import { Component, input, output, signal, computed, effect } from '@angular/core';

@Component({
  selector: 'app-collapsible',
  standalone: true,
  imports: [],
  templateUrl: './collapsible.html',
  styleUrl: './collapsible.scss'
})
export class Collapsible {
  readonly initialExpanded = input(false);
  readonly forceExpanded = input(false);
  readonly open = input<boolean | null>(null);
  readonly toggled = output<boolean>();

  readonly expanded = signal(false);

  readonly isExpanded = computed(() => {
    const ext = this.open();
    if (ext !== null) {
      return ext;
    }
    return this.forceExpanded() || this.expanded();
  });

  constructor() {
    effect(() => {
      if (this.initialExpanded()) {
        this.expanded.set(true);
      }
    });
  }

  toggle(): void {
    const ext = this.open();
    if (ext !== null) {
      this.toggled.emit(!ext);
      return;
    }
    if (!this.forceExpanded()) {
      this.expanded.update(v => !v);
      this.toggled.emit(this.expanded());
    }
  }
}
```

- [ ] **Step 2: Build — expect no errors**

```bash
npm run build --prefix garden-assistant-app 2>&1 | tail -5
```

Expected: `Application bundle generation complete.`

- [ ] **Step 3: Commit**

```bash
git add garden-assistant-app/src/app/shared/ui/collapsible/collapsible.ts
git commit -m "feat(collapsible): add controlled mode via open input and toggled output"
```

---

### Task 4: Frontend — GardenCalendar parent-controlled

**Files:**
- Modify: `garden-assistant-app/src/app/features/garden/garden-calendar/garden-calendar.ts`
- Modify: `garden-assistant-app/src/app/features/garden/garden-calendar/garden-calendar.html`

`GardenCalendar` currently wraps itself in `<app-collapsible [initialExpanded]="true">`. We replace that with a parent-controlled collapsible: `open` and `toggled` pass through to the Collapsible, removing `initialExpanded`.

- [ ] **Step 1: Add open/toggled to GardenCalendar**

In `garden-calendar.ts`, add these two lines alongside the existing `beds = input.required<BedDto[]>()`:

```typescript
readonly open = input<boolean | null>(null);
readonly toggled = output<boolean>();
```

Also add `output` to the imports from `@angular/core`:

```typescript
import { Component, inject, input, output, signal, effect } from '@angular/core';
```

- [ ] **Step 2: Update garden-calendar.html**

Change line 2 from:
```html
  <app-collapsible [initialExpanded]="true">
```
to:
```html
  <app-collapsible [open]="open()" (toggled)="toggled.emit($event)">
```

- [ ] **Step 3: Build — expect no errors**

```bash
npm run build --prefix garden-assistant-app 2>&1 | tail -5
```

Expected: `Application bundle generation complete.`

- [ ] **Step 4: Commit**

```bash
git add garden-assistant-app/src/app/features/garden/garden-calendar/garden-calendar.ts \
        garden-assistant-app/src/app/features/garden/garden-calendar/garden-calendar.html
git commit -m "feat(garden-calendar): support parent-controlled open/close"
```

---

### Task 5: Frontend — GardenWatering component + WateringService update

**Files:**
- Modify: `garden-assistant-app/src/app/shared/services/watering.service.ts`
- Modify: `garden-assistant-app/src/app/api/watering.api.ts`
- Delete: `garden-assistant-app/src/app/shared/services/watering.store.ts`
- Delete: `garden-assistant-app/src/app/features/calendar/calendar-watering/` (folder)
- Delete: `garden-assistant-app/src/app/features/calendar/calendar-watering-today/` (folder)
- Create: `garden-assistant-app/src/app/features/garden/garden-watering/garden-watering.ts`
- Create: `garden-assistant-app/src/app/features/garden/garden-watering/garden-watering.html`
- Create: `garden-assistant-app/src/app/features/garden/garden-watering/garden-watering.scss`

- [ ] **Step 1: Update WateringService — replace methods**

Replace content of `garden-assistant-app/src/app/shared/services/watering.service.ts`:

```typescript
import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { WateringScheduleDto } from '../../api/watering.api';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class WateringService {
  private readonly http = inject(HttpClient);
  private readonly base = environment.apiBaseUrl;

  getGardenSchedule(gardenId: string, halfMonth: number): Promise<WateringScheduleDto> {
    return firstValueFrom(
      this.http.get<WateringScheduleDto>(`${this.base}/api/gardens/${gardenId}/watering/schedule`, {
        params: { halfMonth }
      })
    );
  }
}
```

- [ ] **Step 2: Remove Today types from watering.api.ts**

Replace content of `garden-assistant-app/src/app/api/watering.api.ts`:

```typescript
export type DayOfWeekStr = 'Monday' | 'Tuesday' | 'Wednesday' | 'Thursday' | 'Friday' | 'Saturday' | 'Sunday';

export const WEEK_DAYS: DayOfWeekStr[] = ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday'];

export interface PlantWateringDto {
  plantId: string;
  plantName: string;
  waterNeeds: string;
  timesPerWeek: number;
  recommendedDays: DayOfWeekStr[];
  waterAmountMl?: number;
}

export interface BedWateringDto {
  bedId?: string;
  bedName: string;
  isPersonalPlants: boolean;
  soilType?: string;
  hasMulch: boolean;
  plants: PlantWateringDto[];
}

export interface WateringScheduleDto {
  beds: BedWateringDto[];
}
```

- [ ] **Step 3: Delete obsolete frontend files**

```bash
rm garden-assistant-app/src/app/shared/services/watering.store.ts
rm -rf garden-assistant-app/src/app/features/calendar/calendar-watering
rm -rf garden-assistant-app/src/app/features/calendar/calendar-watering-today
```

- [ ] **Step 4: Create GardenWatering component**

Create `garden-assistant-app/src/app/features/garden/garden-watering/garden-watering.ts`:

```typescript
import { Component, inject, input, output, signal, computed, effect } from '@angular/core';
import { NgClass } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { faDroplet } from '@fortawesome/free-solid-svg-icons';
import { FaIconComponent } from '@fortawesome/angular-fontawesome';
import { WateringService } from '../../../shared/services/watering.service';
import { WateringScheduleDto, WEEK_DAYS, DayOfWeekStr } from '../../../api/watering.api';
import { Collapsible } from '../../../shared/ui/collapsible/collapsible';
import { EmptyState } from '../../../shared/ui/empty-state/empty-state';

@Component({
  selector: 'app-garden-watering',
  standalone: true,
  imports: [TranslateModule, FaIconComponent, NgClass, Collapsible, EmptyState],
  templateUrl: './garden-watering.html',
  styleUrl: './garden-watering.scss'
})
export class GardenWatering {
  readonly gardenId = input.required<string>();
  readonly open = input<boolean | null>(null);
  readonly toggled = output<boolean>();

  private readonly wateringService = inject(WateringService);

  readonly scheduleData = signal<WateringScheduleDto | null>(null);
  readonly loading = signal(false);
  readonly beds = computed(() => this.scheduleData()?.beds ?? []);

  protected readonly faDroplet = faDroplet;
  protected readonly weekDays = WEEK_DAYS;
  protected readonly todayDayOfWeek = this.getTodayDayOfWeek();
  protected readonly weekDayHeaders = WEEK_DAYS.map((day, i) => {
    const date = new Date();
    date.setDate(date.getDate() - date.getDay() + 1 + i);
    return { day, number: date.getDate(), isToday: day === this.todayDayOfWeek };
  });

  private readonly hasLoaded = signal(false);

  constructor() {
    effect(() => {
      if (this.open() && !this.hasLoaded()) {
        this.hasLoaded.set(true);
        this.loadSchedule();
      }
    });
  }

  private async loadSchedule(): Promise<void> {
    this.loading.set(true);
    try {
      const halfMonth = this.getHalfMonth();
      this.scheduleData.set(await this.wateringService.getGardenSchedule(this.gardenId(), halfMonth));
    } finally {
      this.loading.set(false);
    }
  }

  private getHalfMonth(): number {
    const date = new Date();
    return date.getMonth() * 2 + (date.getDate() <= 15 ? 1 : 2);
  }

  private getTodayDayOfWeek(): DayOfWeekStr {
    return WEEK_DAYS[new Date().getDay() === 0 ? 6 : new Date().getDay() - 1];
  }

  hasDot(day: DayOfWeekStr, days: DayOfWeekStr[]): boolean {
    return days.includes(day);
  }

  waterNeedClass(waterNeeds: string): string {
    return `water-need-badge--${waterNeeds.toLowerCase()}`;
  }
}
```

- [ ] **Step 5: Create garden-watering.html**

Create `garden-assistant-app/src/app/features/garden/garden-watering/garden-watering.html`:

```html
<div class="panel">
  <app-collapsible [open]="open()" (toggled)="toggled.emit($event)">
    <div collapsible-header class="section-header">
      <span class="section-header-label">
        <fa-icon [icon]="faDroplet" style="color: var(--color-water)"></fa-icon>
        <span [translate]="'GardenWatering.Title'"></span>
      </span>
    </div>
    <div collapsible-body class="collapsible-body-padded">
      @if (loading()) {
        <app-empty-state icon="⏳" minHeight="100px"></app-empty-state>
      } @else if (beds().length === 0) {
        <app-empty-state messageKey="GardenWatering.EmptyState" minHeight="100px"></app-empty-state>
      } @else {
        @for (bed of beds(); track bed.bedId ?? bed.bedName; let first = $first) {
          <div [class.mt-4]="!first">
            <div class="section-divider-title">{{ bed.bedName }}</div>
            <div class="watering-grid-wrapper"
                 role="region"
                 [attr.aria-label]="'GardenWatering.GridAriaLabel' | translate">
              <div class="watering-grid" role="grid">
                <div class="watering-grid__head-row">
                  <div class="watering-grid__name-cell"></div>
                  @for (header of weekDayHeaders; track header.day) {
                    <div class="watering-grid__day-header"
                         [class.watering-grid__day-header--today]="header.isToday"
                         role="columnheader">
                      {{ header.day.substring(0, 1) }}<br>{{ header.number }}
                    </div>
                  }
                </div>
                @for (plant of bed.plants; track plant.plantId) {
                  <div class="watering-grid__row" role="row">
                    <div class="watering-grid__name-cell" role="rowheader">{{ plant.plantName }}</div>
                    @for (day of weekDays; track day) {
                      <div class="watering-grid__day-cell" role="gridcell">
                        @if (hasDot(day, plant.recommendedDays)) {
                          <div class="watering-dot-cell">
                            <div class="watering-dot"
                                 [attr.aria-label]="'Watering.DotAriaLabel' | translate"></div>
                            @if (plant.waterAmountMl) {
                              <span class="watering-amount-label">
                                {{ plant.waterAmountMl >= 1000
                                    ? ('Watering.WaterAmountL' | translate : { amount: (plant.waterAmountMl / 1000).toFixed(1) })
                                    : ('Watering.WaterAmountMl' | translate : { amount: plant.waterAmountMl }) }}
                              </span>
                            }
                          </div>
                        }
                      </div>
                    }
                  </div>
                }
              </div>
            </div>
          </div>
        }

        <div class="mt-6 px-4">
          <app-collapsible>
            <div collapsible-header>
              <div class="section-header">
                <span class="section-header-label" [translate]="'Watering.FrequenciesTitle'"></span>
              </div>
            </div>
            <div collapsible-body>
              <table class="watering-freq-table"
                     [attr.aria-label]="'Watering.FrequenciesTitle' | translate">
                <thead>
                  <tr>
                    <th [translate]="'Watering.FreqPlantCol'"></th>
                    <th [translate]="'Watering.FreqNeedCol'"></th>
                    <th [translate]="'Watering.FreqRateCol'"></th>
                  </tr>
                </thead>
                <tbody>
                  @for (bed of beds(); track bed.bedId ?? bed.bedName) {
                    @for (plant of bed.plants; track plant.plantId) {
                      <tr>
                        <td>{{ plant.plantName }}</td>
                        <td>
                          <span class="water-need-badge"
                                [ngClass]="waterNeedClass(plant.waterNeeds)">
                            {{ 'Watering.WaterNeed.' + plant.waterNeeds | translate }}
                          </span>
                        </td>
                        <td>{{ 'Watering.TimesPerWeek' | translate : { count: plant.timesPerWeek } }}</td>
                      </tr>
                    }
                  }
                </tbody>
              </table>
            </div>
          </app-collapsible>
        </div>
      }
    </div>
  </app-collapsible>
</div>
```

- [ ] **Step 6: Create garden-watering.scss (empty — styles live in _watering.scss)**

Create `garden-assistant-app/src/app/features/garden/garden-watering/garden-watering.scss`:

```scss
// Styles in src/styles/components/_watering.scss
```

- [ ] **Step 7: Build — expect no errors**

```bash
npm run build --prefix garden-assistant-app 2>&1 | tail -5
```

Expected: `Application bundle generation complete.`

- [ ] **Step 8: Commit**

```bash
git add garden-assistant-app/src/app/shared/services/watering.service.ts \
        garden-assistant-app/src/app/api/watering.api.ts \
        garden-assistant-app/src/app/features/garden/garden-watering/
git add -u garden-assistant-app/src/app/shared/services/watering.store.ts \
           garden-assistant-app/src/app/features/calendar/calendar-watering \
           garden-assistant-app/src/app/features/calendar/calendar-watering-today
git commit -m "feat: add GardenWatering component, simplify WateringService to garden-scoped"
```

---

### Task 6: Frontend — Wire GardenView

**Files:**
- Modify: `garden-assistant-app/src/app/features/garden/garden-view/garden-view.ts`
- Modify: `garden-assistant-app/src/app/features/garden/garden-view/garden-view.html`

- [ ] **Step 1: Add activePanel signal to GardenView**

In `garden-view.ts`:
- Add `GardenWatering` to imports array
- Add `readonly activePanel = signal<'calendar' | 'watering' | null>(null);`

Full updated import block and class opening:

```typescript
import { Component, inject, signal, computed, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { TranslateModule, TranslateService } from '@angular/core';
import { FontAwesomeModule } from '@fortawesome/angular-fontawesome';
import { faPlus, faPen, faArrowLeft, faTrash } from '@fortawesome/free-solid-svg-icons';
import { UpdateGardenRequest, CreateBedRequest, UpdateBedRequest } from '../../../api/garden-assistant-api';
import { GardenStore } from '../../../shared/services/garden.store';
import { DialogService } from '../../../shared/services/dialog.service';
import { GardenDialogService } from '../../../shared/services/garden-dialog.service';
import { BedPanel } from '../bed-panel/bed-panel';
import { GardenCalendar } from '../garden-calendar/garden-calendar';
import { GardenWatering } from '../garden-watering/garden-watering';
import { EmptyState } from '../../../shared/ui/empty-state/empty-state';
```

In the `@Component` decorator, add `GardenWatering` to `imports`:
```typescript
imports: [TranslateModule, FontAwesomeModule, BedPanel, GardenCalendar, GardenWatering, EmptyState],
```

In the class body, add after `readonly hasBeds`:
```typescript
readonly activePanel = signal<'calendar' | 'watering' | null>(null);
```

Note: `TranslateService` import comes from `'@ngx-translate/core'`, not `'@angular/core'`. Keep the existing import as-is.

- [ ] **Step 2: Update garden-view.html — replace the calendar block**

Replace this block at the end of garden-view.html:

```html
    <div style="margin-top: 2.5rem; display: block">
      <app-garden-calendar [beds]="beds()"></app-garden-calendar>
    </div>
```

With:

```html
    <div style="margin-top: 2rem">
      <app-garden-calendar
        [beds]="beds()"
        [open]="activePanel() === 'calendar'"
        (toggled)="activePanel.set($event ? 'calendar' : null)">
      </app-garden-calendar>
    </div>
    <div style="margin-top: 0.75rem">
      <app-garden-watering
        [gardenId]="gardenId()"
        [open]="activePanel() === 'watering'"
        (toggled)="activePanel.set($event ? 'watering' : null)">
      </app-garden-watering>
    </div>
```

- [ ] **Step 3: Build — expect no errors**

```bash
npm run build --prefix garden-assistant-app 2>&1 | tail -5
```

Expected: `Application bundle generation complete.`

- [ ] **Step 4: Commit**

```bash
git add garden-assistant-app/src/app/features/garden/garden-view/garden-view.ts \
        garden-assistant-app/src/app/features/garden/garden-view/garden-view.html
git commit -m "feat(garden-view): add watering panel with mutual-exclusive collapsible coordination"
```

---

### Task 7: Frontend — Clean up /calendar + i18n

**Files:**
- Modify: `garden-assistant-app/src/app/features/calendar/calendar.ts`
- Modify: `garden-assistant-app/src/app/features/calendar/calendar.html`
- Modify: `garden-assistant-app/public/i18n/fr.json`
- Modify: `garden-assistant-app/public/i18n/en.json`

- [ ] **Step 1: Clean calendar.ts**

Remove from `calendar.ts`:
- Import of `CalendarWateringToday`
- Import of `CalendarWatering`
- `faDroplet` from the fontawesome import
- `activeCalendarTab` signal
- `calendarTabOptions` array

The cleaned `@Component` imports array becomes:
```typescript
imports: [TranslateModule, PlantCalendarGantt, CalendarThisMonth, EmptyState, ToggleGroup],
```

The cleaned import lines (remove `faDroplet`, `CalendarWateringToday`, `CalendarWatering`):
```typescript
import { faHeart, faSeedling, faLayerGroup, faTableCellsLarge, faList } from '@fortawesome/free-solid-svg-icons';
```

Remove these lines from the class body:
```typescript
protected readonly activeCalendarTab = signal<'actions' | 'watering'>('actions');
protected readonly calendarTabOptions: ToggleOption[] = [
  { value: 'actions', labelKey: 'Calendar.TabActions', icon: faSeedling },
  { value: 'watering', labelKey: 'Calendar.TabWatering', icon: faDroplet },
];
```

- [ ] **Step 2: Clean calendar.html**

Remove from `calendar.html`:
- The `<app-calendar-watering-today>` element
- The entire `<div class="panel">` that contains the tab toggle and `@if (activeCalendarTab() === 'watering')` block, keeping only the content that was inside `@if (activeCalendarTab() === 'actions')` — but that content must be unwrapped from the if block and placed directly inside the panel.

The cleaned panel block (replacing lines 26–169 of the current calendar.html):

```html
    <div class="panel">
      <div class="calendar-filter-chips" style="display: flex; align-items: center; flex-wrap: wrap; gap: 0.5rem">
        @if (store.sourceFilter() === 'gardenPlants') {
          <app-toggle-group style="margin-right: 0.25rem"
            [options]="groupingOptions"
            [selectedValue]="store.grouping()"
            (valueChange)="onGroupingChange($event)">
          </app-toggle-group>
        }
        @for (filter of filters; track filter.key) {
          @if (availableFilterKeys().has(filter.key)) {
            <button class="calendar-chip"
                    [class.calendar-chip--active]="!hasActiveFilter() || isFilterActive(filter.key)"
                    [class.calendar-chip--inactive]="hasActiveFilter() && !isFilterActive(filter.key)"
                    [style.--chip-color]="filter.color"
                    (click)="toggleFilter(filter.key)">
              <span class="calendar-chip-dot" [style.background-color]="(!hasActiveFilter() || isFilterActive(filter.key)) ? filter.color : '#9ca3af'"></span>
              <span class="calendar-chip-label">{{ filter.labelKey | translate }}</span>
            </button>
          }
        }
      </div>

      @if (store.grouping() === 'byBed') {
        @for (gardenGroup of store.gardenBedGroups(); track gardenGroup.gardenName; let firstGarden = $first) {
          <div [style.margin-top]="firstGarden ? '0' : '1.5rem'">
            <div class="section-divider-title" style="padding: 0 1rem">{{ gardenGroup.gardenName }}</div>
            @for (bed of gardenGroup.beds; track bed.bedName) {
              <div style="margin-bottom: 0.75rem">
                <div class="calendar-bed-title">{{ bed.bedName }}</div>
                <div class="calendar-month-header-row">
                  <div class="calendar-name-spacer"></div>
                  <div class="calendar-month-headers">
                    @for (label of monthLabels; track $index) {
                      <div class="calendar-month-header"
                           [class.calendar-month-header--current]="$index === currentMonthIndex">
                        {{ label }}
                      </div>
                    }
                  </div>
                </div>
                @for (plant of bed.plants; track plant.plantId; let last = $last; let idx = $index) {
                  @let plantInfo = plantStore.findById(plant.plantId);
                  @if (plantInfo) {
                    <div class="calendar-plant-section" [class.calendar-plant-section--last]="last">
                      <app-plant-calendar-gantt
                        [actions]="plant.actions ?? []"
                        [propagationMethod]="plantInfo.propagationMethod!"
                        [frostSensitive]="plantInfo.frostSensitive ?? false"
                        [showHeader]="false"
                        [activeFilters]="ganttFilters()"
                        [plantName]="plantInfo.name!"
                        [oddPlant]="idx % 2 === 1"
                        (plantNameClick)="openPlantDetail(plantInfo)"
                        (harvestReadinessClick)="openHarvestReadiness(plant.plantId!)">
                      </app-plant-calendar-gantt>
                    </div>
                  }
                }
              </div>
            }
          </div>
        }
      } @else if (store.grouping() === 'byGarden') {
        @for (group of store.gardenGroups(); track group.gardenName; let first = $first) {
          <div style="margin-bottom: 1rem" [style.margin-top]="first ? '0' : '1.5rem'">
            <div class="section-divider-title" style="padding: 0 1rem">{{ group.gardenName }}</div>
            <div class="calendar-month-header-row">
              <div class="calendar-name-spacer"></div>
              <div class="calendar-month-headers">
                @for (label of monthLabels; track $index) {
                  <div class="calendar-month-header"
                       [class.calendar-month-header--current]="$index === currentMonthIndex">
                    {{ label }}
                  </div>
                }
              </div>
            </div>
            @for (plant of group.plants; track plant.plantId; let last = $last; let idx = $index) {
              @let plantInfo = plantStore.findById(plant.plantId);
              @if (plantInfo) {
                <div class="calendar-plant-section" [class.calendar-plant-section--last]="last">
                  <app-plant-calendar-gantt
                    [actions]="plant.actions ?? []"
                    [propagationMethod]="plantInfo.propagationMethod!"
                    [frostSensitive]="plantInfo.frostSensitive ?? false"
                    [showHeader]="false"
                    [activeFilters]="ganttFilters()"
                    [plantName]="plantInfo.name!"
                    [oddPlant]="idx % 2 === 1"
                    (plantNameClick)="openPlantDetail(plantInfo)"
                    (harvestReadinessClick)="openHarvestReadiness(plant.plantId!)">
                  </app-plant-calendar-gantt>
                </div>
              }
            }
          </div>
        }
      } @else {
        <div class="calendar-month-header-row">
          <div class="calendar-name-spacer"></div>
          <div class="calendar-label-spacer"></div>
          <div class="calendar-month-headers">
            @for (label of monthLabels; track $index) {
              <div class="calendar-month-header"
                   [class.calendar-month-header--current]="$index === currentMonthIndex">
                {{ label }}
              </div>
            }
          </div>
        </div>
        @for (plant of store.filteredPlants(); track plant.plantId; let last = $last; let idx = $index) {
          @let plantInfo = plantStore.findById(plant.plantId);
          @if (plantInfo) {
            <div class="calendar-plant-section" [class.calendar-plant-section--last]="last">
              <app-plant-calendar-gantt
                [actions]="plant.actions ?? []"
                [propagationMethod]="plantInfo.propagationMethod!"
                [frostSensitive]="plantInfo.frostSensitive ?? false"
                [showHeader]="false"
                [activeFilters]="ganttFilters()"
                [plantName]="plantInfo.name!"
                [oddPlant]="idx % 2 === 1"
                [hasHarvestReadiness]="!!plantInfo.harvestReadiness"
                (plantNameClick)="openPlantDetail(plantInfo)"
                (harvestReadinessClick)="openHarvestReadiness(plant.plantId!)">
              </app-plant-calendar-gantt>
            </div>
          }
        }
      }
    </div>
```

- [ ] **Step 3: Update fr.json**

In the `Calendar` section, remove `TabActions` and `TabWatering` (the two lines added in the previous commit).

In the `Watering` section, remove these keys: `TodayLabel`, `NextWatering`, `PlantBadgeAriaLabel`, `TabActions`, `TabWatering`, `WeekCurrent`, `WeekNext`, `PrevWeek`, `NextWeek`, `GridAriaLabel`, `EmptyState`, `DailyTotalL`, `MyPlantsBed`, and the entire `Day` sub-object.

Add a new `GardenWatering` section after the `GardenCalendar` section:

```json
  "GardenWatering": {
    "Title": "Calendrier d'arrosage",
    "EmptyState": "Aucune planche avec des plantes à arroser.",
    "GridAriaLabel": "Grille d'arrosage"
  },
```

- [ ] **Step 4: Update en.json**

Remove the `Calendar` section entirely (it only contained `TabActions` and `TabWatering`).

In the `Watering` section, remove the same keys as fr.json: `TodayLabel`, `NextWatering`, `PlantBadgeAriaLabel`, `TabActions`, `TabWatering`, `WeekCurrent`, `WeekNext`, `PrevWeek`, `NextWeek`, `GridAriaLabel`, `EmptyState`, `DailyTotalL`, `MyPlantsBed`, `Day`.

Add the `GardenWatering` section:

```json
  "GardenWatering": {
    "Title": "Watering calendar",
    "EmptyState": "No beds with plants to water.",
    "GridAriaLabel": "Watering grid"
  },
```

- [ ] **Step 5: Build — expect no errors**

```bash
npm run build --prefix garden-assistant-app 2>&1 | tail -5
```

Expected: `Application bundle generation complete.`

- [ ] **Step 6: Run all backend tests**

```bash
dotnet test garden-assistant-tests 2>&1 | tail -5
```

Expected: all tests pass.

- [ ] **Step 7: Commit**

```bash
git add garden-assistant-app/src/app/features/calendar/calendar.ts \
        garden-assistant-app/src/app/features/calendar/calendar.html \
        garden-assistant-app/public/i18n/fr.json \
        garden-assistant-app/public/i18n/en.json
git commit -m "refactor(calendar): remove watering — now lives in garden view"
```
