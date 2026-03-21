## [US-076] Indicateurs de maturite — modele de donnees et seed

**En tant que** jardinier,
**je veux** que chaque plante dispose d'indicateurs de maturite (description, criteres visuels, tactiles, temporels),
**afin de** savoir reconnaitre quand mes legumes sont prets a etre recoltes.

### Criteres d'acceptation

- [ ] CA1 : Une entite `HarvestReadiness` existe dans `Data/Entities/` avec les champs : `Id` (Guid, PK), `PlantId` (Guid, FK, unique), `Description` (string, max 2000), `DaysFromTransplant` (int nullable), `DaysFromSowing` (int nullable).
- [ ] CA2 : Une entite `HarvestReadinessCriterion` existe dans `Data/Entities/` avec les champs : `Id` (Guid, PK), `HarvestReadinessId` (Guid, FK), `CriterionType` (enum `HarvestCriterionType` : Visual, Touch, Timing, Technique), `Description` (string, max 1000).
- [ ] CA3 : Les criteres sont tries par `CriterionType` (pas de champ SortOrder).
- [ ] CA4 : Une migration EF Core code-first cree les tables `harvest_readiness` et `harvest_readiness_criteria` avec les conventions snake_case.
- [ ] CA5 : L'endpoint `GET /api/plants/{id}/harvest-readiness` retourne les indicateurs sous forme de DTO.
- [ ] CA6 : Toutes les plantes potageres et aromatiques en base disposent de donnees de seed avec leur description de maturite et leurs criteres.
- [ ] CA7 : Les tests unitaires couvrent le service de lecture (plante sans indicateurs, plante avec criteres complets).

### Notes & contraintes
- Creer `IHarvestReadinessService` / `HarvestReadinessService` pour la logique de lecture.
- L'entite `Plant` gagne une propriete de navigation `HarvestReadiness? HarvestReadiness` (relation one-to-one).
- `HarvestReadiness` et `HarvestReadinessCriterion` sont des **donnees de reference** (seed), pas des donnees utilisateur. La regle CLAUDE.md sur `UserId` ne s'applique pas — pas de colonne `UserId`.
- Les donnees de seed proviennent de l'expertise de l'agent `plant-expert`.
- Le DTO `HarvestReadinessDto` inclut la liste des criteres groupes par type.
- Les plantes ornementales ou de couverture (tagete, bourrache, consoude) n'ont pas necessairement d'indicateurs de maturite.
- Les index sur les FK (`PlantId`, `HarvestReadinessId`) sont crees par convention EF Core — verifier dans la migration.

### Estimation
- **Priorite :** Indispensable
- **Points :** 5
