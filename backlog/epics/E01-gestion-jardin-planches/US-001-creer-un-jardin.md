## [US-001] Créer un jardin

**En tant que** jardinier,
**je veux** créer un nouveau jardin en lui donnant un nom et une description,
**afin de** pouvoir organiser mes espaces de culture de façon distincte.

### Critères d'acceptation

- [x] CA1 : Je peux saisir un nom (obligatoire, max 100 caractères) et une description (facultative).
- [x] CA2 : Le jardin apparaît immédiatement dans la liste de mes jardins après création.
- [x] CA3 : Deux jardins peuvent avoir le même nom sans erreur.
- [x] CA4 : Un jardin vide est créé sans planches — je les ajouterai séparément.
- [x] CA5 : Le jardin est lié à mon compte et invisible pour les autres utilisateurs.

### Notes & contraintes
- Pas de limite sur le nombre de jardins par utilisateur.
- L'API retourne le jardin créé avec son identifiant.
- Backend : `GardenService` + `GardensController` (CRUD complet `/api/gardens`). Entité `Garden` avec `CreatedAtUtc`.
- Frontend : dialog de création, mise à jour immédiate de la liste.

### Estimation
- **Priorité :** Must
- **Points :** 2
- **Statut :** Terminé
