# Backlog — Garden Assistant

> **Legende priorite :** Must . Should . Could . Won't (cette version)
> **Statut :** A faire . En cours . Done

---

## E01 — Gestion de jardin avec planches

> Permettre au jardinier de creer et organiser ses espaces de culture en jardins et planches.

| ID | Titre | Priorite | Points | Statut |
|----|-------|----------|--------|--------|
| [US-001](epics/E01-gestion-jardin-planches/US-001-creer-un-jardin.md) | Creer un jardin | Must | 2 | Done |
| [US-002](epics/E01-gestion-jardin-planches/US-002-lister-mes-jardins.md) | Lister mes jardins | Must | 2 | Done |
| [US-003](epics/E01-gestion-jardin-planches/US-003-modifier-un-jardin.md) | Modifier un jardin | Must | 1 | Done |
| [US-004](epics/E01-gestion-jardin-planches/US-004-supprimer-un-jardin.md) | Supprimer un jardin | Should | 2 | Done |
| [US-005](epics/E01-gestion-jardin-planches/US-005-ajouter-une-planche.md) | Ajouter une planche a un jardin | Must | 3 | A faire |
| [US-006](epics/E01-gestion-jardin-planches/US-006-modifier-une-planche.md) | Modifier une planche | Should | 2 | A faire |
| [US-007](epics/E01-gestion-jardin-planches/US-007-supprimer-une-planche.md) | Supprimer une planche | Should | 2 | A faire |

**Total E01 : 14 points (7 Done / 7 a faire)**

---

## E02 — Association de plantes (residuel)

> Stories restantes apres la refonte E05. US-008/009/010/012 ont ete supprimes (remplaces par E05).

| ID | Titre | Priorite | Points | Statut |
|----|-------|----------|--------|--------|
| [US-011](epics/E02-association-plantes/US-011-ajouter-plante-a-planche.md) | Ajouter une plante a une planche | Must | 5 | A faire |
| [US-013](epics/E02-association-plantes/US-013-supprimer-plante-planche.md) | Retirer une plante d'une planche | Should | 3 | A faire |

**Total E02 : 8 points (0 Done / 8 a faire)**

---

## E03 — Dessiner son jardin avec un outil graphique

> Offrir un editeur visuel pour representer fidelement le terrain, positionner les planches et visualiser les cultures.

| ID | Titre | Priorite | Points | Statut |
|----|-------|----------|--------|--------|
| [US-014](epics/E03-outil-graphique/US-014-dessiner-contours-jardin.md) | Dessiner les contours de mon jardin | Must | 8 | A faire |
| [US-015](epics/E03-outil-graphique/US-015-placer-planches-sur-plan.md) | Placer les planches sur le plan | Must | 8 | A faire |
| [US-016](epics/E03-outil-graphique/US-016-visualiser-plantes-sur-plan.md) | Visualiser les plantes sur le plan | Should | 5 | A faire |
| [US-017](epics/E03-outil-graphique/US-017-ajouter-elements-fixes.md) | Ajouter des elements fixes au plan | Could | 5 | A faire |
| [US-018](epics/E03-outil-graphique/US-018-exporter-plan.md) | Exporter le plan de son jardin | Could | 3 | A faire |

**Total E03 : 29 points (0 Done / 29 a faire)**

---

## E04 — Gestion des rotations de culture

> Permettre au jardinier de suivre l'historique de ses cultures et de planifier ses rotations pour maintenir la sante du sol.

| ID | Titre | Priorite | Points | Statut |
|----|-------|----------|--------|--------|
| [US-019](epics/E04-rotations-culture/US-019-enregistrer-culture.md) | Enregistrer une culture sur une planche | Must | 3 | A faire |
| [US-020](epics/E04-rotations-culture/US-020-consulter-historique-planche.md) | Consulter l'historique de culture d'une planche | Must | 3 | A faire |
| [US-021](epics/E04-rotations-culture/US-021-planifier-rotation.md) | Planifier la rotation de la saison suivante | Must | 8 | A faire |
| [US-022](epics/E04-rotations-culture/US-022-alerte-rotation.md) | Recevoir une alerte en cas de mauvaise rotation | Should | 5 | A faire |
| [US-023](epics/E04-rotations-culture/US-023-visualiser-rotations-multi-annees.md) | Visualiser les rotations sur plusieurs annees | Should | 8 | A faire |
| [US-024](epics/E04-rotations-culture/US-024-export-historique.md) | Exporter l'historique de culture | Could | 3 | A faire |

**Total E04 : 30 points (0 Done / 30 a faire)**

---

## ~~E05 — Compagnonnage vegetal (refonte)~~ SUPPRIME

> Epic supprime. Toutes les stories (US-025 a US-032) ont ete livrees puis remplacees par la refonte UI E06. Les fichiers ont ete supprimes du backlog.

---

## E06 — Refonte UI permaculture (3 colonnes)

> Reconstruire le frontend avec un layout 3 colonnes : catalogue de plantes a gauche, fiches detail multi-selection au centre, recommandations de compagnons et guildes a droite.

