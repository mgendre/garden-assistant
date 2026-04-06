# E25 — Calendrier d'arrosage — Plan d'implémentation

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Ajouter un calendrier d'arrosage hebdomadaire calculé automatiquement (WaterNeeds × saison × sol × paillage) intégré dans la page calendrier existante.

**Architecture:** Service backend `WateringCalculator` (pur, synchrone) + `WateringService` (async, BDD) → 2 endpoints dans `CalendarController` → `WateringStore` Angular → bannière "Arrosage aujourd'hui" (toujours visible) + grille 7 colonnes dans un tab du panel existant.

**Tech Stack:** .NET 10 / EF Core / SQLite (tests) / xUnit + Shouldly — Angular 19 signals / ngx-translate / Tailwind + 7-1 Sass

**Spec de référence :** `docs/superpowers/specs/2026-04-05-calendrier-arrosage-design.md`

---

## Carte des fichiers

### Nouveaux fichiers backend
| Fichier | Rôle |
|---|---|
| `garden-assistant-api/Models/WateringFrequency.cs` | Record résultat du calculator |
| `garden-assistant-api/Services/Watering/IWateringCalculator.cs` | Interface calculator |
| `garden-assistant-api/Services/Watering/WateringCalculator.cs` | Logique pur (matrice + coefficients + jours) |
| `garden-assistant-api/Services/Watering/IWateringService.cs` | Interface service BDD |
| `garden-assistant-api/Services/Watering/WateringService.cs` | Orchestration BDD + calculator → DTOs |
| `garden-assistant-api/DTOs/Watering/WateringTodayDto.cs` | DTO /watering/today |
| `garden-assistant-api/DTOs/Watering/BedWateringTodayDto.cs` | Planche dans today |
| `garden-assistant-api/DTOs/Watering/PlantWateringStatusDto.cs` | Plante dans today |
| `garden-assistant-api/DTOs/Watering/WateringScheduleDto.cs` | DTO /watering/schedule |
| `garden-assistant-api/DTOs/Watering/BedWateringDto.cs` | Planche dans schedule |
| `garden-assistant-api/DTOs/Watering/PlantWateringDto.cs` | Plante dans schedule |

### Fichiers backend modifiés
| Fichier | Changement |
|---|---|
| `garden-assistant-api/Data/Entities/Planting.cs` | + `SoilType? SoilType` (US-342), + `bool HasMulch` (US-343) |
| `garden-assistant-api/Data/Entities/Plant.cs` | + `int? WaterAmountMl` (US-344) |
| `garden-assistant-api/DTOs/Beds/BedDto.cs` | + `SoilType? SoilType`, `bool HasMulch` |
| `garden-assistant-api/DTOs/Beds/CreateBedRequest.cs` | + `SoilType? SoilType`, `bool HasMulch` |
| `garden-assistant-api/DTOs/Beds/UpdateBedRequest.cs` | + `SoilType? SoilType`, `bool HasMulch` |
| `garden-assistant-api/DTOs/Plants/PlantDto.cs` | + `int? WaterAmountMl` |
| `garden-assistant-api/Services/BedService.cs` | Persist + expose SoilType et HasMulch |
| `garden-assistant-api/Controllers/CalendarController.cs` | + 2 endpoints watering |
| `garden-assistant-api/ServiceCollectionExtensions.cs` | Enregistrer WateringCalculator + WateringService |

### Nouveaux fichiers frontend
| Fichier | Rôle |
|---|---|
| `garden-assistant-app/src/app/shared/services/watering.service.ts` | Appels HTTP watering |
| `garden-assistant-app/src/app/shared/services/watering.store.ts` | Signals today + schedule + weekOffset |
| `garden-assistant-app/src/app/features/calendar/calendar-watering-today/calendar-watering-today.ts` | Bannière toujours visible |
| `garden-assistant-app/src/app/features/calendar/calendar-watering-today/calendar-watering-today.html` | Template bannière |
| `garden-assistant-app/src/app/features/calendar/calendar-watering-today/calendar-watering-today.scss` | Styles bannière (vides, tout dans _watering.scss) |
| `garden-assistant-app/src/app/features/calendar/calendar-watering/calendar-watering.ts` | Grille hebdomadaire |
| `garden-assistant-app/src/app/features/calendar/calendar-watering/calendar-watering.html` | Template grille |
| `garden-assistant-app/src/app/features/calendar/calendar-watering/calendar-watering.scss` | Styles grille (vides) |
| `garden-assistant-app/src/styles/components/_watering.scss` | Toutes les classes watering |
| `garden-assistant-app/src/app/api/watering.api.ts` | Types TypeScript DTOs watering |

### Fichiers frontend modifiés
| Fichier | Changement |
|---|---|
| `garden-assistant-app/src/styles/abstracts/_variables.scss` | + 7 tokens `--color-water-*` |
| `garden-assistant-app/src/styles/main.scss` | Import `_watering.scss` |
| `garden-assistant-app/src/app/api/garden-assistant-api.ts` | + `soilType`/`hasMulch` dans BedDto, + `waterAmountMl` dans PlantDto, + enum SoilType |
| `garden-assistant-app/src/app/features/calendar/calendar.ts` | + CalendarWateringToday + tab toggle |
| `garden-assistant-app/src/app/features/calendar/calendar.html` | + bannière + tab toggle |
| `garden-assistant-app/src/app/features/garden/create-bed-dialog/create-bed-dialog.ts` | + SoilType select + HasMulch toggle |
| `garden-assistant-app/src/app/features/garden/create-bed-dialog/create-bed-dialog.html` | + champs SoilType + HasMulch |
| `garden-assistant-app/public/i18n/fr.json` | + clés `Watering.*` et `Bed.SoilType.*` |
| `garden-assistant-app/public/i18n/en.json` | + clés `Watering.*` et `Bed.SoilType.*` |

---

## Task 1 — US-339 : WateringFrequency + WateringCalculator (TDD)

**Files:**
- Create: `garden-assistant-api/Models/WateringFrequency.cs`
- Create: `garden-assistant-api/Services/Watering/IWateringCalculator.cs`
- Create: `garden-assistant-api/Services/Watering/WateringCalculator.cs`
- Create: `garden-assistant-tests/Watering/WateringCalculatorTests.cs`

- [ ] **Step 1 : Créer WateringFrequency**

```csharp
// garden-assistant-api/Models/WateringFrequency.cs
namespace GardenAssistant.Models;

public record WateringFrequency(
    int TimesPerWeek,
    DayOfWeek[] RecommendedDays,
    string? Notes);
```

- [ ] **Step 2 : Créer IWateringCalculator**

```csharp
// garden-assistant-api/Services/Watering/IWateringCalculator.cs
using GardenAssistant.Data.Entities.Enums;
using GardenAssistant.Models;

namespace GardenAssistant.Services.Watering;

public interface IWateringCalculator
{
    WateringFrequency CalculateFrequency(
        WaterNeeds waterNeeds,
        int halfMonth,
        SoilType? soilType = null,
        bool hasMulch = false);
}
```

- [ ] **Step 3 : Écrire les tests (fichier complet)**

```csharp
// garden-assistant-tests/Watering/WateringCalculatorTests.cs
using GardenAssistant.Data.Entities.Enums;
using GardenAssistant.Services.Watering;
using Shouldly;

namespace GardenAssistant.Tests.Watering;

public class WateringCalculatorTests
{
    private readonly WateringCalculator _sut = new();

    // --- Matrice de base ---

    [Theory]
    [InlineData(1,  1)]  // Low, hiver dm1
    [InlineData(4,  1)]  // Low, hiver dm4
    [InlineData(23, 1)]  // Low, hiver dm23
    [InlineData(24, 1)]  // Low, hiver dm24
    public void CalculateFrequency_WhenLowAndWinter_ShouldReturn1(int halfMonth, int expected)
    {
        var result = _sut.CalculateFrequency(WaterNeeds.Low, halfMonth);
        result.TimesPerWeek.ShouldBe(expected);
    }

    [Theory]
    [InlineData(5,  1)]  // Low, printemps
    [InlineData(10, 1)]
    public void CalculateFrequency_WhenLowAndSpring_ShouldReturn1(int halfMonth, int expected)
    {
        var result = _sut.CalculateFrequency(WaterNeeds.Low, halfMonth);
        result.TimesPerWeek.ShouldBe(expected);
    }

    [Theory]
    [InlineData(11, 2)]  // Low, été
    [InlineData(16, 2)]
    public void CalculateFrequency_WhenLowAndSummer_ShouldReturn2(int halfMonth, int expected)
    {
        var result = _sut.CalculateFrequency(WaterNeeds.Low, halfMonth);
        result.TimesPerWeek.ShouldBe(expected);
    }

    [Theory]
    [InlineData(11, 4)]  // Medium, été
    [InlineData(16, 4)]
    public void CalculateFrequency_WhenMediumAndSummer_ShouldReturn4(int halfMonth, int expected)
    {
        var result = _sut.CalculateFrequency(WaterNeeds.Medium, halfMonth);
        result.TimesPerWeek.ShouldBe(expected);
    }

    [Theory]
    [InlineData(11, 5)]  // High, été
    [InlineData(16, 5)]
    public void CalculateFrequency_WhenHighAndSummer_ShouldReturn5(int halfMonth, int expected)
    {
        var result = _sut.CalculateFrequency(WaterNeeds.High, halfMonth);
        result.TimesPerWeek.ShouldBe(expected);
    }

    [Theory]
    [InlineData(5,  3)]  // High, printemps
    [InlineData(17, 3)]  // High, automne
    public void CalculateFrequency_WhenHighAndSpringOrAutumn_ShouldReturn3(int halfMonth, int expected)
    {
        var result = _sut.CalculateFrequency(WaterNeeds.High, halfMonth);
        result.TimesPerWeek.ShouldBe(expected);
    }

    [Theory]
    [InlineData(1,  2)]  // High, hiver
    [InlineData(24, 2)]
    public void CalculateFrequency_WhenHighAndWinter_ShouldReturn2(int halfMonth, int expected)
    {
        var result = _sut.CalculateFrequency(WaterNeeds.High, halfMonth);
        result.TimesPerWeek.ShouldBe(expected);
    }

    // --- RecommendedDays ---

    [Fact]
    public void CalculateFrequency_RecommendedDaysLength_ShouldMatchTimesPerWeek()
    {
        // Low/été → 2x/semaine → 2 jours
        var result = _sut.CalculateFrequency(WaterNeeds.Low, halfMonth: 13);
        result.RecommendedDays.Length.ShouldBe(result.TimesPerWeek);
    }

    [Fact]
    public void CalculateFrequency_WhenOncePerWeek_ShouldRecommendSaturday()
    {
        var result = _sut.CalculateFrequency(WaterNeeds.Low, halfMonth: 7); // printemps → 1x
        result.RecommendedDays.ShouldBe([DayOfWeek.Saturday]);
    }

    [Fact]
    public void CalculateFrequency_WhenTwicePerWeek_ShouldRecommendWedSat()
    {
        var result = _sut.CalculateFrequency(WaterNeeds.Low, halfMonth: 13); // été → 2x
        result.RecommendedDays.ShouldBe([DayOfWeek.Wednesday, DayOfWeek.Saturday]);
    }

    [Fact]
    public void CalculateFrequency_WhenFourPerWeek_ShouldRecommendTueThuSatSun()
    {
        var result = _sut.CalculateFrequency(WaterNeeds.Medium, halfMonth: 13); // été → 4x
        result.RecommendedDays.ShouldBe([DayOfWeek.Tuesday, DayOfWeek.Thursday, DayOfWeek.Saturday, DayOfWeek.Sunday]);
    }
}
```

- [ ] **Step 4 : Lancer les tests (doivent ÉCHOUER)**

```bash
dotnet test garden-assistant-tests --filter "WateringCalculatorTests" 2>&1 | tail -5
```
Résultat attendu : erreur de compilation (type `WateringCalculator` introuvable).

- [ ] **Step 5 : Implémenter WateringCalculator**

