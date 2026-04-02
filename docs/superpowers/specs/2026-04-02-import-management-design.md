# Import Management & Plant Variants — Design Spec

**Date:** 2026-04-02
**Goal:** Transform the seed system from "insert if empty" to "upsert by key" with protection for manually customized plants, and allow users to create personal plant variants from the catalogue.

---

## 1. Overview

The current seed system inserts plants only when the database is empty. This means seed data improvements (corrections, new fields, new plants) require a full DB reset to take effect.

This epic introduces:
1. **Upsert seeding** — seeders update existing plants from JSON, matching by a stable `Key`
2. **Customization lock** — plants modified by an admin or owned by a user are protected from seed overwrites
3. **User plant variants** — users can create personal variants based on catalogue plants
4. **Admin catalogue management** — (separate epic) admin interface for editing the global catalogue

---

## 2. Data Model Changes

### New fields on `Plant`

| Field | Type | Default | Description |
|---|---|---|---|
| `Key` | `string` | required | Stable business key for seed matching (e.g. `tomate-cerise`). Unique among catalogue plants |
| `IsCustomized` | `bool` | `false` | `true` = protected from seed upsert. Set automatically on admin edit or user variant creation |
| `UserId` | `Guid?` | `null` | `null` = catalogue plant (global). Non-null = user variant (private) |

### Plant states

| State | `IsCustomized` | `UserId` | `ParentPlantId` | Seed behavior |
|---|---|---|---|---|
| Catalogue plant (unmodified) | `false` | `null` | `null` or parent species | Upserted |
| Catalogue plant (admin-modified) | `true` | `null` | unchanged | Skipped |
| User variant | `true` | `<guid>` | catalogue plant | Skipped |

### Indexes

- Unique index on `Key` filtered `WHERE user_id IS NULL` — catalogue plants must have unique keys
- Index on `UserId` for querying user variants

### Foreign keys

- `UserId` → `users.id` (cascade delete)
- `ParentPlantId` → `plants.id` (existing, already configured)

### User variant cascade

Deleting a user variant cascades to all dependent entities (plantings, planting entries, guild memberships).

---

## 3. Seeder Upsert Logic

### General flow per seeder

```
for each entry in JSON:
  1. Find plant in DB by Key
  2. If not found → INSERT
  3. If found AND IsCustomized == false → compare fields, UPDATE changed fields
  4. If found AND IsCustomized == true → SKIP
```

### Logging

- **Info** — when a plant is updated, listing changed fields:
  `Plant "Tomate" (key: tomate) updated — HeightAtMaturityCm: 150 → 180, WaterNeeds: Low → Medium`
- **Debug** (with `if (logger.IsEnabled(LogLevel.Debug))` guard) — when a plant is skipped:
  `Plant "Basilic" (key: basilic) skipped — IsCustomized`

### Related entities (associations, actions, mechanisms, harvest readiness)

Before upserting related entities, the seeder builds a set of locked `PlantId`s (where `IsCustomized == true`). Any related entity referencing a locked plant is skipped. Others are upserted normally.

Same logging rules apply to related entities.

### Guilds

Guilds are **always upserted** (name, description). Guild-plant links (`GuildPlant`) referencing locked plants are skipped; others are upserted.

### Key matching

All seeders match JSON entries to DB entities via the `Key` field on `Plant`. The current indirect matching (key → name → DB lookup) is replaced by direct `Key` lookup in the database.

---

## 4. IsCustomized Marking

### Automatic marking

- **Admin edit via API**: when a catalogue plant (`UserId == null`) is updated through the API, the service layer sets `IsCustomized = true` transparently. No manual flag management.
- **User variant creation**: `IsCustomized` is set to `true` at creation time.

### Unlocking (admin only)

A dedicated admin endpoint allows setting `IsCustomized = false` on a catalogue plant, re-submitting it to seed upserts on next startup.

### No admin API in this epic

The admin modification endpoints and UI are a **separate epic** (see Section 7). This epic only adds the `IsCustomized` field and the seeder logic that respects it.

---

## 5. User Plant Variants

### Creation

A user selects a catalogue plant and creates a variant:
- `ParentPlantId` = catalogue plant ID
- `UserId` = current user
- `IsCustomized = true`
- Fields are copied from the parent; user modifies what they want

### Visibility

- Users see the global catalogue + their own variants
- Variants are invisible to other users

### Usage

A variant is a `Plant` — it can be used anywhere a catalogue plant is used (plantings, personal guilds, etc.).

### Deletion

Cascade delete: removing a variant removes all dependent plantings, planting entries, and guild memberships for that variant.

---

## 6. EF Core Migration

A new EF Core migration adds:
- `Key` column (`string`, not null) on `plants`
- `IsCustomized` column (`bool`, default `false`) on `plants`
- `UserId` column (`Guid?`, nullable) on `plants` with FK to `users`
- Unique index on `Key` filtered where `user_id IS NULL`
- Index on `UserId`

No data migration logic — the database is rebuilt from scratch and the seed repopulates everything.

---

## 7. Related Epics (out of scope)

### Admin Catalogue Management (new epic)

- Admin endpoints for CRUD on catalogue plants (auto-sets `IsCustomized = true`)
- Admin endpoint to unlock a plant (`IsCustomized = false`)
- Admin UI for managing the plant catalogue
- Admin UI for managing associations, guilds, actions, harvest readiness

### User Variant UI (separate epic)

- Frontend for creating/editing/deleting user variants
- Variant display in plant catalogue with visual distinction from catalogue plants

---

## 8. Decisions Log

| Decision | Choice | Rationale |
|---|---|---|
| Lock granularity | Per entity (not per field) | Simpler; if an admin touched a plant, they own it entirely |
| User variants vs separate table | Reuse `Plant` table with `UserId` | Simpler; variants work everywhere plants work, no new joins |
| Seed behavior on user variants | Ignored (never touched) | Private user data, seed has no authority |
| Variant deletion | Cascade | Simple; accepted trade-off of losing dependent data |
| Key for matching | `Key` field from JSON | Stable business identifier, independent of name changes |
| Logging | Info for changes, Debug for skips | Visibility on what changed without noise from skips in production |
| Debug log guard | `IsEnabled(LogLevel.Debug)` check | Avoid string formatting overhead when debug is disabled |
| DB migration | Fresh DB, no data migration | PO decision — simpler, seed repopulates everything |
| Variants are only variants | No standalone user plants | Quality anchored to catalogue; associations inherited via parent |
| Guild upsert | Always upsert guilds | Guilds are structural; only plant links respect the lock |
