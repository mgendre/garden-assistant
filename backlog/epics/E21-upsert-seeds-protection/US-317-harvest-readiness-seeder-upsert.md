## [US-317] HarvestReadinessSeeder en mode upsert

**En tant que** développeur,
**je veux** transformer le HarvestReadinessSeeder en mode upsert avec protection des plantes personnalisées,
**afin de** pouvoir corriger et enrichir les indicateurs de maturité sans écraser les modifications manuelles.

### Critères d'acceptation

- [x] CA1 : Le seeder identifie les plantes par `Key` (lookup direct en base).
- [x] CA2 : Si la plante associée est `IsCustomized == true`, l'indicateur de maturité est ignoré.
- [x] CA3 : Si l'indicateur n'existe pas en base, il est inséré.
- [x] CA4 : Si l'indicateur existe et que la plante n'est pas customisée, les champs sont mis à jour.
- [x] CA5 : Les plantes verrouillées sont chargées en un seul `SELECT` avant la boucle.
- [x] CA6 : Logging Info pour les indicateurs mis à jour, Debug (avec guard) pour les indicateurs ignorés.

### Notes & contraintes
- Dépend de US-312 et US-313.
- Même pattern que US-315 et US-316.

### Estimation
- **Priorite :** Indispensable
- **Points :** 3
- **Statut :** Termine
