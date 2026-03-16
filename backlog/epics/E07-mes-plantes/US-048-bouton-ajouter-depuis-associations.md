## [US-048] Ajouter a "Mes plantes" depuis la page Associations

**En tant que** jardinier,
**je veux** pouvoir ajouter une plante a ma liste "Mes plantes" directement depuis la page Associations,
**afin de** ne pas quitter mon travail de planification pour mettre a jour ma collection.

### Criteres d'acceptation

- [x] CA1 : Chaque fiche plante dans le panneau central (PlantCard) affiche un bouton coeur (toggle) en pleine largeur dans le corps de la carte.
- [x] CA2 : Si la plante est deja dans "Mes plantes", le coeur est plein (solid). Sinon, il est vide (regular).
- [x] CA3 : Cliquer sur le bouton appelle `MyPlantsStore.toggle()` et affiche un snackbar de confirmation.
- [x] CA4 : Si la plante est deja sauvee, un dialog de confirmation s'affiche avant le retrait.
- [x] CA5 : L'etat du bouton se met a jour immediatement (optimistic update via signal).

### Notes & contraintes
- Le bouton est integre dans le composant `PlantCard` existant (dans le corps du collapsible, pas dans le header).
- Les cles de traduction : `MyPlants.AddButton`, `MyPlants.RemoveButton`, `Snackbar.PlantAddedToMyPlants`.

### Estimation
- **Priorite :** Must
- **Points :** 2
- **Statut :** Done
