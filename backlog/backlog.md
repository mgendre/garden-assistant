# Backlog — Garden Assistant

> **Légende des priorités :** Indispensable - Important - Optionnel - Hors scope (cette version)
> **Statuts :** À faire - En cours - Terminé

---

## E01 — Gestion des jardins et des planches ✅

> Permettre au jardinier de créer et d'organiser ses espaces de culture sous forme de jardins et de planches. La vue jardin affiche les planches en collapsibles avec détail complet des associations en lecture seule (Option C hybride). L'édition des plantes se fait via redirection vers la page associations.

| ID | Titre | Priorité | Points | Statut |
|----|-------|----------|--------|--------|
| [US-001](epics/E01-gestion-jardin-planches/US-001-créer-un-jardin.md) | Créer un jardin | Indispensable | 2 | ✅ Terminé |
| [US-002](epics/E01-gestion-jardin-planches/US-002-lister-mes-jardins.md) | Lister mes jardins | Indispensable | 2 | ✅ Terminé |
| [US-003](epics/E01-gestion-jardin-planches/US-003-modifier-un-jardin.md) | Modifier un jardin | Indispensable | 1 | ✅ Terminé |
| [US-004](epics/E01-gestion-jardin-planches/US-004-supprimer-un-jardin.md) | Supprimer un jardin | Important | 2 | ✅ Terminé |
| [US-005](epics/E01-gestion-jardin-planches/US-005-ajouter-une-planche.md) | Ajouter une planche à un jardin | Indispensable | 2 | ✅ Terminé |
| [US-006](epics/E01-gestion-jardin-planches/US-006-modifier-une-planche.md) | Modifier une planche (nom) | Important | 1 | ✅ Terminé |
| [US-007](epics/E01-gestion-jardin-planches/US-007-supprimer-une-planche.md) | Supprimer une planche | Important | 2 | ✅ Terminé |
| [US-124](epics/E01-gestion-jardin-planches/US-124-vue-jardin-liste-planches.md) | Vue jardin avec liste des planches en collapsibles | Indispensable | 3 | ✅ Terminé |
| [US-125](epics/E01-gestion-jardin-planches/US-125-détail-planche-associations-readonly.md) | Détail d'une planche avec associations en lecture seule | Indispensable | 8 | ✅ Terminé |
| [US-126](epics/E01-gestion-jardin-planches/US-126-modifier-planche-redirect-associations.md) | Modifier les plantes d'une planche via la page associations | Indispensable | 3 | ✅ Terminé |
| [US-127](epics/E01-gestion-jardin-planches/US-127-calendrier-global-jardin.md) | Calendrier global du jardin avec groupement par planches | Important | 5 | ✅ Terminé |
| [US-128](epics/E01-gestion-jardin-planches/US-128-plant-badge-composant-partagé.md) | Composant PlantBadge réutilisable | Indispensable | 1 | ✅ Terminé |
| [US-129](epics/E01-gestion-jardin-planches/US-129-creation-guilde-depuis-page-guildes.md) | Créer une guilde depuis la page Guildes | Important | 2 | ✅ Terminé |

**Total E01 : 34 points (34 Terminé / 0 En cours / 0 À faire)**

> Epic livre. Backend complet (GardenService, BedService, GardensController, BedsController, 19 tests unitaires). Frontend complet : CRUD jardins et planches, vue jardin avec planches collapsibles, détail planche en lecture seule via PlantAssociationPanel, redirection vers la page associations pour édition, calendrier global avec toggle Vue globale / Groupé par planche. Travaux transverses : composant PlantBadge partagé, mode creation de guilde, intégration des plantes de jardins dans le calendrier principal avec filtre source.

---

## E02 — Associations végétales (résiduel) ✅

> Stories restantes après la refonte E05. Les US-008/009/010/012 ont été supprimées (remplacées par E05).

| ID | Titre | Priorité | Points | Statut |
|----|-------|----------|--------|--------|
| [US-011](epics/E02-association-plantes/US-011-ajouter-plante-a-planche.md) | Ajouter une plante à une planche | Indispensable | 5 | Terminé |
| [US-013](epics/E02-association-plantes/US-013-supprimer-plante-planche.md) | Retirer une plante d'une planche | Important | 3 | Terminé |

**Total E02 : 8 points (8 Terminé / 0 À faire)**

---

## E03 — Éditeur graphique du jardin

> Fournir un éditeur visuel pour représenter fidèlement le terrain, positionner les planches et visualiser les cultures. Inclut les dimensions et formes des planches (ex-E17).

| ID | Titre | Priorité | Points | Statut |
|----|-------|----------|--------|--------|
| [US-014](epics/E03-outil-graphique/US-014-dessiner-contours-jardin.md) | Dessiner les contours du jardin | Indispensable | 8 | À faire |
| [US-015](epics/E03-outil-graphique/US-015-placer-planches-sur-plan.md) | Placer les planches sur le plan | Indispensable | 8 | À faire |
| [US-016](epics/E03-outil-graphique/US-016-visualiser-plantes-sur-plan.md) | Visualiser les plantes sur le plan | Important | 5 | À faire |
| [US-017](epics/E03-outil-graphique/US-017-ajouter-éléments-fixes.md) | Ajouter des éléments fixes sur le plan | Optionnel | 5 | À faire |
| [US-018](epics/E03-outil-graphique/US-018-exporter-plan.md) | Exporter le plan du jardin | Optionnel | 3 | À faire |
| [US-121](epics/E03-outil-graphique/US-121-definir-dimensions-planche.md) | Définir les dimensions d'une planche | Indispensable | 3 | À faire |
| [US-122](epics/E03-outil-graphique/US-122-modifier-dimensions-planche.md) | Modifier les dimensions d'une planche | Important | 2 | À faire |
| [US-123](epics/E03-outil-graphique/US-123-forme-planche.md) | Choisir la forme d'une planche | Important | 5 | À faire |

**Total E03 : 39 points (0 Terminé / 39 À faire)**

> Note : US-121/122/123 proviennent de l'ex-E17 (dimensions et formes des planches), fusionné dans E03 car les dimensions et formes n'ont de sens que dans le contexte de l'éditeur graphique.

---

## E04 — Gestion des rotations de cultures

> Permettre au jardinier de suivre l'historique des cultures et de planifier les rotations pour maintenir la santé du sol.

