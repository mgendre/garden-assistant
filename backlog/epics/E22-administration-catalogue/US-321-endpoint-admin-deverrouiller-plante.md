## [US-321] Endpoint admin pour deverrouiller une plante

**En tant qu'** administrateur,
**je veux** pouvoir remettre `IsCustomized` a `false` sur une plante du catalogue,
**afin de** la re-soumettre au seed upsert lors du prochain demarrage.

### Criteres d'acceptation

- [ ] CA1 : Un endpoint `PATCH /api/admin/plants/{id}/unlock` met `IsCustomized = false` sur la plante ciblee.
- [ ] CA2 : L'endpoint est protege par le role `Admin`.
- [ ] CA3 : Seules les plantes du catalogue (`UserId == null`) peuvent etre deverrouillees. Retour `404` sinon.
- [ ] CA4 : Au prochain demarrage de l'application, le seed upsert met a jour la plante deverrouillee avec les donnees JSON.
- [ ] CA5 : Les tests unitaires couvrent les cas : deverrouillage reussi, plante introuvable, variante utilisateur refusee.

### Notes & contraintes
- Depend de US-320 (meme couche admin).
- L'operation est potentiellement destructive : les modifications manuelles seront ecrasees au prochain seed. Documenter ce comportement dans l'API.

### Estimation
- **Priorite :** Important
- **Points :** 2
- **Statut :** A faire
