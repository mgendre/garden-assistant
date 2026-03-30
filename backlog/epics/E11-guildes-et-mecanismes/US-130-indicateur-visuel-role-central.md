## [US-130] Indicateur visuel du role central dans l'editeur de guilde

**En tant que** jardinier editant une guilde,
**je veux** voir quelles plantes sont centrales et pouvoir changer leur role d'un clic,
**afin de** designer visuellement la guilde autour de mes plantes principales.

### Criteres d'acceptation

- [ ] CA1 : Chaque plante dans l'editeur de guilde affiche une icone etoile (`faStar`). Etoile pleine doree = `Central`, etoile outline = `Companion`.
- [ ] CA2 : Un clic sur l'etoile bascule le role entre `Central` et `Companion`. La modification est stockee localement et persistee au prochain Save de la guilde.
- [ ] CA3 : Les plantes centrales ont une bordure gauche doree (`3px solid var(--color-accent)`) sur leur carte.
- [ ] CA4 : Les plantes centrales sont triees en premier dans la liste.
- [ ] CA5 : Un hint s'affiche quand aucune plante n'est marquee centrale en mode edition : "Designez une ou plusieurs plantes centrales autour desquelles votre guilde est construite."
- [ ] CA6 : En mode lecture seule (vue planche, mode viewing), l'etoile doree est visible a cote du nom mais non cliquable. Les plantes compagnes n'affichent pas d'etoile.
- [ ] CA7 : Pour les guildes officielles (readonly), l'icone est visible mais non cliquable.
- [ ] CA8 : L'icone est accessible : touch target min 44x44px, `aria-label` dynamique, focus ring.
- [ ] CA9 : Cles de traduction ajoutees : `Guild.CentralPlant`, `Guild.Companion`, `Guild.CentralPlantHint`, `Guild.ToggleCentral`, `Guild.ToggleCompanion`.

### Notes & contraintes
- L'etoile est placee dans le header de la plant card, a gauche du coeur (favori).
- Nouveaux inputs sur `PlantCard` : `showCentralToggle`, `isCentral`, `showCentralIndicator`. Nouvel output : `centralToggle`.
- Le `CompanionStore` gere `centralPlantIds` comme signal.
- Ne pas implementer de logique de suggestion basee sur le role central (YAGNI).

### Estimation
- **Priorite :** Important
- **Points :** 5
