## [US-053] Limiter le catalogue API a 20 resultats

**En tant que** developpeur,
**je veux** que l'endpoint `GET /api/plants` retourne un maximum de 20 plantes,
**afin de** garantir des temps de reponse rapides et une interface fluide.

### Criteres d'acceptation

- [x] CA1 : `PlantService.GetAllAsync()` applique `.OrderBy(p => p.Name).Take(20)` sur la requete.
- [x] CA2 : Les plantes sont triees par nom alphabetique avant la limitation.

### Notes & contraintes
- Pas de pagination pour l'instant (YAGNI). A revoir si le catalogue depasse significativement 20 plantes.
- La recherche et le filtrage restent cote client sur les 20 plantes chargees.

### Estimation
- **Priorite :** Must
- **Points :** 1
- **Statut :** Done
