## [US-088] Migrer les seed guildes FR dans `translations`

**En tant que** developpeur,
**je veux** que les donnees de seed des guildes officielles (noms, descriptions) soient chargees dans la table `translations` pour la langue francaise,
**afin de** centraliser toutes les traductions dans un referentiel unique.

### Criteres d'acceptation

- [ ] CA1 : Un seed charge dans `translations` les champs `Name` et `Description` de chaque guilde officielle pour la langue `fr`.
- [ ] CA2 : Les valeurs dans `translations` correspondent exactement aux valeurs actuelles des entites `Guild` en base.
- [ ] CA3 : Le seed est idempotent — une re-execution ne cree pas de doublons.
- [ ] CA4 : Seules les guildes officielles (seed) sont concernees — les guildes utilisateur ne sont pas traduites.
- [ ] CA5 : Les tests unitaires verifient que toutes les guildes officielles ont leurs traductions FR chargees.

### Notes & contraintes
- Les valeurs de seed proviennent du fichier `Data/Seeds/guilds.json` existant.
- Le seed s'execute apres le seed des guildes et apres le seed des langues.

### Estimation
- **Priorite :** Indispensable
- **Points :** 2
