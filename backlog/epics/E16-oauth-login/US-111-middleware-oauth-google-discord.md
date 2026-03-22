## [US-111] Configuration du middleware OAuth Google et Discord

**En tant que** developpeur backend,
**je veux** configurer les schemes d'authentification OAuth pour Google et Discord dans ASP.NET Core,
**afin que** le backend puisse deleguer l'authentification a ces providers externes.

### Criteres d'acceptation

- [ ] CA1 : Le scheme `ExternalCookie` est configure comme intermediaire transitoire (cookie HTTP-only, SameSite=Lax, SecurePolicy=SameAsRequest).
- [ ] CA2 : Le provider Google est configure via `AddGoogle("Google", ...)` avec `SignInScheme = "ExternalCookie"`. ClientId et ClientSecret sont lus depuis la configuration.
- [ ] CA3 : Le provider Discord est configure via `AddDiscord("Discord", ...)` avec `SignInScheme = "ExternalCookie"`. ClientId et ClientSecret sont lus depuis la configuration.
- [ ] CA4 : Les secrets (ClientId, ClientSecret) sont geres via `dotnet user-secrets` en developpement. `appsettings.json` contient les cles avec des valeurs vides (placeholders).
- [ ] CA5 : La structure de configuration suit le schema `Authentication:Google:ClientId`, `Authentication:Google:ClientSecret`, `Authentication:Discord:ClientId`, `Authentication:Discord:ClientSecret`.
- [ ] CA6 : La configuration `Authentication:FrontendCallbackUrl` est ajoutee dans `appsettings.json` (defaut `http://localhost:4200`).
- [ ] CA7 : Le package NuGet `AspNet.Security.OAuth.Discord` est ajoute au projet. Le package Google est deja inclus dans ASP.NET Core.
- [ ] CA8 : L'application compile et demarre sans erreur, meme sans secrets configures (les schemes sont enregistres mais non utilises tant que les endpoints ne sont pas appeles).

### Notes & contraintes
- Le `ExternalCookie` n'est jamais envoye au frontend Angular. Il sert uniquement a transporter les claims entre le middleware OAuth et le callback handler.
- Ne jamais committer de secrets dans le code source.
- Ajouter des providers futurs (ex: GitHub) = un package NuGet + un `Add*()` + la config.

### Estimation
- **Priorite :** Indispensable
- **Points :** 2
