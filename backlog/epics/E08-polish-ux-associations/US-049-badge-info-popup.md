## [US-049] Popup explicatif sur les badges (mecanismes, familles, traits, soleil, eau)

**En tant que** jardinier,
**je veux** pouvoir cliquer sur un badge (mecanisme, famille, trait, soleil, eau) pour voir une explication,
**afin de** comprendre la signification de chaque indicateur sans quitter la page.

### Criteres d'acceptation

- [x] CA1 : Un composant generique `BadgeInfoDialog` (MatDialog) affiche un titre et une description traduits.
- [x] CA2 : Cliquer sur un badge de mecanisme dans les recommandations ouvre le popup avec l'explication du mecanisme.
- [x] CA3 : Cliquer sur un badge de soleil, eau ou trait dans la fiche plante (PlantCard) ouvre le popup avec l'explication correspondante.
- [x] CA4 : Les traductions couvrent les 16 mecanismes, 11 familles, 3 traits, 3 niveaux de soleil et 3 niveaux d'eau (cles `BadgeInfo.*`).
- [x] CA5 : Le popup a une largeur maximale de 400px et un bouton "Compris" pour fermer.
- [x] CA6 : Les badges ont un curseur `pointer` pour indiquer qu'ils sont cliquables.

### Notes & contraintes
- Le composant `BadgeInfoDialog` est dans `shared/ui/badge-info-dialog/`.
- Les styles du dialog sont dans `styles/components/_badges.scss` (7-1 Sass).

### Estimation
- **Priorite :** Should
- **Points :** 3
- **Statut :** Done
