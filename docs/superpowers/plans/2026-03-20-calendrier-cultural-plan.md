# Calendrier Cultural Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Implement the plant calendar feature (E10) — data model, seed data, API endpoints, and frontend calendar page with hybrid compact/Gantt view + harvest readiness indicators.

**Architecture:** Backend entities (`PlantAction`, `HarvestReadiness`, `HarvestReadinessCriterion`) with EF Core code-first, services behind interfaces, thin controllers. Frontend uses Angular signals, standalone components, reusable calendar bar/Gantt components displayed on both the calendar page and plant detail dialog.

**Tech Stack:** .NET 10 / EF Core / PostgreSQL, Angular 21 / Signals / Tailwind CSS v4 / ngx-translate, xUnit / Shouldly

**Spec:** `docs/superpowers/specs/2026-03-20-calendrier-cultural-design.md`

---

## File Structure

### Backend — New Files
- `Data/Entities/Enums/PlantActionType.cs` — 8-value enum
- `Data/Entities/Enums/PropagationMethod.cs` — 4-value enum
- `Data/Entities/Enums/HarvestCriterionType.cs` — 4-value enum
- `Data/Entities/PlantAction.cs` — calendar action entity
- `Data/Entities/HarvestReadiness.cs` — harvest readiness entity
- `Data/Entities/HarvestReadinessCriterion.cs` — criterion entity
- `Data/Seeds/plant-actions.json` — seed data for all plants
- `Data/Seeds/harvest-readiness.json` — seed data for harvest indicators
- `Data/Seeders/PlantActionSeeder.cs` — seeder for plant actions
- `Data/Seeders/HarvestReadinessSeeder.cs` — seeder for harvest readiness
- `DTOs/Plants/PlantActionDto.cs` — action DTO
- `DTOs/Plants/HarvestReadinessDto.cs` — harvest readiness DTO
- `DTOs/Plants/HarvestReadinessCriterionDto.cs` — criterion DTO
- `DTOs/Calendar/CalendarDto.cs` — batch calendar DTO
- `DTOs/Calendar/CalendarPlantDto.cs` — plant with actions for calendar
- `Services/Interfaces/IPlantActionService.cs` — service interface
- `Services/Interfaces/IHarvestReadinessService.cs` — service interface
- `Services/PlantActionService.cs` — service implementation
- `Services/HarvestReadinessService.cs` — service implementation
- `Controllers/CalendarController.cs` — batch calendar endpoint

### Backend — Modified Files
- `Data/Entities/Plant.cs` — add `PropagationMethod`, `FrostSensitive`, navigation properties
- `Data/AppDbContext.cs` — add DbSets + Fluent API config
- `DTOs/Plants/PlantDto.cs` — add `PropagationMethod`, `FrostSensitive`
- `Services/PlantService.cs` — update DTO mapping
- `Controllers/PlantsController.cs` — add `GET {id}/actions` and `GET {id}/harvest-readiness`
- `ServiceCollectionExtensions.cs` — register new services + seeders

### Backend — Test Files
- `garden-assistant-tests/PlantActions/PlantActionServiceTests.cs`
- `garden-assistant-tests/PlantActions/HarvestReadinessServiceTests.cs`
- `garden-assistant-tests/PlantActions/CalendarControllerTests.cs`

### Frontend — New Files
- `src/app/features/calendar/calendar.ts` — calendar page component
- `src/app/features/calendar/calendar.html` — calendar page template
- `src/app/features/calendar/calendar.scss` — calendar page styles
- `src/app/features/calendar/calendar-this-month.ts` — this month widget
- `src/app/features/calendar/calendar-this-month.html` — widget template
- `src/app/shared/ui/plant-calendar-bar/plant-calendar-bar.ts` — compact bar component
- `src/app/shared/ui/plant-calendar-bar/plant-calendar-bar.html` — bar template
- `src/app/shared/ui/plant-calendar-gantt/plant-calendar-gantt.ts` — Gantt detail component
- `src/app/shared/ui/plant-calendar-gantt/plant-calendar-gantt.html` — Gantt template
- `src/app/shared/ui/harvest-readiness/harvest-readiness.ts` — harvest readiness component
- `src/app/shared/ui/harvest-readiness/harvest-readiness.html` — harvest readiness template
- `src/app/shared/services/calendar.store.ts` — calendar signal store
- `src/app/shared/services/calendar.client.ts` — API client for calendar

