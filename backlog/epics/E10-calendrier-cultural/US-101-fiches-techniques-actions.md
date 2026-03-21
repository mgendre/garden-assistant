## [US-101] Fiches techniques par action culturale et par plante

**En tant que** jardinier,
**je veux** pouvoir cliquer sur chaque type d'action dans le calendrier d'une plante (taille, pincage, buttage, semis, etc.) pour voir des instructions detaillees specifiques a cette plante,
**afin de** savoir exactement comment realiser chaque intervention sur chaque legume.

### Criteres d'acceptation

- [ ] CA1 : Un modele de donnees `PlantActionGuide` existe avec les champs : `Id`, `PlantId` (FK), `ActionType`, `Description` (texte detaille), et optionnellement des criteres structures (similaire a HarvestReadiness).
- [ ] CA2 : Les donnees de seed couvrent les interventions cles : comment tailler les tomates (gourmands), comment pincer le basilic, comment butter les poireaux, comment recolter l'ail, etc.
- [ ] CA3 : Cliquer sur le label d'une action dans le Gantt ouvre un popup avec les instructions specifiques a cette plante pour cette action (au lieu du popup educatif generique).
- [ ] CA4 : Si la plante n'a pas de fiche technique pour cette action, le popup educatif generique (BadgeInfo) est affiche en fallback.
- [ ] CA5 : Le meme pattern que « Pret a recolter » est utilise : popup dialog avec texte descriptif + criteres visuels.

### Notes & contraintes
- Etend le pattern de HarvestReadiness a tous les types d'actions (pas seulement la recolte).
- Les notes existantes dans `PlantAction.Notes` peuvent servir de base mais sont trop courtes — les fiches techniques sont plus detaillees.
- Les donnees de seed proviennent de l'agent `plant-expert`.

### Estimation
- **Priorite :** Important
- **Points :** 8
