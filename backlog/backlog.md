# Backlog — Garden Assistant

> **Légende des priorités :** Indispensable · Important · Optionnel · Hors scope (cette version)
> **Statuts :** À faire · En cours · Terminé

---

## E01 — Gestion des jardins et des planches

> Permettre au jardinier de créer et d'organiser ses espaces de culture sous forme de jardins et de planches.

| ID | Titre | Priorité | Points | Statut |
|----|-------|----------|--------|--------|
| [US-001](epics/E01-gestion-jardin-planches/US-001-creer-un-jardin.md) | Créer un jardin | Indispensable | 2 | En cours |
| [US-002](epics/E01-gestion-jardin-planches/US-002-lister-mes-jardins.md) | Lister mes jardins | Indispensable | 2 | En cours |
| [US-003](epics/E01-gestion-jardin-planches/US-003-modifier-un-jardin.md) | Modifier un jardin | Indispensable | 1 | En cours |
| [US-004](epics/E01-gestion-jardin-planches/US-004-supprimer-un-jardin.md) | Supprimer un jardin | Important | 2 | En cours |
| [US-005](epics/E01-gestion-jardin-planches/US-005-ajouter-une-planche.md) | Ajouter une planche à un jardin | Indispensable | 3 | À faire |
| [US-006](epics/E01-gestion-jardin-planches/US-006-modifier-une-planche.md) | Modifier une planche | Important | 2 | À faire |
| [US-007](epics/E01-gestion-jardin-planches/US-007-supprimer-une-planche.md) | Supprimer une planche | Important | 2 | À faire |

**Total E01 : 14 points (0 Terminé / 4 En cours / 7 À faire)**

> Note : US-001 à US-004 disposent d'une API backend fonctionnelle mais sans interface frontend (aucune page, route ou composant). Marqués En cours.

---

## E02 — Associations végétales (résiduel)

> Stories restantes après la refonte E05. Les US-008/009/010/012 ont été supprimées (remplacées par E05).

| ID | Titre | Priorité | Points | Statut |
|----|-------|----------|--------|--------|
| [US-011](epics/E02-association-plantes/US-011-ajouter-plante-a-planche.md) | Ajouter une plante à une planche | Indispensable | 5 | À faire |
| [US-013](epics/E02-association-plantes/US-013-supprimer-plante-planche.md) | Retirer une plante d'une planche | Important | 3 | À faire |

**Total E02 : 8 points (0 Terminé / 8 À faire)**

---

## E03 — Éditeur graphique du jardin

> Fournir un éditeur visuel pour représenter fidèlement le terrain, positionner les planches et visualiser les cultures.

| ID | Titre | Priorité | Points | Statut |
|----|-------|----------|--------|--------|
| [US-014](epics/E03-outil-graphique/US-014-dessiner-contours-jardin.md) | Dessiner les contours du jardin | Indispensable | 8 | À faire |
| [US-015](epics/E03-outil-graphique/US-015-placer-planches-sur-plan.md) | Placer les planches sur le plan | Indispensable | 8 | À faire |
| [US-016](epics/E03-outil-graphique/US-016-visualiser-plantes-sur-plan.md) | Visualiser les plantes sur le plan | Important | 5 | À faire |
| [US-017](epics/E03-outil-graphique/US-017-ajouter-elements-fixes.md) | Ajouter des éléments fixes sur le plan | Optionnel | 5 | À faire |
| [US-018](epics/E03-outil-graphique/US-018-exporter-plan.md) | Exporter le plan du jardin | Optionnel | 3 | À faire |

**Total E03 : 29 points (0 Terminé / 29 À faire)**

---

## E04 — Gestion des rotations de cultures

> Permettre au jardinier de suivre l'historique des cultures et de planifier les rotations pour maintenir la santé du sol.

| ID | Titre | Priorité | Points | Statut |
|----|-------|----------|--------|--------|
| [US-019](epics/E04-rotations-culture/US-019-enregistrer-culture.md) | Enregistrer une culture sur une planche | Indispensable | 3 | À faire |
| [US-020](epics/E04-rotations-culture/US-020-consulter-historique-planche.md) | Consulter l'historique d'une planche | Indispensable | 3 | À faire |
| [US-021](epics/E04-rotations-culture/US-021-planifier-rotation.md) | Planifier la rotation de la saison suivante | Indispensable | 8 | À faire |
| [US-022](epics/E04-rotations-culture/US-022-alerte-rotation.md) | Recevoir des alertes de mauvaise rotation | Important | 5 | À faire |
| [US-023](epics/E04-rotations-culture/US-023-visualiser-rotations-multi-annees.md) | Visualiser les rotations sur plusieurs années | Important | 8 | À faire |
| [US-024](epics/E04-rotations-culture/US-024-export-historique.md) | Exporter l'historique des cultures | Optionnel | 3 | À faire |

