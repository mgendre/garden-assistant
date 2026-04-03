## [US-336] Alerte de compatibilité pH dans l'éditeur de guilde

**En tant que** jardinier confirmé,
**je veux** être alerté quand des plantes de ma guilde ont des fourchettes de pH incompatibles,
**afin de** éviter de regrouper des plantes qui ne prospéreront pas dans le même sol.

### Critères d'acceptation

- [x] CA1 : Quand toutes les plantes de la guilde ont un pH renseigné, le système calcule si les fourchettes pH se chevauchent (intersection non vide).
- [x] CA2 : Si au moins deux plantes ont des fourchettes pH qui ne se chevauchent pas, une alerte est affichée dans le panneau de la guilde.
- [x] CA3 : L'alerte nomme les plantes incompatibles et indique leurs fourchettes respectives (ex: "Myrtille (pH 4.5-5.5) et Chou (pH 6.5-7.5) ont des besoins en pH incompatibles").
- [x] CA4 : L'alerte est de type "warning" (pas bloquante) — le jardinier peut choisir d'ignorer.
- [x] CA5 : Les plantes sans pH renseigné sont ignorées dans le calcul (pas de faux positif).
- [x] CA6 : Le texte de l'alerte est traduit via ngx-translate.
- [x] CA7 : `npm run build --prefix garden-assistant-app` compile sans erreur.

### Notes & contraintes
- Algorithme : deux plantes sont incompatibles si `max(PhMin_A, PhMin_B) > min(PhMax_A, PhMax_B)` (pas d'intersection).
- Cette logique est purement frontend (signal computed dans le CompanionStore ou le composant de guilde).
- Cohérent avec l'esprit de E15 (assistant de guilde) — pourra être intégré dans le panneau assistant plus tard.
- L'alerte ne bloque PAS la sauvegarde de la guilde.

### Estimation
- **Priorite :** Important
- **Points :** 5
- **Statut :** Termine
