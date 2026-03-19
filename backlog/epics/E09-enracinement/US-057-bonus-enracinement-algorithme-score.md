## [US-057] Bonus de complementarite racinaire dans l'algorithme de score

**En tant que** jardinier,
**je veux** que le score de compatibilite entre deux plantes tienne compte de la complementarite de leurs racines,
**afin d'** obtenir des recommandations qui valorisent la stratification du sol en plus des associations documentees.

### Criteres d'acceptation

- [ ] CA1 : Le service de calcul de score applique un bonus de +10 % lorsque deux plantes ont des zones racinaires differentes (ex. superficiel + profond).
- [ ] CA2 : Le bonus est applique apres le calcul du score de base, sans effet cumulatif avec d'autres bonus eventuels.
- [ ] CA3 : Aucune penalite n'est appliquee lorsque deux plantes partagent la meme zone racinaire (les associations negatives documentees gerent deja la competition).
- [ ] CA4 : En mode debug (variable d'environnement ou flag de configuration), le bonus est trace dans les logs avec les valeurs avant et apres.

### Notes & contraintes
- Le service implemente une interface (convention du projet) ; le bonus est une responsabilite du `PlantScoringService` ou equivalent.
- Les tests unitaires couvrent : meme zone (pas de bonus), zones differentes (bonus +10 %), zones `null` (pas de bonus, pas d'erreur).
- Pas de changement de schema de base de donnees pour cette story.

### Estimation
- **Priorite :** Should
- **Points :** 3
