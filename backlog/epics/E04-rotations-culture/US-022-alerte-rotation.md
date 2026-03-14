## [US-022] Recevoir une alerte en cas de mauvaise rotation

**En tant que** jardinier,
**je veux** être alerté si j'essaie de planter une famille botanique trop tôt sur une planche,
**afin d'** éviter l'appauvrissement du sol et les cycles de maladies.

### Critères d'acceptation

- [ ] CA1 : Lorsque j'ajoute une plante à une planche, l'application vérifie si la famille a déjà été cultivée récemment sur cette planche.
- [ ] CA2 : Si la rotation est trop courte (moins de 3 ans pour les familles à risque), une alerte non bloquante s'affiche avec une explication.
- [ ] CA3 : L'alerte indique la dernière date de culture de cette famille sur cette planche.
- [ ] CA4 : Je peux passer outre l'alerte et planter quand même.

### Notes & contraintes
- Familles à risque prioritaires : Solanacées, Cucurbitacées, Apiacées (mildiou, fusariose, nématodes).
- L'alerte est informative, jamais bloquante — le jardinier reste maître de ses décisions.

### Estimation
- **Priorité :** Should
- **Points :** 5
