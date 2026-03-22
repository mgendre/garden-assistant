# OAuth Login — Google & Discord

**Date:** 2026-03-22
**Status:** Draft

## Goal

Add social login (Google, Discord) to the Garden Assistant using the OAuth Authorization Code flow. The system must make it trivial to add more providers later. The existing dev-token flow stays for local development.

## Decisions

- **OAuth flow:** Backend-driven Authorization Code flow (ASP.NET Core OAuth middleware + custom JWT bridge)
- **Provider extensibility:** ASP.NET middleware schemes — adding a provider = NuGet package + config + one `Add*()` call
- **Auto-registration:** First login with a provider creates an account automatically
- **Email storage:** Opt-in via user profile. Default is no email stored (privacy-first)
- **Cross-provider linking:** Only when user has consented to store email — accounts with same email merge
- **Dev token flow:** Kept as-is, restricted to development environment only
- **Deployment:** Single-instance assumed (no horizontal scaling requirement)

## Database Changes

### User Entity (modified)

```
User
  Id: Guid (existing)
  Email: string? (existing → now nullable)
  ConsentEmail: bool (new, default true)
```

- `ConsentEmail` defaults to true — email is stored on first login
- User can opt out via profile toggle, which sets `ConsentEmail = false` and `Email = null`
- `ConsentEmail` controls whether email is captured on next login
- Unique index on `Email` (when not null) to prevent race conditions during cross-provider linking

### ExternalLogin Entity (new)

```
ExternalLogin
  Id: Guid
  UserId: Guid (FK → User, CASCADE delete)
  Provider: string (MaxLength 50, e.g. "Google", "Discord")
  ProviderUserId: string (MaxLength 256)
  CreatedAt: DateTime (UTC)
```

- Unique index on `(Provider, ProviderUserId)`
- Index on `UserId` (FK lookup for profile page, deletion)
- A user can have multiple ExternalLogins (one per provider)

### Migration

- Make `User.Email` nullable
- Add `User.ConsentEmail` column (bool, default true)
- Add unique filtered index on `User.Email` (WHERE Email IS NOT NULL)
- Create `external_logins` table with indexes
- Existing seed user: `ConsentEmail = true` (the default), preserving its existing email
- Update `GenerateAccessToken` to conditionally include email claim only when `User.Email` is not null

## Backend Architecture

### ASP.NET Middleware Setup (Program.cs)

```csharp
services.AddAuthentication()
    .AddCookie("ExternalCookie", o => {
        o.Cookie.SameSite = SameSiteMode.Lax;
        o.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    })
    .AddGoogle("Google", o => {
        o.ClientId = config["Authentication:Google:ClientId"];
        o.ClientSecret = config["Authentication:Google:ClientSecret"];
        o.SignInScheme = "ExternalCookie";
    })
    .AddDiscord("Discord", o => {
        o.ClientId = config["Authentication:Discord:ClientId"];
        o.ClientSecret = config["Authentication:Discord:ClientSecret"];
        o.SignInScheme = "ExternalCookie";
    });
```

The `ExternalCookie` scheme is a transient intermediary — used only during the OAuth callback to pass claims from middleware to our code. No cookies are sent to the Angular client.

### Route Structure

OAuth endpoints live under `/api/auth/oauth/{provider}/...` to avoid collision with existing `/api/auth/token` and `/api/auth/refresh` routes.

The `complete` endpoint is provider-agnostic since the one-time code already maps to a user — the provider's job is done at that point.

### Endpoints (AuthController)

Add `[Authorize]` at class level. Add `[AllowAnonymous]` explicitly on each public endpoint.

| Method | Route | Auth | Purpose |
|---|---|---|---|
| GET | `/api/auth/oauth/{provider}/login` | AllowAnonymous | Challenge → redirect to provider |
| GET | `/api/auth/oauth/{provider}/callback` | AllowAnonymous | Middleware exchanges code, our code finds/creates user, redirects to frontend with one-time code |
| POST | `/api/auth/complete` | AllowAnonymous | Frontend sends one-time code, backend issues JWT + refresh token |
| GET | `/api/auth/token` | AllowAnonymous | Dev-only: returns tokens for seed user (existing, restricted to `IsDevelopment()`) |
| POST | `/api/auth/refresh` | AllowAnonymous | Refresh token rotation (existing) |

### Auth Flow (step by step)

1. **Frontend** opens `GET /api/auth/oauth/google/login`
2. **Backend** issues an OAuth challenge → browser redirects to Google consent screen
3. **Google** redirects to `GET /api/auth/oauth/google/callback?code=...`
4. **ASP.NET middleware** exchanges code for Google tokens, populates `ClaimsPrincipal` with user info (sub, email, name)
5. **Callback handler** extracts provider user ID and email from claims
6. **Lookup** `ExternalLogin` by (Provider="Google", ProviderUserId=sub)
   - **Found** → load existing user
   - **Not found + user has ConsentEmail + email matches existing user** → link to existing user (create ExternalLogin)
   - **Not found + no match** → create new User + ExternalLogin
7. If `user.ConsentEmail == true`, update `user.Email` from provider claims
8. **Generate one-time code** (random, stored in `IMemoryCache`, expires in 5 minutes)
9. **Redirect** to `{FrontendCallbackUrl}/auth/callback?code=xxx&isNew={true|false}`
10. **Frontend** calls `POST /api/auth/complete` with `{ code }`
11. **Backend** validates one-time code, issues JWT access token + refresh token

