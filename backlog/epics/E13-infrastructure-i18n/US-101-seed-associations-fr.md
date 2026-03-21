## [US-101] Migrer les seed associations FR dans `translations`

**En tant que** developpeur,
**je veux** que les notes des 356 associations vegetales soient chargees dans la table `translations` pour la langue francaise,
**afin de** centraliser toutes les traductions dans un referentiel unique.

### Criteres d'acceptation

- [ ] CA1 : Un seed charge dans `translations` le champ `Notes` de chaque `PlantAssociation` pour la langue `fr`.
- [ ] CA2 : Les valeurs dans `translations` correspondent exactement aux valeurs actuelles en base.
- [ ] CA3 : Le seed est idempotent — une re-execution ne cree pas de doublons.
- [ ] CA4 : Les tests unitaires verifient que toutes les associations ont leurs traductions FR chargees.

### Notes & contraintes
- Les valeurs de seed proviennent du fichier `Data/Seeds/associations.json` existant.
- `PlantAssociation` n'a pas de `UserId` — c'est une donnee de reference, pas utilisateur.
- Le seed s'execute apres le seed des associations et apres le seed des langues.

### Estimation
- **Priorite :** Indispensable
- **Points :** 2
