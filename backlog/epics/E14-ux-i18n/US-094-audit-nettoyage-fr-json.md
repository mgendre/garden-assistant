## [US-094] Audit et nettoyage de `fr.json`

**En tant que** developpeur,
**je veux** que le fichier `fr.json` soit nettoye des cles inutilisees et que les incoherences soient corrigees,
**afin de** maintenir un fichier de traduction propre et fiable.

### Criteres d'acceptation

- [ ] CA1 : Toutes les cles presentes dans `fr.json` mais non utilisees dans aucun template (`.html`) ni composant (`.ts`) sont identifiees.
- [ ] CA2 : Les cles correspondant a des fonctionnalites non implementees (Dashboard, Garden, Tasks) sont conservees en l'etat pour les epics futures.
- [ ] CA3 : Les valeurs contenant des incoherences (accents manquants, casse incorrecte, guillemets inconsistants) sont corrigees.
- [ ] CA4 : Le build frontend (`npm run build --prefix garden-assistant-app`) passe sans erreur apres le nettoyage.

### Notes & contraintes
- Ne pas supprimer les cles de fonctionnalites planifiees (Dashboard, Garden, Tasks) — les garder pour les epics futures.
- Verifier avec `grep` que chaque cle est referencee quelque part avant de la supprimer.

### Estimation
- **Priorite :** Important
- **Points :** 2
