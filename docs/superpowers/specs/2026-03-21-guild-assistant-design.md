# E15 — Assistant de creation de guilde

## Problem

When creating a guild, users — especially beginners — don't know which ecological roles should be represented. They add plants without understanding whether the guild is balanced. There is no guidance on what's missing, what mechanisms matter, or whether root layers are well distributed. Experienced users also benefit from a quick gap analysis.

## Solution

A collapsible **"Assistant de guilde"** panel integrated into the guild editor that:
1. Analyses the current guild composition in real-time
2. Shows a checklist of essential mechanisms and root depth layers (satisfied/unsatisfied)
3. Highlights the most impactful unsatisfied gap with a short explanation
4. Provides clickable chips that filter the plant catalog to find plants that fill each gap
5. Warns about harmful associations and low botanical family diversity
6. Offers educational text behind a `?` toggle for users who want to learn

## Scope

**In scope:**
- Gap analysis for 5 priority mechanisms
- Root depth layer coverage (shallow/medium/deep)
- Harmful association warnings
- Botanical family diversity warnings
- Educational help text (toggle)
- Health indicator in panel header
- All computed from existing frontend signals — no new API calls

**Out of scope:**
- Step-by-step wizard (guided order)
- Seasonal coverage analysis
- Perennial-to-annual ratio
- Water needs compatibility
- Vertical canopy layer analysis
- Guild scoring / gamification

## Architecture

### Data flow

All data comes from existing signals in `CompanionStore`:

```
selectedPlants()
  ├── guildIntrinsicMechanisms()      → intrinsic mechanisms covered
  ├── guildRelationalOnlyMechanisms() → relational mechanisms covered
  ├── rootDepthGroups()               → which root layers are occupied
  └── recommendations()
       ├── .selectedPlantAssociations  → all associations (filter effect === 1 for harmful)
       └── .intrinsicMechanismsByPlant → per-plant intrinsic mechanism mapping
```

A mechanism is considered **satisfied** if it appears in either `guildIntrinsicMechanisms()` or `guildRelationalOnlyMechanisms()` — both intrinsic and relational mechanisms count.

A new `GuildAssistantComponent` computes:
- `missingMechanisms`: the 5 priority mechanisms minus those covered (intrinsic OR relational)
- `emptyRootLayers`: root depth categories with no plants
- `highlightedGap`: the top-priority unsatisfied criterion (mechanism or root layer)
- `harmfulAssociations`: count and details from `recommendations().selectedPlantAssociations` where `effect === 1` (Harmful)
- `familyDiversityWarning`: triggers when any single family has ≥3 plants AND >40% of total
- `gapCount`: missing mechanisms + empty root layers (warnings are not included in the count — they are separate alerts)

**Loading state:** While `loading()` is true after a plant add/remove, the assistant retains its previous state. No spinner needed — the update happens fast enough that flicker is not a concern.

### Priority mechanisms (ranked)

These 5 mechanisms are checked by the assistant. They are ordered by ecological importance — the highlight always shows the top-ranking unsatisfied one:

| Rank | Mechanism | Why essential |
|------|-----------|---------------|
| 1 | NitrogenFixation | Without a legume, soil depletes and requires external fertilizer |
| 2 | SoilCover | Bare soil erodes, loses moisture, and gets colonised by weeds |
| 3 | PollinatorAttraction | Fruiting plants need pollinators for adequate yield |
| 4 | DynamicAccumulation | Deep mineral cycling closes the nutrient loop |
| 5 | PredatorAttraction | Natural pest control reduces manual intervention |

The other 11 mechanisms are not flagged as gaps but are shown if present.

### Component structure

```
guild-editor (existing)
  └── guild-assistant (new component)
       ├── header: title + "?" badge + gap count
       ├── educational-text (hidden by default, toggled by "?")
       ├── warnings section (harmful associations + family diversity)
       ├── mechanisms checklist (5 priority mechanisms)
       └── root depth checklist (3 layers)
```

The component is a standalone Angular component using signals, placed inside the guild editor between the plant cards and the existing mechanisms section.

## UI Design

### Panel container

- Uses existing `app-collapsible` component with `.panel` pattern and green accent (`#f0f7f0` background) to distinguish from informational sections
- Visible when `selectedPlants().length >= 1`
- Default: expanded on desktop, collapsed on mobile when 3+ plants

