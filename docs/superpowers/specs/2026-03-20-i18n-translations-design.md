# Design — i18n & Translations Support

**Date :** 2026-03-20
**Epics :** E13 (Infrastructure i18n — backend), E14 (UX i18n — frontend)

---

## Context

Garden Assistant is currently French-only. All UI labels use `ngx-translate` with a single `fr.json` file. Domain data (plant names, guild descriptions, harvest readiness criteria) is stored in French in the database via seed files.

The goal is to add multi-language support with French as default and English as the first additional language, with an open architecture allowing any language to be added later.

## Design Decisions

### Content split

| Content type | Storage | Examples |
|---|---|---|
| UI labels, buttons, navigation, error messages, badge descriptions | Static JSON files (`public/i18n/{lang}.json`) | `Nav.Companions`, `Snackbar.GardenCreated`, `BadgeInfo.Family.Solanaceae.Description` |
| Domain data from the API | Database `translations` table | Plant names/descriptions, guild names/descriptions, harvest readiness criteria |
| User-generated content | Raw in entity (no translation) | Custom guild names, user notes |

### Database schema

**Single generic translation table** — one table for all entity types.

```
languages
├── code (PK, varchar 10) — e.g. "fr", "en"
├── name (varchar 100) — e.g. "Francais", "English"
└── is_default (bool) — exactly one row is true

translations
├── id (Guid, PK)
├── entity_type (varchar 100) — e.g. "Plant", "Guild", "HarvestReadiness"
├── entity_id (Guid)
├── field (varchar 100) — e.g. "Name", "Description"
├── language_code (varchar 10, FK -> languages.code)
├── value (text)
└── UNIQUE(entity_type, entity_id, field, language_code)
```

### Translation resolution strategy

1. Look up requested language in `translations` table
2. If not found, look up default language (`is_default = true`)
3. If not found, return the raw entity field value (backwards compatible)

This means existing entity fields (Name, Description, etc.) remain as-is and serve as the ultimate fallback. The `translations` table stores overrides.

### API contract

- Clients send `Accept-Language` header (e.g. `fr`, `en`)
- An ASP.NET Core middleware or service reads this header and makes it available to the translation service
- Existing endpoints return translated content transparently — no API contract changes needed
- No dedicated translation CRUD endpoints in this iteration

### Frontend integration

- `en.json` mirrors `fr.json` structure exactly
- Language switcher in the shell header (FR/EN toggle)
- Choice persisted in `localStorage` key `lang`, default `fr`
- HTTP interceptor sets `Accept-Language` header on all API calls
- User-generated content (custom guilds, notes) displayed as-is

### Entities requiring translation

| Entity | Translatable fields |
|---|---|
| Plant | Name, Description, Family, Genus |
| Guild | Name, Description |
| HarvestReadiness | Description |
| HarvestReadinessCriterion | Description |
| PlantAssociation | Notes |
| PlantAction | Notes |

**Excluded:** Garden, Planting, PlantingEntry — these are user-scoped data with user-generated content. `ScientificName` is excluded as it is Latin and language-independent.

## Epic Breakdown

### E13 — Infrastructure i18n (38 points)

| ID | Titre | Priorite | Points |
|----|-------|----------|--------|
| US-084 | Table `languages` et seed FR + EN | Indispensable | 2 |
| US-085 | Table generique `translations` | Indispensable | 3 |
| US-086 | Service de traduction (CRUD + resolution par langue) | Indispensable | 5 |
| US-087 | Migrer les seed plantes FR dans `translations` | Indispensable | 3 |
| US-088 | Migrer les seed guildes FR dans `translations` | Indispensable | 2 |
| US-089 | Migrer les seed maturite/criteres FR dans `translations` | Important | 2 |
| US-101 | Migrer les seed associations FR dans `translations` | Indispensable | 2 |
| US-102 | Migrer les seed actions culturales FR dans `translations` | Important | 2 |
| US-090 | Seed des traductions EN pour les plantes | Indispensable | 5 |
| US-091 | Seed des traductions EN pour guildes, maturite, associations et actions | Important | 5 |
| US-092 | API: header `Accept-Language` et resolution dans les endpoints existants | Indispensable | 5 |
| US-093 | Fallback langue par defaut (FR) quand traduction absente | Indispensable | 2 |

### E14 — UX i18n (16 points)

| ID | Titre | Priorite | Points |
|----|-------|----------|--------|
| US-094 | Audit et nettoyage de `fr.json` | Important | 2 |
| US-095 | Creer `en.json` avec toutes les traductions UI en anglais | Indispensable | 3 |
| US-096 | Selecteur de langue dans le shell (header) | Indispensable | 3 |
| US-097 | Persistance du choix de langue (localStorage) | Indispensable | 1 |
| US-098 | Envoyer `Accept-Language` dans tous les appels API | Indispensable | 2 |
| US-099 | Afficher les donnees traduites de l'API (plantes, guildes, maturite) | Indispensable | 3 |
| US-100 | Gestion du contenu utilisateur non traduit (guildes personnalisees, notes) | Important | 2 |

**Total: 54 points (38 + 16)**
