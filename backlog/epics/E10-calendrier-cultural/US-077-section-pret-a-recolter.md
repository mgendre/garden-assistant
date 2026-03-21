## [US-077] Popup « Pret a recolter » depuis le calendrier

**En tant que** jardinier,
**je veux** pouvoir cliquer sur « Recolte » dans le calendrier pour voir comment reconnaitre que mon legume est pret a etre recolte,
**afin de** recolter au bon moment pour un gout et une qualite optimaux.

### Criteres d'acceptation

- [x] CA1 : Cliquer sur le label « Recolte » dans le Gantt ouvre un popup `HarvestReadinessDialog` avec la description et les criteres de maturite de la plante.
- [x] CA2 : Une icone `?` (fa-circle-question) a cote du label « Recolte » indique la presence d'informations supplementaires (uniquement si des donnees de maturite existent).
- [x] CA3 : Le popup affiche le texte descriptif, les jours depuis repiquage/semis en italique (pas en badge), et les criteres groupes par type avec icones (oeil, main, horloge, outil).
- [x] CA4 : Si la plante n'a pas de donnees de maturite, cliquer sur « Recolte » ouvre le popup educatif generique (BadgeInfo).
- [x] CA5 : Le popup est accessible depuis la page calendrier, le panneau associations/guildes, et la fiche plante.

### Notes & contraintes
- Le composant `HarvestReadinessComponent` est reutilise dans le popup dialog.
- Les textes de maturite ont le meme style que la description de la plante (`text-sm leading-7`).

### Estimation
- **Priorite :** Indispensable
- **Points :** 3
