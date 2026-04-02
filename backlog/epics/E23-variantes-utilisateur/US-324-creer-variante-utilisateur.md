## [US-324] Service et endpoints pour creer une variante utilisateur

**En tant que** jardinier,
**je veux** creer une variante personnelle a partir d'une plante du catalogue,
**afin de** adapter les proprietes (dates, espacement, besoins) a ma realite locale sans modifier le catalogue partage.

### Criteres d'acceptation

- [ ] CA1 : Un endpoint `POST /api/plants/{id}/variants` cree une variante a partir d'une plante du catalogue.
- [ ] CA2 : La variante creee a `ParentPlantId` = ID de la plante source, `UserId` = utilisateur courant, `IsCustomized = true`.
- [ ] CA3 : Les champs de la plante parente sont copies dans la variante. L'utilisateur peut surcharger les champs dans le body de la requete.
- [ ] CA4 : Si la plante source n'est pas une plante du catalogue (`UserId != null`), retour `400` — on ne cree pas une variante d'une variante.
- [ ] CA5 : La variante recoit un `Key` derive du parent (ex. `tomate-cerise--user-{userId-short}`).
- [ ] CA6 : Les tests unitaires couvrent les cas : creation reussie, plante source introuvable, creation depuis une variante refusee.

### Notes & contraintes
- Depend de US-312 (champs `Key`, `IsCustomized`, `UserId` sur Plant).
- La profondeur est limitee a un niveau : une variante ne peut pas etre parente d'une autre variante (contrainte existante de E20).
- Les associations sont heritees du parent via le service existant (US-310) — pas de copie explicite.

### Estimation
- **Priorite :** Indispensable
- **Points :** 5
- **Statut :** A faire
