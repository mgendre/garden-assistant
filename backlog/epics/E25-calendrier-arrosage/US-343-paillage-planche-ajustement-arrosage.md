## [US-343] Paillage sur planche : champ booleen et ajustement de l'arrosage

**En tant que** jardinier confirme,
**je veux** indiquer si ma planche est paillee et voir les recommandations d'arrosage reduites en consequence,
**afin de** ne pas sur-arroser un sol protege par du paillis.

### Criteres d'acceptation

- [ ] CA1 : L'entite `Planting` possede un nouveau champ `HasMulch` (bool, defaut `false`).
- [ ] CA2 : Une migration EF Core est generee pour ajouter la colonne `has_mulch` a la table `plantings`.
- [ ] CA3 : Le DTO de creation/modification de planche expose le champ `HasMulch`.
- [ ] CA4 : Le `IWateringCalculator` accepte un parametre optionnel `bool hasMulch` dans sa methode de calcul.
- [ ] CA5 : Quand `hasMulch` est `true`, un coefficient x0.6 est applique sur `TimesPerWeek` (reduction de 40%), arrondi a l'entier le plus proche, minimum 1 en saison active.
- [ ] CA6 : L'ajustement paillage se cumule avec l'ajustement sol (US-342) : le calcul est sol d'abord, puis paillage sur le resultat.
- [ ] CA7 : Les `RecommendedDays` sont recalcules apres application du coefficient pour refleter la nouvelle frequence.
- [ ] CA8 : Le frontend affiche un toggle "Planche paillee" dans le formulaire de creation/modification de planche.
- [ ] CA9 : La grille hebdomadaire (US-341) et le composant "Arrosage aujourd'hui" (US-340) refletent automatiquement l'ajustement paillage.
- [ ] CA10 : Tests unitaires couvrant les cas avec et sans paillage, combines avec differents WaterNeeds et saisons (minimum 6 cas).
- [ ] CA11 : `npm run build` passe sans erreur.

### Notes & contraintes
- Le paillage est un booleen simple pour l'instant (YAGNI). Un type de paillis (paille, BRF, tonte, etc.) avec des coefficients differents pourrait etre une evolution future mais n'est pas dans le scope.
- La reduction de 40% (x0.6) est conforme aux recommandations du plant expert. Le paillage reduit significativement l'evaporation.
- Cette story est livrable independamment de US-342 (sol) : si le sol n'est pas renseigne, seul le paillage s'applique.
- Le champ se nomme `HasMulch` (et non `IsMulched`) pour suivre la convention de nommage du design valide.

### Estimation
- **Priorite :** Important
- **Points :** 3
- **Statut :** A faire
