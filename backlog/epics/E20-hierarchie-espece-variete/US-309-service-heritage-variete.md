## [US-309] Heritage des proprietes et mecanismes au niveau service

**En tant que** jardinier,
**je veux** que lorsqu'une variete est consultee, ses proprietes taxonomiques et ses mecanismes intrinseques soient automatiquement herites de l'espece parente,
**afin de** voir des donnees completes sans que chaque variete doive tout redefinir.

### Criteres d'acceptation

- [ ] CA1 : Quand une variete est chargee par le `PlantService`, les champs `Family`, `Genus` et `ScientificName` sont resolus depuis le parent si non definis sur la variete. `ScientificName` est une exception : il est toujours pris sur la variete si present (pour indiquer la sous-espece).
- [ ] CA2 : Les `IntrinsicMechanisms` d'une variete sont TOUJOURS ceux du parent — le service retourne les mecanismes du parent, jamais ceux de la variete (qui n'en a pas en base).
- [ ] CA3 : Les proprietes culturales (`LifeCycle`, `HeightAtMaturityCm`, `RootDepth`, `SunRequirement`, `WaterNeeds`, `MaxAltitudeM`, `PropagationMethod`, `FrostSensitive`) sont prises sur la variete si definies (non null / non default), sinon heritees du parent.
- [ ] CA4 : Les `Actions` (calendrier cultural) sont prises sur la variete si elle en a, sinon heritees du parent.
- [ ] CA5 : La logique d'heritage est extraite dans une methode dediee (ex: `ResolveVarietyProperties`) pour etre testable unitairement.
- [ ] CA6 : Les tests unitaires couvrent : variete avec surcharge partielle, variete sans surcharge (heritage complet), plante sans parent (aucun heritage).
- [ ] CA7 : Pas de requete N+1 — le parent est charge en eager loading (`Include`) quand on charge une variete.

### Notes & contraintes
- La resolution se fait au niveau service, pas au niveau entite (pas de logique dans l'entite).
- `HarvestReadiness` suit la meme regle que les actions : prise sur la variete si presente, sinon heritee.
- L'heritage est en lecture seule — on ne modifie jamais l'entite en base.

### Estimation
- **Priorite :** Indispensable
- **Points :** 5
