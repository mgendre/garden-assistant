## [US-334] Afficher le type de sol et le pH sur la fiche plante

**En tant que** jardinier,
**je veux** voir le type de sol préféré et la fourchette de pH optimale sur la fiche d'une plante,
**afin de** savoir si cette plante est adaptée à mon sol.

### Critères d'acceptation

- [x] CA1 : La fiche plante (détail) affiche les types de sol sous forme de badges traduits (ex: "Limoneux", "Sableux", "Argileux"). Plusieurs badges si la plante tolère plusieurs types.
- [x] CA2 : La fiche plante affiche la fourchette de pH optimale au format "pH 6.0 - 6.8".
- [x] CA3 : Si la liste des types de sol est vide, la ligne "Type de sol" n'est pas affichée (pas de "Non renseigné").
- [x] CA4 : Si les champs pH sont null, la ligne "pH optimal" n'est pas affichée.
- [x] CA5 : Les labels sont traduits via ngx-translate (clés : `Plant.SoilType`, `Plant.OptimalPh`, `SoilType.Sandy`, `SoilType.Clay`, etc.).
- [x] CA6 : L'affichage est responsive (mobile-first) et utilise les classes du design system existant (`.panel`, etc.).
- [x] CA7 : `npm run build --prefix garden-assistant-app` compile sans erreur.

### Notes & contraintes
- Le type de sol peut être affiché avec le même pattern que WaterNeeds et SunRequirement (badge/icône).
- Les traductions doivent être ajoutées dans `fr.json` et `en.json`.
- Les 7 valeurs de SoilType doivent être traduites : Sandy=Sableux, Silty=Limoneux, Clay=Argileux, Loam=Franc, Chalky=Calcaire, Peaty=Tourbeux, Rocky=Rocailleux.

### Estimation
- **Priorite :** Indispensable
- **Points :** 3
- **Statut :** Termine
