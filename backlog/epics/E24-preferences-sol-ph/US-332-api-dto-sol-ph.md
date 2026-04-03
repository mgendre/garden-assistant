## [US-332] Exposer SoilType et pH dans PlantDto et PlantSummaryDto

**En tant que** developpeur frontend,
**je veux** que l'API retourne les informations de sol et de pH des plantes,
**afin de** pouvoir les afficher dans l'interface.

### Criteres d'acceptation

- [ ] CA1 : `PlantDto` inclut les champs `SoilTypes` (liste de strings, peut etre vide), `OptimalPhMin` (decimal?, nullable) et `OptimalPhMax` (decimal?, nullable).
- [ ] CA2 : L'endpoint `GET /api/plants` retourne les nouveaux champs pour chaque plante.
- [ ] CA3 : L'endpoint `GET /api/plants/{id}` retourne les nouveaux champs.
- [ ] CA4 : Quand aucun type de sol n'est renseigne, `SoilTypes` retourne un tableau vide `[]`. Quand les champs pH sont null, l'API retourne `null`.
- [ ] CA5 : Les tests unitaires du service de mapping sont mis a jour pour couvrir les nouveaux champs.
- [ ] CA6 : Le build backend compile sans erreur ni warning.

### Notes & contraintes
- Les valeurs de `SoilTypes` sont serialisees en strings (noms de l'enum) pour rester coherent avec les autres enums (`WaterNeeds`, `SunRequirement`).
- Le `PlantService` doit inclure les `SoilTypes` via `Include()` ou projection `.Select()`.
- Les varietes heritent des `SoilTypes` et du pH du parent si non definis localement (meme pattern que les autres proprietes, a traiter dans E20 quand il sera livre).

### Estimation
- **Priorite :** Indispensable
- **Points :** 2
- **Statut :** A faire
