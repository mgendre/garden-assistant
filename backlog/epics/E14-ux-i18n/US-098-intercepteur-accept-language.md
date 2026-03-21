## [US-098] Envoyer `Accept-Language` dans tous les appels API

**En tant que** developpeur frontend,
**je veux** qu'un intercepteur HTTP ajoute automatiquement le header `Accept-Language` a tous les appels API,
**afin de** recevoir les donnees traduites sans intervention manuelle dans chaque service.

### Criteres d'acceptation

- [ ] CA1 : Un intercepteur Angular HTTP (`HttpInterceptorFn`) est enregistre dans la configuration de l'application.
- [ ] CA2 : L'intercepteur ajoute le header `Accept-Language` avec la langue active (lue depuis le `TranslateService` ou `localStorage`).
- [ ] CA3 : Tous les appels vers l'API backend incluent le header.
- [ ] CA4 : Le header est mis a jour dynamiquement quand la langue change (sans redemarrage).

### Notes & contraintes
- Utiliser un `HttpInterceptorFn` (fonctionnel, pas une classe — convention Angular moderne).
- Le header suit le format standard HTTP (ex. `fr`, `en`).

### Estimation
- **Priorite :** Indispensable
- **Points :** 2
