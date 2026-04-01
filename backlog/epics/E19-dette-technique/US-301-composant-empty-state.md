## [US-301] Composant `<app-empty-state>` réutilisable

**En tant que** développeur frontend,
**je veux** un composant `EmptyStateComponent` unique pour afficher les états vides et de chargement,
**afin d'** éliminer les 42 occurrences du pattern `empty-state` dupliqué dans 11 fichiers.

### Critères d'acceptation

- [ ] CA1 : Un composant `<app-empty-state>` est créé dans `shared/components/`.
- [ ] CA2 : Le composant accepte les inputs suivants : `icon` (string, emoji ou icône), `messageKey` (clé i18n, optionnelle), `actionKey` (clé i18n du bouton d'action, optionnelle).
- [ ] CA3 : Le composant émet un output `actionClick` lorsque le bouton d'action est cliqué.
- [ ] CA4 : L'état de chargement est couvert par le même composant en passant `icon="⏳"` sans `messageKey`.
- [ ] CA5 : Toutes les occurrences existantes du pattern `.empty-state` dans les templates sont remplacées par `<app-empty-state>`.
- [ ] CA6 : Le rendu visuel est identique à l'existant — pas de régression UI.
- [ ] CA7 : Le build Angular ne produit aucune erreur (`npm run build --prefix garden-assistant-app`).

### Notes & contraintes
- Utiliser les classes CSS `.empty-state`, `.empty-state-icon`, `.empty-state-text` déjà définies — ne pas créer de nouveaux styles.
- Le texte est rendu via le pipe `translate` — ne jamais passer de texte brut dans `messageKey`.
- `actionKey` et l'output `actionClick` sont optionnels : si absent, le bouton n'est pas rendu.

### Estimation
- **Priorité :** Indispensable
- **Points :** 2
- **Statut :** Terminé

### Livré
- Composant `<app-empty-state>` dans `shared/ui/empty-state/`
- Inputs : `icon`, `titleKey`, `messageKey`, `actionKey`, `linkRoute`, `minHeight`
- Output : `actionClick`
- 42 occurrences remplacées dans 11 templates, 24 fichiers modifiés
