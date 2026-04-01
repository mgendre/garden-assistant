## [US-200] Page d'accueil / tableau de bord

**En tant que** jardinier connecte a PermaGarden,
**je veux** disposer d'une page d'accueil unifiee qui regroupe mes jardins, les actions du moment, une plante du jour et des raccourcis vers les outils cles,
**afin de** savoir en un coup d'oeil ce que j'ai a faire aujourd'hui et naviguer rapidement vers n'importe quelle fonctionnalite de l'application.

---

### Criteres d'acceptation

#### Section 1 — Raccourcis outils (Associations & Guildes)

- [ ] CA1 : La page affiche deux cartes raccourcis : « Associations » et « Guildes ».
- [ ] CA2 : Chaque carte contient un titre, une courte description du benefice pour le jardinier (une phrase), et un lien navigant vers la route correspondante (`/companions` et `/guilds`).
- [ ] CA3 : Les cartes raccourcis sont cliquables dans leur integralite (toute la surface est un lien).

#### Section 2 — Mes jardins

- [ ] CA4 : La liste des jardins de l'utilisateur est affichee directement sur la page d'accueil, dans le meme rendu que la page `/garden` (nom du jardin, nombre de planches).
- [ ] CA5 : Un clic sur un jardin navigate vers la page detail du jardin (`/garden/:id`).
- [ ] CA6 : Si l'utilisateur n'a pas encore de jardin, un etat vide s'affiche avec un bouton « Creer mon premier jardin ».
- [ ] CA7 : Un bouton « + Nouveau jardin » est present pour les utilisateurs ayant deja des jardins.

#### Section 3 — Actions du moment (calendrier)

- [ ] CA8 : La section affiche les actions culturales du mois en cours issues du calendrier (`/calendar`), filtrees sur les plantes de « Mes plantes ».
- [ ] CA9 : Chaque action affiche au minimum : le nom de la plante, le type d'action (sowing, transplanting, harvest, etc.) et son icone, et la fenetre de realisation (demi-mois).
- [ ] CA10 : Si aucune action n'est prevue ce mois-ci, un message contextuel l'indique (ex. : « Rien de prevu ce mois-ci, profitez-en pour observer votre jardin ! »).
- [ ] CA11 : Un lien « Voir le calendrier complet » navigate vers `/calendar`.
- [ ] CA12 : Les donnees proviennent de l'endpoint batch calendrier existant (`GET /calendar/batch` ou equivalent) — aucun nouvel endpoint cote backend n'est necessaire.

#### Section 4 — Découverte du jour (plante ou guilde aléatoire)

- [ ] CA13 : Au chargement de la page, un tirage aléatoire choisit entre une **plante du jour** et une **guilde du jour** (50/50).
- [ ] CA14a : **Plante du jour** — la carte affiche : le nom de la plante, sa famille botanique, ses mécanismes principaux (badges via `PlantBadge`), et une action rapide « Voir ses compagnons » navigant vers `/companions?plant=<id>`.
- [ ] CA14b : **Guilde du jour** — la carte affiche : le nom de la guilde, sa description courte, ses plantes membres (via `PlantBadge` avec distinction centrale/compagne), et une action rapide « Explorer cette guilde » navigant vers `/companions?guild=<id>`.
- [ ] CA15 : La sélection change à chaque nouvelle visite (pas de persistance côté serveur requise — le hasard côté client suffit).
- [ ] CA16 : Si le catalogue ou les guildes ne sont pas encore chargés, un skeleton placeholder est affiché.

#### Mise en page et navigation

- [ ] CA17 : La route `/` (racine) est redirigee vers `/home` (la nouvelle page d'accueil). La redirection actuelle vers `/garden` est supprimee.
- [ ] CA18 : Le lien « Accueil » dans la navigation principale pointe vers `/home` et est actif uniquement sur cette route.
- [ ] CA19 : La page est responsive mobile-first : sur mobile (< 640 px) les sections s'empilent verticalement ; sur tablette et desktop les raccourcis et la plante du jour peuvent s'afficher en grille.
- [ ] CA20 : Toutes les chaines de caracteres visibles utilisent `ngx-translate` avec des cles PascalCase sous le namespace `Home.*` (ex. : `Home.Title`, `Home.QuickLinks.Associations.Description`).

---

### Suggestions de sections complementaires

Ces sections ne font pas partie du scope de cette story. Elles sont listees ici pour alimenter le backlog futur.

**A considerer en priorite (Should) :**
- **Meteo et conseil du jour** : afficher la meteo locale (via une API publique) avec un conseil d'action adapte (ex. : « Pluie prevue — ideal pour repiquer »). Necessite de connaitre la localisation du jardin.
- **Prochaines actions (J+7)** : elargir le widget calendrier pour montrer les 7 prochains jours, pas seulement le mois en cours. Donne un sens de l'urgence et aide a planifier.
- **Sante du sol / rappel de rotation** : alerter si une planche du meme jardin accueille la meme famille botanique deux annees de suite. Dependance : E04 (rotations).

**A considerer plus tard (Could) :**
- **Guildes recemment consultees** : afficher les 3 dernieres guildes ouvertes dans l'editeur, pour reprendre rapidement un travail en cours.
- **Fil des nouveautes** : integrer le flux `/whats-new` en format compact (les 2 dernieres entrees), avec un lien « Voir toutes les nouveautes ».
- **Statistiques rapides** : nombre de plantes dans « Mes plantes », nombre de guildes creees, nombre d'associations connues — quelques chiffres rassurants pour les debutants comme pour les utilisateurs avances.
- **Conseil permaculture du jour** : une citation ou un principe de permaculture tire d'une liste statique, pour eduquer et engager. Cout tres faible, impact pedagogique eleve.

---

### Notes & contraintes

- **Reutilisation obligatoire** : les composants de liste de jardins (E01) et le widget « Actions du moment » (US-061, E10) doivent etre extraits en composants partages si ce n'est pas encore le cas, plutot que dupliques.
- **Pas de nouvel endpoint backend** : toutes les donnees necessaires au MVP (jardins, actions calendrier, catalogue plantes) disposent deja d'endpoints. Cette story est pure frontend.
- **Performance** : les trois appels API (jardins, actions du mois, catalogue plante du jour) doivent etre lances en parallele (`Promise.all`) pour eviter une cascade de requetes.
- **Panel pattern** : chaque section utilise la classe `.panel` avec `.panel-header` / `.panel-title` conformement aux conventions du projet.
- **Ordre visuel recommande** (mobile, de haut en bas) :
  1. En-tete de bienvenue (nom de l'utilisateur, date du jour)
  2. Actions du moment — information la plus actionnable, donc en premier
  3. Mes jardins — contexte principal de l'utilisateur
  4. Raccourcis outils (Associations, Guildes) — decouverte et navigation
  5. Plante du jour — contenu educatif / inspiration, en bas de page
- **Redirection `/`** : verifier que les guards d'authentification (E16) redirigent vers `/login` avant `/home`, pas vers `/garden`.

---

### Estimation

- **Priorite :** Must
- **Points :** 8
- **Statut :** A faire

---

### Dependances

| Story | Raison |
|-------|--------|
| US-061 (E10) | Widget actions du mois — deja livre, a extraire en composant partage si besoin |
| US-001/002 (E01) | API jardins — backend fonctionnel, frontend a reconstruire |
| US-080 (E10) | Endpoint batch calendrier — livre |

> Note : cette story peut etre livree sans E01 complet en affichant un etat vide « Mes jardins » si l'API n'est pas disponible. Les sections sont independantes les unes des autres cote frontend.
