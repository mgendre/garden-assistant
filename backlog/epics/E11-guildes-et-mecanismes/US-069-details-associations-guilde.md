## [US-069] Details des associations dans l'editeur de guilde

**En tant que** jardinier,
**je veux** consulter le detail de toutes les associations entre les plantes de ma guilde,
**afin de** comprendre les interactions specifiques (mecanisme, effet, notes) entre chaque paire de plantes.

### Criteres d'acceptation

- [x] CA1 : Une section repliable "Associations" apparait dans l'editeur de guilde lorsque des associations existent entre les plantes selectionnees.
- [x] CA2 : Chaque association affiche la paire de plantes (source -> cible), le mecanisme sous forme de chip colore (vert benefique, rouge nefaste), et les notes explicatives.
- [x] CA3 : La section est repliee par defaut (composant `app-collapsible`).

### Notes & contraintes
- Les associations proviennent du champ `selectedPlantAssociations` de la reponse API.
- Reutilise le composant collapsible existant.

### Estimation
- **Priorite :** Should
- **Points :** 2
- **Statut :** Done
