# Spec — E25 : Calendrier d'arrosage

**Date :** 2026-04-05
**Épique :** E25 — Calendrier d'arrosage
**Stories :** US-339, US-340, US-341, US-342, US-343, US-344

---

## Objectif

Fournir un planning d'arrosage hebdomadaire calculé automatiquement pour chaque jardin, à partir des besoins en eau des plantes, de la saison, du type de sol de la planche, et du paillage. Le résultat est affiché dans la page calendrier existante sous forme de grille 7 jours.

---

## Données botaniques validées (plant-expert)

### Matrice de fréquence (TimesPerWeek)

| WaterNeeds | Hiver (dm 1-4, 23-24) | Printemps (dm 5-10) | Été (dm 11-16) | Automne (dm 17-22) |
|---|---|---|---|---|
| Low    | 1 | 1 | 2 | 1 |
| Medium | 1 | 2 | 4 | 2 |
| High   | 2 | 3 | 5 | 3 |

> High/été = 5 (pas 7). Le quotidien est trop agressif pour la plupart des plantes High (courgette, basilic notamment).

### Coefficients sol (sur `Planting.SoilType`)

| SoilType | Coefficient |
|---|---|
| Sandy  | ×1.3 |
| Loam   | ×1.0 |
| Clay   | ×0.7 |
| Silty  | ×0.9 |
| Chalky | ×1.2 |
| Peaty  | ×0.8 |
| Rocky  | ×1.3 |

Arrondi à l'entier le plus proche. Minimum 1 en saison active (printemps/été/automne). En hiver, peut descendre à 0 pour Low si le coefficient l'impose.

### Coefficient paillage

`HasMulch = true` → ×0.6 (réduction 40%). Appliqué après le coefficient sol. Minimum 1 en saison active.

### WaterAmountMl — seed data (10 plantes courantes)

| Plante | WaterAmountMl |
|---|---|
| Tomate     | 1500 |
| Courgette  | 2000 |
| Laitue     | 500  |
| Basilic    | 400  |
| Carotte    | 500  |
| Poivron    | 1000 |
| Haricot    | 800  |
| Concombre  | 1500 |
| Fraise     | 400  |
| Radis      | 300  |

---

## RecommendedDays — table de correspondance

Les jours recommandés sont une table fixe (pas un calcul dynamique). Priorité : **Samedi → Dimanche → Vendredi**, puis jours restants par espacement maximal. Règle : éviter plus de 2 jours consécutifs quand possible.

| TimesPerWeek | RecommendedDays | Gaps |
|---|---|---|
| 1 | [Sam] | — |
| 2 | [Mer, Sam] | 3j, 4j |
| 3 | [Mer, Sam, Dim] | 3j, 1j, 3j |
| 4 | [Mar, Jeu, Sam, Dim] | 2j, 2j, 1j, 2j |
| 5 | [Lun, Mer, Ven, Sam, Dim] | 2j, 2j, 1j, 1j, 1j |
| 7 | [Lun, Mar, Mer, Jeu, Ven, Sam, Dim] | — |

> Rationale : le samedi et dimanche sont prioritaires car les jardiniers sont disponibles. Le vendredi vient ensuite. Pour 4x, le vendredi est sacrifié pour éviter Ven+Sam+Dim = 3 jours consécutifs avec un trou de 4 jours ensuite.

---

## Architecture backend

### Nouvelles migrations EF Core

1. `AddPlantingWateringFields` — ajoute sur `Planting` :
   - `SoilType? SoilType` (enum nullable)
   - `bool HasMulch` (défaut `false`)

2. `AddPlantWaterAmount` — ajoute sur `Plant` :
   - `int? WaterAmountMl` (nullable)

### Services (sous-dossier `Services/Watering/`)

**`IWateringCalculator` / `WateringCalculator`** — service **synchrone** (aucun accès BDD) :

```csharp
public interface IWateringCalculator
{
    WateringFrequency CalculateFrequency(
        WaterNeeds waterNeeds,
        int halfMonth,
        SoilType? soilType = null,
        bool hasMulch = false);
}

public record WateringFrequency(
    int TimesPerWeek,
    DayOfWeek[] RecommendedDays,
    string? Notes);
```

