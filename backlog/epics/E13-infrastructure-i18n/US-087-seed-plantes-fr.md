## [US-087] Migrer les seed plantes FR dans `translations`

**En tant que** developpeur,
**je veux** que les donnees de seed des plantes (noms, descriptions, familles, genres) soient chargees dans la table `translations` pour la langue francaise,
**afin de** centraliser toutes les traductions dans un referentiel unique.

### Criteres d'acceptation

- [ ] CA1 : Un seed charge dans `translations` les champs `Name`, `Description`, `Family` et `Genus` de chaque plante pour la langue `fr`.
- [ ] CA2 : Les valeurs dans `translations` correspondent exactement aux valeurs actuelles des entites `Plant` en base.
- [ ] CA3 : Les champs originaux des entites `Plant` conservent leur valeur francaise (retrocompatibilite).
- [ ] CA4 : Le seed est idempotent — une re-execution ne cree pas de doublons.
- [ ] CA5 : Les tests unitaires verifient que toutes les plantes ont leurs traductions FR chargees.

### Notes & contraintes
- `ScientificName` est exclu — le latin est independant de la langue.
- Les valeurs de seed proviennent du fichier `Data/Seeds/plants.json` existant.
- Le seed s'execute apres le seed des plantes et apres le seed des langues.

### Estimation
- **Priorite :** Indispensable
- **Points :** 3
