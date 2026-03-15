## [US-033] App shell avec header et navigation

**En tant que** jardinier,
**je veux** voir un header fixe avec le logo PermaGarden et des liens de navigation,
**afin de** me reperer dans l'application et acceder rapidement aux differentes sections.

### Criteres d'acceptation

- [ ] CA1 : Un composant `ShellComponent` enveloppe `<router-outlet>` et affiche un header sticky en haut de page.
- [ ] CA2 : Le header a un fond vert fonce (`--green-deep`) avec le logo "PermaGarden" (feuille stylisee + texte, "Garden" en accent orange).
- [ ] CA3 : Le header contient des liens de navigation : Tableau de bord, Associations, Mon jardin. Les liens non implementes pointent vers une route vide ou sont desactives.
- [ ] CA4 : Le lien actif est visuellement distinct (couleur verte claire, fond semi-transparent).
- [ ] CA5 : Un avatar utilisateur (initiales) est affiche a droite du header.
- [ ] CA6 : Sur mobile (< 768px), les liens de navigation sont masques dans un menu hamburger qui s'ouvre en overlay.
- [ ] CA7 : La route par defaut (`/`) redirige vers `/associations`.
- [ ] CA8 : Le header a une hauteur de 64px desktop, 56px mobile, et `z-index: 100`.

### Notes & contraintes
- Le shell est un layout wrapper ; il ne contient aucune logique metier.
- Les textes du header utilisent les cles de traduction `Nav.*` deja definies dans `fr.json`.
- La route `/associations` sera implementee par US-034.
- Utiliser les variables Sass existantes (`$header-height`, `$color-forest`, etc.) et les utilitaires Tailwind pour le layout.

### Estimation
- **Priorite :** Must
- **Points :** 3
