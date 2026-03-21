## [US-099] Afficher les donnees traduites de l'API (plantes, guildes, maturite)

**En tant que** jardinier anglophone,
**je veux** que les noms et descriptions des plantes, guildes et criteres de maturite s'affichent dans ma langue,
**afin de** comprendre les informations presentees.

### Criteres d'acceptation

- [ ] CA1 : Les noms et descriptions des plantes s'affichent dans la langue selectionnee sur toutes les pages (Associations, Mes plantes, Calendrier).
- [ ] CA2 : Les noms et descriptions des guildes officielles s'affichent dans la langue selectionnee.
- [ ] CA3 : Les criteres de maturite et descriptions de recolte s'affichent dans la langue selectionnee.
- [ ] CA4 : Quand la langue change via le selecteur (US-096), les donnees sont rechargees depuis l'API avec la nouvelle langue.
- [ ] CA5 : Le fallback fonctionne : si une traduction manque, le texte francais s'affiche.
- [ ] CA6 : Pendant le rechargement des donnees apres un changement de langue, les donnees existantes restent affichees jusqu'a reception de la reponse traduite (pas de flash vide).

### Notes & contraintes
- Les stores (signals) doivent re-fetcher les donnees quand la langue change, ou ecouter un signal de changement de langue.
- Les guildes personnalisees de l'utilisateur ne sont pas traduites — elles affichent toujours leur contenu brut.

### Estimation
- **Priorite :** Indispensable
- **Points :** 3
