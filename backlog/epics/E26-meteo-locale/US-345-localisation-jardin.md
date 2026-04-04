## [US-345] Localisation du jardin (latitude / longitude)

**En tant que** jardinier,
**je veux** renseigner la localisation de mon jardin (ville ou coordonnees GPS),
**afin que** l'application puisse recuperer les donnees meteo locales pour adapter ses recommandations.

### Criteres d'acceptation

- [ ] CA1 : Deux champs `Latitude` (decimal nullable) et `Longitude` (decimal nullable) sont ajoutes sur l'entite `Garden` avec une migration EF.
- [ ] CA2 : Le formulaire de creation/modification du jardin propose un champ texte "Ville ou code postal" avec autocompletion via l'API Open-Meteo Geocoding (`https://geocoding-api.open-meteo.com/v1/search`).
- [ ] CA3 : La selection d'un resultat remplit automatiquement latitude, longitude et affiche le nom de la ville selectionne.
- [ ] CA4 : Un bouton "Utiliser ma position" declenche la geolocalisation du navigateur (`navigator.geolocation`). Si l'utilisateur refuse, un message discret l'invite a saisir manuellement. Aucune insistance.
- [ ] CA5 : La localisation est optionnelle. Un jardin sans localisation conserve le comportement actuel (pas de meteo).
- [ ] CA6 : Le `GardenDto` expose `Latitude`, `Longitude` et `LocationName` (string nullable, le nom de la ville).
- [ ] CA7 : Les coordonnees sont arrondies a 2 decimales (precision ~1 km, suffisante pour la meteo et respectueuse de la vie privee).
- [ ] CA8 : Tests unitaires : validation des coordonnees (latitude -90/+90, longitude -180/+180), arrondi, et cas null.

### Notes & contraintes
- Open-Meteo Geocoding est gratuit et sans cle API.
- Le champ `LocationName` est stocke en base pour eviter un appel geocoding inverse a chaque affichage.
- RGPD : la geolocalisation navigateur est purement optionnelle, declenchee uniquement par action utilisateur explicite.
- Cette story est prerequise a toutes les stories meteo de l'epic.

### Estimation
- **Priorite :** Indispensable
- **Points :** 5
