## [US-110] Migration base de donnees : User modifie et ExternalLogin

**En tant que** developpeur backend,
**je veux** mettre a jour l'entite User et creer l'entite ExternalLogin,
**afin de** supporter l'authentification par providers OAuth externes.

### Criteres d'acceptation

- [ ] CA1 : La colonne `Email` de la table `users` est rendue nullable. Les donnees existantes sont preservees.
- [ ] CA2 : Une nouvelle colonne `ConsentEmail` (bool, defaut `true`) est ajoutee a la table `users`.
- [ ] CA3 : Un index unique filtre est cree sur `users.email` (WHERE email IS NOT NULL) pour empecher les doublons d'email.
- [ ] CA4 : Une nouvelle table `external_logins` est creee avec les colonnes : `Id` (Guid), `UserId` (Guid, FK → users, CASCADE delete), `Provider` (string, MaxLength 50), `ProviderUserId` (string, MaxLength 256), `CreatedAt` (DateTime UTC).
- [ ] CA5 : Un index unique est cree sur `(Provider, ProviderUserId)` dans la table `external_logins`.
- [ ] CA6 : Un index est cree sur `UserId` dans la table `external_logins` pour les lookups FK.
- [ ] CA7 : L'entite `ExternalLogin` est ajoutee dans `Data/Entities/` avec la configuration Fluent API dans un fichier de configuration dedie. Le nommage suit la convention `snake_case`.
- [ ] CA8 : L'utilisateur seed existant conserve `ConsentEmail = true` et son email existant.
- [ ] CA9 : La migration EF Core s'applique proprement (`dotnet ef database update`).

### Notes & contraintes
- Migration EF Core code-first uniquement (jamais de SQL manuel).
- Convention `snake_case` via `UseSnakeCaseNamingConvention()`.
- Ne pas modifier la generation de JWT dans cette story (US-112 s'en charge).

### Estimation
- **Priorite :** Indispensable
- **Points :** 3
