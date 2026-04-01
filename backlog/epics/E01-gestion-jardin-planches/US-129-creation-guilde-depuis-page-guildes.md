## [US-129] Créer une guilde depuis la page Guildes avec mode création dédié

**En tant que** jardinier,
**je veux** pouvoir créer une nouvelle guilde directement depuis la page Guildes,
**afin de** ne pas avoir à passer par la page associations pour démarrer une nouvelle composition.

### Criteres d'acceptation

- [x] CA1 : Un bouton "Nouvelle guilde" est disponible sur la page Guildes.
- [x] CA2 : Le clic bascule la page associations en mode "création de guilde" avec une banniere d'information dédiée.
- [x] CA3 : La banniere indique le nombre minimum de plantes requises pour créer une guilde.
- [x] CA4 : Le bouton "Enregistrer" crée la guilde et redirige vers la page Guildes.
- [x] CA5 : Un bouton "Annuler" permet de quitter le mode création sans sauvegarder.

### Notes & contraintes
- Même pattern de navigation que la modification de planche (params `returnTo`, banniere contextuelle).
- La banniere de création de guilde est distincte de la banniere de modification de planche.

### Estimation
- **Priorite :** Should
- **Points :** 2
- **Statut :** Terminé
