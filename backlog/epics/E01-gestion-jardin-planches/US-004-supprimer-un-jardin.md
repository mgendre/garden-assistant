## [US-004] Supprimer un jardin

**En tant que** jardinier,
**je veux** pouvoir supprimer un jardin que je n'utilise plus,
**afin de** garder ma liste de jardins propre et pertinente.

### Critères d'acceptation

- [x] CA1 : Une confirmation explicite est demandée avant la suppression (nom du jardin affiché dans le dialogue).
- [x] CA2 : La suppression efface le jardin, toutes ses planches et toutes les données associées.
- [x] CA3 : Je suis redirigé vers la liste des jardins après suppression.
- [x] CA4 : Je ne peux supprimer que mes propres jardins.

### Notes & contraintes
- Suppression en cascade côté base de données (EF Core cascade delete).
- Action irréversible — pas de corbeille dans cette version.
- Bouton "Supprimer" accessible depuis la page du jardin.

### Estimation
- **Priorité :** Should
- **Points :** 2
- **Statut :** Terminé
