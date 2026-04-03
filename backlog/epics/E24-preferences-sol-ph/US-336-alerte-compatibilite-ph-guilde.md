## [US-336] Alerte de compatibilite pH dans l'editeur de guilde

**En tant que** jardinier confirme,
**je veux** etre alerte quand des plantes de ma guilde ont des fourchettes de pH incompatibles,
**afin de** eviter de regrouper des plantes qui ne prospereront pas dans le meme sol.

### Criteres d'acceptation

- [ ] CA1 : Quand toutes les plantes de la guilde ont un pH renseigne, le systeme calcule si les fourchettes pH se chevauchent (intersection non vide).
- [ ] CA2 : Si au moins deux plantes ont des fourchettes pH qui ne se chevauchent pas, une alerte est affichee dans le panneau de la guilde.
- [ ] CA3 : L'alerte nomme les plantes incompatibles et indique leurs fourchettes respectives (ex: "Myrtille (pH 4.5-5.5) et Chou (pH 6.5-7.5) ont des besoins en pH incompatibles").
- [ ] CA4 : L'alerte est de type "warning" (pas bloquante) — le jardinier peut choisir d'ignorer.
- [ ] CA5 : Les plantes sans pH renseigne sont ignorees dans le calcul (pas de faux positif).
- [ ] CA6 : Le texte de l'alerte est traduit via ngx-translate.
- [ ] CA7 : `npm run build --prefix garden-assistant-app` compile sans erreur.

### Notes & contraintes
- Algorithme : deux plantes sont incompatibles si `max(PhMin_A, PhMin_B) > min(PhMax_A, PhMax_B)` (pas d'intersection).
- Cette logique est purement frontend (signal computed dans le CompanionStore ou le composant de guilde).
- Coherent avec l'esprit de E15 (assistant de guilde) — pourra etre integre dans le panneau assistant plus tard.
- L'alerte ne bloque PAS la sauvegarde de la guilde.

### Estimation
- **Priorite :** Important
- **Points :** 5
- **Statut :** A faire
