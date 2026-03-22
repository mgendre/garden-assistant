# OAuth Login UX Specification

**Date:** 2026-03-22
**Status:** Draft
**Companion spec:** `2026-03-22-oauth-login-design.md` (technical design)

---

## Overview

Three new screens support the OAuth login flow:

1. **Login page** (`/login`) -- entry point for unauthenticated users
2. **Email consent screen** (within `/auth/callback`) -- shown once to new users
3. **Profile page email consent toggle** (`/profile`) -- lets users change their preference

Design goals: welcoming, garden-themed, simple. The login page is the first impression of the app. It must feel warm and inviting -- not like a corporate SSO wall.

---

## Screen 1: Login page

### Route: `/login`
### Purpose: Let unauthenticated users sign in with Google or Discord.

### Layout

This page renders **outside** the shell (no header, no nav). It is a standalone full-viewport layout.

```
+----------------------------------------------------------+
|                                                          |
|                                                          |
|              [Logo leaf] PermaGarden                     |
|                                                          |
|              Votre jardin vous attend.                   |
|              (subtitle text)                             |
|                                                          |
|         +------------------------------------+           |
|         |  [Google icon]  Continuer avec Google  |       |
|         +------------------------------------+           |
|                                                          |
|         +------------------------------------+           |
|         |  [Discord icon] Continuer avec Discord |       |
|         +------------------------------------+           |
|                                                          |
|              (footer text: privacy note)                 |
|                                                          |
+----------------------------------------------------------+
```

**Mobile (default):** Single column, vertically centered. Content area has max-width 360px, centered horizontally. Generous vertical padding (spacing x8 top) ensures it does not feel cramped.

**Tablet (md: 768px+):** Same centered layout. Content area widens slightly to max-width 400px.

**Desktop (lg: 1024px+):** Same centered layout. Optionally, the background could show a subtle decorative illustration (a faded botanical sketch at one side), but this is a **suggestion** -- the centered card approach works well at all sizes without it.

### Background