```csharp
// garden-assistant-api/Services/Watering/WateringCalculator.cs
using GardenAssistant.Data.Entities.Enums;
using GardenAssistant.Models;

namespace GardenAssistant.Services.Watering;

public class WateringCalculator : IWateringCalculator
{
    private static readonly Dictionary<int, DayOfWeek[]> RecommendedDaysMap = new()
    {
        [1] = [DayOfWeek.Saturday],
        [2] = [DayOfWeek.Wednesday, DayOfWeek.Saturday],
        [3] = [DayOfWeek.Wednesday, DayOfWeek.Saturday, DayOfWeek.Sunday],
        [4] = [DayOfWeek.Tuesday, DayOfWeek.Thursday, DayOfWeek.Saturday, DayOfWeek.Sunday],
        [5] = [DayOfWeek.Monday, DayOfWeek.Wednesday, DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday],
        [6] = [DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday],
        [7] = [DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday],
    };

    public WateringFrequency CalculateFrequency(
        WaterNeeds waterNeeds,
        int halfMonth,
        SoilType? soilType = null,
        bool hasMulch = false)
    {
        var baseFrequency = GetBaseFrequency(waterNeeds, GetSeason(halfMonth));
        var adjusted = ApplyCoefficients(baseFrequency, soilType, hasMulch, GetSeason(halfMonth));
        var days = RecommendedDaysMap.TryGetValue(adjusted, out var d) ? d : RecommendedDaysMap[7];
        return new WateringFrequency(adjusted, days, null);
    }

    private static int GetBaseFrequency(WaterNeeds waterNeeds, Season season) => (waterNeeds, season) switch
    {
        (WaterNeeds.Low,    Season.Winter) => 1,
        (WaterNeeds.Low,    Season.Spring) => 1,
        (WaterNeeds.Low,    Season.Summer) => 2,
        (WaterNeeds.Low,    Season.Autumn) => 1,
        (WaterNeeds.Medium, Season.Winter) => 1,
        (WaterNeeds.Medium, Season.Spring) => 2,
        (WaterNeeds.Medium, Season.Summer) => 4,
        (WaterNeeds.Medium, Season.Autumn) => 2,
        (WaterNeeds.High,   Season.Winter) => 2,
        (WaterNeeds.High,   Season.Spring) => 3,
        (WaterNeeds.High,   Season.Summer) => 5,
        (WaterNeeds.High,   Season.Autumn) => 3,
        _ => 1
    };

    private static Season GetSeason(int halfMonth) => halfMonth switch
    {
        >= 1  and <= 4  => Season.Winter,
        >= 5  and <= 10 => Season.Spring,
        >= 11 and <= 16 => Season.Summer,
        >= 17 and <= 22 => Season.Autumn,
        >= 23 and <= 24 => Season.Winter,
        _ => throw new ArgumentOutOfRangeException(nameof(halfMonth))
    };

    private static int ApplyCoefficients(int baseFrequency, SoilType? soilType, bool hasMulch, Season season)
    {
        var frequency = baseFrequency * GetSoilCoefficient(soilType);
        if (hasMulch) { frequency *= 0.6; }
        var rounded = (int)Math.Round(frequency);
        var minimum = season == Season.Winter ? 0 : 1;
        return Math.Max(rounded, minimum);
    }

    private static double GetSoilCoefficient(SoilType? soilType) => soilType switch
    {
        SoilType.Sandy  => 1.3,
        SoilType.Loam   => 1.0,
        SoilType.Clay   => 0.7,
        SoilType.Silty  => 0.9,
        SoilType.Chalky => 1.2,
        SoilType.Peaty  => 0.8,
        SoilType.Rocky  => 1.3,
        _ => 1.0
    };

    private enum Season { Winter, Spring, Summer, Autumn }
}
```

- [ ] **Step 6 : Lancer les tests (doivent PASSER)**

```bash
dotnet test garden-assistant-tests --filter "WateringCalculatorTests" 2>&1 | tail -5
```
Résultat attendu : tous les tests PASS.

- [ ] **Step 7 : Enregistrer dans le conteneur DI**

Dans `garden-assistant-api/ServiceCollectionExtensions.cs`, ajouter après les existants :
```csharp
services.AddSingleton<IWateringCalculator, WateringCalculator>();
```
`AddSingleton` : le service est pur et sans état.

- [ ] **Step 8 : Vérifier la compilation**

```bash
dotnet build garden-assistant-api/garden-assistant-api.csproj 2>&1 | tail -5
```
Résultat attendu : `Build succeeded`.

- [ ] **Step 9 : Commit**

```bash
git add garden-assistant-api/Models/WateringFrequency.cs \
        garden-assistant-api/Services/Watering/IWateringCalculator.cs \
        garden-assistant-api/Services/Watering/WateringCalculator.cs \
        garden-assistant-api/ServiceCollectionExtensions.cs \
        garden-assistant-tests/Watering/WateringCalculatorTests.cs
git commit -m "feat(US-339): WateringCalculator — matrice fréquences et jours recommandés

Co-Authored-By: Claude Sonnet 4.6 <noreply@anthropic.com>"
```

---

## Task 2 — US-340 : DTOs today + IWateringService + GetWateringTodayAsync (TDD)

**Files:**
- Create: `garden-assistant-api/DTOs/Watering/WateringTodayDto.cs`
- Create: `garden-assistant-api/DTOs/Watering/BedWateringTodayDto.cs`
- Create: `garden-assistant-api/DTOs/Watering/PlantWateringStatusDto.cs`
- Create: `garden-assistant-api/Services/Watering/IWateringService.cs`
- Create: `garden-assistant-api/Services/Watering/WateringService.cs`
- Create: `garden-assistant-tests/Watering/WateringServiceTodayTests.cs`

- [ ] **Step 1 : Créer les DTOs**

```csharp
// garden-assistant-api/DTOs/Watering/PlantWateringStatusDto.cs
namespace GardenAssistant.DTOs.Watering;
public record PlantWateringStatusDto(Guid PlantId, string PlantName, bool IsToday, DayOfWeek? NextWateringDay);

// garden-assistant-api/DTOs/Watering/BedWateringTodayDto.cs
namespace GardenAssistant.DTOs.Watering;
public record BedWateringTodayDto(Guid? BedId, string BedName, bool IsPersonalPlants, List<PlantWateringStatusDto> Plants);

// garden-assistant-api/DTOs/Watering/WateringTodayDto.cs
namespace GardenAssistant.DTOs.Watering;
public record WateringTodayDto(List<BedWateringTodayDto> Beds);
```

- [ ] **Step 2 : Créer IWateringService**

```csharp
// garden-assistant-api/Services/Watering/IWateringService.cs
using GardenAssistant.DTOs.Watering;

namespace GardenAssistant.Services.Watering;

public interface IWateringService
{
    Task<WateringTodayDto> GetWateringTodayAsync(Guid userId, DateOnly today);
    Task<WateringScheduleDto> GetWateringScheduleAsync(Guid userId, int halfMonth, string source);
}
```

Note : `WateringScheduleDto` sera créé à la Task 4. Pour compiler en attendant, créer le record vide :
```csharp
// garden-assistant-api/DTOs/Watering/WateringScheduleDto.cs  (stub, complété en Task 4)
namespace GardenAssistant.DTOs.Watering;
public record WateringScheduleDto(List<object> Beds); // remplacé en Task 4
```

- [ ] **Step 3 : Écrire les tests pour GetWateringTodayAsync**

```csharp
// garden-assistant-tests/Watering/WateringServiceTodayTests.cs
using GardenAssistant.Data.Entities;
using GardenAssistant.Data.Entities.Enums;
using GardenAssistant.Services.Watering;
using GardenAssistant.Tests.Infrastructure;
using Shouldly;

namespace GardenAssistant.Tests.Watering;

public class WateringServiceTodayTests : DatabaseTestBase
{
    private readonly WateringService _sut;
    private readonly Guid _userId = Guid.NewGuid();
    private readonly Guid _gardenId = Guid.NewGuid();

    public WateringServiceTodayTests()
    {
        DbContext.Users.Add(new User { Id = _userId, Email = "test@example.com" });
        DbContext.Gardens.Add(new Garden { Id = _gardenId, Name = "Test", UserId = _userId, CreatedAtUtc = DateTime.UtcNow });
        DbContext.SaveChanges();
        _sut = new WateringService(DbContext, new WateringCalculator());
    }

    [Fact]
    public async Task GetWateringTodayAsync_WhenNoBeds_ShouldReturnEmptyBeds()
    {
        var result = await _sut.GetWateringTodayAsync(_userId, DateOnly.FromDateTime(DateTime.UtcNow));
        result.Beds.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetWateringTodayAsync_WhenBedHasPlants_ShouldReturnBedWithPlantStatus()
    {
        var (plant, _) = SeedBedWithPlant(WaterNeeds.Low);

        var result = await _sut.GetWateringTodayAsync(_userId, DateOnly.FromDateTime(DateTime.UtcNow));

        result.Beds.Count.ShouldBe(1);
        result.Beds[0].Plants.Count.ShouldBe(1);
        result.Beds[0].Plants[0].PlantId.ShouldBe(plant.Id);
        result.Beds[0].IsPersonalPlants.ShouldBeFalse();
    }

    [Fact]
    public async Task GetWateringTodayAsync_WhenPlantIsToday_IsToday_ShouldBeTrue()
    {
        // Un dimanche et High/été = 5x avec Dim dans les jours → IsToday = true
        SeedBedWithPlant(WaterNeeds.High);
        var sunday = NextDayOfWeek(DayOfWeek.Sunday);

        var result = await _sut.GetWateringTodayAsync(_userId, sunday);

        result.Beds[0].Plants[0].IsToday.ShouldBeTrue();
    }

    [Fact]
    public async Task GetWateringTodayAsync_WhenOtherUser_ShouldReturnEmptyBeds()
    {
        SeedBedWithPlant(WaterNeeds.Low);
        var otherUser = Guid.NewGuid();
        DbContext.Users.Add(new User { Id = otherUser, Email = "other@example.com" });
        DbContext.SaveChanges();

        var result = await _sut.GetWateringTodayAsync(otherUser, DateOnly.FromDateTime(DateTime.UtcNow));

        result.Beds.ShouldBeEmpty();
    }

    private (Plant plant, Planting bed) SeedBedWithPlant(WaterNeeds waterNeeds)
    {
        var plant = new Plant { Id = Guid.NewGuid(), Key = $"plant-{Guid.NewGuid()}", Name = "Tomate", WaterNeeds = waterNeeds };
        var guild = new Guild { Id = Guid.NewGuid(), UserId = _userId, Name = "Guilde" };
        var bed = new Planting { Id = Guid.NewGuid(), GardenId = _gardenId, UserId = _userId, Name = "Planche", GuildId = guild.Id, CreatedAtUtc = DateTime.UtcNow };
        DbContext.Plants.Add(plant);
        DbContext.Guilds.Add(guild);
        DbContext.Plantings.Add(bed);
        DbContext.GuildPlants.Add(new GuildPlant { GuildId = guild.Id, PlantId = plant.Id });
        DbContext.SaveChanges();
        return (plant, bed);
    }

    private static DateOnly NextDayOfWeek(DayOfWeek day)
    {
        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        while (date.DayOfWeek != day) { date = date.AddDays(1); }
        return date;
    }
}
```

- [ ] **Step 4 : Lancer les tests (doivent ÉCHOUER)**

```bash
dotnet test garden-assistant-tests --filter "WateringServiceTodayTests" 2>&1 | tail -5
```
Résultat attendu : erreur de compilation (`WateringService` introuvable).

- [ ] **Step 5 : Implémenter WateringService.GetWateringTodayAsync**

