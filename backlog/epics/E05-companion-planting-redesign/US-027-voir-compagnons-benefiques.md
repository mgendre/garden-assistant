## [US-027] Voir les plantes compagnes benefiques

**En tant que** jardinier,
**je veux** voir la liste des plantes qui s'associent bien avec ma selection,
**afin de** savoir quoi planter a cote pour maximiser les synergies au potager.

### Criteres d'acceptation

- [ ] CA1 : Des qu'au moins 1 plante est selectionnee, une section "Bons compagnons" s'affiche sous la selection.
- [ ] CA2 : La liste affiche les plantes ayant au moins une association benefique avec une ou plusieurs plantes de la selection.
- [ ] CA3 : Chaque resultat affiche le nom de la plante, son nom scientifique, et un score de compatibilite.
- [ ] CA4 : Les resultats sont tries par score decroissant (les meilleurs compagnons en premier).
- [ ] CA5 : La liste est limitee a 10 resultats maximum (les meilleurs).
- [ ] CA6 : Les plantes deja presentes dans la selection n'apparaissent pas dans les resultats.
- [ ] CA7 : Un indicateur de chargement s'affiche pendant le calcul serveur.
- [ ] CA8 : Si aucun compagnon benefique n'est trouve, un message "Aucun compagnon benefique connu" s'affiche.

### Notes & contraintes
- Le backend gere entierement le calcul de score et le tri. Le frontend se contente d'afficher la reponse.
- L'endpoint `POST /api/plants/companions` existant retourne deja des recommandations avec score ; il sera reutilise ou adapte.

### Estimation
- **Priorite :** Must
- **Points :** 3