### Panel header

```
[🌱] Assistant de guilde  [?]                    [N lacunes]
```

- Plant icon + title on the left
- `?` badge: circular, subtle green border, toggles educational text
- Gap count badge on the right: "N lacunes" or "Guilde equilibree" when all satisfied (green)
- When all satisfied: title icon changes to checkmark

### "?" toggle — Educational text

- Always available, regardless of plant count
- Hidden by default; clicking `?` slides it open, clicking again closes it
- Content: 2-3 sentences explaining what makes a good guild, with inline clickable mechanism chips
- Mechanism chips in the text apply catalog filters (same as gap chips)
- `?` badge visual state: outlined when closed, filled solid when open

### Warnings section (top of panel body)

Shown only when warnings exist. Two types:

**Harmful associations** (orange, border-left `#e65100`):
- Icon: ⚠ + "N association(s) nefaste(s) detectee(s)"
- Below: plant pair names + "voir les details" link that scrolls to existing associations section using `Element.scrollIntoView({ behavior: 'smooth' })`
- Disappears when conflicting plants are removed

**Family diversity** (yellow, border-left `#f9a825`):
- Icon: ⚠ + "Faible diversite de familles"
- Below: "N plantes sur M sont des {FamilyName}. Les plantes d'une meme famille partagent ravageurs et maladies."
- Threshold: ≥3 plants from same family AND >40% of total guild plants
- Can show multiple families if several trigger the threshold

### Mechanisms checklist

Section header: "MECANISMES ESSENTIELS" (uppercase, small, muted)

Each row has three states:

**Satisfied:**
```
[✓] Fixation d'azote                          via Haricot
```
- Green checkmark
- Plant name(s) that provide this mechanism, muted text on right

**Unsatisfied (not highlighted):**
```
[○] Accumulation dynamique                    [↗ Filtrer]
```
- Empty circle, muted
- Purple "Filtrer" chip on right — clicking calls `store.toggleMechanismFilter(mechanism)`

**Unsatisfied + highlighted (top priority gap):**
```
┃ [★] Couverture du sol                       [↗ Filtrer]
┃     Un sol nu perd son humidite et se fait coloniser par les adventices.
```
- Green left border + light green background
- Star icon instead of empty circle
- Short explanation text below the label (1-2 sentences)
- "Filtrer" chip

Items stay in place and change state — they never reorder or disappear.

### Root depth checklist

Section header: "STRATIFICATION RACINAIRE" (uppercase, small, muted)

Same row pattern as mechanisms:

```
[✓] Superficiel (0-30 cm)                     Basilic, Laitue
[○] Moyen (30-60 cm)                          [↗ Filtrer]
[✓] Profond (60+ cm)                          Consoude
```

Clicking "Filtrer" calls `store.toggleRootDepthFilter(depth)`.

Root depth mapping: Superficiel = `RootDepth.Shallow` (0), Moyen = `RootDepth.Medium` (1), Profond = `RootDepth.Deep` (2).

### Completion state

When all 5 mechanisms + 3 root layers are satisfied:
- Header badge: "Guilde equilibree" in green
- Header icon: checkmark replaces plant icon
- Bottom of panel: celebration message "Votre guilde couvre tous les criteres essentiels !"
- Warnings (harmful/family) can still appear above — a "balanced" guild may still have conflicts to address

### Transitions

- Criterion satisfied → checkmark + green flash (400ms fade)
- Criterion unsatisfied → circle + amber flash (400ms fade)
- Items never move positions — spatial stability
- Gap count updates in real-time

### Mobile (≥320px)

- Panel stacks naturally in single column
- Collapsed by default when 3+ plants (header still shows gap count)
- Touch targets: 44px minimum hit area for chips
- Rows use `flex-wrap` to prevent horizontal overflow
- When user taps "Filtrer" chip, smooth scroll up to the catalog panel using `Element.scrollIntoView({ behavior: 'smooth', block: 'start' })`

## Translation keys

Namespace: `GuildAssistant.*` (PascalCase)

