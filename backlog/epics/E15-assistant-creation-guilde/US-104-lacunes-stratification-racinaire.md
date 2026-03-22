## [US-104] Lacunes de stratification racinaire dans l'assistant

**En tant que** jardinier,
**je veux** que l'assistant m'indique quand une couche racinaire (superficielle, moyenne, profonde) est vide dans ma guilde,
**afin d'** obtenir une bonne repartition verticale des racines et exploiter tout le profil du sol.

### Criteres d'acceptation

- [ ] CA1 : La section "Assistant" affiche un avertissement pour chaque couche racinaire (Shallow, Medium, Deep) qui ne contient aucune plante parmi les plantes selectionnees.
- [ ] CA2 : Chaque avertissement de couche manquante est cliquable et applique le filtre de profondeur racinaire correspondant dans le catalogue (appel a `store.toggleRootDepthFilter(depth)`).
- [ ] CA3 : L'avertissement disparait en temps reel lorsqu'une plante couvrant cette couche est ajoutee a la guilde.
- [ ] CA4 : Un texte explicatif court accompagne les lacunes de stratification, expliquant l'interet de la diversite de profondeur racinaire (une seule cle de traduction).

### Notes & contraintes
- Reutilise le signal `rootDepthGroups` existant dans le `CompanionStore`.
- La section stratification existante (US-056) reste en place. L'assistant ajoute les lacunes dans sa propre section, au-dessus de la section stratification detaillee.

### Estimation
- **Priorite :** Indispensable
- **Points :** 3
