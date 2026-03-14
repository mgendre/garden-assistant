## [US-001] Créer un jardin

**En tant que** jardinier,
**je veux** créer un nouveau jardin en lui donnant un nom et une description,
**afin de** pouvoir organiser mes espaces de culture de façon distincte.

### Critères d'acceptation

- [ ] CA1 : Je peux saisir un nom (obligatoire, max 100 caractères) et une description (facultative).
- [ ] CA2 : Le jardin apparaît immédiatement dans la liste de mes jardins après création.
- [ ] CA3 : Deux jardins peuvent avoir le même nom sans erreur.
- [ ] CA4 : Un jardin vide est créé sans planches — je les ajouterai séparément.
- [ ] CA5 : Le jardin est lié à mon compte et invisible pour les autres utilisateurs.

### Notes & contraintes
- Pas de limite sur le nombre de jardins par utilisateur.
- L'API retourne le jardin créé avec son identifiant.

### Estimation
- **Priorité :** Must
- **Points :** 2
