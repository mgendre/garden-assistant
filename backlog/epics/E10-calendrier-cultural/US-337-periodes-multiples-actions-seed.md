## [US-337] Periodes multiples de semis, repiquage et recolte dans le seed

**En tant que** jardinier,
**je veux** que le calendrier reflète les multiples fenetres de culture (semis de printemps ET d'automne, recolte d'ete ET d'hiver, etc.),
**afin de** planifier mes interventions sur les bonnes periodes sans manquer une fenetre.

### Criteres d'acceptation

- [x] CA1 : L'audit du plant-expert est realise pour les 159 plantes du catalogue. Les plantes necessitant des periodes multiples sont identifiees.
- [ ] CA2 : Le fichier `plant-actions.json` est mis a jour avec les periodes multiples pour les plantes suivantes (liste validee par le plant-expert) :

**Corrections fraisier :**
| Plante | Action | Modification |
|---|---|---|
| fraisier | Transplanting | Ajouter periode printemps 5-8 + garder automne existante |
| fraisier | Harvest | Corriger a 10-14 |
| fraisier | Pruning | Corriger a 14-17 |
| fraisier | Division | Corriger a 14-18 |
| fraisier-remontant | Transplanting | Ajouter periode automne 17-19 |

**Nouvelles periodes multiples :**
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

- [ ] CA3 : La PropagationMethod du fraisier est corrigee de `Seed` a `Division` dans `plants.json`.
- [ ] CA4 : Le seed s'execute sans erreur et les periodes multiples sont correctement inserees en base.
- [ ] CA5 : Le calendrier Gantt affiche correctement les periodes multiples (deux barres separees pour la meme action sur la meme plante).
- [ ] CA6 : Les tests de validation du seed passent (pas de cles en doublon, coherence des references).

### Notes & contraintes
- Le systeme supporte deja les periodes multiples du meme type d'action (navet, epinard, etc. en ont deja).
- Le PlantSeeder upsert (E21) gere deja les actions multiples.
- Les cas limites (pois et feve en semis d'automne) sont exclus pour le moment — ils dependent du climat.

### Estimation
- **Priorite :** Indispensable
- **Points :** 3
- **Statut :** A faire
