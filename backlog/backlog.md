# Backlog — Garden Assistant

> **Legende des priorites :** Indispensable - Important - Optionnel - Hors scope (cette version)
> **Statuts :** A faire - En cours - Termine

---

## E01 — Gestion des jardins et des planches

> Permettre au jardinier de creer et d'organiser ses espaces de culture sous forme de jardins et de planches. La vue jardin affiche les planches en collapsibles avec detail complet des associations en lecture seule (Option C hybride). L'edition des plantes se fait via redirection vers la page associations.

| ID | Titre | Priorite | Points | Statut |
|----|-------|----------|--------|--------|
| [US-001](epics/E01-gestion-jardin-planches/US-001-creer-un-jardin.md) | Creer un jardin | Indispensable | 2 | ✅ Termine |
| [US-002](epics/E01-gestion-jardin-planches/US-002-lister-mes-jardins.md) | Lister mes jardins | Indispensable | 2 | ✅ Termine |
| [US-003](epics/E01-gestion-jardin-planches/US-003-modifier-un-jardin.md) | Modifier un jardin | Indispensable | 1 | ✅ Termine |
| [US-004](epics/E01-gestion-jardin-planches/US-004-supprimer-un-jardin.md) | Supprimer un jardin | Important | 2 | ✅ Termine |
| [US-005](epics/E01-gestion-jardin-planches/US-005-ajouter-une-planche.md) | Ajouter une planche a un jardin | Indispensable | 2 | ✅ Termine |
| [US-006](epics/E01-gestion-jardin-planches/US-006-modifier-une-planche.md) | Modifier une planche (nom) | Important | 1 | ✅ Termine |
| [US-007](epics/E01-gestion-jardin-planches/US-007-supprimer-une-planche.md) | Supprimer une planche | Important | 2 | ✅ Termine |
| [US-124](epics/E01-gestion-jardin-planches/US-124-vue-jardin-liste-planches.md) | Vue jardin avec liste des planches en collapsibles | Indispensable | 3 | ✅ Termine |
| [US-125](epics/E01-gestion-jardin-planches/US-125-detail-planche-associations-readonly.md) | Detail d'une planche avec associations en lecture seule | Indispensable | 8 | ✅ Termine |
| [US-126](epics/E01-gestion-jardin-planches/US-126-modifier-planche-redirect-associations.md) | Modifier les plantes d'une planche via la page associations | Indispensable | 3 | ✅ Termine |
| [US-127](epics/E01-gestion-jardin-planches/US-127-calendrier-global-jardin.md) | Calendrier global du jardin avec groupement par planches | Important | 5 | ✅ Termine |
| [US-128](epics/E01-gestion-jardin-planches/US-128-plant-badge-composant-partage.md) | Composant PlantBadge reutilisable | Indispensable | 1 | ✅ Termine |
| [US-129](epics/E01-gestion-jardin-planches/US-129-creation-guilde-depuis-page-guildes.md) | Creer une guilde depuis la page Guildes | Important | 2 | ✅ Termine |

**Total E01 : 34 points (34 Termine / 0 En cours / 0 A faire)**

> Epic livre. Backend complet (GardenService, BedService, GardensController, BedsController, 19 tests unitaires). Frontend complet : CRUD jardins et planches, vue jardin avec planches collapsibles, detail planche en lecture seule via PlantAssociationPanel, redirection vers la page associations pour edition, calendrier global avec toggle Vue globale / Groupé par planche. Travaux transverses : composant PlantBadge partage, mode creation de guilde, integration des plantes de jardins dans le calendrier principal avec filtre source.

---

## E02 — Associations vegetales (residuel)

> Stories restantes apres la refonte E05. Les US-008/009/010/012 ont ete supprimees (remplacees par E05).

| ID | Titre | Priorite | Points | Statut |
|----|-------|----------|--------|--------|
| [US-011](epics/E02-association-plantes/US-011-ajouter-plante-a-planche.md) | Ajouter une plante a une planche | Indispensable | 5 | A faire |
| [US-013](epics/E02-association-plantes/US-013-supprimer-plante-planche.md) | Retirer une plante d'une planche | Important | 3 | A faire |

**Total E02 : 8 points (0 Termine / 8 A faire)**

---

## E03 — Editeur graphique du jardin

