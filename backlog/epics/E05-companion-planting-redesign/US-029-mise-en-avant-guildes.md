## [US-029] Mettre en avant les plantes de guildes

**En tant que** jardinier,
**je veux** voir les plantes membres d'une guilde mises en avant dans les resultats de compagnons benefiques,
**afin de** privilegier des associations eprouvees et comprendre pourquoi elles fonctionnent ensemble.

### Criteres d'acceptation

- [ ] CA1 : Dans la section "Bons compagnons", les plantes appartenant a une guilde partagee avec la selection sont affichees en premier, avant les compagnons benefiques classiques.
- [ ] CA2 : Chaque plante issue d'une guilde porte un badge visuel indiquant le nom de la guilde (ex. "Guilde de la Tomate").
- [ ] CA3 : Le badge de guilde est visuellement distinct (couleur ou icone specifique) pour attirer l'attention.
- [ ] CA4 : Au survol ou au clic du badge de guilde, une infobulle ou un panneau affiche la description de la guilde expliquant les mecanismes synergiques.
- [ ] CA5 : Si une plante appartient a plusieurs guildes pertinentes pour la selection, tous les badges correspondants sont affiches.
- [ ] CA6 : Le tri au sein de la section guilde suit le meme ordre par score que les compagnons classiques.

### Notes & contraintes
- Les guildes sont des groupes curates cote backend (nouvelle entite `Guild` avec nom, description, et table de liaison `GuildPlant`). Le backend n'a pas encore cette entite ; elle doit etre creee.
- Exemples de guildes : "La Guilde de la Tomate" (Tomate, Basilic, Carotte, Persil, Tagete, Bourrache), "Les Trois Soeurs" (Mais, Haricot, Courge).
- Le backend inclut les informations de guilde dans la reponse de l'endpoint companions ; le frontend ne fait pas de requete supplementaire.

### Estimation
- **Priorite :** Must
- **Points :** 5
