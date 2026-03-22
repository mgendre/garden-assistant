# Guild Assistant Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Build a guild creation assistant panel that analyses guild composition in real-time, shows missing mechanisms and root depth gaps as a clickable checklist, and warns about harmful associations and low family diversity.

**Architecture:** A standalone Angular component (`GuildAssistant`) injected into the existing guild editor. All gap analysis is computed client-side from existing `CompanionStore` signals — no new API calls. New computed signals are added to the store for missing mechanisms, family diversity, and highlighted gap.

**Tech Stack:** Angular 19 (signals, standalone components), Tailwind CSS v4, ngx-translate, existing `CompanionStore` / `Collapsible` component

**Spec:** `docs/superpowers/specs/2026-03-21-guild-assistant-design.md`

---

### Task 1: Add translation keys

**Files:**
- Modify: `garden-assistant-app/public/i18n/fr.json`

- [ ] **Step 1: Add GuildAssistant namespace to fr.json**

Add the following keys inside the top-level JSON object in `fr.json`. Insert after the existing `Guilds` section:

```json
"GuildAssistant": {
  "Title": "Assistant de guilde",
  "GapCount": "{{count}} lacune(s)",
  "Balanced": "Guilde équilibrée",
  "BalancedMessage": "Votre guilde couvre tous les critères essentiels !",
  "Filter": "Filtrer",
  "HelpToggle": "Qu'est-ce qu'une bonne guilde ?",
  "HelpText": "Une bonne guilde combine des plantes aux rôles complémentaires : fixation d'azote, couverture du sol, attraction de pollinisateurs et une diversité de profondeurs racinaires. Chaque plante apporte un rôle : nourrir le sol, le protéger, attirer les auxiliaires ou ramener les minéraux profonds.",
  "SectionMechanisms": "Mécanismes essentiels",
  "SectionRootDepth": "Stratification racinaire",
  "Via": "via",
  "HarmfulCount": "{{count}} association(s) néfaste(s) détectée(s)",
  "HarmfulDetails": "voir les détails",
  "FamilyDiversityTitle": "Faible diversité de familles",
  "FamilyDiversityMessage": "{{count}} plantes sur {{total}} sont des {{family}}. Les plantes d'une même famille partagent ravageurs et maladies.",
  "RootShallow": "Superficiel (0-30 cm)",
  "RootMedium": "Moyen (30-60 cm)",
  "RootDeep": "Profond (60+ cm)",
  "Gap": {
    "NitrogenFixation": "Sans légumineuse, le sol s'appauvrira en azote et vos plantes auront besoin d'engrais.",
    "SoilCover": "Un sol nu perd son humidité, s'érode et se fait coloniser par les adventices.",
    "PollinatorAttraction": "Sans fleurs mellifères, la pollinisation sera insuffisante et vos récoltes réduites.",
    "DynamicAccumulation": "Il manque une plante qui ramène les minéraux profonds vers la surface (consoude, pissenlit...).",
    "PredatorAttraction": "Ajoutez des plantes qui attirent les auxiliaires (aneth, achillée) pour un contrôle naturel des ravageurs.",
    "RootDepth": "Diversifiez les profondeurs racinaires pour que chaque plante occupe sa propre niche souterraine."
  }
}
```

- [ ] **Step 2: Verify the build passes**

Run: `npm run build --prefix garden-assistant-app`
Expected: Build succeeds with no errors.

- [ ] **Step 3: Commit**

```bash
git add garden-assistant-app/public/i18n/fr.json
git commit -m "feat(E15): add GuildAssistant translation keys for guild assistant panel"
```

---

### Task 2: Add computed signals to CompanionStore

**Files:**
- Modify: `garden-assistant-app/src/app/shared/services/companion.store.ts`

The store already has `guildIntrinsicMechanisms`, `guildRelationalOnlyMechanisms`, `rootDepthGroups`, `recommendations`, and `selectedPlants`. We add new computed signals for the assistant.

- [ ] **Step 1: Add the PRIORITY_MECHANISMS constant**

Add after the existing `MECHANISM_KEY_MAP` constant (after line 64):

