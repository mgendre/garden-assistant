## [US-048] Ajouter a "Mes plantes" depuis la page Associations

**En tant que** jardinier,
**je veux** pouvoir ajouter une plante a ma liste "Mes plantes" directement depuis la page Associations,
**afin de** ne pas quitter mon travail de planification pour mettre a jour ma collection.

### Criteres d'acceptation

- [ ] CA1 : Chaque fiche plante dans le panneau central (detail) affiche un bouton "Ajouter a mes plantes" (icone + texte).
- [ ] CA2 : Si la plante est deja dans "Mes plantes", le bouton est remplace par un etat "Dans mes plantes" (desactive, style distinct, icone de validation).
- [ ] CA3 : Cliquer sur le bouton appelle `MyPlantStore.addPlant()` et affiche un snackbar de confirmation ("Plante ajoutee a mes plantes").
- [ ] CA4 : Apres l'ajout, le bouton passe immediatement a l'etat "Dans mes plantes" (optimistic update).
- [ ] CA5 : Si l'ajout echoue (erreur API), le bouton revient a l'etat initial et un snackbar d'erreur s'affiche.
- [ ] CA6 : Le bouton est visible uniquement quand l'utilisateur est authentifie.

### Notes & contraintes
- Le bouton s'integre dans le composant `PlantCard` existant ou dans le `PlantDetailPanel`.
- Les cles de traduction : `MyPlants.AddButton`, `MyPlants.AlreadyAdded`, `Snackbar.PlantAddedToMyPlants`.
- **Decision UX en attente :** le wording du bouton ("Ajouter a mes plantes" vs "Ajouter aux favoris") depend de la decision de nommage globale.

### Estimation
- **Priorite :** Must
- **Points :** 2