> Fournir un editeur visuel pour representer fidelement le terrain, positionner les planches et visualiser les cultures. Inclut les dimensions et formes des planches (ex-E17).

| ID | Titre | Priorite | Points | Statut |
|----|-------|----------|--------|--------|
| [US-014](epics/E03-outil-graphique/US-014-dessiner-contours-jardin.md) | Dessiner les contours du jardin | Indispensable | 8 | A faire |
| [US-015](epics/E03-outil-graphique/US-015-placer-planches-sur-plan.md) | Placer les planches sur le plan | Indispensable | 8 | A faire |
| [US-016](epics/E03-outil-graphique/US-016-visualiser-plantes-sur-plan.md) | Visualiser les plantes sur le plan | Important | 5 | A faire |
| [US-017](epics/E03-outil-graphique/US-017-ajouter-elements-fixes.md) | Ajouter des elements fixes sur le plan | Optionnel | 5 | A faire |
| [US-018](epics/E03-outil-graphique/US-018-exporter-plan.md) | Exporter le plan du jardin | Optionnel | 3 | A faire |
| [US-121](epics/E03-outil-graphique/US-121-definir-dimensions-planche.md) | Definir les dimensions d'une planche | Indispensable | 3 | A faire |
| [US-122](epics/E03-outil-graphique/US-122-modifier-dimensions-planche.md) | Modifier les dimensions d'une planche | Important | 2 | A faire |
| [US-123](epics/E03-outil-graphique/US-123-forme-planche.md) | Choisir la forme d'une planche | Important | 5 | A faire |

**Total E03 : 39 points (0 Termine / 39 A faire)**

> Note : US-121/122/123 proviennent de l'ex-E17 (dimensions et formes des planches), fusionne dans E03 car les dimensions et formes n'ont de sens que dans le contexte de l'editeur graphique.

---

## E04 — Gestion des rotations de cultures

> Permettre au jardinier de suivre l'historique des cultures et de planifier les rotations pour maintenir la sante du sol.

| ID | Titre | Priorite | Points | Statut |
|----|-------|----------|--------|--------|
| [US-019](epics/E04-rotations-culture/US-019-enregistrer-culture.md) | Enregistrer une culture sur une planche | Indispensable | 3 | A faire |
| [US-020](epics/E04-rotations-culture/US-020-consulter-historique-planche.md) | Consulter l'historique d'une planche | Indispensable | 3 | A faire |
| [US-021](epics/E04-rotations-culture/US-021-planifier-rotation.md) | Planifier la rotation de la saison suivante | Indispensable | 8 | A faire |
| [US-022](epics/E04-rotations-culture/US-022-alerte-rotation.md) | Recevoir des alertes de mauvaise rotation | Important | 5 | A faire |
| [US-023](epics/E04-rotations-culture/US-023-visualiser-rotations-multi-annees.md) | Visualiser les rotations sur plusieurs annees | Important | 8 | A faire |
| [US-024](epics/E04-rotations-culture/US-024-export-historique.md) | Exporter l'historique des cultures | Optionnel | 3 | A faire |

**Total E04 : 30 points (0 Termine / 30 A faire)**

---

## ~~E05 — Plantes compagnes (refonte)~~ SUPPRIME

> Epique supprimee. Toutes les stories (US-025 a US-032) ont ete livrees puis remplacees par la refonte UI E06. Les fichiers ont ete supprimes du backlog.

---

## E06 — Refonte UI permaculture (mise en page initiale)

> Reconstruire le frontend avec un catalogue de plantes, une selection multi-plantes, et des recommandations. Layout initial a 3 colonnes, evolue vers 2 colonnes dans E11.

