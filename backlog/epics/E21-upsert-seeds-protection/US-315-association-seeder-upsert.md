## [US-315] AssociationSeeder en mode upsert

**En tant que** developpeur,
**je veux** transformer l'AssociationSeeder en mode upsert avec protection des plantes personnalisees,
**afin de** pouvoir corriger et enrichir les associations sans ecraser les modifications manuelles.

### Criteres d'acceptation

- [x] CA1 : Le seeder identifie les plantes source et cible par `Key` (lookup direct en base).
- [x] CA2 : Si la plante source ou cible est `IsCustomized == true`, l'association est ignoree.
- [x] CA3 : Si l'association n'existe pas en base, elle est inseree.
- [x] CA4 : Si l'association existe et que les deux plantes ne sont pas customisees, les champs sont mis a jour.
- [x] CA5 : Les plantes verrouillees (`IsCustomized == true`) sont chargees en un seul `SELECT` avant la boucle (set de PlantId verrouilles).
- [x] CA6 : Logging Info pour les associations mises a jour, Debug (avec guard) pour les associations ignorees.

### Notes & contraintes
- Depend de US-312 et US-313.
- Peut etre realise en parallele de US-314 (meme pattern, entite differente).
- Le matching indirect (key -> name -> DB) est remplace par le lookup direct sur `Key`.

### Estimation
- **Priorite :** Indispensable
- **Points :** 3
- **Statut :** Termine