```typescript
export const PRIORITY_MECHANISMS: AssociationMechanism[] = [
  AssociationMechanism.NitrogenFixation,
  AssociationMechanism.SoilCover,
  AssociationMechanism.PollinatorAttraction,
  AssociationMechanism.DynamicAccumulation,
  AssociationMechanism.PredatorAttraction,
];
```

Also add `AssociationEffect` to the import statement at line 3-12 of `companion.store.ts`:

```typescript
import {
  PlantDto,
  CompanionSearchResultDto,
  CompanionRecommendationRequest,
  GuildDto,
  AssociationMechanism,
  AssociationEffect,
  RootDepth,
  CreateGuildRequest,
  UpdateGuildRequest,
} from '../../api/garden-assistant-api';
```

- [ ] **Step 2: Add `allGuildMechanisms` computed signal**

Add after `guildRelationalOnlyMechanisms` (after line 295):

```typescript
readonly allGuildMechanisms = computed(() => {
  const intrinsic = new Set(this.guildIntrinsicMechanisms());
  for (const m of this.guildRelationalOnlyMechanisms()) {
    intrinsic.add(m);
  }
  return intrinsic;
});
```

- [ ] **Step 3: Add `missingPriorityMechanisms` computed signal**

Add after `allGuildMechanisms`:

```typescript
readonly missingPriorityMechanisms = computed(() => {
  const covered = this.allGuildMechanisms();
  return PRIORITY_MECHANISMS.filter(m => !covered.has(m));
});
```

- [ ] **Step 4: Add `mechanismProviders` computed signal**

This maps each satisfied priority mechanism to the plant name(s) that provide it. Add after `missingPriorityMechanisms`:

```typescript
readonly mechanismProviders = computed(() => {
  const map = new Map<AssociationMechanism, string[]>();
  const intrinsicByPlant = this.recommendations()?.intrinsicMechanismsByPlant ?? [];
  for (const entry of intrinsicByPlant) {
    if (!entry.plantId) { continue; }
    const plant = this.plantStore.findById(entry.plantId);
    if (!plant) { continue; }
    for (const m of entry.mechanisms ?? []) {
      if (!PRIORITY_MECHANISMS.includes(m)) { continue; }
      const names = map.get(m) ?? [];
      names.push(plant.name ?? '');
      map.set(m, names);
    }
  }
  return map;
});
```

- [ ] **Step 5: Add `emptyRootLayers` computed signal**

Add after `mechanismProviders`:

```typescript
readonly emptyRootLayers = computed(() => {
  const groups = this.rootDepthGroups();
  const empty: RootDepth[] = [];
  if (!groups.has(RootDepth.Shallow)) { empty.push(RootDepth.Shallow); }
  if (!groups.has(RootDepth.Medium)) { empty.push(RootDepth.Medium); }
  if (!groups.has(RootDepth.Deep)) { empty.push(RootDepth.Deep); }
  return empty;
});
```

- [ ] **Step 6: Add `rootLayerProviders` computed signal**

```typescript
readonly rootLayerProviders = computed(() => {
  const map = new Map<RootDepth, string[]>();
  for (const [depth, plants] of this.rootDepthGroups()) {
    map.set(depth, plants.map(p => p.name ?? ''));
  }
  return map;
});
```

- [ ] **Step 7: Add `familyDiversityWarnings` computed signal**

```typescript
readonly familyDiversityWarnings = computed(() => {
  const plants = this.selectedPlants();
  if (plants.length < 3) { return []; }
  const familyCounts = new Map<string, number>();
  for (const p of plants) {
    if (!p.family) { continue; }
    familyCounts.set(p.family, (familyCounts.get(p.family) ?? 0) + 1);
  }
  const warnings: { family: string; count: number; total: number }[] = [];
  for (const [family, count] of familyCounts) {
    if (count >= 3 && count / plants.length > 0.4) {
      warnings.push({ family, count, total: plants.length });
    }
  }
  return warnings;
});
```

- [ ] **Step 8: Add `harmfulAssociationPairs` computed signal**

