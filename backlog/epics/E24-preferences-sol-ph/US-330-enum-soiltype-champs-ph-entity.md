## [US-330] Enum SoilType, table de jointure PlantSoilType et champs pH sur l'entite Plant + migration EF

**En tant que** developpeur,
**je veux** ajouter un enum `SoilType`, une table de jointure `PlantSoilType` (many-to-many) et deux champs decimaux `OptimalPhMin` / `OptimalPhMax` sur l'entite Plant avec la migration correspondante,
**afin de** stocker les preferences de sol (multiples) et de pH de chaque plante dans le catalogue.

### Criteres d'acceptation

- [ ] CA1 : Un enum `SoilType` est cree dans `Data/Entities/Enums/` avec les valeurs : `Sandy`, `Silty`, `Clay`, `Loam`, `Chalky`, `Peaty`, `Rocky`.
- [ ] CA2 : Une entite `PlantSoilType` est creee dans `Data/Entities/` avec les proprietes `PlantId` (Guid) et `SoilType` (enum SoilType), cle composite (PlantId, SoilType).
- [ ] CA3 : L'entite `Plant` possede une propriete de navigation `SoilTypes` (`List<PlantSoilType>`).
- [ ] CA4 : La relation est configuree en Fluent API : cle composite, FK vers Plant avec cascade delete.
- [ ] CA5 : Un `DbSet<PlantSoilType>` est ajoute au `AppDbContext`.
- [ ] CA6 : L'entite `Plant` possede une propriete `OptimalPhMin` (decimal?, nullable) representant le pH minimal optimal.
- [ ] CA7 : L'entite `Plant` possede une propriete `OptimalPhMax` (decimal?, nullable) representant le pH maximal optimal.
- [ ] CA8 : La configuration Fluent API ajoute une contrainte CHECK : `optimal_ph_min >= 3.0 AND optimal_ph_max <= 9.0 AND optimal_ph_min <= optimal_ph_max` (quand les deux sont non null).
- [ ] CA9 : Les colonnes `optimal_ph_min` et `optimal_ph_max` utilisent `decimal(3,1)` (precision suffisante pour les valeurs pH).
- [ ] CA10 : Une migration EF Core est generee et applicable sans erreur sur une base vierge.
- [ ] CA11 : Les tests unitaires existants continuent de passer (les nouveaux champs sont nullable donc non-breaking).

### Notes & contraintes
- La table de jointure `plant_soil_types` suit la convention `snake_case` existante.
- La relation many-to-many permet de stocker plusieurs types de sol par plante (ex: tomate → Loam, Sandy, Clay).
- Meme pattern que `PlantIntrinsicMechanism` (cle composite PlantId + enum).
- Les champs pH sont nullable car le seed sera incremental — toutes les plantes ne seront pas renseignees immediatement.
- Cette US est prerequise pour toutes les autres US de l'epic E24.

### Estimation
- **Priorite :** Indispensable
- **Points :** 3
- **Statut :** A faire
