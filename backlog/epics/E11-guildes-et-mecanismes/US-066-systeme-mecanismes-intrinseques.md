## [US-066] Systeme de mecanismes intrinseques des plantes

**En tant que** jardinier,
**je veux** que chaque plante affiche ses mecanismes intrinseques (fixation d'azote, allelopathie, attraction de pollinisateurs, etc.),
**afin de** comprendre les proprietes biologiques de chaque plante independamment de ses associations.

### Criteres d'acceptation

- [x] CA1 : Les proprietes booleennes (NitrogenFixer, AllelopathicRisk, PollinatorPlant) sont remplacees par une table many-to-many `PlantIntrinsicMechanism` avec 8 types de mecanismes.
- [x] CA2 : Les 66 plantes existantes sont mises a jour avec les mecanismes intrinseques corrects via les donnees de seed.
- [x] CA3 : L'API `PlantDto` retourne la liste des mecanismes intrinseques pour chaque plante.
- [x] CA4 : Les mecanismes intrinseques apparaissent en bleu sur les fiches plantes du catalogue et de l'editeur de guilde.
- [x] CA5 : Les mecanismes relationnels (issus des associations) apparaissent en vert avec une icone de lien, distincts des intrinseques.

### Notes & contraintes
- Migration EF Core `AddPlantIntrinsicMechanisms` creee.
- Les anciens champs booleens sont supprimes de l'entite `Plant`.

### Estimation
- **Priorite :** Must
- **Points :** 5
- **Statut :** Done
