## [US-055] Filtre par profondeur d'enracinement dans le catalogue

**En tant que** jardinier,
**je veux** filtrer le catalogue de plantes par profondeur d'enracinement,
**afin de** trouver rapidement des plantes compatibles avec la stratification racinaire de ma guilde.

### Criteres d'acceptation

- [x] CA1 : Le panneau de filtres du catalogue propose une option "Enracinement" avec trois valeurs : Superficiel, Moyen, Profond.
- [x] CA2 : La selection d'un ou plusieurs niveaux filtre la liste en temps reel (signal Angular, pas de rechargement de page).
- [x] CA3 : Le filtre se cumule avec les autres filtres existants (famille, soleil, eau, etc.).
- [x] CA4 : Un badge de compteur affiche le nombre de resultats apres filtrage.

### Notes & contraintes
- Le filtrage cote client est acceptable si le catalogue reste sous 500 plantes (YAGNI pour la pagination serveur).
- Le champ `rootDepth` est deja disponible dans `PlantDto`.

### Estimation
- **Priorite :** Must
- **Points :** 1
- **Statut :** Done
