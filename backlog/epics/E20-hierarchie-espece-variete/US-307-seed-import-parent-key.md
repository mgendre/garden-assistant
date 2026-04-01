## [US-307] Support du champ parentKey dans le seed import

**En tant que** developpeur backend,
**je veux** que le processus d'import des seeds (`plants.json`) supporte un champ `parentKey` sur les entrees de type variete,
**afin de** lier automatiquement chaque variete a son espece parente lors du seeding initial.

### Criteres d'acceptation

- [ ] CA1 : Le modele de deserialization du seed (`PlantSeedDto` ou equivalent) accepte un champ optionnel `parentKey` (string, correspondant au `key` de la plante parente).
- [ ] CA2 : Le processus de seed resout `parentKey` vers le `Id` de la plante parente deja inseree et affecte `ParentPlantId`.
- [ ] CA3 : Si `parentKey` reference une plante inexistante, le seed echoue avec un message d'erreur explicite.
- [ ] CA4 : Les plantes parentes sont inserees avant leurs varietes (tri topologique ou deux passes).
- [ ] CA5 : Le seed est idempotent : relancer le seed ne cree pas de doublons et met a jour les liens parent/enfant.
- [ ] CA6 : `dotnet build` et les tests existants passent sans erreur.

### Notes & contraintes
- Le champ `parentKey` utilise la meme cle que le champ `key` existant dans `plants.json` (ex: `"parentKey": "courge"`).
- Le seed doit gerer l'ordre d'insertion : les especes d'abord, les varietes ensuite.

### Estimation
- **Priorite :** Indispensable
- **Points :** 3
