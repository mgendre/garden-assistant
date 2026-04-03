## [US-312] Ajouter Key et IsCustomized sur l'entité Plant + migration EF

**En tant que** développeur,
**je veux** ajouter les champs `Key`, `IsCustomized` et `UserId` sur l'entité Plant avec la migration correspondante,
**afin de** permettre le matching par clé stable et la protection des plantes personnalisées lors du seed.

### Critères d'acceptation

- [x] CA1 : L'entité `Plant` possède une propriété `Key` (string, not null) représentant une clé métier stable (ex. `tomate-cerise`).
- [x] CA2 : L'entité `Plant` possède une propriété `IsCustomized` (bool, default false).
- [x] CA3 : L'entité `Plant` possède une propriété `UserId` (Guid?, nullable) avec une foreign key vers la table `users` et un `DeleteBehavior.Cascade`.
- [x] CA4 : Un index unique filtré est créé sur `Key` avec la condition `WHERE user_id IS NULL` (les plantes du catalogue ont une clé unique).
- [x] CA5 : Un index est créé sur `UserId` pour optimiser les requêtes de variantes utilisateur.
- [x] CA6 : Une migration EF Core est générée et applicable sans erreur sur une base vierge.
- [x] CA7 : Aucune migration de données n'est nécessaire — la base est reconstruite from scratch.

### Notes & contraintes
- La colonne `key` utilise `snake_case` via la convention existante.
- `Key` est obligatoire pour les plantes du catalogue (`UserId == null`). Pour les variantes utilisateur, `Key` peut être dérivé du parent.
- Cette US est prerequise pour toutes les autres US de l'epic E21.

### Estimation
- **Priorite :** Indispensable
- **Points :** 3
- **Statut :** Termine
