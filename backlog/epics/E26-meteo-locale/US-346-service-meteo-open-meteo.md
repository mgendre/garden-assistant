## [US-346] Service meteo backend (Open-Meteo API)

**En tant que** developpeur,
**je veux** un service backend qui recupere les donnees meteo depuis Open-Meteo,
**afin que** les fonctionnalites meteo disposent d'une source de donnees fiable et cachee.

### Criteres d'acceptation

- [ ] CA1 : Un service `IWeatherService` / `WeatherService` est cree dans `Services/`.
- [ ] CA2 : Le service appelle l'API Open-Meteo Forecast (`https://api.open-meteo.com/v1/forecast`) avec les parametres latitude/longitude.
- [ ] CA3 : Les donnees recuperees couvrent : temperature min/max journaliere, precipitations journalieres (mm), code meteo WMO, pour les 7 jours passes et 7 jours a venir (parametre `past_days=7`).
- [ ] CA4 : Les donnees sont cachees en memoire (`IMemoryCache`) avec une duree de 30 minutes par couple lat/lon arrondi a 2 decimales.
- [ ] CA5 : Un record `WeatherData` expose : `Date`, `TemperatureMin`, `TemperatureMax`, `PrecipitationMm`, `WeatherCode` (int, WMO standard).
- [ ] CA6 : En cas d'erreur API (timeout, 5xx), le service retourne `null` sans faire echouer l'appelant. Un log Warning est emis.
- [ ] CA7 : Le `HttpClient` est configure via `IHttpClientFactory` avec un timeout de 5 secondes.
- [ ] CA8 : Tests unitaires avec HttpClient mocke : cas nominal, cache hit, erreur API.

### Notes & contraintes
- Open-Meteo est gratuit, sans cle API, limite a 10 000 requetes/jour (largement suffisant).
- Pas de stockage en base en v1 : le cache memoire suffit.
- Le code meteo WMO sera utilise par le frontend pour afficher des icones (story ulterieure).
- Route API : `GET /api/weather?gardenId={id}` — le controller resout les coordonnees depuis le jardin.

### Estimation
- **Priorite :** Indispensable
- **Points :** 5