| ID | Titre | Priorite | Points | Statut |
|----|-------|----------|--------|--------|
| [US-033](epics/E06-refonte-ui-permaculture/US-033-app-shell-header-navigation.md) | App shell avec header et navigation | Indispensable | 3 | Termine |
| [US-034](epics/E06-refonte-ui-permaculture/US-034-page-associations-layout-trois-colonnes.md) | Page Associations — mise en page 3 colonnes | Indispensable | 2 | Termine |
| [US-035](epics/E06-refonte-ui-permaculture/US-035-catalogue-plantes-panneau-gauche.md) | Catalogue des plantes (panneau gauche) | Indispensable | 3 | Termine |
| [US-036](epics/E06-refonte-ui-permaculture/US-036-recherche-plantes-catalogue.md) | Recherche dans le catalogue | Indispensable | 1 | Termine |
| [US-037](epics/E06-refonte-ui-permaculture/US-037-tri-catalogue-plantes.md) | Tri du catalogue de plantes | Important | 1 | Termine |
| [US-038](epics/E06-refonte-ui-permaculture/US-038-selection-multi-plantes-panneau-centre.md) | Selection multi-plantes (panneau central) | Indispensable | 5 | Termine |
| [US-039](epics/E06-refonte-ui-permaculture/US-039-panneau-compagnons-recommandations.md) | Panneau de recommandations compagnes | Indispensable | 5 | Termine |
| [US-040](epics/E06-refonte-ui-permaculture/US-040-interactions-compagnons-et-guildes.md) | Ajouter depuis compagnons et guildes | Indispensable | 3 | Termine |
| [US-041](epics/E06-refonte-ui-permaculture/US-041-etats-vides-et-accueil.md) | Etats vides et messages d'accueil | Important | 1 | Termine |
| [US-042](epics/E06-refonte-ui-permaculture/US-042-responsive-mobile-tablette.md) | Adaptation responsive mobile/tablette | Important | 3 | Termine |

**Total E06 : 27 points (27 Termine / 0 A faire)**

> Note : Le layout 3 colonnes (US-034) a evolue vers 2 colonnes dans US-072 (E11). US-039 (panneau de recommandations) a ete integre dans l'editeur de guilde (E11).

---

## E07 — Mes plantes (liste personnelle)

> Permettre au jardinier de tenir une liste personnelle des plantes qu'il cultive ou souhaite cultiver, integree au catalogue et aux recommandations de la page Associations.

| ID | Titre | Priorite | Points | Statut |
|----|-------|----------|--------|--------|
| [US-043](epics/E07-mes-plantes/US-043-api-mes-plantes.md) | API et persistance « Mes plantes » | Indispensable | 3 | Termine |
| [US-044](epics/E07-mes-plantes/US-044-store-mes-plantes.md) | Store de signaux « Mes plantes » | Indispensable | 2 | Termine |
| [US-045](epics/E07-mes-plantes/US-045-page-mes-plantes.md) | Page « Mes plantes » avec gestion de la liste | Indispensable | 5 | Termine |
| [US-046](epics/E07-mes-plantes/US-046-info-box-reusable.md) | Composant info-box reutilisable | Important | 2 | A faire |
| [US-047](epics/E07-mes-plantes/US-047-integration-catalogue-tri.md) | Prioriser « Mes plantes » dans le catalogue | Indispensable | 3 | Termine |
| [US-048](epics/E07-mes-plantes/US-048-bouton-ajouter-depuis-associations.md) | Ajouter a « Mes plantes » depuis Associations | Indispensable | 2 | Termine |

**Total E07 : 17 points (15 Termine / 2 A faire)**

---

## E08 — Finitions UX — page Associations

> Ameliorations UX de la page Associations : popups d'information, fiche detail plante, conteneur visuel de guilde, refactoring Sass et optimisation API.

| ID | Titre | Priorite | Points | Statut |
|----|-------|----------|--------|--------|
| [US-049](epics/E08-polish-ux-associations/US-049-badge-info-popup.md) | Popup d'information sur les badges | Important | 3 | Termine |
| [US-050](epics/E08-polish-ux-associations/US-050-plant-detail-dialog.md) | Dialog detail d'une plante | Important | 2 | Termine |
| [US-051](epics/E08-polish-ux-associations/US-051-guild-container-visual.md) | Conteneur visuel de guilde | Optionnel | 1 | Termine |
| [US-052](epics/E08-polish-ux-associations/US-052-sass-7-1-refactoring.md) | Refactoring styles Sass 7-1 | Important | 3 | Termine |
| ~~US-053~~ | ~~Limiter l'API catalogue a 20 resultats~~ | ~~Indispensable~~ | ~~1~~ | Abandonne |
| [US-065](epics/E08-polish-ux-associations/US-065-associations-manquantes.md) | Indicateur d'associations importantes manquantes | Important | 5 | A faire |

**Total E08 : 14 points (9 Termine / 5 A faire)**

