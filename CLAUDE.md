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

### Backend testing
- **Framework:** xUnit + Moq + Shouldly
- **Naming:** `<Method>_When<Condition>_Should<Outcome>`
  - e.g. `GetPlantAsync_WhenUnauthenticated_ShouldThrowUnauthorized`
- All public service methods must have unit tests

### Database
- EF Core code-first for all schema changes — never hand-write or hand-edit SQL migrations
- Parameterised queries always (EF handles this; `FromSqlRaw` with `@param` only — never string interpolation)
- `snake_case` table and column names via `UseSnakeCaseNamingConvention()`
- Every table that stores user-scoped data **must** include a `UserId` column (Guid) with a foreign key referencing the `users` table. Add this at entity creation time — do not retrofit later.

### Frontend styling — 7-1 Sass pattern

All styles use the 7-1 architecture. One `main.scss` imports everything; never write styles directly in component `.scss` files beyond host-specific overrides.

```
src/styles/
  abstracts/      # variables, functions, mixins, placeholders — no CSS output
  base/           # reset, typography, global element styles
  components/     # one file per reusable UI component (e.g. _button.scss)
  layout/         # header, footer, sidebar, grid
  pages/          # page-specific styles (e.g. _dashboard.scss)
  themes/         # light/dark or seasonal themes
  vendors/        # third-party overrides
  main.scss       # @forward / @use of all 7 folders in order
```

Rules:
- `abstracts/` must never emit CSS on its own (only variables and mixins)
- Use `@use` / `@forward` (not deprecated `@import`)
- Variables and design tokens live in `abstracts/_variables.scss`
- Component styles in `components/` match the Angular component name

### Container baseline
- **Runtime: Podman** — use `podman compose` (not `docker compose`)
- Multi-stage Containerfiles / Dockerfiles (build stage → runtime stage)
- Pin image versions — never use `latest` in production
- Podman runs rootless by default — preserve this, never run as root
- Secrets via `.env` files only — never hardcoded in Containerfiles or compose files

---

## Agents

| Agent | Responsibility |
|---|---|
| `architect` | High-level design, task breakdown, quality & security oversight |
| `backend-dotnet-developer` | ASP.NET Core, services, repositories, EF Core, migrations |
| `backend-tester` | xUnit / Moq / Shouldly unit tests, maximum coverage |
| `ux-designer` | User flows, wireframes, design system, accessibility |
| `frontend-angular-developer` | Angular components, signals, NgRx Signal Store |
| `reviewer` | Final review against these guidelines |
| `devops-engineer` | Docker, docker-compose, container orchestration |
| `database-engineer` | Schema design, EF Core Fluent API, query performance |
| `security-engineer` | OWASP Top 10, JWT/auth, secrets, dependency audit |

---

**Good code for this project is simple, tested, and purposeful.**
When in doubt: do less, name things well, delete what you don't need.