### Frontend — Modified Files
- `src/app/app.routes.ts` — add `/calendrier` route
- `src/app/shared/ui/plant-detail-dialog/plant-detail-dialog.ts` — add calendar + harvest readiness sections
- `src/app/shared/ui/plant-detail-dialog/plant-detail-dialog.html` — add sections
- `src/app/shared/ui/badge-info-dialog/badge-info-dialog.ts` — support action type info (if needed)
- `public/i18n/fr.json` — add Calendar.*, HarvestReadiness.*, BadgeInfo.Action.* keys

---

## Task 1: Backend enums and Plant entity update (US-059 partial)

**Files:**
- Create: `garden-assistant-api/Data/Entities/Enums/PlantActionType.cs`
- Create: `garden-assistant-api/Data/Entities/Enums/PropagationMethod.cs`
- Create: `garden-assistant-api/Data/Entities/Enums/HarvestCriterionType.cs`
- Modify: `garden-assistant-api/Data/Entities/Plant.cs`
- Modify: `garden-assistant-api/DTOs/Plants/PlantDto.cs`
- Modify: `garden-assistant-api/Services/PlantService.cs`

- [ ] **Step 1: Create PlantActionType enum**

```csharp
// Data/Entities/Enums/PlantActionType.cs
namespace GardenAssistant.Data.Entities.Enums;

public enum PlantActionType
{
    IndoorSowing,
    DirectSowing,
    Transplanting,
    Harvest,
    Pruning,
    Pinching,
    Hilling,
    Division
}
```

- [ ] **Step 2: Create PropagationMethod enum**

```csharp
// Data/Entities/Enums/PropagationMethod.cs
namespace GardenAssistant.Data.Entities.Enums;

public enum PropagationMethod
{
    Seed,
    Bulb,
    Tuber,
    Division
}
```

- [ ] **Step 3: Create HarvestCriterionType enum**

```csharp
// Data/Entities/Enums/HarvestCriterionType.cs
namespace GardenAssistant.Data.Entities.Enums;

public enum HarvestCriterionType
{
    Visual,
    Touch,
    Timing,
    Technique
}
```

- [ ] **Step 4: Update Plant entity — add PropagationMethod, FrostSensitive, navigation**

Add to `Plant.cs`:
```csharp
public PropagationMethod PropagationMethod { get; set; }
public bool FrostSensitive { get; set; }
public List<PlantAction> Actions { get; set; } = [];
public HarvestReadiness? HarvestReadiness { get; set; }
```

- [ ] **Step 5: Update PlantDto — add new fields**

Update the record to include `PropagationMethod` and `FrostSensitive` fields.

- [ ] **Step 6: Update PlantService — map new fields in query**

Update the `Select()` in `GetAllAsync()` to include the two new fields.

- [ ] **Step 7: Build and verify**

Run: `dotnet build garden-assistant-api/garden-assistant-api.csproj`

- [ ] **Step 8: Commit**

```
feat(E10): add PlantActionType, PropagationMethod, HarvestCriterionType enums and update Plant entity
```

---

## Task 2: PlantAction and HarvestReadiness entities (US-059 + US-076 partial)

**Files:**
- Create: `garden-assistant-api/Data/Entities/PlantAction.cs`
- Create: `garden-assistant-api/Data/Entities/HarvestReadiness.cs`
- Create: `garden-assistant-api/Data/Entities/HarvestReadinessCriterion.cs`
- Modify: `garden-assistant-api/Data/AppDbContext.cs`

- [ ] **Step 1: Create PlantAction entity**

```csharp
// Data/Entities/PlantAction.cs
using GardenAssistant.Data.Entities.Enums;

namespace GardenAssistant.Data.Entities;

public class PlantAction
{
    public Guid Id { get; set; }
    public Guid PlantId { get; set; }
    public PlantActionType ActionType { get; set; }
    public int HalfMonthStart { get; set; }
    public int HalfMonthEnd { get; set; }
    public string? Notes { get; set; }
    public Plant Plant { get; set; } = null!;
}
```