| ID | Titre | Priorité | Points | Statut |
|----|-------|----------|--------|--------|
| [US-019](epics/E04-rotations-culture/US-019-enregistrer-culture.md) | Enregistrer une culture sur une planche | Indispensable | 3 | À faire |
| [US-020](epics/E04-rotations-culture/US-020-consulter-historique-planche.md) | Consulter l'historique d'une planche | Indispensable | 3 | À faire |
| [US-021](epics/E04-rotations-culture/US-021-planifier-rotation.md) | Planifier la rotation de la saison suivante | Indispensable | 8 | À faire |
| [US-022](epics/E04-rotations-culture/US-022-alerte-rotation.md) | Recevoir des alertes de mauvaise rotation | Important | 5 | À faire |
| [US-023](epics/E04-rotations-culture/US-023-visualiser-rotations-multi-années.md) | Visualiser les rotations sur plusieurs années | Important | 8 | À faire |
| [US-024](epics/E04-rotations-culture/US-024-export-historique.md) | Exporter l'historique des cultures | Optionnel | 3 | À faire |

**Total E04 : 30 points (0 Terminé / 30 À faire)**

---

## ~~E05 — Plantes compagnes (refonte)~~ SUPPRIME

> Épique supprimée. Toutes les stories (US-025 à US-032) ont été livrées puis remplacées par la refonte UI E06. Les fichiers ont été supprimés du backlog.

---

## E06 — Refonte UI permaculture (mise en page initiale) ✅

> Reconstruire le frontend avec un catalogue de plantes, une sélection multi-plantes, et des recommandations. Layout initial à 3 colonnes, évolué vers 2 colonnes dans E11.

| ID | Titre | Priorité | Points | Statut |
|----|-------|----------|--------|--------|
| [US-033](epics/E06-refonte-ui-permaculture/US-033-app-shell-header-navigation.md) | App shell avec header et navigation | Indispensable | 3 | Terminé |
| [US-034](epics/E06-refonte-ui-permaculture/US-034-page-associations-layout-trois-colonnes.md) | Page Associations — mise en page 3 colonnes | Indispensable | 2 | Terminé |
| [US-035](epics/E06-refonte-ui-permaculture/US-035-catalogue-plantes-panneau-gauche.md) | Catalogue des plantes (panneau gauche) | Indispensable | 3 | Terminé |
| [US-036](epics/E06-refonte-ui-permaculture/US-036-recherche-plantes-catalogue.md) | Recherche dans le catalogue | Indispensable | 1 | Terminé |
| [US-037](epics/E06-refonte-ui-permaculture/US-037-tri-catalogue-plantes.md) | Tri du catalogue de plantes | Important | 1 | Terminé |
| [US-038](epics/E06-refonte-ui-permaculture/US-038-sélection-multi-plantes-panneau-centre.md) | sélection multi-plantes (panneau central) | Indispensable | 5 | Terminé |
| [US-039](epics/E06-refonte-ui-permaculture/US-039-panneau-compagnons-recommandations.md) | Panneau de recommandations compagnes | Indispensable | 5 | Terminé |
| [US-040](epics/E06-refonte-ui-permaculture/US-040-interactions-compagnons-et-guildes.md) | Ajouter depuis compagnons et guildes | Indispensable | 3 | Terminé |
| [US-041](epics/E06-refonte-ui-permaculture/US-041-etats-vides-et-accueil.md) | États vides et messages d'accueil | Important | 1 | Terminé |
| [US-042](epics/E06-refonte-ui-permaculture/US-042-responsive-mobile-tablette.md) | Adaptation responsive mobile/tablette | Important | 3 | Terminé |

**Total E06 : 27 points (27 Terminé / 0 À faire)**

> Note : Le layout 3 colonnes (US-034) à évolué vers 2 colonnes dans US-072 (E11). US-039 (panneau de recommandations) à été integre dans l'éditeur de guilde (E11).

---

## E07 — Mes plantes (liste personnelle) ✅

> Permettre au jardinier de tenir une liste personnelle des plantes qu'il cultive ou souhaite cultiver, integree au catalogue et aux recommandations de la page Associations.

| ID | Titre | Priorité | Points | Statut |
|----|-------|----------|--------|--------|
| [US-043](epics/E07-mes-plantes/US-043-api-mes-plantes.md) | API et persistance « Mes plantes » | Indispensable | 3 | Terminé |
| [US-044](epics/E07-mes-plantes/US-044-store-mes-plantes.md) | Store de signaux « Mes plantes » | Indispensable | 2 | Terminé |
| [US-045](epics/E07-mes-plantes/US-045-page-mes-plantes.md) | Page « Mes plantes » avec gestion de la liste | Indispensable | 5 | Terminé |
| ~~US-046~~ | ~~Composant info-box réutilisable~~ | ~~Important~~ | ~~2~~ | Supprime |
| [US-047](epics/E07-mes-plantes/US-047-intégration-catalogue-tri.md) | Prioriser « Mes plantes » dans le catalogue | Indispensable | 3 | Terminé |
| [US-048](epics/E07-mes-plantes/US-048-bouton-ajouter-depuis-associations.md) | Ajouter à « Mes plantes » depuis Associations | Indispensable | 2 | Terminé |

**Total E07 : 15 points (15 Terminé / 0 À faire)**

---

## E08 — Finitions UX — page Associations ✅

> Ameliorations UX de la page Associations : popups d'information, fiche détail plante, conteneur visuel de guilde, refactoring Sass et optimisation API.

| ID | Titre | Priorité | Points | Statut |
|----|-------|----------|--------|--------|
| [US-049](epics/E08-polish-ux-associations/US-049-badge-info-popup.md) | Popup d'information sur les badges | Important | 3 | Terminé |
| [US-050](epics/E08-polish-ux-associations/US-050-plant-détail-dialog.md) | Dialog détail d'une plante | Important | 2 | Terminé |
| [US-051](epics/E08-polish-ux-associations/US-051-guild-container-visual.md) | Conteneur visuel de guilde | Optionnel | 1 | Terminé |
| [US-052](epics/E08-polish-ux-associations/US-052-sass-7-1-refactoring.md) | Refactoring styles Sass 7-1 | Important | 3 | Terminé |
| ~~US-053~~ | ~~Limiter l'API catalogue à 20 resultats~~ | ~~Indispensable~~ | ~~1~~ | Abandonne |
| [US-065](epics/E08-polish-ux-associations/US-065-associations-manquantes.md) | Indicateur d'associations importantes manquantes | Important | 5 | Terminé |

**Total E08 : 14 points (14 Terminé / 0 À faire)**

> Note : US-053 abandonnee — la limite de 20 plantes dans le catalogue à été supprimée. Le catalogue affiche maintenant toutes les plantes avec une scrollbar (max-height 70vh).

