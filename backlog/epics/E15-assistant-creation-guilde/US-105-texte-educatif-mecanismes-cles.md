## [US-105] Texte educatif sur les mecanismes cles d'une bonne guilde

**En tant que** jardinier debutant,
**je veux** comprendre quels mecanismes sont les plus importants dans une guilde et pourquoi,
**afin de** prendre des decisions eclairees lors de la composition de ma guilde.

### Criteres d'acceptation

- [ ] CA1 : Un badge "?" est affiche dans l'en-tete du panneau assistant. Un clic sur ce badge affiche/masque un texte introductif expliquant les grands principes d'une guilde equilibree (fixation d'azote, couverture du sol, attraction de pollinisateurs, diversite racinaire).
- [ ] CA2 : Le texte est masque par defaut. Il est toujours disponible quel que soit le nombre de plantes selectionnees (pas de condition sur le nombre de plantes).
- [ ] CA3 : Les mecanismes cites dans le texte introductif sont des chips cliquables qui appliquent le filtre correspondant dans le catalogue.
- [ ] CA4 : Toutes les chaines de texte utilisent des cles de traduction (PascalCase, namespace `GuildAssistant.*`).

### Notes & contraintes
- Le texte doit rester concis (3-4 phrases maximum). L'objectif est de guider, pas de faire un cours de permaculture.
- Le badge "?" a deux etats visuels : contour vert quand ferme, rempli vert fonce quand ouvert.

### Estimation
- **Priorite :** Important
- **Points :** 2
