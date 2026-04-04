## [US-344] Quantites d'eau indicatives

**En tant que** jardinier debutant,
**je veux** voir une quantite d'eau indicative par plante dans la grille hebdomadaire et le tableau des frequences,
**afin de** savoir approximativement combien d'eau apporter a chaque arrosage.

### Criteres d'acceptation

- [ ] CA1 : Un champ `WaterAmountMl` (int, nullable) est ajoute a l'entite `Plant`.
- [ ] CA2 : Une migration EF Core est generee pour ajouter la colonne `water_amount_ml` a la table `plants`.
- [ ] CA3 : Le seed data est enrichi avec les quantites d'eau pour les plantes courantes (valeurs validees par le plant-expert).
- [ ] CA4 : Le DTO plante expose le champ `WaterAmountMl`.
- [ ] CA5 : La grille hebdomadaire (US-341) affiche la quantite d'eau a cote de chaque cercle bleu (ex : "500 ml").
- [ ] CA6 : La section "Frequences saisonnieres" (US-341) affiche la quantite par arrosage dans une colonne supplementaire.
- [ ] CA7 : Un total journalier indicatif est affiche en bas de chaque colonne de la grille ("~X L").
- [ ] CA8 : Si `WaterAmountMl` est null pour une plante, aucune quantite n'est affichee (pas de valeur par defaut inventee).
- [ ] CA9 : Les quantites sont formatees de maniere lisible : en ml si < 1000, en L si >= 1000 (ex : "500 ml", "1,5 L").
- [ ] CA10 : Tous les textes utilisent `ngx-translate`.
- [ ] CA11 : `npm run build` passe sans erreur.

### Notes & contraintes
- Les quantites sont purement indicatives et volontairement simplistes. Elles servent de repere pour un debutant, pas de reference agronomique.
- Le seed data doit couvrir au minimum les plantes les plus courantes (tomate, courgette, laitue, basilic, carotte, poivron, haricot, concombre, fraise, radis).
- Le plant-expert doit valider les quantites du seed avant merge.
- Cette story depend de US-341 (grille hebdomadaire) pour l'affichage.

### Estimation
- **Priorite :** Optionnel
- **Points :** 8
- **Statut :** A faire
