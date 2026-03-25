# Plant Catalogue UI Fixes & Unification

**Date:** 2026-03-24
**Status:** Approved

## Problem

1. Clicking root depth / mechanism badges in the plant catalogue adds the plant to associations instead of opening the help popup
2. The info button (plant details) is hidden by default — only appears on hover
3. Inconsistent button styles: mix of round (circular) and rectangular buttons across modals and cards
4. Two separate components (`PlantCatalogue` and `PlantPicker`) serve similar purposes but aren't shared

## Design

### 1. Badges open help popup

In `plant-catalogue.html`, transform root depth and mechanism `<span>` elements into `<button>` elements with `(click)` handlers that call `stopPropagation()` and open `BadgeInfoDialog`.

- Root depth badges: add `openRootDepthInfo(plant.rootDepth, $event)` method
- Intrinsic mechanism badges: call existing `openMechanismInfo(m, $event)`
- Beneficial/harmful association badges: call `openMechanismInfo(m, $event)`

### 2. Info button always visible

In `_plant-list.scss`, change `.plant-info-btn`:
- Set `opacity: 0.6` permanently (was `opacity: 0`)
- Remove `.plant-item:hover &` rule

### 3. Rectangular buttons everywhere

All buttons become rectangular with `rounded-lg` (slight rounding). No more `border-radius: 50%` or `rounded-full` on action buttons.

**`_buttons.scss` changes:**
- `.icon-btn`: `rounded-full` -> `rounded-lg`, remove fixed 30x30, add padding
- `.remove-btn`: `rounded-full` -> `rounded-lg`, remove fixed 26x26, add padding
- `.btn-info`: `rounded-full` -> `rounded-lg`

**`_plant-card.scss` changes:**
- `.header-fav-btn`: remove `border-radius: 50%` and fixed 30x30, use `rounded-lg` with padding

**Dialog templates:**
- Ensure all dialog action buttons use `w-full`

**`guild-card.html`:**
- Delete button: `icon-btn` circular -> rectangular with `rounded-lg`

### 4. Unified plant catalogue component

Add two `@Input()` signals to `PlantCatalogue`:
- `mode: 'association' | 'collection'` (default: `'association'`)
- `showFilters: boolean` (default: `true`)

**Template behavior by mode:**
- `'association'`: click = `store.addPlant(plant)`, shows compatibility coloring, shows sort chips (alpha/compat/family), shows mechanism/root filters
- `'collection'`: click = `myPlantsStore.toggle(plant)`, no compatibility coloring, only alpha sort, no filters, excludes already-saved plants

**Component logic:**
- Add `collectionFilteredPlants` computed signal that filters out saved plants
- Template uses `mode === 'collection' ? collectionFilteredPlants() : store.filteredPlants()`

**Migration:**
- `my-plants.html`: replace `<app-plant-picker>` with `<app-plant-catalogue mode="collection" [showFilters]="false">`
- Delete `plant-picker/` directory (component, template, styles)
