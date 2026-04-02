## [US-326] Modifier une variante utilisateur

**En tant que** jardinier,
**je veux** modifier les proprietes de ma variante personnelle,
**afin de** affiner les donnees au fil de mes observations (espacement reel, dates de semis ajustees, etc.).

### Criteres d'acceptation

- [ ] CA1 : Un endpoint `PUT /api/plants/{id}` permet de modifier une variante appartenant a l'utilisateur courant.
- [ ] CA2 : Seul le proprietaire de la variante peut la modifier. Retour `403` si l'utilisateur n'est pas le proprietaire.
- [ ] CA3 : La modification d'une plante du catalogue (`UserId == null`) via cet endpoint est interdite. Retour `403`.
- [ ] CA4 : Le champ `ParentPlantId` ne peut pas etre modifie (immutable apres creation).
- [ ] CA5 : Les tests unitaires couvrent : modification reussie, acces refuse (autre utilisateur), modification du catalogue refusee.

### Notes & contraintes
- Depend de US-324 (creation de variantes).
- `IsCustomized` reste `true` en permanence pour les variantes — pas de changement de statut possible.

### Estimation
- **Priorite :** Indispensable
- **Points :** 3
- **Statut :** A faire