- [ ] **Step 2: Create HarvestReadiness entity**

```csharp
// Data/Entities/HarvestReadiness.cs
namespace GardenAssistant.Data.Entities;

public class HarvestReadiness
{
    public Guid Id { get; set; }
    public Guid PlantId { get; set; }
    public required string Description { get; set; }
    public int? DaysFromTransplant { get; set; }
    public int? DaysFromSowing { get; set; }
    public Plant Plant { get; set; } = null!;
    public List<HarvestReadinessCriterion> Criteria { get; set; } = [];
}
```

- [ ] **Step 3: Create HarvestReadinessCriterion entity**

```csharp
// Data/Entities/HarvestReadinessCriterion.cs
using GardenAssistant.Data.Entities.Enums;

namespace GardenAssistant.Data.Entities;

public class HarvestReadinessCriterion
{
    public Guid Id { get; set; }
    public Guid HarvestReadinessId { get; set; }
    public HarvestCriterionType CriterionType { get; set; }
    public required string Description { get; set; }
    public HarvestReadiness HarvestReadiness { get; set; } = null!;
}
```

- [ ] **Step 4: Update AppDbContext — add DbSets**

Add DbSet properties:
```csharp
public DbSet<PlantAction> PlantActions => Set<PlantAction>();
public DbSet<HarvestReadiness> HarvestReadiness => Set<HarvestReadiness>();
public DbSet<HarvestReadinessCriterion> HarvestReadinessCriteria => Set<HarvestReadinessCriterion>();
```

- [ ] **Step 5: Update AppDbContext — add Fluent API config in OnModelCreating**

Add configuration for PlantAction:
- HasKey on Id
- Property Notes HasMaxLength(1000)
- HasOne(Plant).WithMany(Actions).HasForeignKey(PlantId).OnDelete(Cascade)

Add configuration for HarvestReadiness:
- HasKey on Id
- Property Description IsRequired HasMaxLength(2000)
- HasOne(Plant).WithOne(HarvestReadiness).HasForeignKey(PlantId).OnDelete(Cascade)
- HasIndex on PlantId IsUnique

Add configuration for HarvestReadinessCriterion:
- HasKey on Id
- Property Description IsRequired HasMaxLength(1000)
- HasOne(HarvestReadiness).WithMany(Criteria).HasForeignKey(HarvestReadinessId).OnDelete(Cascade)

- [ ] **Step 6: Create EF migration**

Run: `dotnet ef migrations add AddPlantCalendarEntities --project garden-assistant-api`

- [ ] **Step 7: Build and verify migration**

Run: `dotnet build garden-assistant-api/garden-assistant-api.csproj`

- [ ] **Step 8: Commit**

```
feat(E10): add PlantAction, HarvestReadiness, HarvestReadinessCriterion entities and migration
```

---

## Task 3: Update plants.json seed data with PropagationMethod and FrostSensitive (US-059 partial)

**Files:**
- Modify: `garden-assistant-api/Data/Seeds/plants.json`
- Modify: `garden-assistant-api/Data/Seeders/PlantSeeder.cs`

- [ ] **Step 1: Update plants.json — add propagationMethod and frostSensitive to every plant entry**

Add `"propagationMethod": "Seed"` (default for most), `"frostSensitive": false` (default) to all entries. Override for:
- `"propagationMethod": "Bulb"` for Ail, Oignon, Echalote
- `"propagationMethod": "Tuber"` for Pomme de terre
- `"propagationMethod": "Division"` for Consoude, Menthe, Ciboulette
- `"frostSensitive": true` for Tomate, Poivron, Aubergine, Courgette, Courge, Concombre, Haricot, Basilic, Mais

- [ ] **Step 2: Update PlantSeeder — map new fields from seed record**

Add `PropagationMethod` and `FrostSensitive` to the `PlantSeedRecord` and mapping logic.

- [ ] **Step 3: Build and verify**

Run: `dotnet build garden-assistant-api/garden-assistant-api.csproj`

- [ ] **Step 4: Commit**

```
feat(E10): add propagationMethod and frostSensitive to plant seed data
```

---

## Task 4: Plant actions seed data (US-059 partial)