```csharp
// garden-assistant-api/Services/Watering/WateringService.cs
using GardenAssistant.Data;
using GardenAssistant.DTOs.Watering;
using Microsoft.EntityFrameworkCore;

namespace GardenAssistant.Services.Watering;

public class WateringService(AppDbContext dbContext, IWateringCalculator calculator) : IWateringService
{
    public async Task<WateringTodayDto> GetWateringTodayAsync(Guid userId, DateOnly today)
    {
        var (plantings, plantsById) = await LoadGardenDataAsync(userId);
        var halfMonth = GetHalfMonth(today);

        var beds = plantings
            .Where(p => p.GuildId.HasValue)
            .Select(p => BuildBedTodayDto(p, plantsById, halfMonth, today))
            .Where(b => b.Plants.Count > 0)
            .ToList();

        return new WateringTodayDto(beds);
    }

    public Task<WateringScheduleDto> GetWateringScheduleAsync(Guid userId, int halfMonth, string source)
        => Task.FromResult(new WateringScheduleDto([])); // implémenté en Task 4

    private BedWateringTodayDto BuildBedTodayDto(
        Data.Entities.Planting planting,
        Dictionary<Guid, Data.Entities.Plant> plantsById,
        int halfMonth,
        DateOnly today)
    {
        var plantStatuses = GetPlantIdsForBed(planting.GuildId!.Value, plantsById)
            .Select(plant =>
            {
                var freq = calculator.CalculateFrequency(plant.WaterNeeds, halfMonth);
                var isToday = freq.RecommendedDays.Contains(today.DayOfWeek);
                var next = isToday ? (DayOfWeek?)null : FindNextDay(freq.RecommendedDays, today.DayOfWeek);
                return new PlantWateringStatusDto(plant.Id, plant.Name, isToday, next);
            })
            .ToList();

        return new BedWateringTodayDto(planting.Id, planting.Name, false, plantStatuses);
    }

    private IEnumerable<Data.Entities.Plant> GetPlantIdsForBed(Guid guildId, Dictionary<Guid, Data.Entities.Plant> plantsById)
    {
        return dbContext.GuildPlants
            .Where(gp => gp.GuildId == guildId)
            .Select(gp => gp.PlantId)
            .AsEnumerable()
            .Where(plantsById.ContainsKey)
            .Select(id => plantsById[id]);
    }

    private async Task<(List<Data.Entities.Planting> plantings, Dictionary<Guid, Data.Entities.Plant> plantsById)> LoadGardenDataAsync(Guid userId)
    {
        var plantings = await dbContext.Plantings
            .Where(p => p.UserId == userId && p.GuildId.HasValue)
            .ToListAsync();

        var guildIds = plantings.Select(p => p.GuildId!.Value).ToList();

        var plantIds = await dbContext.GuildPlants
            .Where(gp => guildIds.Contains(gp.GuildId))
            .Select(gp => gp.PlantId)
            .Distinct()
            .ToListAsync();

        var plantsById = await dbContext.Plants
            .Where(p => plantIds.Contains(p.Id))
            .ToDictionaryAsync(p => p.Id);

        return (plantings, plantsById);
    }

    private static int GetHalfMonth(DateOnly date)
        => (date.Month - 1) * 2 + (date.Day <= 15 ? 1 : 2);

    private static DayOfWeek? FindNextDay(DayOfWeek[] days, DayOfWeek today)
    {
        var todayInt = (int)today;
        return days
            .Select(d => ((int)d - todayInt + 7) % 7)
            .Where(diff => diff > 0)
            .OrderBy(diff => diff)
            .Select<int, DayOfWeek?>(diff => (DayOfWeek)((todayInt + diff) % 7))
            .FirstOrDefault()
            ?? days.MinBy(d => (int)d);
    }
}
```

- [ ] **Step 6 : Lancer les tests (doivent PASSER)**

```bash
dotnet test garden-assistant-tests --filter "WateringServiceTodayTests" 2>&1 | tail -5
```
Résultat attendu : tous PASS.

- [ ] **Step 7 : Enregistrer dans DI**

Dans `ServiceCollectionExtensions.cs`, ajouter :
```csharp
services.AddScoped<IWateringService, WateringService>();
```

- [ ] **Step 8 : Commit**

```bash
git add garden-assistant-api/DTOs/Watering/ \
        garden-assistant-api/Services/Watering/IWateringService.cs \
        garden-assistant-api/Services/Watering/WateringService.cs \
        garden-assistant-api/ServiceCollectionExtensions.cs \
        garden-assistant-tests/Watering/WateringServiceTodayTests.cs
git commit -m "feat(US-340): WateringService.GetWateringTodayAsync

Co-Authored-By: Claude Sonnet 4.6 <noreply@anthropic.com>"
```

---

## Task 3 — US-340 : Endpoint GET /api/calendar/watering/today

**Files:**
- Modify: `garden-assistant-api/Controllers/CalendarController.cs`

- [ ] **Step 1 : Injecter IWateringService et ajouter l'endpoint**

Modifier le constructeur et ajouter la méthode :

```csharp
// garden-assistant-api/Controllers/CalendarController.cs
// Remplacer la signature du constructeur
public class CalendarController(
    IUserPlantService userPlantService,
    IPlantActionService plantActionService,
    IWateringService wateringService) : ControllerBase   // ← ajouter wateringService
```

Ajouter l'import en tête :
```csharp
using GardenAssistant.DTOs.Watering;
using GardenAssistant.Services.Watering;
```

Ajouter après l'endpoint existant :
```csharp
[HttpGet("watering/today")]
[ProducesResponseType(typeof(WateringTodayDto), StatusCodes.Status200OK)]
public async Task<IActionResult> GetWateringToday()
{
    var today = DateOnly.FromDateTime(DateTime.UtcNow);
    var result = await wateringService.GetWateringTodayAsync(CallerId, today);
    return Ok(result);
}
```

- [ ] **Step 2 : Vérifier la compilation**

```bash
dotnet build garden-assistant-api/garden-assistant-api.csproj 2>&1 | tail -5
```

- [ ] **Step 3 : Tester manuellement**

```bash
dotnet run --project garden-assistant-api &
sleep 3
# Obtenir un token dev depuis l'auth, puis :
curl -H "Authorization: Bearer <token>" http://localhost:5000/api/calendar/watering/today
# Résultat attendu : { "beds": [] } (ou des planches si des données existent)
kill %1
```

- [ ] **Step 4 : Commit**

```bash
git add garden-assistant-api/Controllers/CalendarController.cs
git commit -m "feat(US-340): endpoint GET /api/calendar/watering/today

Co-Authored-By: Claude Sonnet 4.6 <noreply@anthropic.com>"
```

---

## Task 4 — US-340 : CSS tokens + _watering.scss (bannière)

**Files:**
- Modify: `garden-assistant-app/src/styles/abstracts/_variables.scss`
- Create: `garden-assistant-app/src/styles/components/_watering.scss`
- Modify: `garden-assistant-app/src/styles/main.scss`

- [ ] **Step 1 : Ajouter les tokens CSS dans `_variables.scss`**

Trouver le bloc des variables CSS custom (chercher `--color-`) et ajouter :
```scss
// Watering — couleurs eau
--color-water: #42a5f5;
--color-water-dark: #1565c0;
--color-water-bg: rgba(66, 165, 245, 0.08);
--color-water-bg-medium: rgba(66, 165, 245, 0.15);
--color-water-bg-strong: rgba(66, 165, 245, 0.28);
--color-water-border: rgba(66, 165, 245, 0.25);
--color-water-border-active: rgba(66, 165, 245, 0.4);
```

- [ ] **Step 2 : Créer `_watering.scss` avec les styles de la bannière**

```scss
// garden-assistant-app/src/styles/components/_watering.scss

// ─── Bannière "Arrosage aujourd'hui" ───────────────────────────────────────

.watering-banner {
  display: flex;
  align-items: flex-start;
  flex-wrap: wrap;
  gap: 0.5rem;
  padding: 0.75rem 1.25rem;
  background: var(--color-water-bg);
  border-top: 1px solid var(--color-water-border);
  border-bottom: 1px solid var(--color-water-border);
  margin-bottom: 1.25rem;

  @media (min-width: 768px) {
    align-items: center;
    flex-wrap: nowrap;
  }
}

.watering-banner__icon {
  color: var(--color-water);
  font-size: 1rem;
  flex-shrink: 0;
}

.watering-banner__label {
  font-size: 0.875rem;
  font-weight: 600;
  color: var(--color-water-dark);
  flex-shrink: 0;
}

.watering-banner__badges {
  display: flex;
  flex-wrap: wrap;
  gap: 0.375rem;
}

.watering-banner__next {
  font-size: 0.8125rem;
  color: var(--color-text-muted);
}

.watering-badge {
  display: inline-flex;
  align-items: center;
  min-height: 2rem;
  padding: 0.25rem 0.625rem;
  background: var(--color-water-bg-medium);
  border: 1px solid var(--color-water-border-active);
  border-radius: var(--radius-sm, 6px);
  font-size: 0.8125rem;
  font-weight: 500;
  color: var(--color-water-dark);
  cursor: pointer;
  transition: all var(--transition-fast, 150ms ease);

  &:hover {
    background: var(--color-water-bg-strong);
    border-color: var(--color-water);
    transform: translateY(-1px);
  }

  &:focus-visible {
    outline: 2px solid var(--color-water);
    outline-offset: 2px;
  }

  @media (max-width: 767px) {
    min-height: 2.75rem;
  }
}

// ─── Grille hebdomadaire (ajoutée en Task 8) ───────────────────────────────
```

- [ ] **Step 3 : Importer dans `main.scss`**

Dans `garden-assistant-app/src/styles/main.scss`, trouver la section `@forward 'components/...'` et ajouter :
```scss
@forward 'components/watering';
```

- [ ] **Step 4 : Vérifier la compilation**

```bash
npm run build --prefix garden-assistant-app 2>&1 | tail -10
```
Résultat attendu : `Build succeeded`.

- [ ] **Step 5 : Commit**

```bash
git add garden-assistant-app/src/styles/abstracts/_variables.scss \
        garden-assistant-app/src/styles/components/_watering.scss \
        garden-assistant-app/src/styles/main.scss
git commit -m "feat(US-340): tokens CSS eau et styles bannière arrosage

Co-Authored-By: Claude Sonnet 4.6 <noreply@anthropic.com>"
```

---

## Task 5 — US-340 : WateringService Angular + WateringStore + CalendarWateringTodayComponent

**Files:**
- Create: `garden-assistant-app/src/app/shared/services/watering.service.ts`
- Create: `garden-assistant-app/src/app/shared/services/watering.store.ts`
- Create: `garden-assistant-app/src/app/api/watering.api.ts`
- Create: `garden-assistant-app/src/app/features/calendar/calendar-watering-today/calendar-watering-today.ts`
- Create: `garden-assistant-app/src/app/features/calendar/calendar-watering-today/calendar-watering-today.html`
- Create: `garden-assistant-app/src/app/features/calendar/calendar-watering-today/calendar-watering-today.scss`
- Modify: `garden-assistant-app/src/app/features/calendar/calendar.ts`
- Modify: `garden-assistant-app/src/app/features/calendar/calendar.html`
- Modify: `garden-assistant-app/public/i18n/fr.json`
- Modify: `garden-assistant-app/public/i18n/en.json`

- [ ] **Step 1 : Créer les types TypeScript**

```typescript
// garden-assistant-app/src/app/api/watering.api.ts

export type DayOfWeekStr = 'Monday' | 'Tuesday' | 'Wednesday' | 'Thursday' | 'Friday' | 'Saturday' | 'Sunday';

export const WEEK_DAYS: DayOfWeekStr[] = ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday'];

export interface PlantWateringStatusDto {
  plantId: string;
  plantName: string;
  isToday: boolean;
  nextWateringDay?: DayOfWeekStr;
}

export interface BedWateringTodayDto {
  bedId?: string;
  bedName: string;
  isPersonalPlants: boolean;
  plants: PlantWateringStatusDto[];
}

export interface WateringTodayDto {
  beds: BedWateringTodayDto[];
}

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

- [ ] **Step 2 : Créer WateringService Angular**

```typescript
// garden-assistant-app/src/app/shared/services/watering.service.ts
import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { WateringTodayDto, WateringScheduleDto } from '../../api/watering.api';

