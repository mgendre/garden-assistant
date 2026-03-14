kloi# Associations de plantes — Modèle de données

## Pourquoi ce modèle ?

Le compagnonnage végétal (companion planting) est au cœur de la permaculture. Deux plantes voisines interagissent via des mécanismes biologiques précis : exsudats racinaires, composés volatils, attraction d'insectes, fixation d'azote… Ces interactions ne sont **ni binaires ni symétriques**.

Le modèle de données a été conçu avec l'aide d'un expert en permaculture pour refléter cette réalité.

---

## Principes fondamentaux

### 1. Une association par mécanisme

Une même paire de plantes peut avoir **plusieurs mécanismes actifs simultanément**. Par exemple, le basilic planté près de la tomate :

- repousse les aleurodes par confusion olfactive
- attire des pollinisateurs bénéfiques pour la nouaison
- améliorerait le goût des fruits (usage traditionnel)

Chaque mécanisme est stocké dans **une ligne séparée** dans `plant_associations`. Cela permet de requêter par mécanisme, d'attacher un niveau de confiance scientifique à chaque effet, et d'étendre le modèle sans migration destructive.

### 2. Les associations sont directionnelles

Une association n'est **pas symétrique**. L'effet de A sur B n'est pas le même que l'effet de B sur A.

| Source | Cible | Mécanisme | Effet |
|---|---|---|---|
| Haricot | Maïs | NitrogenFixation | Beneficial |
| Maïs | Haricot | PhysicalSupport | Beneficial |
| Capucine | Rosier | TrapCrop | Beneficial (pour le rosier) |
| Fenouil | Tomate | RootAllelopathy | Harmful |

Le modèle utilise `SourcePlantId` et `TargetPlantId` explicites. Pas de symétrie forcée.

### 3. La distance compte

Un effet allélopathique racinaire n'a de sens qu'au contact ou à moins de 50 cm. Une confusion olfactive peut opérer jusqu'à 2 mètres. Le champ `DistanceEffect` encode cette réalité pour permettre de valider qu'une association déclarée est effectivement activée dans le plan spatial de l'utilisateur.

---

## Schéma de la table `plant_associations`

```
Id              Guid  PK
SourcePlantId   Guid  FK → plants   (la plante qui produit l'effet)
TargetPlantId   Guid  FK → plants   (la plante qui reçoit l'effet)
Mechanism       enum  voir ci-dessous
Effect          enum  Beneficial | Harmful | Neutral
DistanceEffect  enum  Contact | Short | Medium | Field
ConfidenceLevel enum  Anecdotal | FieldObserved | PeerReviewed
Notes           string?
```

**Contrainte unique sur `(SourcePlantId, TargetPlantId, Mechanism)`** : un seul enregistrement par mécanisme et par paire directionnelle.

---

## Mécanismes disponibles

| Mécanisme | Description | Exemple |
|---|---|---|
| `OlfactoryConfusion` | Composés volatils qui brouillent les ravageurs | Basilic → Tomate (aleurodes) |
| `PollinatorAttraction` | Fleurs qui attirent abeilles, bourdons, syrphes | Bourrache → Cucurbitacées |
| `TrapCrop` | Plante sacrifiée pour attirer les ravageurs loin de la cible | Capucine → Rosier (pucerons) |
| `RootAllelopathy` | Exsudats racinaires inhibant la germination ou la croissance | Fenouil → Tomate |
| `AerialRepulsion` | Terpènes volatils repoussant les insectes | Tagète → Tomate (aleurodes) |
| `NitrogenFixation` | Légumineuses enrichissant le sol en azote via rhizobiums | Haricot → Maïs |
| `PredatorAttraction` | Attire les auxiliaires (coccinelles, chrysopes) | Capucine → voisins (via pucerons) |
| `PhysicalSupport` | Support structurel (tuteur vivant) | Maïs → Haricot grimpant |
| `SoilCover` | Couvre le sol, limite l'évaporation et les adventices | Courge → Maïs+Haricot |
| `DynamicAccumulation` | Remonte les minéraux profonds vers la surface | Consoude → voisins |

---

## Catalogue de plantes (`plants`)

La table `plants` est un catalogue global (non lié à un utilisateur). Elle contient les données biologiques nécessaires aux recommandations.

### Champs fonctionnels clés

| Champ | Type | Rôle |
|---|---|---|
| `NitrogenFixer` | bool | Détecte les légumineuses à rhizobiums |
| `AllelopathicRisk` | bool | Signal d'alerte avant association |
| `PollinatorPlant` | bool | Valorisation de la biodiversité |
| `RootDepth` | enum | Shallow / Medium / Deep — complémentarité verticale |
| `HeightAtMaturityCm` | int? | Calcul des conflits d'ombrage |
| `LifeCycle` | enum | Annual / Biennial / Perennial — planification temporelle |

---

## Plans de plantation (`plantings` + `planting_entries`)

Un plan de plantation appartient à un utilisateur via son jardin.

### `planting_entries` — une plante dans un plan

```
PlantId          Guid  FK → plants
PositionX / Y    float  coordonnées en mètres
Layer            enum   Canopy | SubCanopy | Shrub | Herbaceous | GroundCover | Climber | Root
PlannedSowDate   DateOnly?
PlannedHarvestDate DateOnly?
ActualHarvestDate  DateOnly?
```

Les strates verticales (`Layer`) permettent de modéliser les forêts-jardins selon les 7 strates de Robert Hart (canopée, sous-canopée, arbustif, herbacé, couvre-sol, grimpant, racinaire).

---

## Score de compatibilité

L'endpoint `GET /api/plantings/{id}/compatibility` calcule le score d'une plantation en croisant `planting_entries` avec `plant_associations` :

```
Beneficial : 4
Harmful    : 1
Neutral    : 2
Total      : 7
```

Ce score permet d'afficher un indicateur visuel et d'alerter l'utilisateur sur les associations problématiques avant qu'il sème.

---

## Exemple : les Trois Sœurs

La guilde traditionnelle amérindienne (maïs + haricot + courge) génère ces associations dans le modèle :

| Source | Cible | Mécanisme | Effet |
|---|---|---|---|
| Haricot | Maïs | NitrogenFixation | Beneficial |
| Maïs | Haricot | PhysicalSupport | Beneficial |
| Courge | Maïs | SoilCover | Beneficial |
| Courge | Haricot | SoilCover | Beneficial |

Score de compatibilité pour une plantation Trois Sœurs : **4 Beneficial, 0 Harmful**.

---

## Niveau de confiance (`ConfidenceLevel`)

| Valeur | Signification |
|---|---|
| `Anecdotal` | Tradition populaire, jardinage empirique |
| `FieldObserved` | Observations de terrain reproductibles |
| `PeerReviewed` | Étude scientifique publiée et révisée par des pairs |

Toujours afficher le niveau de confiance à l'utilisateur pour qu'il puisse pondérer les recommandations.
