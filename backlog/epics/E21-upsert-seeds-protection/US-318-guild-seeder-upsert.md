## [US-318] GuildSeeder en mode upsert

**En tant que** developpeur,
**je veux** transformer le GuildSeeder en mode upsert avec protection des liens plante-guilde pour les plantes personnalisees,
**afin de** pouvoir corriger et enrichir les guildes sans ecraser les liens vers des plantes modifiees manuellement.

### Criteres d'acceptation

- [x] CA1 : Les guildes (nom, description) sont toujours upsertees, meme si des plantes membres sont customisees.
- [x] CA2 : Les liens `GuildPlant` referençant une plante `IsCustomized == true` sont ignores (non modifies, non supprimes).
- [x] CA3 : Les liens `GuildPlant` referençant une plante non customisee sont upserts normalement.
- [x] CA4 : Les plantes verrouillees sont chargees en un seul `SELECT` avant la boucle.
- [x] CA5 : Le seeder identifie les plantes par `Key` (lookup direct en base).
- [x] CA6 : Logging Info pour les guildes et liens mis a jour, Debug (avec guard) pour les liens ignores.

### Notes & contraintes
- Depend de US-312 et US-313.
- Les guildes sont structurelles : elles sont toujours upsertees. Seuls les liens plante-guilde respectent le verrou `IsCustomized`.

### Estimation
- **Priorite :** Indispensable
- **Points :** 3
- **Statut :** Termine
