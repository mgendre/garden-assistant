## [US-032] Adapter la page aux ecrans mobiles

**En tant que** jardinier consultant mon telephone au jardin,
**je veux** que la page de compagnonnage soit lisible et utilisable sur un petit ecran,
**afin de** verifier mes associations directement sur place.

### Criteres d'acceptation

- [ ] CA1 : Sur ecran < 768px, la mise en page passe en colonne unique (recherche en haut, resultats en dessous).
- [ ] CA2 : Le champ de recherche occupe toute la largeur disponible.
- [ ] CA3 : Les chips de selection s'affichent en wrap sans debordement horizontal.
- [ ] CA4 : Les cartes de resultats (compagnons et plantes a eviter) sont empilees verticalement et occupent toute la largeur.
- [ ] CA5 : Les badges de guilde restent lisibles sans etre tronques.
- [ ] CA6 : Aucun defilement horizontal n'est necessaire.

### Notes & contraintes
- Utiliser les breakpoints Tailwind standards (`sm`, `md`, `lg`).
- Tester sur les largeurs 375px (iPhone SE) et 768px (tablette portrait).

### Estimation
- **Priorite :** Should
- **Points :** 2