**Files:**
- Create: `garden-assistant-api/Data/Seeds/plant-actions.json`
- Create: `garden-assistant-api/Data/Seeders/PlantActionSeeder.cs`
- Modify: `garden-assistant-api/ServiceCollectionExtensions.cs`

- [ ] **Step 1: Create plant-actions.json with all plant calendar data**

JSON format — array of objects:
```json
[
  {
    "plantKey": "tomate",
    "actions": [
      { "actionType": "IndoorSowing", "halfMonthStart": 3, "halfMonthEnd": 6, "notes": "Semer en godets a 20°C..." },
      { "actionType": "Transplanting", "halfMonthStart": 10, "halfMonthEnd": 10, "notes": "Apres les Saints de Glace..." },
      { "actionType": "Harvest", "halfMonthStart": 13, "halfMonthEnd": 20 },
      { "actionType": "Pruning", "halfMonthStart": 11, "halfMonthEnd": 18, "notes": "Supprimer les gourmands..." }
    ]
  }
]
```

Include data for ALL plants in the database. Use the plant expert's data adapted for Swiss plateau climate. Half-month encoding: 1=debut jan, 2=mi-jan, 3=debut fev, ..., 24=mi-dec.

The `plant-expert` agent should be consulted to produce the complete data for all plants.

- [ ] **Step 2: Create PlantActionSeeder**

Follow the PlantSeeder pattern:
- Implements `ISeeder`
- Constructor injection of `AppDbContext` and `IWebHostEnvironment`
- Idempotent: `if (await db.PlantActions.AnyAsync()) return;`
- Deserialize JSON, look up PlantId by matching plant key/name
- Create PlantAction entities with `Guid.NewGuid()`

- [ ] **Step 3: Register seeder in ServiceCollectionExtensions**

Add: `services.AddScoped<ISeeder, PlantActionSeeder>();`

- [ ] **Step 4: Build and verify**

Run: `dotnet build garden-assistant-api/garden-assistant-api.csproj`

- [ ] **Step 5: Commit**

```
feat(E10): add plant actions seed data for all plants (Swiss plateau climate)
```

---

## Task 5: Harvest readiness seed data (US-076 partial)

**Files:**
- Create: `garden-assistant-api/Data/Seeds/harvest-readiness.json`
- Create: `garden-assistant-api/Data/Seeders/HarvestReadinessSeeder.cs`
- Modify: `garden-assistant-api/ServiceCollectionExtensions.cs`

- [ ] **Step 1: Create harvest-readiness.json**

JSON format:
```json
[
  {
    "plantKey": "tomate",
    "description": "La tomate est prete quand elle atteint une couleur uniforme et cede legerement sous la pression.",
    "daysFromTransplant": 70,
    "criteria": [
      { "criterionType": "Visual", "description": "Couleur uniforme, pas d'epaule verte." },
      { "criterionType": "Touch", "description": "Legere souplesse sous la pression, peau lisse et brillante." },
      { "criterionType": "Timing", "description": "60-85 jours apres repiquage selon la variete." },
      { "criterionType": "Technique", "description": "Le fruit se detache facilement en le soulevant et tournant doucement." }
    ]
  }
]
```

Include data for all edible plants (skip ornamentals like tagete, bourrache unless they have edible parts). Use the plant expert's data.

- [ ] **Step 2: Create HarvestReadinessSeeder**

Same pattern as PlantActionSeeder. Idempotent, JSON deserialization, plant key lookup.

- [ ] **Step 3: Register seeder**

Add: `services.AddScoped<ISeeder, HarvestReadinessSeeder>();`

- [ ] **Step 4: Build and verify**

Run: `dotnet build garden-assistant-api/garden-assistant-api.csproj`

- [ ] **Step 5: Commit**

```
feat(E10): add harvest readiness seed data for all edible plants
```

---

## Task 6: Backend services — PlantActionService and HarvestReadinessService (US-059 + US-076)