```typescript
readonly harmfulAssociationPairs = computed(() => {
  const associations = this.recommendations()?.selectedPlantAssociations ?? [];
  const harmful = associations.filter(a => a.effect === AssociationEffect.Harmful);
  const seen = new Set<string>();
  const pairs: { plantA: string; plantB: string }[] = [];
  for (const a of harmful) {
    const key = [a.sourcePlantId, a.targetPlantId].sort().join('-');
    if (seen.has(key)) { continue; }
    seen.add(key);
    pairs.push({
      plantA: this.plantStore.findById(a.sourcePlantId)?.name ?? '',
      plantB: this.plantStore.findById(a.targetPlantId)?.name ?? '',
    });
  }
  return pairs;
});
```

- [ ] **Step 9: Add `assistantGapCount` computed signal**

```typescript
readonly assistantGapCount = computed(() =>
  this.missingPriorityMechanisms().length + this.emptyRootLayers().length
);
```

- [ ] **Step 10: Verify the build passes**

Run: `npm run build --prefix garden-assistant-app`
Expected: Build succeeds with no errors.

- [ ] **Step 11: Commit**

```bash
git add garden-assistant-app/src/app/shared/services/companion.store.ts
git commit -m "feat(E15): add guild assistant computed signals to CompanionStore"
```

---

### Task 3: Create GuildAssistant component

**Files:**
- Create: `garden-assistant-app/src/app/features/companions/guild-assistant/guild-assistant.ts`
- Create: `garden-assistant-app/src/app/features/companions/guild-assistant/guild-assistant.html`

- [ ] **Step 1: Create the component class**

Create `garden-assistant-app/src/app/features/companions/guild-assistant/guild-assistant.ts`:

```typescript
import { Component, inject, signal, computed } from '@angular/core';
import { TranslateModule } from '@ngx-translate/core';
import { CompanionStore, PRIORITY_MECHANISMS } from '../../../shared/services/companion.store';
import { AssociationMechanism, RootDepth } from '../../../api/garden-assistant-api';
import { Collapsible } from '../../../shared/ui/collapsible/collapsible';

interface MechanismRow {
  mechanism: AssociationMechanism;
  key: string;
  satisfied: boolean;
  providers: string[];
  highlighted: boolean;
}

interface RootDepthRow {
  depth: RootDepth;
  translationKey: string;
  satisfied: boolean;
  providers: string[];
  highlighted: boolean;
}

const ROOT_DEPTH_KEYS: Record<RootDepth, string> = {
  [RootDepth.Shallow]: 'GuildAssistant.RootShallow',
  [RootDepth.Medium]: 'GuildAssistant.RootMedium',
  [RootDepth.Deep]: 'GuildAssistant.RootDeep',
};

const HELP_MECHANISM_CHIPS: { mechanism: AssociationMechanism; key: string }[] = [
  { mechanism: AssociationMechanism.NitrogenFixation, key: 'NitrogenFixation' },
  { mechanism: AssociationMechanism.SoilCover, key: 'SoilCover' },
  { mechanism: AssociationMechanism.PollinatorAttraction, key: 'PollinatorAttraction' },
];

@Component({
  selector: 'app-guild-assistant',
  standalone: true,
  imports: [TranslateModule, Collapsible],
  templateUrl: './guild-assistant.html',
  styleUrl: './guild-assistant.scss'
})
export class GuildAssistant {
  protected readonly store = inject(CompanionStore);
  readonly helpOpen = signal(false);
  protected readonly helpChips = HELP_MECHANISM_CHIPS;

  readonly mechanismRows = computed<MechanismRow[]>(() => {
    const covered = this.store.allGuildMechanisms();
    const providers = this.store.mechanismProviders();
    let firstMissing = true;
    return PRIORITY_MECHANISMS.map(m => {
      const satisfied = covered.has(m);
      let highlighted = false;
      if (!satisfied && firstMissing) {
        highlighted = true;
        firstMissing = false;
      }
      return {
        mechanism: m,
        key: this.store.getMechanismKey(m),
        satisfied,
        providers: providers.get(m) ?? [],
        highlighted,
      };
    });
  });

  readonly rootDepthRows = computed<RootDepthRow[]>(() => {
    const empty = new Set(this.store.emptyRootLayers());
    const providers = this.store.rootLayerProviders();
    const mechanismsMissing = this.store.missingPriorityMechanisms().length > 0;
    let firstMissing = true;
    return [RootDepth.Shallow, RootDepth.Medium, RootDepth.Deep].map(depth => {
      const satisfied = !empty.has(depth);
      let highlighted = false;
      if (!satisfied && !mechanismsMissing && firstMissing) {
        highlighted = true;
        firstMissing = false;
      }
      return {
        depth,
        translationKey: ROOT_DEPTH_KEYS[depth],
        satisfied,
        providers: providers.get(depth) ?? [],
        highlighted,
      };
    });
  });

  readonly isBalanced = computed(() => this.store.assistantGapCount() === 0);

  toggleHelp(): void {
    this.helpOpen.update(v => !v);
  }

  filterMechanism(mechanism: AssociationMechanism): void {
    this.store.toggleMechanismFilter(mechanism);
    this.scrollToCatalog();
  }

  filterRootDepth(depth: RootDepth): void {
    this.store.toggleRootDepthFilter(depth);
    this.scrollToCatalog();
  }

  scrollToAssociations(): void {
    const el = document.querySelector('.guild-associations');
    if (el) {
      el.scrollIntoView({ behavior: 'smooth' });
    }
  }

  private scrollToCatalog(): void {
    const el = document.querySelector('.plant-catalogue');
    if (el) {
      el.scrollIntoView({ behavior: 'smooth', block: 'start' });
    }
  }
}
```

