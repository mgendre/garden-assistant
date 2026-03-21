## [US-089] Migrer les seed maturite/criteres FR dans `translations`

**En tant que** developpeur,
**je veux** que les donnees de seed de maturite et de criteres de recolte soient chargees dans la table `translations` pour la langue francaise,
**afin de** centraliser toutes les traductions dans un referentiel unique.

### Criteres d'acceptation

- [ ] CA1 : Un seed charge dans `translations` le champ `Description` de chaque `HarvestReadiness` pour la langue `fr`.
- [ ] CA2 : Un seed charge dans `translations` le champ `Description` de chaque `HarvestReadinessCriterion` pour la langue `fr`.
- [ ] CA3 : Les valeurs dans `translations` correspondent exactement aux valeurs actuelles en base.
- [ ] CA4 : Le seed est idempotent — une re-execution ne cree pas de doublons.
- [ ] CA5 : Les tests unitaires verifient que toutes les entites de maturite ont leurs traductions FR chargees.

### Notes & contraintes
- Les valeurs de seed proviennent du fichier `Data/Seeds/harvest-readiness.json` existant.
- Le seed s'execute apres les seeds de maturite et apres le seed des langues.

### Estimation
- **Priorite :** Important
- **Points :** 2
