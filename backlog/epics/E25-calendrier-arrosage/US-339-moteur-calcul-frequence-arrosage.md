## [US-339] Moteur de calcul de la frequence d'arrosage

**En tant que** jardinier,
**je veux** que l'application calcule automatiquement la frequence d'arrosage recommandee pour chaque plante selon ses besoins en eau et la saison,
**afin de** savoir combien de fois par semaine arroser chaque plante sans consulter un guide externe.

### Criteres d'acceptation

- [ ] CA1 : Un service `IWateringCalculator` / `WateringCalculator` est cree dans `Services/`.
- [ ] CA2 : Le service expose une methode `CalculateFrequency(WaterNeeds waterNeeds, int halfMonth)` qui retourne un `WateringFrequency` (record avec `TimesPerWeek` (int), `RecommendedDays` (DayOfWeek[]) et `Notes` (string?)).
- [ ] CA3 : La matrice de base est implementee :
  - **Low** : 1x/semaine (printemps/automne), 2x/semaine (ete), 0-1x/semaine (hiver)
  - **Medium** : 2x/semaine (printemps/automne), 3-4x/semaine (ete), 1x/semaine (hiver)
  - **High** : 3x/semaine (printemps/automne), quotidien (ete), 1-2x/semaine (hiver)
- [ ] CA4 : Les saisons sont derivees du demi-mois (1-24) : hiver = 1-4 et 23-24, printemps = 5-10, ete = 11-16, automne = 17-22.
- [ ] CA5 : `RecommendedDays` contient les jours de la semaine repartis uniformement (ex : 3x/semaine = lundi, mercredi, vendredi ; 2x/semaine = mardi, samedi).
- [ ] CA6 : Le service est enregistre dans le conteneur DI.
- [ ] CA7 : Des tests unitaires couvrent toutes les combinaisons WaterNeeds x saison (minimum 12 cas) et verifient que `RecommendedDays.Length == TimesPerWeek`.

### Notes & contraintes
- La matrice est volontairement simple (YAGNI). Les ajustements sol/paillage viendront dans des stories ulterieures (US-342, US-343).
- Le demi-mois suit la convention existante de `PlantAction.HalfMonthStart` (1 = 1ere quinzaine de janvier, 24 = 2e quinzaine de decembre).
- Le `TimesPerWeek` est un entier pour rester simple. Une plage (min/max) pourra etre ajoutee plus tard si necessaire.
- La repartition des jours est algorithmique : espacement regulier sur 7 jours, pas de persistence.
- Pas de dependance a d'autres stories non livrees.

### Estimation
- **Priorite :** Indispensable
- **Points :** 3
- **Statut :** A faire
