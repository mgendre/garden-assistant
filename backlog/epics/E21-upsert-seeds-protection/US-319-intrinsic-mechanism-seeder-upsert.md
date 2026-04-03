## [US-319] PlantIntrinsicMechanismSeeder en mode upsert

**En tant que** développeur,
**je veux** transformer le seeder des mécanismes intrinsèques en mode upsert avec protection des plantes personnalisées,
**afin de** pouvoir corriger et enrichir les mécanismes sans écraser les modifications manuelles.

### Critères d'acceptation

- [x] CA1 : Le seeder identifie les plantes par `Key` (lookup direct en base).
- [x] CA2 : Si la plante associée est `IsCustomized == true`, les mécanismes intrinsèques de cette plante sont ignorés.
- [x] CA3 : Si le mécanisme n'existe pas en base pour cette plante, il est inséré.
- [x] CA4 : Si le mécanisme existe et que la plante n'est pas customisée, les champs sont mis à jour.
- [x] CA5 : Les plantes verrouillées sont chargées en un seul `SELECT` avant la boucle.
- [x] CA6 : Logging Info pour les mécanismes mis à jour, Debug (avec guard) pour les mécanismes ignorés.

### Notes & contraintes
- Dépend de US-312 et US-313.
- Vérifier si les mécanismes intrinsèques sont seeds dans le PlantSeeder ou dans un seeder dédié. Adapter l'implémentation en conséquence.
- Même pattern que US-315/316/317.

### Estimation
- **Priorite :** Indispensable
- **Points :** 2
- **Statut :** Termine
