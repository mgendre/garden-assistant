## [US-021] Planifier la rotation de la saison suivante

**En tant que** jardinier,
**je veux** recevoir des recommandations de rotation pour chaque planche pour la prochaine saison,
**afin de** respecter les bonnes pratiques agronomiques sans avoir à tout mémoriser.

### Critères d'acceptation

- [ ] CA1 : Pour chaque planche, l'application propose une famille botanique recommandée pour la saison suivante.
- [ ] CA2 : La recommandation évite de replanter la même famille que lors des N dernières saisons (N configurable, défaut : 3 ans).
- [ ] CA3 : La recommandation tient compte de la logique classique de rotation : légumineuses → légumes-feuilles → légumes-racines → légumes-fruits.
- [ ] CA4 : Je peux accepter ou ignorer la recommandation pour chaque planche.
- [ ] CA5 : Les recommandations ignorées sont mémorisées (ne se ré-affichent pas lors du même cycle de planification).

### Notes & contraintes
- La rotation recommandée est fondée sur des règles fixes, pas sur un modèle d'IA.
- La règle des 3 ans correspond aux recommandations standard pour les Solanacées et Cucurbitacées.

### Estimation
- **Priorité :** Must
- **Points :** 8
