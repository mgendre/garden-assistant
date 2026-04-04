## [US-351] Alerte gel sur la page jardin

**En tant que** jardinier,
**je veux** etre prevenu quand du gel est prevu dans les prochains jours,
**afin de** proteger mes plantes sensibles au froid a temps.

### Criteres d'acceptation

- [ ] CA1 : Si la temperature minimale prevue dans les 3 prochains jours est inferieure ou egale a 0 degre C, une banniere d'alerte est affichee en haut de la page jardin.
- [ ] CA2 : La banniere indique la date et la temperature minimale prevue (ex: "Gel prevu vendredi nuit : -2 degres C. Protegez vos plantes sensibles au froid.").
- [ ] CA3 : La banniere est visuellement distincte (couleur d'alerte, icone givre) et utilise le composant `<app-info-banner>` existant ou un style coherent avec le design system.
- [ ] CA4 : L'alerte n'apparait que si le jardin a une localisation et que les donnees meteo sont disponibles.
- [ ] CA5 : Les textes utilisent ngx-translate (cles `Weather.FrostAlert`, `Weather.FrostAlertMessage`).
- [ ] CA6 : L'alerte est masquable par l'utilisateur pour la session en cours (pas de persistance, simple signal local).

### Notes & contraintes
- L'alerte gel est une des fonctionnalites les plus demandees par les jardiniers. Elle a une valeur enorme pour un cout de developpement modeste.
- Le seuil de 0 degre C est le point de gel standard. Pas de seuil configurable en v1.
- Depend de US-347 (endpoint meteo) et US-348 (widget meteo, pour le placement sur la page).

### Estimation
- **Priorite :** Important
- **Points :** 3
