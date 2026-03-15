## [US-026] Selectionner des plantes sous forme de chips

**En tant que** jardinier,
**je veux** cliquer sur un resultat de recherche pour l'ajouter a ma selection, visible sous forme de chip,
**afin de** constituer progressivement la liste de plantes dont je veux verifier les associations.

### Criteres d'acceptation

- [ ] CA1 : Cliquer sur une plante dans les resultats de recherche l'ajoute a la selection.
- [ ] CA2 : Chaque plante selectionnee apparait comme un chip (tag) affichant le nom de la plante et un bouton de suppression (x).
- [ ] CA3 : Une plante deja selectionnee ne peut pas etre ajoutee une seconde fois ; elle est visuellement marquee comme "deja selectionnee" dans les resultats.
- [ ] CA4 : Cliquer sur le (x) d'un chip retire la plante de la selection.
- [ ] CA5 : Le champ de recherche est vide apres l'ajout d'une plante, pret pour une nouvelle recherche.
- [ ] CA6 : La zone des chips s'adapte en hauteur si plusieurs plantes sont selectionnees (pas de limite stricte de selection).

### Notes & contraintes
- Pas de nombre maximum impose pour la selection, mais l'interface doit rester utilisable avec 10+ chips (wrap sur plusieurs lignes).
- Sur mobile, les chips s'affichent en grille adaptative.

### Estimation
- **Priorite :** Must
- **Points :** 2
