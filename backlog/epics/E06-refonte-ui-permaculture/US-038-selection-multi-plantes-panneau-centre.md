## [US-038] Selection multi-plantes avec fiches detail au centre

**En tant que** jardinier,
**je veux** cliquer sur des plantes du catalogue pour les ajouter au panneau central sous forme de fiches detail,
**afin de** examiner les caracteristiques de plusieurs plantes simultanement et comparer leurs profils.

### Criteres d'acceptation

- [ ] CA1 : Cliquer sur une plante dans le catalogue (panneau gauche) l'ajoute a la selection et affiche sa fiche detail dans le panneau central.
- [ ] CA2 : Chaque fiche detail affiche : icone (lettre initiale), nom, nom latin, badge de famille, badges ensoleillement et arrosage, description, et une grille de caracteristiques (hauteur, cycle de vie, enracinement, fixateur d'azote).
- [ ] CA3 : Plusieurs plantes peuvent etre selectionnees simultanement ; chaque nouvelle selection ajoute une fiche en dessous des precedentes.
- [ ] CA4 : Cliquer sur une plante deja selectionnee la retire de la selection et supprime sa fiche du panneau central.
- [ ] CA5 : Chaque fiche a un bouton de fermeture (x) permettant de retirer la plante de la selection.
- [ ] CA6 : Le panneau central est scrollable si le contenu depasse la hauteur visible.
- [ ] CA7 : Les valeurs des enums backend (`SunRequirement`, `WaterNeeds`, `RootDepth`, `LifeCycle`) sont traduites en texte lisible en francais.
- [ ] CA8 : Un bouton "Tout effacer" permet de reinitialiser la selection en un clic (visible des que 2+ plantes sont selectionnees).

### Notes & contraintes
- La selection est un signal Angular (`WritableSignal<PlantDto[]>`) partage entre les trois panneaux.
- La fiche detail utilise uniquement les donnees de `PlantDto` (pas d'appel API supplementaire).
- La fiche apparait avec une animation fade-in subtile.
- Les cles de traduction `Companions.RemovePlant` et `Companions.ClearAll` existent deja.

### Estimation
- **Priorite :** Must
- **Points :** 5
- **Statut :** Done
