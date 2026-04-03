## [US-333] Tests unitaires pour les proprietes sol et pH

**En tant que** developpeur,
**je veux** que les proprietes de sol et pH soient couvertes par des tests unitaires,
**afin de** garantir la fiabilite du mapping, de la validation et du seed.

### Criteres d'acceptation

- [ ] CA1 : Tests du PlantSeeder : verifient que `SoilTypes` (liste), `OptimalPhMin`, `OptimalPhMax` sont correctement importes depuis le JSON.
- [ ] CA2 : Tests du PlantSeeder : verifient que les plantes `IsCustomized = true` ne sont pas ecrasees pour les champs sol/pH.
- [ ] CA3 : Tests du mapping entity vers DTO : verifient que les champs sol/pH sont correctement mappes dans `PlantDto` (SoilTypes en liste de strings).
- [ ] CA3bis : Tests du PlantSeeder : verifient le diff des SoilTypes lors du upsert (ajout/suppression, meme pattern que IntrinsicMechanisms).
- [ ] CA4 : Tests de la contrainte CHECK : verifient qu'un `OptimalPhMin > OptimalPhMax` est rejete par EF (test d'integration).
- [ ] CA5 : Tests de la contrainte CHECK : verifient qu'un pH hors bornes (< 3.0 ou > 9.0) est rejete.
- [ ] CA6 : Tous les tests passent (`dotnet test garden-assistant-tests`).

### Notes & contraintes
- Convention de nommage : `<Method>_When<Condition>_Should<Outcome>`.
- Framework : xUnit + Moq + Shouldly.
- CA4 et CA5 sont des tests d'integration (necessitent `WebApplicationFactory` ou une base de test).

### Estimation
- **Priorite :** Indispensable
- **Points :** 3
- **Statut :** A faire
