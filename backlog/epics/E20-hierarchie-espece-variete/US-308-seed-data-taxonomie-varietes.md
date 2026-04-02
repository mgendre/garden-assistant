## [US-308] Correction taxonomique et ajout des varietes dans les seeds

**En tant que** jardinier,
**je veux** que le catalogue distingue les especes de leurs varietes (courgette = variete de courge, butternut = variete de courge musquee, etc.),
**afin de** comprendre les liens botaniques entre les plantes et beneficier de donnees plus precises.

### Criteres d'acceptation

- [ ] CA1 : L'entree `courge` dans `plants.json` est corrigee : elle represente l'espece Cucurbita pepo (et non le genre Cucurbita au sens large). Famille, genre, nom scientifique sont corrects.
- [ ] CA2 : `courgette` (Cucurbita pepo var. cylindrica) est liee a `courge` via `parentKey: "courge"`.
- [ ] CA3 : Une entree `courge-musquee` (Cucurbita moschata) est ajoutee comme espece a part entiere (pas une variete de courge).
- [ ] CA4 : `butternut` (Cucurbita moschata var. butternut) est ajoutee comme variete de `courge-musquee`.
- [ ] CA5 : `potimarron` (Cucurbita maxima var. potimarron) est ajoute comme variete de `potiron` (Cucurbita maxima, deja present).
- [ ] CA6 : `patisson` (Cucurbita pepo var. clypeata) est ajoute comme variete de `courge`.
- [ ] CA7 : Les varietes heritent de la taxonomie de leur parent (Family, Genus) — seul `ScientificName` peut differer pour preciser la variete.
- [ ] CA8 : Les varietes peuvent surcharger les proprietes culturales (HeightAtMaturityCm, WaterNeeds, etc.) ou les laisser null pour heriter du parent.
- [ ] CA9 : Les varietes n'ont PAS de `intrinsicMechanisms` propres dans le seed — elles heritent de ceux du parent.
- [ ] CA10 : Les varietes n'ont PAS d'associations propres dans `associations.json`.
- [ ] CA11 : Le seed s'applique sans erreur et les nouvelles plantes apparaissent dans la base.

### Notes & contraintes
- Validation botanique requise avec l'agent `plant-expert` / `plant-encyclopedia` avant merge.
- Les varietes de courges sont un cas emblematique en permaculture car souvent confondues. Ce seed doit clarifier la taxonomie.
- `potiron` (Cucurbita maxima) existe deja dans le seed — il ne faut pas le modifier en variete, c'est une espece distincte.

### Estimation
- **Priorite :** Indispensable
- **Points :** 5
