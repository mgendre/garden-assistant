## [US-079] Popups educatives des types d'actions culturales

**En tant que** jardinier debutant,
**je veux** pouvoir cliquer sur un type d'action (semis interieur, buttage, pincage, etc.) pour obtenir une explication claire de ce que c'est et comment le faire,
**afin de** comprendre les interventions recommandees par le calendrier.

### Criteres d'acceptation

- [ ] CA1 : Un composant `ActionTypeBadgeInfoComponent` affiche une popup educative pour chaque type d'action, suivant le meme pattern que les BadgeInfo existants (ensoleillement, enracinement, mecanismes).
- [ ] CA2 : Chaque popup contient : une description de l'action, pourquoi c'est important, les principes de base pour bien le faire.
- [ ] CA3 : Les popups sont accessibles depuis la page calendrier (clic sur la legende ou les chips de filtre) et depuis la vue Gantt (clic sur le label du type d'action).
- [ ] CA4 : Les 8 types d'actions ont leur popup (valeurs enum) : `IndoorSowing`, `DirectSowing`, `Transplanting`, `Harvest`, `Pruning`, `Pinching`, `Hilling`, `Division`.
- [ ] CA5 : Les cles de traduction suivent la convention `BadgeInfo.Action.*` (coherent avec les BadgeInfo existants).

### Notes & contraintes
- Reutiliser le composant `badge-info-dialog` existant et son pattern d'ouverture.
- Le contenu pedagogique est fourni par l'agent `plant-expert`.
- Les textes sont en francais dans `fr.json`.

### Estimation
- **Priorite :** Important
- **Points :** 2
