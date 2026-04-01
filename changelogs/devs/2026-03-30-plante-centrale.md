---
date: 2026-03-30
title: "Rôle central dans les guildes — backend, store et UI"
---

### Backend

- Nouvel enum `GuildPlantRole` (`Central`, `Companion`) dans `Data/Entities/Enums/`
- Propriété `Role` ajoutée à l'entité `GuildPlant` avec migration `AddGuildPlantRole`
- DTOs mis à jour : `GuildPlantMemberDto` expose le rôle, nouveau `GuildPlantRequest` remplace la liste de `Guid` dans `CreateGuildRequest` et `UpdateGuildRequest`
- `GuildService` : tri des plantes centrales en premier (`OrderByDescending(Role)`), persistance du rôle à la création et mise à jour

### Frontend

- `CompanionStore` : nouveaux signaux `centralPlantIds`, `hasCentralPlants`, `sortedSelectedPlants` ; méthodes `toggleCentralPlant()`, `isCentralPlant()`
- `PlantCard` : inputs `showCentralToggle`, `isCentral`, `showCentralIndicator` ; étoile dorée cliquable (FontAwesome `faStar` / `faStarRegular`) avec cible tactile 44x44px
- `PlantDetailPanel` : tri des plantes centrales en tête, liseré or (`plant-detail--central`), encadré d'aide contextuel quand aucune plante centrale désignée
- `PlantAssociationPanel` : nouveau composant réutilisable pour afficher les associations en lecture seule (prévu aussi pour le détail de planche US-125)
- Nouvelles clés de traduction : `Guild.CentralPlant`, `Guild.CentralPlantHint`, `Guild.ToggleCentral`, `Guild.ToggleCompanion`
