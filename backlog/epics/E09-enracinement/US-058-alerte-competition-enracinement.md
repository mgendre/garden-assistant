## [US-058] Alertes de densite racinaire par zone

**En tant que** jardinier,
**je veux** etre averti lorsque trop de plantes partagent la meme zone racinaire,
**afin d'** eviter une competition racinaire qui reduirait les rendements.

### Criteres d'acceptation

- [x] CA1 : Un avertissement de densite apparait dans la colonne de stratification lorsque plus de 3 plantes occupent la meme zone racinaire.
- [x] CA2 : L'avertissement est visuellement distinct des alertes de conflit d'association (texte d'avertissement dans la zone, pas de badge par paire).

### Notes & contraintes
- Approche simplifiee par rapport a la specification initiale : avertissement par zone (densite) au lieu de par paire de plantes. Le systeme de paires de competition et le badge ambre par paire ont ete abandonnes au profit d'un avertissement global par zone, plus simple et plus utile en pratique.
- Le bouton "ignorer" par paire n'est plus pertinent avec l'approche par zone.
- Integre dans le composant `app-root-stratification` (US-056).

### Estimation
- **Priorite :** Could
- **Points :** 1
- **Statut :** Done
