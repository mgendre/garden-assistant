## [US-315] AssociationSeeder en mode upsert

**En tant que** développeur,
**je veux** transformer l'AssociationSeeder en mode upsert avec protection des plantes personnalisées,
**afin de** pouvoir corriger et enrichir les associations sans écraser les modifications manuelles.

### Critères d'acceptation

- [x] CA1 : Le seeder identifie les plantes source et cible par `Key` (lookup direct en base).
- [x] CA2 : Si la plante source ou cible est `IsCustomized == true`, l'association est ignorée.
- [x] CA3 : Si l'association n'existe pas en base, elle est insérée.
- [x] CA4 : Si l'association existe et que les deux plantes ne sont pas customisées, les champs sont mis à jour.
- [x] CA5 : Les plantes verrouillées (`IsCustomized == true`) sont chargées en un seul `SELECT` avant la boucle (set de PlantId verrouillés).
- [x] CA6 : Logging Info pour les associations mises à jour, Debug (avec guard) pour les associations ignorées.

### Notes & contraintes
- Dépend de US-312 et US-313.
- Peut être réalisé en parallèle de US-314 (même pattern, entité différente).
- Le matching indirect (key → name → DB) est remplacé par le lookup direct sur `Key`.

### Estimation
- **Priorite :** Indispensable
- **Points :** 3
- **Statut :** Termine
