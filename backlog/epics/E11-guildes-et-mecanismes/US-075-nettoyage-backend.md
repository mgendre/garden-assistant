## [US-075] Nettoyage du backend (controleurs et DTOs inutilises)

**En tant que** developpeur,
**je veux** supprimer les controleurs, services et DTOs inutilises du backend,
**afin de** reduire la surface de code a maintenir et clarifier l'architecture.

### Criteres d'acceptation

- [x] CA1 : Les controleurs `GardenController`, `PlantingController` et `PlantingEntryController` sont supprimes.
- [x] CA2 : Les DTOs `GuildDetailDto`, `GuildSummaryDto`, `PlantToAvoidDto` et `GuildInfoDto` sont supprimes ou fusionnes dans `GuildDto` et `CompanionRecommendationDto`.
- [x] CA3 : Les services et routes associes aux controleurs supprimes sont nettoyes.
- [x] CA4 : Le build backend (`dotnet build`) passe sans erreur.

### Notes & contraintes
- Story technique (pas de valeur directe pour le jardinier, mais reduit la dette technique).
- Les entites `Garden`, `Planting`, `PlantingEntry` restent dans le modele de donnees pour usage futur.

### Estimation
- **Priorite :** Should
- **Points :** 2
- **Statut :** Done