- [ ] **Step 2: Create the template**

Create `garden-assistant-app/src/app/features/companions/guild-assistant/guild-assistant.html`:

```html
<app-collapsible [initialExpanded]="true">
  <div collapsible-header class="assistant-header">
    <div class="assistant-header-left">
      @if (isBalanced()) {
        <span class="assistant-icon">✓</span>
      } @else {
        <span class="assistant-icon">🌱</span>
      }
      <span class="assistant-title" [translate]="'GuildAssistant.Title'"></span>
      <button
        class="assistant-help-badge"
        [class.assistant-help-badge--active]="helpOpen()"
        [title]="'GuildAssistant.HelpToggle' | translate"
        (click)="toggleHelp(); $event.stopPropagation()">?</button>
    </div>
    <div class="assistant-header-right">
      @if (isBalanced()) {
        <span class="assistant-gap-count assistant-gap-count--balanced" [translate]="'GuildAssistant.Balanced'"></span>
      } @else {
        <span class="assistant-gap-count">{{ 'GuildAssistant.GapCount' | translate:{ count: store.assistantGapCount() } }}</span>
      }
    </div>
  </div>

  <div collapsible-body>
    @if (helpOpen()) {
      <div class="assistant-help-text">
        <span [translate]="'GuildAssistant.HelpText'"></span>
        <div class="assistant-help-chips">
          @for (chip of helpChips; track chip.mechanism) {
            <button class="assistant-filter-chip" (click)="filterMechanism(chip.mechanism)">
              {{ 'Plant.Mechanism.' + chip.key | translate }}
            </button>
          }
        </div>
      </div>
    }

    <div class="assistant-body">
      @if (store.harmfulAssociationPairs().length) {
        <div class="assistant-warning assistant-warning--harmful">
          <div class="assistant-warning-header">
            <span>⚠️</span>
            <strong>{{ 'GuildAssistant.HarmfulCount' | translate:{ count: store.harmfulAssociationPairs().length } }}</strong>
          </div>
          <div class="assistant-warning-body">
            @for (pair of store.harmfulAssociationPairs(); track $index) {
              <span class="assistant-conflict-pair">{{ pair.plantA }} ↔ {{ pair.plantB }}</span>
            }
            <a class="assistant-details-link" (click)="scrollToAssociations()">
              {{ 'GuildAssistant.HarmfulDetails' | translate }}
            </a>
          </div>
        </div>
      }

      @for (warning of store.familyDiversityWarnings(); track warning.family) {
        <div class="assistant-warning assistant-warning--family">
          <div class="assistant-warning-header">
            <span>⚠️</span>
            <strong [translate]="'GuildAssistant.FamilyDiversityTitle'"></strong>
          </div>
          <div class="assistant-warning-body">
            <span>{{ 'GuildAssistant.FamilyDiversityMessage' | translate:{ count: warning.count, total: warning.total, family: warning.family } }}</span>
          </div>
        </div>
      }

      <div class="assistant-section-header" [translate]="'GuildAssistant.SectionMechanisms'"></div>

      @for (row of mechanismRows(); track row.mechanism) {
        <div class="assistant-row"
             [class.assistant-row--satisfied]="row.satisfied"
             [class.assistant-row--highlighted]="row.highlighted">
          @if (row.satisfied) {
            <span class="assistant-check">✓</span>
          } @else if (row.highlighted) {
            <span class="assistant-star">★</span>
          } @else {
            <span class="assistant-empty">○</span>
          }
          <span class="assistant-row-label">{{ 'Plant.Mechanism.' + row.key | translate }}</span>
          @if (row.satisfied) {
            <span class="assistant-row-provider">{{ 'GuildAssistant.Via' | translate }} {{ row.providers.join(', ') }}</span>
          } @else {
            <button class="assistant-filter-chip" (click)="filterMechanism(row.mechanism)">
              ↗ {{ 'GuildAssistant.Filter' | translate }}
            </button>
          }
        </div>
        @if (row.highlighted) {
          <div class="assistant-row-explanation">
            {{ 'GuildAssistant.Gap.' + row.key | translate }}
          </div>
        }
      }

      <div class="assistant-section-header assistant-section-header--roots" [translate]="'GuildAssistant.SectionRootDepth'"></div>

      @for (row of rootDepthRows(); track row.depth) {
        <div class="assistant-row"
             [class.assistant-row--satisfied]="row.satisfied"
             [class.assistant-row--highlighted]="row.highlighted">
          @if (row.satisfied) {
            <span class="assistant-check">✓</span>
          } @else if (row.highlighted) {
            <span class="assistant-star">★</span>
          } @else {
            <span class="assistant-empty">○</span>
          }
          <span class="assistant-row-label" [translate]="row.translationKey"></span>
          @if (row.satisfied) {
            <span class="assistant-row-provider">{{ row.providers.join(', ') }}</span>
          } @else {
            <button class="assistant-filter-chip" (click)="filterRootDepth(row.depth)">
              ↗ {{ 'GuildAssistant.Filter' | translate }}
            </button>
          }
        </div>
        @if (row.highlighted) {
          <div class="assistant-row-explanation">
            {{ 'GuildAssistant.Gap.RootDepth' | translate }}
          </div>
        }
      }

      @if (isBalanced()) {
        <div class="assistant-balanced-message" [translate]="'GuildAssistant.BalancedMessage'"></div>
      }
    </div>
  </div>
</app-collapsible>
```

