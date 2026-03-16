## [US-040] Ajouter des plantes depuis les compagnons et les guildes

**En tant que** jardinier,
**je veux** cliquer sur un compagnon benefique pour l'ajouter a ma selection, ou cliquer sur une guilde pour ajouter toutes ses plantes,
**afin de** construire progressivement une association de plantes optimale.

### Criteres d'acceptation

- [ ] CA1 : Cliquer sur une plante dans la liste "Plantes benefiques" l'ajoute a la selection (panneau central) et met a jour les recommandations (panneau droit).
- [ ] CA2 : Si la plante cliquee est deja dans la selection, le clic n'a aucun effet (pas de doublon).
- [ ] CA3 : Cliquer sur une carte de guilde recupere le detail de la guilde via `GET /api/Guilds/{id}` et ajoute toutes ses plantes a la selection.
- [ ] CA4 : Les plantes de la guilde deja presentes dans la selection ne sont pas ajoutees en doublon.
- [ ] CA5 : Apres l'ajout des plantes d'une guilde, les recommandations du panneau droit se mettent a jour automatiquement.
- [ ] CA6 : Un indicateur visuel (spinner ou feedback) confirme l'ajout pendant le rechargement des recommandations.

### Notes & contraintes
- L'appel `GET /api/Guilds/{id}` est necessaire car `GuildInfoDto` (retourne dans les recommandations) ne contient que l'id et le nom, pas les plantes membres. `GuildDetailDto` contient la liste `plants: GuildPlantMemberDto[]`.
- Il faut ensuite faire un `GET /api/Plants` (ou retrouver dans le cache local) pour obtenir les `PlantDto` complets des plantes de la guilde.
- Ce story depend de US-038 (selection multi-plantes) et US-039 (panneau compagnons).

### Estimation
- **Priorite :** Must
- **Points :** 3
- **Statut :** Done
