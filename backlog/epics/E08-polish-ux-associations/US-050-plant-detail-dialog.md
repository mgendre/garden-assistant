## [US-050] Fiche detail plante en popup

**En tant que** jardinier,
**je veux** pouvoir ouvrir une fiche detail complete pour n'importe quelle plante depuis les listes (catalogue, recommandations),
**afin de** consulter les informations detaillees sans quitter mon contexte de travail.

### Criteres d'acceptation

- [x] CA1 : Un composant `PlantDetailDialog` (MatDialog) affiche une `PlantCard` complete et depliee.
- [x] CA2 : Un bouton d'information (icone `faCircleInfo`) est present sur chaque element du catalogue de plantes.
- [x] CA3 : Un bouton d'information est present sur chaque compagnon (bon et a eviter) dans le panneau de recommandations.
- [x] CA4 : Cliquer sur le bouton d'information ouvre le dialog avec la fiche complete de la plante.
- [x] CA5 : Le dialog a une largeur maximale de 500px et 90vw sur mobile.
- [x] CA6 : Le bouton d'information ne declenche pas la selection de la plante (stopPropagation).

### Notes & contraintes
- Le composant `PlantDetailDialog` est dans `shared/ui/plant-detail-dialog/`.
- Reutilise `PlantCard` avec `initialExpanded: true` pour afficher la fiche depliee.
- Cle de traduction : `Plant.ViewDetails`.

### Estimation
- **Priorite :** Should
- **Points :** 2
- **Statut :** Done
