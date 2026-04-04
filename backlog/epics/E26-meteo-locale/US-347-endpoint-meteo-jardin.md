## [US-347] Endpoint API meteo du jardin

**En tant que** application frontend,
**je veux** un endpoint REST qui retourne les donnees meteo pour un jardin,
**afin d'** afficher la meteo locale et alimenter les adaptations d'arrosage.

### Criteres d'acceptation

- [ ] CA1 : Route `GET /api/gardens/{gardenId}/weather` retourne un `WeatherForecastDto`.
- [ ] CA2 : `WeatherForecastDto` contient : `LocationName` (string), `Days` (liste de `WeatherDayDto` : `Date`, `TemperatureMin`, `TemperatureMax`, `PrecipitationMm`, `WeatherCode`, `WeatherDescription`).
- [ ] CA3 : Si le jardin n'a pas de localisation, retourne `404 Not Found` avec un message explicite.
- [ ] CA4 : Si l'API Open-Meteo est indisponible, retourne `503 Service Unavailable`.
- [ ] CA5 : L'endpoint est protege par `[Authorize]` et verifie que le jardin appartient a l'utilisateur connecte.
- [ ] CA6 : `WeatherDescription` est une chaine humainement lisible derivee du code WMO (ex: 0 = "Ciel degagé", 61 = "Pluie légère").
- [ ] CA7 : Tests unitaires : cas nominal, jardin sans localisation, jardin d'un autre utilisateur.

### Notes & contraintes
- Le mapping WMO code -> description est un dictionnaire statique cote backend.
- La route suit la convention RESTful : la meteo est une sous-ressource du jardin.
- Depend de US-345 (localisation) et US-346 (service meteo).

### Estimation
- **Priorite :** Indispensable
- **Points :** 3
