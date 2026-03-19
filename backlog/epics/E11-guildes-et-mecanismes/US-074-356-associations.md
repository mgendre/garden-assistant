## [US-074] 356 associations vegetales documentees

**En tant que** jardinier,
**je veux** disposer d'une base de 356 associations vegetales documentees,
**afin de** recevoir des recommandations fiables basees sur des interactions reelles entre plantes.

### Criteres d'acceptation

- [x] CA1 : Le fichier de seed `associations.json` contient 356 associations couvrant les plantes du catalogue.
- [x] CA2 : Les 118 nouvelles associations couvrent les plantes des 30 nouvelles guildes.
- [x] CA3 : Chaque association specifie le mecanisme, l'effet (benefique ou nefaste), et des notes explicatives.

### Notes & contraintes
- Les associations sont chargees au demarrage via `AssociationSeeder` depuis `associations.json`.

### Estimation
- **Priorite :** Must
- **Points :** 3
- **Statut :** Done
