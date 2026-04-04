## [US-340] Composant "Arrosage aujourd'hui"

**En tant que** jardinier,
**je veux** voir d'un coup d'oeil quelles plantes je dois arroser aujourd'hui des que j'ouvre le calendrier,
**afin de** ne rien oublier lors de ma session d'arrosage quotidienne.

### Criteres d'acceptation

- [ ] CA1 : Un composant `calendar-watering-today` est cree.
- [ ] CA2 : Le composant est affiche au-dessus des tabs dans la page calendrier existante, toujours visible quel que soit le tab actif.
- [ ] CA3 : Le composant affiche des badges cliquables pour chaque plante a arroser aujourd'hui, en couleur bleue eau (`#42a5f5`).
- [ ] CA4 : La determination "a arroser aujourd'hui" utilise le moteur de calcul (US-339) : le jour courant est compare aux `RecommendedDays` de chaque plante.
- [ ] CA5 : Si aucune plante n'est a arroser aujourd'hui, le composant affiche le prochain jour d'arrosage prevu (ex : "Prochain arrosage : mercredi").
- [ ] CA6 : Un endpoint API est cree (ou l'existant est enrichi) pour retourner les donnees d'arrosage du jour pour toutes les plantes de l'utilisateur.
- [ ] CA7 : L'endpoint est protege par `[Authorize]` et filtre par `UserId`.
- [ ] CA8 : Pas de N+1 : les plantes et leurs besoins en eau sont charges en une seule requete.
- [ ] CA9 : Le composant est mobile-first et fonctionne a partir de 320px.
- [ ] CA10 : Tous les textes utilisent `ngx-translate` avec des cles en PascalCase (ex : `Watering.Today`, `Watering.NextWatering`).
- [ ] CA11 : Les classes du design system sont utilisees (badges, couleurs).
- [ ] CA12 : `npm run build` passe sans erreur.

### Notes & contraintes
- Le composant est independant des tabs : il reste visible que le jardinier consulte les actions culturales ou l'arrosage.
- Le bleu eau (`#42a5f5`) distingue visuellement l'arrosage des actions culturales (vert).
- Les traductions fr et en sont ajoutees dans `public/i18n/`.
- Utiliser Angular signals pour le state management.
- Depend de US-339 (moteur de calcul) pour la logique de determination des jours d'arrosage.

### Estimation
- **Priorite :** Indispensable
- **Points :** 5
- **Statut :** A faire
