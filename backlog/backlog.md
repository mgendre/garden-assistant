# Backlog — Garden Assistant

> **Priority legend:** Must . Should . Could . Won't (this version)
> **Status:** Todo . In Progress . Done

---

## E01 — Garden and bed management

> Allow the gardener to create and organise growing spaces as gardens and beds.

| ID | Title | Priority | Points | Status |
|----|-------|----------|--------|--------|
| [US-001](epics/E01-gestion-jardin-planches/US-001-creer-un-jardin.md) | Create a garden | Must | 2 | In Progress |
| [US-002](epics/E01-gestion-jardin-planches/US-002-lister-mes-jardins.md) | List my gardens | Must | 2 | In Progress |
| [US-003](epics/E01-gestion-jardin-planches/US-003-modifier-un-jardin.md) | Edit a garden | Must | 1 | In Progress |
| [US-004](epics/E01-gestion-jardin-planches/US-004-supprimer-un-jardin.md) | Delete a garden | Should | 2 | In Progress |
| [US-005](epics/E01-gestion-jardin-planches/US-005-ajouter-une-planche.md) | Add a bed to a garden | Must | 3 | Todo |
| [US-006](epics/E01-gestion-jardin-planches/US-006-modifier-une-planche.md) | Edit a bed | Should | 2 | Todo |
| [US-007](epics/E01-gestion-jardin-planches/US-007-supprimer-une-planche.md) | Delete a bed | Should | 2 | Todo |

**Total E01: 14 points (0 Done / 4 In Progress / 7 Todo)**

> Note: US-001 to US-004 have a working backend API but no frontend UI (no page, route, or components). Marked as In Progress.

---

## E02 — Plant associations (residual)

> Remaining stories after the E05 overhaul. US-008/009/010/012 were removed (replaced by E05).

| ID | Title | Priority | Points | Status |
|----|-------|----------|--------|--------|
| [US-011](epics/E02-association-plantes/US-011-ajouter-plante-a-planche.md) | Add a plant to a bed | Must | 5 | Todo |
| [US-013](epics/E02-association-plantes/US-013-supprimer-plante-planche.md) | Remove a plant from a bed | Should | 3 | Todo |

**Total E02: 8 points (0 Done / 8 Todo)**

---

## E03 — Visual garden editor

> Provide a visual editor to faithfully represent the terrain, position beds, and visualise crops.

| ID | Title | Priority | Points | Status |
|----|-------|----------|--------|--------|
| [US-014](epics/E03-outil-graphique/US-014-dessiner-contours-jardin.md) | Draw garden outlines | Must | 8 | Todo |
| [US-015](epics/E03-outil-graphique/US-015-placer-planches-sur-plan.md) | Place beds on the map | Must | 8 | Todo |
| [US-016](epics/E03-outil-graphique/US-016-visualiser-plantes-sur-plan.md) | Visualise plants on the map | Should | 5 | Todo |
| [US-017](epics/E03-outil-graphique/US-017-ajouter-elements-fixes.md) | Add fixed elements to the map | Could | 5 | Todo |
| [US-018](epics/E03-outil-graphique/US-018-exporter-plan.md) | Export the garden plan | Could | 3 | Todo |

**Total E03: 29 points (0 Done / 29 Todo)**

---

## E04 — Crop rotation management

> Allow the gardener to track crop history and plan rotations to maintain soil health.

| ID | Title | Priority | Points | Status |
|----|-------|----------|--------|--------|
| [US-019](epics/E04-rotations-culture/US-019-enregistrer-culture.md) | Record a crop on a bed | Must | 3 | Todo |
| [US-020](epics/E04-rotations-culture/US-020-consulter-historique-planche.md) | View crop history for a bed | Must | 3 | Todo |
| [US-021](epics/E04-rotations-culture/US-021-planifier-rotation.md) | Plan next season's rotation | Must | 8 | Todo |
| [US-022](epics/E04-rotations-culture/US-022-alerte-rotation.md) | Receive bad-rotation alerts | Should | 5 | Todo |
| [US-023](epics/E04-rotations-culture/US-023-visualiser-rotations-multi-annees.md) | Visualise multi-year rotations | Should | 8 | Todo |
| [US-024](epics/E04-rotations-culture/US-024-export-historique.md) | Export crop history | Could | 3 | Todo |

**Total E04: 30 points (0 Done / 30 Todo)**

---

## ~~E05 — Companion planting (overhaul)~~ REMOVED

> Epic removed. All stories (US-025 to US-032) were delivered then replaced by the E06 UI overhaul. Files have been deleted from the backlog.

---

## E06 — Permaculture UI overhaul (3-column layout)

> Rebuild the frontend with a 3-column layout: plant catalogue on the left, multi-select detail cards in the centre, companion and guild recommendations on the right.

