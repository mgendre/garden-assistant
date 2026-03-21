## [US-097] Persistance du choix de langue (localStorage)

**En tant que** jardinier,
**je veux** que mon choix de langue soit memorise entre les sessions,
**afin de** ne pas avoir a le re-selectionner a chaque visite.

### Criteres d'acceptation

- [ ] CA1 : Le choix de langue est stocke dans `localStorage` sous la cle `lang`.
- [ ] CA2 : Au demarrage de l'application, la langue stockee est chargee automatiquement.
- [ ] CA3 : Si aucune langue n'est stockee, la langue par defaut (`fr`) est utilisee.
- [ ] CA4 : Le changement de langue via le selecteur (US-096) met a jour `localStorage` immediatement.

### Notes & contraintes
- Pas de persistance cote serveur dans cette iteration — `localStorage` suffit.
- Si la valeur stockee ne correspond a aucune langue supportee, fallback vers `fr`.

### Estimation
- **Priorite :** Indispensable
- **Points :** 1
