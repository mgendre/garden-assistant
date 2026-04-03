## [US-331] Seed data sol et pH pour les plantes du catalogue

**En tant que** jardinier,
**je veux** que les plantes du catalogue aient des informations de sol et de pH pré-renseignées,
**afin de** pouvoir consulter ces données sans avoir à les saisir moi-même.

### Critères d'acceptation

- [x] CA1 : Le fichier `plants.json` inclut les champs `soilTypes` (tableau de strings), `optimalPhMin` et `optimalPhMax` pour chaque plante.
- [x] CA2 : Au minimum 80% des plantes existantes du catalogue ont des valeurs de sol et de pH renseignées.
- [x] CA3 : Les valeurs sont botaniquement correctes (validées par le plant-expert) : fourchettes pH cohérentes et types de sol adaptés à chaque espèce.
- [x] CA4 : Le `PlantSeeder` prend en charge les nouveaux champs lors du upsert : les champs pH suivent le pattern existant, les `soilTypes` sont upserts en diff (ajout/suppression) comme les `IntrinsicMechanisms`.
- [x] CA5 : Le seed s'exécute sans erreur et les données sont correctement insérées en base.
- [x] CA6 : Les plantes avec `IsCustomized = true` ne voient pas leurs nouvelles valeurs écrasées par le seed.

### Notes & contraintes
- Exemples de valeurs attendues (à valider par le plant-expert) :
  - Tomate : `soilTypes: ["Loam", "Sandy", "Clay"]`, `optimalPhMin: 6.0`, `optimalPhMax: 6.8`
  - Carotte : `soilTypes: ["Sandy", "Loam"]`, `optimalPhMin: 6.0`, `optimalPhMax: 6.8`
  - Myrtille : `soilTypes: ["Peaty"]`, `optimalPhMin: 4.5`, `optimalPhMax: 5.5`
  - Lavande : `soilTypes: ["Chalky", "Rocky"]`, `optimalPhMin: 6.5`, `optimalPhMax: 7.5`
- Le pattern de seed upsert est déjà établi par E21 — réutiliser le même mécanisme.
- Le upsert des `soilTypes` suit le même pattern que les `IntrinsicMechanisms` dans le PlantSeeder (diff set).
- Les plantes pour lesquelles les données ne sont pas disponibles ont un tableau vide et des pH null.

### Estimation
- **Priorite :** Indispensable
- **Points :** 5
- **Statut :** Termine
