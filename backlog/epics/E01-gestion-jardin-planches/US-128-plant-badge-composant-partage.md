## [US-128] Composant PlantBadge réutilisable

**En tant que** développeur frontend,
**je veux** un composant `PlantBadge` unique pour afficher une plante sous forme de badge coloré,
**afin d'** assurer une cohérence visuelle entre les planches, les guildes et le panneau d'associations.

### Criteres d'acceptation

- [x] CA1 : Un composant `PlantBadge` est disponible et affiche le nom de la plante dans un badge coloré.
- [x] CA2 : Le composant accepte un input pour marquer la plante comme "centrale" (accent visuel distinct).
- [x] CA3 : Le badge est utilise dans les headers des collapsibles de planches (vue jardin).
- [x] CA4 : Le badge est utilise dans les cards de guildes.
- [x] CA5 : Le badge est utilise dans le `PlantAssociationPanel`.

### Notes & contraintes
- Composant partagé — aucune duplication du style de badge dans d'autres composants.
- L'accent visuel de la plante centrale est distinct des autres badges (style différencié).

### Estimation
- **Priorite :** Indispensable
- **Points :** 1
- **Statut :** Terminé
