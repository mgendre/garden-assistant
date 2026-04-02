## [US-320] Endpoint admin pour modifier une plante du catalogue

**En tant qu'** administrateur,
**je veux** pouvoir modifier les proprietes d'une plante du catalogue via l'API,
**afin de** corriger ou enrichir les donnees botaniques sans attendre un nouveau deploiement.

### Criteres d'acceptation

- [ ] CA1 : Un endpoint `PUT /api/admin/plants/{id}` permet de modifier les proprietes d'une plante du catalogue (`UserId == null`).
- [ ] CA2 : Le service positionne automatiquement `IsCustomized = true` sur la plante modifiee.
- [ ] CA3 : L'endpoint est protege par le role `Admin` (attribut `[Authorize(Roles = "Admin")]`).
- [ ] CA4 : Si la plante n'existe pas ou est une variante utilisateur (`UserId != null`), l'endpoint retourne `404`.
- [ ] CA5 : Le DTO de requete valide les champs obligatoires (nom, famille, etc.).
- [ ] CA6 : Les tests unitaires couvrent les cas : modification reussie, plante introuvable, acces non-admin refuse.

### Notes & contraintes
- Depend de US-312 (champ `IsCustomized` sur l'entite).
- Le flag `IsCustomized` est invisible pour l'admin — il est gere automatiquement par le service.

### Estimation
- **Priorite :** Indispensable
- **Points :** 3
- **Statut :** A faire
