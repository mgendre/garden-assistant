## [US-330] Enum SoilType, table de jointure PlantSoilType et champs pH sur l'entité Plant + migration EF

**En tant que** développeur,
**je veux** ajouter un enum `SoilType`, une table de jointure `PlantSoilType` (many-to-many) et deux champs décimaux `OptimalPhMin` / `OptimalPhMax` sur l'entité Plant avec la migration correspondante,
**afin de** stocker les préférences de sol (multiples) et de pH de chaque plante dans le catalogue.

### Critères d'acceptation

- [x] CA1 : Un enum `SoilType` est créé dans `Data/Entities/Enums/` avec les valeurs : `Sandy`, `Silty`, `Clay`, `Loam`, `Chalky`, `Peaty`, `Rocky`.
- [x] CA2 : Une entité `PlantSoilType` est créée dans `Data/Entities/` avec les propriétés `PlantId` (Guid) et `SoilType` (enum SoilType), clé composite (PlantId, SoilType).
- [x] CA3 : L'entité `Plant` possède une propriété de navigation `SoilTypes` (`List<PlantSoilType>`).
- [x] CA4 : La relation est configurée en Fluent API : clé composite, FK vers Plant avec cascade delete.
- [x] CA5 : Un `DbSet<PlantSoilType>` est ajouté au `AppDbContext`.
- [x] CA6 : L'entité `Plant` possède une propriété `OptimalPhMin` (decimal?, nullable) représentant le pH minimal optimal.
- [x] CA7 : L'entité `Plant` possède une propriété `OptimalPhMax` (decimal?, nullable) représentant le pH maximal optimal.
- [x] CA8 : La configuration Fluent API ajoute une contrainte CHECK : `optimal_ph_min >= 3.0 AND optimal_ph_max <= 9.0 AND optimal_ph_min <= optimal_ph_max` (quand les deux sont non null).
- [x] CA9 : Les colonnes `optimal_ph_min` et `optimal_ph_max` utilisent `decimal(3,1)` (précision suffisante pour les valeurs pH).
- [x] CA10 : Une migration EF Core est générée et applicable sans erreur sur une base vierge.
- [x] CA11 : Les tests unitaires existants continuent de passer (les nouveaux champs sont nullable donc non-breaking).

### Notes & contraintes
- La table de jointure `plant_soil_types` suit la convention `snake_case` existante.
- La relation many-to-many permet de stocker plusieurs types de sol par plante (ex: tomate → Loam, Sandy, Clay).
- Même pattern que `PlantIntrinsicMechanism` (clé composite PlantId + enum).
- Les champs pH sont nullable car le seed sera incrémental — toutes les plantes ne seront pas renseignées immédiatement.
- Cette US est prerequise pour toutes les autres US de l'epic E24.

### Estimation
- **Priorite :** Indispensable
- **Points :** 3
- **Statut :** Termine
