## [US-331] Seed data sol et pH pour les plantes du catalogue

**En tant que** jardinier,
**je veux** que les plantes du catalogue aient des informations de sol et de pH pre-renseignees,
**afin de** pouvoir consulter ces donnees sans avoir a les saisir moi-meme.

### Criteres d'acceptation

- [ ] CA1 : Le fichier `plants.json` inclut les champs `soilTypes` (tableau de strings), `optimalPhMin` et `optimalPhMax` pour chaque plante.
- [ ] CA2 : Au minimum 80% des plantes existantes du catalogue ont des valeurs de sol et de pH renseignees.
- [ ] CA3 : Les valeurs sont botaniquement correctes (validees par le plant-expert) : fourchettes pH coherentes et types de sol adaptes a chaque espece.
- [ ] CA4 : Le `PlantSeeder` prend en charge les nouveaux champs lors du upsert : les champs pH suivent le pattern existant, les `soilTypes` sont upserts en diff (ajout/suppression) comme les `IntrinsicMechanisms`.
- [ ] CA5 : Le seed s'execute sans erreur et les donnees sont correctement inserees en base.
- [ ] CA6 : Les plantes avec `IsCustomized = true` ne voient pas leurs nouvelles valeurs ecrasees par le seed.

### Notes & contraintes
- Exemples de valeurs attendues (a valider par le plant-expert) :
  - Tomate : `soilTypes: ["Loam", "Sandy", "Clay"]`, `optimalPhMin: 6.0`, `optimalPhMax: 6.8`
  - Carotte : `soilTypes: ["Sandy", "Loam"]`, `optimalPhMin: 6.0`, `optimalPhMax: 6.8`
  - Myrtille : `soilTypes: ["Peaty"]`, `optimalPhMin: 4.5`, `optimalPhMax: 5.5`
  - Lavande : `soilTypes: ["Chalky", "Rocky"]`, `optimalPhMin: 6.5`, `optimalPhMax: 7.5`
- Le pattern de seed upsert est deja etabli par E21 — reutiliser le meme mecanisme.
- Le upsert des `soilTypes` suit le meme pattern que les `IntrinsicMechanisms` dans le PlantSeeder (diff set).
- Les plantes pour lesquelles les donnees ne sont pas disponibles ont un tableau vide et des pH null.

### Estimation
- **Priorite :** Indispensable
- **Points :** 5
- **Statut :** A faire
