## [US-128] Role de la plante dans la guilde (backend)

**En tant que** jardinier,
**je veux** que chaque plante d'une guilde ait un role explicite (Centrale ou Compagne),
**afin de** savoir quelles plantes la guilde est concue pour servir et lesquelles jouent un role de soutien.

### Criteres d'acceptation

- [ ] CA1 : L'entite `GuildPlant` possede une propriete `Role` de type `GuildPlantRole` (enum : `Central = 0`, `Companion = 1`). La colonne en base est `role` (integer, NOT NULL, default `1` = Companion).
- [ ] CA2 : Une migration EF Core `AddGuildPlantRole` ajoute la colonne avec valeur par defaut. Les lignes existantes recoivent la valeur `Companion`.
- [ ] CA3 : `GuildPlantMemberDto` inclut le champ `Role` (string : `"Central"` ou `"Companion"`).
- [ ] CA4 : `CreateGuildRequest` et `UpdateGuildRequest` acceptent une liste d'objets `{ plantId, role }` au lieu d'une simple liste de `plantId`. La propriete `role` est optionnelle et vaut `Companion` par defaut.
- [ ] CA5 : `GuildService.CreateAsync` et `UpdateAsync` persistent le role de chaque plante.
- [ ] CA6 : Les endpoints GET retournent le role de chaque plante dans la guilde.
- [ ] CA7 : Les tests unitaires couvrent la creation/mise a jour d'une guilde avec des plantes centrales et compagnes.

### Notes & contraintes
- L'enum `GuildPlantRole` vit dans `Data/Entities/GuildPlantRole.cs`.
- Le format de `CreateGuildRequest.PlantIds` change de `List<Guid>` a `List<GuildPlantRequest>` (record avec `PlantId` + `Role?`). Rupture propre (le frontend est le seul client).
- Pas d'endpoint dedie "toggle role" : la mise a jour passe par le PUT existant sur la guilde.

### Estimation
- **Priorite :** Indispensable
- **Points :** 5
