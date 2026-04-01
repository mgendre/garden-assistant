## [US-302] Composant `<app-toggle-group>` réutilisable

**En tant que** développeur frontend,
**je veux** un composant `ToggleGroupComponent` pour les sélecteurs de type bouton-radio inline,
**afin d'** éviter la duplication du pattern `inline-flex / border / btn-xs` présent dans 3 fichiers (calendrier, garden-calendar).

### Critères d'acceptation

- [ ] CA1 : Un composant `<app-toggle-group>` est créé dans `shared/components/`.
- [ ] CA2 : Le composant accepte un input `options` : tableau d'objets `{ value: string; labelKey: string; icon?: string }`.
- [ ] CA3 : Le composant accepte un input `selectedValue` (valeur active courante).
- [ ] CA4 : Le composant émet un output `valueChange` avec la nouvelle valeur sélectionnée au clic.
- [ ] CA5 : Le style actif/inactif reproduit fidèlement le comportement existant (`btn-primary` actif, `btn-ghost` inactif).
- [ ] CA6 : Les 5 occurrences existantes du pattern toggle inline sont remplacées par `<app-toggle-group>`.
- [ ] CA7 : Le build Angular ne produit aucune erreur (`npm run build --prefix garden-assistant-app`).

### Notes & contraintes
- Ne créer aucun style CSS spécifique au composant — réutiliser exclusivement les classes `.btn`, `.btn-xs`, `.btn-primary`, `.btn-ghost` existantes.
- Le composant est générique : il ne connaît pas le domaine (calendrier, source, groupement). La logique métier reste dans le composant parent.
- Compatible mobile : le groupe doit s'adapter à des viewports `≥ 320px`.

### Estimation
- **Priorité :** Important
- **Points :** 2
- **Statut :** A faire