```
GuildAssistant.Title                    = "Assistant de guilde"
GuildAssistant.GapCount                 = "{{count}} lacune(s)"
GuildAssistant.Balanced                 = "Guilde equilibree"
GuildAssistant.BalancedMessage          = "Votre guilde couvre tous les criteres essentiels !"
GuildAssistant.Filter                   = "Filtrer"
GuildAssistant.HelpToggle               = "Qu'est-ce qu'une bonne guilde ?"
GuildAssistant.HelpText                 = "Une bonne guilde combine des plantes aux roles complementaires : fixation d'azote, couverture du sol, attraction de pollinisateurs et une diversite de profondeurs racinaires. Chaque plante apporte un role : nourrir le sol, le proteger, attirer les auxiliaires ou ramener les mineraux profonds."
GuildAssistant.SectionMechanisms        = "Mecanismes essentiels"
GuildAssistant.SectionRootDepth         = "Stratification racinaire"
GuildAssistant.Via                      = "via"
GuildAssistant.HarmfulCount             = "{{count}} association(s) nefaste(s) detectee(s)"
GuildAssistant.HarmfulDetails           = "voir les details"
GuildAssistant.FamilyDiversityTitle     = "Faible diversite de familles"
GuildAssistant.FamilyDiversityMessage   = "{{count}} plantes sur {{total}} sont des {{family}}. Les plantes d'une meme famille partagent ravageurs et maladies."
GuildAssistant.RootShallow              = "Superficiel (0-30 cm)"
GuildAssistant.RootMedium               = "Moyen (30-60 cm)"
GuildAssistant.RootDeep                 = "Profond (60+ cm)"

GuildAssistant.Gap.NitrogenFixation     = "Sans legumineuse, le sol s'appauvrira en azote et vos plantes auront besoin d'engrais."
GuildAssistant.Gap.SoilCover            = "Un sol nu perd son humidite, s'erode et se fait coloniser par les adventices."
GuildAssistant.Gap.PollinatorAttraction = "Sans fleurs melliferes, la pollinisation sera insuffisante et vos recoltes reduites."
GuildAssistant.Gap.DynamicAccumulation  = "Il manque une plante qui ramene les mineraux profonds vers la surface (consoude, pissenlit...)."
GuildAssistant.Gap.PredatorAttraction   = "Ajoutez des plantes qui attirent les auxiliaires (aneth, achillee) pour un controle naturel des ravageurs."
GuildAssistant.Gap.RootDepth            = "Diversifiez les profondeurs racinaires pour que chaque plante occupe sa propre niche souterraine."
```

## Backlog mapping

| Story | What it delivers | Points |
|-------|-----------------|--------|
| US-106 | Panel container (collapsible, header, gap count) | 3 |
| US-103 | Mechanism gap analysis (5 priority, checklist, clickable filter chips) | 5 |
| US-104 | Root depth gap analysis (3 layers, clickable filter chips) | 3 |
| US-105 | Educational text behind "?" toggle with clickable mechanism chips | 2 |
| US-107 | Harmful association warning in assistant panel | 2 |
| US-108 | Health indicator ("N lacunes" / "Guilde equilibree" in header) | 2 |
| US-109 | Botanical family diversity warning | 2 |
| **Total** | | **19** |

### Delivery order

1. **US-106** (panel) + **US-103** (mechanisms) + **US-104** (root depth) — MVP core
2. **US-108** (health indicator) + **US-105** (educational text) — enrich
3. **US-107** (harmful associations) + **US-109** (family diversity) — warnings

## Files to create/modify

### New files
- `garden-assistant-app/src/app/features/companions/guild-assistant/guild-assistant.ts` — component
- `garden-assistant-app/src/app/features/companions/guild-assistant/guild-assistant.html` — template

### Modified files
- `garden-assistant-app/src/app/features/companions/guild-editor/guild-editor.html` — insert `<app-guild-assistant>` between plant cards and mechanisms section
- `garden-assistant-app/src/app/features/companions/guild-editor/guild-editor.ts` — import new component
- `garden-assistant-app/src/app/shared/services/companion.store.ts` — add computed signals for missing mechanisms, family diversity, highlighted gap
- `garden-assistant-app/public/i18n/fr.json` — add `GuildAssistant.*` keys

### No backend changes
All data already available from existing API responses and frontend signals.
