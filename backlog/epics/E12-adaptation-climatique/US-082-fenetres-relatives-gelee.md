## [US-082] Calcul des fenetres de semis/repiquage relatives a la derniere gelee

**En tant que** jardinier,
**je veux** que les fenetres de semis interieur et de repiquage des plantes geleives soient calculees relativement a ma date de derniere gelee,
**afin que** le calendrier soit precis pour mon microclimat plutot que base sur des moyennes generiques.

### Criteres d'acceptation

- [ ] CA1 : Les actions de type `IndoorSowing` et `Transplanting` des plantes `FrostSensitive` sont recalculees si le jardin a une `LastFrostDate` renseignee.
- [ ] CA2 : Le calcul utilise un decalage par plante (ex. tomate : semis interieur = 8 semaines avant derniere gelee, repiquage = 2 semaines apres).
- [ ] CA3 : Un champ `WeeksBeforeLastFrost` (int nullable) et `WeeksAfterLastFrost` (int nullable) sont ajoutes sur `PlantAction` pour les plantes geleives.
- [ ] CA4 : Si `LastFrostDate` n'est pas renseignee, le comportement par defaut (mois absolus) est conserve.
- [ ] CA5 : Le calendrier frontend affiche les fenetres ajustees quand disponibles.

### Notes & contraintes
- Le calcul est cote serveur pour eviter la duplication de logique.
- Les decalages sont fournis dans les donnees de seed.

### Estimation
- **Priorite :** Indispensable
- **Points :** 5