@Injectable({ providedIn: 'root' })
export class WateringService {
  private readonly http = inject(HttpClient);

  getWateringToday(): Promise<WateringTodayDto> {
    return firstValueFrom(this.http.get<WateringTodayDto>('/api/calendar/watering/today'));
  }

  getWateringSchedule(halfMonth: number, source: string): Promise<WateringScheduleDto> {
    return firstValueFrom(
      this.http.get<WateringScheduleDto>('/api/calendar/watering/schedule', {
        params: { halfMonth, source }
      })
    );
  }
}
```

- [ ] **Step 3 : Créer WateringStore**

```typescript
// garden-assistant-app/src/app/shared/services/watering.store.ts
import { Injectable, inject, signal, computed, effect } from '@angular/core';
import { CalendarStore } from './calendar.store';
import { WateringService } from './watering.service';
import { WateringTodayDto, WateringScheduleDto, DayOfWeekStr } from '../../api/watering.api';

const DAY_LABELS: Record<DayOfWeekStr, string> = {
  Monday: 'Watering.Day.Monday', Tuesday: 'Watering.Day.Tuesday',
  Wednesday: 'Watering.Day.Wednesday', Thursday: 'Watering.Day.Thursday',
  Friday: 'Watering.Day.Friday', Saturday: 'Watering.Day.Saturday',
  Sunday: 'Watering.Day.Sunday',
};

@Injectable({ providedIn: 'root' })
export class WateringStore {
  private readonly calendarStore = inject(CalendarStore);
  private readonly wateringService = inject(WateringService);

  readonly todayData = signal<WateringTodayDto | null>(null);
  readonly scheduleData = signal<WateringScheduleDto | null>(null);
  readonly loadingToday = signal(false);
  readonly loadingSchedule = signal(false);
  readonly weekOffset = signal(0); // 0 = semaine courante, 1 = suivante
  readonly scheduleTabActive = signal(false);

  readonly todayPlants = computed(() =>
    this.todayData()?.beds.flatMap(b => b.plants.filter(p => p.isToday)) ?? []
  );

  readonly nextWateringDayKey = computed(() => {
    const allPlants = this.todayData()?.beds.flatMap(b => b.plants) ?? [];
    const first = allPlants.find(p => !p.isToday && p.nextWateringDay);
    return first?.nextWateringDay ? DAY_LABELS[first.nextWateringDay] : null;
  });

  private readonly reloadOnFilterChange = effect(() => {
    this.calendarStore.sourceFilter(); // lecture du signal pour s'abonner
    if (this.scheduleTabActive()) {
      this.loadSchedule();
    }
  });

  async loadToday(): Promise<void> {
    this.loadingToday.set(true);
    try {
      this.todayData.set(await this.wateringService.getWateringToday());
    } finally {
      this.loadingToday.set(false);
    }
  }

  async loadSchedule(): Promise<void> {
    this.loadingSchedule.set(true);
    try {
      const halfMonth = this.getHalfMonth(this.weekOffset());
      const source = this.calendarStore.sourceFilter();
      this.scheduleData.set(await this.wateringService.getWateringSchedule(halfMonth, source));
    } finally {
      this.loadingSchedule.set(false);
    }
  }

  private getHalfMonth(weekOffset: number): number {
    const date = new Date();
    date.setDate(date.getDate() + weekOffset * 7);
    return date.getMonth() * 2 + (date.getDate() <= 15 ? 1 : 2);
  }
}
```

- [ ] **Step 4 : Créer CalendarWateringTodayComponent**

```typescript
// garden-assistant-app/src/app/features/calendar/calendar-watering-today/calendar-watering-today.ts
import { Component, inject, OnInit } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';
import { faDroplet } from '@fortawesome/free-solid-svg-icons';
import { FaIconComponent } from '@fortawesome/angular-fontawesome';
import { WateringStore } from '../../../shared/services/watering.store';

@Component({
  selector: 'app-calendar-watering-today',
  standalone: true,
  imports: [TranslateModule, FaIconComponent],
  templateUrl: './calendar-watering-today.html',
  styleUrl: './calendar-watering-today.scss',
  host: { style: 'display:block' }
})
export class CalendarWateringToday implements OnInit {
  protected readonly store = inject(WateringStore);
  protected readonly faDroplet = faDroplet;

  async ngOnInit(): Promise<void> {
    await this.store.loadToday();
  }
}
```

```html
<!-- garden-assistant-app/src/app/features/calendar/calendar-watering-today/calendar-watering-today.html -->
@if (!store.loadingToday() && (store.todayPlants().length > 0 || store.nextWateringDayKey())) {
  <div class="watering-banner" role="status" aria-live="polite">
    <fa-icon [icon]="faDroplet" class="watering-banner__icon" aria-hidden="true"></fa-icon>
    <span class="watering-banner__label" [translate]="'Watering.TodayLabel'"></span>

    @if (store.todayPlants().length > 0) {
      <div class="watering-banner__badges">
        @for (plant of store.todayPlants(); track plant.plantId) {
          <button class="watering-badge"
                  [attr.aria-label]="'Watering.PlantBadgeAriaLabel' | translate : { name: plant.plantName }">
            {{ plant.plantName }}
          </button>
        }
      </div>
    } @else {
      <span class="watering-banner__next">
        {{ 'Watering.NextWatering' | translate : { day: store.nextWateringDayKey()! | translate } }}
      </span>
    }
  </div>
}
```

```scss
// garden-assistant-app/src/app/features/calendar/calendar-watering-today/calendar-watering-today.scss
// Styles définis dans _watering.scss
```

- [ ] **Step 5 : Ajouter les clés i18n**

Dans `garden-assistant-app/public/i18n/fr.json`, ajouter dans la section existante (avant le dernier `}`) :
```json
"Watering": {
  "TodayLabel": "Arrosage aujourd'hui",
  "NextWatering": "Prochain arrosage : {{day}}",
  "PlantBadgeAriaLabel": "Voir le détail de {{name}}",
  "Day": {
    "Monday": "lundi",
    "Tuesday": "mardi",
    "Wednesday": "mercredi",
    "Thursday": "jeudi",
    "Friday": "vendredi",
    "Saturday": "samedi",
    "Sunday": "dimanche"
  },
  "TabActions": "Actions culturales",
  "TabWatering": "Arrosage",
  "WeekCurrent": "Semaine courante",
  "WeekNext": "Semaine suivante",
  "PrevWeek": "Semaine précédente",
  "NextWeek": "Semaine suivante",
  "GridAriaLabel": "Calendrier d'arrosage, semaine du {{date}}",
  "FrequenciesTitle": "Fréquences saisonnières",
  "FreqPlantCol": "Plante",
  "FreqNeedCol": "Besoin en eau",
  "FreqRateCol": "Fréquence",
  "WaterNeed": {
    "Low": "Faible",
    "Medium": "Moyen",
    "High": "Élevé"
  },
  "TimesPerWeek": "{{count}}×/sem.",
  "EmptyState": "Aucune plante avec un calendrier d'arrosage.",
  "WaterAmountMl": "{{amount}} ml",
  "WaterAmountL": "{{amount}} L",
  "DailyTotalL": "~{{total}} L",
  "MyPlantsBed": "Mes plantes"
}
```

Dans `garden-assistant-app/public/i18n/en.json`, ajouter :
```json
"Watering": {
  "TodayLabel": "Watering today",
  "NextWatering": "Next watering: {{day}}",
  "PlantBadgeAriaLabel": "View details for {{name}}",
  "Day": {
    "Monday": "Monday",
    "Tuesday": "Tuesday",
    "Wednesday": "Wednesday",
    "Thursday": "Thursday",
    "Friday": "Friday",
    "Saturday": "Saturday",
    "Sunday": "Sunday"
  },
  "TabActions": "Growing calendar",
  "TabWatering": "Watering",
  "WeekCurrent": "Current week",
  "WeekNext": "Next week",
  "PrevWeek": "Previous week",
  "NextWeek": "Next week",
  "GridAriaLabel": "Watering calendar, week of {{date}}",
  "FrequenciesTitle": "Seasonal frequencies",
  "FreqPlantCol": "Plant",
  "FreqNeedCol": "Water needs",
  "FreqRateCol": "Frequency",
  "WaterNeed": {
    "Low": "Low",
    "Medium": "Medium",
    "High": "High"
  },
  "TimesPerWeek": "{{count}}×/week",
  "EmptyState": "No plants with a watering schedule.",
  "WaterAmountMl": "{{amount}} ml",
  "WaterAmountL": "{{amount}} L",
  "DailyTotalL": "~{{total}} L",
  "MyPlantsBed": "My plants"
}
```

- [ ] **Step 6 : Intégrer la bannière dans `calendar.ts` et `calendar.html`**

Dans `calendar.ts`, ajouter l'import :
```typescript
import { CalendarWateringToday } from './calendar-watering-today/calendar-watering-today';
```
Ajouter `CalendarWateringToday` dans le tableau `imports` du `@Component`.

Dans `calendar.html`, insérer **après** `<app-calendar-this-month>` et **avant** le `<div class="panel">` :
```html
@if (store.allCalendarPlants().length > 0) {
  <app-calendar-watering-today></app-calendar-watering-today>
}
```

- [ ] **Step 7 : npm run build (doit PASSER sans erreur)**

```bash
npm run build --prefix garden-assistant-app 2>&1 | tail -15
```

- [ ] **Step 8 : Commit**

```bash
git add garden-assistant-app/src/app/api/watering.api.ts \
        garden-assistant-app/src/app/shared/services/watering.service.ts \
        garden-assistant-app/src/app/shared/services/watering.store.ts \
        garden-assistant-app/src/app/features/calendar/calendar-watering-today/ \
        garden-assistant-app/src/app/features/calendar/calendar.ts \
        garden-assistant-app/src/app/features/calendar/calendar.html \
        garden-assistant-app/public/i18n/fr.json \
        garden-assistant-app/public/i18n/en.json
git commit -m "feat(US-340): bannière Arrosage aujourd'hui

Co-Authored-By: Claude Sonnet 4.6 <noreply@anthropic.com>"
```

---

## Task 6 — US-341 : DTOs schedule + GetWateringScheduleAsync (TDD)

**Files:**
- Modify: `garden-assistant-api/DTOs/Watering/WateringScheduleDto.cs` (remplacer le stub)
- Create: `garden-assistant-api/DTOs/Watering/BedWateringDto.cs`
- Create: `garden-assistant-api/DTOs/Watering/PlantWateringDto.cs`
- Create: `garden-assistant-tests/Watering/WateringServiceScheduleTests.cs`
- Modify: `garden-assistant-api/Services/Watering/WateringService.cs`

- [ ] **Step 1 : Remplacer les DTOs schedule**

```csharp
// garden-assistant-api/DTOs/Watering/PlantWateringDto.cs
using GardenAssistant.Data.Entities.Enums;
namespace GardenAssistant.DTOs.Watering;
public record PlantWateringDto(Guid PlantId, string PlantName, WaterNeeds WaterNeeds, int TimesPerWeek, DayOfWeek[] RecommendedDays, int? WaterAmountMl);

// garden-assistant-api/DTOs/Watering/BedWateringDto.cs
using GardenAssistant.Data.Entities.Enums;
namespace GardenAssistant.DTOs.Watering;
public record BedWateringDto(Guid? BedId, string BedName, bool IsPersonalPlants, SoilType? SoilType, bool HasMulch, List<PlantWateringDto> Plants);

// garden-assistant-api/DTOs/Watering/WateringScheduleDto.cs  (remplacer le stub)
namespace GardenAssistant.DTOs.Watering;
public record WateringScheduleDto(List<BedWateringDto> Beds);
```

- [ ] **Step 2 : Écrire les tests**

```csharp
// garden-assistant-tests/Watering/WateringServiceScheduleTests.cs
using GardenAssistant.Data.Entities;
using GardenAssistant.Data.Entities.Enums;
using GardenAssistant.Services.Watering;
using GardenAssistant.Tests.Infrastructure;
using Shouldly;

