## [US-319] PlantIntrinsicMechanismSeeder en mode upsert

**En tant que** developpeur,
**je veux** transformer le seeder des mecanismes intrinseques en mode upsert avec protection des plantes personnalisees,
**afin de** pouvoir corriger et enrichir les mecanismes sans ecraser les modifications manuelles.

### Criteres d'acceptation

- [x] CA1 : Le seeder identifie les plantes par `Key` (lookup direct en base).
- [x] CA2 : Si la plante associee est `IsCustomized == true`, les mecanismes intrinseques de cette plante sont ignores.
- [x] CA3 : Si le mecanisme n'existe pas en base pour cette plante, il est insere.
- [x] CA4 : Si le mecanisme existe et que la plante n'est pas customisee, les champs sont mis a jour.
- [x] CA5 : Les plantes verrouillees sont chargees en un seul `SELECT` avant la boucle.
- [x] CA6 : Logging Info pour les mecanismes mis a jour, Debug (avec guard) pour les mecanismes ignores.

### Notes & contraintes
- Depend de US-312 et US-313.
- Verifier si les mecanismes intrinseques sont seeds dans le PlantSeeder ou dans un seeder dedie. Adapter l'implementation en consequence.
- Meme pattern que US-315/316/317.

### Estimation
- **Priorite :** Indispensable
- **Points :** 2
- **Statut :** Termine
