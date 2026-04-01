## [US-126] Modifier les plantes d'une planche via la page associations

**En tant que** jardinier,
**je veux** cliquer sur "Modifier les plantes" dans une planche pour etre redirige vers la page associations avec les plantes de ma planche deja chargees,
**afin de** beneficier de l'editeur complet (catalogue, filtres, mecanismes) pour ajuster ma planche.

### Criteres d'acceptation

- [x] CA1 : Un bouton "Modifier les plantes" est visible dans la section d'actions de chaque planche.
- [x] CA2 : Le clic navigue vers `/companions` avec les params `bedName` et `returnTo=/garden/{gardenId}`.
- [x] CA3 : La page associations charge automatiquement les plantes de la planche et vide la selection precedente.
- [x] CA4 : Une banniere d'information bleue affiche « Modification de la planche « X » » sur la page associations.
- [x] CA5 : Des boutons "Annuler", "Retour a la planche" et "Enregistrer" remplacent le bouton de sauvegarde habituel. Apres enregistrement ou annulation, l'utilisateur est redirige vers la vue jardin.

### Notes & contraintes
- La navigation utilise les query params `bedName` et `returnTo` pour contextualiser la page associations.
- La selection est videe a l'entree pour partir de zero avec les plantes de la planche.
- Le retour se fait vers `/garden/{gardenId}` (URL preservee via le param `returnTo`).

### Estimation
- **Priorite :** Indispensable
- **Points :** 3
- **Statut :** Terminé
