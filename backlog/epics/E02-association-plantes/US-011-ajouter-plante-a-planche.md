## [US-011] Ajouter une plante à une planche

**En tant que** jardinier,
**je veux** associer une plante à une planche de mon jardin,
**afin de** savoir ce que je cultive sur chaque planche et planifier mes associations.

### Critères d'acceptation

- [x] CA1 : Je peux rechercher et sélectionner une plante depuis la base de données.
- [ ] CA2 : Je peux préciser la quantité ou le nombre de pieds (facultatif). → Reporté
- [x] CA3 : La plante apparaît immédiatement dans la liste des cultures de la planche.
- [x] CA4 : Je peux ajouter plusieurs plantes différentes à la même planche.
- [x] CA5 : Si la plante est incompatible avec une autre déjà présente sur la planche, un avertissement s'affiche (non bloquant).

### Notes & contraintes
- Implémenté via le flow planche → éditeur de guilde : la planche est liée à une guilde, et l'ajout de plantes passe par le catalogue de compagnons.
- L'avertissement d'incompatibilité est couvert par l'assistant de guilde (associations néfastes, pH incompatible, compétition racinaire).
- CA2 (quantité) reporté — pas de besoin immédiat.

### Estimation
- **Priorité :** Indispensable
- **Points :** 5
- **Statut :** Termine
