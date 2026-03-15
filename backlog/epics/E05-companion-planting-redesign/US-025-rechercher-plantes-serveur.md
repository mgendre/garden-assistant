## [US-025] Rechercher des plantes via le serveur

**En tant que** jardinier,
**je veux** rechercher des plantes en tapant au moins un caractere dans un champ de recherche,
**afin de** trouver rapidement une plante sans charger toute la base.

### Criteres d'acceptation

- [ ] CA1 : Le champ de recherche est vide a l'ouverture de la page ; aucune liste de plantes n'est affichee.
- [ ] CA2 : Aucune requete n'est envoyee au serveur tant que l'utilisateur n'a pas saisi au moins 1 caractere.
- [ ] CA3 : Apres saisie d'au moins 1 caractere, une requete est envoyee au backend qui filtre par nom commun ou nom latin (insensible a la casse, recherche partielle).
- [ ] CA4 : Les resultats s'affichent sous le champ de recherche dans une liste deroulante ou un panneau de resultats.
- [ ] CA5 : Un indicateur de chargement (spinner) est visible pendant l'appel serveur.
- [ ] CA6 : Si aucun resultat ne correspond, un message "Aucune plante trouvee" s'affiche.
- [ ] CA7 : Les requetes sont anti-rebond (debounce >= 300 ms) pour eviter de surcharger le backend.

### Notes & contraintes
- Le backend doit exposer un endpoint de recherche avec parametre `q` (query string). L'endpoint actuel `GET /api/plants` retourne tout sans filtre ; un nouveau `GET /api/plants/search?q=tom` est necessaire.
- Le filtrage, le tri et la limite de resultats sont cotes serveur (pas de filtre client-side sur la liste complete).

### Estimation
- **Priorite :** Must
- **Points :** 3