| ID | Titre | Priorite | Points | Statut |
|----|-------|----------|--------|--------|
| [US-033](epics/E06-refonte-ui-permaculture/US-033-app-shell-header-navigation.md) | App shell avec header et navigation | Must | 3 | Done |
| [US-034](epics/E06-refonte-ui-permaculture/US-034-page-associations-layout-trois-colonnes.md) | Page associations layout 3 colonnes | Must | 2 | Done |
| [US-035](epics/E06-refonte-ui-permaculture/US-035-catalogue-plantes-panneau-gauche.md) | Catalogue de plantes (panneau gauche) | Must | 3 | Done |
| [US-036](epics/E06-refonte-ui-permaculture/US-036-recherche-plantes-catalogue.md) | Recherche dans le catalogue | Must | 1 | Done |
| [US-037](epics/E06-refonte-ui-permaculture/US-037-tri-catalogue-plantes.md) | Trier le catalogue de plantes | Should | 1 | Done |
| [US-038](epics/E06-refonte-ui-permaculture/US-038-selection-multi-plantes-panneau-centre.md) | Selection multi-plantes (panneau centre) | Must | 5 | Done |
| [US-039](epics/E06-refonte-ui-permaculture/US-039-panneau-compagnons-recommandations.md) | Panneau de recommandations compagnons | Must | 5 | Done |
| [US-040](epics/E06-refonte-ui-permaculture/US-040-interactions-compagnons-et-guildes.md) | Ajouter depuis compagnons et guildes | Must | 3 | Done |
| [US-041](epics/E06-refonte-ui-permaculture/US-041-etats-vides-et-accueil.md) | Etats vides et messages d'accueil | Should | 1 | Done |
| [US-042](epics/E06-refonte-ui-permaculture/US-042-responsive-mobile-tablette.md) | Adaptation responsive mobile/tablette | Should | 3 | Done |

**Total E06 : 27 points (27 Done / 0 a faire)**

---

## E07 — Mes plantes (liste personnelle)

> Permettre au jardinier de maintenir une liste personnelle de plantes qu'il cultive ou souhaite cultiver, integree au catalogue et aux recommandations de la page Associations.

| ID | Titre | Priorite | Points | Statut |
|----|-------|----------|--------|--------|
| [US-043](epics/E07-mes-plantes/US-043-api-mes-plantes.md) | API et persistance "Mes plantes" | Must | 3 | A faire |
| [US-044](epics/E07-mes-plantes/US-044-store-mes-plantes.md) | Store signal pour "Mes plantes" | Must | 2 | Done |
| [US-045](epics/E07-mes-plantes/US-045-page-mes-plantes.md) | Page "Mes plantes" avec liste et gestion | Must | 5 | Done |
| [US-046](epics/E07-mes-plantes/US-046-info-box-reusable.md) | Composant info-box reutilisable | Should | 2 | A faire |
| [US-047](epics/E07-mes-plantes/US-047-integration-catalogue-tri.md) | Prioriser "Mes plantes" dans le catalogue | Must | 3 | Done |
| [US-048](epics/E07-mes-plantes/US-048-bouton-ajouter-depuis-associations.md) | Ajouter a "Mes plantes" depuis Associations | Must | 2 | Done |

**Total E07 : 17 points (12 Done / 5 a faire)**

---

## E08 — Polish UX page Associations

> Ameliorations UX de la page Associations : popups informatifs, fiche detail plante, conteneur visuel de guilde, refactoring Sass et optimisation API.

| ID | Titre | Priorite | Points | Statut |
|----|-------|----------|--------|--------|
| [US-049](epics/E08-polish-ux-associations/US-049-badge-info-popup.md) | Popup explicatif sur les badges | Should | 3 | Done |
| [US-050](epics/E08-polish-ux-associations/US-050-plant-detail-dialog.md) | Fiche detail plante en popup | Should | 2 | Done |
| [US-051](epics/E08-polish-ux-associations/US-051-guild-container-visual.md) | Conteneur visuel de guilde | Could | 1 | Done |
| [US-052](epics/E08-polish-ux-associations/US-052-sass-7-1-refactoring.md) | Refactoring styles 7-1 Sass | Should | 3 | Done |
| [US-053](epics/E08-polish-ux-associations/US-053-limiter-catalogue-api.md) | Limiter le catalogue API a 20 resultats | Must | 1 | Done |

**Total E08 : 10 points (10 Done / 0 a faire)**

---

## Recapitulatif

| Epic | Points | Done | A faire |
|------|--------|------|---------|
| E01 — Gestion jardin & planches | 14 | 7 | 7 |
| E02 — Association de plantes (residuel) | 8 | 0 | 8 |
| E03 — Outil graphique | 29 | 0 | 29 |
| E04 — Rotations de culture | 30 | 0 | 30 |
| ~~E05 — Compagnonnage vegetal~~ | ~~19~~ | — | — |
| E06 — Refonte UI permaculture | 27 | 27 | 0 |
| E07 — Mes plantes | 17 | 12 | 5 |
| E08 — Polish UX Associations | 10 | 10 | 0 |
| **Total (actif)** | **135** | **56** | **79** |

---

*Backlog gere par l'agent Product Owner -- derniere mise a jour : 2026-03-16*
