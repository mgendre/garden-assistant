## [US-333] Tests unitaires pour les propriétés sol et pH

**En tant que** développeur,
**je veux** que les propriétés de sol et pH soient couvertes par des tests unitaires,
**afin de** garantir la fiabilité du mapping, de la validation et du seed.

### Critères d'acceptation

- [x] CA1 : Tests du PlantSeeder : vérifient que `SoilTypes` (liste), `OptimalPhMin`, `OptimalPhMax` sont correctement importés depuis le JSON.
- [x] CA2 : Tests du PlantSeeder : vérifient que les plantes `IsCustomized = true` ne sont pas écrasées pour les champs sol/pH.
- [x] CA3 : Tests du mapping entity vers DTO : vérifient que les champs sol/pH sont correctement mappés dans `PlantDto` (SoilTypes en liste de strings).
- [x] CA3bis : Tests du PlantSeeder : vérifient le diff des SoilTypes lors du upsert (ajout/suppression, même pattern que IntrinsicMechanisms).
- [x] CA4 : Tests de la contrainte CHECK : vérifient qu'un `OptimalPhMin > OptimalPhMax` est rejeté par EF (test d'intégration).
- [x] CA5 : Tests de la contrainte CHECK : vérifient qu'un pH hors bornes (< 3.0 ou > 9.0) est rejeté.
- [x] CA6 : Tous les tests passent (`dotnet test garden-assistant-tests`).

### Notes & contraintes
- Convention de nommage : `<Method>_When<Condition>_Should<Outcome>`.
- Framework : xUnit + Moq + Shouldly.
- CA4 et CA5 sont des tests d'intégration (nécessitent `WebApplicationFactory` ou une base de test).

### Estimation
- **Priorite :** Indispensable
- **Points :** 3
- **Statut :** Termine
