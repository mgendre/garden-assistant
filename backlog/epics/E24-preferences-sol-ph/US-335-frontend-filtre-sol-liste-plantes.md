## [US-335] Filtrer les plantes par type de sol dans la liste

**En tant que** jardinier,
**je veux** filtrer la liste des plantes par type de sol,
**afin de** trouver rapidement les plantes adaptées à mon terrain.

### Critères d'acceptation

- [x] CA1 : La page de liste des plantes affiche un filtre "Type de sol" (dropdown ou chips) avec les 7 valeurs de l'enum + "Tous".
- [x] CA2 : La sélection d'un type de sol filtre instantanément la liste (côté client, pas d'appel API supplémentaire).
- [x] CA3 : Le filtre est combinable avec les filtres existants (recherche textuelle, famille, etc.).
- [x] CA4 : Les plantes sans type de sol renseigné (liste vide) sont masquées quand un filtre sol est actif.
- [x] CA4bis : Une plante qui a plusieurs types de sol apparaît quand le filtre correspond à l'un quelconque de ses types (OR, pas AND).
- [x] CA5 : Le label du filtre est traduit via ngx-translate.
- [x] CA6 : L'affichage du filtre est responsive (mobile-first) et cohérent avec les filtres existants.
- [x] CA7 : `npm run build --prefix garden-assistant-app` compile sans erreur.

### Notes & contraintes
- Le filtre est purement client-side car la liste complète des plantes est déjà chargée.
- Reprendre le pattern des filtres existants sur la page.

### Estimation
- **Priorite :** Important
- **Points :** 3
- **Statut :** Termine