| ID | Title | Priority | Points | Status |
|----|-------|----------|--------|--------|
| [US-033](epics/E06-refonte-ui-permaculture/US-033-app-shell-header-navigation.md) | App shell with header and navigation | Must | 3 | Done |
| [US-034](epics/E06-refonte-ui-permaculture/US-034-page-associations-layout-trois-colonnes.md) | Associations page 3-column layout | Must | 2 | Done |
| [US-035](epics/E06-refonte-ui-permaculture/US-035-catalogue-plantes-panneau-gauche.md) | Plant catalogue (left panel) | Must | 3 | Done |
| [US-036](epics/E06-refonte-ui-permaculture/US-036-recherche-plantes-catalogue.md) | Catalogue search | Must | 1 | Done |
| [US-037](epics/E06-refonte-ui-permaculture/US-037-tri-catalogue-plantes.md) | Sort the plant catalogue | Should | 1 | Done |
| [US-038](epics/E06-refonte-ui-permaculture/US-038-selection-multi-plantes-panneau-centre.md) | Multi-plant selection (centre panel) | Must | 5 | Done |
| [US-039](epics/E06-refonte-ui-permaculture/US-039-panneau-compagnons-recommandations.md) | Companion recommendations panel | Must | 5 | Done |
| [US-040](epics/E06-refonte-ui-permaculture/US-040-interactions-compagnons-et-guildes.md) | Add from companions and guilds | Must | 3 | Done |
| [US-041](epics/E06-refonte-ui-permaculture/US-041-etats-vides-et-accueil.md) | Empty states and welcome messages | Should | 1 | Done |
| [US-042](epics/E06-refonte-ui-permaculture/US-042-responsive-mobile-tablette.md) | Responsive mobile/tablet adaptation | Should | 3 | Done |

**Total E06: 27 points (27 Done / 0 Todo)**

---

## E07 — My Plants (personal list)

> Allow the gardener to maintain a personal list of plants they grow or wish to grow, integrated with the catalogue and the Associations page recommendations.

| ID | Title | Priority | Points | Status |
|----|-------|----------|--------|--------|
| [US-043](epics/E07-mes-plantes/US-043-api-mes-plantes.md) | "My Plants" API and persistence | Must | 3 | Done |
| [US-044](epics/E07-mes-plantes/US-044-store-mes-plantes.md) | Signal store for "My Plants" | Must | 2 | Done |
| [US-045](epics/E07-mes-plantes/US-045-page-mes-plantes.md) | "My Plants" page with list management | Must | 5 | Done |
| [US-046](epics/E07-mes-plantes/US-046-info-box-reusable.md) | Reusable info-box component | Should | 2 | Todo |
| [US-047](epics/E07-mes-plantes/US-047-integration-catalogue-tri.md) | Prioritise "My Plants" in catalogue | Must | 3 | Done |
| [US-048](epics/E07-mes-plantes/US-048-bouton-ajouter-depuis-associations.md) | Add to "My Plants" from Associations | Must | 2 | Done |

**Total E07: 17 points (15 Done / 2 Todo)**

---

## E08 — Associations page UX polish

> UX improvements for the Associations page: info popups, plant detail card, guild visual container, Sass refactoring, and API optimisation.

| ID | Title | Priority | Points | Status |
|----|-------|----------|--------|--------|
| [US-049](epics/E08-polish-ux-associations/US-049-badge-info-popup.md) | Explanatory badge popup | Should | 3 | Done |
| [US-050](epics/E08-polish-ux-associations/US-050-plant-detail-dialog.md) | Plant detail dialog | Should | 2 | Done |
| [US-051](epics/E08-polish-ux-associations/US-051-guild-container-visual.md) | Guild visual container | Could | 1 | Done |
| [US-052](epics/E08-polish-ux-associations/US-052-sass-7-1-refactoring.md) | Sass 7-1 style refactoring | Should | 3 | Done |
| [US-053](epics/E08-polish-ux-associations/US-053-limiter-catalogue-api.md) | Limit catalogue API to 20 results | Must | 1 | Todo |

**Total E08: 10 points (9 Done / 1 Todo)**

> Note: US-053 was previously marked Done but no pagination or limiting exists in the backend. Reverted to Todo.

---

## E09 — Root depth awareness

> Leverage root depth data already present on plants to improve companion planting recommendations and guild design.

| ID | Title | Priority | Points | Status |
|----|-------|----------|--------|--------|
| US-054 | Root depth badge on plant cards | Must | 1 | Done |
| US-055 | Root depth filter in catalogue | Must | 1 | Done |
| US-056 | Root stratification indicator in guild editor | Should | 3 | Todo |
| US-057 | Root depth bonus in scoring algorithm | Should | 3 | Todo |
| US-058 | Root competition warnings for same-depth plants | Could | 2 | Todo |

**Total E09: 10 points (2 Done / 8 Todo)**

---

## Summary

| Epic | Points | Done | In Progress | Todo |
|------|--------|------|-------------|------|
| E01 — Garden & bed management | 14 | 0 | 4 | 7 |
| E02 — Plant associations (residual) | 8 | 0 | 0 | 8 |
| E03 — Visual garden editor | 29 | 0 | 0 | 29 |
| E04 — Crop rotation management | 30 | 0 | 0 | 30 |
| ~~E05 — Companion planting~~ | ~~19~~ | — | — | — |
| E06 — Permaculture UI overhaul | 27 | 27 | 0 | 0 |
| E07 — My Plants | 17 | 15 | 0 | 2 |
| E08 — Associations UX polish | 10 | 9 | 0 | 1 |
| E09 — Root depth awareness | 10 | 2 | 0 | 8 |
| **Total (active)** | **145** | **53** | **4** | **85** |

---

*Backlog managed by the Product Owner agent — last updated: 2026-03-18*
