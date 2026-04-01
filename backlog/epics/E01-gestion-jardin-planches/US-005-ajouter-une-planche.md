## [US-005] Ajouter une planche à un jardin

**En tant que** jardinier,
**je veux** ajouter une planche de culture à l'un de mes jardins,
**afin de** structurer mon espace de culture en zones distinctes.

### Critères d'acceptation

- [x] CA1 : Je peux donner un nom facultatif à la planche (ex. "Planche tomates 2025").
- [x] CA2 : La planche apparaît immédiatement dans la vue du jardin après création.
- [x] CA3 : Je peux ajouter plusieurs planches au même jardin.

### Notes & contraintes
- Les dimensions et la forme de la planche sont gerees dans E03 (editeur graphique).
- Pas de position géographique dans cette story — la position sera gérée dans l'outil graphique (E03).
- Backend : `BedService` + `BedsController` (CRUD `/api/gardens/{gardenId}/beds`). Une guilde est créée automatiquement à la création de la planche.
- Frontend : dialog "Ajouter une planche" depuis la vue jardin.

### Estimation
- **Priorité :** Must
- **Points :** 2
- **Statut :** Terminé
