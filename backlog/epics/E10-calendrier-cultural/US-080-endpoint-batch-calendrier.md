## [US-080] Endpoint batch calendrier "Mes plantes"

**En tant que** developpeur frontend,
**je veux** un endpoint qui retourne les actions culturales de toutes les plantes de ma liste "Mes plantes" en un seul appel,
**afin d'** eviter N appels individuels et ameliorer les performances de la page calendrier.

### Criteres d'acceptation

- [ ] CA1 : L'endpoint `GET /api/calendar/my-plants` retourne un `CalendarDto` contenant pour chaque plante de "Mes plantes" : les infos de base (id, nom, propagationMethod, frostSensitive) et la liste de ses `PlantActionDto`.
- [ ] CA2 : L'endpoint est protege par `[Authorize]` et filtre par l'utilisateur authentifie.
- [ ] CA3 : Si l'utilisateur n'a aucune plante dans "Mes plantes", l'endpoint retourne une liste vide (pas d'erreur).
- [ ] CA4 : Les tests unitaires couvrent : utilisateur avec plantes, utilisateur sans plantes.

### Notes & contraintes
- Creer un `CalendarController` avec `[ApiController]`, `[Authorize]`, `[Route("api/calendar")]`.
- L'endpoint utilise le service `IUserPlantService` existant pour recuperer les plantes de l'utilisateur, puis `IPlantActionService` pour les actions.
- Le `CalendarDto` est un DTO dedie dans `DTOs/Calendar/`.
- Le widget "Ce mois-ci" (US-061) filtre les actions cote client a partir des donnees deja chargees par cet endpoint — pas d'endpoint `this-month` separe (YAGNI).

### Estimation
- **Priorite :** Indispensable
- **Points :** 2
