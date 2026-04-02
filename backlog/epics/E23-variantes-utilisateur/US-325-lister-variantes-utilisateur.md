## [US-325] Lister les variantes d'un utilisateur

**En tant que** jardinier,
**je veux** voir mes variantes personnelles aux cotes du catalogue global,
**afin de** choisir la bonne plante (catalogue ou variante) quand je planifie mes cultures.

### Criteres d'acceptation

- [ ] CA1 : L'endpoint `GET /api/plants` retourne les plantes du catalogue (`UserId == null`) plus les variantes de l'utilisateur courant.
- [ ] CA2 : Les variantes d'autres utilisateurs ne sont jamais visibles.
- [ ] CA3 : Chaque plante du DTO indique si c'est une variante (`isVariant: true`) et le nom du parent (`parentPlantName`).
- [ ] CA4 : Un endpoint `GET /api/plants/my-variants` retourne uniquement les variantes de l'utilisateur courant.
- [ ] CA5 : Les tests unitaires verifient : catalogue + variantes de l'utilisateur retournes, variantes d'un autre utilisateur exclues.

### Notes & contraintes
- Depend de US-324 (creation de variantes).
- L'endpoint existant `GET /api/plants` doit etre modifie pour inclure les variantes de l'utilisateur — attention a ne pas casser les consommateurs existants.

### Estimation
- **Priorite :** Indispensable
- **Points :** 3
- **Statut :** A faire
