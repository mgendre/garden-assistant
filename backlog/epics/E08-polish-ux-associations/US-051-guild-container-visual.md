## [US-051] Conteneur visuel de guilde autour des plantes selectionnees

**En tant que** jardinier,
**je veux** voir un encadrement visuel autour des plantes que j'ai selectionnees quand elles forment une guilde,
**afin de** comprendre visuellement que ces plantes forment un groupe complementaire.

### Criteres d'acceptation

- [x] CA1 : Un conteneur `.guild-container` entoure le panneau de detail des plantes selectionnees sur la page Associations.
- [x] CA2 : Le conteneur n'est visible (bordure, ombre, fond) que lorsque des guildes existent pour les plantes selectionnees (classe conditionnelle `.guild-active`).
- [x] CA3 : Un header avec emoji et titre "Guilde" s'affiche en haut du conteneur quand il est actif.
- [x] CA4 : Le style utilise une bordure orange subtile et un fond blanc avec ombre douce.

### Notes & contraintes
- Les styles sont dans `companions.scss` (specifique a la page).
- Le panneau de guilde (`guild-panel`) reste en dehors du conteneur, en dessous.

### Estimation
- **Priorite :** Could
- **Points :** 1
- **Statut :** Done