### One-Time Code Storage

Short-lived codes (5 min TTL) stored in `IMemoryCache`. Each code maps to a `UserId`. Consumed on first use (deleted after exchange).

This assumes single-instance deployment. If horizontal scaling is needed later, migrate to a distributed cache or database-backed approach.

### Provider Validation

The `{provider}` route parameter is validated against a list of configured authentication schemes. Unknown providers return 400.

### Dev Token Endpoint

The existing `GET /api/auth/token` endpoint must be gated behind `IWebHostEnvironment.IsDevelopment()`. Return 404 in non-development environments.

### Cleanup

Move the existing `RefreshRequest` record from `AuthController.cs` to its own file, per one-class-per-file convention.

## Frontend Architecture

### New Route: `/auth/callback`

- Receives `code` and `isNew` query params from backend redirect
- Calls `POST /api/auth/complete` with the one-time code
- On success: stores tokens in auth service, navigates to `/companions`
- On error: navigates to login page with error message

### Login Page (`/login`)

- Designed by UX agent
- Two buttons: "Sign in with Google" / "Sign in with Discord"
- Each button navigates to `GET /api/auth/oauth/{provider}/login`
- Shown when no valid token exists

### Auth Guard

- New `authGuard` applied to all routes except `/login` and `/auth/*`
- If no access token → redirect to `/login`
- In dev mode: bypassed because `initialize()` auto-fetches tokens

### Auth Service Changes

- `initialize()` in dev mode: keeps current auto-token behavior, then `startupService.loadAll()` runs normally
- `initialize()` in prod mode: if no token, does nothing — guard redirects to `/login`
- `startupService.loadAll()` must be deferred in prod mode: only called after successful login (from the auth callback component or after a successful token refresh)
- New method: `completeOAuthLogin(code: string): Promise<void>` — calls `POST /api/auth/complete`, stores tokens, then calls `startupService.loadAll()`
- Token storage stays in-memory (unchanged)

### Profile Page — Email Consent Toggle

- New section in user profile
- Toggle: "Allow email storage"
- Explanation text: when off, no notifications and each provider creates a separate account
- Toggling off: calls API to clear email and set `ConsentEmail = false`
- Toggling on: sets `ConsentEmail = true`, email will be captured on next provider login

## Configuration

### appsettings.json

```json
{
  "Authentication": {
    "FrontendCallbackUrl": "http://localhost:4200",
    "Google": {
      "ClientId": "",
      "ClientSecret": ""
    },
    "Discord": {
      "ClientId": "",
      "ClientSecret": ""
    }
  }
}
```

Secrets managed via `dotnet user-secrets` in development, environment variables in production.

### Adding a New Provider (e.g. GitHub)

1. `dotnet add package AspNet.Security.OAuth.GitHub`
2. Add config section in `appsettings.json`
3. Add `.AddGitHub()` in `Program.cs` with `SignInScheme = "ExternalCookie"`
4. Add provider name to the allowed-providers list
5. Frontend: add a button on the login page

No new controllers, services, or database changes needed.

## Security Considerations

- Client secrets stored in user-secrets / environment variables, never in source
- One-time codes are short-lived (5 min), single-use, stored server-side
- CSRF protection via OAuth `state` parameter (handled by ASP.NET middleware)
- Provider tokens are never sent to or stored on the frontend
- Existing JWT security model (short access token, server-side refresh rotation) unchanged
- `ExternalCookie` is transient, HTTP-only, short-lived, SameSite=Lax, Secure=SameAsRequest
- Dev token endpoint restricted to development environment
- Unique index on `User.Email` prevents duplicate accounts during concurrent cross-provider linking
- `[Authorize]` at controller class level, `[AllowAnonymous]` only on explicitly public endpoints

## Testing

### Service Tests (xUnit + Moq + Shouldly)

**ExternalLoginService (or equivalent):**
- `FindOrCreateUser_WhenNewProvider_ShouldCreateUserAndExternalLogin`
- `FindOrCreateUser_WhenExistingExternalLogin_ShouldReturnExistingUser`
- `FindOrCreateUser_WhenEmailMatchesExistingUser_ShouldLinkToExistingUser`
- `FindOrCreateUser_WhenConsentEmail_ShouldStoreEmail`
- `FindOrCreateUser_WhenNoEmailConsent_ShouldNotStoreEmail`

**One-Time Code Service:**
- `GenerateCode_ShouldReturnUniqueCode`
- `ValidateCode_WhenValid_ShouldReturnUserId`
- `ValidateCode_WhenExpired_ShouldReturnNull`
- `ValidateCode_WhenAlreadyUsed_ShouldReturnNull`
- `ValidateCode_WhenUnknown_ShouldReturnNull`

**AuthService (existing, updated):**
- `GenerateAccessToken_WhenEmailNull_ShouldOmitEmailClaim`
- `GenerateAccessToken_WhenEmailPresent_ShouldIncludeEmailClaim`

### Integration Tests (WebApplicationFactory)

- `OAuthComplete_WhenValidCode_ShouldReturn200WithTokens`
- `OAuthComplete_WhenInvalidCode_ShouldReturn401`
- `OAuthLogin_WhenUnknownProvider_ShouldReturn400`
- `GetToken_WhenNotDevelopment_ShouldReturn404`

## Out of Scope

- Password-based authentication
- Account deletion flow
- Notification system (just the email consent plumbing)
- Admin roles / permissions
- Horizontal scaling / distributed cache
