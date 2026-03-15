## [US-047] Prioriser "Mes plantes" dans le catalogue Associations

**En tant que** jardinier,
**je veux** que mes plantes apparaissent en premier dans le catalogue de la page Associations,
**afin de** trouver rapidement les plantes que je cultive sans chercher dans tout le catalogue.

### Criteres d'acceptation

- [ ] CA1 : En tri alphabetique ("A-Z"), les plantes presentes dans "Mes plantes" apparaissent en premier (triees alphabetiquement entre elles), suivies des autres plantes (triees alphabetiquement).
- [ ] CA2 : En tri compatibilite, l'ordre est : score de compatibilite d'abord, puis "Mes plantes" avant les autres a score egal, puis alphabetique.
- [ ] CA3 : En tri par famille, les plantes de "Mes plantes" apparaissent en premier dans chaque groupe de famille.
- [ ] CA4 : Les plantes de "Mes plantes" ont un indicateur visuel discret dans le catalogue (ex. petit badge etoile ou bordure coloree) permettant de les distinguer des autres.
- [ ] CA5 : Dans le panneau de recommandations (compagnons benefiques), les plantes presentes dans "Mes plantes" apparaissent en premier a score egal.

### Notes & contraintes
- Le `CompanionStore.filteredPlants` computed doit integrer les donnees du `MyPlantStore.myPlantIds`.
- L'indicateur visuel doit etre subtil pour ne pas surcharger l'interface deja riche.
- Ce story ne modifie pas le backend -- le tri est entierement client-side.

### Estimation
- **Priorite :** Must
- **Points :** 3
