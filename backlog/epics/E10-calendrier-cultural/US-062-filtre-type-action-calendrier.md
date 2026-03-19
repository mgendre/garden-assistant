## [US-062] Filtre par type d'action sur la page Calendrier

**En tant que** jardinier,
**je veux** filtrer le calendrier pour n'afficher que certains types d'actions (ex. uniquement les semis ou uniquement les recoltes),
**afin de** me concentrer sur une seule dimension de planification a la fois.

### Criteres d'acceptation

- [ ] CA1 : Des puces de filtre (chips Angular Material) au-dessus de la grille permettent d'activer ou de desactiver chaque type d'action independamment.
- [ ] CA2 : Tous les types sont actives par defaut au chargement de la page.
- [ ] CA3 : L'etat des filtres persiste durant la session navigateur (signal local) ; il est remis a zero a la prochaine visite.
- [ ] CA4 : Le filtrage est instantane cote client — pas de nouvel appel API.

### Notes & contraintes
- Le composant de chips reutilise les couleurs par type d'action definies pour la grille (US-060) pour assurer la coherence visuelle.
- YAGNI : pas de persistance en base de donnees pour les preferences de filtre.

### Estimation
- **Priorite :** Should
- **Points :** 2
