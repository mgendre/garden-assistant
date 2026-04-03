## [US-334] Afficher le type de sol et le pH sur la fiche plante

**En tant que** jardinier,
**je veux** voir le type de sol prefere et la fourchette de pH optimale sur la fiche d'une plante,
**afin de** savoir si cette plante est adaptee a mon sol.

### Criteres d'acceptation

- [ ] CA1 : La fiche plante (detail) affiche les types de sol sous forme de badges traduits (ex: "Limoneux", "Sableux", "Argileux"). Plusieurs badges si la plante tolere plusieurs types.
- [ ] CA2 : La fiche plante affiche la fourchette de pH optimale au format "pH 6.0 - 6.8".
- [ ] CA3 : Si la liste des types de sol est vide, la ligne "Type de sol" n'est pas affichee (pas de "Non renseigne").
- [ ] CA4 : Si les champs pH sont null, la ligne "pH optimal" n'est pas affichee.
- [ ] CA5 : Les labels sont traduits via ngx-translate (cles : `Plant.SoilType`, `Plant.OptimalPh`, `SoilType.Sandy`, `SoilType.Clay`, etc.).
- [ ] CA6 : L'affichage est responsive (mobile-first) et utilise les classes du design system existant (`.panel`, etc.).
- [ ] CA7 : `npm run build --prefix garden-assistant-app` compile sans erreur.

### Notes & contraintes
- Le type de sol peut etre affiche avec le meme pattern que WaterNeeds et SunRequirement (badge/icone).
- Les traductions doivent etre ajoutees dans `fr.json` et `en.json`.
- Les 7 valeurs de SoilType doivent etre traduites : Sandy=Sableux, Silty=Limoneux, Clay=Argileux, Loam=Franc, Chalky=Calcaire, Peaty=Tourbeux, Rocky=Rocailleux.

### Estimation
- **Priorite :** Indispensable
- **Points :** 3
- **Statut :** A faire
