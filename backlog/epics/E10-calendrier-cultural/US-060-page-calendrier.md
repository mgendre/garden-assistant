## [US-060] Page Calendrier — grille annuelle des actions culturales

**En tant que** jardinier,
**je veux** consulter une page "Calendrier" affichant sur une grille de 12 mois toutes les actions culturales de mes plantes,
**afin de** planifier mes interventions au jardin semaine par semaine sans perdre de temps a chercher les informations.

### Criteres d'acceptation

- [ ] CA1 : Un element "Calendrier" est visible dans la navigation principale de l'application et pointe vers la nouvelle page.
- [ ] CA2 : La grille affiche en lignes les plantes de "Mes Plantes" et en colonnes les 12 mois (Jan–Dec).
- [ ] CA3 : Chaque action applicable a un mois apparait sous forme de puce coloree dans la cellule correspondante ; la couleur est propre au type d'action (ex. vert pour semis, orange pour recolte, bleu pour repiquage, rouge pour taille).
- [ ] CA4 : Les colonnes correspondant au trimestre en cours sont mises en evidence (fond leger).
- [ ] CA5 : Sur mobile, les colonnes de mois defilent horizontalement (overflow-x) ; les noms de plantes restent fixes a gauche.
- [ ] CA6 : Si "Mes Plantes" est vide, un etat vide invite l'utilisateur a ajouter des plantes depuis le catalogue.

### Notes & contraintes
- La page appelle `GET /api/plants/{id}/actions` pour chaque plante de "Mes Plantes" ; grouper les appels ou prevoir un endpoint batch si la liste est longue (>20 plantes).
- Reutiliser le signal store "Mes Plantes" existant comme source de donnees.
- Les puces sont des composants Angular simples avec Tailwind — pas de bibliotheque de calendrier externe.
- Les cles de traduction suivent la convention : `Calendar.MonthJan`, `Calendar.ActionType.Semis`, etc.

### Estimation
- **Priorite :** Must
- **Points :** 5
