## [US-071] Panneau des guildes toujours visible

**En tant que** jardinier,
**je veux** acceder a la liste de toutes les guildes depuis l'editeur, meme sans selection de plantes,
**afin de** pouvoir explorer et charger une guilde existante a tout moment.

### Criteres d'acceptation

- [x] CA1 : Un panneau repliable "Guildes" apparait en permanence sous l'editeur de guilde.
- [x] CA2 : Sans selection de plantes, le panneau affiche toutes les guildes disponibles.
- [x] CA3 : Avec des plantes selectionnees, le panneau filtre pour montrer les guildes associees.
- [x] CA4 : Le nombre de guildes affichees est indique dans l'en-tete du panneau.

### Notes & contraintes
- Reutilise le composant `app-guild-panel` existant.
- Le panneau utilise le composant `app-collapsible` pour etre repliable.

### Estimation
- **Priorite :** Should
- **Points :** 2
- **Statut :** Done