**Files:**
- Create: `garden-assistant-api/Services/Interfaces/IPlantActionService.cs`
- Create: `garden-assistant-api/Services/Interfaces/IHarvestReadinessService.cs`
- Create: `garden-assistant-api/Services/PlantActionService.cs`
- Create: `garden-assistant-api/Services/HarvestReadinessService.cs`
- Create: `garden-assistant-api/DTOs/Plants/PlantActionDto.cs`
- Create: `garden-assistant-api/DTOs/Plants/HarvestReadinessDto.cs`
- Create: `garden-assistant-api/DTOs/Plants/HarvestReadinessCriterionDto.cs`
- Modify: `garden-assistant-api/ServiceCollectionExtensions.cs`

- [ ] **Step 1: Write PlantActionService tests**

Create `garden-assistant-tests/PlantActions/PlantActionServiceTests.cs`:
- `GetByPlantIdAsync_WhenPlantHasNoActions_ShouldReturnEmpty`
- `GetByPlantIdAsync_WhenPlantHasActions_ShouldReturnAllActions`
- `GetByPlantIdAsync_WhenPlantDoesNotExist_ShouldReturnEmpty`

- [ ] **Step 2: Run tests to verify they fail**

Run: `dotnet test garden-assistant-tests --filter "PlantActionServiceTests"`

- [ ] **Step 3: Create DTOs**

```csharp
// DTOs/Plants/PlantActionDto.cs
public record PlantActionDto(
    Guid Id,
    PlantActionType ActionType,
    int HalfMonthStart,
    int HalfMonthEnd,
    string? Notes
);
```

```csharp
// DTOs/Plants/HarvestReadinessDto.cs
public record HarvestReadinessDto(
    string Description,
    int? DaysFromTransplant,
    int? DaysFromSowing,
    List<HarvestReadinessCriterionDto> Criteria
);
```

```csharp
// DTOs/Plants/HarvestReadinessCriterionDto.cs
public record HarvestReadinessCriterionDto(
    HarvestCriterionType CriterionType,
    string Description
);
```

- [ ] **Step 4: Create IPlantActionService + PlantActionService**

```csharp
public interface IPlantActionService
{
    Task<List<PlantActionDto>> GetByPlantIdAsync(Guid plantId);
}
```

Implementation: query `PlantActions.Where(pa => pa.PlantId == plantId)`, select to DTO, order by ActionType then HalfMonthStart.

- [ ] **Step 5: Run PlantActionService tests — verify pass**

- [ ] **Step 6: Write HarvestReadinessService tests**

Create `garden-assistant-tests/PlantActions/HarvestReadinessServiceTests.cs`:
- `GetByPlantIdAsync_WhenPlantHasNoReadiness_ShouldReturnNull`
- `GetByPlantIdAsync_WhenPlantHasReadiness_ShouldReturnWithCriteria`

- [ ] **Step 7: Create IHarvestReadinessService + HarvestReadinessService**

Implementation: query `HarvestReadiness.Include(Criteria).FirstOrDefault(hr => hr.PlantId == plantId)`, map to DTO, order criteria by CriterionType.

- [ ] **Step 8: Run all tests — verify pass**

Run: `dotnet test garden-assistant-tests`

- [ ] **Step 9: Register services in DI**

- [ ] **Step 10: Commit**

```
feat(E10): add PlantActionService and HarvestReadinessService with tests
```

---

## Task 7: API endpoints — PlantsController + CalendarController (US-059 + US-076 + US-080)

**Files:**
- Modify: `garden-assistant-api/Controllers/PlantsController.cs`
- Create: `garden-assistant-api/Controllers/CalendarController.cs`
- Create: `garden-assistant-api/DTOs/Calendar/CalendarDto.cs`
- Create: `garden-assistant-api/DTOs/Calendar/CalendarPlantDto.cs`

- [ ] **Step 1: Add endpoints to PlantsController**

```csharp
[HttpGet("{id:guid}/actions")]
[ProducesResponseType(typeof(List<PlantActionDto>), StatusCodes.Status200OK)]
public async Task<IActionResult> GetActions(Guid id) =>
    Ok(await plantActionService.GetByPlantIdAsync(id));

[HttpGet("{id:guid}/harvest-readiness")]
[ProducesResponseType(typeof(HarvestReadinessDto), StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
public async Task<IActionResult> GetHarvestReadiness(Guid id)
{
    var result = await harvestReadinessService.GetByPlantIdAsync(id);
    return result is null ? NotFound() : Ok(result);
}
```

