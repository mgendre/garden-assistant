## [US-031] Afficher les etats vides et l'accueil

**En tant que** jardinier,
**je veux** voir un message clair quand je n'ai rien selectionne ou quand aucun resultat n'est disponible,
**afin de** comprendre comment utiliser l'outil sans me sentir perdu.

### Criteres d'acceptation

- [ ] CA1 : A l'ouverture de la page, un message d'accueil explique en une phrase le fonctionnement : "Recherchez et selectionnez des plantes pour decouvrir leurs meilleurs compagnons et les associations a eviter."
- [ ] CA2 : Le message d'accueil disparait des qu'au moins 1 plante est selectionnee.
- [ ] CA3 : Si la recherche retourne 0 resultats, le message "Aucune plante trouvee" s'affiche dans la zone de resultats de recherche.
- [ ] CA4 : Si les compagnons benefiques sont vides, le message "Aucun compagnon benefique connu" s'affiche dans la section correspondante.
- [ ] CA5 : Si les plantes a eviter sont vides, la section n'est pas affichee (pas de zone vide).

### Notes & contraintes
- Les messages sont en francais, coherents avec le ton du reste de l'application.
- L'etat d'accueil doit etre visuellement agreable, pas une page blanche.

### Estimation
- **Priorite :** Should
- **Points :** 1
