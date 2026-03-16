## [US-044] Store signal pour "Mes plantes"

**En tant que** developpeur frontend,
**je veux** un store Angular (signal-based) qui charge et gere la liste "Mes plantes",
**afin que** tous les composants de l'application puissent acceder a cette liste de maniere reactive.

### Criteres d'acceptation

- [x] CA1 : Un service `MyPlantsStore` (injectable, `providedIn: 'root'`) expose les signaux `plants` (liste de `PlantDto`), `savedIds` (computed `Set<string>`), et les methodes `isSaved(id)`, `toggle(plant)`.
- [ ] CA2 : ~~La methode `loadMyPlants()` est appelee au demarrage~~ — report a US-043 (API). Actuellement, la persistance utilise `localStorage`.
- [x] CA3 : La methode `toggle(plant)` ajoute ou retire la plante de la liste et met a jour le signal localement.
- [x] CA4 : La methode `isSaved(plantId)` retourne un boolean indiquant si la plante est dans la liste.
- [x] CA5 : Un snackbar de confirmation s'affiche a l'ajout/retrait (cles `Snackbar.PlantAddedToMyPlants`, `Snackbar.PlantRemovedFromMyPlants`).

### Notes & contraintes
- Suit le meme pattern que `CompanionStore` (signals, pas d'Observable, pas de NgRx).
- La persistance est actuellement en `localStorage`. Le passage a l'API sera fait dans US-043.
- Le store est reutilise par le catalogue, les recommandations et la page "Mes plantes".

### Estimation
- **Priorite :** Must
- **Points :** 2
- **Statut :** Done
