## [US-059] Modele de donnees des actions culturales par plante

**En tant que** jardinier,
**je veux** que chaque plante dispose d'un calendrier d'actions culturales (semis, repiquage, recolte, taille, pincage, buttage, division) avec leurs fenetres en demi-mois,
**afin d'** avoir des informations phenologiques fiables directement dans l'application.

### Criteres d'acceptation

- [ ] CA1 : Une entite `PlantAction` existe dans `Data/Entities/` avec les champs : `Id` (Guid, PK), `PlantId` (Guid, FK), `ActionType` (enum `PlantActionType` : IndoorSowing, DirectSowing, Transplanting, Harvest, Pruning, Pinching, Hilling, Division), `HalfMonthStart` (int, 1-24), `HalfMonthEnd` (int, 1-24), `Notes` (string nullable, max 1000). Encodage demi-mois : 1 = debut janvier, 2 = mi-janvier, 3 = debut fevrier, ..., 24 = mi-decembre.
- [ ] CA2 : Plusieurs lignes sont possibles par plante/action pour supporter les doubles fenetres (ex. epinard printemps + automne). `HalfMonthEnd < HalfMonthStart` signifie que la fenetre chevauche l'annee suivante.
- [ ] CA3 : Une migration EF Core code-first cree la table `plant_actions` avec les conventions snake_case du projet.
- [ ] CA4 : Un enum `PropagationMethod` (`Seed`, `Bulb`, `Tuber`, `Division`) est ajoute sur l'entite `Plant`. Valeur par defaut : `Seed`.
- [ ] CA5 : Un champ `FrostSensitive` (bool, defaut `false`) est ajoute sur l'entite `Plant`.
- [ ] CA6 : Le `PlantDto` existant est mis a jour pour inclure les champs `PropagationMethod` et `FrostSensitive`.
- [ ] CA7 : L'entite `Plant` gagne une propriete de navigation `List<PlantAction> Actions`.
- [ ] CA8 : L'endpoint `GET /api/plants/{id}/actions` est ajoute au `PlantsController` et retourne toutes les actions de la plante sous forme de `PlantActionDto` (sans exposer l'entite EF directement).
- [ ] CA9 : Toutes les plantes en base disposent de donnees de seed avec leurs actions principales, calibrees sur le climat suisse (plateau, ~400-600m).
- [ ] CA10 : Les tests unitaires couvrent le service de lecture des actions (plante inexistante, plante sans action, liste complete, double fenetre).

### Notes & contraintes
- L'entite `PlantAction` va dans `Data/Entities/PlantAction.cs` — une classe par fichier.
- Le DTO `PlantActionDto` est distinct de l'entite.
- Creer `IPlantActionService` / `PlantActionService` pour la logique de lecture des actions.
- `PlantAction` et `HarvestReadiness` sont des **donnees de reference** (seed), pas des donnees utilisateur. La regle CLAUDE.md sur `UserId` ne s'applique pas — pas de colonne `UserId`.
- Pas de `ClimateZone` pour cette iteration — les zones climatiques seront ajoutees dans E12.
- Pas d'endpoints d'administration (CRUD) pour cette iteration.
- Les donnees de seed proviennent de l'expertise de l'agent `plant-expert`.
- Les index sur les FK (`PlantId`) sont crees par convention EF Core — verifier dans la migration.

### Estimation
- **Priorite :** Indispensable
- **Points :** 5