> Note : US-053 abandonnee — la limite de 20 plantes dans le catalogue a ete supprimee. Le catalogue affiche maintenant toutes les plantes avec une scrollbar (max-height 70vh).

---

## E09 — Conscience de l'enracinement

> Exploiter les donnees de profondeur racinaire deja presentes sur les plantes pour ameliorer les recommandations de plantes compagnes et la conception des guildes.

| ID | Titre | Priorite | Points | Statut |
|----|-------|----------|--------|--------|
| [US-054](epics/E09-enracinement/US-054-badge-enracinement-fiche-plante.md) | Badge d'enracinement sur les fiches plantes | Indispensable | 1 | Termine |
| [US-055](epics/E09-enracinement/US-055-filtre-enracinement-catalogue.md) | Filtre enracinement dans le catalogue | Indispensable | 1 | Termine |
| [US-056](epics/E09-enracinement/US-056-indicateur-stratification-guilde.md) | Indicateur de stratification dans l'editeur de guilde | Important | 3 | Termine |
| [US-057](epics/E09-enracinement/US-057-bonus-enracinement-algorithme-score.md) | Bonus enracinement dans l'algorithme de score | Important | 3 | A faire |
| [US-058](epics/E09-enracinement/US-058-alerte-competition-enracinement.md) | Alertes de densite racinaire par zone | Optionnel | 1 | Termine |

**Total E09 : 9 points (6 Termine / 3 A faire)**

> Note : US-056 livree avec une approche colonnes (au lieu de coupe SVG). US-058 simplifiee : avertissement de densite par zone au lieu de badges par paire. US-057 reste a faire (bonus algorithmique cote backend).

---

## E10 — Calendrier cultural des plantes

> Fournir aux jardiniers un calendrier cultural personnalise base sur leur liste « Mes plantes », affichant les actions cles (semis, repiquage, recolte, taille, pincage, buttage, division) avec leurs fenetres de realisation en demi-mois. Vue Gantt par plante avec indicateurs de maturite.

| ID | Titre | Priorite | Points | Statut |
|----|-------|----------|--------|--------|
| [US-059](epics/E10-calendrier-cultural/US-059-modele-action-culturale.md) | Modele de donnees des actions culturales | Indispensable | 5 | Termine |
| [US-060](epics/E10-calendrier-cultural/US-060-page-calendrier.md) | Page calendrier (vue Gantt par plante) | Indispensable | 5 | Termine |
| [US-061](epics/E10-calendrier-cultural/US-061-widget-actions-du-mois.md) | Widget « En ce moment / Prochainement » | Important | 3 | Termine |
| [US-062](epics/E10-calendrier-cultural/US-062-filtre-type-action-calendrier.md) | Filtre par type d'action sur le calendrier | Important | 2 | Termine |
| [US-063](epics/E10-calendrier-cultural/US-063-semis-successifs.md) | Suggestions de semis successifs | Optionnel | 5 | A faire |
| [US-064](epics/E10-calendrier-cultural/US-064-alertes-taille-pincage.md) | Alertes taille et pincage | Important | 3 | Termine |
| [US-076](epics/E10-calendrier-cultural/US-076-indicateurs-maturite-modele.md) | Indicateurs de maturite — modele et seed | Indispensable | 5 | Termine |
| [US-077](epics/E10-calendrier-cultural/US-077-section-pret-a-recolter.md) | Popup « Pret a recolter » depuis le calendrier | Indispensable | 3 | Termine |
| [US-078](epics/E10-calendrier-cultural/US-078-calendrier-fiche-plante.md) | Calendrier cultural dans la fiche plante | Indispensable | 3 | Termine |
| [US-079](epics/E10-calendrier-cultural/US-079-popups-educatives-actions.md) | Popups educatives des types d'actions | Important | 2 | Termine |
| [US-080](epics/E10-calendrier-cultural/US-080-endpoint-batch-calendrier.md) | Endpoint batch calendrier « Mes plantes » | Indispensable | 2 | Termine |
| [US-101](epics/E10-calendrier-cultural/US-101-fiches-techniques-actions.md) | Fiches techniques par action culturale et par plante | Important | 8 | A faire |

**Total E10 : 46 points (33 Termine / 13 A faire)**

