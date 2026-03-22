## [US-108] Indicateur de sante de la guilde

**En tant que** jardinier,
**je veux** voir un indicateur visuel synthetique de la qualite de ma guilde,
**afin de** savoir d'un coup d'oeil si ma guilde est equilibree ou si elle necessite des ajustements.

### Criteres d'acceptation

- [ ] CA1 : Un indicateur textuel (par exemple "2 lacunes restantes" ou "Guilde equilibree") est affiche dans l'en-tete du panneau assistant.
- [ ] CA2 : Le nombre de lacunes est la somme des mecanismes prioritaires manquants + couches racinaires vides. Les alertes (associations nefastes, diversite de familles) ne sont pas comptabilisees dans ce nombre — elles sont affichees separement.
- [ ] CA3 : Lorsque le nombre de lacunes est 0, l'indicateur affiche un message positif et change de couleur (vert).
- [ ] CA4 : L'indicateur se met a jour en temps reel a chaque ajout ou retrait de plante.

### Notes & contraintes
- Indicateur purement textuel et leger — pas de jauge, pas de pourcentage. L'objectif est la clarte, pas la gamification.
- Calcul frontend uniquement a partir des signaux existants.

### Estimation
- **Priorite :** Optionnel
- **Points :** 2
