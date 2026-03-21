## [US-084] Table `languages` et seed FR + EN

**En tant que** developpeur,
**je veux** une table `languages` en base de donnees avec les langues francaise et anglaise pre-chargees,
**afin de** disposer d'un referentiel de langues supportees par l'application.

### Criteres d'acceptation

- [ ] CA1 : Une entite `Language` existe dans `Data/Entities/` avec les champs : `Code` (string, PK, max 10), `Name` (string, max 100), `IsDefault` (bool).
- [ ] CA2 : Une migration EF Core code-first cree la table `languages` avec les conventions snake_case du projet.
- [ ] CA3 : Un seed charge deux lignes : `{ Code: "fr", Name: "Francais", IsDefault: true }` et `{ Code: "en", Name: "English", IsDefault: false }`.
- [ ] CA4 : Une contrainte garantit qu'exactement une langue peut etre `IsDefault = true` (geree par le code applicatif, pas par contrainte DB).
- [ ] CA5 : Les tests unitaires verifient le chargement des langues et la resolution de la langue par defaut.

### Notes & contraintes
- La table `languages` est une donnee de reference, pas une donnee utilisateur — pas de colonne `UserId`.
- Le `Code` suit le standard ISO 639-1 (2 lettres).
- Pas d'endpoint d'administration pour cette iteration.

### Estimation
- **Priorite :** Indispensable
- **Points :** 2