namespace GardenAssistant.Tests.Watering;

public class WateringServiceScheduleTests : DatabaseTestBase
{
    private readonly WateringService _sut;
    private readonly Guid _userId = Guid.NewGuid();
    private readonly Guid _gardenId = Guid.NewGuid();

    public WateringServiceScheduleTests()
    {
        DbContext.Users.Add(new User { Id = _userId, Email = "test@example.com" });
        DbContext.Gardens.Add(new Garden { Id = _gardenId, Name = "Test", UserId = _userId, CreatedAtUtc = DateTime.UtcNow });
        DbContext.SaveChanges();
        _sut = new WateringService(DbContext, new WateringCalculator());
    }

    [Fact]
    public async Task GetWateringScheduleAsync_WhenNoBeds_ShouldReturnEmptyBeds()
    {
        var result = await _sut.GetWateringScheduleAsync(_userId, halfMonth: 13, source: "gardenPlants");
        result.Beds.ShouldBeEmpty();
    }

    [Fact]
    public async Task GetWateringScheduleAsync_WhenBedHasPlants_ShouldReturnBedWithFrequency()
    {
        var plant = new Plant { Id = Guid.NewGuid(), Key = "tomate", Name = "Tomate", WaterNeeds = WaterNeeds.High };
        var guild = new Guild { Id = Guid.NewGuid(), UserId = _userId, Name = "G" };
        var bed = new Planting { Id = Guid.NewGuid(), GardenId = _gardenId, UserId = _userId, Name = "Planche", GuildId = guild.Id, CreatedAtUtc = DateTime.UtcNow };
        DbContext.Plants.Add(plant);
        DbContext.Guilds.Add(guild);
        DbContext.Plantings.Add(bed);
        DbContext.GuildPlants.Add(new GuildPlant { GuildId = guild.Id, PlantId = plant.Id });
        await DbContext.SaveChangesAsync();

        var result = await _sut.GetWateringScheduleAsync(_userId, halfMonth: 13, source: "gardenPlants");

        result.Beds.Count.ShouldBe(1);
        result.Beds[0].BedId.ShouldBe(bed.Id);
        result.Beds[0].Plants.Count.ShouldBe(1);
        result.Beds[0].Plants[0].TimesPerWeek.ShouldBe(5); // High/été
        result.Beds[0].Plants[0].RecommendedDays.Length.ShouldBe(5);
    }

    [Fact]
    public async Task GetWateringScheduleAsync_WhenSourceIsMyPlants_ShouldReturnPersonalPlantsBed()
    {
        var user = await DbContext.Users.FindAsync(_userId);
        var plant = new Plant { Id = Guid.NewGuid(), Key = "laitue", Name = "Laitue", WaterNeeds = WaterNeeds.Medium };
        DbContext.Plants.Add(plant);
        DbContext.UserPlants.Add(new UserPlant { UserId = _userId, PlantId = plant.Id });
        await DbContext.SaveChangesAsync();

        var result = await _sut.GetWateringScheduleAsync(_userId, halfMonth: 13, source: "myPlants");

        result.Beds.Count.ShouldBe(1);
        result.Beds[0].IsPersonalPlants.ShouldBeTrue();
        result.Beds[0].BedId.ShouldBeNull();
        result.Beds[0].Plants.Count.ShouldBe(1);
    }

    [Fact]
    public async Task GetWateringScheduleAsync_WhenOtherUser_ShouldReturnEmptyBeds()
    {
        var otherUser = Guid.NewGuid();
        DbContext.Users.Add(new User { Id = otherUser, Email = "other@test.com" });
        DbContext.SaveChanges();

        var result = await _sut.GetWateringScheduleAsync(otherUser, halfMonth: 13, source: "gardenPlants");
        result.Beds.ShouldBeEmpty();
    }
}
```

- [ ] **Step 3 : Lancer les tests (doivent ÉCHOUER)**

```bash
dotnet test garden-assistant-tests --filter "WateringServiceScheduleTests" 2>&1 | tail -5
```

- [ ] **Step 4 : Implémenter GetWateringScheduleAsync dans WateringService**

Remplacer le stub de `GetWateringScheduleAsync` dans `WateringService.cs` :

```csharp
public async Task<WateringScheduleDto> GetWateringScheduleAsync(Guid userId, int halfMonth, string source)
{
    var beds = new List<BedWateringDto>();

    if (source is "gardenPlants" or "all")
    {
        beds.AddRange(await BuildGardenBedsAsync(userId, halfMonth));
    }

    if (source is "myPlants" or "all")
    {
        var personalBed = await BuildPersonalPlantsBedAsync(userId, halfMonth);
        if (personalBed.Plants.Count > 0) { beds.Add(personalBed); }
    }

    return new WateringScheduleDto(beds);
}

private async Task<List<BedWateringDto>> BuildGardenBedsAsync(Guid userId, int halfMonth)
{
    var (plantings, plantsById) = await LoadGardenDataAsync(userId);

    return plantings
        .Where(p => p.GuildId.HasValue)
        .Select(p =>
        {
            var plants = dbContext.GuildPlants
                .Where(gp => gp.GuildId == p.GuildId!.Value)
                .Select(gp => gp.PlantId)
                .AsEnumerable()
                .Where(plantsById.ContainsKey)
                .Select(id => BuildPlantWateringDto(plantsById[id], halfMonth, null, false))
                .ToList();

            return new BedWateringDto(p.Id, p.Name, false, null, false, plants);
        })
        .ToList();
}

private async Task<BedWateringDto> BuildPersonalPlantsBedAsync(Guid userId, int halfMonth)
{
    var userPlantIds = await dbContext.UserPlants
        .Where(up => up.UserId == userId)
        .Select(up => up.PlantId)
        .ToListAsync();

    var plants = await dbContext.Plants
        .Where(p => userPlantIds.Contains(p.Id))
        .ToListAsync();

    var plantDtos = plants
        .Select(p => BuildPlantWateringDto(p, halfMonth, null, false))
        .ToList();

    return new BedWateringDto(null, "MyPlants", true, null, false, plantDtos);
}

private PlantWateringDto BuildPlantWateringDto(
    Data.Entities.Plant plant,
    int halfMonth,
    Data.Entities.Enums.SoilType? soilType,
    bool hasMulch)
{
    var freq = calculator.CalculateFrequency(plant.WaterNeeds, halfMonth, soilType, hasMulch);
    return new PlantWateringDto(plant.Id, plant.Name, plant.WaterNeeds, freq.TimesPerWeek, freq.RecommendedDays, null);
}
```

- [ ] **Step 5 : Lancer les tests (doivent PASSER)**

```bash
dotnet test garden-assistant-tests --filter "WateringServiceScheduleTests" 2>&1 | tail -5
```

- [ ] **Step 6 : Commit**

```bash
git add garden-assistant-api/DTOs/Watering/ \
        garden-assistant-api/Services/Watering/WateringService.cs \
        garden-assistant-tests/Watering/WateringServiceScheduleTests.cs
git commit -m "feat(US-341): WateringService.GetWateringScheduleAsync

Co-Authored-By: Claude Sonnet 4.6 <noreply@anthropic.com>"
```

---

## Task 7 — US-341 : Endpoint /watering/schedule + sérialisation enums

**Files:**
- Modify: `garden-assistant-api/Controllers/CalendarController.cs`
- Modify: `garden-assistant-api/Program.cs`

- [ ] **Step 1 : Configurer JsonStringEnumConverter globalement**

Dans `garden-assistant-api/Program.cs`, trouver `builder.Services.AddControllers()` et remplacer :
```csharp
builder.Services.AddControllers()
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter()));
```

> Note : cette config s'applique à tous les endpoints. `WaterNeeds`, `SoilType`, `DayOfWeek` seront sérialisés en string. Les enums déjà exposés (`PlantActionType`, `GuildPlantRole`, etc.) doivent être vérifiés côté frontend — s'ils étaient numériques avant, des adaptations peuvent être nécessaires dans `garden-assistant-api.ts`. Vérifier au build Angular.

- [ ] **Step 2 : Ajouter l'endpoint**

Dans `CalendarController.cs`, ajouter après `/watering/today` :
```csharp
[HttpGet("watering/schedule")]
[ProducesResponseType(typeof(WateringScheduleDto), StatusCodes.Status200OK)]
public async Task<IActionResult> GetWateringSchedule(
    [FromQuery][Range(1, 24)] int halfMonth,
    [FromQuery] string source = "all")
{
    var result = await wateringService.GetWateringScheduleAsync(CallerId, halfMonth, source);
    return Ok(result);
}
```

- [ ] **Step 3 : Vérifier la compilation**

```bash
dotnet build garden-assistant-api/garden-assistant-api.csproj 2>&1 | tail -5
dotnet test garden-assistant-tests 2>&1 | tail -5
```
Les deux doivent réussir.

- [ ] **Step 4 : Commit**

```bash
git add garden-assistant-api/Controllers/CalendarController.cs \
        garden-assistant-api/Program.cs
git commit -m "feat(US-341): endpoint GET /api/calendar/watering/schedule + JsonStringEnumConverter

Co-Authored-By: Claude Sonnet 4.6 <noreply@anthropic.com>"
```

---

## Task 8 — US-341 : Grille hebdomadaire CalendarWateringComponent

**Files:**
- Modify: `garden-assistant-app/src/styles/components/_watering.scss`
- Create: `garden-assistant-app/src/app/features/calendar/calendar-watering/calendar-watering.ts`
- Create: `garden-assistant-app/src/app/features/calendar/calendar-watering/calendar-watering.html`
- Create: `garden-assistant-app/src/app/features/calendar/calendar-watering/calendar-watering.scss`
- Modify: `garden-assistant-app/src/app/features/calendar/calendar.ts`
- Modify: `garden-assistant-app/src/app/features/calendar/calendar.html`

- [ ] **Step 1 : Ajouter les styles de la grille dans `_watering.scss`**

Ajouter après la section bannière :
```scss
// ─── Grille hebdomadaire ───────────────────────────────────────────────────

.watering-week-nav {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 0.75rem 1.25rem;
  border-bottom: 1px solid var(--color-border-subtle);
}

.watering-week-label {
  font-size: 0.9375rem;
  font-weight: 600;
  color: var(--color-text-dark);
}

.watering-grid-wrapper {
  overflow-x: auto;
  -webkit-overflow-scrolling: touch;
}

.watering-grid {
  display: table;
  width: 100%;
  min-width: 380px;
  border-collapse: collapse;
}

.watering-grid__head-row,
.watering-grid__row {
  display: table-row;
}

.watering-grid__name-cell {
  display: table-cell;
  width: 160px;
  min-width: 120px;
  padding: 0.5rem 0.75rem;
  font-size: 0.8125rem;
  font-weight: 500;
  color: var(--color-text);
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
  vertical-align: middle;

  @media (max-width: 767px) {
    width: 100px;
  }
}

.watering-grid__day-header {
  display: table-cell;
  padding: 0.5rem 0.25rem;
  text-align: center;
  font-size: 0.75rem;
  font-weight: 500;
  background: var(--color-parchment);
  vertical-align: middle;
  min-width: 36px;

  &--today {
    background: var(--color-water-bg-medium);
    color: var(--color-water-dark);
    font-weight: 700;
  }
}

.watering-grid__day-cell {
  display: table-cell;
  text-align: center;
  padding: 0.25rem;
  vertical-align: middle;

  .watering-grid__row:nth-child(even) & {
    background: var(--color-parchment);
  }
}

.watering-dot {
  width: 28px;
  height: 28px;
  background: var(--color-water);
  border-radius: 50%;
  margin: 0 auto;
  transition: transform var(--transition-fast, 150ms ease);

  &:hover {
    transform: scale(1.15);
  }
}

// ─── Tableau fréquences saisonnières ───────────────────────────────────────

