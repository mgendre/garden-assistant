## [US-014] Dessiner les contours de mon jardin

**En tant que** jardinier,
**je veux** définir la forme et les dimensions de mon jardin sur un canvas interactif,
**afin d'** avoir une représentation visuelle fidèle à la réalité de mon terrain.

### Critères d'acceptation

- [ ] CA1 : Je peux dessiner un polygone libre pour définir la forme de mon jardin.
- [ ] CA2 : Je peux saisir les dimensions réelles (en mètres) pour mettre le plan à l'échelle.
- [ ] CA3 : Je peux modifier ou effacer le contour dessiné.
- [ ] CA4 : Le dessin est sauvegardé et rechargé à ma prochaine visite.
- [ ] CA5 : Sur mobile, le dessin est réalisable au doigt (touch events supportés).

### Notes & contraintes
- Utiliser une bibliothèque Canvas/SVG existante (ex. Konva.js ou Fabric.js) plutôt que de développer from scratch.
- La précision du dessin n'a pas à être au centimètre — l'expérience utilisateur prime sur la précision géométrique.

### Estimation
- **Priorité :** Must
- **Points :** 8
