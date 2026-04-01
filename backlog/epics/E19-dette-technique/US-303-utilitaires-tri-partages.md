## [US-303] Extraire les utilitaires de tri partagés

**En tant que** développeur frontend,
**je veux** des fonctions de tri centralisées dans un module utilitaire partagé,
**afin d'** éliminer les 16 occurrences dupliquées de `sortByNameFr` et les 3 copies de `sortCalendarEntries` dans l'ensemble du codebase.

### Critères d'acceptation

- [ ] CA1 : Un fichier `shared/utils/sort.utils.ts` est créé et exporte les fonctions : `sortByNameFr<T extends { nameFr?: string }>(items: T[]): T[]` et `sortCalendarEntries(entries: CalendarEntry[]): CalendarEntry[]`.
- [ ] CA2 : Toutes les implémentations locales de `sortByNameFr` dans les composants et services sont supprimées et remplacées par l'import depuis `sort.utils.ts`.
- [ ] CA3 : Toutes les implémentations locales de `sortCalendarEntries` sont supprimées et remplacées par l'import depuis `sort.utils.ts`.
- [ ] CA4 : Aucune régression fonctionnelle sur le tri du catalogue, des guildes, des planches et du calendrier.
- [ ] CA5 : Le build Angular ne produit aucune erreur (`npm run build --prefix garden-assistant-app`).

### Notes & contraintes
- Les fonctions sont pures (sans effet de bord) et n'injectent aucun service Angular.
- Ne pas créer de service Angular pour du tri stateless — des fonctions exportées suffisent (YAGNI).
- Si des variantes de tri existent (ex. tri par famille, tri par date), ne les extraire que si elles sont dupliquées au moins deux fois.

### Estimation
- **Priorité :** Important
- **Points :** 1
- **Statut :** Terminé

### Livré
- `getEarliestHalfMonth()` extrait dans `shared/constants/plant-action.constants.ts` — remplace 4 copies privées (bed-panel, garden-calendar, guild-editor, calendar.store)
- `sortMechanisms()` extrait comme méthode privée dans `CompanionStore` — remplace 5 copies inline
- `sortByNameFr` non extrait en utilitaire car trop simple (1 ligne), laissé inline