Logique interne :
1. Matrice `WaterNeeds × saison` → `TimesPerWeek` de base
2. Application du coefficient sol (table de correspondance)
3. Application du coefficient paillage si `hasMulch`
4. Arrondi + clamp minimum
5. Lecture de la table `RecommendedDays` selon le `TimesPerWeek` final

**`IWateringService` / `WateringService`** — service async, orchestre BDD + calculator :

```csharp
public interface IWateringService
{
    Task<WateringTodayDto> GetWateringTodayAsync(Guid userId, DateOnly today);
    Task<WateringScheduleDto> GetWateringScheduleAsync(Guid userId, int halfMonth, string source);
}
```

Chargement sans N+1 : une seule requête EF chargeant `Plantings → Guild → GuildPlants → Plant` pour l'utilisateur.

### Endpoints dans `CalendarController`

```
GET /api/calendar/watering/today
GET /api/calendar/watering/schedule?halfMonth=1..24&source=all|myPlants|gardenPlants
```

`halfMonth` validé via `[Range(1, 24)]`. `today` résolu par `DateOnly.FromDateTime(DateTime.UtcNow)` dans le service.

### DTOs (sous-dossier `DTOs/Watering/`)

```csharp
// /watering/today — structuré par planche
record WateringTodayDto(List<BedWateringTodayDto> Beds);
record BedWateringTodayDto(Guid BedId, string BedName, List<PlantWateringStatusDto> Plants);
record PlantWateringStatusDto(Guid PlantId, string PlantName, bool IsToday, DayOfWeek? NextWateringDay);

// /watering/schedule
record WateringScheduleDto(List<BedWateringDto> Beds);
record BedWateringDto(Guid BedId, string BedName, SoilType? SoilType, bool HasMulch, List<PlantWateringDto> Plants);
record PlantWateringDto(Guid PlantId, string PlantName, WaterNeeds WaterNeeds, int TimesPerWeek, DayOfWeek[] RecommendedDays, int? WaterAmountMl);
```

`DayOfWeek`, `SoilType`, `WaterNeeds` sérialisés en **string** via `JsonStringEnumConverter` (configuré globalement dans `Program.cs`).

---

## Architecture frontend

### Nouveaux fichiers

```
features/calendar/
  calendar-watering-today/
    calendar-watering-today.ts / .html / .scss
  calendar-watering/
    calendar-watering.ts / .html / .scss
shared/services/
  watering.store.ts            ← nouveau
```

### WateringStore

Store séparé dans `shared/services/watering.store.ts`. Injecte `CalendarStore` pour lire `sourceFilter` en lecture seule — une seule source de vérité pour le filtre.

Chargement :
- `calendar-watering-today` : charge au `ngOnInit` (toujours visible)
- `calendar-watering` : **lazy** à l'activation du tab, rechargement automatique si `sourceFilter` change pendant que le tab est actif via `effect()`

### DayOfWeek côté Angular

