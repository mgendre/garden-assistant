## [US-068] Avertissements de conflits dans l'editeur de guilde

**En tant que** jardinier,
**je veux** voir clairement les associations nefastes entre les plantes de ma guilde,
**afin d'** eviter de planter ensemble des especes incompatibles.

### Criteres d'acceptation

- [x] CA1 : Lorsque des associations nefastes existent entre les plantes selectionnees, une section rouge "Conflits" apparait dans l'editeur de guilde.
- [x] CA2 : Chaque conflit affiche les noms des deux plantes concernees et les mecanismes impliques (chips rouges cliquables).
- [x] CA3 : Un clic sur un mecanisme ouvre la popup d'explication existante.

### Notes & contraintes
- Les conflits proviennent du champ `selectedPlantConflicts` de la reponse API existante.
- Pas de nouvel appel API necessaire.

### Estimation
- **Priorite :** Must
- **Points :** 2
- **Statut :** Done
