## [US-037] Trier le catalogue de plantes

**En tant que** jardinier,
**je veux** trier le catalogue par nom, famille ou autre critere,
**afin de** parcourir les plantes dans l'ordre qui m'est le plus utile.

### Criteres d'acceptation

- [ ] CA1 : Des chips de tri sont affiches sous le champ de recherche : "A-Z" (defaut, actif), "Famille".
- [ ] CA2 : Cliquer sur un chip l'active (fond vert, texte blanc) et desactive le precedent.
- [ ] CA3 : Le tri "A-Z" trie par nom commun en ordre alphabetique (locale `fr`).
- [ ] CA4 : Le tri "Famille" regroupe les plantes par famille botanique, en ordre alphabetique de famille puis de nom a l'interieur de chaque famille.
- [ ] CA5 : Le tri se combine avec la recherche en cours (US-036) : si un filtre est actif, le tri s'applique aux resultats filtres.
- [ ] CA6 : Le tri selectionne est conserve quand la recherche change.

### Notes & contraintes
- Les tris "par nombre d'allies" et "par nombre d'ennemis" visibles dans la maquette ne sont pas implementes dans ce story car `PlantDto` ne contient pas ces compteurs. Ils pourront etre ajoutes dans un story futur si le backend les expose.
- Le tri est client-side (la liste complete est en memoire).

### Estimation
- **Priorite :** Should
- **Points :** 1