- [ ] **Step 2: Create Calendar DTOs**

```csharp
// DTOs/Calendar/CalendarPlantDto.cs
public record CalendarPlantDto(
    Guid Id,
    string Name,
    PropagationMethod PropagationMethod,
    bool FrostSensitive,
    List<PlantActionDto> Actions
);

// DTOs/Calendar/CalendarDto.cs
public record CalendarDto(List<CalendarPlantDto> Plants);
```

- [ ] **Step 3: Create CalendarController with batch endpoint**

```csharp
[ApiController]
[Authorize]
[Route("api/calendar")]
public class CalendarController(
    IUserPlantService userPlantService,
    IPlantActionService plantActionService) : ControllerBase
{
    [HttpGet("my-plants")]
    [ProducesResponseType(typeof(CalendarDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyPlantsCalendar()
    {
        // Get user ID from claims
        // Get user's plants
        // For each plant, get actions
        // Return CalendarDto
    }
}
```

- [ ] **Step 4: Build and verify**

Run: `dotnet build garden-assistant-api/garden-assistant-api.csproj`

- [ ] **Step 5: Commit**

```
feat(E10): add calendar API endpoints (plant actions, harvest readiness, batch)
```

---

## Task 8: Frontend — calendar store and API client (US-060 prep)

**Files:**
- Create: `garden-assistant-app/src/app/shared/services/calendar.client.ts`
- Create: `garden-assistant-app/src/app/shared/services/calendar.store.ts`

- [ ] **Step 1: Create CalendarClient — manual API client**

Since the NSwag-generated client won't have the new endpoints yet, create a manual client:
- `getMyPlantsCalendar(): Promise<CalendarDto>`
- `getPlantActions(plantId: string): Promise<PlantActionDto[]>`
- `getHarvestReadiness(plantId: string): Promise<HarvestReadinessDto | null>`

Use the same fetch-based pattern as the generated client, with the app's base URL and auth headers.

- [ ] **Step 2: Create CalendarStore with signals**

```typescript
@Injectable({ providedIn: 'root' })
export class CalendarStore {
  readonly calendarData = signal<CalendarDto | null>(null);
  readonly loading = signal(false);
  readonly expandedPlantId = signal<string | null>(null);
  readonly activeFilters = signal<PlantActionType[]>(ALL_ACTION_TYPES);

  async loadCalendar(): Promise<void> { ... }
  togglePlantExpanded(plantId: string): void { ... }
  toggleFilter(actionType: PlantActionType): void { ... }
  get currentMonthActions(): ... { ... }
}
```

- [ ] **Step 3: Build frontend**

Run: `npm run build --prefix garden-assistant-app`

- [ ] **Step 4: Commit**

```
feat(E10): add calendar API client and signal store
```

---

## Task 9: Frontend — PlantCalendarBar + PlantCalendarGantt components (US-060 core)

**Files:**
- Create: `garden-assistant-app/src/app/shared/ui/plant-calendar-bar/plant-calendar-bar.ts`
- Create: `garden-assistant-app/src/app/shared/ui/plant-calendar-bar/plant-calendar-bar.html`
- Create: `garden-assistant-app/src/app/shared/ui/plant-calendar-gantt/plant-calendar-gantt.ts`
- Create: `garden-assistant-app/src/app/shared/ui/plant-calendar-gantt/plant-calendar-gantt.html`

- [ ] **Step 1: Create PlantCalendarBarComponent**

Standalone component with inputs:
- `actions: PlantActionDto[]`
- `frostSensitive: boolean`

Renders a compact row of 24 half-month cells with colored bars stacked per action type. Frost indicator (snowflake icon) on half-months 1-10 (before mid-May) for frost-sensitive plants on Transplanting/DirectSowing actions.

- [ ] **Step 2: Create PlantCalendarGanttComponent**

Standalone component with inputs:
- `actions: PlantActionDto[]`
- `propagationMethod: PropagationMethod`
- `frostSensitive: boolean`

Renders one row per action type with horizontal bars spanning the half-month range. Labels adapt based on propagationMethod ("Plantation" vs "Semis"). Highlight current half-month.

