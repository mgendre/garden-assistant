# Calendrier cultural des plantes — Design Spec

**Date :** 2026-03-20
**Epic :** E10 — Calendrier cultural des plantes (enrichi) + E12 — Adaptation climatique (future)

---

## Contexte

Le jardinier veut consulter pour chaque plante un calendrier visuel (style paquet de graines) indiquant quand semer, repiquer, recolter, tailler, etc. Il veut aussi savoir comment reconnaitre qu'un legume est pret a etre recolte (signes visuels, texture, timing).

## Decisions de design

### Modele de donnees

**Entite `PlantAction`** (table `plant_actions`) — entite separee de `Plant` :

| Champ | Type | Description |
|-------|------|-------------|
| `Id` | Guid, PK | Identifiant unique |
| `PlantId` | Guid, FK → plants | Plante concernee |
| `ActionType` | enum `PlantActionType` | Type d'action culturale |
| `HalfMonthStart` | int (1-24) | Debut de la fenetre (1=debut jan, 2=mi-jan, ..., 24=mi-dec) |
| `HalfMonthEnd` | int (1-24) | Fin de la fenetre (meme encodage) |
| `Notes` | string? (max 1000) | Note explicative |

**Enum `PlantActionType`** (8 valeurs) :

| Valeur | Label FR | Couleur |
|--------|----------|---------|
| `IndoorSowing` | Semis interieur | Violet `#8b5cf6` |
| `DirectSowing` | Semis en pleine terre | Vert `#22c55e` |
| `Transplanting` | Repiquage | Bleu `#3b82f6` |
| `Harvest` | Recolte | Orange `#f59e0b` |
| `Pruning` | Taille | Rouge `#ef4444` |
| `Pinching` | Pincage | Rose `#ec4899` |
| `Hilling` | Buttage | Indigo `#6366f1` |
| `Division` | Division | Cyan `#06b6d4` |

**Regles du modele :**
- Plusieurs lignes possibles par plante/action (double fenetre : epinard printemps + automne)
- `HalfMonthEnd < HalfMonthStart` signifie que la fenetre chevauche l'annee suivante (ex. poireau recolte demi-mois 15 → 6)
- Pas de `ClimateZone` pour l'instant — donnees calibrees sur la Suisse (plateau, ~400-600m)

**Nouveaux champs sur l'entite `Plant`** :

| Champ | Type | Description |
|-------|------|-------------|
| `PropagationMethod` | enum | `Seed` (defaut), `Bulb`, `Tuber`, `Division` |
| `FrostSensitive` | bool | `true` pour les plantes geleives (tomate, poivron, aubergine, courge, courgette, haricot, basilic, mais) |

Le frontend utilise `PropagationMethod` pour adapter les labels : "Plantation" au lieu de "Semis" pour les bulbes et tubercules.

**Entite `HarvestReadiness`** (table `harvest_readiness`) :

| Champ | Type | Description |
|-------|------|-------------|
| `Id` | Guid, PK | Identifiant unique |
| `PlantId` | Guid, FK → plants (unique) | Plante concernee |
| `Description` | string (max 2000) | Texte descriptif court (2-3 phrases) |
| `DaysFromTransplant` | int? | Jours typiques depuis repiquage |
| `DaysFromSowing` | int? | Jours typiques depuis semis |

**Entite `HarvestReadinessCriterion`** (table `harvest_readiness_criteria`) :

| Champ | Type | Description |
|-------|------|-------------|
| `Id` | Guid, PK | Identifiant unique |
| `HarvestReadinessId` | Guid, FK | Lien vers HarvestReadiness |
| `CriterionType` | enum `HarvestCriterionType` | `Visual`, `Touch`, `Timing`, `Technique` |
| `Description` | string (max 1000) | Le critere |

Tri par `CriterionType` (pas de champ SortOrder).

### API Backend

| Methode | Route | Description |
|---------|-------|-------------|
| `GET` | `/api/plants/{id}/actions` | Actions culturales d'une plante |
| `GET` | `/api/plants/{id}/harvest-readiness` | Indicateurs de maturite d'une plante |
| `GET` | `/api/calendar/my-plants` | Actions culturales batch pour "Mes plantes" |
Tous en lecture seule, `[Authorize]`, dans un `CalendarController`. Pas de CRUD admin dans cette iteration. Le filtrage "ce mois-ci" (US-061) est fait cote client a partir des donnees batch — pas d'endpoint `this-month` separe (YAGNI).

### Approche visuelle — Hybride (grille compacte + Gantt deploye)

**Page calendrier (`/calendar`)** :
- Une carte par plante avec son nom et un diagramme Gantt (une ligne par type d'action)
- Plantes triees par nom
- Widget "En ce moment / Prochainement" en haut (demi-mois courant + suivant)
- Filtres par type d'action (chips)
- Mois courant mis en evidence
- Icone gel sur les demi-mois a risque pour les plantes `FrostSensitive`
- Etat vide si "Mes plantes" est vide
- Le `PlantStore` fournit les infos plante (nom, propagationMethod, frostSensitive) — pas de duplication dans le DTO calendrier

**Fiche plante (plant-detail-dialog)** :
- Section "Calendrier cultural" avec le composant Gantt (meme composant reutilise)
- Section "Pret a recolter" avec texte descriptif + criteres structures avec icones

**Composants reutilisables** :
- `PlantCalendarGanttComponent` — vue Gantt detaillee en demi-mois
- `HarvestReadinessComponent` — indicateurs de maturite
- Popups educatives via `BadgeInfoDialog` existant

Les labels s'adaptent selon `PropagationMethod` : "Plantation" au lieu de "Semis" pour Bulbe/Tubercule.

### Popups educatives des types d'actions

Meme pattern que les BadgeInfo existants (ensoleillement, enracinement, mecanismes). Chaque type d'action a une popup expliquant :
- Ce que c'est
- Pourquoi c'est important
- Comment le faire

### Donnees de seed

- Actions culturales pour **toutes les plantes** en base (~30+), calibrees sur le climat suisse plateau
- Indicateurs de maturite pour tous les legumes et aromatiques
- `PropagationMethod` renseigne pour chaque plante
- `FrostSensitive` renseigne pour les plantes geleives
- Support des doubles fenetres (epinard, navet, ail)

### Scope futur (E12 — Adaptation climatique)

Prepare mais pas implemente :
- Parametre "date derniere gelee" sur le jardin
- Calcul des fenetres relatif a la derniere gelee
- Zones climatiques differenciees

---

## Stories

### Modifications E10

- **US-059** (mise a jour) : Enum enrichi a 8 types, pas de ClimateZone, `PropagationMethod` + `FrostSensitive` sur Plant, seed toutes les plantes
- **US-060** (mise a jour) : Style hybride grille compacte + Gantt deploye, indicateur gel, composant reutilise dans fiche plante

### Nouvelles stories E10

- **US-076** : Indicateurs de maturite — modele + seed
- **US-077** : Section "Pret a recolter" dans la fiche plante
- **US-078** : Calendrier cultural dans la fiche plante (Gantt)
- **US-079** : Popups educatives des types d'actions
- **US-080** : Endpoint batch calendrier "Mes plantes"

### Nouvelle epic E12

- **US-081** : Parametre "date derniere gelee" sur le jardin
- **US-082** : Calcul des fenetres relatives a la derniere gelee
- **US-083** : Zones climatiques differenciees
