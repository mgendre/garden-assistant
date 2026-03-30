## [US-129] Role central et completude des associations dans les guildes officielles

**En tant que** jardinier consultant les guildes officielles,
**je veux** que les plantes centrales soient identifiees et que toutes les associations entre plantes de chaque guilde soient documentees,
**afin de** comprendre autour de quelles plantes chaque guilde est construite et voir les interactions entre toutes les plantes.

### Criteres d'acceptation

- [ ] CA1 : Le fichier `guilds.json` est enrichi : chaque entree de `plantKeys` devient un objet `{ "key": "tomate", "role": "Central" }` ou reste une string simple (interprete comme `Companion` par defaut).
- [ ] CA2 : Le `GuildSeeder` lit le nouveau format et persiste le role dans `GuildPlant.Role`.
- [ ] CA3 : Les 50 guildes officielles ont des roles corrects valides par le plant-expert. Exemples :
  - Trois Soeurs : mais, haricot-a-rames, courge = Central
  - Guilde de la Tomate : tomate = Central
  - Guilde Mediterraneenne : romarin, thym, sauge-officinale, lavande = Central
- [ ] CA4 : Le seeder reste retrocompatible : les anciennes entrees sans role fonctionnent (defaut Companion).
- [ ] CA5 : Le fichier `associations.json` est enrichi pour couvrir toutes les paires de plantes au sein de chaque guilde officielle. Actuellement ~73% des paires sont manquantes (608 sur 836). Chaque association ajoutee doit avoir un mecanisme, un effet, et une note valides par le plant-expert.
- [ ] CA6 : Aucune guilde officielle n'a 0% de couverture d'associations. Toutes les guildes ont au minimum les associations les plus significatives documentees.

### Notes & contraintes
- Travail de validation des 50 guildes et de leurs associations a faire avec le plant-expert.
- Le fichier JSON doit supporter les deux formats (string et objet) pour les plantKeys.
- L'enrichissement de `associations.json` concerne les paires manquantes dans les guildes. Certaines paires peuvent legitimement ne pas avoir d'association (plantes neutres entre elles) — dans ce cas, documenter avec `effect: "Neutral"` ou ignorer.
- Prioriser les guildes les plus utilisees et celles ayant 0% de couverture (Mediterraneenne, Racines, Pollinisateurs, Engrais Verts).

### Estimation
- **Priorite :** Indispensable
- **Points :** 8
