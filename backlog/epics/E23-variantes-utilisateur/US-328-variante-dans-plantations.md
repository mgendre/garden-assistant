## [US-328] Utiliser une variante dans les plantations

**En tant que** jardinier,
**je veux** pouvoir utiliser mes variantes personnelles dans mes plantations et guildes personnelles,
**afin de** planifier mes cultures avec les donnees adaptees a ma realite locale.

### Criteres d'acceptation

- [ ] CA1 : Les endpoints de selection de plantes (plantings, guildes) incluent les variantes de l'utilisateur courant dans les resultats.
- [ ] CA2 : Une variante peut etre assignee a une planche (planting) exactement comme une plante du catalogue.
- [ ] CA3 : Une variante peut etre ajoutee a une guilde personnelle de l'utilisateur.
- [ ] CA4 : Les associations heritees du parent sont prises en compte quand la variante est utilisee dans une planche ou guilde.
- [ ] CA5 : Les tests unitaires verifient : variante utilisable dans un planting, variante utilisable dans une guilde, associations heritees correctes.

### Notes & contraintes
- Depend de US-325 (lister variantes) et US-310 (heritage des associations).
- Les services existants qui acceptent un `PlantId` doivent fonctionner sans modification avec une variante — une variante est une `Plant` comme une autre.
- Verifier que les filtres existants (par famille, par mecanisme) incluent correctement les variantes.

### Estimation
- **Priorite :** Important
- **Points :** 3
- **Statut :** A faire
