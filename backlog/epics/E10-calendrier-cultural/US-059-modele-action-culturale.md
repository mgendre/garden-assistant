## [US-059] Modele de donnees des actions culturales par plante

**En tant que** jardinier,
**je veux** que chaque plante dispose d'un calendrier d'actions culturales (semis, repiquage, recolte, taille…) avec leurs fenetres mensuelles,
**afin d'** avoir des informations phenologiques fiables directement dans l'application.

### Criteres d'acceptation

- [ ] CA1 : Une entite `PlantAction` existe dans `Data/Entities/` avec les champs : `Id` (Guid, PK), `PlantId` (Guid, FK), `ActionType` (enum : SemisInterieur, SemisEnPlace, Repiquer, MiseEnPlace, Recolte, Taille), `MonthStart` (int, 1-12), `MonthEnd` (int, 1-12), `ClimateZone` (string nullable : Nord / Centre / Sud), `Notes` (string nullable).
- [ ] CA2 : Une migration EF Core code-first cree la table `plant_actions` avec les conventions snake_case du projet.
- [ ] CA3 : L'endpoint `GET /api/plants/{id}/actions` retourne toutes les actions de la plante sous forme de DTO (sans exposer l'entite EF directement).
- [ ] CA4 : Des endpoints d'administration (`POST`, `PUT`, `DELETE /api/plants/{id}/actions`) permettent de gerer les actions ; ils sont proteges par un role `Admin`.
- [ ] CA5 : Au moins 10 legumes courants (tomate, courgette, carotte, salade, radis, haricot, poireau, epinard, concombre, basilic) ont des donnees de seed avec leurs actions principales.

### Notes & contraintes
- L'entite `PlantAction` va dans `Data/Entities/PlantAction.cs` — une classe par fichier.
- Le DTO `PlantActionDto` est distinct de l'entite.
- Les tests unitaires couvrent le service de lecture des actions (plante inexistante, plante sans action, liste complete).
- `ClimateZone` est nullable : une action sans zone s'applique a toutes les zones.

### Estimation
- **Priorite :** Must
- **Points :** 5
