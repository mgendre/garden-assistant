## [US-036] Recherche dans le catalogue de plantes

**En tant que** jardinier,
**je veux** filtrer le catalogue en tapant dans un champ de recherche,
**afin de** trouver rapidement une plante par son nom commun ou latin.

### Criteres d'acceptation

- [ ] CA1 : Un champ de recherche avec icone loupe est affiche en haut du panneau gauche, sous l'en-tete "Catalogue".
- [ ] CA2 : La recherche filtre la liste localement (client-side) sur le nom commun et le nom latin, insensible a la casse et aux accents.
- [ ] CA3 : Le filtrage est instantane (pas de debounce necessaire car la liste est deja chargee en memoire).
- [ ] CA4 : Le compteur de plantes se met a jour pour refleter le nombre de resultats filtres.
- [ ] CA5 : Si aucune plante ne correspond, un message "Aucune plante trouvee" s'affiche dans la liste.
- [ ] CA6 : Le placeholder du champ est "Rechercher une plante...".

### Notes & contraintes
- Le filtrage est client-side car `GET /api/Plants` retourne un catalogue de taille raisonnable (< 200 plantes). Pas besoin de `search?q=` pour cette fonctionnalite.
- La cle de traduction `Companions.SearchPlaceholder` existe deja.
- Ce story est volontairement separe du tri (US-037) pour rester petit et testable.

### Estimation
- **Priorite :** Must
- **Points :** 1
- **Statut :** Done
