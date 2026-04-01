## [US-125] Detail d'une planche avec associations en lecture seule

**En tant que** jardinier,
**je veux** voir en detail les plantes de ma planche avec leurs associations, mecanismes, besoins et calendrier quand j'ouvre le collapsible,
**afin de** comprendre l'etat de sante de ma planche sans quitter la vue jardin.

### Criteres d'acceptation

- [x] CA1 : Une barre de sante resume les associations (X benefiques, Y nefastes, Z lacunes mecanismes).
- [x] CA2 : Les plantes sont affichees en plant cards collapsibles (meme composant que partout ailleurs) avec : famille, eau, soleil, enracinement, mecanismes. Controlee par l'input `showPlantCards`.
- [x] CA3 : La section Associations liste les paires de plantes avec le mecanisme et l'effet (benefique/nefaste).
- [x] CA4 : La section Assistant montre les mecanismes prioritaires satisfaits/manquants et la stratification racinaire.
- [x] CA5 : La section Calendrier affiche un Gantt combine de toutes les plantes de la planche.
- [x] CA6 : Toutes les sections sont en lecture seule — aucune modification possible directement.
- [x] CA7 : Un skeleton/loader s'affiche pendant le chargement des donnees d'association.
- [x] CA8 : Un composant shared unique `PlantAssociationPanel` est utilise a la fois par le guild editor et par la vue planche.

### Notes & contraintes
- `PlantAssociationPanel` accepte un input `showPlantCards` (boolean) pour controler l'affichage des plant cards.
- Le composant `PlantBadge` est extrait en composant reutilisable, utilise dans les planches, les guildes et le panneau d'associations.
- Les sections (associations, assistant, calendrier) sont collapsibles a l'interieur du panneau.
- Les donnees sont chargees via `CompanionService` directement, pas via `CompanionStore`.

### Estimation
- **Priorite :** Indispensable
- **Points :** 8
- **Statut :** Terminé
