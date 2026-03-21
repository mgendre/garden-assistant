## [US-062] Filtre par type d'action sur la page Calendrier

**En tant que** jardinier,
**je veux** filtrer le calendrier pour n'afficher qu'un seul type d'action a la fois,
**afin de** me concentrer sur une seule dimension de planification.

### Criteres d'acceptation

- [x] CA1 : Des chips de filtre au-dessus de la grille permettent de selectionner un type d'action.
- [x] CA2 : Un seul filtre est actif a la fois (single-select). Cliquer un chip actif le desactive (retour a tout afficher).
- [x] CA3 : Les semis interieur et direct sont regroupes dans un seul filtre « Semis ».
- [x] CA4 : Quand un filtre est actif, seules les plantes ayant ce type d'action sont affichees, et le Gantt ne montre que les lignes correspondantes.
- [x] CA5 : Quand un filtre est actif, les plantes sont triees par date la plus precoce de ce type d'action.
- [x] CA6 : Sans filtre actif, tous les chips apparaissent colores. Avec un filtre, seul le chip actif est colore, les autres sont gris.
- [x] CA7 : Les chips ne s'affichent que pour les types d'action qui ont au moins une plante.
- [x] CA8 : Le filtrage est instantane cote client.

### Notes & contraintes
- Le tri par defaut (sans filtre) est : semis ascendant, puis repiquage, puis recolte, puis nom alphabetique.

### Estimation
- **Priorite :** Important
- **Points :** 2
