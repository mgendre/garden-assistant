## [US-013] Retirer une plante d'une planche

**En tant que** jardinier,
**je veux** pouvoir retirer une plante d'une planche (fin de culture, erreur de saisie),
**afin de** maintenir une vue exacte de ce qui pousse sur chaque planche.

### Critères d'acceptation

- [x] CA1 : Je peux retirer une plante depuis la liste des cultures de la planche.
- [x] CA2 : Une confirmation est demandée avant la suppression (via la suppression de la plante dans l'éditeur de guilde).
- [ ] CA3 : Si la plante fait partie de l'historique de rotation, une option me permet de la marquer comme "récoltée" plutôt que de la supprimer. → Reporté (dépend de E04 — rotations)

### Notes & contraintes
- Implémenté via l'éditeur de guilde : retirer une plante de la planche = la retirer de la guilde liée.
- CA3 (marquer comme récoltée / historique rotation) reporté à E04.

### Estimation
- **Priorité :** Important
- **Points :** 3
- **Statut :** Termine
