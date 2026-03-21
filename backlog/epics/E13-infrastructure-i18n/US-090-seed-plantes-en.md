## [US-090] Seed des traductions EN pour les plantes

**En tant que** jardinier anglophone,
**je veux** que les noms, descriptions, familles et genres des plantes soient disponibles en anglais,
**afin de** pouvoir utiliser l'application dans ma langue.

### Criteres d'acceptation

- [ ] CA1 : Un fichier de seed ou un complement au seed existant charge les traductions anglaises des champs `Name`, `Description`, `Family` et `Genus` de chaque plante.
- [ ] CA2 : Toutes les plantes presentes en base disposent de traductions anglaises completes.
- [ ] CA3 : Les traductions sont botaniquement correctes — les noms communs anglais correspondent aux especes (ex. "Tomato" pour Solanum lycopersicum, "Basil" pour Ocimum basilicum).
- [ ] CA4 : Le seed est idempotent.
- [ ] CA5 : Les descriptions anglaises sont informatives et de qualite equivalente aux descriptions francaises.
- [ ] CA6 : Les tests unitaires verifient que toutes les plantes ont leurs traductions EN chargees.

### Notes & contraintes
- Les traductions anglaises sont produites ou validees par l'agent `plant-expert`.
- `ScientificName` est exclu (latin, independant de la langue).
- Les familles et genres ont des noms anglais courants (ex. Solanaceae reste "Solanaceae", mais Family "Solanacees" en FR -> "Nightshade family" en EN si utilise en contexte vulgarise). Aligner sur le choix existant dans `fr.json` / `BadgeInfo.Family`.

### Estimation
- **Priorite :** Indispensable
- **Points :** 5
