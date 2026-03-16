## [US-052] Refactoring styles vers architecture 7-1 Sass

**En tant que** developpeur frontend,
**je veux** que tous les styles partages soient extraits dans l'architecture 7-1 Sass,
**afin de** centraliser les styles reutilisables et reduire la duplication entre composants.

### Criteres d'acceptation

- [x] CA1 : Les styles de liste de plantes (`.panel-header`, `.plant-item`, `.search-wrap`, etc.) sont dans `styles/components/_plant-list.scss`.
- [x] CA2 : Les styles de carte plante (`.plant-card`, `.detail-*`, `.fav-btn-action`, etc.) sont dans `styles/components/_plant-card.scss`.
- [x] CA3 : Les styles du collapsible (`.collapsible-trigger`, `.collapsible-body`, `.chevron`) sont dans `styles/components/_collapsible.scss`.
- [x] CA4 : Les styles des compagnons (`.companion-item`, `.right-scroll`, section headers) sont dans `styles/components/_companion-list.scss`.
- [x] CA5 : Les styles des guildes (`.guild-card`, `.guild-plant-chip`) sont dans `styles/components/_guilds.scss`.
- [x] CA6 : Les styles de page (`.page-container`, `.page-header`) sont dans `styles/base/_page.scss`.
- [x] CA7 : Les styles de layout (`.two-col`, `.three-col`) sont dans `styles/layout/_layout.scss`.
- [x] CA8 : Tous les nouveaux partials sont enregistres dans `main.scss`.
- [x] CA9 : Les fichiers SCSS de composants ne contiennent plus que des variantes specifiques au composant.

### Notes & contraintes
- Les composants utilisent `ViewEncapsulation.None` pour que les styles globaux s'appliquent.
- Utilise `@use`/`@forward` (jamais `@import`).

### Estimation
- **Priorite :** Should
- **Points :** 3
- **Statut :** Done
