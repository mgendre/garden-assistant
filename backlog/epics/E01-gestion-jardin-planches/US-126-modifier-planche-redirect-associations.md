## [US-126] Modifier les plantes d'une planche via la page associations

**En tant que** jardinier,
**je veux** cliquer sur "Modifier" dans une planche pour etre redirige vers la page associations avec les plantes de ma planche deja chargees,
**afin de** beneficier de l'editeur complet (catalogue, filtres, mecanismes) pour ajuster ma planche.

### Criteres d'acceptation

- [ ] CA1 : Un bouton "Modifier" est visible en bas du contenu de chaque planche ouverte.
- [ ] CA2 : Le clic navigue vers `/companions?guild={bedGuildId}&returnTo=/garden`.
- [ ] CA3 : La page associations charge automatiquement les plantes de la planche.
- [ ] CA4 : Apres sauvegarde sur la page associations, l'utilisateur est redirige vers la vue jardin.
- [ ] CA5 : Les modifications sont refletees immediatement dans la vue jardin au retour.

### Notes & contraintes
- Meme pattern que la page guildes qui redirige deja vers `/companions?guild=ID`.
- Le parametre `returnTo` permet le retour automatique apres sauvegarde.
- Chaque planche est liee a une guilde (relation bed → guild).

### Estimation
- **Priorite :** Indispensable
- **Points :** 3
