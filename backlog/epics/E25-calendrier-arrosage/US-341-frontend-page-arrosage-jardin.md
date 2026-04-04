## [US-341] Tab "Arrosage" avec grille hebdomadaire

**En tant que** jardinier debutant,
**je veux** voir sur une grille hebdomadaire quels jours arroser chaque plante,
**afin de** suivre un planning visuel clair sans me poser de questions.

### Criteres d'acceptation

- [ ] CA1 : Un toggle tab (Actions culturales / Arrosage) est ajoute dans la page calendrier existante, utilisant un toggle-group.
- [ ] CA2 : Le tab "Actions culturales" affiche le contenu Gantt existant, inchange.
- [ ] CA3 : Un composant `calendar-watering` est cree pour le contenu du tab "Arrosage".
- [ ] CA4 : La grille hebdomadaire affiche 7 colonnes (Lun a Dim) avec une ligne par plante.
- [ ] CA5 : Les jours d'arrosage sont marques par des cercles bleus (`#42a5f5`) sur les jours recommandes (issus de `RecommendedDays` du moteur de calcul).
- [ ] CA6 : Un toggle permet de basculer entre la semaine courante et la semaine suivante.
- [ ] CA7 : Les plantes sont groupees par jardin/planche, avec le meme pattern de groupement que le Gantt existant.
- [ ] CA8 : Une section "Frequences saisonnieres" est affichee sous la grille dans un `<app-collapsible>`, avec un tableau plante / besoin en eau / frequence actuelle.
- [ ] CA9 : Le composant est mobile-first et fonctionne a partir de 320px. Sur mobile, la grille reste lisible (scroll horizontal si necessaire).
- [ ] CA10 : Un etat vide est affiche si le jardinier n'a aucune plante ("Aucune plante a arroser").
- [ ] CA11 : Tous les textes utilisent `ngx-translate` avec des cles en PascalCase (ex : `Watering.WeeklyGrid`, `Watering.CurrentWeek`).
- [ ] CA12 : Les classes du design system sont utilisees (`.page-container`, `.panel`, `.empty-state`, `<app-collapsible>`, etc.).
- [ ] CA13 : `npm run build` passe sans erreur.

### Notes & contraintes
- Le tab "Actions culturales" reste le tab par defaut a l'ouverture (comportement actuel preserve).
- Le toggle semaine courante/suivante ne necessite pas de persistence : c'est un etat local du composant.
- La couleur bleue eau (`#42a5f5`) distingue l'arrosage des actions culturales (vert).
- Les traductions fr et en sont ajoutees dans `public/i18n/`.
- Utiliser Angular signals pour le state management.
- Depend de US-339 (moteur de calcul) et de US-340 (endpoint API).

### Estimation
- **Priorite :** Indispensable
- **Points :** 5
- **Statut :** A faire