---

## E09 — Conscience de l'enracinement ✅

> Exploiter les données de profondeur racinaire deja presentes sur les plantes pour ameliorer les recommandations de plantes compagnes et la conception des guildes.

| ID | Titre | Priorité | Points | Statut |
|----|-------|----------|--------|--------|
| [US-054](epics/E09-enracinement/US-054-badge-enracinement-fiche-plante.md) | Badge d'enracinement sur les fiches plantes | Indispensable | 1 | Terminé |
| [US-055](epics/E09-enracinement/US-055-filtre-enracinement-catalogue.md) | Filtre enracinement dans le catalogue | Indispensable | 1 | Terminé |
| [US-056](epics/E09-enracinement/US-056-indicateur-stratification-guilde.md) | Indicateur de stratification dans l'éditeur de guilde | Important | 3 | Terminé |
| [US-057](epics/E09-enracinement/US-057-bonus-enracinement-algorithme-score.md) | Bonus enracinement dans l'algorithme de score | Important | 3 | Terminé |
| [US-058](epics/E09-enracinement/US-058-alerte-competition-enracinement.md) | Alertes de densite racinaire par zone | Optionnel | 1 | Terminé |

**Total E09 : 9 points (9 Terminé / 0 À faire)**

> Note : US-056 livree avec une approche colonnes (au lieu de coupe SVG). US-058 simplifiee : avertissement de densite par zone au lieu de badges par paire. US-057 reste à faire (bonus algorithmique cote backend).

---

## E10 — Calendrier cultural des plantes

> Fournir aux jardiniers un calendrier cultural personnalise base sur leur liste « Mes plantes », affichant les actions cles (semis, repiquage, récolte, taille, pincage, buttage, division) avec leurs fenetres de realisation en demi-mois. Vue Gantt par plante avec indicateurs de maturite.

| ID | Titre | Priorité | Points | Statut |
|----|-------|----------|--------|--------|
| [US-059](epics/E10-calendrier-cultural/US-059-modele-action-culturale.md) | Modele de données des actions culturales | Indispensable | 5 | Terminé |
| [US-060](epics/E10-calendrier-cultural/US-060-page-calendrier.md) | Page calendrier (vue Gantt par plante) | Indispensable | 5 | Terminé |
| [US-061](epics/E10-calendrier-cultural/US-061-widget-actions-du-mois.md) | Widget « En ce moment / Prochainement » | Important | 3 | Terminé |
| [US-062](epics/E10-calendrier-cultural/US-062-filtre-type-action-calendrier.md) | Filtre par type d'action sur le calendrier | Important | 2 | Terminé |
| [US-063](epics/E10-calendrier-cultural/US-063-semis-successifs.md) | Suggestions de semis successifs | Optionnel | 5 | À faire |
| [US-064](epics/E10-calendrier-cultural/US-064-alertes-taille-pincage.md) | Alertes taille et pincage | Important | 3 | Terminé |
| [US-076](epics/E10-calendrier-cultural/US-076-indicateurs-maturite-modele.md) | Indicateurs de maturite — modele et seed | Indispensable | 5 | Terminé |
| [US-077](epics/E10-calendrier-cultural/US-077-section-pret-a-récolter.md) | Popup « Pret à récolter » depuis le calendrier | Indispensable | 3 | Terminé |
| [US-078](epics/E10-calendrier-cultural/US-078-calendrier-fiche-plante.md) | Calendrier cultural dans la fiche plante | Indispensable | 3 | Terminé |
| [US-079](epics/E10-calendrier-cultural/US-079-popups-educatives-actions.md) | Popups educatives des types d'actions | Important | 2 | Terminé |
| [US-080](epics/E10-calendrier-cultural/US-080-endpoint-batch-calendrier.md) | Endpoint batch calendrier « Mes plantes » | Indispensable | 2 | Terminé |
| [US-101](epics/E10-calendrier-cultural/US-101-fiches-techniques-actions.md) | Fiches techniques par action culturale et par plante | Important | 8 | À faire |
| [US-337](epics/E10-calendrier-cultural/US-337-périodes-multiples-actions-seed.md) | périodes multiples de semis, repiquage et récolte dans le seed | Indispensable | 3 | Terminé |
| [US-338](epics/E10-calendrier-cultural/US-338-clic-barre-gantt-modal-action.md) | Clic sur barre Gantt → modal explicatif de l'action | Important | 3 | Terminé |

**Total E10 : 52 points (39 Terminé / 13 À faire)**

> Livre le 2026-03-20. US-063 (semis successifs) reste à faire. Calendrier integre dans la page dediee, la fiche plante (plant-card collapsible), et le panneau associations/guildes. Indicateurs de maturite accessibles via popup depuis la ligne récolte du Gantt. Widget demi-mois « En ce moment / Prochainement » en 2 colonnes. Filtre single-select par type d'action avec tri par date. Bouton favori deplace en bas du popup détail.

---

## E11 — Guildes, mecanismes et refonte éditeur ✅

> Enrichir l'éditeur de guilde avec les mecanismes intrinseques et relationnels, les avertissements de conflit, le détail des associations, et elargir la base de données à 50 guildes et 356 associations. Simplifier le layout en 2 colonnes et nettoyer le backend. Ajouter la notion de plante centrale dans les guildes.

