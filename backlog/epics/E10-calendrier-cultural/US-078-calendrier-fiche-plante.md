## [US-078] Calendrier cultural dans la fiche plante

**En tant que** jardinier,
**je veux** voir dans la fiche detail d'une plante son calendrier cultural sous forme de bandes colorees sur 12 mois,
**afin de** connaitre les periodes de semis, repiquage, recolte et autres interventions en un coup d'oeil.

### Criteres d'acceptation

- [ ] CA1 : Une section "Calendrier cultural" apparait dans le `plant-detail-dialog`, affichant le composant `PlantCalendarGanttComponent` (meme composant que la vue deployee de US-060).
- [ ] CA2 : Le calendrier affiche une ligne par type d'action applicable a la plante, avec les barres colorees sur les mois concernes.
- [ ] CA3 : Les labels s'adaptent selon `PropagationMethod` : "Plantation" au lieu de "Semis" pour Bulbe/Tubercule.
- [ ] CA4 : Un indicateur de gel (icone flocon) apparait sur les mois a risque pour les plantes `FrostSensitive`.
- [ ] CA5 : Le mois courant est mis en evidence dans la barre.
- [ ] CA6 : La section est masquee si la plante n'a aucune action culturale.

### Notes & contraintes
- Les donnees proviennent de `GET /api/plants/{id}/actions`.
- Le composant Gantt est le meme que celui de la page calendrier (US-060) — pas de duplication.
- Les cles de traduction reutilisent `Calendar.ActionType.*`.

### Estimation
- **Priorite :** Indispensable
- **Points :** 3