- [ ] **Step 3: Create the styles**

Create `garden-assistant-app/src/app/features/companions/guild-assistant/guild-assistant.scss`:

```scss
.assistant-panel {
  border-bottom: 1px solid rgba(45, 106, 79, 0.1);
}

.assistant-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0.625rem 1.25rem;
  background: #f0f7f0;
}

.assistant-header-left {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.assistant-icon {
  font-size: 1rem;
}

.assistant-title {
  font-size: 0.8125rem;
  font-weight: 600;
  color: var(--color-primary-dark, #2e5a2e);
}

.assistant-help-badge {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 1.25rem;
  height: 1.25rem;
  border-radius: 50%;
  background: #e0ede0;
  color: #5a7a5a;
  font-size: 0.6875rem;
  font-weight: 700;
  cursor: pointer;
  border: 1px solid #c0d8c0;
  transition: all 0.25s ease;
  padding: 0;
  line-height: 1;

  &--active {
    background: #2e7d32;
    color: white;
    border-color: #2e7d32;
  }
}

.assistant-gap-count {
  font-size: 0.6875rem;
  color: #5a7a5a;
  background: #e0ede0;
  padding: 0.125rem 0.5rem;
  border-radius: 0.625rem;

  &--balanced {
    color: #2e7d32;
  }
}

.assistant-help-text {
  padding: 0.625rem 1rem;
  margin: 0.75rem 1.25rem 0;
  background: #f5f5f5;
  border-radius: 0.375rem;
  font-size: 0.75rem;
  line-height: 1.5;
  color: #555;
}

.assistant-help-chips {
  display: flex;
  flex-wrap: wrap;
  gap: 0.375rem;
  margin-top: 0.5rem;
}

.assistant-body {
  padding: 0.75rem 1.25rem;
}

.assistant-warning {
  padding: 0.5rem 0.75rem;
  border-radius: 0.375rem;
  margin-bottom: 0.625rem;

  &--harmful {
    background: #fff3e0;
    border-left: 3px solid #e65100;
  }

  &--family {
    background: #fff8e1;
    border-left: 3px solid #f9a825;
  }
}

.assistant-warning-header {
  display: flex;
  align-items: center;
  gap: 0.375rem;
  margin-bottom: 0.25rem;
  font-size: 0.75rem;
}

.assistant-warning-body {
  font-size: 0.6875rem;
  color: #666;
  padding-left: 1.375rem;
  display: flex;
  flex-wrap: wrap;
  gap: 0.25rem;
}

.assistant-conflict-pair {
  font-size: 0.6875rem;
  color: #555;
}

.assistant-details-link {
  color: #5e35b1;
  text-decoration: underline;
  font-size: 0.6875rem;
  cursor: pointer;
}

.assistant-section-header {
  font-size: 0.625rem;
  text-transform: uppercase;
  letter-spacing: 0.06em;
  color: #5a7a5a;
  font-weight: 600;
  margin-bottom: 0.5rem;

  &--roots {
    margin-top: 0.625rem;
  }
}

.assistant-row {
  display: flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.375rem 0.625rem;
  margin-bottom: 0.125rem;
  min-height: 2.75rem;
  flex-wrap: wrap;
  transition: background-color 0.4s ease;

  &--highlighted {
    background: #e8f5e9;
    border-radius: 0.375rem;
    border-left: 3px solid #2e7d32;
    margin-bottom: 0;
  }
}

.assistant-check {
  color: #2e7d32;
  font-size: 0.75rem;
  flex-shrink: 0;
}

.assistant-star {
  color: #2e7d32;
  font-size: 0.875rem;
  flex-shrink: 0;
}

.assistant-empty {
  color: #bbb;
  font-size: 0.75rem;
  flex-shrink: 0;
}

.assistant-row-label {
  flex: 1;
  font-size: 0.75rem;
  min-width: 0;

  .assistant-row--highlighted & {
    font-weight: 600;
  }
}

.assistant-row-provider {
  font-size: 0.6875rem;
  color: #888;
  text-align: right;
}

.assistant-filter-chip {
  background: #ede7f6;
  color: #5e35b1;
  padding: 0.0625rem 0.5rem;
  border-radius: 0.625rem;
  font-size: 0.625rem;
  cursor: pointer;
  border: none;
  font-weight: 600;
  white-space: nowrap;
  transition: opacity 0.2s ease;
  min-height: 1.5rem;

  &:hover {
    opacity: 0.8;
  }
}

.assistant-row-explanation {
  font-size: 0.6875rem;
  color: #555;
  line-height: 1.4;
  padding: 0.125rem 0.625rem 0.5rem 1.875rem;
  background: #e8f5e9;
  border-left: 3px solid #2e7d32;
  border-radius: 0 0 0.375rem 0.375rem;
  margin-bottom: 0.125rem;
}

.assistant-balanced-message {
  margin-top: 0.75rem;
  padding: 0.625rem 0.75rem;
  background: #e8f5e9;
  border-radius: 0.375rem;
  text-align: center;
  font-size: 0.75rem;
  color: #2e7d32;
}
```

