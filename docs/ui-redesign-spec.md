# UI/UX Redesign Specification

**Status:** Design spec for `frontend-angular-developer`
**Author:** UX Designer
**Date:** 2026-03-14

---

## Table of Contents

1. [Design overview](#1-design-overview)
2. [Design tokens and Tailwind changes](#2-design-tokens-and-tailwind-changes)
3. [Header and navigation](#3-header-and-navigation)
4. [Page content area](#4-page-content-area)
5. [Dashboard page](#5-dashboard-page)
6. [Mon Jardin page](#6-mon-jardin-page)
7. [Taches page](#7-taches-page)
8. [Associations page](#8-associations-page)
9. [Dialogs](#9-dialogs)
10. [Accessibility checklist](#10-accessibility-checklist)
11. [Migration notes](#11-migration-notes)

---

## 1. Design overview

### Goal

Replace the current sidebar navigation with a top header bar. Adopt a mobile-first approach where every layout works from 320px upward. Keep the green/nature-inspired aesthetic but make it feel lighter and more modern.

### Design principles

- **Mobile-first:** Default styles target 320px+. Enhance at `md` (768px) and `lg` (1024px).
- **Content over chrome:** The header should be compact so most of the viewport is usable content.
- **Outdoor readability:** High contrast text, generous touch targets, sunlight-friendly color choices (warm backgrounds, no thin grey text).
- **Consistency:** All pages share the same page header pattern (category label + heading + optional action).

### Classification key

Throughout this document:
- **(R)** = Required -- must be implemented exactly as specified
- **(REC)** = Recommended -- strong preference, deviate only with good reason
- **(S)** = Suggestion -- open to frontend interpretation

---

## 2. Design tokens and Tailwind changes

### Tokens to add in `tailwind.css` `@theme` block

```
--color-header-bg: #1e3d1e           (reuses forest)
--color-header-text: #ffffff
--color-header-text-muted: rgba(255, 255, 255, 0.6)
--color-header-hover-bg: rgba(255, 255, 255, 0.1)
--color-header-active-bg: rgba(255, 255, 255, 0.15)
--color-header-border: rgba(255, 255, 255, 0.1)

--spacing-header-height: 64px
--spacing-header-height-mobile: 56px
```

**(R)** These tokens go into the existing `@theme {}` block in `src/tailwind.css`.

### Tokens to remove from `_variables.scss`

**(R)** Remove `$sidebar-width: 260px;` -- no longer needed.
**(R)** Remove `$shadow-sidebar` -- no longer needed.

### Tokens to add to `_variables.scss`

```scss
$header-height:        64px;
$header-height-mobile: 56px;
$z-header:             100;
$z-mobile-menu:        110;
$z-mobile-overlay:     105;
$content-max-width:    1200px;   // update from 1100px
```

**(R)** Update `$z-sidebar` to `$z-header` (rename, same value 100).

---

## 3. Header and navigation

### Overview

The sidebar (`<aside>`) and mobile topbar are replaced by a single `<header>` element that spans the full viewport width at the top of the page. Navigation links are displayed horizontally on desktop and collapse into a hamburger-triggered slide-down panel on mobile.

### 3.1 Header structure

**(R)** Classification for all items in this section.

```
+----------------------------------------------------------------------+
| [Logo + Brand]          [Nav Links (desktop)]          [Future: User] |
+----------------------------------------------------------------------+
```

**Desktop (lg: 1024px+):**

```
+------------------------------------------------------------------------+
| [leaf icon] Garden Assistant    Dashboard  Mon jardin  Taches  Assoc.  |
+------------------------------------------------------------------------+
```

**Tablet (md: 768px - 1023px):**

Same as desktop but nav labels may be slightly shorter. The horizontal nav still fits at this width given there are only 4 items.

**Mobile (< 768px):**

```
+--------------------------------------------+
| [hamburger]  Garden Assistant               |
+--------------------------------------------+
| (panel slides down when hamburger tapped)  |
| > Tableau de bord                          |
| > Mon jardin                               |
| > Taches                                   |
| > Associations                             |
+--------------------------------------------+
```

### 3.2 Header layout specification

**(R)** The `<header>` is a fixed-position bar at the top of the viewport. It does not scroll with content.

| Property | Mobile (< 768px) | md (768px+) / lg (1024px+) |
|---|---|---|
| Height | 56px | 64px |
| Background | `bg-forest` (#1e3d1e) | `bg-forest` (#1e3d1e) |
| Position | `fixed top-0 left-0 right-0` | `fixed top-0 left-0 right-0` |
| Z-index | `z-[100]` | `z-[100]` |
| Padding x | `px-4` (16px) | `px-6` (24px) |
| Inner layout | `flex items-center justify-between` | `flex items-center` |
| Shadow | `shadow-md` | `shadow-md` |

**(R)** The page content area must have `pt-14 md:pt-16` (56px / 64px) to offset below the fixed header.

### 3.3 Brand / logo area (left side)

**(R)** Structure and placement. **(S)** Exact icon style.

| Element | Spec |
|---|---|
| Container | `flex items-center gap-2.5 md:gap-3` |
| Icon | Leaf SVG icon (not emoji), 28x28px on mobile, 32x32px on desktop. Color: `text-sage` (#8ab88a). Alternatively, keep the current green rounded-square container with leaf icon inside. |
| Brand text | `font-['DM_Serif_Display'] text-base md:text-lg text-white tracking-wide` |
| Link | The brand area is a `<a routerLink="/dashboard">` for home navigation |

**(R)** Replace the emoji-based leaf icon with an SVG leaf. No emojis in the header.

### 3.4 Desktop navigation (horizontal links)

**(R)** Visible at `md` breakpoint and above. Hidden on mobile.

| Property | Value |
|---|---|
| Container | `hidden md:flex items-center gap-1 ml-8` |
| Each link | `<a routerLink="..." routerLinkActive="...">` |
| Link padding | `px-4 py-2` |
| Link text | `text-sm font-medium text-white/60 hover:text-white` |
| Link hover bg | `hover:bg-white/10 rounded-lg` |
| Active state | `bg-white/15 text-white rounded-lg ring-1 ring-white/20` |
| Transition | `transition-colors duration-200` |
| Min touch target | Each link must be at least 44px tall (the py-2 + text gives ~36px, so use `min-h-[44px] flex items-center`) |

Nav items (labels for desktop):

| Label | Route |
|---|---|
| Tableau de bord | /dashboard |
| Mon jardin | /garden |
| Taches | /tasks |
| Associations | /companions |

**(REC)** Consider adding a subtle SVG icon (16x16) before each label on desktop for visual scanning. Icons should be simple line-style (not filled). If icons are added, use `gap-2` between icon and text.

### 3.5 Mobile hamburger button (left side, visible < md)

**(R)** All items in this section.

| Property | Value |
|---|---|
| Visibility | `md:hidden` |
| Size | `h-11 w-11` (44x44 touch target) |
| Style | `flex items-center justify-center rounded-lg text-white/80 hover:bg-white/10 hover:text-white` |
| Icon | Hamburger SVG (3 horizontal lines) when closed, X icon when open |
| Aria | `aria-label="Ouvrir le menu"` / `"Fermer le menu"`, `aria-expanded` bound to open state |
| Position | Left side of header, before the brand text |

### 3.6 Mobile navigation panel

**(R)** All items in this section.

When the hamburger is tapped, a panel slides down below the header. It does not push the page content -- it overlays on top of it.

| Property | Value |
|---|---|
| Visibility | Only when `menuOpen` signal is true and screen < md |
| Position | `fixed top-[56px] left-0 right-0` (directly below the mobile header) |
| Z-index | `z-[105]` |
| Background | `bg-forest` (same as header, seamless) |
| Border | `border-t border-white/10` on top edge |
| Animation | Slide down from 0 height with `transition-all duration-300 ease-in-out`. When closed: `max-h-0 overflow-hidden opacity-0`. When open: `max-h-[300px] opacity-100`. |
| Shadow | `shadow-lg` on the panel bottom edge |

**Nav links inside the panel:**

| Property | Value |
|---|---|
| Layout | `flex flex-col` |
| Link padding | `px-6 py-4` (generous touch targets) |
| Link text | `text-base font-medium text-white/70` |
| Link hover | `hover:bg-white/10 hover:text-white` |
| Active link | `bg-white/15 text-white border-l-3 border-canopy` |
| Dividers | `border-b border-white/5` between items |
| Behavior | Clicking a link closes the panel and navigates |

### 3.7 Mobile backdrop overlay

**(R)** When the mobile menu panel is open, a semi-transparent overlay covers the content behind it.

| Property | Value |
|---|---|
| Visibility | Only when `menuOpen` is true and screen < md |
| Position | `fixed inset-0` |
| Z-index | `z-[100]` (below the header z-[100] and panel z-[105]) |
| Background | `bg-black/40 backdrop-blur-sm` |
| Behavior | Clicking the overlay closes the menu |
| Transition | `transition-opacity duration-300` |

**(R)** When the overlay is visible, set `inert` attribute on the `<main>` content area to trap focus in the header/panel region.

### 3.8 Shell component changes

**(R)** Rename `sidebarOpen` signal to `menuOpen`.
**(R)** Rename `toggleSidebar()` to `toggleMenu()`, `closeSidebar()` to `closeMenu()`.
**(R)** Remove the `activePageLabel` computed signal (it was only used in the mobile topbar subtitle; no longer needed).
**(R)** Remove the `emoji` field from `NavItem` interface. Replace with optional `icon` field if adding SVG icons.

---

## 4. Page content area

### 4.1 Main content wrapper

**(R)** The `<main>` element sits below the fixed header.

| Property | Mobile | md | lg |
|---|---|---|---|
| Padding top | `pt-14` (56px for header) | `pt-16` (64px) | `pt-16` |
| Background | `bg-parchment` | `bg-parchment` | `bg-parchment` |
| Min height | `min-h-screen` | `min-h-screen` | `min-h-screen` |

### 4.2 Content container (inside `<main>`)

**(R)** The content is centered with a max width and horizontal padding.

| Property | Mobile | md | lg |
|---|---|---|---|
| Max width | none (full-width) | `max-w-5xl` (1024px) | `max-w-6xl` (1152px) |
| Margin | `mx-auto` | `mx-auto` | `mx-auto` |
| Padding x | `px-4` (16px) | `px-6` (24px) | `px-8` (32px) |
| Padding y | `py-6` (24px) | `py-8` (32px) | `py-10` (40px) |

**(R)** Update `$content-max-width` to `1200px` in `_variables.scss`.

### 4.3 Page header pattern

**(REC)** Every page (except Dashboard which has its own hero) follows this standard header pattern:

```
[category label]         [optional action button]
[Page heading h1]
```

| Element | Classes |
|---|---|
| Container | `mb-6 md:mb-8 flex flex-col sm:flex-row sm:items-end sm:justify-between gap-4` |
| Category label | `text-xs font-semibold uppercase tracking-widest text-canopy-muted` |
| Heading h1 | `font-['DM_Serif_Display'] text-2xl sm:text-3xl lg:text-4xl text-forest` |
| Action button | Aligned right on sm+, full-width on mobile |

---

## 5. Dashboard page

**Route:** `/dashboard`
**Purpose:** Landing page. Welcomes the user and provides quick navigation to features.

### 5.1 Layout

```
+------------------------------------------+
| HEADER (fixed)                           |
+------------------------------------------+
|                                          |
|  +------------------------------------+  |
|  |  Hero section                      |  |
|  |  Welcome text + seasonal badge     |  |
|  +------------------------------------+  |
|                                          |
|  [Card 1]  [Card 2]  [Card 3]           |
|                                          |
+------------------------------------------+
```

### 5.2 Hero section

**(R)** Structure and responsive behavior. **(S)** Exact gradient stops.

| Property | Mobile | md | lg |
|---|---|---|---|
| Margin bottom | `mb-6` | `mb-8` | `mb-10` |
| Border radius | `rounded-xl` | `rounded-2xl` | `rounded-2xl` |
| Padding | `px-5 py-8` | `px-8 py-10` | `px-10 py-14` |
| Background | `bg-gradient-to-br from-forest via-forest-light to-canopy` | same | same |

**Hero content:**

| Element | Spec |
|---|---|
| Welcome label | `text-xs sm:text-sm font-medium uppercase tracking-widest text-sage` |
| Heading h1 | `font-['DM_Serif_Display'] text-2xl sm:text-3xl lg:text-5xl leading-snug text-white` |
| Description | `mt-3 md:mt-4 text-sm md:text-base leading-relaxed text-white/70 max-w-xl` |
| Seasonal badges | `mt-6 md:mt-8 flex flex-wrap items-center gap-2` |
| Each badge | `rounded-full bg-white/10 px-3 sm:px-4 py-1.5 text-xs sm:text-sm text-white/80 ring-1 ring-white/20` |

**(R)** Remove emojis from badges. Use plain text only (e.g., "Printemps" and "14 Mar 2026").

### 5.3 Feature cards

**(R)** Responsive grid. **(REC)** Card content structure.

| Property | Mobile | sm (640px+) | lg |
|---|---|---|---|
| Grid | `grid gap-4` (stacked) | `grid-cols-2 gap-5` | `grid-cols-3 gap-5` |
| Third card on sm | `sm:col-span-2` | -- | `lg:col-span-1` |

**Each card:**

| Property | Value |
|---|---|
| Container | `<a routerLink="...">` (entire card is a link) |
| Background | `bg-cream` |
| Border | `border border-border-green` |
| Radius | `rounded-xl` |
| Padding | `px-5 py-5 md:px-6 md:py-6` |
| Shadow | `shadow-sm` |
| Hover | `hover:-translate-y-0.5 hover:shadow-md hover:border-canopy/40 transition-all duration-200` |
| Focus | `focus:outline-none focus:ring-2 focus:ring-canopy focus:ring-offset-2 focus:ring-offset-parchment` |
| Icon | Replace emoji with a styled SVG icon (24x24) in a `h-10 w-10 md:h-11 md:w-11` container with `rounded-xl bg-parchment ring-1 ring-canopy/15`. Use `text-canopy` color for the icon. |
| Category | `text-xs font-semibold uppercase tracking-widest text-canopy-muted` |
| Title | `mt-1 font-['DM_Serif_Display'] text-xl md:text-2xl text-forest` |
| Description | `mt-2 text-sm leading-relaxed text-earth` |

**(R)** Replace emoji icons with SVG icons:
- Mon jardin: Leaf/sprout icon
- Associations: Handshake or two-leaves icon
- Taches: Clipboard/checklist icon

---

## 6. Mon Jardin page

**Route:** `/garden`
**Purpose:** CRUD management of user gardens (list, create, delete).

### 6.1 Layout

```
+------------------------------------------+
| HEADER                                   |
+------------------------------------------+
|                                          |
|  [Manage]            [+ Add Garden btn]  |
|  [My Gardens]                            |
|                                          |
|  +------------------------------------+  |
|  | Garden table or card list          |  |
|  +------------------------------------+  |
|                                          |
+------------------------------------------+
```

### 6.2 Page header

Uses the standard page header pattern from section 4.3.

| Element | Spec |
|---|---|
| Category label | "Gestion" |
| Heading | "Mes jardins" |
| Action button | "Ajouter un jardin" button, right-aligned on sm+ |

**Add Garden button:**

| Property | Value |
|---|---|
| Mobile | Full-width: `w-full sm:w-auto` |
| Style | `flex items-center justify-center gap-2 rounded-xl bg-earth px-5 py-2.5 text-sm font-semibold text-white shadow-md` |
| Hover | `hover:bg-earth-dark hover:-translate-y-0.5 hover:shadow-lg` |
| Focus | `focus:outline-none focus:ring-2 focus:ring-earth focus:ring-offset-2 focus:ring-offset-parchment` |
| Icon | Replace `<mat-icon>add</mat-icon>` with an inline SVG plus icon (16x16) |
| Min height | `min-h-[44px]` |

### 6.3 Garden list -- responsive strategy

**(R)** The Mat Table is hard to read on mobile. Use a dual layout:

**Desktop (md+):** Keep the Mat Table with current column structure (name, description, actions).

**Mobile (< md):** Render gardens as a stacked card list instead of a table. Each card shows:

```
+------------------------------------+
| Garden name                    [X] |
| Description text (or em dash)      |
+------------------------------------+
```

**Implementation approach:** Use `@if` with a responsive signal or CSS `hidden md:block` / `md:hidden` to show/hide the table vs. card list.

### 6.4 Desktop table (md+)

**(REC)** Keep the existing Mat Table structure. Adjust these styles:

| Property | Value |
|---|---|
| Container | `hidden md:block overflow-hidden rounded-xl border border-border-green bg-cream shadow-sm` |
| Header row bg | `bg-parchment` |
| Header text | `text-xs font-semibold uppercase tracking-widest text-canopy` |
| Cell text (name) | `font-medium text-forest` |
| Cell text (description) | `text-sm text-earth` |
| Row hover | `hover:bg-parchment/60 transition-colors` |
| Delete button | `h-11 w-11` (44px touch target), `rounded-lg text-bark-muted hover:bg-red-50 hover:text-red-600 focus:outline-none focus:ring-2 focus:ring-red-300` |
| Delete icon | Replace `<mat-icon>delete</mat-icon>` with an inline SVG trash icon |

### 6.5 Mobile card list (< md)

**(R)** New component or template block for mobile garden display.

| Property | Value |
|---|---|
| Container | `md:hidden flex flex-col gap-3` |
| Each card | `rounded-xl border border-border-green bg-cream px-4 py-4 shadow-sm` |
| Card layout | `flex items-start justify-between gap-3` |
| Name | `text-sm font-medium text-forest` |
| Description | `mt-1 text-sm text-earth` (or show em dash if empty) |
| Delete button | `shrink-0 h-11 w-11 flex items-center justify-center rounded-lg text-bark-muted hover:bg-red-50 hover:text-red-600` |

### 6.6 Empty state

**(REC)** Keep the current empty state design but:
- Replace the emoji with an SVG illustration (a simple leaf/sprout in a circle)
- Ensure the "create" button is full-width on mobile: `w-full sm:w-auto`

### 6.7 Loading state

**(R)** Keep the Mat Spinner. Center it vertically with `py-16 md:py-20`.

---

## 7. Taches page

**Route:** `/tasks`
**Purpose:** Placeholder page for an upcoming task management feature.

### 7.1 Layout

```
+------------------------------------------+
| HEADER                                   |
+------------------------------------------+
|                                          |
|  [Upcoming]                              |
|  [Taches]                                |
|                                          |
|  +------------------------------------+  |
|  | Empty state illustration           |  |
|  | "No tasks yet"                     |  |
|  | "Coming soon" badge                |  |
|  +------------------------------------+  |
|                                          |
+------------------------------------------+
```

### 7.2 Page header

Standard page header pattern:
- Category: "A venir"
- Heading: "Taches"

### 7.3 Empty state

**(REC)** Adjust for mobile:

| Property | Mobile | md+ |
|---|---|---|
| Padding y | `py-16` | `py-24` |
| Icon container | `h-14 w-14 md:h-16 md:w-16 rounded-xl md:rounded-2xl` |
| Heading | `text-xl md:text-2xl` |
| Description max-w | `max-w-xs` | `max-w-xs` |

**(R)** Replace the checkmark emoji with an SVG clipboard/checklist icon.

### 7.4 "Coming soon" badge

| Property | Value |
|---|---|
| Style | `rounded-full bg-canopy/10 px-4 py-1.5 text-xs font-semibold text-canopy` |
| Text | "Fonctionnalite a venir" |

---

## 8. Associations page

**Route:** `/companions`
**Purpose:** Search plants and view companion planting data (good companions with guild badges, plants to avoid).

### 8.1 Layout

```
+------------------------------------------+
| HEADER                                   |
+------------------------------------------+
|                                          |
|  [h1: Associations de plantes]           |
|  [description text]                      |
|                                          |
|  [Search input with dropdown]            |
|                                          |
|  [Selected plant chips]                  |
|                                          |
|  [Good companions]  [Plants to avoid]    |
|  (2 cols on lg, stacked on mobile)       |
|                                          |
+------------------------------------------+
```

### 8.2 Page header

**(REC)** Adapt to standard pattern:
- Category: "Permaculture"
- Heading: "Associations de plantes"
- Description: Below heading, `text-sm text-earth max-w-2xl`

### 8.3 Search input

**(R)** The search input is already well-designed. Adjust for mobile:

| Property | Mobile | md+ |
|---|---|---|
| Margin bottom | `mb-4` | `mb-6` |
| Input padding | `py-3 pl-11 pr-10` | same |
| Border radius | `rounded-xl` | `rounded-xl` |
| Font size | `text-sm` | `text-sm` |

**(R)** Ensure the clear button has a 44x44 minimum touch target. Currently it relies on the input height -- verify the button `<button>` itself has `min-h-[44px] min-w-[44px]`.

### 8.4 Search dropdown

**(R)** Responsive adjustments:

| Property | Value |
|---|---|
| Max height | `max-h-[280px]` on mobile, `max-h-[320px]` on md+ |
| Item padding | `px-4 py-3.5` on mobile for larger touch targets |
| Z-index | `z-20` (unchanged) |

### 8.5 Selected plant chips

**(REC)** Keep current design. Adjustments:

| Property | Value |
|---|---|
| Container | `mb-6 md:mb-8 flex flex-wrap items-center gap-2` |
| Chip remove button | Ensure `min-h-[44px] min-w-[44px]` including padding. Current `h-6 w-6` is too small. Increase to `h-8 w-8` or add padding around the chip. |
| "Clear all" button | `min-h-[44px]` for touch target |

### 8.6 Empty state (no plants selected)

**(REC)** Adjust for mobile:

| Property | Mobile | md+ |
|---|---|---|
| Padding | `px-5 py-12` | `px-6 py-20` |
| Icon container | `h-16 w-16 md:h-20 md:w-20` |
| Heading | `text-lg md:text-xl` |
| Description | `text-sm max-w-sm` |

**(R)** Replace the plant SVG path with a cleaner version or keep as-is. No emojis.

### 8.7 Results grid

| Property | Mobile | lg |
|---|---|---|
| Layout | `grid gap-6` (stacked) | `grid-cols-2 gap-8` |
| Section heading | `text-lg md:text-xl` |

### 8.8 Good companion cards

**(REC)** Keep current card design. Mobile adjustments:

| Property | Mobile | md+ |
|---|---|---|
| Card padding | `px-4 py-3.5` | `px-5 py-4` |
| Plant name | `text-sm font-medium text-forest` | same |
| Scientific name | Below plant name (stacked) on mobile; inline on md+ |
| Guild badges | `flex flex-wrap gap-1.5` -- allow wrapping |

**(R)** Guild badge tooltips: On mobile, tooltips (hover-based) do not work. Use `(click)` to toggle visibility on mobile, or use Angular Material tooltip (`matTooltip`) which handles touch. The current CSS-only hover tooltip is inaccessible on touch devices.

### 8.9 Plants-to-avoid cards

**(REC)** Keep current design. Same mobile padding adjustments as good companion cards.

### 8.10 Loading state

**(R)** Keep the spinner. Adjust `py-12 md:py-16`.

---

## 9. Dialogs

### 9.1 Create Garden dialog

**(REC)** The Angular Material dialog is fine. Adjustments:

| Property | Value |
|---|---|
| Width | `width: '90vw', maxWidth: '480px'` (responsive on small screens) |
| Dialog title | Keep `font-['DM_Serif_Display'] text-2xl text-forest` |
| Form fields | No changes needed; Mat Form Field handles responsiveness |
| Cancel button | `min-h-[44px]` |
| Submit button | `min-h-[44px]` |
| Button layout on mobile | Stack vertically on very small screens: `flex flex-col-reverse sm:flex-row sm:justify-end gap-3` |

### 9.2 Confirm dialog (delete)

**(REC)** Same responsive treatment as create dialog:
- `width: '90vw', maxWidth: '400px'`
- `min-h-[44px]` on both buttons
- Stack buttons vertically on small screens

---

## 10. Accessibility checklist

**(R)** All items in this section are required (WCAG 2.1 AA).

### Color contrast

| Pair | Foreground | Background | Contrast ratio | Passes |
|---|---|---|---|---|
| Body text on parchment | #1c2b1c | #f8f4ef | 12.8:1 | Yes (AAA) |
| White on forest | #ffffff | #1e3d1e | 11.2:1 | Yes (AAA) |
| White/60 on forest | rgba(255,255,255,0.6) | #1e3d1e | ~5.8:1 | Yes (AA normal text) |
| Canopy-muted on parchment | #5a8a50 | #f8f4ef | 3.6:1 | Yes (AA large text only) |
| Earth on cream | #6b4226 | #fefcf8 | 7.5:1 | Yes (AAA) |

**(R)** The category labels (canopy-muted on parchment at 3.6:1) qualify as large text at the current `text-xs uppercase tracking-widest` styling only if they are bold and at least 14px. At 12px (`text-xs`) they do not meet AA for normal text. **Fix:** Change category labels to `text-canopy` (#4a7c3f) which gives 4.6:1 on parchment, passing AA for normal text. Or increase size to `text-sm` at 14px bold.

### Keyboard navigation

- **(R)** All nav links, buttons, and form controls must be reachable via Tab key
- **(R)** Mobile menu must trap focus when open (use `inert` on `<main>`)
- **(R)** Focus indicators: every interactive element must show a visible focus ring. Use `focus:outline-none focus:ring-2 focus:ring-canopy focus:ring-offset-2` as the standard pattern. For elements on dark backgrounds (header), use `focus:ring-white/50`
- **(R)** Escape key closes the mobile menu
- **(R)** Dialog focus management is handled by Angular Material (no changes needed)

### Touch targets

- **(R)** Every button, link, and interactive element must be at least 44x44px
- **(R)** Specific items to verify:
  - Header nav links on desktop (add `min-h-[44px]`)
  - Hamburger button (already 44px -- good)
  - Delete button in garden table (already `h-11 w-11` -- good)
  - Chip remove buttons (currently `h-6 w-6` -- must increase)
  - "Clear all" button in chips area
  - Dialog action buttons

### Labels and ARIA

- **(R)** Mobile menu button: `aria-label`, `aria-expanded`, `aria-controls="mobile-nav"`
- **(R)** Mobile nav panel: `id="mobile-nav"`, `role="navigation"`, `aria-label="Menu principal"`
- **(R)** Desktop nav: `<nav aria-label="Navigation principale">`
- **(R)** Search input has associated `<label>` (currently sr-only -- good, keep it)
- **(R)** Delete buttons have `aria-label` (currently present -- good, keep it)

### Reduced motion

**(REC)** Wrap all CSS transitions/animations in a `prefers-reduced-motion` media query fallback. With Tailwind, use `motion-safe:` prefix on transition/animation utilities, or add a global rule:

```css
@media (prefers-reduced-motion: reduce) {
  *, *::before, *::after {
    animation-duration: 0.01ms !important;
    transition-duration: 0.01ms !important;
  }
}
```

---

## 11. Migration notes

### Files to modify

| File | Change |
|---|---|
| `src/app/layout/shell/shell.ts` | Remove sidebar logic, add header/menu logic, update NavItem interface, rename signals |
| `src/app/layout/shell/shell.html` | Complete rewrite: remove sidebar markup, add header with horizontal nav + mobile menu |
| `src/app/layout/shell/shell.scss` | Remove sidebar styles; should remain minimal (`:host { display: block; }`) since Tailwind handles styling |
| `src/tailwind.css` | Add new color tokens for header |
| `src/styles/abstracts/_variables.scss` | Remove `$sidebar-width`, `$shadow-sidebar`; add `$header-height`, `$z-header`, etc.; update `$content-max-width` |
| `src/app/features/dashboard/dashboard.html` | Replace emojis with SVGs, adjust responsive padding, responsive grid |
| `src/app/features/garden/garden.html` | Add mobile card list alongside table, replace mat-icons with SVGs, responsive adjustments |
| `src/app/features/garden/garden.ts` | Update dialog config for responsive widths |
| `src/app/features/tasks/tasks.html` | Replace emoji, adjust responsive padding |
| `src/app/features/companions/companions.ts` | Mobile tooltip solution for guild badges, touch target fixes on chip remove buttons, responsive padding |
| `src/app/features/garden/create-garden-dialog/create-garden-dialog.html` | Responsive button layout |
| `src/app/shared/confirm-dialog/confirm-dialog.html` | Responsive button layout |

### Files that do NOT change

- `src/app/app.routes.ts` -- route structure unchanged
- `src/app/app.config.ts` -- no config changes
- `src/app/core/auth/*` -- no auth changes
- `src/app/api/*` -- no API changes
- Service files (`garden.service.ts`, `companion-search.service.ts`) -- no logic changes

### Suggested implementation order

1. **Tailwind tokens + Sass variables** -- add new tokens, remove old ones
2. **Shell component** -- header + mobile menu (this unblocks all pages)
3. **Dashboard** -- hero + cards responsive pass
4. **Mon Jardin** -- mobile card list + table adjustments
5. **Taches** -- minor responsive pass
6. **Associations** -- touch target fixes + tooltip solution
7. **Dialogs** -- responsive width + button stacking
8. **Accessibility audit** -- verify contrast, focus, touch targets across all pages

### SVG icon strategy

**(REC)** Rather than including SVG markup inline in every template, create a small set of reusable SVG icon components or use Angular Material's `MatIconRegistry` to register custom SVG icons. This keeps templates cleaner and icons consistent. However, inline SVGs are also acceptable for a small icon set (under 10 icons).

Icons needed across the app:
- Leaf / sprout (brand, garden feature card, empty states)
- Clipboard / checklist (tasks feature card, tasks empty state)
- Handshake or linked leaves (associations feature card)
- Hamburger menu (mobile nav)
- Close / X (mobile nav, chip remove, search clear)
- Plus (add garden button)
- Trash (delete garden)
- Search magnifier (companions search -- already SVG)
- Star (companion score -- already SVG)
- Checkmark (good companions icon -- already SVG)
- Warning triangle (plants to avoid -- already SVG)

---

## Appendix: ASCII wireframes

### Desktop layout (lg+)

```
+======================================================================+
| [leaf] Garden Assistant   Tableau de bord  Mon jardin  Taches  Assoc |  <- fixed header, 64px
+======================================================================+
|                                                                      |
|      +------------------------------------------------------+       |
|      |                                                      |       |
|      |   Page content (max-w-6xl, centered)                 |       |
|      |                                                      |       |
|      +------------------------------------------------------+       |
|                                                                      |
|      bg-parchment fills full viewport                                |
+----------------------------------------------------------------------+
```

### Mobile layout (< 768px)

```
+---------------------------+
| [=] Garden Assistant      |  <- fixed header, 56px
+---------------------------+
|                           |
|  Page content             |
|  (full width, px-4)       |
|                           |
+---------------------------+
```

### Mobile with menu open

```
+---------------------------+
| [X] Garden Assistant      |  <- fixed header
+---------------------------+
| > Tableau de bord         |  <- slide-down panel
| > Mon jardin              |
| > Taches                  |
| > Associations            |
+---------------------------+
| (dark overlay)            |
|                           |
| (content behind, inert)   |
|                           |
+---------------------------+
```
