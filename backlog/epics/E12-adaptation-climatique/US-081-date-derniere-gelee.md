## [US-081] Parametre "date derniere gelee" sur le jardin

**En tant que** jardinier,
**je veux** pouvoir indiquer la date moyenne de derniere gelee pour mon jardin,
**afin que** le calendrier cultural adapte les fenetres de semis et repiquage a mon microclimat.

### Criteres d'acceptation

- [ ] CA1 : Un champ `LastFrostDate` (DateOnly nullable) est ajoute sur l'entite `Garden`.
- [ ] CA2 : Le formulaire de creation/modification du jardin permet de saisir cette date (selecteur de jour/mois, sans annee).
- [ ] CA3 : Une aide contextuelle explique ce qu'est la derniere gelee et comment la determiner pour sa region.
- [ ] CA4 : La valeur par defaut est `null` (pas de date renseignee = comportement actuel avec mois absolus).

### Notes & contraintes
- Le champ stocke un jour+mois (ex. 15 mai). L'annee n'est pas pertinente.
- Cette story pose les bases pour US-082 (calcul relatif).

### Estimation
- **Priorite :** Indispensable
- **Points :** 3
