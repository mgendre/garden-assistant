## [US-316] PlantActionSeeder en mode upsert

**En tant que** développeur,
**je veux** transformer le PlantActionSeeder en mode upsert avec protection des plantes personnalisées,
**afin de** pouvoir corriger et enrichir les actions culturales sans écraser les modifications manuelles.

### Critères d'acceptation

- [x] CA1 : Le seeder identifie les plantes par `Key` (lookup direct en base).
- [x] CA2 : Si la plante associée est `IsCustomized == true`, l'action est ignorée.
- [x] CA3 : Si l'action n'existe pas en base, elle est insérée.
- [x] CA4 : Si l'action existe et que la plante n'est pas customisée, les champs sont mis à jour.
- [x] CA5 : Les plantes verrouillées sont chargées en un seul `SELECT` avant la boucle.
- [x] CA6 : Logging Info pour les actions mises à jour, Debug (avec guard) pour les actions ignorées.

### Notes & contraintes
- Dépend de US-312 et US-313.
- Même pattern que US-315 — possibilité d'extraire une méthode utilitaire commune pour le check IsCustomized + logging.

### Estimation
- **Priorite :** Indispensable
- **Points :** 3
- **Statut :** Termine