**Total E04 : 30 points (0 Terminé / 30 À faire)**

---

## ~~E05 — Plantes compagnes (refonte)~~ SUPPRIMÉ

> Épique supprimée. Toutes les stories (US-025 à US-032) ont été livrées puis remplacées par la refonte UI E06. Les fichiers ont été supprimés du backlog.

---

## E06 — Refonte UI permaculture (mise en page 3 colonnes)

> Reconstruire le frontend avec une mise en page 3 colonnes : catalogue des plantes à gauche, fiches de sélection multiple au centre, recommandations compagnes et guildes à droite.

| ID | Titre | Priorité | Points | Statut |
|----|-------|----------|--------|--------|
| [US-033](epics/E06-refonte-ui-permaculture/US-033-app-shell-header-navigation.md) | App shell avec header et navigation | Indispensable | 3 | Terminé |
| [US-034](epics/E06-refonte-ui-permaculture/US-034-page-associations-layout-trois-colonnes.md) | Page Associations — mise en page 3 colonnes | Indispensable | 2 | Terminé |
| [US-035](epics/E06-refonte-ui-permaculture/US-035-catalogue-plantes-panneau-gauche.md) | Catalogue des plantes (panneau gauche) | Indispensable | 3 | Terminé |
| [US-036](epics/E06-refonte-ui-permaculture/US-036-recherche-plantes-catalogue.md) | Recherche dans le catalogue | Indispensable | 1 | Terminé |
| [US-037](epics/E06-refonte-ui-permaculture/US-037-tri-catalogue-plantes.md) | Tri du catalogue de plantes | Important | 1 | Terminé |
| [US-038](epics/E06-refonte-ui-permaculture/US-038-selection-multi-plantes-panneau-centre.md) | Sélection multi-plantes (panneau central) | Indispensable | 5 | Terminé |
| [US-039](epics/E06-refonte-ui-permaculture/US-039-panneau-compagnons-recommandations.md) | Panneau de recommandations compagnes | Indispensable | 5 | Terminé |
| [US-040](epics/E06-refonte-ui-permaculture/US-040-interactions-compagnons-et-guildes.md) | Ajouter depuis compagnons et guildes | Indispensable | 3 | Terminé |
| [US-041](epics/E06-refonte-ui-permaculture/US-041-etats-vides-et-accueil.md) | États vides et messages d'accueil | Important | 1 | Terminé |
| [US-042](epics/E06-refonte-ui-permaculture/US-042-responsive-mobile-tablette.md) | Adaptation responsive mobile/tablette | Important | 3 | Terminé |

**Total E06 : 27 points (27 Terminé / 0 À faire)**

---

## E07 — Mes plantes (liste personnelle)

> Permettre au jardinier de tenir une liste personnelle des plantes qu'il cultive ou souhaite cultiver, intégrée au catalogue et aux recommandations de la page Associations.

| ID | Titre | Priorité | Points | Statut |
|----|-------|----------|--------|--------|
| [US-043](epics/E07-mes-plantes/US-043-api-mes-plantes.md) | API et persistance « Mes plantes » | Indispensable | 3 | Terminé |
| [US-044](epics/E07-mes-plantes/US-044-store-mes-plantes.md) | Store de signaux « Mes plantes » | Indispensable | 2 | Terminé |
| [US-045](epics/E07-mes-plantes/US-045-page-mes-plantes.md) | Page « Mes plantes » avec gestion de la liste | Indispensable | 5 | Terminé |
| [US-046](epics/E07-mes-plantes/US-046-info-box-reusable.md) | Composant info-box réutilisable | Important | 2 | À faire |
| [US-047](epics/E07-mes-plantes/US-047-integration-catalogue-tri.md) | Prioriser « Mes plantes » dans le catalogue | Indispensable | 3 | Terminé |
| [US-048](epics/E07-mes-plantes/US-048-bouton-ajouter-depuis-associations.md) | Ajouter à « Mes plantes » depuis Associations | Indispensable | 2 | Terminé |

**Total E07 : 17 points (15 Terminé / 2 À faire)**

---

## E08 — Finitions UX — page Associations

> Améliorations UX de la page Associations : popups d'information, fiche détail plante, conteneur visuel de guilde, refactoring Sass et optimisation API.

