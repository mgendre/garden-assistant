## [US-117] Auth guard frontend et adaptation du flux de demarrage

**En tant que** jardinier,
**je veux** etre redirige vers la page de login si je ne suis pas connecte,
**afin que** mes donnees soient protegees.

### Criteres d'acceptation

- [ ] CA1 : Un `authGuard` (functional guard Angular) est cree. Il verifie la presence d'un access token valide dans l'auth service. Si absent, redirige vers `/login`.
- [ ] CA2 : Le guard est applique a toutes les routes sauf `/login` et `/auth/*`.
- [ ] CA3 : En mode developpement (`environment.production === false`), le guard est bypasse car `initialize()` recupere automatiquement les tokens via le dev-token endpoint.
- [ ] CA4 : La methode `initialize()` de l'auth service est adaptee : en mode production, si aucun token n'est present, elle ne fait rien (pas d'erreur). Le guard se charge de la redirection.
- [ ] CA5 : `startupService.loadAll()` est deplace : en mode production, il est appele uniquement apres un login reussi (depuis `AuthCallbackComponent` ou apres un refresh token reussi), jamais au demarrage.
- [ ] CA6 : En mode developpement, le flux actuel est preserve : `initialize()` recupere les tokens puis `startupService.loadAll()` charge les donnees.
- [ ] CA7 : L'application compile et fonctionne correctement en mode developpement (aucune regression).

### Notes & contraintes
- Le guard utilise `inject()` pour acceder a l'auth service et au router.
- Ne pas casser le flux de developpement existant — les developpeurs doivent pouvoir continuer a travailler sans configurer OAuth.
- Le refresh token flow existant doit continuer a fonctionner : si un refresh reussit, `loadAll()` doit etre appele si ce n'est pas deja fait.

### Estimation
- **Priorite :** Indispensable
- **Points :** 3
