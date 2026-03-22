## [US-114] Restriction du endpoint dev-token a l'environnement de developpement

**En tant que** responsable securite,
**je veux** que le endpoint `GET /api/auth/token` retourne 404 en dehors de l'environnement de developpement,
**afin d'** empecher l'obtention de tokens sans authentification en production.

### Criteres d'acceptation

- [ ] CA1 : Le endpoint `GET /api/auth/token` verifie `IWebHostEnvironment.IsDevelopment()`. Si `false`, retourne 404 (NotFound).
- [ ] CA2 : En environnement de developpement, le comportement existant est inchange (retour de tokens pour l'utilisateur seed).
- [ ] CA3 : Le comportement est verifie par un test d'integration (`GetToken_WhenNotDevelopment_ShouldReturn404`).

### Notes & contraintes
- Changement minimal dans le controller existant.
- Cette story peut etre livree independamment des autres stories OAuth.

### Estimation
- **Priorite :** Indispensable
- **Points :** 1