.watering-freq-table {
  width: 100%;
  border-collapse: collapse;
  font-size: 0.8125rem;

  th {
    text-align: left;
    font-size: 0.75rem;
    font-weight: 600;
    color: var(--color-text-muted);
    text-transform: uppercase;
    letter-spacing: 0.06em;
    padding: 0.375rem 0.75rem;
    border-bottom: 1px solid var(--color-border-subtle);
  }

  td {
    padding: 0.5rem 0.75rem;
    border-bottom: 1px solid var(--color-border-subtle);
  }

  tr:nth-child(even) td {
    background: var(--color-parchment);
  }
}

.water-need-badge {
  display: inline-block;
  padding: 0.125rem 0.5rem;
  border-radius: var(--radius-sm, 6px);
  font-size: 0.75rem;
  font-weight: 500;

  &--high   { background: var(--color-water-bg-medium); color: var(--color-water-dark); }
  &--medium { background: var(--color-water-bg); color: var(--color-water); }
  &--low    { background: var(--color-parchment); color: var(--color-text-muted); }
}
```

- [ ] **Step 2 : Créer CalendarWateringComponent**

```typescript
// garden-assistant-app/src/app/features/calendar/calendar-watering/calendar-watering.ts
import { Component, inject, computed, input, effect } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';
import { faChevronLeft, faChevronRight } from '@fortawesome/free-solid-svg-icons';
import { FaIconComponent } from '@fortawesome/angular-fontawesome';
import { WateringStore } from '../../../shared/services/watering.store';
import { Collapsible } from '../../../shared/ui/collapsible/collapsible';
import { EmptyState } from '../../../shared/ui/empty-state/empty-state';
import { WEEK_DAYS, DayOfWeekStr } from '../../../api/watering.api';

@Component({
  selector: 'app-calendar-watering',
  standalone: true,
  imports: [TranslateModule, FaIconComponent, Collapsible, EmptyState],
  templateUrl: './calendar-watering.html',
  styleUrl: './calendar-watering.scss',
  host: { style: 'display:block' }
})
export class CalendarWatering {
  readonly active = input(false);
  protected readonly store = inject(WateringStore);
  protected readonly faLeft = faChevronLeft;
  protected readonly faRight = faChevronRight;

  protected readonly weekDays = WEEK_DAYS;
  protected readonly todayDayOfWeek = this.getTodayDayOfWeek();

  protected readonly weekDayHeaders = computed(() => {
    const offset = this.store.weekOffset();
    return WEEK_DAYS.map((day, i) => {
      const date = new Date();
      date.setDate(date.getDate() - date.getDay() + 1 + i + offset * 7); // lundi = 0
      return { day, number: date.getDate(), isToday: offset === 0 && day === this.todayDayOfWeek };
    });
  });

  protected readonly beds = computed(() => this.store.scheduleData()?.beds ?? []);

  private readonly loadOnActivation = effect(() => {
    if (this.active()) {
      this.store.scheduleTabActive.set(true);
      this.store.loadSchedule();
    } else {
      this.store.scheduleTabActive.set(false);
    }
  });

  prevWeek(): void {
    if (this.store.weekOffset() > 0) { this.store.weekOffset.update(v => v - 1); this.store.loadSchedule(); }
  }

  nextWeek(): void {
    if (this.store.weekOffset() < 1) { this.store.weekOffset.update(v => v + 1); this.store.loadSchedule(); }
  }

  hasDot(day: DayOfWeekStr, days: DayOfWeekStr[]): boolean {
    return days.includes(day);
  }

  waterNeedClass(waterNeeds: string): string {
    return `water-need-badge--${waterNeeds.toLowerCase()}`;
  }