Type union (pas d'enum TypeScript) :
```ts
export type DayOfWeek = 'Monday' | 'Tuesday' | 'Wednesday' | 'Thursday' | 'Friday' | 'Saturday' | 'Sunday';
const WEEK_DAYS: DayOfWeek[] = ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday'];
```

Position dans la grille : `WEEK_DAYS.indexOf(day)` → index colonne 0–6.

---

## Design UX

### Tokens CSS (`abstracts/_variables.scss`)

```scss
--color-water: #42a5f5;
--color-water-dark: #1565c0;
--color-water-bg: rgba(66, 165, 245, 0.08);
--color-water-bg-medium: rgba(66, 165, 245, 0.15);
--color-water-bg-strong: rgba(66, 165, 245, 0.28);
--color-water-border: rgba(66, 165, 245, 0.25);
--color-water-border-active: rgba(66, 165, 245, 0.4);
```

Nouveau fichier `styles/components/_watering.scss` importé dans `main.scss`.

### Position dans la page calendrier

```
[ Toggle source ]
[ Ce mois-ci / Upcoming ]
[ Bannière "Arrosage aujourd'hui" ]   ← nouveau, toujours visible
[ Panel : tab toggle Actions / Arrosage ]
  → tab Actions : Gantt existant inchangé
  → tab Arrosage : CalendarWatering
```

### Composant 1 — Bannière "Arrosage aujourd'hui" (`.watering-banner`)

Bande horizontale autonome (pas dans un `.panel`), fond `--color-water-bg`, bordures top/bottom `--color-water-border`.

- **État avec plantes** : icône `fa-droplet` + label "Arrosage aujourd'hui" + badges `.watering-badge` cliquables (bleu eau) par plante à arroser
- **État vide** : "Prochain arrosage : [jour]" en texte secondaire
- Mobile : icône + label sur ligne 1, badges sur ligne 2 (flex-wrap). Desktop : tout sur une ligne.
- Accessibilité : `role="status"` + `aria-live="polite"`, badges = `<button>` avec `aria-label`

### Composant 2 — Tab toggle dans le panel

`<app-toggle-group>` existant, placé entre `.panel-header` et `.calendar-filter-chips`.
- Options : `fa-seedling` "Actions culturales" | `fa-droplet` "Arrosage"
- Les chips de filtres d'action sont masquées quand "Arrosage" est actif
- Le toggle groupement byBed/byGarden reste visible uniquement dans "Actions culturales"

### Composant 3 — Grille hebdomadaire (`.watering-grid`)

Tableau 7 colonnes + colonne nom de plante. Classe `.watering-dot` (cercle bleu `#42a5f5`, 28px) dans les cellules des jours recommandés.

- Toggle semaine courante / suivante (`.btn .btn-secondary .btn-sm`) — le demi-mois peut changer entre les deux semaines → les fréquences s'adaptent
- Groupement par jardin/planche : reprend exactement le pattern du Gantt (`.section-divider-title`, `.calendar-bed-title`)
- Mobile : `overflow-x: auto` sur le conteneur, `min-width: 380px` sur la grille
- Jour courant : fond `rgba(66,165,245,0.12)`, texte `#1565c0`, `font-weight: 700`
- Section repliable "Fréquences saisonnières" via `<app-collapsible>` : tableau plante / badge besoin eau / fréquence

### Badges besoin en eau (`.water-need-badge`)

- High → fond `rgba(66,165,245,0.2)`, texte `#1565c0`
- Medium → fond `rgba(66,165,245,0.1)`, texte `#42a5f5`
- Low → fond `var(--color-parchment)`, texte `var(--color-text-muted)`

---

## Ordre de livraison des stories

```
US-339 (moteur calcul)
    ↓
US-340 (endpoint today + bannière frontend)
US-341 (endpoint schedule + grille frontend)   ← en parallèle avec US-340
    ↓
US-342 (SoilType sur Planting + ajustement)
US-343 (HasMulch sur Planting + ajustement)    ← en parallèle avec US-342
    ↓
US-344 (WaterAmountMl — optionnel)
```

US-342 et US-343 sont indépendantes entre elles mais dépendent de US-339 (calculator) étant déjà en place.

---

## Fichiers à créer ou modifier

### Backend
| Fichier | Action |
|---|---|
| `Data/Entities/Planting.cs` | + `SoilType? SoilType`, `bool HasMulch` |
| `Data/Entities/Plant.cs` | + `int? WaterAmountMl` |
| `Services/Watering/IWateringCalculator.cs` | Créer |
| `Services/Watering/WateringCalculator.cs` | Créer |
| `Services/Watering/IWateringService.cs` | Créer |
| `Services/Watering/WateringService.cs` | Créer |
| `DTOs/Watering/*.cs` | Créer (6 records) |
| `Models/WateringFrequency.cs` | Créer |
| `Controllers/CalendarController.cs` | + 2 endpoints watering |
| `Program.cs` | Enregistrement DI + JsonStringEnumConverter |
| Migrations | `AddPlantingWateringFields`, `AddPlantWaterAmount` |

### Frontend
| Fichier | Action |
|---|---|
| `styles/abstracts/_variables.scss` | + 7 tokens `--color-water-*` |
| `styles/components/_watering.scss` | Créer |
| `styles/main.scss` | Importer `_watering.scss` |
| `shared/services/watering.store.ts` | Créer |
| `features/calendar/calendar-watering-today/` | Créer composant |
| `features/calendar/calendar-watering/` | Créer composant |
| `features/calendar/calendar.ts` / `.html` | Intégrer bannière + tab toggle |
| `api/garden-assistant-api.ts` | + types DTO watering |
| `public/i18n/fr.json` + `en.json` | + clés `Watering.*` |
