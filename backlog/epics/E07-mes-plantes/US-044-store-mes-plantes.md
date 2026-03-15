## [US-044] Store signal pour "Mes plantes"

**En tant que** developpeur frontend,
**je veux** un store Angular (signal-based) qui charge et gere la liste "Mes plantes",
**afin que** tous les composants de l'application puissent acceder a cette liste de maniere reactive.

### Criteres d'acceptation

- [ ] CA1 : Un service `MyPlantStore` (injectable, `providedIn: 'root'`) expose les signaux `myPlants` (liste de `PlantDto`), `myPlantIds` (computed `Set<string>`), et `loading` (boolean).
- [ ] CA2 : La methode `loadMyPlants()` est appelee au demarrage de l'application (dans le `APP_INITIALIZER` ou equivalent) et remplit le signal `myPlants`.
- [ ] CA3 : La methode `addPlant(plantId: string)` appelle `POST /api/MyPlants/{plantId}`, puis met a jour le signal localement (optimistic update ou re-fetch).
- [ ] CA4 : La methode `removePlant(plantId: string)` appelle `DELETE /api/MyPlants/{plantId}`, puis met a jour le signal.
- [ ] CA5 : La methode `isMyPlant(plantId: string)` retourne un boolean indiquant si la plante est dans la liste.
- [ ] CA6 : Les erreurs API sont gerees avec un message snackbar traduit (cle `Snackbar.MyPlantAddError`, `Snackbar.MyPlantRemoveError`).

### Notes & contraintes
- Suit le meme pattern que `CompanionStore` (signals, pas d'Observable, pas de NgRx).
- Le chargement au demarrage doit etre non-bloquant : si l'appel echoue, l'application demarre quand meme avec une liste vide.
- Un service HTTP `MyPlantService` (avec interface) encapsule les appels API.

### Estimation
- **Priorite :** Must
- **Points :** 2