| ID | Titre | Priorité | Points | Statut |
|----|-------|----------|--------|--------|
| [US-066](epics/E11-guildes-et-mecanismes/US-066-systeme-mecanismes-intrinseques.md) | Systeme de mecanismes intrinseques des plantes | Indispensable | 5 | Terminé |
| [US-067](epics/E11-guildes-et-mecanismes/US-067-filtres-mecanismes-catalogue.md) | Filtres par mecanisme dans le catalogue | Important | 2 | Terminé |
| [US-068](epics/E11-guildes-et-mecanismes/US-068-avertissements-conflits-guilde.md) | Avertissements de conflits dans la guilde | Indispensable | 2 | Terminé |
| [US-069](epics/E11-guildes-et-mecanismes/US-069-détails-associations-guilde.md) | détails des associations dans la guilde | Important | 2 | Terminé |
| [US-070](epics/E11-guildes-et-mecanismes/US-070-resume-mecanismes-guilde.md) | Resume des mecanismes de la guilde | Important | 1 | Terminé |
| [US-071](epics/E11-guildes-et-mecanismes/US-071-panneau-guildes-permanent.md) | Panneau des guildes toujours visible | Important | 2 | Terminé |
| [US-072](epics/E11-guildes-et-mecanismes/US-072-layout-deux-colonnes.md) | Layout deux colonnes (catalogue + éditeur) | Indispensable | 2 | Terminé |
| [US-073](epics/E11-guildes-et-mecanismes/US-073-50-guildes-officielles.md) | 50 guildes officielles de permaculture | Indispensable | 5 | Terminé |
| [US-074](epics/E11-guildes-et-mecanismes/US-074-356-associations.md) | 356 associations végétales documentees | Indispensable | 3 | Terminé |
| [US-075](epics/E11-guildes-et-mecanismes/US-075-nettoyage-backend.md) | Nettoyage backend (controleurs et DTOs) | Important | 2 | Terminé |
| [US-128](epics/E11-guildes-et-mecanismes/US-128-role-plante-guilde-backend.md) | Role de la plante dans la guilde (backend) | Indispensable | 5 | Terminé |
| [US-129](epics/E11-guildes-et-mecanismes/US-129-role-central-seed-guildes.md) | Role central et completude associations dans les guildes | Indispensable | 8 | Terminé |
| [US-130](epics/E11-guildes-et-mecanismes/US-130-indicateur-visuel-role-central.md) | Indicateur visuel du role central dans l'éditeur | Important | 5 | Terminé |

**Total E11 : 44 points (44 Terminé / 0 À faire)**

> Note : US-128 et US-130 livres le 2026-03-30. Inclut : enum GuildPlantRole (Central/Companion), migration EF, API mise à jour, etoile doree toggle dans le plant card, tri des plantes centrales en premier, bordure doree sur les compagnons de la plante centrale dans le catalogue, composant shared PlantAssociationPanel (associations, mecanismes, stratification, calendrier) integre dans le guild editor. API enrichie avec LinkedPlantIds sur CompanionRecommendationDto. US-129 reste à faire : les 3 premieres guildes ont des roles, 47 restantes + completude associations à valider avec le plant-expert.

---

## E12 — Adaptation climatique

> Permettre au jardinier d'adapter le calendrier cultural à son microclimat en renseignant la date de derniere gelée et en sélectionnant une zone climatique. Les fenetres de semis et repiquage sont recalculees en consequence.

| ID | Titre | Priorité | Points | Statut |
|----|-------|----------|--------|--------|
| [US-081](epics/E12-adaptation-climatique/US-081-date-derniere-gelée.md) | Parametre « date derniere gelée » sur le jardin | Indispensable | 3 | À faire |
| [US-082](epics/E12-adaptation-climatique/US-082-fenetres-relatives-gelée.md) | Calcul des fenetres relatives à la derniere gelée | Indispensable | 5 | À faire |
| [US-083](epics/E12-adaptation-climatique/US-083-zones-climatiques.md) | Zones climatiques avec données differenciees | Important | 8 | À faire |

**Total E12 : 16 points (0 Terminé / 16 À faire)**

---

## E13 — Infrastructure i18n (backend)

> Mettre en place l'infrastructure de traduction en base de données avec une table generique, migrer les données de seed existantes, et exposer les traductions via l'API. Architecture ouverte : francais (défaut) + anglais initialement, extensible à toute langue.

| ID | Titre | Priorité | Points | Statut |
|----|-------|----------|--------|--------|
| [US-084](epics/E13-infrastructure-i18n/US-084-table-languages.md) | Table `languages` et seed FR + EN | Indispensable | 2 | À faire |
| [US-085](epics/E13-infrastructure-i18n/US-085-table-translations.md) | Table generique `translations` | Indispensable | 3 | À faire |
| [US-086](epics/E13-infrastructure-i18n/US-086-service-traduction.md) | Service de traduction (CRUD + resolution par langue) | Indispensable | 5 | À faire |
| [US-087](epics/E13-infrastructure-i18n/US-087-seed-plantes-fr.md) | Migrer les seed plantes FR dans `translations` | Indispensable | 3 | À faire |
| [US-088](epics/E13-infrastructure-i18n/US-088-seed-guildes-fr.md) | Migrer les seed guildes FR dans `translations` | Indispensable | 2 | À faire |
| [US-089](epics/E13-infrastructure-i18n/US-089-seed-maturite-fr.md) | Migrer les seed maturite/criteres FR dans `translations` | Important | 2 | À faire |
| [US-101](epics/E13-infrastructure-i18n/US-101-seed-associations-fr.md) | Migrer les seed associations FR dans `translations` | Indispensable | 2 | À faire |
| [US-102](epics/E13-infrastructure-i18n/US-102-seed-actions-culturales-fr.md) | Migrer les seed actions culturales FR dans `translations` | Important | 2 | À faire |
| [US-090](epics/E13-infrastructure-i18n/US-090-seed-plantes-en.md) | Seed des traductions EN pour les plantes | Indispensable | 5 | À faire |
| [US-091](epics/E13-infrastructure-i18n/US-091-seed-guildes-maturite-en.md) | Seed des traductions EN pour guildes, maturite, associations et actions | Important | 5 | À faire |
| [US-092](epics/E13-infrastructure-i18n/US-092-api-accept-language.md) | API: header `Accept-Language` et resolution dans les endpoints existants | Indispensable | 5 | À faire |
| [US-093](epics/E13-infrastructure-i18n/US-093-fallback-langue-défaut.md) | Fallback langue par défaut (FR) quand traduction absente | Indispensable | 2 | À faire |

**Total E13 : 38 points (0 Terminé / 38 À faire)**

---

## E14 — UX i18n (frontend)

> Ajouter le support multilingue cote frontend : fichier de traductions anglais, selecteur de langue, persistance du choix, et nettoyage du fichier francais existant. Depend de E13 pour les données traduites de l'API.

| ID | Titre | Priorité | Points | Statut |
|----|-------|----------|--------|--------|
| [US-094](epics/E14-ux-i18n/US-094-audit-nettoyage-fr-json.md) | Audit et nettoyage de `fr.json` | Important | 2 | À faire |
| [US-095](epics/E14-ux-i18n/US-095-créer-en-json.md) | créer `en.json` avec toutes les traductions UI en anglais | Indispensable | 3 | À faire |
| [US-096](epics/E14-ux-i18n/US-096-selecteur-langue.md) | Selecteur de langue dans le shell (header) | Indispensable | 3 | À faire |
| [US-097](epics/E14-ux-i18n/US-097-persistance-langue.md) | Persistance du choix de langue (localStorage) | Indispensable | 1 | À faire |
| [US-098](epics/E14-ux-i18n/US-098-intercepteur-accept-language.md) | Envoyer `Accept-Language` dans tous les appels API | Indispensable | 2 | À faire |
| [US-099](epics/E14-ux-i18n/US-099-afficher-données-traduites.md) | Afficher les données traduites de l'API (plantes, guildes, maturite) | Indispensable | 3 | À faire |
| [US-100](epics/E14-ux-i18n/US-100-contenu-utilisateur-non-traduit.md) | Gestion du contenu utilisateur non traduit (guildes personnalisees, notes) | Important | 2 | À faire |