- [ ] **Step 3: Build frontend**

Run: `npm run build --prefix garden-assistant-app`

- [ ] **Step 4: Commit**

```
feat(E10): add PlantCalendarBar and PlantCalendarGantt reusable components
```

---

## Task 10: Frontend — Calendar page (US-060)

**Files:**
- Create: `garden-assistant-app/src/app/features/calendar/calendar.ts`
- Create: `garden-assistant-app/src/app/features/calendar/calendar.html`
- Create: `garden-assistant-app/src/app/features/calendar/calendar.scss`
- Modify: `garden-assistant-app/src/app/app.routes.ts`
- Modify: `public/i18n/fr.json`

- [ ] **Step 1: Create CalendarPage component**

Standalone component that:
- Loads calendar data via CalendarStore on init
- Displays month headers (Jan-Dec, each split into 2 half-month columns)
- Lists plants with PlantCalendarBarComponent for each
- Click on plant row toggles expanded Gantt view below
- Highlights current month column
- Empty state if no plants

- [ ] **Step 2: Add route `/calendrier`**

Add to `app.routes.ts`:
```typescript
{
  path: 'calendrier',
  loadComponent: () => import('./features/calendar/calendar').then(m => m.Calendar)
}
```

- [ ] **Step 3: Add navigation link**

Add "Calendrier" to the app header navigation (same pattern as existing links).

- [ ] **Step 4: Add translation keys**

Add to `fr.json`: `Calendar.Title`, `Calendar.EmptyState`, `Calendar.MonthJan`-`Calendar.MonthDec`, `Calendar.ActionType.*` for all 8 types, `Calendar.FrostWarning`.

- [ ] **Step 5: Build frontend**

Run: `npm run build --prefix garden-assistant-app`

- [ ] **Step 6: Commit**

```
feat(E10): add calendar page with hybrid compact/Gantt view
```

---

## Task 11: Frontend — Calendar in plant detail dialog (US-078)

**Files:**
- Modify: `garden-assistant-app/src/app/shared/ui/plant-detail-dialog/plant-detail-dialog.ts`
- Modify: `garden-assistant-app/src/app/shared/ui/plant-detail-dialog/plant-detail-dialog.html`

- [ ] **Step 1: Add calendar section to plant detail dialog**

Add the `PlantCalendarGanttComponent` in the dialog template, after the existing plant info. Load plant actions via CalendarClient when dialog opens. Show a loading state while fetching.

- [ ] **Step 2: Add translation keys**

- [ ] **Step 3: Build frontend**

Run: `npm run build --prefix garden-assistant-app`

- [ ] **Step 4: Commit**

```
feat(E10): add calendar section to plant detail dialog
```

---

## Task 12: Frontend — Harvest readiness section in plant detail (US-077)

**Files:**
- Create: `garden-assistant-app/src/app/shared/ui/harvest-readiness/harvest-readiness.ts`
- Create: `garden-assistant-app/src/app/shared/ui/harvest-readiness/harvest-readiness.html`
- Modify: `garden-assistant-app/src/app/shared/ui/plant-detail-dialog/plant-detail-dialog.ts`
- Modify: `garden-assistant-app/src/app/shared/ui/plant-detail-dialog/plant-detail-dialog.html`
- Modify: `public/i18n/fr.json`

- [ ] **Step 1: Create HarvestReadinessComponent**

Standalone component with input `readiness: HarvestReadinessDto | null`.
Displays:
- Description text
- Badge with days from transplant/sowing if available
- Criteria list grouped by type with icons: fa-eye (Visual), fa-hand (Touch), fa-clock (Timing), fa-screwdriver-wrench (Technique)

- [ ] **Step 2: Integrate in plant detail dialog**

Load harvest readiness data via CalendarClient. Add the component below the calendar section. Hide if null.

- [ ] **Step 3: Add translation keys**

`HarvestReadiness.Title`, `HarvestReadiness.DaysFromTransplant`, `HarvestReadiness.DaysFromSowing`, `HarvestReadiness.CriterionType.*`

- [ ] **Step 4: Build frontend**

Run: `npm run build --prefix garden-assistant-app`

- [ ] **Step 5: Commit**

```
feat(E10): add harvest readiness section to plant detail dialog
```