> Livre le 2026-03-20. US-063 (semis successifs) reste a faire. Calendrier integre dans la page dediee, la fiche plante (plant-card collapsible), et le panneau associations/guildes. Indicateurs de maturite accessibles via popup depuis la ligne Recolte du Gantt. Widget demi-mois « En ce moment / Prochainement » en 2 colonnes. Filtre single-select par type d'action avec tri par date. Bouton favori deplace en bas du popup detail.

---

## E11 — Guildes, mecanismes et refonte editeur

> Enrichir l'editeur de guilde avec les mecanismes intrinseques et relationnels, les avertissements de conflit, le detail des associations, et elargir la base de donnees a 50 guildes et 356 associations. Simplifier le layout en 2 colonnes et nettoyer le backend. Ajouter la notion de plante centrale dans les guildes.

| ID | Titre | Priorite | Points | Statut |
|----|-------|----------|--------|--------|
| [US-066](epics/E11-guildes-et-mecanismes/US-066-systeme-mecanismes-intrinseques.md) | Systeme de mecanismes intrinseques des plantes | Indispensable | 5 | Termine |
| [US-067](epics/E11-guildes-et-mecanismes/US-067-filtres-mecanismes-catalogue.md) | Filtres par mecanisme dans le catalogue | Important | 2 | Termine |
| [US-068](epics/E11-guildes-et-mecanismes/US-068-avertissements-conflits-guilde.md) | Avertissements de conflits dans la guilde | Indispensable | 2 | Termine |
| [US-069](epics/E11-guildes-et-mecanismes/US-069-details-associations-guilde.md) | Details des associations dans la guilde | Important | 2 | Termine |
| [US-070](epics/E11-guildes-et-mecanismes/US-070-resume-mecanismes-guilde.md) | Resume des mecanismes de la guilde | Important | 1 | Termine |
| [US-071](epics/E11-guildes-et-mecanismes/US-071-panneau-guildes-permanent.md) | Panneau des guildes toujours visible | Important | 2 | Termine |
| [US-072](epics/E11-guildes-et-mecanismes/US-072-layout-deux-colonnes.md) | Layout deux colonnes (catalogue + editeur) | Indispensable | 2 | Termine |
| [US-073](epics/E11-guildes-et-mecanismes/US-073-50-guildes-officielles.md) | 50 guildes officielles de permaculture | Indispensable | 5 | Termine |
| [US-074](epics/E11-guildes-et-mecanismes/US-074-356-associations.md) | 356 associations vegetales documentees | Indispensable | 3 | Termine |
| [US-075](epics/E11-guildes-et-mecanismes/US-075-nettoyage-backend.md) | Nettoyage backend (controleurs et DTOs) | Important | 2 | Termine |
| [US-128](epics/E11-guildes-et-mecanismes/US-128-role-plante-guilde-backend.md) | Role de la plante dans la guilde (backend) | Indispensable | 5 | Termine |
| [US-129](epics/E11-guildes-et-mecanismes/US-129-role-central-seed-guildes.md) | Role central et completude associations dans les guildes | Indispensable | 8 | A faire |
| [US-130](epics/E11-guildes-et-mecanismes/US-130-indicateur-visuel-role-central.md) | Indicateur visuel du role central dans l'editeur | Important | 5 | Termine |

**Total E11 : 44 points (36 Termine / 8 A faire)**

> Note : US-128 et US-130 livres le 2026-03-30. Inclut : enum GuildPlantRole (Central/Companion), migration EF, API mise a jour, etoile doree toggle dans le plant card, tri des plantes centrales en premier, bordure doree sur les compagnons de la plante centrale dans le catalogue, composant shared PlantAssociationPanel (associations, mecanismes, stratification, calendrier) integre dans le guild editor. API enrichie avec LinkedPlantIds sur CompanionRecommendationDto. US-129 reste a faire : les 3 premieres guildes ont des roles, 47 restantes + completude associations a valider avec le plant-expert.

---

## E12 — Adaptation climatique

> Permettre au jardinier d'adapter le calendrier cultural a son microclimat en renseignant la date de derniere gelee et en selectionnant une zone climatique. Les fenetres de semis et repiquage sont recalculees en consequence.

