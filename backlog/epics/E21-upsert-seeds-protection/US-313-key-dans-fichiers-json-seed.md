## [US-313] Ajouter le champ key dans les fichiers JSON de seed

**En tant que** developpeur,
**je veux** m'assurer que tous les fichiers JSON de seed utilisent un champ `key` stable et coherent,
**afin de** permettre le matching fiable entre les donnees JSON et les entites en base.

### Criteres d'acceptation

- [ ] CA1 : Le fichier `plants.json` possede un champ `key` sur chaque entree (deja present — verifier la coherence).
- [ ] CA2 : Le fichier `associations.json` reference les plantes par `key` (source et target) au lieu du nom.
- [ ] CA3 : Le fichier `guilds.json` reference les plantes membres par `key`.
- [ ] CA4 : Le fichier `plant-actions.json` reference les plantes par `key`.
- [ ] CA5 : Le fichier `harvest-readiness.json` reference les plantes par `key`.
- [ ] CA6 : Toutes les cles sont en kebab-case, stables et uniques par plante du catalogue.
- [ ] CA7 : Les seeders existants continuent de fonctionner avec les nouveaux champs (pas de regression).

### Notes & contraintes
- Certains fichiers utilisent peut-etre deja `key` — verifier et completer si necessaire.
- Les cles doivent correspondre exactement entre tous les fichiers JSON.
- Cette US peut etre realisee en parallele de US-312.

### Estimation
- **Priorite :** Indispensable
- **Points :** 2
- **Statut :** A faire
