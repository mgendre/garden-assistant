## [US-120] Tests d'integration OAuth

**En tant que** developpeur,
**je veux** des tests d'integration validant les endpoints OAuth de bout en bout,
**afin de** detecter les regressions sur le flux d'authentification complet.

### Criteres d'acceptation

- [ ] CA1 : `OAuthComplete_WhenValidCode_ShouldReturn200WithTokens` — verifie que `POST /api/auth/complete` avec un code valide retourne un access token et un refresh token.
- [ ] CA2 : `OAuthComplete_WhenInvalidCode_ShouldReturn401` — verifie que `POST /api/auth/complete` avec un code invalide retourne 401.
- [ ] CA3 : `OAuthLogin_WhenUnknownProvider_ShouldReturn400` — verifie que `GET /api/auth/oauth/unknown/login` retourne 400.
- [ ] CA4 : `GetToken_WhenNotDevelopment_ShouldReturn404` — verifie que le dev-token endpoint retourne 404 en environnement non-dev.
- [ ] CA5 : `UpdateEmailConsent_WhenToggleOff_ShouldClearEmail` — verifie que `PUT /api/user/profile/email-consent` avec `consentEmail=false` supprime l'email.
- [ ] CA6 : Les tests utilisent `WebApplicationFactory` avec une base de test reelle.
- [ ] CA7 : Tous les tests passent (`dotnet test garden-assistant-tests`).

### Notes & contraintes
- Les tests d'integration ne testent pas le redirect OAuth reel (impossible sans provider). Ils testent les endpoints `complete`, `login` (validation provider), et `email-consent`.
- Pour le test du code a usage unique, le test injecte un code dans le cache avant d'appeler `complete`.

### Estimation
- **Priorite :** Indispensable
- **Points :** 5
