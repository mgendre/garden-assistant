## [US-112] Flux d'authentification OAuth backend (login, callback, complete)

**En tant que** jardinier,
**je veux** pouvoir m'authentifier via Google ou Discord,
**afin d'** acceder a mon compte sans creer de mot de passe.

### Criteres d'acceptation

- [ ] CA1 : `GET /api/auth/oauth/{provider}/login` (AllowAnonymous) declenche un challenge OAuth vers le provider. Si le provider est inconnu (non configure), retourne 400.
- [ ] CA2 : `GET /api/auth/oauth/{provider}/callback` (AllowAnonymous) est le callback OAuth. Le middleware ASP.NET echange le code contre les tokens du provider et peuple le `ClaimsPrincipal`. Le handler extrait le provider user ID et l'email depuis les claims.
- [ ] CA3 : Le callback recherche un `ExternalLogin` existant par `(Provider, ProviderUserId)`. Si trouve, marque `isNew = false`. Sinon, marque `isNew = true`.
- [ ] CA4 : Le callback genere un code a usage unique (aleatoire, stocke dans `IMemoryCache`, TTL 5 minutes). Le code est associe au provider, provider user ID, email du provider, et flag `isNew`.
- [ ] CA5 : Le callback redirige vers `{FrontendCallbackUrl}/auth/callback?code=xxx&isNew={true|false}`.
- [ ] CA6 : `POST /api/auth/complete` (AllowAnonymous) recoit `{ code, storeEmail }`. Valide le code (usage unique, non expire). Si invalide ou expire, retourne 401.
- [ ] CA7 : Pour un utilisateur existant (`isNew = false`), `complete` emet un JWT access token + refresh token (meme logique que le flux actuel).
- [ ] CA8 : Pour un nouvel utilisateur (`isNew = true`), `complete` cree le User avec `ConsentEmail = storeEmail`. Si `storeEmail == true`, stocke l'email du provider ; sinon `Email = null`. Cree l'ExternalLogin. Emet JWT + refresh token.
- [ ] CA9 : Pour un nouvel utilisateur avec `storeEmail == true`, si l'email correspond a un User existant, lie l'ExternalLogin a ce User au lieu d'en creer un nouveau (cross-provider linking).
- [ ] CA10 : La generation du JWT inclut le claim email uniquement si `User.Email` n'est pas null. Si `User.Email` est null, le claim email est omis.
- [ ] CA11 : Un service `IOAuthService` (ou equivalent) encapsule la logique metier (lookup, creation, linking). Le controller reste mince.
- [ ] CA12 : Le record `RefreshRequest` est deplace de `AuthController.cs` vers son propre fichier, conformement a la convention un-fichier-par-classe.
- [ ] CA13 : La validation du parametre `{provider}` se fait contre la liste des schemes d'authentification configures.

### Notes & contraintes
- Les tokens du provider (access token Google/Discord) ne sont jamais envoyes au frontend ni stockes.
- Le code a usage unique est stocke en `IMemoryCache` (single-instance). Migration vers cache distribue si scaling necessaire.
- Les endpoints OAuth sont sous `/api/auth/oauth/{provider}/...` pour eviter les collisions avec `/api/auth/token` et `/api/auth/refresh`.

### Estimation
- **Priorite :** Indispensable
- **Points :** 8
