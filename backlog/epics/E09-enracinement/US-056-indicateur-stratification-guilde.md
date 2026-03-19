## [US-056] Indicateur de stratification racinaire dans l'editeur de guilde

**En tant que** jardinier,
**je veux** visualiser la coupe transversale du sol pour ma selection de plantes dans l'editeur de guilde,
**afin de** verifier que mes plantes occupent des zones racinaires complementaires et non concurrentes.

### Criteres d'acceptation

- [ ] CA1 : Lorsque au moins 2 plantes sont selectionnees dans le panneau central, une section "Stratification racinaire" apparait dans ce meme panneau.
- [ ] CA2 : Chaque plante est positionnee dans la bonne zone de la coupe : Superficiel (0-30 cm), Moyen (30-60 cm) ou Profond (>60 cm) selon son champ `rootDepth`.
- [ ] CA3 : Les plantes partageant la meme zone sont mises en evidence visuellement comme concurrentes potentielles (ex. contour ambre).
- [ ] CA4 : Sur mobile, la section est repliee par defaut et peut etre developpee par l'utilisateur.

### Notes & contraintes
- Le rendu de la coupe est un composant Angular pur (SVG ou div avec Tailwind), sans bibliotheque de visualisation supplementaire.
- Les donnees `rootDepth` viennent du signal de selection existant — pas de nouvel appel API.
- Ne pas dupliquer la logique de detection de concurrence : reutiliser ou appeler le meme calcul que US-058 si les deux stories sont livrees ensemble.

### Estimation
- **Priorite :** Should
- **Points :** 3
