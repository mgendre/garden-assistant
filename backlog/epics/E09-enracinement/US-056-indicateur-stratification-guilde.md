## [US-056] Indicateur de stratification racinaire dans l'editeur de guilde

**En tant que** jardinier,
**je veux** visualiser la repartition des plantes par profondeur racinaire dans l'editeur de guilde,
**afin de** verifier que mes plantes occupent des zones racinaires complementaires et non concurrentes.

### Criteres d'acceptation

- [x] CA1 : Lorsque au moins 2 plantes sont selectionnees, une section repliable "Stratification racinaire" apparait dans l'editeur de guilde.
- [x] CA2 : Les plantes sont reparties dans 3 colonnes (Superficiel 15-30 cm, Moyen 30-60 cm, Profond 60 cm+) avec un fond en degrade visuel.
- [x] CA3 : Un avertissement de densite apparait lorsque plus de 3 plantes partagent la meme zone racinaire.
- [x] CA4 : Un bouton filtre par colonne permet de filtrer le catalogue par profondeur racinaire directement depuis la vue stratification.

### Notes & contraintes
- Approche finale : 3 colonnes cote a cote (au lieu de la coupe transversale SVG envisagee initialement). Plus lisible et plus simple.
- Composant `app-root-stratification` separe, reutilisable.
- Donnees `rootDepth` issues du signal de selection existant -- pas de nouvel appel API.

### Estimation
- **Priorite :** Should
- **Points :** 3
- **Statut :** Done
