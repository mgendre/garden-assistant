## [US-109] Alerte diversite des familles botaniques dans l'assistant

**En tant que** jardinier en train de composer une guilde,
**je veux** que l'assistant me previenne lorsque trop de plantes appartiennent a la meme famille botanique,
**afin de** reduire le risque de propagation de maladies et ravageurs specifiques a une famille.

### Criteres d'acceptation

- [ ] CA1 : L'assistant detecte lorsqu'une famille botanique represente plus de 40 % des plantes de la guilde ET qu'au moins 3 plantes de cette famille sont presentes. La verification s'effectue en temps reel a chaque ajout ou retrait de plante.
- [ ] CA2 : Lorsqu'un depassement est detecte, un avertissement s'affiche dans le panneau assistant, indiquant le nom de la famille surrepresentee, le nombre de plantes concernees, et le pourcentage qu'elles representent.
- [ ] CA3 : Le message d'avertissement explique le risque : les plantes d'une meme famille partagent les memes maladies et ravageurs, ce qui augmente la vulnerabilite de la guilde en cas d'attaque.
- [ ] CA4 : Si plusieurs familles depassent le seuil simultanement, un avertissement distinct est affiche pour chacune.
- [ ] CA5 : L'avertissement disparait en temps reel lorsque le retrait d'une plante fait passer la famille sous le seuil.

### Notes & contraintes
- Le champ `botanicalFamily` existe deja sur l'entite `Plant` et est expose dans le DTO. Le calcul est purement frontend, sans nouvel appel API.
- Le seuil (40 %, minimum 3) est defini comme constante dans le service ou le store, pour pouvoir etre ajuste facilement.
- Cet avertissement s'affiche apres les alertes d'associations nefastes (US-107) et avant les lacunes de mecanismes (US-103).

### Estimation
- **Priorite :** Important
- **Points :** 2