| ID | Titre | Priorite | Points | Statut |
|----|-------|----------|--------|--------|
| [US-081](epics/E12-adaptation-climatique/US-081-date-derniere-gelee.md) | Parametre « date derniere gelee » sur le jardin | Indispensable | 3 | A faire |
| [US-082](epics/E12-adaptation-climatique/US-082-fenetres-relatives-gelee.md) | Calcul des fenetres relatives a la derniere gelee | Indispensable | 5 | A faire |
| [US-083](epics/E12-adaptation-climatique/US-083-zones-climatiques.md) | Zones climatiques avec donnees differenciees | Important | 8 | A faire |

**Total E12 : 16 points (0 Termine / 16 A faire)**

---

## E13 — Infrastructure i18n (backend)

> Mettre en place l'infrastructure de traduction en base de donnees avec une table generique, migrer les donnees de seed existantes, et exposer les traductions via l'API. Architecture ouverte : francais (defaut) + anglais initialement, extensible a toute langue.

| ID | Titre | Priorite | Points | Statut |
|----|-------|----------|--------|--------|
| [US-084](epics/E13-infrastructure-i18n/US-084-table-languages.md) | Table `languages` et seed FR + EN | Indispensable | 2 | A faire |
| [US-085](epics/E13-infrastructure-i18n/US-085-table-translations.md) | Table generique `translations` | Indispensable | 3 | A faire |
| [US-086](epics/E13-infrastructure-i18n/US-086-service-traduction.md) | Service de traduction (CRUD + resolution par langue) | Indispensable | 5 | A faire |
| [US-087](epics/E13-infrastructure-i18n/US-087-seed-plantes-fr.md) | Migrer les seed plantes FR dans `translations` | Indispensable | 3 | A faire |
| [US-088](epics/E13-infrastructure-i18n/US-088-seed-guildes-fr.md) | Migrer les seed guildes FR dans `translations` | Indispensable | 2 | A faire |
| [US-089](epics/E13-infrastructure-i18n/US-089-seed-maturite-fr.md) | Migrer les seed maturite/criteres FR dans `translations` | Important | 2 | A faire |
| [US-101](epics/E13-infrastructure-i18n/US-101-seed-associations-fr.md) | Migrer les seed associations FR dans `translations` | Indispensable | 2 | A faire |
| [US-102](epics/E13-infrastructure-i18n/US-102-seed-actions-culturales-fr.md) | Migrer les seed actions culturales FR dans `translations` | Important | 2 | A faire |
| [US-090](epics/E13-infrastructure-i18n/US-090-seed-plantes-en.md) | Seed des traductions EN pour les plantes | Indispensable | 5 | A faire |
| [US-091](epics/E13-infrastructure-i18n/US-091-seed-guildes-maturite-en.md) | Seed des traductions EN pour guildes, maturite, associations et actions | Important | 5 | A faire |
| [US-092](epics/E13-infrastructure-i18n/US-092-api-accept-language.md) | API: header `Accept-Language` et resolution dans les endpoints existants | Indispensable | 5 | A faire |
| [US-093](epics/E13-infrastructure-i18n/US-093-fallback-langue-defaut.md) | Fallback langue par defaut (FR) quand traduction absente | Indispensable | 2 | A faire |

**Total E13 : 38 points (0 Termine / 38 A faire)**

---

## E14 — UX i18n (frontend)

> Ajouter le support multilingue cote frontend : fichier de traductions anglais, selecteur de langue, persistance du choix, et nettoyage du fichier francais existant. Depend de E13 pour les donnees traduites de l'API.

| ID | Titre | Priorite | Points | Statut |
|----|-------|----------|--------|--------|
| [US-094](epics/E14-ux-i18n/US-094-audit-nettoyage-fr-json.md) | Audit et nettoyage de `fr.json` | Important | 2 | A faire |
| [US-095](epics/E14-ux-i18n/US-095-creer-en-json.md) | Creer `en.json` avec toutes les traductions UI en anglais | Indispensable | 3 | A faire |
| [US-096](epics/E14-ux-i18n/US-096-selecteur-langue.md) | Selecteur de langue dans le shell (header) | Indispensable | 3 | A faire |
| [US-097](epics/E14-ux-i18n/US-097-persistance-langue.md) | Persistance du choix de langue (localStorage) | Indispensable | 1 | A faire |
| [US-098](epics/E14-ux-i18n/US-098-intercepteur-accept-language.md) | Envoyer `Accept-Language` dans tous les appels API | Indispensable | 2 | A faire |
| [US-099](epics/E14-ux-i18n/US-099-afficher-donnees-traduites.md) | Afficher les donnees traduites de l'API (plantes, guildes, maturite) | Indispensable | 3 | A faire |
| [US-100](epics/E14-ux-i18n/US-100-contenu-utilisateur-non-traduit.md) | Gestion du contenu utilisateur non traduit (guildes personnalisees, notes) | Important | 2 | A faire |

