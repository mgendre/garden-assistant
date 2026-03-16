## [US-045] Page "Mes plantes" avec liste et gestion

**En tant que** jardinier,
**je veux** voir la liste de toutes mes plantes sur une page dediee accessible depuis la navigation,
**afin de** consulter, ajouter et retirer des plantes de ma collection personnelle.

### Criteres d'acceptation

- [x] CA1 : Un lien "Mes plantes" apparait dans le header de navigation (desktop et mobile), entre "Associations" et "Mon jardin".
- [x] CA2 : La route `/my-plants` affiche un composant `MyPlantsPage` a l'interieur du shell.
- [x] CA3 : La page affiche un titre "Mes plantes" et un sous-titre descriptif.
- [x] CA4 : Les plantes sont listees par ordre alphabetique. Chaque plante est affichee via le composant `PlantCard` reutilisable.
- [x] CA5 : Chaque carte de plante possede un bouton coeur (toggle) permettant de retirer la plante, avec confirmation via dialog.
- [x] CA6 : Un champ de recherche en haut de la liste permet de filtrer localement par nom commun ou latin.
- [x] CA7 : Si la liste est vide, un etat vide s'affiche avec un message invitant le jardinier a ajouter ses premieres plantes.
- [x] CA8 : Un bouton "Ajouter une plante" ouvre un plant-picker (dialog) pour chercher dans le catalogue et ajouter a la liste.

### Notes & contraintes
- Le composant reutilise `PlantCard` (DRY) avec `ViewEncapsulation.None`.
- Les cles de traduction utilisent le prefixe `MyPlants.*`.

### Estimation
- **Priorite :** Must
- **Points :** 5
- **Statut :** Done
