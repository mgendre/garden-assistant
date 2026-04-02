## [US-311] PlantDto enrichi pour les varietes

**En tant que** developpeur frontend,
**je veux** que le `PlantDto` retourne par l'API indique clairement si une plante est une variete et expose les donnees resolues (fusionnees parent + variete),
**afin de** pouvoir afficher les varietes correctement sans logique d'heritage cote client.

### Criteres d'acceptation

- [ ] CA1 : `PlantDto` expose un champ `bool IsVariety` (true si `ParentPlantId` est non null).
- [ ] CA2 : `PlantDto` expose un champ `Guid? ParentPlantId` et `string? ParentPlantName` quand la plante est une variete.
- [ ] CA3 : `PlantDto` expose un champ `List<PlantSummaryDto> Varieties` quand la plante est une espece avec des varietes (liste des enfants avec Id, Name, ScientificName).
- [ ] CA4 : Toutes les proprietes du `PlantDto` sont resolues (heritage applique) — le frontend recoit toujours des donnees completes, jamais de nulls dus a l'heritage.
- [ ] CA5 : Le endpoint `GET /api/plants` retourne les especes ET les varietes. Les varietes sont identifiables par `isVariety: true`.
- [ ] CA6 : Le endpoint `GET /api/plants/{id}` retourne les donnees resolues, que l'id soit une espece ou une variete.
- [ ] CA7 : Le mapping entite → DTO applique la resolution d'heritage (via le service de la US-309) avant la projection.
- [ ] CA8 : Le build backend et les tests passent sans erreur.

### Notes & contraintes
- `PlantSummaryDto` est un DTO leger (Id, Name, ScientificName) pour lister les varietes d'une espece sans charger toutes les donnees.
- Le frontend n'a aucune logique d'heritage — tout est resolu cote serveur.
- Les endpoints existants ne cassent pas : les plantes sans hierarchie continuent de fonctionner comme avant (`IsVariety = false`, `Varieties = []`).

### Estimation
- **Priorite :** Indispensable
- **Points :** 3
