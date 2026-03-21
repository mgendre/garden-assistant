## [US-093] Fallback langue par defaut (FR) quand traduction absente

**En tant que** jardinier,
**je veux** que l'application affiche toujours un texte lisible meme si la traduction dans ma langue n'est pas disponible,
**afin de** ne jamais voir de champs vides ou d'erreurs.

### Criteres d'acceptation

- [ ] CA1 : Si une traduction n'existe pas pour la langue demandee, le service retourne la traduction dans la langue par defaut (FR).
- [ ] CA2 : Si la traduction dans la langue par defaut n'existe pas non plus, le service retourne la valeur brute du champ de l'entite.
- [ ] CA3 : Le fallback est transparent — le client ne sait pas si la valeur vient d'une traduction ou du champ brut.
- [ ] CA4 : Les tests unitaires couvrent les trois niveaux de fallback : traduction trouvee, fallback langue par defaut, fallback valeur brute.

### Notes & contraintes
- Ce comportement est implemente dans le `TranslationService` (US-086) mais cette story garantit le test exhaustif de la chaine de fallback dans un contexte d'integration (service -> API -> reponse).
- Le fallback ne doit pas generer de requetes supplementaires inutiles — optimiser les requetes batch.

### Estimation
- **Priorite :** Indispensable
- **Points :** 2
