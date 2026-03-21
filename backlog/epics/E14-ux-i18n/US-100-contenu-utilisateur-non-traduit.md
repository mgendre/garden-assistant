## [US-100] Gestion du contenu utilisateur non traduit (guildes personnalisees, notes)

**En tant que** jardinier,
**je veux** que mes guildes personnalisees et mes notes s'affichent telles que je les ai saisies, quelle que soit la langue selectionnee,
**afin de** ne pas perdre ou alterer mon contenu personnel.

### Criteres d'acceptation

- [ ] CA1 : Les guildes creees par l'utilisateur affichent toujours leur nom et description bruts, sans tentative de traduction.
- [ ] CA2 : Les notes sur les associations et les plantations s'affichent telles que saisies.
- [ ] CA3 : L'interface ne montre pas d'indicateur de "traduction manquante" pour le contenu utilisateur.
- [ ] CA4 : Le formulaire de creation/edition de guilde ne propose pas de champ de langue.

### Notes & contraintes
- Le contenu utilisateur est identifie par la presence d'un `UserId` sur l'entite (guildes personnalisees) ou par la nature du champ (notes).
- Le `TranslationService` backend ne cherche pas de traduction pour les entites utilisateur.
- Dans une iteration future, on pourrait permettre aux utilisateurs de saisir du contenu multilingue, mais c'est hors scope.

### Estimation
- **Priorite :** Important
- **Points :** 2
