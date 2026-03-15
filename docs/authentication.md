# Authentification JWT

## Principe

Garden Assistant utilise un système JWT à deux jetons :

| Jeton | Durée | Stockage | Usage |
|---|---|---|---|
| **Access token** | 15 min | Mémoire (signal Angular) | Envoyé à chaque requête API |
| **Refresh token** | 30 jours | Mémoire uniquement | Renouvelle l'access token |

Les jetons ne sont **jamais** écrits en `localStorage` ni en `sessionStorage` — ils disparaissent à la fermeture de l'onglet.

---

## Flux de démarrage

```
App Angular démarre
  └─ APP_INITIALIZER → AuthService.initialize()
       └─ GET /api/auth/token   (pas d'auth requise)
            └─ Backend retourne { accessToken, refreshToken }
                 └─ Stockés en mémoire dans AuthService
```

Toutes les requêtes API suivantes incluent automatiquement l'en-tête :
```
Authorization: Bearer <accessToken>
```

---

## Flux de renouvellement

```
Requête API → 401 Unauthorized
  └─ createAuthFetch() intercepte le 401
       └─ POST /api/auth/refresh  { refreshToken }
            ├─ Succès → nouveaux jetons stockés → requête rejouée une fois
            └─ Échec  → jetons effacés, requête échoue
```

---

## Architecture frontend

### `AuthService` (`src/app/core/auth/auth.service.ts`)

| Membre | Type | Rôle |
|---|---|---|
| `accessToken` | `Signal<string \| null>` | Jeton courant lisible par les composants |
| `refreshToken` | `string \| null` (privé) | Jeton de renouvellement, jamais exposé |
| `initialize()` | `Promise<void>` | Appelé au démarrage via `APP_INITIALIZER` |
| `refresh()` | `Promise<void>` | Renouvelle les jetons via le refresh token |
| `createAuthFetch()` | fetch wrapper | Injecté dans chaque client NSwag |

### `createAuthFetch()`

Les clients NSwag (générés automatiquement) utilisent `fetch` natif, pas `HttpClient`. Le wrapper retourné par `createAuthFetch()` :

1. Ajoute l'en-tête `Authorization: Bearer` à chaque requête
2. Ignore l'en-tête pour les URLs `/api/auth/` (évite les boucles infinies)
3. Sur 401 : tente un refresh puis rejoue la requête une fois

### Intercepteur Angular (`src/app/core/auth/auth.interceptor.ts`)

Couverture identique pour tout usage futur de `HttpClient` dans l'application.

---

## Architecture backend

### `AuthController`

| Endpoint | Auth | Description |
|---|---|---|
| `GET /api/auth/token` | Aucune | Retourne des jetons pour le premier utilisateur en base (dev) |
| `POST /api/auth/refresh` | Aucune | Échange un refresh token valide contre de nouveaux jetons |

### `AuthService`

- `GetDevelopmentTokenAsync()` — récupère le premier utilisateur et génère les deux jetons
- `CreateTokensAsync(user)` — génère l'access token (HMAC-SHA256) et stocke le refresh token en base
- `RefreshAsync(refreshToken)` — vérifie l'expiration, supprime l'ancien refresh token (rotation), génère de nouveaux jetons

### Rotation des refresh tokens

À chaque refresh, l'ancien token est **supprimé** et un nouveau est émis. Cela garantit qu'un token volé ne peut être utilisé qu'une seule fois.

---

## Configuration (`appsettings.json`)

```json
{
  "Jwt": {
    "Key": "<min 32 caractères, via user-secrets en dev>",
    "Issuer": "garden-assistant",
    "Audience": "garden-assistant-app",
    "AccessTokenMinutes": 15,
    "RefreshTokenDays": 30
  }
}
```

La clé JWT est validée au démarrage : une exception est levée si elle est absente ou inférieure à 32 caractères.