---

## Task 13: Frontend — This month widget (US-061)

**Files:**
- Create: `garden-assistant-app/src/app/features/calendar/calendar-this-month.ts`
- Create: `garden-assistant-app/src/app/features/calendar/calendar-this-month.html`
- Modify: `garden-assistant-app/src/app/features/calendar/calendar.html`
- Modify: `public/i18n/fr.json`

- [ ] **Step 1: Create CalendarThisMonthComponent**

Standalone component that filters actions from CalendarStore data for the current half-month. Groups by action type, lists plant names. Click on plant name opens plant detail dialog.

- [ ] **Step 2: Integrate at top of calendar page**

- [ ] **Step 3: Add translation keys**

`Calendar.ThisMonth.Title`, `Calendar.ThisMonth.Empty`

- [ ] **Step 4: Build frontend**

Run: `npm run build --prefix garden-assistant-app`

- [ ] **Step 5: Commit**

```
feat(E10): add "this month" widget to calendar page
```

---

## Task 14: Frontend — Action type filter chips (US-062)

**Files:**
- Modify: `garden-assistant-app/src/app/features/calendar/calendar.ts`
- Modify: `garden-assistant-app/src/app/features/calendar/calendar.html`

- [ ] **Step 1: Add filter chips above the grid**

Use Angular Material chips or styled Tailwind buttons. Each chip represents an action type with its color. Toggle on/off via CalendarStore.activeFilters signal. All active by default. Filter is applied client-side — only show matching bars in the grid.

- [ ] **Step 2: Build frontend**

Run: `npm run build --prefix garden-assistant-app`

- [ ] **Step 3: Commit**

```
feat(E10): add action type filter chips to calendar page
```

---

## Task 15: Frontend — Action type educational popups (US-079)

**Files:**
- Modify: `public/i18n/fr.json`
- Modify: calendar and Gantt components to open badge-info-dialog on label click

- [ ] **Step 1: Add BadgeInfo.Action.* translation keys**

For each of the 8 action types, add title and description keys:
`BadgeInfo.Action.IndoorSowing.Title`, `BadgeInfo.Action.IndoorSowing.Description`, etc.

- [ ] **Step 2: Add click handlers on action type labels**

In the Gantt component and filter chips, clicking an action type label opens the existing `BadgeInfoDialog` with the appropriate keys.

- [ ] **Step 3: Build frontend**

Run: `npm run build --prefix garden-assistant-app`

- [ ] **Step 4: Commit**

```
feat(E10): add educational popups for action types
```

---

## Task 16: Backend tests for CalendarController (US-080)

**Files:**
- Create: `garden-assistant-tests/PlantActions/CalendarControllerTests.cs`

- [ ] **Step 1: Write integration tests**

- `GetMyPlantsCalendar_WhenUserHasPlants_ShouldReturnCalendarWithActions`
- `GetMyPlantsCalendar_WhenUserHasNoPlants_ShouldReturnEmptyList`

- [ ] **Step 2: Run all tests**

Run: `dotnet test garden-assistant-tests`

- [ ] **Step 3: Commit**

```
test(E10): add CalendarController integration tests
```

---

## Dependency Graph

```
Task 1 (enums + Plant update)
  └─> Task 2 (entities + migration)
       ├─> Task 3 (plant seed update)
       ├─> Task 4 (actions seed data) ──> Task 6 (services)
       └─> Task 5 (harvest seed data) ──> Task 6 (services)
                                            └─> Task 7 (API endpoints)
                                                 ├─> Task 8 (frontend store)
                                                 │    └─> Task 9 (bar + gantt components)
                                                 │         └─> Task 10 (calendar page)
                                                 │              ├─> Task 13 (this month widget)
                                                 │              └─> Task 14 (filter chips)
                                                 ├─> Task 11 (calendar in plant detail)
                                                 ├─> Task 12 (harvest readiness UI)
                                                 ├─> Task 15 (educational popups)
                                                 └─> Task 16 (controller tests)
```

Tasks 3, 4, 5 can run in parallel after Task 2.
Tasks 11, 12, 15, 16 can run in parallel after Task 7.
Tasks 13, 14 depend on Task 10.
