## [US-096] Selecteur de langue dans le shell (header)

**En tant que** jardinier,
**je veux** pouvoir changer la langue de l'application depuis le header,
**afin de** basculer entre francais et anglais facilement.

### Criteres d'acceptation

- [ ] CA1 : Un selecteur de langue est visible dans le header (shell) sur toutes les pages.
- [ ] CA2 : Le selecteur affiche les langues disponibles (FR / EN).
- [ ] CA3 : Au clic, la langue de l'application change immediatement (sans rechargement de page).
- [ ] CA4 : Le changement de langue met a jour toutes les etiquettes UI via ngx-translate (`TranslateService.use()`).
- [ ] CA5 : Le selecteur est responsive et fonctionne sur mobile (>= 320px).
- [ ] CA6 : La langue active est visuellement distinguee dans le selecteur.

### Notes & contraintes
- Design mobile-first — le selecteur doit s'integrer au header existant sans casser le layout.
- Le changement de langue declenche aussi un rechargement des donnees API (pour obtenir les traductions DB).
- L'UX designer est consulte pour le placement et le style du selecteur.

### Estimation
- **Priorite :** Indispensable
- **Points :** 3
