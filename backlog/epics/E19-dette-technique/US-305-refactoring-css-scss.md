## [US-305] Refactoring CSS/SCSS — Variables et factorisation

**En tant que** développeur frontend,
**je veux** que toutes les couleurs et styles répétés dans les fichiers SCSS et les templates utilisent les variables CSS du thème,
**afin d'** éliminer les valeurs hexadécimales codées en dur et les styles inline qui rendent la maintenance du design system difficile.

### Problèmes identifiés

#### 1. Couleurs hexadécimales codées en dur dans les fichiers SCSS

**`_badges.scss`**
- `.badge-water` : `background: #e3f2fd; color: #1565c0` → doit utiliser `var(--color-info-bg)` et `var(--color-info)`
- `.mechanism-blue` : `background: #e3f2fd; color: #1565c0` → idem
- `.badge-sun` : `background: #fff8e1` — aucune variable existante ; créer `--color-warning-tint` dans `_variables.scss`
- `.stat-green`, `.stat-orange`, `.stat-red`, `.mechanism-green`, `.mechanism-red` : couleurs sans variable correspondante — les aligner sur `--color-success`, `--color-danger`, ou créer les variables manquantes

**`_plant-association-panel.scss`**
- `.association-item--harmful` : `border-left: 3px solid #ef5350` → `var(--color-danger-text)` ou nouvelle variable sémantique
- `.association-harmful-icon` : `color: #ef5350` → idem
- `.section-header-warning` : `color: #f57c00` — aucune variable existante ; créer `--color-warning-strong` ou utiliser `--color-warning`

**`_plant-card.scss`**
- `.detail-name` : `color: #1a3a2a` — couleur de marque non tokenisée ; créer `--color-forest-dark` dans `_variables.scss`
- `.header-fav-btn:hover` et `.btn-fav:hover` : `color: #e53935; background: #fff5f5; border-color: rgba(229, 57, 53, 0.2)` → créer `--color-danger-hover-bg: #fff5f5` et utiliser `--color-danger-text` pour la couleur
- `.allelopathic-warning` : `background: #fff8e1; border: 1px solid #ffe0b2; color: #e65100` — palette warning non tokénisée, à aligner avec les variables `--color-warning-*` existantes
- `.remove-btn:hover` : `background: #fce4ec; color: #c62828` → utiliser `--color-danger-bg` et `--color-danger-text`

**`_buttons.scss`**
- `.btn-danger:hover` : `background: #7f1d1d` — créer `--color-danger-dark: #7f1d1d`
- `.btn-fav.saved` et `:hover` : `color: #e53935; background: #fff5f5; border-color: rgba(229, 57, 53, 0.3); background: #fde8e8` — même correction que `_plant-card.scss`

#### 2. Styles inline dans les templates

**`guild-editor.html`** (3 occurrences)
- Lignes 3, 12 : `style="padding: 0.75rem 1.25rem; display: flex; align-items: center; gap: 0.75rem; background: #e3f2fd; border-color: #90caf9; flex-wrap: wrap"` — à remplacer par `<app-info-banner>` (US-304) ou une classe `.info-banner` dans `_panels.scss`
- Ligne 69 : `style="padding: 0.75rem 1.25rem; margin: 0.5rem 1rem; border-radius: 0.5rem; background: #e3f2fd; border: 1px solid #90caf9; color: #1565c0; font-size: 0.8125rem"` — même correction

**`bed-panel.html`** (2 occurrences)
- Ligne 11 : `style="display: flex; flex-wrap: wrap; gap: 6px; margin-top: 6px"` → classes Tailwind `flex flex-wrap gap-1.5 mt-1.5`
- Ligne 45 : `style="border-top: 1px solid var(--color-border); padding: 1rem 1.25rem"` → créer une classe `.panel-footer` dans `_panels.scss`
- Lignes 46–48 : `style="display: flex; align-items: center; ..."` → classes Tailwind

**`calendar.html`** (3 occurrences)
- Ligne 19 : `style="display: flex; align-items: center; flex-wrap: wrap; gap: 0.5rem; margin-bottom: 1.25rem"` → classes Tailwind `flex items-center flex-wrap gap-2 mb-5`
- Ligne 53 : `style="display: inline-flex; border-radius: 0.375rem; border: 1px solid var(--color-border); margin-right: 0.25rem"` → extraire vers une classe `.toggle-group` (ou utiliser `<app-toggle-group>` de US-302)
- Ligne 85 : `style="margin-bottom: 1rem"` → classe Tailwind `mb-4`

#### 3. Définitions dupliquées

- `.btn-primary` et `.btn-success` ont des définitions identiques dans `_buttons.scss` — `.btn-success` doit étendre ou aliaser `.btn-primary`
- `.header-fav-btn` dans `_plant-card.scss` duplique partiellement `.btn-fav` de `_buttons.scss` — à consolider

### Critères d'acceptation

- [ ] CA1 : Aucune valeur hexadécimale codée en dur ne subsiste dans les fichiers SCSS listés ci-dessus — toutes remplacées par des variables CSS du thème ou des nouvelles variables ajoutées dans `_variables.scss`.
- [ ] CA2 : Les nouvelles variables CSS nécessaires (`--color-forest-dark`, `--color-danger-dark`, `--color-danger-hover-bg`, `--color-warning-tint`, `--color-warning-strong`) sont ajoutées dans `abstracts/_variables.scss` sous `@theme`.
- [ ] CA3 : Les styles inline identifiés dans `guild-editor.html`, `bed-panel.html` et `calendar.html` sont remplacés par des classes Tailwind ou des classes SCSS nommées.
- [ ] CA4 : `.btn-success` est supprimé ou redirigé vers `.btn-primary` (comportement identique).
- [ ] CA5 : Le rendu visuel de l'application est identique à l'existant — aucune régression.
- [ ] CA6 : Le build Angular ne produit aucune erreur (`npm run build --prefix garden-assistant-app`).

### Notes & contraintes
- Cette story est un refactoring pur : zéro changement fonctionnel ou visuel perceptible par le jardinier.
- Traiter US-304 (`<app-info-banner>`) avant US-305 pour les bannières inline de `guild-editor.html` — les deux stories peuvent avancer en parallèle si l'équipe est disponible.
- Vérifier le rendu sur mobile (320px) avant de valider chaque correction de style inline.

### Estimation
- **Priorité :** Important
- **Points :** 3
- **Statut :** A faire
