## [US-054] Badge de profondeur d'enracinement sur les fiches plante

**En tant que** jardinier,
**je veux** voir d'un coup d'oeil la profondeur d'enracinement d'une plante sur sa fiche,
**afin de** choisir des combinaisons complementaires sans avoir a consulter des references externes.

### Criteres d'acceptation

- [x] CA1 : Chaque fiche plante affiche un badge visuel indiquant la profondeur racinaire : superficiel (0-30 cm), moyen (30-60 cm) ou profond (>60 cm).
- [x] CA2 : Le badge utilise une icone distinctive pour chaque niveau (ex. herbe/arbuste/arbre).
- [x] CA3 : Un clic sur le badge ouvre une explication via le composant `BadgeInfoDialog` existant.
- [x] CA4 : Le badge est visible sur mobile sans troncature.

### Notes & contraintes
- Le champ `rootDepth` est deja present dans l'entite `Plant` et dans `PlantDto`.
- Reutiliser le composant `BadgeInfoDialog` existant pour l'explication.
- Cle de traduction : `BadgeInfo.RootDepth.*`.

### Estimation
- **Priorite :** Must
- **Points :** 1
- **Statut :** Done
