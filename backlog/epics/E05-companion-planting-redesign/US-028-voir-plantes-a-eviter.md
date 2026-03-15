## [US-028] Voir les plantes a eviter

**En tant que** jardinier,
**je veux** voir les plantes incompatibles avec ma selection,
**afin d'** eviter de planter des voisins qui se nuiraient mutuellement.

### Criteres d'acceptation

- [ ] CA1 : Des qu'au moins 1 plante est selectionnee, une section "Plantes a eviter" s'affiche sous la section des bons compagnons.
- [ ] CA2 : La liste affiche les plantes ayant au moins une association nefaste avec une ou plusieurs plantes de la selection.
- [ ] CA3 : Chaque resultat affiche le nom de la plante et la raison de l'incompatibilite (mecanisme en langage clair, ex. "Toxines racinaires").
- [ ] CA4 : La section est visuellement distincte des bons compagnons (couleur d'alerte, bordure rouge ou orange).
- [ ] CA5 : Si aucune plante a eviter n'est trouvee, la section n'est pas affichee du tout (pas de section vide).
- [ ] CA6 : Les plantes deja presentes dans la selection n'apparaissent pas dans cette liste.

### Notes & contraintes
- Le backend retourne la liste des plantes nefastes en une seule reponse avec les benefiques (ou via un endpoint separe). Toute la logique de filtrage est cote serveur.
- Le mecanisme (raison de l'incompatibilite) doit etre traduit en francais dans le frontend via un dictionnaire de labels.

### Estimation
- **Priorite :** Must
- **Points :** 2
