## [US-070] Resume des mecanismes de la guilde

**En tant que** jardinier,
**je veux** voir un resume des mecanismes apportes par l'ensemble de ma guilde,
**afin d'** evaluer d'un coup d'oeil la completude fonctionnelle de mon association de plantes.

### Criteres d'acceptation

- [x] CA1 : Une section "Mecanismes" apparait dans l'editeur de guilde lorsque les plantes selectionnees apportent au moins un mecanisme.
- [x] CA2 : Les mecanismes intrinseques de la guilde sont affiches en bleu (badges `badge-trait`).
- [x] CA3 : Les mecanismes relationnels exclusifs (non intrinseques) sont affiches en vert avec icone lien (badges `badge-positive`).
- [x] CA4 : Un clic sur un mecanisme ouvre la popup d'explication.

### Notes & contraintes
- Calcul purement cote client a partir des signaux `guildIntrinsicMechanisms` et `guildRelationalOnlyMechanisms`.

### Estimation
- **Priorite :** Should
- **Points :** 1
- **Statut :** Done
