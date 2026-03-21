## [US-095] Creer `en.json` avec toutes les traductions UI en anglais

**En tant que** jardinier anglophone,
**je veux** que toutes les etiquettes, boutons et messages de l'interface soient disponibles en anglais,
**afin de** pouvoir utiliser l'application dans ma langue.

### Criteres d'acceptation

- [ ] CA1 : Un fichier `public/i18n/en.json` est cree avec exactement la meme structure que `fr.json`.
- [ ] CA2 : Toutes les cles presentes dans `fr.json` ont leur equivalent anglais dans `en.json`.
- [ ] CA3 : Les traductions anglaises sont naturelles et idiomatiques (pas de traduction mot-a-mot).
- [ ] CA4 : Les parametres d'interpolation (`{{name}}`, `{{count}}`, `{{status}}`) sont preserves a l'identique.
- [ ] CA5 : Le build frontend passe sans erreur avec `en.json` selectionne comme langue active.

### Notes & contraintes
- Les cles utilisent le meme format PascalCase avec dots que `fr.json`.
- Les emojis dans les valeurs (ensoleillement, arrosage, etc.) sont conserves a l'identique.
- Les descriptions longues (BadgeInfo) doivent etre traduites avec precision botanique.

### Estimation
- **Priorite :** Indispensable
- **Points :** 3
