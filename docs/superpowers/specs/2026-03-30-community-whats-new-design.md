# Community "What's New" — Design Spec

**Date:** 2026-03-30
**Goal:** Broadcast project progress to two audiences (gardeners and developers) via a community-writer agent that generates changelog content, a frontend page that displays it, and a README section that highlights the latest update.

---

## 1. Overview

The project is going public. To communicate progress and share the passion for gardening, we need:

1. A **`community-writer` Claude Code agent** that analyzes git history and generates human-readable changelog content
2. **Static markdown files** in the repo for both user-facing and developer-facing changelogs
3. A **frontend `/whats-new` page** that displays user-facing entries
4. A **README.md "Nouveautes" section** that always shows the latest update with links to both changelogs

No backend changes. No database changes. No new API endpoints.

---

## 2. Community-Writer Agent

### Identity

A new Claude Code agent defined in `.claude/agents/community-writer.md`. Invoked manually when the maintainer decides enough has changed to warrant an update.

### Responsibilities

- Analyze git history (commits, PR merges) since the last changelog entry
- Cross-reference with `backlog/` epics to understand feature context and motivation
- Generate user-facing changelog (French, enthusiastic gardener tone)
- Generate developer-facing changelog (French, technical but accessible)
- Update the index file for the frontend
- Update the README.md "Nouveautes" section

### Workflow

1. **Find the last entry** — scan `changelogs/users/` for the most recent file by date in the filename. If no file exists (first run), use the full git history or a starting point provided by the user.
2. **Read git history** — `git log --oneline` since that date, plus `git log --merges` for PR context.
3. **Cross-reference backlog** — read `backlog/` to map commits to epics and understand the "why" behind changes.
4. **Generate 3 files:**
   - `changelogs/users/YYYY-MM-DD.md`
   - `changelogs/devs/YYYY-MM-DD.md`
   - Update `changelogs/whats-new.index.json` (prepend new entry)
5. **Update README.md** — replace the "Nouveautes" section with a 2-3 sentence summary and links to both changelog files.
6. **Leave files uncommitted** — the maintainer reviews, edits if needed, then commits.

### Output Tone

- **User-facing:** French, warm, enthusiastic about gardening. Focuses on what the user can now do. No technical jargon. Written as if talking to a fellow gardener.
- **Developer-facing:** French, technical, structured by area (backend, frontend, infrastructure). Mentions entities, endpoints, components changed. Highlights areas open for contribution.

---

## 3. File Structure

```
changelogs/
  users/
    2026-03-30.md
    2026-03-15.md
    ...
  devs/
    2026-03-30.md
    2026-03-15.md
    ...
  whats-new.index.json
```

### User Changelog Format

```markdown
---
date: 2026-03-30
title: "Votre jardin devient plus intelligent"
---

## Calendrier cultural

Planifiez vos semis, plantations et recoltes grace au nouveau calendrier
en diagramme de Gantt...

## Assistant de guildes

L'assistant analyse desormais vos guildes et suggere des plantes
compagnes pour combler les manques...
```

### Developer Changelog Format

```markdown
---
date: 2026-03-30
title: "Calendar system, guild assistant, root visualization"
---

## Backend

- Ajout des entites `PlantAction`, `HarvestReadiness` et migrations associees
- Nouveau `CalendarController` avec endpoints pour les actions culturales

## Frontend

- Composant Gantt chart avec filtres par type d'action
- Guild assistant : analyse des associations manquantes

## Infrastructure

- ...
```

### Index File Format

`changelogs/whats-new.index.json`:

```json
[
  { "date": "2026-03-30", "title": "Votre jardin devient plus intelligent", "file": "2026-03-30.md" },
  { "date": "2026-03-15", "title": "Les racines prennent de la profondeur", "file": "2026-03-15.md" }
]
```

Entries ordered newest-first. The frontend reads this to build the listing page.

---

## 4. README.md Integration

The agent adds/replaces a `## Nouveautes` section in the root `README.md`, positioned after the project description and before `## Documentation`:

```markdown
## Nouveautes

Planifiez vos semis et recoltes grace au nouveau calendrier cultural en
diagramme de Gantt. L'assistant de guildes suggere desormais des plantes
compagnes pour combler les manques dans vos associations.

- [Nouveautes pour les jardiniers](changelogs/users/2026-03-30.md)
- [Changelog technique](changelogs/devs/2026-03-30.md)
```

Only the most recent update is shown. The section is fully replaced on each agent run.

---

## 5. Frontend — /whats-new Page

### Route

Added to `app.routes.ts` as a lazy-loaded standalone component:

```typescript
{
  path: 'whats-new',
  loadComponent: () => import('./features/whats-new/whats-new').then(m => m.WhatsNew)
}
```

### Components

**`WhatsNew`** (page component):
- On init, fetches `changelogs/whats-new.index.json` via `HttpClient`
- Displays entries in reverse chronological order (newest first)
- For each entry, fetches the corresponding markdown file and renders it
- Uses Angular signals for state (loading, entries list)

**`WhatsNewEntry`** (child component):
- Receives title, date, and markdown content as inputs
- Renders the date as a subtle subtitle, title as panel header
- Renders markdown content as HTML using a markdown library

### Markdown Rendering

Add `ngx-markdown` as a dependency. It integrates well with Angular and supports rendering markdown strings to HTML in templates.

### Styling

- Each entry uses the `.panel` pattern (`.panel` > `.panel-header` + `.panel-title` + content)
- Date displayed with subtle styling below the title
- Mobile-first layout — single column, readable on all viewports
- No special styling needed beyond existing panel and typography patterns

### Navigation

Add a "Nouveautes" link to the header nav in `shell.html`:
- Desktop nav: new `routerLink="/whats-new"` link
- Mobile nav: same link in the mobile menu
- Translation key: `Nav.WhatsNew`
- Add the translation to `public/i18n/fr.json`: `"Nav.WhatsNew": "Nouveautes"`

### No Authentication Required

The page fetches static assets only — no backend API calls, no auth needed. Accessible to all visitors.

---

## 6. Build Integration

Add asset copy rules in `angular.json` to include changelog files in the build output:

```json
{
  "glob": "**/*.md",
  "input": "../changelogs/users",
  "output": "/changelogs"
},
{
  "glob": "whats-new.index.json",
  "input": "../changelogs",
  "output": "/changelogs"
}
```

This makes the files available at `/changelogs/2026-03-30.md` and `/changelogs/whats-new.index.json` from the Angular app's origin.

---

## 7. Dependencies

| Dependency | Purpose | Scope |
|---|---|---|
| `ngx-markdown` | Render markdown to HTML in Angular templates | Frontend (npm) |

No backend dependencies. No database changes.

---

## 8. Scope Summary

| Item | Type | Count |
|---|---|---|
| New agent definition | `.claude/agents/community-writer.md` | 1 file |
| Changelog directories | `changelogs/users/`, `changelogs/devs/` | 2 directories |
| Index file | `changelogs/whats-new.index.json` | 1 file |
| Frontend page component | `WhatsNew` | 1 component |
| Frontend entry component | `WhatsNewEntry` | 1 component |
| Nav link | "Nouveautes" in shell header | 1 link |
| Translation key | `Nav.WhatsNew` | 1 key |
| Build config | `angular.json` asset rules | 1 config change |
| README section | "Nouveautes" in root README.md | 1 section |
| npm dependency | `ngx-markdown` | 1 package |

**Not in scope:** backend changes, database changes, API endpoints, authentication, developer-facing frontend page (devs read the files in the repo directly).
