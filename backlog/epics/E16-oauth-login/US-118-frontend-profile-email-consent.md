## [US-118] Toggle consentement email dans la page profil

**En tant que** jardinier,
**je veux** pouvoir activer ou desactiver le stockage de mon email depuis mon profil,
**afin de** garder le controle sur mes donnees personnelles a tout moment.

### Criteres d'acceptation

- [ ] CA1 : Une section "Email et confidentialite" est ajoutee dans la page profil (ou creee si la page profil n'existe pas encore).
- [ ] CA2 : La section affiche un toggle "Autoriser le stockage de mon email" avec l'etat actuel (`consentEmail` depuis `GET /api/user/profile`).
- [ ] CA3 : Un texte explicatif sous le toggle indique les consequences : desactiver supprime l'email, empeche les notifications, et chaque provider cree un compte separe.
- [ ] CA4 : La liste des providers lies est affichee (noms depuis le profil API).
- [ ] CA5 : Un changement de toggle appelle `PUT /api/user/profile/email-consent` et met a jour l'affichage en cas de succes.
- [ ] CA6 : Un snackbar confirme le changement ("Preferences mises a jour").
- [ ] CA7 : Toutes les chaines sont traduites (cles `Profile.*`).
- [ ] CA8 : Le composant compile sans erreur.

### Notes & contraintes
- Si la page profil n'existe pas, cette story la cree avec un layout minimal (header + section email consent). D'autres sections pourront etre ajoutees plus tard.
- Mobile-first : le layout s'adapte aux petits ecrans.

### Estimation
- **Priorite :** Important
- **Points :** 3
