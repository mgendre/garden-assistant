## [US-107] Alerte associations nefastes dans l'assistant

**En tant que** jardinier,
**je veux** que l'assistant me previenne lorsque ma guilde contient des associations nefastes,
**afin de** ne pas planter ensemble des especes incompatibles sans en etre conscient.

### Criteres d'acceptation

- [ ] CA1 : Lorsque des associations nefastes existent entre les plantes selectionnees, la section "Assistant" affiche un avertissement en haut du panneau, au-dessus des lacunes de mecanismes.
- [ ] CA2 : L'avertissement indique le nombre de conflits et un lien "voir les details" qui scroll vers (ou ouvre) la section "Detail des associations" existante (US-069).
- [ ] CA3 : L'avertissement disparait en temps reel lorsque les plantes en conflit sont retirees de la guilde.

### Notes & contraintes
- La section "Detail des associations" (US-069) et les "Avertissements de conflits" (US-068) existent deja. Cette story ajoute une mention dans le panneau assistant pour consolider l'information, sans dupliquer l'affichage complet des conflits.
- Reutilise le computed `hasHarmfulAssociations` existant dans le `GuildEditor`.

### Estimation
- **Priorite :** Important (confirme pour le MVP)
- **Points :** 2
