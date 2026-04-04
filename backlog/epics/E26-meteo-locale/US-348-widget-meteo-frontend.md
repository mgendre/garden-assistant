## [US-348] Widget meteo sur la page jardin

**En tant que** jardinier,
**je veux** voir la meteo actuelle et les previsions sur ma page jardin,
**afin de** planifier mes interventions en un coup d'oeil sans quitter l'application.

### Criteres d'acceptation

- [ ] CA1 : Un composant `WeatherWidgetComponent` affiche la meteo du jour (temperature min/max, icone meteo, precipitations) et un resume des 3 prochains jours.
- [ ] CA2 : Les icones meteo sont derivees du code WMO (soleil, nuages, pluie, neige, orage — 5 a 8 icones suffisent). Utiliser des icones SVG ou Material Icons.
- [ ] CA3 : Le widget est affiche en haut de la page jardin, uniquement si le jardin a une localisation.
- [ ] CA4 : Si les donnees meteo sont indisponibles (API down), le widget affiche un message discret "Meteo indisponible" sans bloquer le reste de la page.
- [ ] CA5 : Le widget est responsive (mobile-first) : affichage compact sur mobile, etendu sur desktop.
- [ ] CA6 : Un service Angular `WeatherService` appelle l'endpoint `GET /api/gardens/{gardenId}/weather` et expose les donnees via un signal.
- [ ] CA7 : Les textes utilisent ngx-translate (cles `Weather.Today`, `Weather.Forecast`, `Weather.Unavailable`, etc.).
- [ ] CA8 : Le widget affiche les precipitations cumulees des 3 derniers jours (ex: "12 mm de pluie ces 3 derniers jours").

### Notes & contraintes
- Le widget ne se rafraichit pas automatiquement (pas de polling). L'utilisateur recharge la page pour actualiser.
- Le design suit le pattern `.panel` du projet.
- Depend de US-347 (endpoint API).

### Estimation
- **Priorite :** Indispensable
- **Points :** 5