The page background uses `--color-parchment` (#faf8f4). A very subtle radial gradient tinted with `--color-moss-tint` (#e4f0e6) adds depth without distraction:

```css
background: radial-gradient(ellipse at 50% 0%, var(--color-moss-tint) 0%, var(--color-parchment) 60%);
```

This is **recommended** -- it adds warmth. The fallback is flat `--color-parchment`.

### Components

#### Logo block
- Reuses the existing `.logo-leaf` and `.logo-text` / `.logo-accent` classes from `_header.scss`
- Displayed larger than in the header: the leaf icon at 48x48px, the text at `2rem` (mobile) / `2.5rem` (md+)
- Centered horizontally
- The leaf, text, and accent color remain identical to the header logo so users recognize the brand

#### Tagline
- Text: translation key `Login.Tagline`
- Uses `--font-display` (DM Serif Display), `--color-forest` (#1a5c1e)
- Size: `1.125rem` mobile, `1.25rem` md+
- Centered, placed below the logo with `margin-top: 8px`

#### Subtitle
- Text: translation key `Login.Subtitle`
- Uses `--font-body` (Lato), `--color-text-muted` (#5a7a5a)
- Size: `0.875rem`
- Centered, placed below the tagline with `margin-top: 4px`

#### Social login buttons
- Two buttons, stacked vertically with a `12px` gap
- Full width within the content area (max-width 360px on mobile / 400px on md+)
- Height: `48px` (meets 44px touch target requirement)
- Border-radius: `var(--radius-base)` (12px) -- rounded but not pill-shaped, to feel substantial
- Font: `--font-body`, `font-weight: 600`, `0.9375rem` (15px)
- Each button has the provider's brand icon (SVG, 20x20px) on the left, with `12px` gap to text
- Transitions: `background var(--transition-fast), transform var(--transition-fast), box-shadow var(--transition-fast)`

**Google button:**
- Default: `background: white`, `border: 1px solid #dadce0`, `color: #3c4043`
- Hover: `background: #f8f9fa`, `box-shadow: 0 1px 3px rgba(0,0,0,0.12)`
- This follows Google's own branding guidelines for sign-in buttons (white background, dark text)

**Discord button:**
- Default: `background: #5865F2`, `border: 1px solid #5865F2`, `color: white`
- Hover: `background: #4752C4`, `box-shadow: 0 2px 8px rgba(88,101,242,0.3)`
- Uses Discord's brand color per their brand guidelines

**Focus state (both):**
- `outline: none`, `box-shadow: var(--shadow-focus)` (the green focus ring from the design system)

**Disabled state (during redirect):**
- `opacity: 0.6`, `cursor: not-allowed`, `pointer-events: none`
- The clicked button shows a small spinner (CSS-only, 16x16px) replacing the provider icon

#### Privacy note (footer)
- Text: translation key `Login.PrivacyNote`
- Placed below buttons with `margin-top: 24px`
- Size: `0.75rem`, color: `--color-text-muted`, centered
- Max-width: 320px, `line-height: 1.5`

### States

| State | Behavior |
|---|---|
| Default | Two buttons visible, no loading indicators |
| Loading (after click) | Clicked button shows spinner, both buttons disabled. This prevents double-clicks during the redirect |
| Error (redirected back with error) | A dismissible alert appears above the buttons. Uses `--color-danger` text on a light red background (`rgba(155, 28, 28, 0.06)` bg, `1px solid rgba(155, 28, 28, 0.15)` border). Translation key: `Login.Error` |

### New CSS needed

A new Sass partial `src/styles/pages/_login.scss` should be created and added to `main.scss` under the Pages section. This keeps login-specific styles (background gradient, button sizing, logo enlargement) isolated. The social button styles could alternatively live in `src/styles/components/_buttons.scss` as `.btn-social`, `.btn-social-google`, `.btn-social-discord` if they might be reused elsewhere -- this is a **suggestion**.

### Interaction notes

- Clicking a social button navigates the browser to `GET /api/auth/oauth/{provider}/login`. This is a full-page navigation (not an XHR call), so the Angular app unloads. The loading state only needs to last a moment before the browser redirects.
- If the user arrives at `/login` but already has a valid token, the guard should redirect them to `/companions` (the default authenticated route). No flash of the login page.

---

## Screen 2: Email consent screen

### Route: `/auth/callback` (conditional content for new users)
### Purpose: Ask new users whether they consent to storing their email address.

### Layout

This screen also renders **outside** the shell (no header, no nav), since the user is not yet fully authenticated.

```
+----------------------------------------------------------+
|                                                          |
|              [Logo leaf] PermaGarden                     |
|                                                          |
|         +------------------------------------+           |
|         |  panel                             |           |
|         |                                    |           |
|         |  Bienvenue !                       |           |
|         |  (welcome heading)                 |           |
|         |                                    |           |
|         |  user@example.com                  |           |
|         |  (email from provider)             |           |
|         |                                    |           |
|         |  [x] Stocker mon adresse email     |           |
|         |  (checkbox + label)                |           |
|         |                                    |           |
|         |  (explanation paragraph)           |           |
|         |                                    |           |
|         |  [    Continuer    ]               |           |
|         |  (primary action button)           |           |
|         +------------------------------------+           |
|                                                          |
+----------------------------------------------------------+
```

**Mobile (default):** Centered vertically and horizontally. Panel has max-width 400px, horizontal margin 16px (so effective max on small screens is screen width minus 32px).

**Tablet / Desktop:** Same centered layout, panel stays at max-width 440px.

### Background

Same as the login page: `--color-parchment` with the optional radial gradient.

### Components

#### Logo block (small)
- Same logo as login page but smaller: leaf at 36x36px, text at `1.5rem`
- Centered above the panel with `margin-bottom: 24px`
- Provides brand continuity between the login redirect and this screen

#### Consent panel
- Uses the existing `.panel` class (white background, rounded corners, subtle shadow and border)
- Padding: `24px` mobile, `32px` md+

#### Welcome heading
- Text: translation key `EmailConsent.Welcome`
- Uses heading styles (h2 -- `--font-display`, `--color-forest`)
- `margin-bottom: 16px`

#### Email display
- Shows the user's email from the provider
- Styled as a read-only info block: `--color-parchment` background, `1px solid var(--color-border)`, `border-radius: var(--radius-sm)` (6px), `padding: 10px 14px`
- Font: `--font-body`, `0.9375rem`, `--color-text`, `font-weight: 500`
- A small mail icon (SVG or FontAwesome) to the left, `--color-canopy` color
- `margin-bottom: 20px`

#### Checkbox with label
- **Required:** Uses a native `<input type="checkbox">` with an associated `<label>` element
- Checkbox is **pre-checked** (per spec: consent defaults to true)
- Label text: translation key `EmailConsent.StoreEmailLabel`
- Font: `--font-body`, `0.875rem`, `--color-text`
- The checkbox itself should be styled to match the garden theme: `--color-canopy` (#519a66) when checked, with a smooth transition. Use `accent-color: var(--color-canopy)` for simple native styling, or a custom checkbox with a visible checkmark
- Touch target: the `<label>` wraps or is `for`-associated with the checkbox, giving a large click area. Minimum 44px row height
- `margin-bottom: 12px`

#### Explanation text
- Text: translation key `EmailConsent.Explanation`
- Size: `0.8125rem` (13px), color: `--color-text-muted`, `line-height: 1.6`
- `margin-bottom: 24px`
- This explains what storing the email enables (notifications, cross-provider account linking) and what happens if unchecked (each provider creates a separate account, no notifications)

#### Continue button
- Full width within the panel
- Uses the `@mixin btn-primary` style from the design system: `--color-earth` (#ffaa00) background, white text, pill shape (border-radius: 9999px), font-weight 700, `0.875rem`
- Height: `44px` (touch target)
- Text: translation key `EmailConsent.Continue`
- Hover: `--color-earth-dark`, slight lift (`translateY(-1px)`)
- Focus: `box-shadow: var(--shadow-focus)`
- Disabled (during submission): `opacity: 0.6`, shows spinner

### States

| State | Behavior |
|---|---|
| Loading (initial) | While fetching the pending email from the backend: the panel shows a centered spinner or skeleton. No content flicker. Translation key for screen reader: `EmailConsent.Loading` |
| Default | Email displayed, checkbox pre-checked, continue button enabled |
| Submitting | Continue button disabled with spinner, checkbox disabled |
| Error | Inline error message below the continue button. Uses `--color-danger` text. Translation key: `EmailConsent.Error`. Also a "retry" affordance -- the button re-enables |

### Interaction notes

- Arriving at `/auth/callback?code=xxx&isNew=false` (existing user): this screen is **never shown**. The component immediately calls the complete endpoint and redirects to `/companions` on success. A loading spinner is shown during this brief moment.
- Arriving at `/auth/callback?code=xxx&isNew=true` (new user): the email consent screen is displayed.
- Clicking "Continue" calls `completeOAuthLogin(code, storeEmail)`. On success, navigate to `/companions`. On error, show inline error.
- If the user unchecks the checkbox, the explanation text should subtly update to reinforce the consequence. **Suggestion:** swap the explanation text to a variant (translation key `EmailConsent.ExplanationUnchecked`) that specifically says "Each provider will create a separate account and notifications will not be available."

---

## Screen 3: Profile page -- Email consent toggle

### Route: `/profile`
### Purpose: Let users view their account info and change email consent preference.

### Layout

This page renders **inside** the shell (header + nav visible) since the user is authenticated. It follows the standard page pattern.

```
+----------------------------------------------------------+
| [header with nav]                                        |
+----------------------------------------------------------+
| page-container                                           |
|                                                          |
|   page-header                                            |
|   h1: Mon profil                                         |
|   subtitle: Gerez vos preferences                       |
|                                                          |
|   +--------------------------------------------------+   |
|   | panel: Connexions                                |   |
|   | panel-header with panel-title                    |   |
|   |                                                  |   |
|   |  [Google icon] Google     connecte               |   |
|   |  [Discord icon] Discord   non connecte           |   |
|   +--------------------------------------------------+   |
|                                                          |
|   +--------------------------------------------------+   |
|   | panel: Adresse email                             |   |
|   | panel-header with panel-title                    |   |
|   |                                                  |   |
|   |  user@example.com (or "Aucun email stocke")      |   |
|   |                                                  |   |
|   |  [Toggle] Autoriser le stockage de mon email     |   |
|   |                                                  |   |
|   |  (explanation text)                              |   |
|   +--------------------------------------------------+   |
|                                                          |
+----------------------------------------------------------+
```

**Mobile (default):** Single column, full width with `page-container` padding (`px-4 pt-4`). Panels stack vertically with `gap: 20px`.

**Tablet (md: 768px+):** Same single column, wider padding from `page-container` (`px-12 pt-6`). Panels max-width `640px` to avoid overly wide settings rows.

**Desktop (lg: 1024px+):** Same -- profile pages do not need multi-column layout.

### Components

#### Page header
- Reuses `.page-header`, `.page-subtitle` classes
- Title: translation key `Profile.Title` (h1)
- Subtitle: translation key `Profile.Subtitle`

#### Connected providers panel
- Uses `.panel` with `.panel-header` + `.panel-title`
- Panel title: translation key `Profile.ProvidersTitle`
- Lists each provider as a row with:
  - Provider icon (same SVGs as login page, 20x20px)
  - Provider name (text, `--font-body`, `0.875rem`, `font-weight: 600`)
  - Status badge on the right: connected or not connected
    - Connected: uses `.btn-pill .btn-pill-forest` style (green background, green text) with text from `Profile.ProviderConnected`
    - Not connected: uses `.btn-pill .btn-pill-muted` style with text from `Profile.ProviderNotConnected`
  - Rows separated by `1px solid var(--color-divider)`
  - Row padding: `12px 20px`, min-height: `48px` (touch-friendly)
- This panel is **read-only** for now. Linking additional providers is out of scope but the layout accommodates it for the future.

#### Email consent panel
- Uses `.panel` with `.panel-header` + `.panel-title`
- Panel title: translation key `Profile.EmailTitle`
- Content padding: `20px`

**Email display row:**
- If email stored: show the email in `--color-text`, `0.9375rem`, `font-weight: 500`
- If no email stored: show placeholder text from `Profile.NoEmail` in `--color-text-muted`, `0.875rem`, italic
- `margin-bottom: 16px`

**Toggle row:**
- A horizontal row: toggle on the left, label text on the right
- Toggle: use Angular Material `mat-slide-toggle` (already available in the project as a dependency). Style it with `--color-canopy` (#519a66) as the active track color via Angular Material theming or CSS override
- Label: translation key `Profile.EmailToggleLabel`, `--font-body`, `0.875rem`, `--color-text`
- The toggle and label must be associated (`<label>` wrapping or `for` attribute) for accessibility
- `margin-bottom: 12px`

**Explanation text:**
- Text: translation key `Profile.EmailExplanation`
- Size: `0.8125rem`, color: `--color-text-muted`, `line-height: 1.6`
- Same content as the email consent screen: explains what email storage enables

### States

| State | Behavior |
|---|---|
| Loading | Panel bodies show skeleton placeholders (two lines for providers, one line + toggle for email). Use `--color-moss-tint` as the skeleton bar color with a subtle pulse animation |
| Default (consent on) | Email displayed, toggle on, explanation text shown |
| Default (consent off) | "No email stored" placeholder, toggle off, explanation text shown |
| Toggling | Toggle disabled briefly while the API call completes. A snackbar confirms success. Translation keys: `Profile.EmailConsentEnabled` / `Profile.EmailConsentDisabled` |
| Error | Snackbar with error message, toggle reverts to previous state. Translation key: `Profile.EmailConsentError` |

### Interaction notes

- Toggling OFF: the API clears the email and sets `ConsentEmail = false`. The email display immediately updates to the "No email stored" placeholder after the API succeeds.
- Toggling ON: the API sets `ConsentEmail = true`. The email will only be populated on the user's next OAuth login. The email display stays as "No email stored" until then -- this should be communicated. **Recommended:** Add a small info note (translation key `Profile.EmailWillUpdateOnNextLogin`) that appears when consent is on but no email is stored, using the `.info-card` component style.
- The toggle should have a confirmation step when toggling OFF, since it deletes the email. **Recommended:** Use a simple inline confirmation -- when the user toggles off, replace the toggle row temporarily with "Are you sure? [Confirm] [Cancel]" using `btn-pill-danger` for confirm and `btn-pill-muted` for cancel.

---

## Translation keys

All keys use PascalCase. Default language is French (`fr.json`).

### Login

| Key | French text |
|---|---|
| `Login.Tagline` | `Votre jardin vous attend.` |
| `Login.Subtitle` | `Connectez-vous pour commencer` |
| `Login.ContinueWithGoogle` | `Continuer avec Google` |
| `Login.ContinueWithDiscord` | `Continuer avec Discord` |
| `Login.PrivacyNote` | `En vous connectant, vous acceptez nos conditions d'utilisation.` |
| `Login.Error` | `La connexion a echoue. Veuillez reessayer.` |
| `Login.Loading` | `Connexion en cours...` |

### EmailConsent

| Key | French text |
|---|---|
| `EmailConsent.Welcome` | `Bienvenue !` |
| `EmailConsent.StoreEmailLabel` | `Stocker mon adresse email` |
| `EmailConsent.Explanation` | `Votre email permet de recevoir des notifications et de lier vos comptes si vous utilisez plusieurs fournisseurs de connexion.` |
| `EmailConsent.ExplanationUnchecked` | `Sans stockage de l'email, chaque fournisseur creera un compte separe et les notifications ne seront pas disponibles.` |
| `EmailConsent.Continue` | `Continuer` |
| `EmailConsent.Loading` | `Chargement...` |
| `EmailConsent.Error` | `Une erreur est survenue. Veuillez reessayer.` |

### Profile

| Key | French text |
|---|---|
| `Profile.Title` | `Mon profil` |
| `Profile.Subtitle` | `Gerez vos preferences de compte` |
| `Profile.ProvidersTitle` | `Connexions` |
| `Profile.ProviderConnected` | `Connecte` |
| `Profile.ProviderNotConnected` | `Non connecte` |
| `Profile.EmailTitle` | `Adresse email` |
| `Profile.NoEmail` | `Aucun email stocke` |
| `Profile.EmailToggleLabel` | `Autoriser le stockage de mon email` |
| `Profile.EmailExplanation` | `Votre email permet de recevoir des notifications et de lier vos comptes si vous utilisez plusieurs fournisseurs de connexion.` |
| `Profile.EmailConsentEnabled` | `Stockage de l'email active` |
| `Profile.EmailConsentDisabled` | `Email supprime et stockage desactive` |
| `Profile.EmailConsentError` | `Impossible de modifier la preference. Veuillez reessayer.` |
| `Profile.EmailWillUpdateOnNextLogin` | `Votre email sera enregistre lors de votre prochaine connexion.` |
| `Profile.ConfirmDisable` | `Etes-vous sur ? Votre email sera supprime.` |
| `Profile.Confirm` | `Confirmer` |
| `Profile.Cancel` | `Annuler` |

### Nav (addition)

| Key | French text |
|---|---|
| `Nav.Profile` | `Profil` |

---

## Accessibility requirements

### Required (WCAG 2.1 AA)

1. **Colour contrast:**
   - All text on `--color-parchment` background passes 4.5:1. Verified: `--color-text` (#1a2e1a) on `--color-parchment` (#faf8f4) = ~14.5:1. `--color-text-muted` (#5a7a5a) on `--color-parchment` = ~4.7:1 (passes for normal text).
   - White text on `--color-earth` (#ffaa00) button: contrast is ~1.8:1 -- **fails**. Use `--color-text` (#1a2e1a) as button text color instead of white for the Continue button, or darken the button to `#d48a00`. **Decision: use dark text (`#1a2e1a`) on the amber button.** This matches WCAG and the dark-on-amber pattern is visually strong.
   - White text on Discord blue (#5865F2): ~4.6:1 -- passes.
   - Dark text (#3c4043) on white Google button: ~10:1 -- passes.
   - `--color-forest` (#1a5c1e) heading text on `--color-parchment`: ~8.2:1 -- passes.

2. **Keyboard navigation:**
   - Login page: Tab order is Google button, then Discord button. Both focusable with visible focus ring (`--shadow-focus`).
   - Consent screen: Tab order is checkbox, then Continue button.
   - Profile page: Tab order follows DOM order -- toggle, then confirm/cancel if visible.
   - All interactive elements must have `:focus-visible` styles using `--shadow-focus`.

3. **Form labels:**
   - Checkbox on consent screen: `<label for="email-consent">` wrapping or pointing to the input.
   - Toggle on profile page: `<label>` associated with `mat-slide-toggle` via `aria-labelledby` or wrapping.

4. **Touch targets:**
   - Social login buttons: 48px height (exceeds 44px minimum).
   - Checkbox row: label extends the tap area to full row width, row height >= 44px.
   - Toggle row: same treatment.
   - Continue button: 44px height.
   - Provider status rows on profile: 48px min-height.

5. **Screen reader announcements:**
   - Login page: `<main>` landmark, `<h1>` with the brand name ("PermaGarden -- Connexion") for context.
   - Consent screen: `<main>` landmark, `<h1>` "Bienvenue", email read aloud in context.
   - Profile page: standard `<h1>`, panels use `<section>` with `aria-labelledby` pointing to panel titles.
   - Loading states: `aria-live="polite"` region for status updates.
   - Snackbar confirmations: `role="status"` or Angular Material's built-in snackbar accessibility.

6. **Reduced motion:**
   - All transitions respect `prefers-reduced-motion: reduce`. When active, set `transition-duration: 0ms` and disable `transform` effects.

---

## Design system reuse summary

| Existing element | Used where |
|---|---|
| `.logo-leaf`, `.logo-text`, `.logo-accent` | Login page logo, consent screen logo |
| `.panel`, `.panel-header`, `.panel-title` | Consent screen card, profile panels |
| `.page-container`, `.page-header`, `.page-subtitle` | Profile page layout |
| `@mixin btn-primary` | Continue button on consent screen |
| `.btn-pill`, `.btn-pill-forest`, `.btn-pill-muted`, `.btn-pill-danger` | Provider status badges, confirm/cancel actions |
| `.info-card` | "Email will update on next login" note |
| `.empty-state` pattern | Not needed (no empty list state) |
| `--shadow-focus` | Focus rings on all interactive elements |
| `--transition-fast`, `--transition-base` | Button hover/focus transitions |
| `--radius-base`, `--radius-sm` | Button and input rounding |

### New styles to create

| Style | Location | Purpose |
|---|---|---|
| `.login-page` | `pages/_login.scss` | Full-viewport centered layout with background gradient |
| `.login-logo` | `pages/_login.scss` | Enlarged logo variant for login/consent screens |
| `.btn-social` | `components/_buttons.scss` | Base social login button (height, padding, border-radius, icon spacing) |
| `.btn-social-google` | `components/_buttons.scss` | Google brand colors |
| `.btn-social-discord` | `components/_buttons.scss` | Discord brand colors |

---

## New files and routes

### Routes to add

```
/login          -- LoginComponent (outside shell, no auth guard)
/auth/callback  -- AuthCallbackComponent (outside shell, no auth guard)
/profile        -- ProfileComponent (inside shell, auth guard)
```

The shell layout (header + nav) must be conditional. Routes for `/login` and `/auth/*` should not render inside the shell. One approach: use a layout route. Authenticated routes are children of a route that renders the Shell component; `/login` and `/auth/callback` are sibling routes that render without the shell wrapper.

### Components to create

| Component | Description |
|---|---|
| `LoginComponent` | Full login page with social buttons |
| `AuthCallbackComponent` | Handles OAuth callback, conditionally shows email consent |
| `ProfileComponent` | User profile page with providers list and email toggle |

### Sass files to create

| File | Add to `main.scss` |
|---|---|
| `src/styles/pages/_login.scss` | Under "6. Pages" |

Social button styles can go in the existing `src/styles/components/_buttons.scss`.

---

## Navigation to profile

The profile page needs to be reachable from the UI. **Recommended approach:**

- Add a user avatar or icon button in the header (right side, after the nav links on desktop; in the mobile menu as a link)
- On click: navigates to `/profile`
- For now (no avatar uploaded): show a generic user circle icon using FontAwesome (`faUserCircle`) or initials
- This is a simple addition to the existing shell header

---

## Classification summary

| Decision | Classification |
|---|---|
| Login page renders outside the shell | **Required** |
| Social buttons follow provider brand guidelines | **Required** |
| Consent checkbox is pre-checked | **Required** (per spec) |
| Accessible contrast on Continue button (dark text on amber) | **Required** |
| All text uses translation keys | **Required** |
| Focus rings use `--shadow-focus` | **Required** |
| Touch targets >= 44px | **Required** |
| Screen reader landmarks and labels | **Required** |
| `prefers-reduced-motion` support | **Required** |
| Radial gradient background on login/consent | **Recommended** |
| Inline confirmation for toggling off email consent | **Recommended** |
| Info note when consent is on but no email stored | **Recommended** |
| Dynamic explanation text when checkbox unchecked | **Suggestion** |
| Decorative botanical illustration on desktop login | **Suggestion** |
| Profile link in header as user icon | **Recommended** |
