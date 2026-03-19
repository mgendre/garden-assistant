## [US-063] Suggestions de semis successifs dans le calendrier

**En tant que** jardinier,
**je veux** que le calendrier suggere des dates de semis echelonnees pour les plantes qui le necessitent (radis, laitue, epinard…),
**afin d'** etaler mes recoltes sur plusieurs semaines plutot que de tout avoir a la fois.

### Criteres d'acceptation

- [ ] CA1 : Les plantes dont le champ `successionSowing` est vrai affichent plusieurs marques de semis reparties regulierement sur leur fenetre de semis, espacees selon l'intervalle configure.
- [ ] CA2 : L'intervalle entre semis successifs est configurable par plante (champ `successionIntervalWeeks`, valeur par defaut : 3 semaines) ; il est inclus dans `PlantAction` ou dans un champ supplementaire de l'entite `Plant`.
- [ ] CA3 : Une infobulle sur chaque marque de semis successif explique le concept ("Semis tous les 3 semaines pour etaler la recolte").
- [ ] CA4 : Si l'intervalle ne permet qu'un seul semis dans la fenetre, une seule marque est affichee (pas de semis fictifs hors fenetre).

### Notes & contraintes
- `successionSowing` et `successionIntervalWeeks` peuvent etre des champs de l'entite `Plant` ou de l'action `SemisEnPlace` selon l'arbitrage du backend developer.
- Le calcul des dates est realise cote client a partir de `MonthStart` et `MonthEnd`.
- Les legumes de seed (US-059) incluent radis, laitue, epinard avec `successionSowing: true`.

### Estimation
- **Priorite :** Could
- **Points :** 5