**Total E14 : 16 points (0 Termine / 16 A faire)**

> Note : E14 depend de E13 — le selecteur de langue et l'intercepteur HTTP n'ont de sens qu'avec l'infrastructure backend en place. US-094 et US-095 peuvent etre demarrees en parallele de E13.

---

## E15 — Assistant de creation de guilde

> Guider le jardinier pas a pas dans la composition d'une guilde equilibree : analyse des lacunes (mecanismes manquants, couches racinaires vides, conflits), texte educatif, et filtres cliquables pour trouver rapidement les plantes complementaires.

| ID | Titre | Priorite | Points | Statut |
|----|-------|----------|--------|--------|
| [US-103](epics/E15-assistant-creation-guilde/US-103-analyse-lacunes-mecanismes.md) | Analyse des lacunes de mecanismes dans la guilde | Indispensable | 5 | A faire |
| [US-104](epics/E15-assistant-creation-guilde/US-104-lacunes-stratification-racinaire.md) | Lacunes de stratification racinaire dans l'assistant | Indispensable | 3 | A faire |
| [US-105](epics/E15-assistant-creation-guilde/US-105-texte-educatif-mecanismes-cles.md) | Texte educatif sur les mecanismes cles d'une bonne guilde | Important | 2 | A faire |
| [US-106](epics/E15-assistant-creation-guilde/US-106-panneau-assistant-guilde.md) | Panneau assistant dans l'editeur de guilde | Indispensable | 3 | A faire |
| [US-107](epics/E15-assistant-creation-guilde/US-107-alerte-associations-nefastes-assistant.md) | Alerte associations nefastes dans l'assistant | Important (MVP) | 2 | A faire |
| [US-108](epics/E15-assistant-creation-guilde/US-108-indicateur-sante-guilde.md) | Indicateur de sante de la guilde | Optionnel | 2 | A faire |
| [US-109](epics/E15-assistant-creation-guilde/US-109-alerte-diversite-familles-botaniques.md) | Alerte diversite des familles botaniques | Important | 2 | A faire |

**Total E15 : 19 points (0 Termine / 19 A faire)**

> Note : Aucune dependance backend — toute la logique s'appuie sur les signaux et DTOs existants dans le `CompanionStore`. L'ordre de livraison recommande est : US-106 (conteneur) en parallele de US-103 + US-104 (logique), puis US-105, US-107 et US-109 (enrichissement), et enfin US-108 (optionnel). US-107 est confirme pour le MVP.

---

## E16 — Authentification OAuth (Google & Discord)

> Ajouter l'authentification sociale via Google et Discord en utilisant le flux Authorization Code. Auto-enregistrement au premier login, consentement email, cross-provider linking, et restriction du dev-token a l'environnement de developpement.

| ID | Titre | Priorite | Points | Statut |
|----|-------|----------|--------|--------|
| [US-110](epics/E16-oauth-login/US-110-migration-user-external-login.md) | Migration BDD : User modifie et ExternalLogin | Indispensable | 3 | A faire |
| [US-111](epics/E16-oauth-login/US-111-middleware-oauth-google-discord.md) | Middleware OAuth Google et Discord | Indispensable | 2 | A faire |
| [US-112](epics/E16-oauth-login/US-112-backend-auth-flow.md) | Flux OAuth backend (login, callback, complete) | Indispensable | 8 | A faire |
| [US-113](epics/E16-oauth-login/US-113-backend-profile-email-consent.md) | Endpoint profil : toggle consentement email | Indispensable | 3 | A faire |
| [US-114](epics/E16-oauth-login/US-114-dev-token-environment-gating.md) | Restriction dev-token a l'environnement dev | Indispensable | 1 | A faire |
| [US-115](epics/E16-oauth-login/US-115-frontend-login-page.md) | Page de login frontend | Indispensable | 3 | A faire |
| [US-116](epics/E16-oauth-login/US-116-frontend-auth-callback.md) | Callback OAuth et consentement email | Indispensable | 5 | A faire |
| [US-117](epics/E16-oauth-login/US-117-frontend-auth-guard-startup.md) | Auth guard et flux de demarrage | Indispensable | 3 | A faire |
| [US-118](epics/E16-oauth-login/US-118-frontend-profile-email-consent.md) | Toggle consentement email dans le profil | Important | 3 | A faire |
| [US-119](epics/E16-oauth-login/US-119-tests-unitaires-services-oauth.md) | Tests unitaires services OAuth | Indispensable | 5 | A faire |
| [US-120](epics/E16-oauth-login/US-120-tests-integration-oauth.md) | Tests d'integration OAuth | Indispensable | 5 | A faire |

