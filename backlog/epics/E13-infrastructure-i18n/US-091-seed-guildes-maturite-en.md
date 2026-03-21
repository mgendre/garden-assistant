## [US-091] Seed des traductions EN pour guildes, maturite, associations et actions

**En tant que** jardinier anglophone,
**je veux** que les guildes officielles, criteres de maturite, notes d'associations et notes d'actions culturales soient disponibles en anglais,
**afin de** pouvoir utiliser l'application dans ma langue.

### Criteres d'acceptation

- [ ] CA1 : Un seed charge les traductions anglaises des champs `Name` et `Description` de chaque guilde officielle.
- [ ] CA2 : Un seed charge les traductions anglaises du champ `Description` de chaque `HarvestReadiness` et `HarvestReadinessCriterion`.
- [ ] CA3 : Un seed charge les traductions anglaises du champ `Notes` de chaque `PlantAssociation`.
- [ ] CA4 : Un seed charge les traductions anglaises du champ `Notes` de chaque `PlantAction`.
- [ ] CA5 : Les traductions sont de qualite equivalente aux textes francais.
- [ ] CA6 : Le seed est idempotent.
- [ ] CA7 : Les tests unitaires verifient que toutes les entites ont leurs traductions EN chargees.

### Notes & contraintes
- Les traductions anglaises sont produites ou validees par l'agent `plant-expert`.
- Seules les guildes officielles (seed) sont concernees.
- Les notes d'associations (356) et d'actions culturales (302+) representent un volume important — prevoir un fichier de seed dedie.

### Estimation
- **Priorite :** Important
- **Points :** 5
