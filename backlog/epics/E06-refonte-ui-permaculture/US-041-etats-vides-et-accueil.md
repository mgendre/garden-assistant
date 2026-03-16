## [US-041] Etats vides et messages d'accueil

**En tant que** jardinier arrivant sur la page pour la premiere fois,
**je veux** voir des messages clairs quand aucune plante n'est selectionnee,
**afin de** comprendre comment utiliser l'outil sans me sentir perdu.

### Criteres d'acceptation

- [ ] CA1 : Le panneau central affiche un etat vide avec une icone (plantule) et le texte "Selectionnez une plante dans le catalogue pour decouvrir ses associations".
- [ ] CA2 : Le panneau droit affiche un etat vide avec une icone (poignee de main) et le texte "Les associations benefiques, guildes et plantes a eviter apparaitront ici".
- [ ] CA3 : Les etats vides disparaissent des qu'au moins 1 plante est selectionnee.
- [ ] CA4 : Si toutes les plantes sont retirees de la selection, les etats vides reapparaissent.
- [ ] CA5 : Les etats vides sont centres verticalement et horizontalement dans leur panneau respectif.

### Notes & contraintes
- Les textes utilisent les cles `Companions.EmptyTitle` et `Companions.EmptyDesc` existantes. De nouvelles cles seront ajoutees pour les textes specifiques a chaque panneau.
- Le design des etats vides reprend le style de la maquette : icone en opacite reduite, texte en `color-text-muted`, centre.

### Estimation
- **Priorite :** Should
- **Points :** 1
- **Statut :** Done
