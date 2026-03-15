# CLAUDE.md - Development Guidelines

Garden Assistant · Angular (frontend) · .NET 10 / ASP.NET Core (backend) · PostgreSQL 17

---

## Core Principles

### KISS — Keep It Simple, Stupid
- Use framework built-ins before reaching for custom solutions
- Write self-explanatory code; clear names beat comments
- Break complex components or services into small, focused units
- Start simple; add complexity only when a real need arises

### DRY — Don't Repeat Yourself
- Extract repeated logic into shared services, helpers, or base classes
- One authoritative place for each piece of domain knowledge

### YAGNI — You Aren't Gonna Need It
- Implement only what current requirements demand
- No abstract layers for a single implementation
- No premature optimisation before measuring

### Clean Code
- **Never write comments** — code must be self-explanatory through clear naming
- Name things precisely: variables, methods, and classes should read like prose
- Delete dead code rather than commenting it out
- **Always use `{ }` braces** on all control-flow blocks (`if`, `else`, `for`, `foreach`, `while`, `using`, etc.) — even single-line bodies. This applies to both C# and TypeScript

---

## Conventions

These apply across all layers. Agents reference this section rather than restating rules.

### Secrets management
- All secrets via environment variables or .NET user secrets (`dotnet user-secrets`)
- `.env` files gitignored; `.env.example` committed with placeholder values only
- Never log or return secrets in API responses, error messages, or container images

### API design
- Follow RESTful conventions; return consistent HTTP status codes and problem-detail errors
- Keep controllers thin: validate input → call service → return result. No business logic in controllers
- Map EF entities to DTOs — never expose EF entities directly on API contracts
- Use `async/await` throughout the backend; no `.Result` or `.Wait()`
- Validate at system boundaries (API input, external calls); trust internal code
- **Services must implement an interface** (e.g. `IGardenService` / `GardenService`). Inject via the interface, not the concrete class

### Security by default
- The `security-engineer` agent reviews every feature that touches auth, secrets, or data access
- All endpoints require authentication by default (`[Authorize]`); explicitly opt out with `[AllowAnonymous]` only when intentional
- JWT: short-lived access tokens (≤15 min) + long-lived refresh tokens stored server-side
- Apply `[ApiController]` validation, rate limiting, and CORS policy on all controllers
- OWASP Top 10 is the minimum security baseline

### Backend testing
- **Framework:** xUnit + Moq + Shouldly
- **Naming:** `<Method>_When<Condition>_Should<Outcome>`
  - e.g. `GetPlantAsync_WhenUnauthenticated_ShouldThrowUnauthorized`
- All public service methods must have unit tests
- Integration tests use `WebApplicationFactory` fixtures against a real test database

### Database
- EF Core code-first for all schema changes — never hand-write or hand-edit SQL migrations
- Parameterised queries always (EF handles this; `FromSqlRaw` with `@param` only — never string interpolation)
- `snake_case` table and column names via `UseSnakeCaseNamingConvention()`
- Every table that stores user-scoped data **must** include a `UserId` column (Guid) with a foreign key referencing the `users` table. Add this at entity creation time — do not retrofit later.
- Entities live in `Data/Entities/`

### Frontend conventions
- Use **Angular signals** for all state management — no NgRx or other state libraries
- Prefer `async/await` and Promises over RxJS Observables; use `firstValueFrom()` when bridging Observable APIs (e.g. `MatDialog.afterClosed()`)
- **Mobile-first**: all pages and components must work on mobile viewports (≥ 320px). Use responsive Tailwind breakpoints (`sm:`, `md:`, `lg:`) to progressively enhance for larger screens
- **Separate template files**: always use `templateUrl` pointing to a `.html` file — never inline `template` in components
- **i18n with ngx-translate**: all user-facing text must use `{{ 'Key' | translate }}` or `[translate]="'Key'"`. Translation keys use **PascalCase** (e.g. `Companions.GoodTitle`, `Snackbar.GardenCreated`). Translation files live in `public/i18n/{lang}.json`. Default language is `fr`
- **After every frontend change, run `npm run build --prefix garden-assistant-app` and fix all errors before considering the task done**

### Frontend styling — 7-1 Sass + Tailwind CSS
- **7-1 Sass architecture** (`main.scss`) is the structural foundation:
  - `abstracts/` — variables, mixins (no CSS output)
  - `base/` — reset, typography (h1–h6, body, brand)
  - `components/` — reusable component styles Sass can express
  - `layout/` — header, footer
  - `vendors/` — Angular Material overrides, Tailwind import
- **Typography is defined once in `base/_typography.scss`** — headings (`h1`, `h2`, `h3`) get font-family, color, and responsive sizes from Sass. Never add `font-['DM_Serif_Display']`, heading size, or heading color classes in templates. Only add contextual overrides (e.g. `text-white` on a dark background)
- **Tailwind CSS v4** is used for layout and utility styling in templates. Reusable component classes (`.card-interactive`, `.btn-primary`, `.empty-state`, etc.) are defined via `@layer components` in `tailwind.css`
- **Always check if an existing component class or Tailwind utility covers your need before writing new CSS**
- Variables and design tokens live in `abstracts/_variables.scss` (Sass) and `@theme` in `tailwind.css` (Tailwind)
- Use `@use` / `@forward` in Sass — never the deprecated `@import`

### Container baseline
- **Runtime: Podman** — use `podman compose` (not `docker compose`)
- Multi-stage Containerfiles / Dockerfiles (build stage → runtime stage)
- Pin image versions — never use `latest` in production
- Podman runs rootless by default — preserve this, never run as root
- Secrets via `.env` files only — never hardcoded in Containerfiles or compose files

### Running commands from the repo root

Always run commands from the repo root — never `cd` into subdirectories.

| Task | Command |
|---|---|
| Frontend build | `npm run build --prefix garden-assistant-app` |
| Frontend dev server | `npm run start --prefix garden-assistant-app` |
| Frontend tests | `npm run test --prefix garden-assistant-app` |
| Backend build | `dotnet build garden-assistant-api/garden-assistant-api.csproj` |
| Backend tests | `dotnet test garden-assistant-tests` |
| EF migrations | `dotnet ef migrations add <Name> --project garden-assistant-api` |
| EF update DB | `dotnet ef database update --project garden-assistant-api` |
| Compose (DB) | `podman compose up -d db` |

---

## Agents

| Agent | Responsibility |
|---|---|
| `architect` | High-level design, task breakdown, quality & security oversight |
| `backend-dotnet-developer` | ASP.NET Core, services, repositories, EF Core, migrations |
| `backend-tester` | xUnit / Moq / Shouldly unit and integration tests, maximum coverage |
| `ux-designer` | UX/UI design — user flows, wireframes, visual design, design system, accessibility, micro-interactions |
| `frontend-angular-developer` | Angular components, signals, services — **mobile-first, must run `npm run build` after every change and fix all errors** |
| `reviewer` | Final review against these guidelines |
| `devops-engineer` | Podman, podman-compose, container orchestration |
| `database-engineer` | Schema design, EF Core Fluent API, query performance |
| `security-engineer` | OWASP Top 10, JWT/auth, secrets, dependency audit — reviews every auth/data feature |
| `plant-expert` | Permaculture, companion planting, plant families, growth cycles, soil interactions — consulted for any domain model involving plants, associations, or garden planning |
| `technical-writer` | Creates and maintains all project documentation in `docs/` — architecture decisions, domain model explanations, API guides, onboarding — keeping `docs/README.md` as the up-to-date index |

---

**Good code for this project is simple, tested, and purposeful.**
When in doubt: do less, name things well, delete what you don't need.
