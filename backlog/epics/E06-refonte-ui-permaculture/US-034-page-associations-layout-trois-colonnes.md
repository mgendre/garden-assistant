## [US-034] Page associations avec layout trois colonnes

**En tant que** jardinier,
**je veux** acceder a une page "Associations" organisee en trois colonnes (catalogue, detail, compagnons),
**afin de** voir d'un coup d'oeil le catalogue de plantes, mes plantes selectionnees, et leurs associations.

### Criteres d'acceptation

- [ ] CA1 : La route `/associations` affiche un composant `CompanionsPage` a l'interieur du shell.
- [ ] CA2 : Un titre "Associations de plantes" et un sous-titre descriptif sont affiches au-dessus de la grille.
- [ ] CA3 : La page est divisee en trois colonnes : gauche (280px), centre (flexible), droite (320px), avec un `gap` de 1.25rem.
- [ ] CA4 : Chaque colonne est un panel blanc avec coins arrondis (16px), bordure subtile et ombre legere.
- [ ] CA5 : Les trois colonnes sont rendues meme si leur contenu est vide (etats vides geres par US-040).
- [ ] CA6 : Sur tablette (< 1024px), les colonnes se reduisent proportionnellement (240px / 1fr / 280px).
- [ ] CA7 : Sur mobile (< 768px), les colonnes s'empilent verticalement en une seule colonne pleine largeur.

### Notes & contraintes
- Ce story cree la structure de la page uniquement ; le contenu de chaque colonne est couvert par US-035 a US-040.
- Utiliser CSS Grid pour le layout trois colonnes.
- Les cles de traduction `Companions.Title` et `Companions.Description` existent deja.

### Estimation
- **Priorite :** Must
- **Points :** 2
- **Statut :** Done
