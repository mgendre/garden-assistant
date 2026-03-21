## [US-061] Widget « En ce moment / Prochainement » sur le calendrier

**En tant que** jardinier,
**je veux** voir en un coup d'oeil les actions culturales a realiser en ce moment et celles a venir,
**afin de** ne rien oublier sans avoir a parcourir tout le calendrier.

### Criteres d'acceptation

- [x] CA1 : Deux panels cote a cote (2 colonnes sur desktop, empiles sur mobile) sont affiches en haut de la page Calendrier.
- [x] CA2 : Le premier panel « En ce moment » affiche les actions du demi-mois courant, groupees par type d'action avec les noms des plantes concernees.
- [x] CA3 : Le second panel « Prochainement » affiche les actions du demi-mois suivant.
- [x] CA4 : Le titre de chaque panel inclut le label du demi-mois (ex. « En ce moment — Debut mars »).
- [x] CA5 : Cliquer sur un nom de plante ouvre la fiche detail plante (dialog existant).
- [x] CA6 : Si aucune action n'est prevue, un message d'etat vide adapte est affiche.
- [x] CA7 : Les panels utilisent le style `.panel` standard du projet.

### Notes & contraintes
- Les labels de demi-mois sont traduits via `Calendar.HalfMonth.1` a `Calendar.HalfMonth.24`.
- Le filtrage se fait cote client a partir des donnees deja chargees par l'endpoint batch (US-080).

### Estimation
- **Priorite :** Important
- **Points :** 3
