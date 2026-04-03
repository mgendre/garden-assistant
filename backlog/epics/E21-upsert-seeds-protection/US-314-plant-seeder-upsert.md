## [US-314] PlantSeeder en mode upsert par Key

**En tant que** développeur,
**je veux** transformer le PlantSeeder pour qu'il fonctionne en mode upsert basé sur le champ `Key`,
**afin de** pouvoir corriger et enrichir les données de plantes sans nécessiter un reset complet de la base.

### Critères d'acceptation

- [x] CA1 : Le seeder cherche chaque plante en base par `Key` au lieu de vérifier si la table est vide.
- [x] CA2 : Si la plante n'existe pas en base, elle est insérée.
- [x] CA3 : Si la plante existe et `IsCustomized == false`, les champs modifiés sont mis à jour.
- [x] CA4 : Si la plante existe et `IsCustomized == true`, elle est ignorée (aucune modification).
- [x] CA5 : Un log `Info` est émis pour chaque plante mise à jour, listant les champs modifiés (ex. `Plant "Tomate" (key: tomate) updated — HeightAtMaturityCm: 150 -> 180`).
- [x] CA6 : Un log `Debug` est émis pour chaque plante ignorée, avec un guard `if (logger.IsEnabled(LogLevel.Debug))` pour éviter le coût du formatage.
- [x] CA7 : Le seeder fonctionne correctement sur une base vierge (toutes les plantes insérées).
- [x] CA8 : Le seeder fonctionne correctement sur une base existante avec des plantes déjà présentes.

### Notes & contraintes
- Dépend de US-312 (champ `Key` sur l'entité) et US-313 (champ `key` dans les JSON).
- Le matching indirect (key → name → DB lookup) est remplacé par un lookup direct sur `Key`.
- Les performances doivent rester acceptables : charger toutes les plantes existantes en un seul `SELECT` avant la boucle d'upsert (éviter N+1).

### Estimation
- **Priorite :** Indispensable
- **Points :** 5
- **Statut :** Termine
