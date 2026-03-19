## [US-072] Layout deux colonnes (catalogue + editeur)

**En tant que** jardinier,
**je veux** un layout simplifie avec le catalogue a gauche et l'editeur de guilde a droite,
**afin d'** avoir plus d'espace pour le catalogue et une interface moins chargee.

### Criteres d'acceptation

- [x] CA1 : La page Associations utilise un layout deux colonnes : catalogue (420px fixe) et editeur de guilde (flexible).
- [x] CA2 : L'ancien panneau de recommandations (troisieme colonne) est supprime.
- [x] CA3 : Sur mobile, les colonnes s'empilent verticalement.
- [x] CA4 : Les informations de recommandations sont integrees dans l'editeur de guilde (conflits, associations, mecanismes).

### Notes & contraintes
- Remplace le layout 3 colonnes de US-034. La description de US-034 est historiquement correcte mais le layout a evolue.
- Le CSS utilise `grid-template-columns: 420px 1fr` dans `_layout.scss`.

### Estimation
- **Priorite :** Must
- **Points :** 2
- **Statut :** Done