**Total E14 : 16 points (0 Terminé / 16 À faire)**

> Note : E14 depend de E13 — le selecteur de langue et l'intercepteur HTTP n'ont de sens qu'avec l'infrastructure backend en place. US-094 et US-095 peuvent etre demarrees en parallele de E13.

---

## E15 — Assistant de creation de guilde ✅

> Guider le jardinier pas à pas dans la composition d'une guilde equilibree : analyse des lacunes (mecanismes manquants, couches racinaires vides, conflits), texte éducatif, et filtres cliquables pour trouver rapidement les plantes complementaires.

| ID | Titre | Priorité | Points | Statut |
|----|-------|----------|--------|--------|
| [US-103](epics/E15-assistant-creation-guilde/US-103-analyse-lacunes-mecanismes.md) | Analyse des lacunes de mecanismes dans la guilde | Indispensable | 5 | Terminé |
| [US-104](epics/E15-assistant-creation-guilde/US-104-lacunes-stratification-racinaire.md) | Lacunes de stratification racinaire dans l'assistant | Indispensable | 3 | Terminé |
| [US-105](epics/E15-assistant-creation-guilde/US-105-texte-éducatif-mecanismes-cles.md) | Texte éducatif sur les mecanismes cles d'une bonne guilde | Important | 2 | Terminé |
| [US-106](epics/E15-assistant-creation-guilde/US-106-panneau-assistant-guilde.md) | Panneau assistant dans l'éditeur de guilde | Indispensable | 3 | Terminé |
| [US-107](epics/E15-assistant-creation-guilde/US-107-alerte-associations-néfastes-assistant.md) | Alerte associations néfastes dans l'assistant | Important (MVP) | 2 | Terminé |
| [US-108](epics/E15-assistant-creation-guilde/US-108-indicateur-santé-guilde.md) | Indicateur de santé de la guilde | Optionnel | 2 | Terminé |
| [US-109](epics/E15-assistant-creation-guilde/US-109-alerte-diversite-familles-botaniques.md) | Alerte diversite des familles botaniques | Important | 2 | Terminé |

**Total E15 : 19 points (19 Terminé / 0 À faire)**

> Note : Aucune dependance backend — toute la logique s'appuie sur les signaux et DTOs existants dans le `CompanionStore`. L'ordre de livraison recommande est : US-106 (conteneur) en parallele de US-103 + US-104 (logique), puis US-105, US-107 et US-109 (enrichissement), et enfin US-108 (optionnel). US-107 est confirme pour le MVP.

---

## E16 — Authentification OAuth (Google & Discord)

> Ajouter l'authentification sociale via Google et Discord en utilisant le flux Authorization Code. Auto-enregistrement au premier login, consentement email, cross-provider linking, et restriction du dev-token à l'environnement de developpement.

| ID | Titre | Priorité | Points | Statut |
|----|-------|----------|--------|--------|
| [US-110](epics/E16-oauth-login/US-110-migration-user-external-login.md) | Migration BDD : User modifie et ExternalLogin | Indispensable | 3 | À faire |
| [US-111](epics/E16-oauth-login/US-111-middleware-oauth-google-discord.md) | Middleware OAuth Google et Discord | Indispensable | 2 | À faire |
| [US-112](epics/E16-oauth-login/US-112-backend-auth-flow.md) | Flux OAuth backend (login, callback, complété) | Indispensable | 8 | À faire |
| [US-113](epics/E16-oauth-login/US-113-backend-profile-email-consent.md) | Endpoint profil : toggle consentement email | Indispensable | 3 | À faire |
| [US-114](epics/E16-oauth-login/US-114-dev-token-environment-gating.md) | Restriction dev-token à l'environnement dev | Indispensable | 1 | À faire |
| [US-115](epics/E16-oauth-login/US-115-frontend-login-page.md) | Page de login frontend | Indispensable | 3 | À faire |
| [US-116](epics/E16-oauth-login/US-116-frontend-auth-callback.md) | Callback OAuth et consentement email | Indispensable | 5 | À faire |
| [US-117](epics/E16-oauth-login/US-117-frontend-auth-guard-startup.md) | Auth guard et flux de demarrage | Indispensable | 3 | À faire |
| [US-118](epics/E16-oauth-login/US-118-frontend-profile-email-consent.md) | Toggle consentement email dans le profil | Important | 3 | À faire |
| [US-119](epics/E16-oauth-login/US-119-tests-unitaires-services-oauth.md) | Tests unitaires services OAuth | Indispensable | 5 | À faire |
| [US-120](epics/E16-oauth-login/US-120-tests-intégration-oauth.md) | Tests d'intégration OAuth | Indispensable | 5 | À faire |

**Total E16 : 41 points (0 Terminé / 41 À faire)**

> Ordre de livraison : US-110 (migration) → US-111 (middleware) → US-112 (flux backend) → US-113 (profil) et US-114 (dev-token) en parallele → US-115 (login page) → US-116 (callback) → US-117 (guard) → US-118 (profil frontend) → US-119 et US-120 (tests). US-114 peut etre livree à tout moment (aucune dependance). Spec de reference : `docs/superpowers/specs/2026-03-22-oauth-login-design.md`.

---

## ~~E17 — Dimensions et formes des planches~~ FUSIONNE DANS E03

> Épique fusionnée dans E03 (éditeur graphique). Les stories US-121/122/123 sont desormais dans E03.

---

## E18 — Page d'accueil / tableau de bord

> Offrir au jardinier une page d'accueil unifiee (`/home`) regroupant les actions du moment, la liste de ses jardins, des raccourcis vers les outils cles (Associations, Guildes), et une plante du jour. La route `/` redirige desormais vers `/home`.

| ID | Titre | Priorité | Points | Statut |
|----|-------|----------|--------|--------|
| [US-200](epics/E17-page-accueil/US-200-page-accueil.md) | Page d'accueil avec jardins, actions, raccourcis et plante du jour | Indispensable | 8 | À faire |
| [US-201](epics/E17-page-accueil/US-201-conseils-saisonniers.md) | Conseils permaculturels saisonniers par demi-mois | Important | 5 | À faire |

