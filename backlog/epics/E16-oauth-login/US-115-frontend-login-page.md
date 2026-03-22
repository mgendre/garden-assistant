## [US-115] Page de login frontend

**En tant que** jardinier,
**je veux** voir une page de connexion avec des boutons Google et Discord,
**afin de** choisir mon mode d'authentification.

### Criteres d'acceptation

- [ ] CA1 : Une nouvelle route `/login` est creee avec un composant `LoginComponent`.
- [ ] CA2 : La page affiche deux boutons : "Se connecter avec Google" et "Se connecter avec Discord", chacun avec le logo du provider.
- [ ] CA3 : Un clic sur un bouton navigue vers `GET /api/auth/oauth/{provider}/login` (navigation pleine page, pas un appel XHR — le navigateur suit la redirection OAuth).
- [ ] CA4 : Le design est mobile-first, centre verticalement et horizontalement. Le branding Garden Assistant (logo, nom) est visible au-dessus des boutons.
- [ ] CA5 : Si un query param `error` est present (ex: `/login?error=expired`), un message d'erreur traduit est affiche au-dessus des boutons.
- [ ] CA6 : Toutes les chaines sont traduites via ngx-translate (cles `Login.*`).
- [ ] CA7 : La page compile sans erreur (`npm run build --prefix garden-assistant-app`).

### Notes & contraintes
- Le design final est defini par le UX designer. La story definit le comportement, pas les details visuels.
- La route `/login` ne necessite pas d'authentification (pas de guard).
- En mode developpement, cette page n'est pas affichee (le flux dev-token existant pre-authentifie).

### Estimation
- **Priorite :** Indispensable
- **Points :** 3
