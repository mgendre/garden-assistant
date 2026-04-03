## [US-314] PlantSeeder en mode upsert par Key

**En tant que** developpeur,
**je veux** transformer le PlantSeeder pour qu'il fonctionne en mode upsert base sur le champ `Key`,
**afin de** pouvoir corriger et enrichir les donnees de plantes sans necessiter un reset complet de la base.

### Criteres d'acceptation

- [x] CA1 : Le seeder cherche chaque plante en base par `Key` au lieu de verifier si la table est vide.
- [x] CA2 : Si la plante n'existe pas en base, elle est inseree.
- [x] CA3 : Si la plante existe et `IsCustomized == false`, les champs modifies sont mis a jour.
- [x] CA4 : Si la plante existe et `IsCustomized == true`, elle est ignoree (aucune modification).
- [x] CA5 : Un log `Info` est emis pour chaque plante mise a jour, listant les champs modifies (ex. `Plant "Tomate" (key: tomate) updated — HeightAtMaturityCm: 150 -> 180`).
- [x] CA6 : Un log `Debug` est emis pour chaque plante ignoree, avec un guard `if (logger.IsEnabled(LogLevel.Debug))` pour eviter le cout du formatage.
- [x] CA7 : Le seeder fonctionne correctement sur une base vierge (toutes les plantes inserees).
- [x] CA8 : Le seeder fonctionne correctement sur une base existante avec des plantes deja presentes.

### Notes & contraintes
- Depend de US-312 (champ `Key` sur l'entite) et US-313 (champ `key` dans les JSON).
- Le matching indirect (key -> name -> DB lookup) est remplace par un lookup direct sur `Key`.
- Les performances doivent rester acceptables : charger toutes les plantes existantes en un seul `SELECT` avant la boucle d'upsert (eviter N+1).

### Estimation
- **Priorite :** Indispensable
- **Points :** 5
- **Statut :** Termine
