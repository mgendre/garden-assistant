## [US-002] Lister mes jardins

**En tant que** jardinier,
**je veux** voir la liste de tous mes jardins sur une page dédiée,
**afin de** naviguer rapidement vers celui que je veux consulter ou modifier.

### Critères d'acceptation

- [x] CA1 : La liste affiche le nom, la description courte et le nombre de planches de chaque jardin.
- [x] CA2 : La liste est vide et un message d'invitation s'affiche si je n'ai pas encore de jardin.
- [x] CA3 : Je peux accéder à un jardin en cliquant dessus.
- [x] CA4 : Seuls mes jardins apparaissent (isolation par utilisateur).

### Notes & contraintes
- Tri par défaut : alphabétique (implémenté) — la spec initiale prévoyait date de création décroissante.
- Pagination inutile pour l'instant (YAGNI) — à reconsidérer au-delà de 50 jardins.
- Composant `list-panel` réutilisable créé pour l'affichage en liste.
- Navigation : "Mes jardins" est le premier item de la nav, route par défaut `/garden`.

### Estimation
- **Priorité :** Must
- **Points :** 2
- **Statut :** Terminé