**Total E18 : 13 points (0 Terminé / 13 À faire)**

---

## E19 — Dette technique frontend ✅

> Factoriser le code frontend pour eliminer les duplications identifiees : dialogs, etats vides, toggles, utilitaires de tri, banniere info, et couleurs CSS non tokenisees. Aucun changement visible pour le jardinier — benefice exclusivement pour la maintenabilite et la coherence du code.

| ID | Titre | Priorité | Points | Statut |
|----|-------|----------|--------|--------|
| [US-300](epics/E19-dette-technique/US-300-dialog-service.md) | DialogService — Centraliser l'ouverture des dialogs | Indispensable | 3 | ✅ Terminé |
| [US-301](epics/E19-dette-technique/US-301-composant-empty-state.md) | Composant `<app-empty-state>` réutilisable | Indispensable | 2 | ✅ Terminé |
| [US-302](epics/E19-dette-technique/US-302-composant-toggle-group.md) | Composant `<app-toggle-group>` réutilisable | Important | 2 | Terminé |
| [US-303](epics/E19-dette-technique/US-303-utilitaires-tri-partagés.md) | Extraire les utilitaires de tri partagés | Important | 1 | ✅ Terminé |
| [US-304](epics/E19-dette-technique/US-304-composant-info-banner.md) | Composant `<app-info-banner>` réutilisable | Optionnel | 1 | ✅ Terminé |
| [US-305](epics/E19-dette-technique/US-305-refactoring-css-scss.md) | Refactoring CSS/SCSS — Variables et factorisation | Important | 3 | Terminé |

**Total E19 : 12 points (12 Terminé / 0 À faire)**

> Restant : US-302 (toggle group, 2 pts) et US-305 (refactoring CSS, 3 pts).

---

## E20 — Hierarchie espece-variété

> Ajouter une relation parent/enfant (espece → variété) sur l'entite Plant. Les variétés heritent de la taxonomie, des mecanismes intrinseques et des associations de leur espece parente. Heritage resolu cote serveur — le frontend recoit des données complétés. Scope MVP : modele + migration + seed data + service + API. Pas d'UI de gestion des variétés.

| ID | Titre | Priorité | Points | Statut |
|----|-------|----------|--------|--------|
| [US-306](epics/E20-hierarchie-espece-variété/US-306-parent-plant-id-entity-migration.md) | ParentPlantId sur l'entite Plant + migration EF | Indispensable | 3 | Terminé |
| [US-307](epics/E20-hierarchie-espece-variété/US-307-seed-import-parent-key.md) | Support du champ parentKey dans le seed import | Indispensable | 3 | Terminé |
| [US-308](epics/E20-hierarchie-espece-variété/US-308-seed-data-taxonomie-variétés.md) | Correction taxonomique et ajout des variétés dans les seeds | Indispensable | 5 | Terminé |
| [US-309](epics/E20-hierarchie-espece-variété/US-309-service-heritage-variété.md) | Heritage des propriétés et mecanismes au niveau service | Indispensable | 5 | Terminé |
| [US-310](epics/E20-hierarchie-espece-variété/US-310-heritage-associations-variété.md) | Heritage des associations au niveau service | Indispensable | 5 | Terminé |
| [US-311](epics/E20-hierarchie-espece-variété/US-311-api-dto-variété.md) | PlantDto enrichi pour les variétés | Indispensable | 3 | Terminé |
| [US-329](epics/E20-hierarchie-espece-variété/US-329-noms-alternatifs-plantes.md) | Noms alternatifs des plantes | Important | 3 | À faire |

**Total E20 : 27 points (24 Terminé / 3 À faire)**

> Ordre de livraison : US-306 (migration) → US-307 (seed import) → US-308 (seed data) → US-309 (heritage propriétés) et US-310 (heritage associations) en parallele → US-311 (API/DTO). US-308 necessite validation botanique par le plant-expert avant merge.

---

## E21 — Upsert des seeds et protection des données ✅

> Transformer les seeders de "insert if empty" en "upsert by Key" avec protection des plantes personnalisees (`IsCustomized`). Ajouter les champs `Key`, `IsCustomized` et `UserId` sur l'entite Plant. Chaque seeder (plantes, associations, actions, maturite, guildes, mecanismes) est converti individuellement. Spec de reference : `docs/superpowers/specs/2026-04-02-import-management-design.md`.

| ID | Titre | Priorité | Points | Statut |
|----|-------|----------|--------|--------|
| [US-312](epics/E21-upsert-seeds-protection/US-312-key-is-customized-plant-entity.md) | Key et IsCustomized sur l'entite Plant + migration EF | Indispensable | 3 | Terminé |
| [US-313](epics/E21-upsert-seeds-protection/US-313-key-dans-fichiers-json-seed.md) | Champ key dans les fichiers JSON de seed | Indispensable | 2 | Terminé |
| [US-314](epics/E21-upsert-seeds-protection/US-314-plant-seeder-upsert.md) | PlantSeeder en mode upsert par Key | Indispensable | 5 | Terminé |
| [US-315](epics/E21-upsert-seeds-protection/US-315-association-seeder-upsert.md) | AssociationSeeder en mode upsert | Indispensable | 3 | Terminé |
| [US-316](epics/E21-upsert-seeds-protection/US-316-plant-action-seeder-upsert.md) | PlantActionSeeder en mode upsert | Indispensable | 3 | Terminé |
| [US-317](epics/E21-upsert-seeds-protection/US-317-harvest-readiness-seeder-upsert.md) | HarvestReadinessSeeder en mode upsert | Indispensable | 3 | Terminé |
| [US-318](epics/E21-upsert-seeds-protection/US-318-guild-seeder-upsert.md) | GuildSeeder en mode upsert | Indispensable | 3 | Terminé |
| [US-319](epics/E21-upsert-seeds-protection/US-319-intrinsic-mechanism-seeder-upsert.md) | PlantIntrinsicMechanismSeeder en mode upsert | Indispensable | 2 | Terminé |

**Total E21 : 24 points (24 Terminé / 0 À faire)**

> Ordre de livraison : US-312 (entity + migration) et US-313 (JSON keys) en parallele → US-314 (PlantSeeder) en premier (les autres seeders dependent des plantes) → US-315, US-316, US-317, US-318, US-319 en parallele. Pattern commun : charger le set de PlantId verrouilles en un SELECT, puis boucle upsert avec logging Info/Debug.

