# Documentation — Garden Assistant

## Domaine métier

- [Associations de plantes](plant-associations.md) — Modèle de données, mécanismes biologiques, et principes de compagnonnage

## Sécurité

- [Authentification JWT](authentication.md) — Access token + refresh token, flux de démarrage et renouvellement

## Architecture technique

Le projet suit les guidelines définies dans [`CLAUDE.md`](../CLAUDE.md).

| Couche | Technologie |
|---|---|
| Frontend | Angular 21, Tailwind CSS v4, Angular Signals |
| Backend | .NET 10 / ASP.NET Core, EF Core Code-First |
| Base de données | PostgreSQL 17, snake_case via EFCore.NamingConventions |
| Auth | JWT — access token (15 min) + refresh token (30 jours) |
| Conteneurs | Podman, rootless |
