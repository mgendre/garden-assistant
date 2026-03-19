## [US-058] Alerte de competition racinaire entre plantes de meme profondeur

**En tant que** jardinier,
**je veux** etre averti lorsque plusieurs plantes selectionnees partagent la meme zone racinaire sans association documentee entre elles,
**afin d'** eviter une competition racinaire silencieuse qui reduirait les rendements.

### Criteres d'acceptation

- [ ] CA1 : Un badge d'avertissement apparait sur les fiches plante du panneau central lorsque la plante partage sa zone racinaire avec une autre plante selectionnee.
- [ ] CA2 : Le badge d'avertissement est visuellement distinct du badge "mauvaise association" : couleur ambre (vs rouge) et icone differente.
- [ ] CA3 : L'avertissement n'est affiche que si aucune association documentee n'existe entre les deux plantes concernees, pour eviter le double avertissement.
- [ ] CA4 : L'utilisateur peut ignorer l'avertissement pour une paire donnee ; ce choix persiste durant la session (signal local, pas de persistance serveur).

### Notes & contraintes
- La detection de concurrence racinaire est calculee cote client a partir des signaux de selection existants.
- Si US-056 est livre en meme temps, partager la logique de detection de zone racinaire commune.
- Pas de nouvel endpoint API : toutes les donnees necessaires (`rootDepth`, associations) sont deja chargees.

### Estimation
- **Priorite :** Could
- **Points :** 2
