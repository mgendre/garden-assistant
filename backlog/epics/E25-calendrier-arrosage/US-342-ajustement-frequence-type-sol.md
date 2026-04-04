## [US-342] Ajustement de la frequence d'arrosage selon le type de sol

**En tant que** jardinier confirme,
**je veux** que le planning d'arrosage tienne compte du type de sol de ma planche,
**afin de** recevoir des recommandations plus precises adaptees a mon terrain.

### Criteres d'acceptation

- [ ] CA1 : Si le champ `SoilType` n'existe pas encore sur l'entite `Planting` (planche), il est ajoute (enum `SoilType`, nullable) avec migration EF Core.
- [ ] CA2 : Le DTO de creation/modification de planche expose le champ `SoilType`.
- [ ] CA3 : Le `IWateringCalculator` accepte un parametre optionnel `SoilType?` dans sa methode de calcul.
- [ ] CA4 : Le type de sol applique un coefficient multiplicateur sur `TimesPerWeek` :
  - `Sandy` : x1.3 (sol drainant, seche vite)
  - `Loam` : x1.0 (aucun ajustement)
  - `Clay` : x0.7 (retient l'eau)
  - Autres types : x1.0 par defaut
- [ ] CA5 : Le resultat est arrondi a l'entier le plus proche, avec un minimum de 1 en saison active (printemps/ete/automne). En hiver, il peut descendre a 0 pour les plantes Low.
- [ ] CA6 : Les `RecommendedDays` sont recalcules apres application du coefficient pour refleter la nouvelle frequence.
- [ ] CA7 : Si `SoilType` est null, aucun ajustement n'est applique (comportement existant inchange).
- [ ] CA8 : Le frontend permet de selectionner le type de sol lors de la creation/modification d'une planche (select avec les valeurs de l'enum).
- [ ] CA9 : La grille hebdomadaire (US-341) et le composant "Arrosage aujourd'hui" (US-340) refletent automatiquement l'ajustement sol.
- [ ] CA10 : Tests unitaires couvrant les combinaisons sol x WaterNeeds x saison (minimum 12 cas, dont Sandy, Loam, Clay).
- [ ] CA11 : `npm run build` passe sans erreur.

### Notes & contraintes
- Le sol est une propriete de la planche, pas de la plante. Une planche a un seul type de sol.
- L'enum `SoilType` existe peut-etre deja dans `Data/Entities/Enums/SoilType.cs` (verifier et reutiliser).
- Cette story est independante de US-343 (paillage) mais les deux coefficients se cumulent : sol d'abord, puis paillage.
- Les coefficients sont volontairement simples (3 valeurs distinctes). Un raffinement par type de sol supplementaire pourra etre ajoute plus tard.

### Estimation
- **Priorite :** Important
- **Points :** 5
- **Statut :** A faire
