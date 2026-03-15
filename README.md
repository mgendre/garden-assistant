# Garden Assistant

Application de gestion de jardin en permaculture — Angular + .NET 10 + PostgreSQL.

## Documentation

- [Vue d'ensemble de la documentation](docs/README.md)

## Démarrage rapide

### Prérequis

- .NET 10 SDK
- Node.js 20+
- PostgreSQL 17
- Podman

### Backend

```bash
cd garden-assistant-api
dotnet user-secrets set "ConnectionStrings:Default" "Host=localhost;Database=garden_assistant;Username=postgres;Password=yourpassword"
dotnet user-secrets set "Jwt:Key" "votre-clé-secrète-minimum-32-caractères"
dotnet run
```

### Frontend

```bash
cd garden-assistant-app
npm install
npm start
```

L'application est accessible sur `http://localhost:4200`.