---

## E22 — Administration du catalogue de plantes

> Interface d'administration pour modifier le catalogue global de plantes. Les modifications admin positionnent automatiquement `IsCustomized = true`, protegeant la plante du seed upsert. Un endpoint de deverrouillage permet de re-soumettre une plante au seed. Depend de E21 pour le champ `IsCustomized`.

| ID | Titre | Priorité | Points | Statut |
|----|-------|----------|--------|--------|
| [US-320](epics/E22-administration-catalogue/US-320-endpoint-admin-modifier-plante.md) | Endpoint admin pour modifier une plante du catalogue | Indispensable | 3 | À faire |
| [US-321](epics/E22-administration-catalogue/US-321-endpoint-admin-deverrouiller-plante.md) | Endpoint admin pour deverrouiller une plante | Important | 2 | À faire |
| [US-322](epics/E22-administration-catalogue/US-322-endpoint-admin-associations.md) | Endpoint admin pour modifier les associations d'une plante | Important | 3 | À faire |
| [US-323](epics/E22-administration-catalogue/US-323-ui-admin-catalogue.md) | UI admin de gestion du catalogue | Important | 5 | À faire |

**Total E22 : 13 points (0 Terminé / 13 À faire)**

> Ordre de livraison : US-320 (modifier plante) → US-321 (deverrouiller) et US-322 (associations) en parallele → US-323 (UI admin, depend des 3 endpoints). E22 depend de E21 (US-312 minimum).

---

## E23 — Variantes utilisateur

> Permettre aux jardiniers de créer des variantes personnelles à partir du catalogue. Une variante est une Plant avec `UserId` non null et `IsCustomized = true`, invisible aux autres utilisateurs. Backend only — l'UI de gestion des variantes fera l'objet d'un epic separe. Depend de E20 (heritage espece-variété) et E21 (champs Key/IsCustomized/UserId).

| ID | Titre | Priorité | Points | Statut |
|----|-------|----------|--------|--------|
| [US-324](epics/E23-variantes-utilisateur/US-324-créer-variante-utilisateur.md) | Service et endpoints pour créer une variante utilisateur | Indispensable | 5 | À faire |
| [US-325](epics/E23-variantes-utilisateur/US-325-lister-variantes-utilisateur.md) | Lister les variantes d'un utilisateur | Indispensable | 3 | À faire |
| [US-326](epics/E23-variantes-utilisateur/US-326-modifier-variante-utilisateur.md) | Modifier une variante utilisateur | Indispensable | 3 | À faire |
| [US-327](epics/E23-variantes-utilisateur/US-327-supprimer-variante-cascade.md) | Supprimer une variante utilisateur (cascade) | Important | 3 | À faire |
| [US-328](epics/E23-variantes-utilisateur/US-328-variante-dans-plantations.md) | Utiliser une variante dans les plantations | Important | 3 | À faire |

**Total E23 : 17 points (0 Terminé / 17 À faire)**

> Ordre de livraison : US-324 (creation) → US-325 (listing) et US-326 (modification) en parallele → US-327 (suppression cascade) → US-328 (intégration plantations). E23 depend de E20 (US-306 ParentPlantId) et E21 (US-312 Key/IsCustomized/UserId).

---

## E24 — Preferences de sol et pH des plantes ✅

> Enrichir l'entite Plant avec les types de sol compatibles (many-to-many via `PlantSoilType`) et la fourchette de pH optimale (deux decimaux). Afficher ces informations dans la fiche plante, proposer un filtre par type de sol, et alerter sur les incompatibilites pH dans les guildes. Le multi-sol permet de stocker plusieurs types de sol par plante (ex: tomate → Loam, Sandy, Clay), meme pattern que `PlantIntrinsicMechanism`.

| ID | Titre | Priorité | Points | Statut |
|----|-------|----------|--------|--------|
| [US-330](epics/E24-preferences-sol-ph/US-330-enum-soiltype-champs-ph-entity.md) | Enum SoilType, table PlantSoilType (many-to-many) et champs pH + migration EF | Indispensable | 3 | Terminé |
| [US-331](epics/E24-preferences-sol-ph/US-331-seed-data-sol-ph.md) | Seed data sol et pH pour les plantes du catalogue | Indispensable | 5 | Terminé |
| [US-332](epics/E24-preferences-sol-ph/US-332-api-dto-sol-ph.md) | Exposer SoilType et pH dans PlantDto | Indispensable | 2 | Terminé |
| [US-333](epics/E24-preferences-sol-ph/US-333-tests-unitaires-sol-ph.md) | Tests unitaires pour les propriétés sol et pH | Indispensable | 3 | Terminé |
| [US-334](epics/E24-preferences-sol-ph/US-334-frontend-affichage-sol-ph-fiche-plante.md) | Afficher le type de sol et le pH sur la fiche plante | Indispensable | 3 | Terminé |
| [US-335](epics/E24-preferences-sol-ph/US-335-frontend-filtre-sol-liste-plantes.md) | Filtrer les plantes par type de sol | Important | 3 | Terminé |
| [US-336](epics/E24-preferences-sol-ph/US-336-alerte-compatibilite-ph-guilde.md) | Alerte de compatibilite pH dans l'éditeur de guilde | Important | 5 | Terminé |

**Total E24 : 24 points (24 Terminé / 0 À faire)**

> Ordre de livraison : US-330 (entity + migration) → US-331 (seed data, necessite validation plant-expert) et US-332 (API/DTO) en parallele → US-333 (tests) → US-334 (affichage fiche) → US-335 (filtre liste) et US-336 (alerte guilde) en parallele. US-331 est la story la plus lourde car elle necessite l'expertise botanique pour chaque plante.

---

## E25 — Calendrier d'arrosage

> Fournir un planning d'arrosage hebdomadaire pour chaque jardin, calcule automatiquement à partir des besoins en eau des plantes, de la saison, et optionnellement du type de sol et du paillage. Le jardinier debutant sait quand arroser ; le confirme ajuste selon son contexte.