**Total E16 : 41 points (0 Termine / 41 A faire)**

> Ordre de livraison : US-110 (migration) → US-111 (middleware) → US-112 (flux backend) → US-113 (profil) et US-114 (dev-token) en parallele → US-115 (login page) → US-116 (callback) → US-117 (guard) → US-118 (profil frontend) → US-119 et US-120 (tests). US-114 peut etre livree a tout moment (aucune dependance). Spec de reference : `docs/superpowers/specs/2026-03-22-oauth-login-design.md`.

---

## ~~E17 — Dimensions et formes des planches~~ FUSIONNE DANS E03

> Epique fusionnee dans E03 (editeur graphique). Les stories US-121/122/123 sont desormais dans E03.

---

## E18 — Page d'accueil / tableau de bord

> Offrir au jardinier une page d'accueil unifiee (`/home`) regroupant les actions du moment, la liste de ses jardins, des raccourcis vers les outils cles (Associations, Guildes), et une plante du jour. La route `/` redirige desormais vers `/home`.

| ID | Titre | Priorite | Points | Statut |
|----|-------|----------|--------|--------|
| [US-200](epics/E17-page-accueil/US-200-page-accueil.md) | Page d'accueil avec jardins, actions, raccourcis et plante du jour | Indispensable | 8 | A faire |
| [US-201](epics/E17-page-accueil/US-201-conseils-saisonniers.md) | Conseils permaculturels saisonniers par demi-mois | Important | 5 | A faire |

**Total E18 : 13 points (0 Termine / 13 A faire)**

---

## Recapitulatif

| Epique | Points | Termine | En cours | A faire |
|--------|--------|---------|----------|---------|
| E01 — Gestion des jardins et planches | 31 | 0 | 8 | 23 |
| E02 — Associations vegetales (residuel) | 8 | 0 | 0 | 8 |
| E03 — Editeur graphique du jardin | 39 | 0 | 0 | 39 |
| E04 — Gestion des rotations de cultures | 30 | 0 | 0 | 30 |
| ~~E05 — Plantes compagnes (refonte)~~ | ~~19~~ | — | — | — |
| E06 — Refonte UI permaculture | 27 | 27 | 0 | 0 |
| E07 — Mes plantes | 17 | 15 | 0 | 2 |
| E08 — Finitions UX Associations | 14 | 9 | 0 | 5 |
| E09 — Conscience de l'enracinement | 9 | 6 | 0 | 3 |
| E10 — Calendrier cultural | 46 | 33 | 0 | 13 |
| E11 — Guildes, mecanismes et refonte editeur | 44 | 36 | 0 | 8 |
| E12 — Adaptation climatique | 16 | 0 | 0 | 16 |
| E13 — Infrastructure i18n (backend) | 38 | 0 | 0 | 38 |
| E14 — UX i18n (frontend) | 16 | 0 | 0 | 16 |
| E15 — Assistant de creation de guilde | 19 | 0 | 0 | 19 |
| E16 — Authentification OAuth | 41 | 0 | 0 | 41 |
| ~~E17 — Dimensions et formes des planches~~ | ~~10~~ | — | — | — |
| E18 — Page d'accueil / tableau de bord | 13 | 0 | 0 | 13 |
| **Total (actif)** | **398** | **126** | **8** | **264** |

---

*Backlog gere par l'agent Product Owner — derniere mise a jour : 2026-03-31 (ajout US-201)*
