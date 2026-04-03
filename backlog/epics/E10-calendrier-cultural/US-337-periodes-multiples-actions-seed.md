## [US-337] Périodes multiples de semis, repiquage et récolte dans le seed

**En tant que** jardinier,
**je veux** que le calendrier reflète les multiples fenêtres de culture (semis de printemps ET d'automne, récolte d'été ET d'hiver, etc.),
**afin de** planifier mes interventions sur les bonnes périodes sans manquer une fenêtre.

### Critères d'acceptation

- [x] CA1 : L'audit du plant-expert est réalisé pour les 159 plantes du catalogue. Les plantes nécessitant des périodes multiples sont identifiées.
- [ ] CA2 : Le fichier `plant-actions.json` est mis a jour avec les periodes multiples pour les plantes suivantes (liste validee par le plant-expert) :

**Corrections fraisier :**
| Plante | Action | Modification |
|---|---|---|
| fraisier | Transplanting | Ajouter période printemps 5-8 + garder automne existante |
| fraisier | Harvest | Corriger à 10-14 |
| fraisier | Pruning | Corriger à 14-17 |
| fraisier | Division | Corriger à 14-18 |
| fraisier-remontant | Transplanting | Ajouter période automne 17-19 |

**Nouvelles périodes multiples :**
| Plante | Action | Periodes |
|---|---|---|
| radis | DirectSowing | 5-12 + 17-20 |
| radis | Harvest | 7-14 + 19-22 |
| roquette | DirectSowing | 5-10 + 15-20 |
| roquette | Harvest | 7-12 + 17-22 |
| laitue | DirectSowing | 3-12 + 15-19 |
| laitue | Transplanting | 5-12 + 15-19 |
| laitue | Harvest | 7-14 + 17-23 |
| coriandre | DirectSowing | 5-10 + 17-19 |
| persil | DirectSowing | 5-12 + 15-18 |
| cerfeuil | DirectSowing | 5-10 + 15-19 |
| chou | Transplanting | 5-10 + 13-16 |
| chou | Harvest | 9-14 + 19-24 |
| chou-fleur | Transplanting | 5-8 + 13-15 |
| chou-fleur | Harvest | 11-14 + 19-23 |
| brocoli | Transplanting | 7-10 + 13-15 |
| brocoli | Harvest | 11-14 + 19-6 |
| fenouil-bulbeux | DirectSowing | 7-10 + 13-16 |
| chou-chinois | DirectSowing | 7-8 + 13-17 |

- [ ] CA3 : La PropagationMethod du fraisier est corrigée de `Seed` à `Division` dans `plants.json`.
- [ ] CA4 : Le seed s'exécute sans erreur et les périodes multiples sont correctement insérées en base.
- [ ] CA5 : Le calendrier Gantt affiche correctement les périodes multiples (deux barres séparées pour la même action sur la même plante).
- [ ] CA6 : Les tests de validation du seed passent (pas de clés en doublon, cohérence des références).

### Notes & contraintes
- Le système supporte déjà les périodes multiples du même type d'action (navet, épinard, etc. en ont déjà).
- Le PlantSeeder upsert (E21) gère déjà les actions multiples.
- Les cas limites (pois et fève en semis d'automne) sont exclus pour le moment — ils dépendent du climat.

### Estimation
- **Priorite :** Indispensable
- **Points :** 3
- **Statut :** A faire
