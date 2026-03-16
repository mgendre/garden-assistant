## [US-039] Panneau de recommandations de compagnons

**En tant que** jardinier,
**je veux** voir les plantes compagnes, les guildes et les plantes a eviter pour ma selection,
**afin de** planifier mes associations en connaissance de cause.

### Criteres d'acceptation

- [ ] CA1 : Des qu'au moins 1 plante est selectionnee, le panneau droit appelle `POST /api/plants/companions` avec les IDs des plantes selectionnees.
- [ ] CA2 : La section "Plantes benefiques" affiche les `goodCompanions` tries par score decroissant : icone (lettre initiale), nom, nom latin, et les mecanismes d'association traduits en francais.
- [ ] CA3 : Un badge compteur vert affiche le nombre de compagnons benefiques.
- [ ] CA4 : La section "Guildes associees" affiche les guildes (`guilds` de chaque `CompanionRecommendationDto`) avec nom, description, et chips des plantes membres.
- [ ] CA5 : La section "Plantes a eviter" affiche les `plantsToAvoid` avec icone, nom, et mecanismes. Un badge compteur rouge affiche le nombre.
- [ ] CA6 : Si `selectedPlantConflicts` n'est pas vide, un bandeau d'alerte s'affiche en haut du panneau indiquant les paires de plantes selectionnees qui sont incompatibles entre elles.
- [ ] CA7 : Un indicateur de chargement s'affiche pendant l'appel API.
- [ ] CA8 : Si aucun compagnon benefique n'est trouve, le message "Aucun compagnon benefique connu" s'affiche.
- [ ] CA9 : Les plantes deja selectionnees n'apparaissent pas dans les listes de compagnons benefiques.

### Notes & contraintes
- Les mecanismes (`AssociationMechanism`) sont traduits via les cles `Companions.Mechanism.*` deja definies dans `fr.json`.
- Les guildes sont dedupliquees (une guilde n'apparait qu'une fois meme si plusieurs compagnons en font partie).
- L'appel API est declenche a chaque changement de selection avec un debounce de 300ms pour eviter les appels en rafale.
- Les cles `Companions.GoodTitle`, `Companions.AvoidTitle`, `Companions.GuildsSectionTitle`, `Companions.ConflictTitle`, `Companions.ConflictBetween` existent deja.

### Estimation
- **Priorite :** Must
- **Points :** 5
- **Statut :** Done
