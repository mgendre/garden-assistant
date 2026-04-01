## [US-304] Composant `<app-info-banner>` réutilisable

**En tant que** développeur frontend,
**je veux** un composant `InfoBannerComponent` pour les bannières d'information contextuelles,
**afin d'** éliminer les 3 occurrences du bloc `background: #e3f2fd; border: #90caf9; color: #1565c0` dupliqué dans les templates de l'éditeur de guilde et de l'édition de planche.

### Critères d'acceptation

- [ ] CA1 : Un composant `<app-info-banner>` est créé dans `shared/components/`.
- [ ] CA2 : Le composant accepte les inputs suivants : `emoji` (string, optionnel), `messageKey` (clé i18n du message principal), `actionLabel` (texte du bouton d'action, optionnel), `variant` (union type `'info' | 'warning'`, défaut `'info'`).
- [ ] CA3 : Le composant émet un output `actionClick` si `actionLabel` est fourni.
- [ ] CA4 : La variante `info` utilise les variables CSS `--color-info`, `--color-info-bg`, `--color-info-border` — aucune couleur hexadécimale codée en dur.
- [ ] CA5 : La variante `warning` utilise les variables CSS `--color-warning`, `--color-warning-bg`, `--color-warning-border`.
- [ ] CA6 : Les 3 bannières inline dans `guild-editor.html` et `bed-panel.html` sont remplacées par `<app-info-banner>`.
- [ ] CA7 : Le build Angular ne produit aucune erreur (`npm run build --prefix garden-assistant-app`).

### Notes & contraintes
- Les couleurs sont obligatoirement lues depuis les variables CSS du thème (`_variables.scss`) — jamais de valeurs hexadécimales dans le composant ou son style.
- Le composant suit le pattern `.panel` pour la structure — ne pas créer de classes CSS supplémentaires si les utilitaires Tailwind existants suffisent.

### Estimation
- **Priorité :** Optionnel
- **Points :** 1
- **Statut :** A faire
