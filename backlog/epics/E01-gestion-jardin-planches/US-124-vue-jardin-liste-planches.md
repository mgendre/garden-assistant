## [US-124] Vue jardin avec liste des planches en collapsibles

**En tant que** jardinier,
**je veux** voir la liste de mes planches sous forme de panneaux collapsibles quand j'ouvre un jardin,
**afin de** naviguer facilement entre mes differentes zones de culture.

### Criteres d'acceptation

- [ ] CA1 : La page jardin affiche toutes les planches du jardin selectionne en panneaux collapsibles.
- [ ] CA2 : Chaque panneau affiche le nom de la planche et le nombre de plantes associees.
- [ ] CA3 : Sur mobile, un seul panneau est ouvert a la fois (mode accordeon).
- [ ] CA4 : Sur desktop, plusieurs panneaux peuvent etre ouverts simultanement.
- [ ] CA5 : Un etat vide est affiche si le jardin n'a pas de planches ("Aucune planche. Commencez par en creer une.").
- [ ] CA6 : Les donnees d'associations ne sont chargees que quand un panneau est ouvert (lazy loading).

### Notes & contraintes
- Pattern identique a "Mes plantes" avec le composant `app-collapsible`.
- Layout single-column, centre, max-width ~800px sur desktop.

### Estimation
- **Priorite :** Indispensable
- **Points :** 3
