## [US-065] Indicateur d'associations importantes manquantes

**En tant que** jardinier,
**je veux** voir quelles associations importantes manquent dans ma selection de plantes,
**afin de** completer intelligemment ma guilde et ne pas rater des synergies cles.

### Criteres d'acceptation

- [ ] CA1 : Lorsque au moins 2 plantes sont selectionnees, une section "Associations manquantes" apparait dans le panneau de recommandations et liste les plantes non selectionnees ayant des associations positives documentees avec au moins une plante de la selection.
- [ ] CA2 : Les associations manquantes sont triees par niveau de confiance decroissant (PeerReviewed > FieldObserved > Anecdotal) ; seules les associations de niveau FieldObserved ou superieur sont affichees par defaut.
- [ ] CA3 : Chaque entree affiche : le nom de la plante suggeree, le benefice apporte (libelle de l'association), et le niveau de confiance sous forme de badge.
- [ ] CA4 : Un bouton "Ajouter" sur chaque entree ajoute directement la plante suggeree a la selection courante, de la meme maniere que le bouton d'ajout existant dans le panneau de recommandations.
- [ ] CA5 : Sur mobile, la section est repliee par defaut et peut etre developpee par l'utilisateur.

### Notes & contraintes
- Les "plantes manquantes" sont calculees cote client a partir des donnees d'associations deja chargees — pas de nouvel endpoint.
- Reutiliser l'API et le signal d'associations existants ; ne pas dupliquer la logique de chargement.
- Si aucune association manquante ne repond aux criteres, la section n'est pas affichee (pas d'etat vide superflu).
- Cles de traduction : `Associations.MissingTitle`, `Associations.MissingEmpty`.

### Estimation
- **Priorite :** Should
- **Points :** 5
