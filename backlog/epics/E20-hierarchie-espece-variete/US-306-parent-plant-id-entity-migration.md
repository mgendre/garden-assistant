## [US-306] ParentPlantId sur l'entite Plant + migration EF

**En tant que** developpeur,
**je veux** ajouter une relation parent optionnelle sur l'entite Plant,
**afin de** modeliser le lien espece-variete (ex. Courge -> Courgette).

### Criteres d'acceptation

- [ ] CA1 : L'entite `Plant` possede une propriete nullable `ParentPlantId` (Guid?) avec une navigation `ParentPlant`.
- [ ] CA2 : La configuration EF Fluent API definit la relation self-referencing (one-to-many) avec `DeleteBehavior.Restrict` (supprimer un parent est interdit tant qu'il a des varietes).
- [ ] CA3 : Une migration EF Core est generee et applicable sans erreur sur la base existante.
- [ ] CA4 : La colonne `parent_plant_id` est nullable, avec un index et une foreign key vers `plants.id`.
- [ ] CA5 : Une plante parente (espece) a `ParentPlantId = null`. Une variete pointe vers son espece parente.
- [ ] CA6 : La profondeur est limitee a un seul niveau : une variete ne peut pas etre parente d'une autre variete (validation dans le service ou contrainte metier documentee).

### Notes & contraintes
- Aucune donnee existante n'est modifiee dans cette US — le seed et la migration de donnees sont traites dans US-307 et US-308.
- La navigation inverse `ICollection<Plant> Varieties` est optionnelle mais recommandee pour faciliter les requetes.

### Estimation
- **Priorite :** Indispensable
- **Points :** 3
- **Statut :** A faire
