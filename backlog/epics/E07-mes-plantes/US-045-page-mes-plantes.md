## [US-045] Page "Mes plantes" avec liste et gestion

**En tant que** jardinier,
**je veux** voir la liste de toutes mes plantes sur une page dediee accessible depuis la navigation,
**afin de** consulter, ajouter et retirer des plantes de ma collection personnelle.

### Criteres d'acceptation

- [ ] CA1 : Un lien "Mes plantes" apparait dans le header de navigation (desktop et mobile), entre "Associations" et "Mon jardin".
- [ ] CA2 : La route `/my-plants` affiche un composant `MyPlantsPage` a l'interieur du shell.
- [ ] CA3 : La page affiche un titre "Mes plantes" et un sous-titre descriptif.
- [ ] CA4 : Les plantes sont listees par ordre alphabetique. Chaque element affiche l'icone (emoji de famille), le nom commun, le nom latin, et un badge de famille botanique.
- [ ] CA5 : Chaque element a un bouton de suppression (icone corbeille ou x) permettant de retirer la plante de la liste, avec confirmation.
- [ ] CA6 : Un champ de recherche en haut de la liste permet de filtrer localement par nom commun ou latin.
- [ ] CA7 : Si la liste est vide, un etat vide s'affiche avec un message invitant le jardinier a ajouter ses premieres plantes.
- [ ] CA8 : Un bouton "Ajouter une plante" ouvre un dialogue (ou panneau) de recherche dans le catalogue complet pour ajouter une plante a la liste.

### Notes & contraintes
- La cle de traduction du lien de navigation : `Nav.MyPlants`.
- **Decision UX en attente :** le nom de la fonctionnalite ("Mes plantes" vs "Favoris") doit etre valide par un review UX. Utiliser "Mes plantes" comme titre par defaut. Les cles de traduction utilisent le prefixe `MyPlants.*` pour faciliter un renommage eventuel.
- Le composant reutilise les memes styles de carte de plante que le catalogue de la page Associations (DRY).

### Estimation
- **Priorite :** Must
- **Points :** 5
