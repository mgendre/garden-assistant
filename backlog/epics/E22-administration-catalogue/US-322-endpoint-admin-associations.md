## [US-322] Endpoint admin pour modifier les associations d'une plante

**En tant qu'** administrateur,
**je veux** pouvoir ajouter, modifier et supprimer les associations d'une plante du catalogue,
**afin de** corriger les relations entre plantes sans attendre un nouveau deploiement.

### Criteres d'acceptation

- [ ] CA1 : Des endpoints CRUD sous `/api/admin/plants/{id}/associations` permettent de gerer les associations d'une plante du catalogue.
- [ ] CA2 : Toute modification d'association positionne automatiquement `IsCustomized = true` sur la plante source.
- [ ] CA3 : Les endpoints sont proteges par le role `Admin`.
- [ ] CA4 : Si la plante source n'est pas une plante du catalogue (`UserId != null`), retour `404`.
- [ ] CA5 : Les tests unitaires couvrent les cas : ajout, modification, suppression, plante introuvable, acces non-admin refuse.

### Notes & contraintes
- Depend de US-320 (meme couche admin, meme pattern `IsCustomized`).
- Le flag `IsCustomized` sur la plante source protege toutes ses associations du seed upsert.

### Estimation
- **Priorite :** Important
- **Points :** 3
- **Statut :** A faire
