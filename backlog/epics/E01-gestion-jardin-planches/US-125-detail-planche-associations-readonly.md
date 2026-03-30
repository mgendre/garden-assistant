## [US-125] Detail d'une planche avec associations en lecture seule

**En tant que** jardinier,
**je veux** voir en detail les plantes de ma planche avec leurs associations, mecanismes, besoins et calendrier quand j'ouvre le collapsible,
**afin de** comprendre l'etat de sante de ma planche sans quitter la vue jardin.

### Criteres d'acceptation

- [ ] CA1 : Une barre de sante resume les associations (X benefiques, Y nefastes, Z lacunes mecanismes).
- [ ] CA2 : Les plantes sont affichees en plant cards collapsibles (meme composant que partout ailleurs) avec : famille, eau, soleil, enracinement, mecanismes.
- [ ] CA3 : La section Associations liste les paires de plantes avec le mecanisme et l'effet (benefique/nefaste).
- [ ] CA4 : La section Assistant montre les mecanismes prioritaires satisfaits/manquants et la stratification racinaire.
- [ ] CA5 : La section Calendrier affiche un Gantt combine de toutes les plantes de la planche.
- [ ] CA6 : Toutes les sections sont en lecture seule — aucune modification possible directement.
- [ ] CA7 : Un skeleton/loader s'affiche pendant le chargement des donnees d'association.
- [ ] CA8 : Un composant shared unique `PlantAssociationPanel` est utilise a la fois par le guild editor et par la vue planche. Ce composant prend en entree les plantes et les associations et affiche toutes les sections (barre de sante, associations, mecanismes, stratification, calendrier).

### Notes & contraintes
- **Un seul composant shared** : `PlantAssociationPanel` regroupe toute la vue d'associations dans un composant reutilisable. Inputs : `plants[]`, `associations[]`, `calendarEntries[]`, `readonly` (boolean), `centralPlantIds` (Set). Le guild editor et la vue planche utilisent le meme composant.
- Le guild editor existant est refactore pour deleguer l'affichage a ce composant (pas de duplication).
- `RootStratification` est refactore pour accepter des inputs au lieu d'injecter `CompanionStore`.
- Les donnees sont chargees via `CompanionService` directement, pas via le `CompanionStore` (singleton de la page associations).
- Les sections (associations, assistant, calendrier) sont elles-memes collapsibles a l'interieur du composant.
- La barre de sante utilise des badges colores : vert (benefique), rouge (nefaste), orange (lacune).

### Estimation
- **Priorite :** Indispensable
- **Points :** 8
