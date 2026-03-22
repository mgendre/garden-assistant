## [US-116] Callback OAuth frontend et ecran de consentement email

**En tant que** jardinier qui vient de s'authentifier via Google ou Discord,
**je veux** que la connexion se finalise automatiquement (ou que je puisse choisir le stockage de mon email si c'est ma premiere visite),
**afin d'** acceder a mon jardin sans friction.

### Criteres d'acceptation

- [ ] CA1 : Une nouvelle route `/auth/callback` est creee avec un composant `AuthCallbackComponent`.
- [ ] CA2 : Le composant lit les query params `code` et `isNew` depuis l'URL.
- [ ] CA3 : Si `isNew=false`, le composant appelle immediatement `POST /api/auth/complete` avec `{ code, storeEmail: true }` et, en cas de succes, stocke les tokens puis navigue vers `/companions`.
- [ ] CA4 : Si `isNew=true`, le composant affiche un ecran de consentement email : checkbox pre-cochee "Stocker mon email pour activer les notifications et lier mes comptes entre providers", avec une explication courte sous la checkbox.
- [ ] CA5 : L'ecran de consentement affiche un bouton "Continuer" qui appelle `POST /api/auth/complete` avec `{ code, storeEmail }` correspondant a l'etat de la checkbox.
- [ ] CA6 : En cas de succes, les tokens sont stockes dans l'auth service, `startupService.loadAll()` est appele, puis navigation vers `/companions`.
- [ ] CA7 : En cas d'erreur (code invalide, expire), navigation vers `/login?error=callback-failed`.
- [ ] CA8 : Un spinner est affiche pendant les appels API.
- [ ] CA9 : Toutes les chaines sont traduites (cles `AuthCallback.*`).
- [ ] CA10 : Le composant compile sans erreur.

### Notes & contraintes
- La route `/auth/callback` ne necessite pas d'authentification (pas de guard).
- Le `completeOAuthLogin` dans l'auth service encapsule l'appel API + stockage tokens + chargement startup.

### Estimation
- **Priorite :** Indispensable
- **Points :** 5
