## [US-332] Exposer SoilType et pH dans PlantDto et PlantSummaryDto

**En tant que** développeur frontend,
**je veux** que l'API retourne les informations de sol et de pH des plantes,
**afin de** pouvoir les afficher dans l'interface.

### Critères d'acceptation

- [x] CA1 : `PlantDto` inclut les champs `SoilTypes` (liste de strings, peut être vide), `OptimalPhMin` (decimal?, nullable) et `OptimalPhMax` (decimal?, nullable).
- [x] CA2 : L'endpoint `GET /api/plants` retourne les nouveaux champs pour chaque plante.
- [x] CA3 : L'endpoint `GET /api/plants/{id}` retourne les nouveaux champs.
- [x] CA4 : Quand aucun type de sol n'est renseigné, `SoilTypes` retourne un tableau vide `[]`. Quand les champs pH sont null, l'API retourne `null`.
- [x] CA5 : Les tests unitaires du service de mapping sont mis à jour pour couvrir les nouveaux champs.
- [x] CA6 : Le build backend compile sans erreur ni warning.

### Notes & contraintes
- Les valeurs de `SoilTypes` sont sérialisées en strings (noms de l'enum) pour rester cohérent avec les autres enums (`WaterNeeds`, `SunRequirement`).
- Le `PlantService` doit inclure les `SoilTypes` via `Include()` ou projection `.Select()`.
- Les variétés héritent des `SoilTypes` et du pH du parent si non définis localement (même pattern que les autres propriétés, à traiter dans E20 quand il sera livré).

### Estimation
- **Priorite :** Indispensable
- **Points :** 2
- **Statut :** Termine
