## [US-086] Service de traduction (CRUD + resolution par langue)

**En tant que** developpeur,
**je veux** un service de traduction capable de resoudre les valeurs traduites d'une entite en fonction de la langue demandee,
**afin de** centraliser la logique de traduction et la rendre reutilisable par tous les services existants.

### Criteres d'acceptation

- [ ] CA1 : Une interface `ITranslationService` et son implementation `TranslationService` existent.
- [ ] CA2 : Le service expose une methode `GetTranslationsAsync(string entityType, Guid entityId, string languageCode)` qui retourne un dictionnaire `field -> value`.
- [ ] CA3 : Le service expose une methode `GetBulkTranslationsAsync(string entityType, IEnumerable<Guid> entityIds, string languageCode)` pour les requetes en lot.
- [ ] CA4 : Le service expose une methode `ResolveAsync(string entityType, Guid entityId, string field, string languageCode, string fallback)` qui retourne la valeur traduite ou le fallback.
- [ ] CA5 : La strategie de resolution est : langue demandee -> langue par defaut -> valeur fallback (champ brut de l'entite).
- [ ] CA6 : Le service est enregistre dans le conteneur DI via son interface.
- [ ] CA7 : Les tests unitaires couvrent : resolution avec traduction existante, fallback vers langue par defaut, fallback vers valeur brute, requete en lot, langue inexistante.

### Notes & contraintes
- Le service ne modifie pas les entites existantes — il enrichit les DTOs au moment du mapping.
- Le service est injecte dans les services existants (PlantService, GuildService, etc.) qui l'utilisent lors de la construction des DTOs.
- Les resultats de traduction sont mis en cache en memoire par langue pour eviter des requetes repetees sur les endpoints frequents (ex. liste des plantes). Le cache est invalide au redemarrage de l'application.
- Pas d'endpoints CRUD de traduction dans cette iteration.

### Estimation
- **Priorite :** Indispensable
- **Points :** 5
