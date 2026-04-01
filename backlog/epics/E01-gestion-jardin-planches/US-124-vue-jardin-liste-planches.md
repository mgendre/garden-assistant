## [US-124] Vue jardin avec liste des planches en collapsibles

**En tant que** jardinier,
**je veux** voir la liste de mes planches sous forme de panneaux collapsibles quand j'ouvre un jardin,
**afin de** naviguer facilement entre mes differentes zones de culture.

### Criteres d'acceptation

- [x] CA1 : La page jardin affiche toutes les planches du jardin selectionne en panneaux collapsibles.
- [x] CA2 : Chaque panneau affiche le nom de la planche et les badges des plantes associees dans le header.
- [x] CA3 : Sur mobile, un seul panneau est ouvert a la fois (mode accordeon).
- [x] CA4 : Sur desktop, plusieurs panneaux peuvent etre ouverts simultanement.
- [x] CA5 : Un etat vide est affiche si le jardin n'a pas de planches ("Aucune planche. Commencez par en creer une.").
- [x] CA6 : Les donnees d'associations ne sont chargees que quand un panneau est ouvert (lazy loading au init).

### Notes & contraintes
- Les badges de plantes (composant `PlantBadge`) s'affichent dans le header du collapsible.
- La plante centrale est mise en accent visuel distinct dans le header.
- Les donnees de toutes les planches sont chargees au demarrage (lazy init), non au clic.
- CA2 remplace le spec initial "nombre de plantes" : les badges visuels sont plus utiles que le compte.

### Estimation
- **Priorite :** Indispensable
- **Points :** 3
- **Statut :** Terminé
