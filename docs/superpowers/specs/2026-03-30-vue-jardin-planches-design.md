# Vue Jardin — Planches et Associations (Option C Hybride)

**Date :** 2026-03-30
**Stories :** US-124, US-125, US-126, US-127

---

## Resume

La vue jardin affiche les planches en collapsibles. Ouvrir une planche montre le detail complet des associations, mecanismes, calendrier en **lecture seule**. L'edition des plantes se fait via redirection vers la page associations existante.

Un calendrier global du jardin est disponible, avec option de groupement par planche.

---

## Approche choisie : Option C (Hybride)

- **Lecture complete sur place** : toutes les infos visibles directement dans le collapsible de la planche
- **Edition via redirection** : bouton "Modifier" redirige vers `/companions?guild={bedGuildId}&returnTo=/garden`

### Pourquoi cette approche

| Alternative | Rejet |
|---|---|
| Option A (editeur embarque) | `CompanionStore` est un singleton complexe, inutilisable en multiple instances. Editeur complet dans un collapsible imbrique = inutilisable sur mobile |
| Option B (resume + redirect) | Trop superficiel. Le jardinier veut voir associations et calendrier sans naviguer |

---

## Architecture technique

### Decouplage du CompanionStore

La vue jardin NE DOIT PAS utiliser `CompanionStore` (singleton de la page associations). Les donnees sont chargees via `CompanionService` directement et stockees dans des signaux locaux au composant `BedPanel`.

Le seul lien avec la page associations est le bouton "Modifier" qui navigue vers `/companions?guild={bedGuildId}&returnTo=/garden`.

### Composants

**Principe cle : un seul composant shared entre guild editor et vue planche.**

**Composant shared unique :**

- `PlantAssociationPanel` (shared) — composant unique regroupant toute la vue d'associations. Affiche : barre de sante, plant cards, liste d'associations, resume mecanismes, stratification racinaire, calendrier Gantt. Toutes les sections sont collapsibles a l'interieur.
  - **Inputs** : `plants: PlantDto[]`, `associations: PlantAssociationDto[]`, `calendarEntries: PlantCalendarDto[]`, `readonly: boolean`, `centralPlantIds: Set<Guid>`
  - **Outputs** : `centralToggle: { plantId }`, `removePlant: { plantId }` (emis seulement si `readonly = false`)
  - Utilise par le guild editor (`readonly = false`) ET par la vue planche (`readonly = true`)
  - Contient : `RootStratification` (refactore pour accepter des inputs au lieu d'injecter `CompanionStore`)

**Composants feature (vue jardin) :**

- `BedList` — page container (`/garden`). Charge les planches du jardin selectionne. Rend les panneaux.
- `BedPanel` — panneau collapsible d'une planche. Charge les donnees au moment de l'expansion (lazy). Passe les donnees a `PlantAssociationPanel` avec `readonly = true`.
- `GardenCalendar` — Gantt global de toutes les plantes du jardin avec toggle groupement par planche.

**Composants reutilises (sans modification) :**

- `app-plant-card` — mode read-only (pas de `removable`, pas de `hideFavButton`)
- `app-collapsible` — pour toutes les sections imbriquees
- `app-plant-calendar-gantt` — pour le calendrier

**Refactoring du guild editor :** le guild editor existant (`guild-editor.ts`, `guild-assistant.ts`) est refactore pour deleguer l'affichage a `PlantAssociationPanel` avec `readonly = false`. Le `CompanionStore` reste le fournisseur de donnees pour le guild editor, mais les donnees sont passees au composant shared via inputs.

---

## Layout

### Mobile (320px+)

Single-column. Mode accordeon (un seul panneau ouvert a la fois).

```
+------------------------------------------+
|  Mon jardin : [Selecteur jardin]         |
+------------------------------------------+
|  [Onglet: Planches] [Onglet: Calendrier] |
+------------------------------------------+
|  [+ Ajouter une planche]                |
+------------------------------------------+
|  > Potager Sud          5 plantes    [v] |
|  +--------------------------------------+|
|  | Barre de sante                       ||
|  | [3 benefiques] [1 nefaste] [2 lacunes]|
|  |                                      ||
|  | Plant cards (collapsibles)           ||
|  | > Tomate  Solanaceae  Plein soleil   ||
|  | > Basilic Lamiaceae   Mi-ombre       ||
|  | ...                                  ||
|  |                                      ||
|  | > Associations                       ||
|  | > Assistant mecanismes               ||
|  | > Calendrier                         ||
|  |                                      ||
|  | [Modifier]                           ||
|  +--------------------------------------+|
+------------------------------------------+
|  > Aromatiques           3 plantes       |
+------------------------------------------+
|  > Courges               4 plantes       |
+------------------------------------------+
```

### Desktop (1024px+)

Single-column centree (max-width ~800px). Plusieurs panneaux ouverts simultanement.

---

## Etats

| Etat | Comportement |
|---|---|
| Jardin vide | "Aucune planche. Commencez par en creer une." + bouton ajout |
| Planche vide | "Aucune plante. Ajoutez des plantes pour voir les associations." + bouton vers associations |
| 1 plante | Plant card visible. Sections associations/assistant masquees. Note : "Ajoutez une plante pour voir les associations." |
| 2+ plantes | Toutes les sections visibles |
| Chargement | Skeleton dans le collapsible ouvert |
| Erreur | "Impossible de charger les associations. Reessayez." + bouton retry |

---

## Relation Planche-Guilde

Chaque planche est liee a une guilde (FK `guildId` sur l'entite `Planting`). Cela permet de reutiliser le pattern existant `/companions?guild=ID` pour l'edition. La guilde est creee automatiquement lors de la creation de la planche.

---

## Calendrier global du jardin (US-127)

- Onglet "Calendrier" dans la vue jardin
- Gantt de toutes les plantes de toutes les planches
- Toggle : vue a plat / groupement par planche (headers de section)
- Reutilise `PlantCalendarGantt` avec un wrapper pour le groupement

---

## Flux utilisateur

1. Page jardin → selectionner un jardin → liste des planches collapsibles
2. Ouvrir une planche → chargement lazy → detail complet read-only
3. Consulter associations, mecanismes, calendrier
4. Cliquer "Modifier" → `/companions?guild={bedGuildId}&returnTo=/garden`
5. Editer sur la page associations (catalogue, filtres, etc.)
6. Sauvegarder → retour automatique a `/garden`
7. Vue jardin rafraichie avec les nouvelles donnees
