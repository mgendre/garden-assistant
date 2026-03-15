## [US-046] Composant info-box reutilisable

**En tant que** jardinier,
**je veux** voir un encart informatif sur la page "Mes plantes" m'expliquant les benefices futurs de cette liste,
**afin de** comprendre pourquoi maintenir cette liste est utile meme si toutes les fonctionnalites ne sont pas encore disponibles.

### Criteres d'acceptation

- [ ] CA1 : Un composant generique `InfoBoxComponent` est cree, acceptant en input un titre, un contenu (texte ou template), une icone, et un type (`info`, `tip`, `warning`).
- [ ] CA2 : Le type `info` affiche un fond bleu clair avec icone d'information. Le type `tip` affiche un fond vert clair. Le type `warning` affiche un fond orange clair.
- [ ] CA3 : L'info-box est fermable : un bouton (x) permet de la masquer. L'etat "ferme" est conserve en `localStorage` avec une cle configurable pour ne pas reapparaitre a chaque visite.
- [ ] CA4 : Sur la page "Mes plantes", une info-box de type `tip` s'affiche avec le texte : "En maintenant votre liste de plantes a jour, vous pourrez bientot suivre vos dates de semis et recevoir des rappels via un calendrier de culture personnalise."
- [ ] CA5 : L'info-box est responsive et s'affiche en pleine largeur sur mobile.

### Notes & contraintes
- Le composant est generique et pourra etre reutilise sur d'autres pages (ex. page jardin, rotations).
- Les textes utilisent les cles de traduction `MyPlants.InfoBoxTitle` et `MyPlants.InfoBoxContent`.
- Pas de logique metier dans ce composant -- affichage pur.

### Estimation
- **Priorite :** Should
- **Points :** 2
