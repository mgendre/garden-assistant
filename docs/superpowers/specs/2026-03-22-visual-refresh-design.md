# Visual Refresh — Stronger Contrast, Neutral Text, Bolder Panels

## Problem

The app uses green for everything — text, borders, shadows, dividers, headings. This creates a flat, monochromatic feel that looks AI-generated. Muted text (`#5a7a5a`) fails WCAG AA contrast. Borders are nearly invisible (`rgba(45,106,79, 0.08)`). Font sizes are too small in many places.

## Solution

Decouple neutral UI chrome (text, borders, shadows) from the brand green. Reserve green for high-signal moments only (buttons, active states, success badges). Use warm neutral grays for everything else.

## Scope

**In scope:**
- Design token updates in `_variables.scss`
- Heading color change in `_typography.scss`
- Panel border/shadow in `_panels.scss`
- Replace `#d8f3dc` dividers with neutral gray
- Replace `rgba(45,106,79, low-opacity)` borders with tokens
- Font size bumps for small text
- Remove `:root` overrides in `_reset.scss`

**Out of scope:**
- Layout changes
- Component restructuring
- New features

## Design Tokens

### Colors

| Token | Before | After |
|---|---|---|
| `--color-text` | `#1a2e1a` (green-black) | `#2c2c2c` (warm charcoal) |
| `--color-text-muted` | `#5a7a5a` (green, fails WCAG) | `#6b7280` (gray-500, passes WCAG) |
| `--color-text-secondary` | (new) | `#4b5563` (gray-600) |
| `--color-text-dark` | `#333333` | `#1f2937` (gray-800) |
| `--color-border` | `#c2e0c8` (green) | `#d1d5db` (gray-300) |
| `--color-border-subtle` | (new) | `#e5e7eb` (gray-200) |
| `--color-divider` | `#e0f0dc` (green) | `#e5e7eb` (gray-200) |
| `--color-surface` | (new) | `#ffffff` |
| `--color-surface-hover` | (new) | `#f3f4f6` (gray-100) |

### Shadows

| Token | Before | After |
|---|---|---|
| `--shadow-sm` | `0 1px 4px rgba(30,61,30,0.08)` | `0 1px 3px rgba(0,0,0,0.06)` |
| `--shadow-base` | `0 2px 16px rgba(30,61,30,0.10)` | `0 2px 8px rgba(0,0,0,0.08), 0 4px 16px rgba(0,0,0,0.04)` |
| `--shadow-focus` | keep green | keep green (accessibility) |

### What stays green

- Primary buttons/CTAs (`--color-forest` bg)
- Active nav items
- Success badges (`.badge-positive`, `.badge-compat-good`)
- Active/selected chips
- Focus rings (`--shadow-focus`)
- Semantic colors (`--color-success`, `--color-forest`, `--color-canopy`)

## Typography

Headings change from `color: var(--color-forest)` to `color: var(--color-text)`.

### Font size minimums

| Element | Before | After |
|---|---|---|
| Labels (uppercase) | `0.625-0.6875rem` | `0.75rem` |
| Badges/chips | `0.6875rem` | `0.75rem` |
| Descriptions | `0.875rem` | `0.9375rem` |
| Mechanism icon | `0.55rem` | `0.625rem` |

## Panels

### Before
```scss
border: 1px solid rgba(45, 106, 79, 0.1);
box-shadow: 0 1px 3px rgba(26, 58, 42, 0.05), 0 8px 24px rgba(26, 58, 42, 0.04);
```

### After
```scss
border: 1px solid var(--color-border);
box-shadow: var(--shadow-base);
```

## Dividers

All `#d8f3dc` and `rgba(45,106,79, low-opacity)` used as borders/dividers are replaced with `var(--color-border-subtle)` or `var(--color-border)`.

## Implementation order

1. `_variables.scss` — update tokens
2. `_reset.scss` — remove `:root` overrides
3. `_typography.scss` — heading color
4. `_panels.scss` — border + shadow
5. Replace `#d8f3dc` everywhere
6. Replace `rgba(45,106,79,...)` borders/dividers
7. Font size bumps
8. Build and verify
