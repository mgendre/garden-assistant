## [US-350] Adaptation de l'arrosage en periode de canicule

**En tant que** jardinier,
**je veux** que les recommandations d'arrosage augmentent automatiquement quand il fait tres chaud,
**afin de** proteger mes plantes de la deshydratation pendant les canicules.

### Criteres d'acceptation

- [ ] CA1 : Le `WateringCalculator` accepte un parametre optionnel `forecastMaxTemperature` (decimal, temperature max prevue pour les 3 prochains jours).
- [ ] CA2 : Si la temperature max prevue depasse 33 degres C, la frequence d'arrosage est augmentee de 50 % (arrondi superieur, plafond = 7, soit quotidien).
- [ ] CA3 : Un indicateur visuel "Canicule — arrosez tot le matin ou tard le soir" est affiche sur le composant "Arrosage aujourd'hui" quand le seuil est atteint.
- [ ] CA4 : Le seuil de 33 degres C est configurable via une constante nommee.
- [ ] CA5 : L'adaptation canicule et l'adaptation precipitations (US-349) se cumulent : s'il fait 35 degres C mais qu'il a plu 25 mm, la pluie prime (pas d'arrosage).
- [ ] CA6 : Tests unitaires : temperature 25, 33, 35, 40 degres C — verification de la frequence resultante. Test de cumul avec precipitations.

### Notes & contraintes
- Le seuil de 33 degres C est un compromis : en France metropolitaine, les plans canicule se declenchent a 35 degres C, mais les plantes souffrent des 33 degres C.
- La recommandation d'arroser matin/soir est un conseil permaculture standard pour eviter l'evaporation.
- Depend de US-339 (moteur arrosage, E25) et US-346 (service meteo, E26).

### Estimation
- **Priorite :** Important
- **Points :** 3
