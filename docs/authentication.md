# JWT Authentication

## Overview

Garden Assistant uses a two-token JWT system:

| Token | Lifetime | Storage | Purpose |
|---|---|---|---|
| **Access token** | 15 min | Memory (Angular signal) | Sent with every API request |
| **Refresh token** | 30 days | Memory only | Renews the access token |

Tokens are **never** written to `localStorage` or `sessionStorage` — they are cleared when the browser tab closes.

---

## Bootstrap flow

```
Angular app starts
  └─ APP_INITIALIZER → AuthService.initialize()
       └─ GET /api/auth/token   (no auth required)
            └─ Backend returns { accessToken, refreshToken }
                 └─ Stored in memory within AuthService
```

All subsequent API requests automatically include the header:
```
Authorization: Bearer <accessToken>
```

---

## Renewal flow

```
API request → 401 Unauthorized
  └─ createAuthFetch() intercepts the 401
       └─ POST /api/auth/refresh  { refreshToken }
            ├─ Success → new tokens stored → request retried once
            └─ Failure → tokens cleared, request fails
```

---

## Frontend architecture

### `AuthService` (`src/app/core/auth/auth.service.ts`)

| Member | Type | Role |
|---|---|---|
| `accessToken` | `Signal<string \| null>` | Current token readable by components |
| `refreshToken` | `string \| null` (private) | Renewal token, never exposed |
| `initialize()` | `Promise<void>` | Called at startup via `APP_INITIALIZER` |
| `refresh()` | `Promise<void>` | Renews tokens using the refresh token |
| `createAuthFetch()` | fetch wrapper | Injected into each NSwag client |

### `createAuthFetch()`

NSwag clients (auto-generated) use native `fetch`, not `HttpClient`. The wrapper returned by `createAuthFetch()`:

1. Adds the `Authorization: Bearer` header to every request
2. Skips the header for `/api/auth/` URLs (prevents infinite loops)
3. On 401: attempts a refresh then retries the request once

### Angular interceptor (`src/app/core/auth/auth.interceptor.ts`)

Provides identical coverage for any future use of `HttpClient` in the application.

---

## Backend architecture

### `AuthController`

| Endpoint | Auth | Description |
|---|---|---|
| `GET /api/auth/token` | None | Returns tokens for the first user in the database (dev) |
| `POST /api/auth/refresh` | None | Exchanges a valid refresh token for new tokens |

### `AuthService`

- `GetDevelopmentTokenAsync()` — retrieves the first user and generates both tokens
- `CreateTokensAsync(user)` — generates the access token (HMAC-SHA256) and stores the refresh token in the database
- `RefreshAsync(refreshToken)` — checks expiration, deletes the old refresh token (rotation), generates new tokens

### Refresh token rotation

On each refresh, the old token is **deleted** and a new one is issued. This ensures a stolen token can only be used once.

---

## Configuration (`appsettings.json`)

```json
{
  "Jwt": {
    "Key": "<min 32 characters, via user-secrets in dev>",
    "Issuer": "garden-assistant",
    "Audience": "garden-assistant-app",
    "AccessTokenMinutes": 15,
    "RefreshTokenDays": 30
  }
}
```

The JWT key is validated at startup: an exception is thrown if it is missing or shorter than 32 characters.
