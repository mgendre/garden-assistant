## [US-300] DialogService — Centraliser l'ouverture des dialogs

**En tant que** développeur frontend,
**je veux** un `DialogService` avec des méthodes typées pour ouvrir chaque type de dialog de l'application,
**afin d'** éliminer les 30+ appels dupliqués à `MatDialog.open` dispersés dans 12+ fichiers.

### Critères d'acceptation

- [ ] CA1 : Un `DialogService` (avec interface `IDialogService`) est créé dans `shared/services/`.
- [ ] CA2 : Le service expose les méthodes typées suivantes : `openPlantDetail(plant)`, `openBadgeInfo(badge)`, `openHarvestReadiness(plantId, plantName)` (avec fallback si données absentes), `confirm(messageKey, options?)`.
- [ ] CA3 : `openHarvestReadiness` gère le cas où les données de maturité ne sont pas disponibles et affiche un état de fallback plutôt que de lancer une erreur.
- [ ] CA4 : Chaque composant qui appelait directement `MatDialog.open` délègue désormais au `DialogService`.
- [ ] CA5 : Aucun appel direct à `MatDialog.open` ne subsiste dans les composants métier.
- [ ] CA6 : Le build Angular ne produit aucune erreur (`npm run build --prefix garden-assistant-app`).

### Notes & contraintes
- Le `DialogService` est injecté via son interface — les composants ne dépendent pas de la classe concrète.
- Les types des données passées à chaque dialog doivent correspondre aux DTOs existants, sans en créer de nouveaux.
- Environ 100 lignes de boilerplate éliminées à l'issue de cette story.
- Ne pas modifier le comportement visible des dialogs — refactoring pur.

### Estimation
- **Priorité :** Indispensable
- **Points :** 3
- **Statut :** A faire
