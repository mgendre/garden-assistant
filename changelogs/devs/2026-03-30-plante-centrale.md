---
date: 2026-03-30
title: "Role central dans les guildes — backend, store et UI"
---

### Backend

- Nouvel enum `GuildPlantRole` (`Central`, `Companion`) dans `Data/Entities/Enums/`
- Propriete `Role` ajoutee a l'entite `GuildPlant` avec migration `AddGuildPlantRole`
- DTOs mis a jour : `GuildPlantMemberDto` expose le role, nouveau `GuildPlantRequest` remplace la liste de `Guid` dans `CreateGuildRequest` et `UpdateGuildRequest`
- `GuildService` : tri des plantes centrales en premier (`OrderByDescending(Role)`), persistence du role a la creation et mise a jour

### Frontend

- `CompanionStore` : nouveaux signaux `centralPlantIds`, `hasCentralPlants`, `sortedSelectedPlants` ; methodes `toggleCentralPlant()`, `isCentralPlant()`
- `PlantCard` : inputs `showCentralToggle`, `isCentral`, `showCentralIndicator` ; etoile doree cliquable (FontAwesome `faStar` / `faStarRegular`) avec cible tactile 44x44px
- `PlantDetailPanel` : tri des plantes centrales en tete, liseré or (`plant-detail--central`), encadre d'aide contextuel quand aucune plante centrale designee
- `PlantAssociationPanel` : nouveau composant reutilisable pour afficher les associations en lecture seule (prevu aussi pour le detail de planche US-125)
- Nouvelles cles de traduction : `Guild.CentralPlant`, `Guild.CentralPlantHint`, `Guild.ToggleCentral`, `Guild.ToggleCompanion`
