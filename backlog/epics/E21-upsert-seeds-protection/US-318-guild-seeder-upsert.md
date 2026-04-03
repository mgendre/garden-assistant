## [US-318] GuildSeeder en mode upsert

**En tant que** développeur,
**je veux** transformer le GuildSeeder en mode upsert avec protection des liens plante-guilde pour les plantes personnalisées,
**afin de** pouvoir corriger et enrichir les guildes sans écraser les liens vers des plantes modifiées manuellement.

### Critères d'acceptation

- [x] CA1 : Les guildes (nom, description) sont toujours upsertées, même si des plantes membres sont customisées.
- [x] CA2 : Les liens `GuildPlant` référençant une plante `IsCustomized == true` sont ignorés (non modifiés, non supprimés).
- [x] CA3 : Les liens `GuildPlant` référençant une plante non customisée sont upserts normalement.
- [x] CA4 : Les plantes verrouillées sont chargées en un seul `SELECT` avant la boucle.
- [x] CA5 : Le seeder identifie les plantes par `Key` (lookup direct en base).
- [x] CA6 : Logging Info pour les guildes et liens mis à jour, Debug (avec guard) pour les liens ignorés.

### Notes & contraintes
- Dépend de US-312 et US-313.
- Les guildes sont structurelles : elles sont toujours upsertées. Seuls les liens plante-guilde respectent le verrou `IsCustomized`.

### Estimation
- **Priorite :** Indispensable
- **Points :** 3
- **Statut :** Termine
