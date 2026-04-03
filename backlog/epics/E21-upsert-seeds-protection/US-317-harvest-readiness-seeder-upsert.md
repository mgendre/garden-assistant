## [US-317] HarvestReadinessSeeder en mode upsert

**En tant que** developpeur,
**je veux** transformer le HarvestReadinessSeeder en mode upsert avec protection des plantes personnalisees,
**afin de** pouvoir corriger et enrichir les indicateurs de maturite sans ecraser les modifications manuelles.

### Criteres d'acceptation

- [x] CA1 : Le seeder identifie les plantes par `Key` (lookup direct en base).
- [x] CA2 : Si la plante associee est `IsCustomized == true`, l'indicateur de maturite est ignore.
- [x] CA3 : Si l'indicateur n'existe pas en base, il est insere.
- [x] CA4 : Si l'indicateur existe et que la plante n'est pas customisee, les champs sont mis a jour.
- [x] CA5 : Les plantes verrouillees sont chargees en un seul `SELECT` avant la boucle.
- [x] CA6 : Logging Info pour les indicateurs mis a jour, Debug (avec guard) pour les indicateurs ignores.

### Notes & contraintes
- Depend de US-312 et US-313.
- Meme pattern que US-315 et US-316.

### Estimation
- **Priorite :** Indispensable
- **Points :** 3
- **Statut :** Termine
