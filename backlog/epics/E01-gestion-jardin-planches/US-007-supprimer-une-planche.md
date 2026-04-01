## [US-007] Supprimer une planche

**En tant que** jardinier,
**je veux** supprimer une planche que je n'utilise plus,
**afin de** garder la représentation de mon jardin fidèle à la réalité.

### Critères d'acceptation

- [x] CA1 : Une confirmation est demandée avant la suppression, avec mention des plantes qui seront perdues.
- [x] CA2 : La suppression efface la planche et toutes les plantes qui lui sont associées.
- [x] CA3 : La vue du jardin est mise à jour immédiatement sans rechargement.

### Notes & contraintes
- Action irréversible. L'historique de culture (rotations) doit rester accessible même si la planche est supprimée (à confirmer avec l'équipe lors du refinement E04).
- La guilde associée à la planche est supprimée automatiquement par le backend.
- Le dialog de confirmation affiche les noms des plantes qui seront perdues.

### Estimation
- **Priorité :** Should
- **Points :** 2
- **Statut :** Terminé