- [ ] **Step 4: Verify the build passes**

Run: `npm run build --prefix garden-assistant-app`
Expected: Build succeeds with no errors.

- [ ] **Step 5: Commit**

```bash
git add garden-assistant-app/src/app/features/companions/guild-assistant/
git commit -m "feat(E15): create GuildAssistant component with template and styles"
```

---

### Task 4: Integrate GuildAssistant into GuildEditor

**Files:**
- Modify: `garden-assistant-app/src/app/features/companions/guild-editor/guild-editor.ts`
- Modify: `garden-assistant-app/src/app/features/companions/guild-editor/guild-editor.html`

- [ ] **Step 1: Import GuildAssistant in guild-editor.ts**

In `guild-editor.ts`, add the import at the top (after line 18):

```typescript
import { GuildAssistant } from '../guild-assistant/guild-assistant';
```

Then add `GuildAssistant` to the `imports` array in the `@Component` decorator (line 31). Change:

```typescript
imports: [TranslateModule, FontAwesomeModule, PlantDetailPanel, GuildPanel, Collapsible, RootStratification, PlantCalendarGantt],
```

to:

```typescript
imports: [TranslateModule, FontAwesomeModule, PlantDetailPanel, GuildPanel, Collapsible, RootStratification, PlantCalendarGantt, GuildAssistant],
```

