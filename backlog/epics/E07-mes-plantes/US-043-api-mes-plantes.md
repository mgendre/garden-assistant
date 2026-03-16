## [US-043] API et persistance de la liste "Mes plantes"

**En tant que** jardinier,
**je veux** que ma liste de plantes personnelle soit sauvegardee sur le serveur,
**afin de** la retrouver a chaque connexion, quel que soit l'appareil utilise.

### Criteres d'acceptation

- [ ] CA1 : Une nouvelle table `user_plants` existe en base avec les colonnes `id` (Guid, PK), `user_id` (Guid, FK vers `users`), `plant_id` (Guid, FK vers `plants`), `created_at` (timestamp). Un index unique empeche les doublons `(user_id, plant_id)`.
- [ ] CA2 : `GET /api/MyPlants` retourne la liste des plantes de l'utilisateur connecte (tableau de `PlantDto`), triee par nom alphabetique.
- [ ] CA3 : `POST /api/MyPlants/{plantId}` ajoute une plante a la liste de l'utilisateur. Retourne 201 si ajout reussi, 409 si la plante est deja presente, 404 si `plantId` n'existe pas.
- [ ] CA4 : `DELETE /api/MyPlants/{plantId}` retire une plante de la liste. Retourne 204 si suppression reussie, 404 si la plante n'est pas dans la liste.
- [ ] CA5 : Tous les endpoints sont proteges par `[Authorize]` et filtrent par `UserId` de l'utilisateur connecte.
- [ ] CA6 : Le service implemente une interface `IMyPlantService` conformement aux conventions du projet.
- [ ] CA7 : Des tests unitaires couvrent les cas nominaux et les cas d'erreur (plante inexistante, doublon, suppression d'une plante absente).

### Notes & contraintes
- L'entite `UserPlant` va dans `Data/Entities/`.
- La migration EF Core doit etre generee (code-first).
- Pas de logique de pagination pour l'instant (YAGNI -- un jardinier aura rarement plus de 50 plantes).

### Estimation
- **Priorite :** Must
- **Points :** 3
