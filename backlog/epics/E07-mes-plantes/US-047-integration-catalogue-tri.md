## [US-047] Prioriser "Mes plantes" dans le catalogue Associations

**En tant que** jardinier,
**je veux** que mes plantes soient facilement identifiables dans le catalogue et les recommandations,
**afin de** trouver rapidement les plantes que je cultive.

### Criteres d'acceptation

- [x] CA1 : Les plantes presentes dans "Mes plantes" ont un indicateur visuel (icone coeur) dans le catalogue de la page Associations.
- [x] CA2 : Les plantes presentes dans "Mes plantes" ont un indicateur visuel (icone coeur) dans le panneau de recommandations (bons compagnons et plantes a eviter).
- [x] CA3 : Dans le panneau de recommandations (compagnons benefiques), les plantes presentes dans "Mes plantes" apparaissent en premier a score egal, puis triees alphabetiquement.
- [x] CA4 : L'indicateur visuel est subtil (petit coeur plein, couleur accent) pour ne pas surcharger l'interface.

### Notes & contraintes
- Le `CompanionStore.goodCompanions` computed integre les donnees du `MyPlantsStore.savedIds` pour le tri.
- Ce story ne modifie pas le backend — le tri est entierement client-side.

### Estimation
- **Priorite :** Must
- **Points :** 3
- **Statut :** Done