- [ ] **Step 2: Insert component in guild-editor.html**

In `guild-editor.html`, insert the assistant component between the plant detail panel (line 47) and the mechanisms section (line 49). Add after `<app-plant-detail-panel></app-plant-detail-panel>`:

```html
    @if (store.selectedPlants().length >= 1) {
      <app-guild-assistant></app-guild-assistant>
    }
```

- [ ] **Step 3: Verify the build passes**

Run: `npm run build --prefix garden-assistant-app`
Expected: Build succeeds with no errors.

- [ ] **Step 4: Manual smoke test**

Run: `npm run start --prefix garden-assistant-app`

Verify:
1. Select 1 plant → assistant panel appears with all gaps shown
2. The top-priority missing mechanism (NitrogenFixation) is highlighted with explanation
3. Click "Filtrer" chip → plant catalog filters by that mechanism
4. Add a legume (e.g., Haricot) → NitrogenFixation row shows checkmark + plant name, highlight moves to next gap
5. Click "?" badge → educational text appears/disappears
6. Add 3+ Solanaceae → family diversity warning appears
7. Add plants with harmful association → orange warning appears with "voir les details" link
8. Fill all 5 mechanisms + 3 root depths → header shows "Guilde équilibrée" + celebration message
9. Remove a plant → state reverts correctly

- [ ] **Step 5: Commit**

```bash
git add garden-assistant-app/src/app/features/companions/guild-editor/guild-editor.ts garden-assistant-app/src/app/features/companions/guild-editor/guild-editor.html
git commit -m "feat(E15): integrate GuildAssistant into guild editor"
```

---

### Task 5: Mobile responsiveness and polish

**Files:**
- Modify: `garden-assistant-app/src/app/features/companions/guild-assistant/guild-assistant.scss`

- [ ] **Step 1: Add mobile-specific styles**

Add at the end of `guild-assistant.scss`:

```scss
@media (max-width: 640px) {
  .assistant-row {
    padding: 0.5rem 0.375rem;
    min-height: 2.75rem;
  }

  .assistant-filter-chip {
    min-height: 2.75rem;
    padding: 0.375rem 0.75rem;
    font-size: 0.6875rem;
  }

  .assistant-help-badge {
    width: 2.75rem;
    height: 2.75rem;
    font-size: 0.875rem;
  }

  .assistant-body {
    padding: 0.75rem 0.75rem;
  }

  .assistant-header {
    padding: 0.625rem 0.75rem;
  }

  .assistant-help-text {
    margin: 0.75rem 0.75rem 0;
  }
}
```

- [ ] **Step 2: Verify the build passes**

Run: `npm run build --prefix garden-assistant-app`
Expected: Build succeeds with no errors.

- [ ] **Step 3: Commit**

```bash
git add garden-assistant-app/src/app/features/companions/guild-assistant/guild-assistant.scss
git commit -m "feat(E15): add mobile responsiveness to guild assistant panel"
```
