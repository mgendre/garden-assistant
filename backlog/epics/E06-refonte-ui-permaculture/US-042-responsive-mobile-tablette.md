## [US-042] Adaptation responsive mobile et tablette

**En tant que** jardinier consultant mon telephone ou ma tablette au jardin,
**je veux** que la page d'associations soit lisible et utilisable sur petit ecran,
**afin de** verifier mes associations directement sur place.

### Criteres d'acceptation

- [ ] CA1 : Sur ecran < 768px, les trois colonnes s'empilent verticalement : catalogue en haut, puis detail, puis compagnons.
- [ ] CA2 : Sur mobile, le catalogue est replie par defaut et affiche uniquement le champ de recherche. Un bouton permet de deplier la liste complete.
- [ ] CA3 : Les fiches detail occupent toute la largeur disponible.
- [ ] CA4 : Les cartes du panneau droit (compagnons, guildes, plantes a eviter) s'empilent verticalement et occupent toute la largeur.
- [ ] CA5 : Aucun defilement horizontal n'est necessaire sur un ecran de 320px de large.
- [ ] CA6 : Sur tablette (768px - 1024px), le layout passe en deux colonnes : catalogue + detail fusionnes a gauche, compagnons a droite.

### Notes & contraintes
- Utiliser les breakpoints Tailwind (`sm:`, `md:`, `lg:`) conformement aux conventions du projet.
- Tester sur les largeurs 320px (petit mobile), 375px (iPhone SE), 768px (tablette portrait), et 1024px (tablette paysage).
- Ce story est independant du contenu des panneaux mais depend de US-034 (layout de base).

### Estimation
- **Priorite :** Should
- **Points :** 3
- **Statut :** Done
