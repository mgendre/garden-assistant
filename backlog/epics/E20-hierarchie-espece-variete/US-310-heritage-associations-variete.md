## [US-310] Heritage des associations au niveau service

**En tant que** jardinier,
**je veux** que lorsqu'une variete est consultee, ses associations (compagnonnage) soient automatiquement heritees de l'espece parente,
**afin de** connaitre les plantes compagnes d'une variete sans avoir a dupliquer les associations en base.

### Criteres d'acceptation

- [ ] CA1 : Les varietes n'ont aucune association propre en base de donnees. Toute requete d'associations pour une variete resout les associations de son parent.
- [ ] CA2 : Le service d'associations, lorsqu'il recoit un `plantId` correspondant a une variete, retourne les associations du `ParentPlantId`.
- [ ] CA3 : Dans les DTOs retournes, les associations indiquent qu'elles proviennent de l'espece parente (ex: un champ `inheritedFromParentId` ou `inheritedFromParentName`).
- [ ] CA4 : Le calcul des scores de compagnonnage (recommandations) fonctionne correctement avec des varietes : une variete est traitee comme son parent pour le calcul.
- [ ] CA5 : Les guildes contenant une variete beneficient des associations de l'espece parente pour les avertissements de conflits et les recommandations.
- [ ] CA6 : Tests unitaires couvrant : associations heritees du parent, variete sans parent (comportement normal), coherence des scores.
- [ ] CA7 : Pas de requete N+1 — la resolution du parent pour les associations est faite en une seule requete.

### Notes & contraintes
- Heritage pur a la lecture : aucune association n'est dupliquee en base pour les varietes.
- Si une variete est dans une guilde, les conflits et recommandations sont calcules via les associations du parent.
- Le endpoint `/api/plants/{id}/associations` doit fonctionner de maniere transparente, que `id` soit une espece ou une variete.

### Estimation
- **Priorite :** Indispensable
- **Points :** 5
