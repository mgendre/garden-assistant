## [US-316] PlantActionSeeder en mode upsert

**En tant que** developpeur,
**je veux** transformer le PlantActionSeeder en mode upsert avec protection des plantes personnalisees,
**afin de** pouvoir corriger et enrichir les actions culturales sans ecraser les modifications manuelles.

### Criteres d'acceptation

- [ ] CA1 : Le seeder identifie les plantes par `Key` (lookup direct en base).
- [ ] CA2 : Si la plante associee est `IsCustomized == true`, l'action est ignoree.
- [ ] CA3 : Si l'action n'existe pas en base, elle est inseree.
- [ ] CA4 : Si l'action existe et que la plante n'est pas customisee, les champs sont mis a jour.
- [ ] CA5 : Les plantes verrouillees sont chargees en un seul `SELECT` avant la boucle.
- [ ] CA6 : Logging Info pour les actions mises a jour, Debug (avec guard) pour les actions ignorees.

### Notes & contraintes
- Depend de US-312 et US-313.
- Meme pattern que US-315 — possibilite d'extraire une methode utilitaire commune pour le check IsCustomized + logging.

### Estimation
- **Priorite :** Indispensable
- **Points :** 3
- **Statut :** A faire
