## [US-004] Supprimer un jardin

**En tant que** jardinier,
**je veux** pouvoir supprimer un jardin que je n'utilise plus,
**afin de** garder ma liste de jardins propre et pertinente.

### Critères d'acceptation

- [ ] CA1 : Une confirmation explicite est demandée avant la suppression (nom du jardin affiché dans le dialogue).
- [ ] CA2 : La suppression efface le jardin, toutes ses planches et toutes les données associées.
- [ ] CA3 : Je suis redirigé vers la liste des jardins après suppression.
- [ ] CA4 : Je ne peux supprimer que mes propres jardins.

### Notes & contraintes
- Suppression en cascade côté base de données (EF Core cascade delete).
- Action irréversible — pas de corbeille dans cette version.

### Estimation
- **Priorité :** Should
- **Points :** 2
