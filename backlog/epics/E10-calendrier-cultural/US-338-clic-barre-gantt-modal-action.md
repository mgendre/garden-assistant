## [US-338] Cliquer sur une barre du Gantt ouvre un modal explicatif de l'action

**En tant que** jardinier debutant,
**je veux** pouvoir cliquer sur une barre du calendrier Gantt pour obtenir une explication de l'action (recolte, taille, repiquage, division, etc.),
**afin de** comprendre ce que je dois faire et pourquoi a cette periode.

### Criteres d'acceptation

- [ ] CA1 : Les barres d'action dans le composant `plant-calendar-bar` ont un curseur `pointer` au survol.
- [ ] CA2 : Au survol (hover), la barre d'action devient plus sombre (assombrir la couleur de fond, ex: `filter: brightness(0.85)` ou opacite).
- [ ] CA3 : Un clic sur une barre d'action ouvre le modal educatif du type d'action correspondant (reutilise le `badge-info-dialog` existant avec les cles `BadgeInfo.Action.*` de US-079).
- [ ] CA4 : Si une fiche technique specifique a la plante existe (US-101, futur), elle est affichee a la place du modal generique. Sinon, fallback sur le modal generique.
- [ ] CA5 : Le clic fonctionne sur toutes les vues qui utilisent `plant-calendar-bar` : page calendrier, fiche plante, panneau associations.
- [ ] CA6 : Le composant emet un evenement `(actionClick)` avec le type d'action, laissant le parent gerer l'ouverture du modal.
- [ ] CA7 : L'interaction ne casse pas le layout existant et reste fonctionnelle sur mobile (tap).
- [ ] CA8 : `npm run build --prefix garden-assistant-app` compile sans erreur.

### Notes & contraintes
- Le composant `plant-calendar-bar` affiche de petites barres colorees (3px de haut). L'interaction hover/clic doit s'appliquer a la cellule entiere du demi-mois (pas uniquement la barre de 3px) pour rester utilisable sur mobile.
- Les popups educatives generiques (US-079) sont deja implementees — reutiliser le meme pattern.
- La transition hover doit etre subtile et rapide (`transition: filter 150ms`).

### Estimation
- **Priorite :** Important
- **Points :** 3
- **Statut :** A faire