| ID | Titre | Priorité | Points | Statut |
|----|-------|----------|--------|--------|
| [US-339](epics/E25-calendrier-arrosage/US-339-moteur-calcul-frequence-arrosage.md) | Moteur de calcul de la frequence d'arrosage (WaterNeeds x saison + jours recommandes) | Indispensable | 3 | A faire |
| [US-340](epics/E25-calendrier-arrosage/US-340-endpoint-planning-arrosage-planche.md) | Composant "Arrosage aujourd'hui" (badges plantes a arroser, au-dessus des tabs) | Indispensable | 5 | A faire |
| [US-341](epics/E25-calendrier-arrosage/US-341-frontend-page-arrosage-jardin.md) | Tab "Arrosage" avec grille hebdomadaire 7 jours + frequences saisonnieres | Indispensable | 5 | A faire |
| [US-342](epics/E25-calendrier-arrosage/US-342-ajustement-frequence-type-sol.md) | Ajustement frequence selon type de sol (Sandy x1.3, Clay x0.7) | Important | 5 | A faire |
| [US-343](epics/E25-calendrier-arrosage/US-343-paillage-planche-ajustement-arrosage.md) | Paillage sur planche (HasMulch x0.6) et ajustement arrosage | Important | 3 | A faire |
| [US-344](epics/E25-calendrier-arrosage/US-344-detail-hebdo-quantites-indicatives.md) | Quantites d'eau indicatives (WaterAmountMl + seed data) | Optionnel | 8 | A faire |

**Total E25 : 29 points (0 Terminé / 0 En cours / 29 À faire)**

> Ordre de livraison : US-339 (moteur de calcul, prerequis a tout) -> US-340 (composant "Arrosage aujourd'hui") + US-341 (tab Arrosage + grille hebdo) en parallele. US-342 (sol) et US-343 (paillage) sont independantes entre elles mais enrichissent le calcul de US-339. US-344 (quantites d'eau) depend de US-341 et est optionnelle.

---

## E26 — Meteo locale et adaptation climatique

> Integrer les donnees meteo en temps reel (Open-Meteo, gratuit et sans cle API) pour adapter dynamiquement les recommandations du jardinier. La localisation du jardin permet de recuperer temperature, precipitations et previsions. Ces donnees enrichissent l'arrosage (reduction si pluie, augmentation si canicule) et declenchent des alertes (gel). E26 complete E12 (adaptation climatique statique/manuelle) et E25 (calendrier d'arrosage) avec des donnees meteo temps reel.

| ID | Titre | Priorite | Points | Statut |
|----|-------|----------|--------|--------|
| [US-345](epics/E26-meteo-locale/US-345-localisation-jardin.md) | Localisation du jardin (latitude / longitude) | Indispensable | 5 | A faire |
| [US-346](epics/E26-meteo-locale/US-346-service-meteo-open-meteo.md) | Service meteo backend (Open-Meteo API) | Indispensable | 5 | A faire |
| [US-347](epics/E26-meteo-locale/US-347-endpoint-meteo-jardin.md) | Endpoint API meteo du jardin | Indispensable | 3 | A faire |
| [US-348](epics/E26-meteo-locale/US-348-widget-meteo-frontend.md) | Widget meteo sur la page jardin | Indispensable | 5 | A faire |
| [US-349](epics/E26-meteo-locale/US-349-adaptation-arrosage-precipitations.md) | Adaptation de l'arrosage selon les precipitations recentes | Important | 5 | A faire |
| [US-350](epics/E26-meteo-locale/US-350-adaptation-arrosage-canicule.md) | Adaptation de l'arrosage en periode de canicule | Important | 3 | A faire |
| [US-351](epics/E26-meteo-locale/US-351-alerte-gel.md) | Alerte gel sur la page jardin | Important | 3 | A faire |

**Total E26 : 29 points (0 Termine / 0 En cours / 29 A faire)**

> Ordre de livraison : US-345 (localisation, prerequis a tout) -> US-346 (service meteo backend) -> US-347 (endpoint API) -> US-348 (widget frontend). US-349 (precipitations) et US-350 (canicule) dependent de US-346 et de E25/US-339 (moteur arrosage), livrables en parallele. US-351 (alerte gel) depend de US-347 et US-348.

---

## Recapitulatif

| Epique | Statut | Points | Termine | En cours | A faire |
|--------|--------|--------|---------|----------|---------|
| E01 — Gestion des jardins et planches | ✅ | 34 | 34 | 0 | 0 |
| E02 — Associations vegetales (residuel) | ✅ | 8 | 8 | 0 | 0 |
| E03 — Editeur graphique du jardin | | 39 | 0 | 0 | 39 |
| E04 — Gestion des rotations de cultures | | 30 | 0 | 0 | 30 |
| ~~E05 — Plantes compagnes (refonte)~~ | ~~Annulee~~ | ~~19~~ | — | — | — |
| E06 — Refonte UI permaculture | ✅ | 27 | 27 | 0 | 0 |
| E07 — Mes plantes | ✅ | 15 | 15 | 0 | 0 |
| E08 — Finitions UX Associations | ✅ | 14 | 14 | 0 | 0 |
| E09 — Conscience de l'enracinement | ✅ | 9 | 9 | 0 | 0 |
| E10 — Calendrier cultural | | 52 | 33 | 0 | 19 |
| E11 — Guildes, mecanismes et refonte editeur | ✅ | 44 | 44 | 0 | 0 |
| E12 — Adaptation climatique | | 16 | 0 | 0 | 16 |
| E13 — Infrastructure i18n (backend) | | 38 | 0 | 0 | 38 |
| E14 — UX i18n (frontend) | | 16 | 0 | 0 | 16 |
| E15 — Assistant de creation de guilde | ✅ | 19 | 19 | 0 | 0 |
| E16 — Authentification OAuth | | 41 | 0 | 0 | 41 |
| ~~E17 — Dimensions et formes des planches~~ | ~~Annulee~~ | ~~10~~ | — | — | — |
| E18 — Page d'accueil / tableau de bord | | 13 | 0 | 0 | 13 |
| E19 — Dette technique frontend | ✅ | 12 | 12 | 0 | 0 |
| E20 — Hierarchie espece-variete | | 27 | 24 | 0 | 3 |
| E21 — Upsert des seeds et protection des donnees | ✅ | 24 | 24 | 0 | 0 |
| E22 — Administration du catalogue de plantes | | 13 | 0 | 0 | 13 |
| E23 — Variantes utilisateur | | 17 | 0 | 0 | 17 |
| E24 — Preferences de sol et pH | ✅ | 24 | 24 | 0 | 0 |
| E25 — Calendrier d'arrosage | | 29 | 0 | 0 | 29 |
| E26 — Meteo locale et adaptation climatique | | 29 | 0 | 0 | 29 |
| **Total (actif)** | | **580** | **271** | **0** | **309** |

---

*Backlog gere par l'agent Product Owner — derniere mise a jour : 2026-04-03 (E26 creee — meteo locale 7 stories 29 pts)*
