## [US-061] Widget "Ce mois-ci" sur le tableau de bord

**En tant que** jardinier,
**je veux** voir en un coup d'oeil les actions culturales a realiser ce mois-ci pour mes plantes,
**afin de** ne rien oublier sans avoir a parcourir tout le calendrier.

### Criteres d'acceptation

- [ ] CA1 : Une section "Ce mois-ci" est visible en haut de la page Calendrier (ou sur le tableau de bord si celui-ci existe) et liste toutes les actions dont la fenetre mensuelle inclut le mois courant.
- [ ] CA2 : Les actions sont regroupees par type (Semis, Repiquage, Recolte, Taille…) avec un intitule de groupe claire.
- [ ] CA3 : Chaque action affiche le nom de la plante concernee ; cliquer dessus ouvre la fiche plante (dialog existant ou page detail).
- [ ] CA4 : Si aucune action n'est prevue ce mois-ci, un message d'etat vide adapte est affiche (ex. "Rien a faire ce mois-ci — profitez-en pour observer votre jardin !").

### Notes & contraintes
- Le mois courant est determine cote client (`new Date().getMonth()`).
- Les donnees proviennent des appels deja realises pour la grille (US-060) : pas de nouvel appel API si les deux stories sont livrees ensemble.
- Le widget est un composant Angular reutilisable (`CalendarThisMonthComponent`).

### Estimation
- **Priorite :** Should
- **Points :** 3
