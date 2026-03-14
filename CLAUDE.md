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
- **After every frontend change, run `npm run build --prefix garden-assistant-app` and fix all errors before considering the task done**

### Frontend styling — Tailwind CSS
- **Tailwind CSS v4** is the primary styling framework; use utility classes directly in templates
- **Always check if an existing Tailwind utility or custom style covers your need before writing new CSS**
- Reserve custom Sass only for styles Tailwind cannot express (e.g. complex animations, Material overrides)
- Global custom styles follow the 7-1 Sass architecture via `main.scss`
- Variables and design tokens live in `abstracts/_variables.scss`
- Use `@use` / `@forward` — never the deprecated `@import`

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
| `ux-designer` | User flows, wireframes, design system, accessibility |
| `frontend-angular-developer` | Angular components, signals, services — **must run `npm run build` after every change and fix all errors** |
| `reviewer` | Final review against these guidelines |
| `devops-engineer` | Podman, podman-compose, container orchestration |
| `database-engineer` | Schema design, EF Core Fluent API, query performance |
| `security-engineer` | OWASP Top 10, JWT/auth, secrets, dependency audit — reviews every auth/data feature |
| `plant-expert` | Permaculture, companion planting, plant families, growth cycles, soil interactions — consulted for any domain model involving plants, associations, or garden planning |
| `technical-writer` | Creates and maintains all project documentation in `docs/` — architecture decisions, domain model explanations, API guides, onboarding — keeping `docs/README.md` as the up-to-date index |

---

**Good code for this project is simple, tested, and purposeful.**
When in doubt: do less, name things well, delete what you don't need.
