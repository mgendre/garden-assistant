## [US-113] Endpoint profil utilisateur : toggle consentement email

**En tant que** jardinier,
**je veux** pouvoir activer ou desactiver le stockage de mon email depuis mon profil,
**afin de** controler mes donnees personnelles.

### Criteres d'acceptation

- [ ] CA1 : `PUT /api/user/profile/email-consent` (Authorize) accepte `{ consentEmail: bool }`.
- [ ] CA2 : Si `consentEmail = false`, le backend met `ConsentEmail = false` et `Email = null` pour l'utilisateur. L'email est efface.
- [ ] CA3 : Si `consentEmail = true`, le backend met `ConsentEmail = true`. L'email n'est pas rempli immediatement — il sera capture au prochain login via un provider OAuth.
- [ ] CA4 : L'endpoint retourne le profil mis a jour (au minimum : `consentEmail`, `email`).
- [ ] CA5 : `GET /api/user/profile` (Authorize) retourne le profil de l'utilisateur connecte, incluant `email`, `consentEmail`, et la liste des providers lies (noms des providers depuis `ExternalLogin`).
- [ ] CA6 : Un service `IUserProfileService` (ou equivalent) encapsule la logique. Le controller reste mince.

### Notes & contraintes
- L'endpoint `GET /api/user/profile` est aussi utile pour le frontend pour afficher les providers lies dans la page profil.
- Pas de suppression de compte dans cette story (hors scope).

### Estimation
- **Priorite :** Indispensable
- **Points :** 3
