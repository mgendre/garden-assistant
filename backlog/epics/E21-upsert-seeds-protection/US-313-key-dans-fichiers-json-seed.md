## [US-313] Ajouter le champ key dans les fichiers JSON de seed

**En tant que** développeur,
**je veux** m'assurer que tous les fichiers JSON de seed utilisent un champ `key` stable et cohérent,
**afin de** permettre le matching fiable entre les données JSON et les entités en base.

### Critères d'acceptation

- [x] CA1 : Le fichier `plants.json` possède un champ `key` sur chaque entrée (déjà présent — vérifier la cohérence).
- [x] CA2 : Le fichier `associations.json` référence les plantes par `key` (source et target) au lieu du nom.
- [x] CA3 : Le fichier `guilds.json` référence les plantes membres par `key`.
- [x] CA4 : Le fichier `plant-actions.json` référence les plantes par `key`.
- [x] CA5 : Le fichier `harvest-readiness.json` référence les plantes par `key`.
- [x] CA6 : Toutes les clés sont en kebab-case, stables et uniques par plante du catalogue.
- [x] CA7 : Les seeders existants continuent de fonctionner avec les nouveaux champs (pas de régression).

### Notes & contraintes
- Certains fichiers utilisent peut-être déjà `key` — vérifier et compléter si nécessaire.
- Les clés doivent correspondre exactement entre tous les fichiers JSON.
- Cette US peut être réalisée en parallèle de US-312.

### Estimation
- **Priorite :** Indispensable
- **Points :** 2
- **Statut :** Termine
