## [US-064] Alertes de taille et de pincage dans le calendrier

**En tant que** jardinier,
**je veux** voir dans le calendrier quand tailler ou pincer mes plantes (tomates, courges, arbres fruitiers…),
**afin de** ne pas manquer ces interventions essentielles pour la productivite et la sante des plantes.

### Criteres d'acceptation

- [ ] CA1 : Les actions de type Taille et Pincage apparaissent dans la grille aux mois correspondants avec une icone distincte (ciseau SVG ou classe CSS equivalente — pas d'emoji).
- [ ] CA2 : L'icone de taille/pincage se differencie visuellement des icones de semis et de recolte (forme et couleur differentes).
- [ ] CA3 : Cliquer sur une marque de taille ou pincage ouvre une infobulle ou un panneau lateral avec la note explicative associee a l'action (champ `Notes` de `PlantAction`), par exemple "Supprimez les gourmands des que la premiere grappe est visible".
- [ ] CA4 : Les plantes de seed (US-059) incluant la tomate ont au moins une action Taille ou Pincage avec une note explicative.

### Notes & contraintes
- Reutiliser le composant d'infobulle ou de dialog existant pour afficher la note — pas de nouveau composant de popup si l'existant convient.
- L'icone ciseau est un SVG inline ou une icone Angular Material (`content_cut`) — jamais un emoji dans le code.

### Estimation
- **Priorite :** Should
- **Points :** 3
