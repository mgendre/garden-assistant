## [US-127] Calendrier global du jardin avec groupement par planches

**En tant que** jardinier,
**je veux** voir un calendrier Gantt de toutes les plantes de mon jardin, avec la possibilite de grouper par planche,
**afin d'** avoir une vue d'ensemble des actions culturales a mener sur l'ensemble de mon jardin.

### Criteres d'acceptation

- [x] CA1 : Un calendrier Gantt global est disponible dans la vue jardin, affichant toutes les plantes de toutes les planches.
- [x] CA2 : Un toggle (groupe de boutons) permet de grouper les plantes par planche (avec un separateur visuel titre entre les groupes).
- [x] CA3 : Par defaut, le calendrier est en "Vue globale" (vue a plat) avec deduplication des plantes.
- [x] CA4 : Le calendrier supporte le meme niveau de detail que le calendrier existant (actions, demi-mois, highlight du mois courant).
- [x] CA5 : Le calendrier est accessible depuis la vue jardin dans un panneau collapsible (expanded par defaut).

### Notes & contraintes
- Reutilise le composant `PlantCalendarGantt` existant.
- Vue "Groupee par planche" : `section-divider-title` (extrait en classe typographique partagee) separe les groupes.
- Vue "Globale" : plantes dedupliquees (une plante presente dans plusieurs planches n'apparait qu'une fois).
- Le calendrier principal (`/companions`) integre aussi les plantes de jardins avec un filtre source : Toutes / Mes plantes / Plantes de mes jardins, et un groupement par jardin.

### Estimation
- **Priorite :** Important
- **Points :** 5
- **Statut :** Terminé
