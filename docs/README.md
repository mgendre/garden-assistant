# Documentation — Garden Assistant

## Business domain

- [Plant associations](plant-associations.md) — Data model, biological mechanisms, and companion planting principles

## Security

- [JWT Authentication](authentication.md) — Access token + refresh token, bootstrap and renewal flows

## Technical architecture

| Layer | Technology |
|---|---|
| Frontend | Angular 21, Tailwind CSS v4, Angular Signals |
| Backend | .NET 10 / ASP.NET Core, EF Core Code-First |
| Database | PostgreSQL 17, snake_case via EFCore.NamingConventions |
| Auth | JWT — access token (15 min) + refresh token (30 days) |
| Containers | Podman, rootless |
