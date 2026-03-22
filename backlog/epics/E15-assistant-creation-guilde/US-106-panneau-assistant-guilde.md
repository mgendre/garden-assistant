## [US-106] Panneau assistant dans l'editeur de guilde

**En tant que** jardinier,
**je veux** voir toutes les recommandations de l'assistant regroupees dans un panneau dedie dans l'editeur de guilde,
**afin de** suivre facilement la progression de ma guilde vers un equilibre ecologique.

### Criteres d'acceptation

- [ ] CA1 : Un panneau collapsible "Assistant" apparait dans l'editeur de guilde des que 1 plante ou plus est selectionnee. Il utilise le composant `app-collapsible` existant et le pattern `.panel` avec un accent vert (`#f0f7f0`).
- [ ] CA2 : Le panneau regroupe, dans l'ordre : (a) le texte educatif (US-105, toggle via badge "?"), (b) les alertes (US-107, US-109), (c) les lacunes de mecanismes (US-103), (d) les lacunes de stratification racinaire (US-104).
- [ ] CA3 : Lorsqu'aucune lacune ne reste (tous les mecanismes prioritaires couverts et toutes les couches racinaires occupees et aucun conflit), le panneau affiche un message de felicitation indiquant que la guilde est bien equilibree.
- [ ] CA4 : Le panneau est place dans l'editeur de guilde entre la section "Plantes" et la section "Mecanismes" existante.
- [ ] CA5 : Le panneau s'adapte au responsive mobile (empilement vertical, pas de debordement horizontal).

### Notes & contraintes
- Ce panneau est le conteneur visuel ; les US-103, US-104 et US-105 fournissent le contenu.
- Le panneau utilise le pattern `.panel` existant avec une couleur d'accent differente (a definir avec le UX designer) pour se distinguer des sections informatives.
- Pas de nouvel appel API.

### Estimation
- **Priorite :** Indispensable
- **Points :** 3