| ID | Titre | Priorité | Points | Statut |
|----|-------|----------|--------|--------|
| [US-049](epics/E08-polish-ux-associations/US-049-badge-info-popup.md) | Popup d'information sur les badges | Important | 3 | Terminé |
| [US-050](epics/E08-polish-ux-associations/US-050-plant-detail-dialog.md) | Dialog détail d'une plante | Important | 2 | Terminé |
| [US-051](epics/E08-polish-ux-associations/US-051-guild-container-visual.md) | Conteneur visuel de guilde | Optionnel | 1 | Terminé |
| [US-052](epics/E08-polish-ux-associations/US-052-sass-7-1-refactoring.md) | Refactoring styles Sass 7-1 | Important | 3 | Terminé |
| [US-053](epics/E08-polish-ux-associations/US-053-limiter-catalogue-api.md) | Limiter l'API catalogue à 20 résultats | Indispensable | 1 | À faire |
| [US-065](epics/E08-polish-ux-associations/US-065-associations-manquantes.md) | Indicateur d'associations importantes manquantes | Important | 5 | À faire |

**Total E08 : 15 points (9 Terminé / 6 À faire)**

> Note : US-053 était précédemment marquée Terminé mais aucune pagination ni limitation n'existe dans le backend. Repassée À faire.

---

## E09 — Conscience de l'enracinement

> Exploiter les données de profondeur racinaire déjà présentes sur les plantes pour améliorer les recommandations de plantes compagnes et la conception des guildes.

| ID | Titre | Priorité | Points | Statut |
|----|-------|----------|--------|--------|
| [US-054](epics/E09-enracinement/US-054-badge-enracinement-fiche-plante.md) | Badge d'enracinement sur les fiches plantes | Indispensable | 1 | Terminé |
| [US-055](epics/E09-enracinement/US-055-filtre-enracinement-catalogue.md) | Filtre enracinement dans le catalogue | Indispensable | 1 | Terminé |
| [US-056](epics/E09-enracinement/US-056-indicateur-stratification-guilde.md) | Indicateur de stratification dans l'éditeur de guilde | Important | 3 | À faire |
| [US-057](epics/E09-enracinement/US-057-bonus-enracinement-algorithme-score.md) | Bonus enracinement dans l'algorithme de score | Important | 3 | À faire |
| [US-058](epics/E09-enracinement/US-058-alerte-competition-enracinement.md) | Alertes de compétition racinaire | Optionnel | 2 | À faire |

**Total E09 : 10 points (2 Terminé / 8 À faire)**

---

## E10 — Calendrier cultural des plantes

> Fournir aux jardiniers un calendrier cultural personnalisé basé sur leur liste « Mes plantes », affichant les actions clés (semis, repiquage, mise en place, récolte, taille) avec leurs fenêtres de réalisation recommandées.

| ID | Titre | Priorité | Points | Statut |
|----|-------|----------|--------|--------|
| [US-059](epics/E10-calendrier-cultural/US-059-modele-action-culturale.md) | Modèle de données des actions culturales | Indispensable | 5 | À faire |
| [US-060](epics/E10-calendrier-cultural/US-060-page-calendrier.md) | Page calendrier (grille annuelle) | Indispensable | 5 | À faire |
| [US-061](epics/E10-calendrier-cultural/US-061-widget-actions-du-mois.md) | Widget « Ce mois-ci » | Important | 3 | À faire |
| [US-062](epics/E10-calendrier-cultural/US-062-filtre-type-action-calendrier.md) | Filtre par type d'action sur le calendrier | Important | 2 | À faire |
| [US-063](epics/E10-calendrier-cultural/US-063-semis-successifs.md) | Suggestions de semis successifs | Optionnel | 5 | À faire |
| [US-064](epics/E10-calendrier-cultural/US-064-alertes-taille-pincage.md) | Alertes taille et pinçage | Important | 3 | À faire |

**Total E10 : 23 points (0 Terminé / 23 À faire)**

---

## Récapitulatif

| Épique | Points | Terminé | En cours | À faire |
|--------|--------|---------|----------|---------|
| E01 — Gestion des jardins et planches | 14 | 0 | 4 | 7 |
| E02 — Associations végétales (résiduel) | 8 | 0 | 0 | 8 |
| E03 — Éditeur graphique du jardin | 29 | 0 | 0 | 29 |
| E04 — Gestion des rotations de cultures | 30 | 0 | 0 | 30 |
| ~~E05 — Plantes compagnes (refonte)~~ | ~~19~~ | — | — | — |
| E06 — Refonte UI permaculture | 27 | 27 | 0 | 0 |
| E07 — Mes plantes | 17 | 15 | 0 | 2 |
| E08 — Finitions UX Associations | 15 | 9 | 0 | 6 |
| E09 — Conscience de l'enracinement | 10 | 2 | 0 | 8 |
| E10 — Calendrier cultural | 23 | 0 | 0 | 23 |
| **Total (actif)** | **173** | **53** | **4** | **113** |

---

*Backlog géré par l'agent Product Owner — dernière mise à jour : 2026-03-19*
