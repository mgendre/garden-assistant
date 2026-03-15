## [US-035] Catalogue de plantes dans le panneau gauche

**En tant que** jardinier,
**je veux** voir la liste complete de mes plantes dans un panneau scrollable a gauche,
**afin de** parcourir le catalogue et selectionner les plantes qui m'interessent.

### Criteres d'acceptation

- [ ] CA1 : Au chargement de la page, toutes les plantes sont recuperees via `GET /api/Plants` et affichees dans le panneau gauche.
- [ ] CA2 : Chaque plante affiche : une icone (premiere lettre du nom sur fond vert pale), le nom commun, le nom latin en italique, et un badge de famille botanique colore.
- [ ] CA3 : Les familles connues (Solanacee, Alliacee, Cucurbitacee, Apiacee, Brassicacee, Asteracee, Lamiacee, Legumineuse) ont chacune un badge avec couleur distincte.
- [ ] CA4 : La liste est scrollable verticalement (`max-height: calc(100vh - 300px)`) avec une scrollbar fine.
- [ ] CA5 : Un compteur affiche le nombre de plantes visibles (ex. "24 plantes") dans l'en-tete du panneau.
- [ ] CA6 : Un indicateur de chargement s'affiche pendant le chargement initial des plantes.
- [ ] CA7 : Les plantes selectionnees sont visuellement marquees (fond vert pale, barre verte a gauche).

### Notes & contraintes
- `PlantDto` n'a pas de champ emoji ; utiliser la premiere lettre du nom dans un cercle colore comme icone de substitution.
- Le tri par defaut est alphabetique (A-Z) par nom commun. Le changement de tri est couvert par US-037.
- Le champ `family` de `PlantDto` est une chaine libre ; mapper les familles connues a des couleurs cote frontend.

### Estimation
- **Priorite :** Must
- **Points :** 3
