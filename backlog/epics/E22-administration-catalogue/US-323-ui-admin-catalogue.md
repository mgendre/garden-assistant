## [US-323] UI admin de gestion du catalogue

**En tant qu'** administrateur,
**je veux** disposer d'une page d'administration pour lister, modifier et deverrouiller les plantes du catalogue,
**afin de** gerer visuellement les donnees botaniques sans passer par des appels API manuels.

### Criteres d'acceptation

- [ ] CA1 : Une page `/admin/plants` liste toutes les plantes du catalogue avec pagination.
- [ ] CA2 : Chaque plante affiche un indicateur visuel (badge ou icone) si `IsCustomized == true`.
- [ ] CA3 : Un clic sur une plante ouvre un formulaire d'edition avec les champs principaux.
- [ ] CA4 : Un bouton "Deverrouiller" est disponible sur les plantes customisees, avec une confirmation avant action.
- [ ] CA5 : La page est accessible uniquement aux utilisateurs avec le role `Admin`.
- [ ] CA6 : La page est responsive (mobile-first) et utilise les composants du design system existant (panels, buttons, forms).

### Notes & contraintes
- Depend de US-320 (endpoint modifier), US-321 (endpoint deverrouiller), US-322 (endpoint associations).
- Utiliser les patterns existants : `.page-container`, `.panel`, `.btn`, `.form-input`.
- La gestion des associations dans l'UI admin est optionnelle pour cette US — se concentrer sur la liste et l'edition des plantes.

### Estimation
- **Priorite :** Important
- **Points :** 5
- **Statut :** A faire
