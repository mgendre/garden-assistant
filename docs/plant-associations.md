# Plant Associations — Data Model

## Why this model?

Companion planting is at the heart of permaculture. Two neighbouring plants interact through specific biological mechanisms: root exudates, volatile compounds, insect attraction, nitrogen fixation... These interactions are **neither binary nor symmetric**.

The data model was designed with the help of a permaculture expert to reflect this reality.

---

## Core principles

### 1. One association per mechanism

The same pair of plants can have **multiple active mechanisms simultaneously**. For example, basil planted near tomato:

- repels whiteflies through olfactory confusion
- attracts pollinators beneficial for fruit set
- is said to improve fruit flavour (traditional use)

Each mechanism is stored as **a separate row** in `plant_associations`. This allows querying by mechanism, attaching a scientific confidence level to each effect, and extending the model without destructive migrations.

### 2. Associations are directional

An association is **not symmetric**. The effect of A on B is not the same as the effect of B on A.

| Source | Target | Mechanism | Effect |
|---|---|---|---|
| Bean | Corn | NitrogenFixation | Beneficial |
| Corn | Bean | PhysicalSupport | Beneficial |
| Nasturtium | Rose | TrapCrop | Beneficial (for the rose) |
| Fennel | Tomato | RootAllelopathy | Harmful |

The model uses explicit `SourcePlantId` and `TargetPlantId`. No forced symmetry.

### 3. Distance matters

A root allelopathic effect only applies at contact or within 50 cm. Olfactory confusion can operate up to 2 metres. The `DistanceEffect` field encodes this reality to validate whether a declared association is actually active in the user's spatial plan.

---

## `plant_associations` table schema

```
Id              Guid  PK
SourcePlantId   Guid  FK → plants   (the plant producing the effect)
TargetPlantId   Guid  FK → plants   (the plant receiving the effect)
Mechanism       enum  see below
Effect          enum  Beneficial | Harmful | Neutral
DistanceEffect  enum  Contact | Short | Medium | Field
ConfidenceLevel enum  Anecdotal | FieldObserved | PeerReviewed
Notes           string?
```

**Unique constraint on `(SourcePlantId, TargetPlantId, Mechanism)`**: one record per mechanism per directional pair.

---

## Available mechanisms

| Mechanism | Description | Example |
|---|---|---|
| `OlfactoryConfusion` | Volatile compounds that confuse pests | Basil → Tomato (whiteflies) |
| `PollinatorAttraction` | Flowers that attract bees, bumblebees, hoverflies | Borage → Cucurbits |
| `TrapCrop` | Sacrificial plant that lures pests away from the target | Nasturtium → Rose (aphids) |
| `RootAllelopathy` | Root exudates inhibiting germination or growth | Fennel → Tomato |
| `AerialRepulsion` | Volatile terpenes repelling insects | Marigold → Tomato (whiteflies) |
| `NitrogenFixation` | Legumes enriching the soil with nitrogen via rhizobia | Bean → Corn |
| `PredatorAttraction` | Attracts beneficial insects (ladybugs, lacewings) | Nasturtium → neighbours (via aphids) |
| `PhysicalSupport` | Structural support (living trellis) | Corn → Climbing bean |
| `SoilCover` | Covers soil, reduces evaporation and weeds | Squash → Corn+Bean |
| `DynamicAccumulation` | Brings deep minerals to the surface | Comfrey → neighbours |

---

## Plant catalogue (`plants`)

The `plants` table is a global catalogue (not user-scoped). It contains the biological data needed for recommendations.

### Key functional fields

| Field | Type | Role |
|---|---|---|
| `NitrogenFixer` | bool | Identifies legumes with rhizobia |
| `AllelopathicRisk` | bool | Warning signal before association |
| `PollinatorPlant` | bool | Biodiversity value |
| `RootDepth` | enum | Shallow / Medium / Deep — vertical complementarity |
| `HeightAtMaturityCm` | int? | Shade conflict calculation |
| `LifeCycle` | enum | Annual / Biennial / Perennial — temporal planning |

---

## Planting plans (`plantings` + `planting_entries`)

A planting plan belongs to a user through their garden.

### `planting_entries` — a plant in a plan

```
PlantId          Guid  FK → plants
PositionX / Y    float  coordinates in metres
Layer            enum   Canopy | SubCanopy | Shrub | Herbaceous | GroundCover | Climber | Root
PlannedSowDate   DateOnly?
PlannedHarvestDate DateOnly?
ActualHarvestDate  DateOnly?
```

The vertical layers (`Layer`) model forest gardens according to Robert Hart's 7 layers (canopy, sub-canopy, shrub, herbaceous, ground cover, climber, root).

---

## Compatibility score

The endpoint `GET /api/plantings/{id}/compatibility` calculates a planting's score by cross-referencing `planting_entries` with `plant_associations`:

```
Beneficial : 4
Harmful    : 1
Neutral    : 2
Total      : 7
```

This score provides a visual indicator and alerts the user to problematic associations before sowing.

---

## Example: the Three Sisters

The traditional Native American guild (corn + bean + squash) generates these associations in the model:

| Source | Target | Mechanism | Effect |
|---|---|---|---|
| Bean | Corn | NitrogenFixation | Beneficial |
| Corn | Bean | PhysicalSupport | Beneficial |
| Squash | Corn | SoilCover | Beneficial |
| Squash | Bean | SoilCover | Beneficial |

Compatibility score for a Three Sisters planting: **4 Beneficial, 0 Harmful**.

---

## Confidence level (`ConfidenceLevel`)

| Value | Meaning |
|---|---|
| `Anecdotal` | Folk tradition, empirical gardening |
| `FieldObserved` | Reproducible field observations |
| `PeerReviewed` | Published, peer-reviewed scientific study |

Always display the confidence level to the user so they can weigh recommendations accordingly.
