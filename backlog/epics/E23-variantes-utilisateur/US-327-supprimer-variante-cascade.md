## [US-327] Supprimer une variante utilisateur (cascade)

**En tant que** jardinier,
**je veux** supprimer une variante personnelle que je n'utilise plus,
**afin de** garder ma liste de plantes propre et pertinente.

### Criteres d'acceptation

- [ ] CA1 : Un endpoint `DELETE /api/plants/{id}` supprime une variante appartenant a l'utilisateur courant.
- [ ] CA2 : La suppression cascade vers les entites dependantes : plantings, planting entries, guild memberships.
- [ ] CA3 : Seul le proprietaire de la variante peut la supprimer. Retour `403` sinon.
- [ ] CA4 : La suppression d'une plante du catalogue (`UserId == null`) via cet endpoint est interdite. Retour `403`.
- [ ] CA5 : Un dialog de confirmation cote frontend est recommande (hors scope — US frontend separee).
- [ ] CA6 : Les tests unitaires couvrent : suppression reussie avec cascade, acces refuse, suppression du catalogue refusee.

### Notes & contraintes
- Depend de US-324 (creation de variantes).
- La cascade est geree par EF Core via `DeleteBehavior.Cascade` sur les relations dependantes.
- La suppression est destructive et irreversible — les plantations utilisant cette variante seront supprimees.

### Estimation
- **Priorite :** Important
- **Points :** 3
- **Statut :** A faire
