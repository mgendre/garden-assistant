## [US-312] Ajouter Key et IsCustomized sur l'entite Plant + migration EF

**En tant que** developpeur,
**je veux** ajouter les champs `Key`, `IsCustomized` et `UserId` sur l'entite Plant avec la migration correspondante,
**afin de** permettre le matching par cle stable et la protection des plantes personnalisees lors du seed.

### Criteres d'acceptation

- [ ] CA1 : L'entite `Plant` possede une propriete `Key` (string, not null) representant une cle metier stable (ex. `tomate-cerise`).
- [ ] CA2 : L'entite `Plant` possede une propriete `IsCustomized` (bool, default false).
- [ ] CA3 : L'entite `Plant` possede une propriete `UserId` (Guid?, nullable) avec une foreign key vers la table `users` et un `DeleteBehavior.Cascade`.
- [ ] CA4 : Un index unique filtre est cree sur `Key` avec la condition `WHERE user_id IS NULL` (les plantes du catalogue ont une cle unique).
- [ ] CA5 : Un index est cree sur `UserId` pour optimiser les requetes de variantes utilisateur.
- [ ] CA6 : Une migration EF Core est generee et applicable sans erreur sur une base vierge.
- [ ] CA7 : Aucune migration de donnees n'est necessaire — la base est reconstruite from scratch.

### Notes & contraintes
- La colonne `key` utilise `snake_case` via la convention existante.
- `Key` est obligatoire pour les plantes du catalogue (`UserId == null`). Pour les variantes utilisateur, `Key` peut etre derive du parent.
- Cette US est prerequise pour toutes les autres US de l'epic E21.

### Estimation
- **Priorite :** Indispensable
- **Points :** 3
- **Statut :** A faire
