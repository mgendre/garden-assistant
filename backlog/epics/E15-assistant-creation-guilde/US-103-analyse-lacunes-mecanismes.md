## [US-103] Analyse des lacunes de mecanismes dans la guilde

**En tant que** jardinier en train de composer une guilde,
**je veux** que l'assistant m'indique quels mecanismes importants sont absents de ma guilde,
**afin de** savoir quels roles ecologiques restent a couvrir pour obtenir une guilde equilibree.

### Criteres d'acceptation

- [ ] CA1 : Un service frontend (`GuildAssistantService` ou computed signals dans le store) calcule les mecanismes manquants a partir des mecanismes intrinseques et relationnels deja presents dans la guilde. Les mecanismes prioritaires sont : NitrogenFixation, SoilCover, PollinatorAttraction, DynamicAccumulation, PredatorAttraction.
- [ ] CA2 : Lorsque au moins 1 plante est selectionnee, la section "Assistant" affiche la liste des mecanismes sous forme de checklist (satisfaits avec coche verte, manquants avec chips cliquables).
- [ ] CA3 : Le mecanisme manquant le plus prioritaire est mis en surbrillance (fond vert, bordure gauche) avec une phrase courte expliquant pourquoi il est important (via les cles de traduction `GuildAssistant.Gap.*`). Les autres manquants sont affiches sans explication.
- [ ] CA4 : Un clic sur un mecanisme manquant applique le filtre correspondant dans le catalogue (appel a `store.toggleMechanismFilter(mechanism)`), permettant au jardinier de trouver rapidement une plante qui comble ce manque.
- [ ] CA5 : Lorsqu'un mecanisme est comble (intrinseque OU relationnel), la ligne passe a l'etat "satisfait" (coche verte + nom de la plante). Les lignes ne changent jamais d'ordre — stabilite spatiale.

### Notes & contraintes
- Calcul purement frontend a partir des signaux existants (`guildIntrinsicMechanisms`, `guildRelationalOnlyMechanisms`).
- Les 5 mecanismes prioritaires sont un sous-ensemble des 16 mecanismes. Les 11 autres ne sont pas signales comme manquants mais restent visibles s'ils sont presents.
- Pas de nouvel appel API.

### Estimation
- **Priorite :** Indispensable
- **Points :** 5
