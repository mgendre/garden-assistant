## [US-335] Filtrer les plantes par type de sol dans la liste

**En tant que** jardinier,
**je veux** filtrer la liste des plantes par type de sol,
**afin de** trouver rapidement les plantes adaptees a mon terrain.

### Criteres d'acceptation

- [ ] CA1 : La page de liste des plantes affiche un filtre "Type de sol" (dropdown ou chips) avec les 7 valeurs de l'enum + "Tous".
- [ ] CA2 : La selection d'un type de sol filtre instantanement la liste (cote client, pas d'appel API supplementaire).
- [ ] CA3 : Le filtre est combinable avec les filtres existants (recherche textuelle, famille, etc.).
- [ ] CA4 : Les plantes sans type de sol renseigne (liste vide) sont masquees quand un filtre sol est actif.
- [ ] CA4bis : Une plante qui a plusieurs types de sol apparait quand le filtre correspond a l'un quelconque de ses types (OR, pas AND).
- [ ] CA5 : Le label du filtre est traduit via ngx-translate.
- [ ] CA6 : L'affichage du filtre est responsive (mobile-first) et coherent avec les filtres existants.
- [ ] CA7 : `npm run build --prefix garden-assistant-app` compile sans erreur.

### Notes & contraintes
- Le filtre est purement client-side car la liste complete des plantes est deja chargee.
- Reprendre le pattern des filtres existants sur la page.

### Estimation
- **Priorite :** Important
- **Points :** 3
- **Statut :** A faire
