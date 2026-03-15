## [US-024] Exporter l'historique de culture

**En tant que** jardinier,
**je veux** exporter l'historique de culture de mes planches au format CSV,
**afin de** l'archiver, le partager avec un autre jardinier ou l'analyser dans un tableur.

### Critères d'acceptation

- [ ] CA1 : Je peux exporter l'historique complet de tous mes jardins en un seul fichier CSV.
- [ ] CA2 : Je peux aussi exporter l'historique d'un seul jardin.
- [ ] CA3 : Le CSV contient : jardin, planche, plante, famille botanique, date début, date fin.
- [ ] CA4 : L'encodage est UTF-8 avec BOM (pour compatibilité Excel).

### Notes & contraintes
- Export côté serveur pour gérer des volumes de données potentiellement importants.

### Estimation
- **Priorité :** Could
- **Points :** 3
