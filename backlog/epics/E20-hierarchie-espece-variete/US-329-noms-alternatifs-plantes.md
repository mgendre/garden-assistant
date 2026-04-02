## [US-329] Noms alternatifs des plantes

**En tant que** jardinier,
**je veux** que chaque plante puisse avoir des noms alternatifs (ex. mache = doucette = salade de ble),
**afin de** retrouver facilement une plante quel que soit le nom commun que j'utilise.

### Criteres d'acceptation

- [ ] CA1 : L'entite `Plant` possede une propriete `AlternativeNames` de type `List<string>`. La colonne est stockee en `jsonb` (PostgreSQL) via la configuration EF Fluent API.
- [ ] CA2 : Une migration EF Core est generee et applicable sans erreur sur la base existante. La colonne `alternative_names` est nullable et par defaut `[]`.
- [ ] CA3 : `PlantDto` expose un champ `AlternativeNames` (liste de strings). Le mapping entity → DTO est en place.
- [ ] CA4 : Le fichier `plants.json` du seed data contient un champ `alternativeNames` renseigne pour au moins 20 plantes ayant des noms alternatifs bien connus en francais (ex. mache/doucette, tomate/pomme d'amour, topinambour/artichaut de Jerusalem, courgette/zucchini, roquette/rucola, ciboulette/civette, etc.).
- [ ] CA5 : La recherche dans le catalogue (frontend) interroge aussi les noms alternatifs — une recherche sur "doucette" remonte la mache.
- [ ] CA6 : La recherche dans "Mes plantes" (frontend) interroge aussi les noms alternatifs.
- [ ] CA7 : La page de detail d'une plante affiche les noms alternatifs sous le nom principal (ex. "Aussi appelee : doucette, salade de ble"). Si la liste est vide, rien n'est affiche.

### Notes & contraintes
- Le stockage `jsonb` est prefere a une table de jointure : la liste est courte, en lecture seule (seed), et ne necessite pas de requetes relationnelles.
- La recherche cote frontend peut etre un filtre local (les plantes sont deja chargees). Pas besoin d'un endpoint de recherche dedie.
- Le choix des 20+ plantes avec noms alternatifs doit etre valide par le plant-expert.
- Cette US enrichit le modele Plant sans dependre de la hierarchie espece-variete (US-306). Elle peut etre livree en parallele.

### Estimation
- **Priorite :** Important
- **Points :** 3
- **Statut :** A faire
