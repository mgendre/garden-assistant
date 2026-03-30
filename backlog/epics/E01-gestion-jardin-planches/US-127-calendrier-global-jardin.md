## [US-127] Calendrier global du jardin avec groupement par planches

**En tant que** jardinier,
**je veux** voir un calendrier Gantt de toutes les plantes de mon jardin, avec la possibilite de grouper par planche,
**afin d'** avoir une vue d'ensemble des actions culturales a mener sur l'ensemble de mon jardin.

### Criteres d'acceptation

- [ ] CA1 : Un calendrier Gantt global est disponible dans la vue jardin, affichant toutes les plantes de toutes les planches.
- [ ] CA2 : Un toggle permet de grouper les plantes par planche (avec un separateur visuel entre les groupes).
- [ ] CA3 : Par defaut, les plantes sont affichees a plat (sans groupement).
- [ ] CA4 : Le calendrier supporte le meme niveau de detail que le calendrier existant (actions, demi-mois, highlight du mois courant).
- [ ] CA5 : Le calendrier est accessible depuis la vue jardin (section ou onglet dedie).

### Notes & contraintes
- Reutilise le composant `PlantCalendarGantt` existant.
- Le groupement par planche ajoute des headers de section dans le Gantt.
- Sur mobile, le Gantt scrolle horizontalement (comportement existant).

### Estimation
- **Priorite :** Important
- **Points :** 5
