## [US-083] Zones climatiques avec donnees differenciees

**En tant que** jardinier,
**je veux** que l'application propose des zones climatiques (ex. plaine, moyenne altitude, altitude) avec des calendriers adaptes,
**afin de** recevoir des recommandations pertinentes pour mon environnement.

### Criteres d'acceptation

- [ ] CA1 : Un champ `ClimateZone` est ajoute sur `PlantAction` (enum nullable).
- [ ] CA2 : Les zones sont definies (a preciser lors du design) : ex. Plaine (<600m), Colline (600-1000m), Montagne (>1000m) ou Nord/Centre/Sud.
- [ ] CA3 : Le jardin de l'utilisateur est associe a une zone climatique.
- [ ] CA4 : Le calendrier filtre les actions par la zone du jardin. Si aucune action specifique a la zone n'existe, les actions sans zone (generiques) sont utilisees.
- [ ] CA5 : Les donnees de seed sont enrichies avec des fenetres differenciees par zone pour les plantes principales.

### Notes & contraintes
- Le choix des zones (altitude vs latitude vs USDA) sera decide lors du design de cette story.
- Cette story depend de US-081 et US-082.

### Estimation
- **Priorite :** Important
- **Points :** 8
