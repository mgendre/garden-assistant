## [US-003] Modifier un jardin

**En tant que** jardinier,
**je veux** pouvoir changer le nom et la description d'un jardin existant,
**afin de** corriger une erreur ou refléter une évolution de mon espace.

### Critères d'acceptation

- [x] CA1 : Je peux modifier le nom et la description depuis la page du jardin.
- [x] CA2 : Les modifications sont sauvegardées immédiatement et visibles sans rechargement de page.
- [x] CA3 : Un nom vide est refusé avec un message d'erreur explicite.
- [x] CA4 : Je ne peux modifier que mes propres jardins (403 pour les autres).

### Notes & contraintes
- Les planches et plantes existantes ne sont pas affectées par ce changement.
- Implémenté via bouton "Modifier" sur la page jardin (dialog d'édition).

### Estimation
- **Priorité :** Must
- **Points :** 1
- **Statut :** Terminé
