## [US-085] Table generique `translations`

**En tant que** developpeur,
**je veux** une table generique `translations` capable de stocker les traductions de n'importe quel champ de n'importe quelle entite,
**afin d'** avoir une architecture de traduction unique et extensible.

### Criteres d'acceptation

- [ ] CA1 : Une entite `Translation` existe dans `Data/Entities/` avec les champs : `Id` (Guid, PK), `EntityType` (string, max 100), `EntityId` (Guid), `Field` (string, max 100), `LanguageCode` (string, FK -> `languages.code`), `Value` (text).
- [ ] CA2 : Une contrainte d'unicite existe sur `(EntityType, EntityId, Field, LanguageCode)`.
- [ ] CA3 : Un index est cree sur `(EntityType, EntityId, LanguageCode)` pour optimiser les requetes de resolution.
- [ ] CA4 : Une migration EF Core code-first cree la table `translations` avec les conventions snake_case du projet.
- [ ] CA5 : La relation `Translation.LanguageCode -> Language.Code` est configuree en Fluent API avec `OnDelete(DeleteBehavior.Cascade)`.
- [ ] CA6 : Les tests unitaires verifient la creation et l'unicite des traductions.

### Notes & contraintes
- La table `translations` est une donnee de reference/systeme — pas de colonne `UserId`.
- `EntityType` contient le nom de l'entite (ex. "Plant", "Guild", "HarvestReadiness").
- `EntityId` est le `Guid` de l'entite traduite.
- `Field` est le nom du champ traduit (ex. "Name", "Description").
- Pas de contrainte FK vers les entites traduites (la table est generique).

### Estimation
- **Priorite :** Indispensable
- **Points :** 3
