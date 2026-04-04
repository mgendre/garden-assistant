# Garden Assistant

Permaculture garden management application — Angular 21 + .NET 10 + PostgreSQL 17.

## Nouveautés

Grosse mise a jour du 3 avril : le catalogue de plantes a ete entierement repense avec des filtres en panneau lateral, les 159 plantes affichent desormais leurs preferences de sol et de pH, et les seeders passent en mode upsert pour que votre catalogue se mette a jour automatiquement sans ecraser vos modifications.

- [Nouveautes pour les jardiniers](changelogs/users/2026-04-03-recap.md)
- [Changelog technique](changelogs/devs/2026-04-03-recap.md)

## Documentation

- [Documentation overview](docs/README.md)

## Quick start

> **Just want to run the app?** You only need Podman. See [One-command startup](#one-command-startup).

### Prerequisites (local development)

- .NET 10 SDK
- Node.js 20+
- Podman

### Database

Start PostgreSQL via Podman Compose from the repository root:

```bash
cp .env.example .env
# Edit .env with your credentials
podman compose up -d db
```

### Backend

```bash
dotnet user-secrets --project garden-assistant-api set "ConnectionStrings:Default" "Host=localhost;Database=garden_assistant;Username=garden_user;Password=changeme"
dotnet user-secrets --project garden-assistant-api set "Jwt:Key" "your-secret-key-minimum-32-characters"
dotnet run --project garden-assistant-api
```

### Frontend

```bash
npm install --prefix garden-assistant-app
npm run start --prefix garden-assistant-app
```

The application is available at `http://localhost:4200`.

## Hosting

The `hosting/` directory contains a production-ready Podman Compose setup with PostgreSQL, the .NET API, the Angular frontend, and Traefik as a reverse proxy. The only prerequisite is **Podman**.

### One-command startup

The startup script creates a `.env` file with generated secrets and starts all services:

```bash
# Linux / macOS
./hosting/start.sh

# Windows (PowerShell)
.\hosting\start.ps1
```

On first run, the script generates secure random values for `JWT_KEY` and `POSTGRES_PASSWORD`. Edit `hosting/.env` afterwards to set `APP_DOMAIN` and `ACME_EMAIL` if you need TLS.

### Manual setup

All hosting commands run from the `hosting/` directory:

```bash
cd hosting
cp .env.prod.example .env
```

Edit `.env` with your values:

| Variable | Description |
|---|---|
| `POSTGRES_DB` | Database name |
| `POSTGRES_USER` | Database user |
| `POSTGRES_PASSWORD` | Database password |
| `APP_DOMAIN` | Domain name (e.g. `garden.example.com`) |
| `JWT_KEY` | JWT signing key (minimum 32 characters) |
| `ACME_EMAIL` | Email for Let's Encrypt certificates (TLS only) |

Start the full stack (HTTP, no TLS):

```bash
podman compose up -d --build
```

The application is available at `http://<APP_DOMAIN>:8080`.

Start with TLS via Let's Encrypt:

```bash
podman compose -f docker-compose.yaml -f docker-compose.tls.yaml up -d --build
```

The application is available at `https://<APP_DOMAIN>:8443`.

To force a complete rebuild from scratch (no cache):

```bash
podman compose down
podman compose build --no-cache
podman compose up -d
```
