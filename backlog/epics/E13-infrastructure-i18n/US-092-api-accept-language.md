## [US-092] API: header `Accept-Language` et resolution dans les endpoints existants

**En tant que** developpeur frontend,
**je veux** que l'API retourne les donnees traduites en fonction du header `Accept-Language` envoye par le client,
**afin de** ne pas avoir a gerer la traduction des donnees cote frontend.

### Criteres d'acceptation

- [ ] CA1 : Un middleware ou service ASP.NET Core lit le header `Accept-Language` de chaque requete et rend le code langue disponible via DI (ex. `ILanguageContext`).
- [ ] CA2 : Les services existants (`PlantService`, `GuildService`, etc.) utilisent le `TranslationService` pour enrichir les DTOs avec les valeurs traduites.
- [ ] CA3 : L'endpoint `GET /api/plants` retourne les plantes avec les champs traduits selon la langue demandee.
- [ ] CA4 : L'endpoint `GET /api/guilds` retourne les guildes avec les champs traduits selon la langue demandee.
- [ ] CA5 : Les endpoints de detail (plant detail, guild detail) retournent egalement les traductions.
- [ ] CA6 : Si le header `Accept-Language` est absent, la langue par defaut (FR) est utilisee.
- [ ] CA7 : Le contrat API (DTOs) ne change pas — les champs existants contiennent simplement la valeur traduite.
- [ ] CA8 : Les tests unitaires couvrent la resolution de langue via header et le fallback.

### Notes & contraintes
- Utiliser `RequestLocalizationMiddleware` d'ASP.NET Core ou un middleware custom leger.
- Les guildes utilisateur ne sont pas affectees — elles retournent leur contenu brut.
- Les performances sont a surveiller — les requetes en lot (bulk) doivent utiliser `GetBulkTranslationsAsync`.

### Estimation
- **Priorite :** Indispensable
- **Points :** 5
