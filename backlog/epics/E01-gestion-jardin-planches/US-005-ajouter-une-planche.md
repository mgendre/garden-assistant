## [US-005] Ajouter une planche à un jardin

**En tant que** jardinier,
**je veux** ajouter une planche de culture à l'un de mes jardins en précisant ses dimensions,
**afin de** structurer mon espace de culture en zones distinctes.

### Critères d'acceptation

- [ ] CA1 : Je peux saisir la longueur et la largeur de la planche (en mètres, décimales acceptées).
- [ ] CA2 : Je peux donner un nom facultatif à la planche (ex. "Planche tomates 2025").
- [ ] CA3 : La planche apparaît immédiatement dans la vue du jardin après création.
- [ ] CA4 : Les dimensions doivent être supérieures à 0 — un message d'erreur s'affiche sinon.
- [ ] CA5 : Je peux ajouter plusieurs planches au même jardin.

### Notes & contraintes
- Une planche en permaculture fait rarement plus de 1,2 m de large (accès sans piétiner). Pas de validation stricte côté app, mais un avertissement si la largeur dépasse 1,5 m.
- Pas de position géographique dans cette story — la position sera gérée dans l'outil graphique (E03).

### Estimation
- **Priorité :** Must
- **Points :** 3