  private getTodayDayOfWeek(): DayOfWeekStr {
    return WEEK_DAYS[new Date().getDay() === 0 ? 6 : new Date().getDay() - 1];
  }
}
```

```html
<!-- garden-assistant-app/src/app/features/calendar/calendar-watering/calendar-watering.html -->
@if (store.loadingSchedule()) {
  <div class="calendar-loading"><div class="calendar-loading-spinner"></div></div>
} @else if (beds().length === 0) {
  <app-empty-state icon="💧" messageKey="Watering.EmptyState"></app-empty-state>
} @else {
  <!-- Navigation semaine -->
  <div class="watering-week-nav">
    <button class="btn btn-secondary btn-sm"
            [disabled]="store.weekOffset() === 0"
            (click)="prevWeek()"
            [attr.aria-label]="'Watering.PrevWeek' | translate">
      <fa-icon [icon]="faLeft"></fa-icon>
    </button>
    <span class="watering-week-label">
      {{ 'Watering.WeekCurrent' | translate }}
    </span>
    <button class="btn btn-secondary btn-sm"
            [disabled]="store.weekOffset() === 1"
            (click)="nextWeek()"
            [attr.aria-label]="'Watering.NextWeek' | translate">
      <fa-icon [icon]="faRight"></fa-icon>
    </button>
  </div>

  <!-- Grille par lit/planche -->
  @for (bed of beds(); track bed.bedId ?? bed.bedName) {
    <div [style.margin-top]="'1rem'">
      @if (!bed.isPersonalPlants) {
        <div class="calendar-bed-title">{{ bed.bedName }}</div>
      } @else {
        <div class="calendar-bed-title">{{ 'Watering.MyPlantsBed' | translate }}</div>
      }

      <div class="watering-grid-wrapper"
           role="region"
           [attr.aria-label]="'Watering.GridAriaLabel' | translate">
        <div class="watering-grid" role="grid">
          <!-- En-têtes jours -->
          <div class="watering-grid__head-row">
            <div class="watering-grid__name-cell"></div>
            @for (header of weekDayHeaders(); track header.day) {
              <div class="watering-grid__day-header"
                   [class.watering-grid__day-header--today]="header.isToday"
                   role="columnheader">
                {{ header.day.substring(0, 1) }}<br>{{ header.number }}
              </div>
            }
          </div>
          <!-- Lignes plantes -->
          @for (plant of bed.plants; track plant.plantId) {
            <div class="watering-grid__row" role="row">
              <div class="watering-grid__name-cell" role="rowheader">{{ plant.plantName }}</div>
              @for (day of weekDays; track day) {
                <div class="watering-grid__day-cell" role="gridcell">
                  @if (hasDot(day, plant.recommendedDays)) {
                    <div class="watering-dot" [attr.aria-label]="'Arrosage prévu'"></div>
                  }
                </div>
              }
            </div>
          }
        </div>
      </div>
    </div>
  }

  <!-- Fréquences saisonnières -->
  <div style="margin-top: 1.5rem; padding: 0 1rem">
    <app-collapsible>
      <div collapsible-header>
        <div class="section-header">
          <span class="section-header-label" [translate]="'Watering.FrequenciesTitle'"></span>
        </div>
      </div>
      <div collapsible-body>
        <table class="watering-freq-table" [attr.aria-label]="'Watering.FrequenciesTitle' | translate">
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
                    <span class="water-need-badge" [ngClass]="waterNeedClass(plant.waterNeeds)">
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
```

```scss
// garden-assistant-app/src/app/features/calendar/calendar-watering/calendar-watering.scss
// Styles dans _watering.scss
```

- [ ] **Step 3 : Ajouter le tab toggle dans `calendar.ts` et `calendar.html`**

Dans `calendar.ts`, ajouter :
```typescript
import { CalendarWatering } from './calendar-watering/calendar-watering';
// Dans les imports du @Component : ajouter CalendarWatering
// Ajouter propriété :
protected readonly calendarTabOptions: ToggleOption[] = [
  { value: 'actions', labelKey: 'Watering.TabActions', icon: faSeedling },
  { value: 'watering', labelKey: 'Watering.TabWatering', icon: faDroplet },
];
protected readonly activeCalendarTab = signal<'actions' | 'watering'>('actions');
```
Ajouter l'import de `faDroplet` depuis `@fortawesome/free-solid-svg-icons`.

Dans `calendar.html`, dans le `<div class="panel">`, **avant** `.calendar-filter-chips`, insérer :
```html
<div style="padding: 0.75rem 1.25rem; border-bottom: 1px solid var(--color-border-subtle)">
  <app-toggle-group
    [options]="calendarTabOptions"
    [selectedValue]="activeCalendarTab()"
    (valueChange)="activeCalendarTab.set($any($event))">
  </app-toggle-group>
</div>
```

Envelopper les `.calendar-filter-chips` et le contenu Gantt dans :
```html
@if (activeCalendarTab() === 'actions') {
  <!-- tout le contenu Gantt existant -->
}
<app-calendar-watering [active]="activeCalendarTab() === 'watering'"></app-calendar-watering>
```

- [ ] **Step 4 : npm run build (doit PASSER)**

```bash
npm run build --prefix garden-assistant-app 2>&1 | grep -E "error|warning|Error" | head -20
```
Corriger toute erreur avant de continuer.

- [ ] **Step 5 : Commit**

```bash
git add garden-assistant-app/src/styles/components/_watering.scss \
        garden-assistant-app/src/app/features/calendar/calendar-watering/ \
        garden-assistant-app/src/app/features/calendar/calendar.ts \
        garden-assistant-app/src/app/features/calendar/calendar.html
git commit -m "feat(US-341): grille hebdomadaire arrosage et tab toggle

Co-Authored-By: Claude Sonnet 4.6 <noreply@anthropic.com>"
```

---

## Task 9 — US-342 : SoilType sur Planting + migration + calculator

**Files:**
- Modify: `garden-assistant-api/Data/Entities/Planting.cs`
- Modify: `garden-assistant-api/DTOs/Beds/BedDto.cs`
- Modify: `garden-assistant-api/DTOs/Beds/CreateBedRequest.cs`
- Modify: `garden-assistant-api/DTOs/Beds/UpdateBedRequest.cs`
- Modify: `garden-assistant-api/Services/BedService.cs`
- Modify: `garden-assistant-api/Services/Watering/WateringService.cs`
- Modify: `garden-assistant-tests/Watering/WateringCalculatorTests.cs`
- Modify: `garden-assistant-app/src/app/api/garden-assistant-api.ts`
- Modify: `garden-assistant-app/src/app/features/garden/create-bed-dialog/create-bed-dialog.ts`
- Modify: `garden-assistant-app/src/app/features/garden/create-bed-dialog/create-bed-dialog.html`

- [ ] **Step 1 : Ajouter SoilType sur Planting**

```csharp
// garden-assistant-api/Data/Entities/Planting.cs
using GardenAssistant.Data.Entities.Enums;

public class Planting
{
    public Guid Id { get; set; }
    public Guid GardenId { get; set; }
    public Guid UserId { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public DateOnly? PlannedDate { get; set; }
    public Guid? GuildId { get; set; }
    public SoilType? SoilType { get; set; }   // ← nouveau
    public DateTime CreatedAtUtc { get; set; }
}
```

- [ ] **Step 2 : Générer la migration**

```bash
dotnet ef migrations add AddPlantingSoilType \
  --project garden-assistant-api \
  --startup-project garden-assistant-api
```
Vérifier que la migration ne contient que la colonne `soil_type`.

- [ ] **Step 3 : Écrire les tests du calculator avec sol**

Ajouter dans `garden-assistant-tests/Watering/WateringCalculatorTests.cs` :
```csharp
// --- Coefficients sol ---

[Fact]
public void CalculateFrequency_WhenSandySoilAndLowSummer_ShouldIncreaseFrequency()
{
    // Low/été base=2, Sandy ×1.3 → round(2.6)=3
    var result = _sut.CalculateFrequency(WaterNeeds.Low, halfMonth: 13, soilType: SoilType.Sandy);
    result.TimesPerWeek.ShouldBe(3);
}

[Fact]
public void CalculateFrequency_WhenClaySoilAndHighSummer_ShouldDecreaseFrequency()
{
    // High/été base=5, Clay ×0.7 → round(3.5)=4
    var result = _sut.CalculateFrequency(WaterNeeds.High, halfMonth: 13, soilType: SoilType.Clay);
    result.TimesPerWeek.ShouldBe(4);
}

[Fact]
public void CalculateFrequency_WhenPeatySoilAndMediumSummer_ShouldDecreaseFrequency()
{
    // Medium/été base=4, Peaty ×0.8 → round(3.2)=3
    var result = _sut.CalculateFrequency(WaterNeeds.Medium, halfMonth: 13, soilType: SoilType.Peaty);
    result.TimesPerWeek.ShouldBe(3);
}

[Fact]
public void CalculateFrequency_WhenChalkySoilAndLowSpring_ShouldIncreaseFrequency()
{
    // Low/printemps base=1, Chalky ×1.2 → round(1.2)=1 (minimum 1)
    var result = _sut.CalculateFrequency(WaterNeeds.Low, halfMonth: 7, soilType: SoilType.Chalky);
    result.TimesPerWeek.ShouldBe(1);
}

[Fact]
public void CalculateFrequency_WhenLoamSoil_ShouldKeepBaseFrequency()
{
    var withLoam   = _sut.CalculateFrequency(WaterNeeds.Medium, halfMonth: 13, soilType: SoilType.Loam);
    var withNull   = _sut.CalculateFrequency(WaterNeeds.Medium, halfMonth: 13);
    withLoam.TimesPerWeek.ShouldBe(withNull.TimesPerWeek);
}

[Fact]
public void CalculateFrequency_WhenNullSoil_ShouldUseDefaultCoefficient()
{
    var result = _sut.CalculateFrequency(WaterNeeds.Medium, halfMonth: 13, soilType: null);
    result.TimesPerWeek.ShouldBe(4); // Medium/été base=4, ×1.0
}

[Theory]
[InlineData(SoilType.Sandy,  3)]   // 2 × 1.3 = 2.6 → 3
[InlineData(SoilType.Rocky,  3)]   // 2 × 1.3 = 2.6 → 3
[InlineData(SoilType.Silty,  2)]   // 2 × 0.9 = 1.8 → 2
[InlineData(SoilType.Chalky, 2)]   // 2 × 1.2 = 2.4 → 2
[InlineData(SoilType.Clay,   1)]   // 2 × 0.7 = 1.4 → 1
[InlineData(SoilType.Peaty,  2)]   // 2 × 0.8 = 1.6 → 2
public void CalculateFrequency_SoilCoefficients_LowSummer(SoilType soil, int expected)
{
    var result = _sut.CalculateFrequency(WaterNeeds.Low, halfMonth: 13, soilType: soil);
    result.TimesPerWeek.ShouldBe(expected);
}

[Fact]
public void CalculateFrequency_WhenSoilAdjusted_RecommendedDaysLength_ShouldMatchTimesPerWeek()
{
    var result = _sut.CalculateFrequency(WaterNeeds.Low, halfMonth: 13, soilType: SoilType.Sandy);
    result.RecommendedDays.Length.ShouldBe(result.TimesPerWeek);
}
```

- [ ] **Step 4 : Lancer les tests (doivent PASSER — la logique sol est déjà dans le calculator)**

```bash
dotnet test garden-assistant-tests --filter "WateringCalculatorTests" 2>&1 | tail -5
```

- [ ] **Step 5 : Mettre à jour BedDto, CreateBedRequest, UpdateBedRequest**

```csharp
// garden-assistant-api/DTOs/Beds/BedDto.cs
using GardenAssistant.Data.Entities.Enums;
namespace GardenAssistant.DTOs.Beds;
public record BedDto(Guid Id, string Name, Guid? GuildId, List<Guid> PlantIds, SoilType? SoilType, bool HasMulch);

// garden-assistant-api/DTOs/Beds/CreateBedRequest.cs
using GardenAssistant.Data.Entities.Enums;
namespace GardenAssistant.DTOs.Beds;
public record CreateBedRequest(string? Name, SoilType? SoilType, bool HasMulch = false);

// garden-assistant-api/DTOs/Beds/UpdateBedRequest.cs
using GardenAssistant.Data.Entities.Enums;
namespace GardenAssistant.DTOs.Beds;
public record UpdateBedRequest(string? Name, SoilType? SoilType, bool HasMulch = false);
```

- [ ] **Step 6 : Mettre à jour BedService**

Dans `BedService.cs`, mettre à jour les 4 usages de `BedDto` pour inclure `SoilType` et `HasMulch`, et persister les nouvelles valeurs dans `CreateAsync` et `UpdateAsync` :

```csharp
// Dans GetByGardenIdAsync, remplacer la projection :
return beds.Select(b => new BedDto(
    b.Id, b.Name, b.GuildId,
    b.GuildId.HasValue ? plantIdsByGuild.GetValueOrDefault(b.GuildId.Value, []) : [],
    b.SoilType, b.HasMulch));

// Dans CreateAsync, ajouter lors de la création du bed :
var bed = new Planting
{
    Id = Guid.NewGuid(), GardenId = gardenId, UserId = userId,
    Name = bedName, GuildId = guild.Id, CreatedAtUtc = DateTime.UtcNow,
    SoilType = request.SoilType, HasMulch = request.HasMulch
};
// return new BedDto(bed.Id, bed.Name, guild.Id, [], bed.SoilType, bed.HasMulch);

// Dans UpdateAsync :
bed.SoilType = request.SoilType;
// return new BedDto(bed.Id, bed.Name, bed.GuildId, plantIds, bed.SoilType, bed.HasMulch);
```

- [ ] **Step 7 : Mettre à jour WateringService pour utiliser le sol de la planche**

Dans `WateringService.BuildGardenBedsAsync`, passer `p.SoilType` au `BuildPlantWateringDto` :
```csharp
var plants = dbContext.GuildPlants
    .Where(gp => gp.GuildId == p.GuildId!.Value)
    .Select(gp => gp.PlantId)
    .AsEnumerable()
    .Where(plantsById.ContainsKey)
    .Select(id => BuildPlantWateringDto(plantsById[id], halfMonth, p.SoilType, p.HasMulch))
    .ToList();

return new BedWateringDto(p.Id, p.Name, false, p.SoilType, p.HasMulch, plants);
```

Et dans `BuildBedTodayDto` (pour /today), passer également `planting.SoilType` et `planting.HasMulch` :
```csharp
var freq = calculator.CalculateFrequency(plant.WaterNeeds, halfMonth, planting.SoilType, planting.HasMulch);
```

- [ ] **Step 8 : Vérifier la compilation backend**

```bash
dotnet build garden-assistant-api/garden-assistant-api.csproj 2>&1 | tail -5
dotnet test garden-assistant-tests 2>&1 | tail -5
```

- [ ] **Step 9 : Mettre à jour le client TypeScript**

Dans `garden-assistant-app/src/app/api/garden-assistant-api.ts` :

Ajouter l'enum `SoilType` après `WaterNeeds` (ligne ~1218) :
```typescript
export enum SoilType {
    Sandy = "Sandy",
    Silty = "Silty",
    Clay = "Clay",
    Loam = "Loam",
    Chalky = "Chalky",
    Peaty = "Peaty",
    Rocky = "Rocky"
}
```

Mettre à jour `BedDto` (ligne ~996) :
```typescript
export interface BedDto {
    id?: string;
    name?: string;
    guildId?: string | undefined;
    plantIds?: string[];
    soilType?: SoilType | undefined;   // ← nouveau
    hasMulch?: boolean;                // ← nouveau
}
```

Mettre à jour `CreateBedRequest` et `UpdateBedRequest` :
```typescript
export interface CreateBedRequest {
    name?: string | undefined;
    soilType?: SoilType | undefined;
    hasMulch?: boolean;
}

export interface UpdateBedRequest {
    name?: string | undefined;
    soilType?: SoilType | undefined;
    hasMulch?: boolean;
}
```

- [ ] **Step 10 : Ajouter le select SoilType dans CreateBedDialog**

Dans `create-bed-dialog.ts`, mettre à jour les interfaces et ajouter un signal :
```typescript
export interface CreateBedDialogData {
  mode: 'create' | 'edit';
  name?: string;
  soilType?: string;
}

export interface CreateBedDialogResult {
  name?: string;
  soilType?: string;
}

// Dans la classe :
readonly soilType = signal(this.data.soilType ?? '');

readonly soilTypeOptions = [
  { value: '', labelKey: 'Bed.SoilType.None' },
  { value: 'Sandy',  labelKey: 'Bed.SoilType.Sandy' },
  { value: 'Loam',   labelKey: 'Bed.SoilType.Loam' },
  { value: 'Clay',   labelKey: 'Bed.SoilType.Clay' },
  { value: 'Silty',  labelKey: 'Bed.SoilType.Silty' },
  { value: 'Chalky', labelKey: 'Bed.SoilType.Chalky' },
  { value: 'Peaty',  labelKey: 'Bed.SoilType.Peaty' },
  { value: 'Rocky',  labelKey: 'Bed.SoilType.Rocky' },
];

save(): void {
  this.dialogRef.close({
    name: this.name().trim() || undefined,
    soilType: this.soilType() || undefined,
  } as CreateBedDialogResult);
}
```

Dans `create-bed-dialog.html`, ajouter après le champ nom :
```html
<div>
  <label class="block text-sm font-medium mb-1" [translate]="'Bed.SoilTypeLabel'"></label>
  <select class="form-input"
          [value]="soilType()"
          (change)="soilType.set($any($event.target).value)">
    @for (opt of soilTypeOptions; track opt.value) {
      <option [value]="opt.value">{{ opt.labelKey | translate }}</option>
    }
  </select>
</div>
```

Ajouter les clés i18n dans `fr.json` :
```json
"Bed": {
  "SoilTypeLabel": "Type de sol de la planche",
  "SoilType": {
    "None": "Non renseigné",
    "Sandy": "Sableux",
    "Loam": "Limoneux (loam)",
    "Clay": "Argileux",
    "Silty": "Limoneux fin",
    "Chalky": "Calcaire",
    "Peaty": "Tourbeux",
    "Rocky": "Rocailleux"
  }
}
```

Et dans `en.json` :
```json
"Bed": {
  "SoilTypeLabel": "Bed soil type",
  "SoilType": {
    "None": "Not specified",
    "Sandy": "Sandy",
    "Loam": "Loam",
    "Clay": "Clay",
    "Silty": "Silty",
    "Chalky": "Chalky",
    "Peaty": "Peaty",
    "Rocky": "Rocky"
  }
}
```

- [ ] **Step 11 : npm run build (doit PASSER)**

```bash
npm run build --prefix garden-assistant-app 2>&1 | grep -E "^.*(error|Error)" | head -20
```

- [ ] **Step 12 : Appliquer la migration**

```bash
dotnet ef database update --project garden-assistant-api --startup-project garden-assistant-api
```

- [ ] **Step 13 : Commit**

```bash
git add garden-assistant-api/Data/Entities/Planting.cs \
        garden-assistant-api/DTOs/Beds/ \
        garden-assistant-api/Services/BedService.cs \
        garden-assistant-api/Services/Watering/WateringService.cs \
        garden-assistant-api/Migrations/ \
        garden-assistant-tests/Watering/WateringCalculatorTests.cs \
        garden-assistant-app/src/app/api/garden-assistant-api.ts \
        garden-assistant-app/src/app/features/garden/create-bed-dialog/ \
        garden-assistant-app/public/i18n/
git commit -m "feat(US-342): SoilType sur planche — ajustement fréquence arrosage

Co-Authored-By: Claude Sonnet 4.6 <noreply@anthropic.com>"
```

---

## Task 10 — US-343 : HasMulch sur Planting + migration + calculator

**Files:**
- Modify: `garden-assistant-api/Data/Entities/Planting.cs`
- Modify: `garden-assistant-tests/Watering/WateringCalculatorTests.cs`
- Modify: `garden-assistant-app/src/app/features/garden/create-bed-dialog/create-bed-dialog.ts`
- Modify: `garden-assistant-app/src/app/features/garden/create-bed-dialog/create-bed-dialog.html`

- [ ] **Step 1 : Ajouter HasMulch sur Planting**

```csharp
// Dans garden-assistant-api/Data/Entities/Planting.cs, ajouter :
public bool HasMulch { get; set; } = false;   // ← nouveau (après SoilType)
```

- [ ] **Step 2 : Générer la migration**

```bash
dotnet ef migrations add AddPlantingHasMulch \
  --project garden-assistant-api \
  --startup-project garden-assistant-api
```
Vérifier que la migration ne contient que la colonne `has_mulch`.

- [ ] **Step 3 : Écrire les tests paillage**

Ajouter dans `WateringCalculatorTests.cs` :
```csharp
// --- Coefficient paillage ---

[Fact]
public void CalculateFrequency_WhenMulchAndHighSummer_ShouldReduceFrequency()
{
    // High/été base=5, hasMulch ×0.6 → round(3)=3
    var result = _sut.CalculateFrequency(WaterNeeds.High, halfMonth: 13, hasMulch: true);
    result.TimesPerWeek.ShouldBe(3);
}

[Fact]
public void CalculateFrequency_WhenMulchAndLowWinter_ShouldNotGoBelowZero()
{
    // Low/hiver base=1, hasMulch ×0.6 → round(0.6)=1 (minimum 0 en hiver, mais round donne 1)
    var result = _sut.CalculateFrequency(WaterNeeds.Low, halfMonth: 2, hasMulch: true);
    result.TimesPerWeek.ShouldBeGreaterThanOrEqualTo(0);
}

[Fact]
public void CalculateFrequency_WhenMulchAndLowSpring_ShouldKeepMinimum1()
{
    // Low/printemps base=1, hasMulch ×0.6 → round(0.6)=1 (minimum 1 en saison active)
    var result = _sut.CalculateFrequency(WaterNeeds.Low, halfMonth: 7, hasMulch: true);
    result.TimesPerWeek.ShouldBe(1);
}

[Fact]
public void CalculateFrequency_WhenSandySoilAndMulch_ShouldCombineCoefficients()
{
    // Low/été base=2, Sandy ×1.3 → 2.6, ×0.6 → 1.56 → round(2)=2
    var result = _sut.CalculateFrequency(WaterNeeds.Low, halfMonth: 13, soilType: SoilType.Sandy, hasMulch: true);
    result.TimesPerWeek.ShouldBe(2);
}

[Fact]
public void CalculateFrequency_WhenMulch_RecommendedDaysLength_ShouldMatchTimesPerWeek()
{
    var result = _sut.CalculateFrequency(WaterNeeds.High, halfMonth: 13, hasMulch: true);
    result.RecommendedDays.Length.ShouldBe(result.TimesPerWeek);
}

[Fact]
public void CalculateFrequency_WhenNoMulch_ShouldNotAffectFrequency()
{
    var withMulch    = _sut.CalculateFrequency(WaterNeeds.Medium, halfMonth: 13, hasMulch: true);
    var withoutMulch = _sut.CalculateFrequency(WaterNeeds.Medium, halfMonth: 13, hasMulch: false);
    withMulch.TimesPerWeek.ShouldBeLessThanOrEqualTo(withoutMulch.TimesPerWeek);
}
```

- [ ] **Step 4 : Lancer les tests (doivent PASSER — logique déjà implémentée)**

```bash
dotnet test garden-assistant-tests --filter "WateringCalculatorTests" 2>&1 | tail -5
```

- [ ] **Step 5 : Ajouter le toggle HasMulch dans le dialog**

Dans `create-bed-dialog.ts`, ajouter :
```typescript
readonly hasMulch = signal(this.data.hasMulch ?? false);

// Mettre à jour CreateBedDialogData / Result :
export interface CreateBedDialogData {
  mode: 'create' | 'edit';
  name?: string;
  soilType?: string;
  hasMulch?: boolean;   // ← nouveau
}
export interface CreateBedDialogResult {
  name?: string;
  soilType?: string;
  hasMulch?: boolean;   // ← nouveau
}

// Mettre à jour save() :
save(): void {
  this.dialogRef.close({
    name: this.name().trim() || undefined,
    soilType: this.soilType() || undefined,
    hasMulch: this.hasMulch(),
  } as CreateBedDialogResult);
}
```

Dans `create-bed-dialog.html`, ajouter après le select SoilType :
```html
<div class="flex items-center gap-2">
  <input type="checkbox"
         id="hasMulch"
         [checked]="hasMulch()"
         (change)="hasMulch.set($any($event.target).checked)" />
  <label for="hasMulch" class="text-sm" [translate]="'Bed.HasMulchLabel'"></label>
</div>
```

Ajouter dans `fr.json` (dans le bloc `Bed`) :
```json
"HasMulchLabel": "Planche paillée"
```
Et dans `en.json` :
```json
"HasMulchLabel": "Mulched bed"
```

- [ ] **Step 6 : Appliquer la migration**

```bash
dotnet ef database update --project garden-assistant-api --startup-project garden-assistant-api
```

- [ ] **Step 7 : npm run build**

```bash
npm run build --prefix garden-assistant-app 2>&1 | grep -E "^.*(error|Error)" | head -20
```

- [ ] **Step 8 : Commit**

```bash
git add garden-assistant-api/Data/Entities/Planting.cs \
        garden-assistant-api/Migrations/ \
        garden-assistant-tests/Watering/WateringCalculatorTests.cs \
        garden-assistant-app/src/app/features/garden/create-bed-dialog/ \
        garden-assistant-app/public/i18n/
git commit -m "feat(US-343): HasMulch sur planche — réduction 40% fréquence arrosage

Co-Authored-By: Claude Sonnet 4.6 <noreply@anthropic.com>"
```

---

## Task 11 — US-344 : WaterAmountMl (optionnel)

**Files:**
- Modify: `garden-assistant-api/Data/Entities/Plant.cs`
- Modify: `garden-assistant-api/DTOs/Plants/PlantDto.cs`
- Modify seed JSON files (10 plantes)
- Modify: `garden-assistant-api/Services/Watering/WateringService.cs`
- Modify: `garden-assistant-app/src/app/api/garden-assistant-api.ts`
- Modify: `garden-assistant-app/src/app/features/calendar/calendar-watering/calendar-watering.html`

- [ ] **Step 1 : Ajouter WaterAmountMl sur Plant**

```csharp
// garden-assistant-api/Data/Entities/Plant.cs — ajouter dans la classe :
public int? WaterAmountMl { get; set; }
```

- [ ] **Step 2 : Générer la migration**

```bash
dotnet ef migrations add AddPlantWaterAmount \
  --project garden-assistant-api \
  --startup-project garden-assistant-api
```

- [ ] **Step 3 : Mettre à jour PlantDto**

Dans `garden-assistant-api/DTOs/Plants/PlantDto.cs`, ajouter la propriété `WaterAmountMl` dans le record.
Chercher le fichier avec : `cat garden-assistant-api/DTOs/Plants/PlantDto.cs`
Ajouter `int? WaterAmountMl` parmi les propriétés existantes.

- [ ] **Step 4 : Ajouter waterAmountMl dans les seed JSON**

Chercher les fichiers de seed avec `ls garden-assistant-api/Data/Seeds/`. Localiser les entrées des 10 plantes suivantes et ajouter `"waterAmountMl"` :

| Plante (chercher par `"key"`) | waterAmountMl |
|---|---|
| tomate / tomato | 1500 |
| courgette / zucchini | 2000 |
| laitue / lettuce | 500 |
| basilic / basil | 400 |
| carotte / carrot | 500 |
| poivron / pepper | 1000 |
| haricot / bean | 800 |
| concombre / cucumber | 1500 |
| fraise / strawberry | 400 |
| radis / radish | 300 |

- [ ] **Step 5 : Exposer WaterAmountMl dans WateringService**

Dans `WateringService.BuildPlantWateringDto`, inclure `WaterAmountMl` :
```csharp
return new PlantWateringDto(plant.Id, plant.Name, plant.WaterNeeds, freq.TimesPerWeek, freq.RecommendedDays, plant.WaterAmountMl);
```

- [ ] **Step 6 : Mettre à jour le client TypeScript**

Dans `garden-assistant-api.ts`, ajouter dans `PlantDto` :
```typescript
waterAmountMl?: number | undefined;
```

- [ ] **Step 7 : Afficher les quantités dans la grille**

Dans `calendar-watering.html`, dans `.watering-dot`, ajouter un tooltip ou une étiquette si `plant.waterAmountMl` est défini.

Modifier la cellule du dot pour afficher la quantité sous le cercle (si présente) :
```html
@if (hasDot(day, plant.recommendedDays)) {
  <div style="display:flex; flex-direction:column; align-items:center; gap:2px">
    <div class="watering-dot" aria-label="Arrosage prévu"></div>
    @if (plant.waterAmountMl) {
      <span style="font-size:0.625rem; color:var(--color-water-dark)">
        {{ plant.waterAmountMl >= 1000
            ? ('Watering.WaterAmountL' | translate : { amount: (plant.waterAmountMl / 1000).toFixed(1) })
            : ('Watering.WaterAmountMl' | translate : { amount: plant.waterAmountMl }) }}
      </span>
    }
  </div>
}
```

- [ ] **Step 8 : Appliquer la migration**

```bash
dotnet ef database update --project garden-assistant-api --startup-project garden-assistant-api
```

- [ ] **Step 9 : npm run build + tous les tests**

```bash
npm run build --prefix garden-assistant-app 2>&1 | grep -E "^.*(error|Error)" | head -20
dotnet test garden-assistant-tests 2>&1 | tail -10
```

- [ ] **Step 10 : Commit**

```bash
git add garden-assistant-api/Data/Entities/Plant.cs \
        garden-assistant-api/DTOs/Plants/PlantDto.cs \
        garden-assistant-api/Data/Seeds/ \
        garden-assistant-api/Services/Watering/WateringService.cs \
        garden-assistant-api/Migrations/ \
        garden-assistant-app/src/app/api/garden-assistant-api.ts \
        garden-assistant-app/src/app/features/calendar/calendar-watering/calendar-watering.html
git commit -m "feat(US-344): WaterAmountMl — quantités d'eau indicatives sur 10 plantes

Co-Authored-By: Claude Sonnet 4.6 <noreply@anthropic.com>"
```

---

## Auto-review — couverture de la spec

| Exigence spec | Task couvrant |
|---|---|
| Matrice Low/Medium/High × 4 saisons (valeurs entières validées) | Task 1 |
| RecommendedDays priorité Sam/Dim/Ven, max 2 consécutifs | Task 1 |
| Coefficients sol (7 types) | Task 1 + Task 9 |
| Coefficient paillage ×0.6 | Task 1 + Task 10 |
| WaterAmountMl seed 10 plantes | Task 11 |
| DTOs WateringTodayDto / WateringScheduleDto | Task 2 + Task 6 |
| Endpoint /watering/today | Task 3 |
| Endpoint /watering/schedule + source param | Task 7 |
| JsonStringEnumConverter (DayOfWeek en string) | Task 7 |
| DI WateringCalculator (singleton) + WateringService (scoped) | Task 1 + Task 2 |
| Tokens CSS --color-water-* | Task 4 |
| `_watering.scss` (vars, pas de hex) | Task 4 + Task 8 |
| Bannière CalendarWateringToday | Task 5 |
| Tab toggle Actions / Arrosage | Task 8 |
| Grille 7 colonnes + dots bleus | Task 8 |
| Toggle semaine courante/suivante | Task 8 |
| Section fréquences saisonnières (collapsible) | Task 8 |
| WateringStore (lazy, effect sur sourceFilter) | Task 5 |
| SoilType sur Planting + migration | Task 9 |
| HasMulch sur Planting + migration | Task 10 |
| Select SoilType dans dialog planche | Task 9 |
| Toggle HasMulch dans dialog planche | Task 10 |
| i18n fr + en (toutes les clés Watering.* et Bed.SoilType.*) | Task 5 + Task 9 + Task 10 |
| npm run build sans erreur après chaque story | Tasks 5, 8, 9, 10, 11 |
| Mobile-first (watering-grid overflow-x auto) | Task 8 |
| Accessibilité (role, aria-label, aria-live) | Tasks 5 + 8 |
