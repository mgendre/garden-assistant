## [US-349] Adaptation de l'arrosage selon les precipitations recentes

**En tant que** jardinier,
**je veux** que les recommandations d'arrosage tiennent compte de la pluie recente,
**afin de** ne pas arroser inutilement quand il a suffisamment plu.

### Criteres d'acceptation

- [ ] CA1 : Le `WateringCalculator` (US-339) accepte un parametre optionnel `recentPrecipitationMm` (decimal, precipitations cumulees des 3 derniers jours).
- [ ] CA2 : Si les precipitations des 3 derniers jours depassent 10 mm, la frequence d'arrosage de la semaine est reduite de 50 % (arrondi superieur, minimum 0).
- [ ] CA3 : Si les precipitations des 3 derniers jours depassent 20 mm, l'arrosage est supprime pour la semaine en cours (frequence = 0).
- [ ] CA4 : Un indicateur visuel est affiche sur le composant "Arrosage aujourd'hui" (US-340) quand la pluie recente a reduit ou supprime l'arrosage (ex: icone pluie + "Il a suffisamment plu, pas besoin d'arroser").
- [ ] CA5 : Le seuil de 10 mm et 20 mm est configurable via des constantes nommees dans le service (pas de magic numbers).
- [ ] CA6 : Si le jardin n'a pas de localisation (pas de donnees meteo), le calcul d'arrosage reste inchange (comportement actuel).
- [ ] CA7 : Tests unitaires : precipitations 0 mm, 10 mm, 15 mm, 20 mm, 30 mm — verification de la frequence resultante pour chaque cas.

### Notes & contraintes
- Les precipitations viennent de l'API Open-Meteo via US-346. Le service d'arrosage recoit la valeur pre-calculee, il ne connait pas l'API meteo directement (decouplage).
- Seuils inspires de la permaculture : 10 mm correspond a un arrosage moyen, 20 mm a un arrosage copieux.
- Depend de US-339 (moteur arrosage, E25) et US-346 (service meteo, E26).

### Estimation
- **Priorite :** Important
- **Points :** 5
