## [US-060] Page Calendrier — vue Gantt par plante

**En tant que** jardinier,
**je veux** consulter une page "Calendrier" affichant pour chaque plante de ma liste un diagramme Gantt de ses actions culturales sur 12 mois en demi-mois,
**afin de** planifier mes interventions au jardin en un coup d'oeil.

### Criteres d'acceptation

- [ ] CA1 : Un element "Calendrier" est visible dans la navigation principale et pointe vers la nouvelle page (`/calendar`).
- [ ] CA2 : Chaque plante de "Mes Plantes" est affichee dans une carte avec son nom et un diagramme Gantt (`PlantCalendarGanttComponent`) montrant une ligne par type d'action avec barres horizontales colorees sur les demi-mois.
- [ ] CA3 : Les plantes sont triees par nom.
- [ ] CA4 : Le composant Gantt est reutilisable et egalement utilise dans la fiche plante (US-078).
- [ ] CA5 : Les colonnes correspondant au mois en cours sont mises en evidence (fond leger).
- [ ] CA6 : Les plantes `FrostSensitive` affichent un indicateur de gel (icone flocon) sur les demi-mois a risque (avant mi-mai) pour les actions de repiquage et semis en pleine terre.
- [ ] CA7 : Les labels s'adaptent selon `PropagationMethod` : "Plantation" au lieu de "Semis" pour les plantes de type Bulbe ou Tubercule.
- [ ] CA8 : Sur mobile, chaque carte Gantt defilent horizontalement (overflow-x).
- [ ] CA9 : Si "Mes Plantes" est vide, un etat vide invite l'utilisateur a ajouter des plantes depuis le catalogue.
- [ ] CA10 : Le `PlantStore` existant est utilise pour les informations plante (nom, propagationMethod, frostSensitive) — pas de duplication de donnees dans le DTO calendrier.

### Notes & contraintes
- La page utilise l'endpoint batch `GET /api/calendar/my-plants` (US-080) pour charger les actions. Le `CalendarPlantDto` ne contient que `PlantId` et `Actions`.
- Le `PlantStore` (toujours charge) fournit les informations plante.
- Le composant `PlantCalendarGanttComponent` est un composant Angular standalone avec Tailwind.
- Le filtrage par type d'action est couvert par US-062 (story separee).
- Les cles de traduction suivent la convention : `Calendar.ActionType.IndoorSowing`, etc.

### Estimation
- **Priorite :** Indispensable
- **Points :** 5
