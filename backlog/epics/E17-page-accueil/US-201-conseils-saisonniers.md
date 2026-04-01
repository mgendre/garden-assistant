## [US-201] Conseils permaculturels saisonniers par demi-mois

**En tant que** jardinier utilisant PermaGarden,
**je veux** voir des conseils permaculturels adaptes a la periode en cours (demi-mois),
**afin de** savoir quelles actions prioritaires realiser dans mon jardin selon le rythme de la saison, sans dependre de plantes specifiques.

---

### Criteres d'acceptation

- [ ] CA1 : La section « Conseils du moment » est affichee sur la page d'accueil (`/home`), dans la section « Actions du moment » (US-200), apres les actions culturales des plantes.
- [ ] CA2 : Les conseils affiches correspondent au demi-mois courant (1–24) — la periode est determinee cote client selon la date du jour, par la meme logique que le calendrier existant.
- [ ] CA3 : Chaque conseil est affiche sous forme d'item liste avec une icone representative de sa categorie (paillage, compost, biodiversite, sol, eau, planification).
- [ ] CA4 : Si le demi-mois courant n'a pas de conseils associes, la section n'est pas affichee (absence silencieuse).
- [ ] CA5 : Les conseils sont aussi visibles dans la page calendrier (`/calendar`), dans un panel dedie « Conseils permaculturels », au meme niveau que les widgets « En ce moment » et « Prochainement ».
- [ ] CA6 : Dans le calendrier, les conseils changes au fil de la navigation : ils correspondent au demi-mois actuellement consulte (pas forcement le courant).
- [ ] CA7 : Les donnees sont chargees depuis un fichier JSON statique embarque dans le frontend — aucun appel API backend n'est necessaire.
- [ ] CA8 : Le composant utilise le pattern `.panel` / `.panel-header` / `.panel-title` du projet.
- [ ] CA9 : Toutes les chaines de caracteres de l'interface (titre de section, libelles de categories) utilisent `ngx-translate` sous le namespace `SeasonalTips.*`.
- [ ] CA10 : Le contenu des conseils est en francais et stocke directement dans le JSON (pas via i18n — le contenu est fixe et monolingue dans cette version).

---

### Contenu des conseils (donnees seed JSON)

Les 24 entrees ci-dessous constituent le contenu initial a integrer dans `assets/data/seasonal-tips.json`.
Chaque entree a la structure : `{ "period": number, "tips": [{ "category": string, "text": string }] }`.

Categories disponibles : `mulching` | `compost` | `green-manure` | `soil` | `water` | `frost` | `biodiversity` | `planning`

#### Periode 1 — Premiere quinzaine de janvier

- **planning** : Commencez le bilan de la saison ecoulee : notez ce qui a bien fonctionne, les associations reussies, les echecs. C'est le meilleur moment pour planifier les rotations de l'annee.
- **planning** : Consultez vos notes de semis de l'annee precedente et etablissez un calendrier previsionnel pour les 12 prochains mois.
- **biodiversity** : Verifiez les abris a insectes et hotels a insectes : degagez les entrees obstruees par la neige ou les feuilles mortes.
- **soil** : Si le sol n'est pas gele, profitez-en pour observer la structure : la presence de vers de terre est un bon indicateur de sante du sol apres la saison.

#### Periode 2 — Deuxieme quinzaine de janvier

- **planning** : Passez les commandes de semences aupres de producteurs bio et paysans : les varietes rares partent vite. Privilegiez les varietes anciennes et libres.
- **compost** : Retournez le tas de compost si la temperature le permet. Ajoutez des matieres carbonees (paille, carton) si le tas est trop humide.
- **biodiversity** : Installez ou reparez les nichoirs a oiseaux avant la saison de nidification. Les mesanges sont de precieuses allies contre les pucerons.
- **frost** : Verifiez l'etat des protections hivernales (voiles, paillis epais) sur les plantes sensibles et les vivaces recemment plantees.

#### Periode 3 — Premiere quinzaine de fevrier

- **planning** : Dessinez ou mettez a jour le plan de votre jardin : planifiez les rotations par famille botanique (solanees, cucurbitacees, fabacees, apiacees) sur 4 ans minimum.
- **soil** : Prelabourez mentalement vos planches : identifiez celles qui accueilleront des gros mangeurs (tomates, courges) et prevoyez un apport de compost mur en surface.
- **compost** : Demarrez un nouveau tas de compost avec les residus de taille et les matieres organiques hivernales. Alternez couches brunes et vertes.
- **biodiversity** : Semez des plantes a floraison precoce en godets (pensees, primeveres) pour les premieres abeilles sorties de torpeur.

#### Periode 4 — Deuxieme quinzaine de fevrier

- **soil** : Epandez le compost mur en surface des planches en no-dig : 3 a 5 cm suffisent. Ne bechex pas — laissez les vers integrer les nutriments naturellement.
- **green-manure** : Si une planche est libre et le sol degage, semez une phacelie ou de la moutarde blanche comme engrais vert de printemps. Ils protegent le sol nu et nourrissent les premiers pollinisateurs.
- **planning** : Etablissez votre liste de semis en interieur pour les prochaines semaines : tomates, poivrons et aubergines demandent 8 a 10 semaines avant repiquage.
- **frost** : Sur le Plateau suisse, les gelees tardives sont frequentes jusqu'en avril. Ne sortez aucune plante sensible avant la mi-mai.

#### Periode 5 — Premiere quinzaine de mars

- **soil** : Griffez legerement la surface des planches pour briser la croute et favoriser la germination des semences directes a venir. Ne pas retourner le sol.
- **green-manure** : Enfouissez en superficie (coupez et laissez sur place) les engrais verts semes a l'automne : epinard d'hiver, seigle, phacelie. Attendez 2 semaines avant de semer.
- **compost** : Le compost hivernal commence a se rechauffer : c'est le bon moment pour un retournement energique et un apport d'azote (tontes vertes, marc de cafe).
- **water** : Verifiez l'etat de votre systeme de recuperation d'eau de pluie : nettoyez les filtres et les citernes apres l'hiver.
- **biodiversity** : Laissez une zone de sol nu et ensoleille pour les abeilles solitaires terricoles qui emergent en mars.

#### Periode 6 — Deuxieme quinzaine de mars

- **mulching** : Paillez les planches de fraises, d'asparagus et de vivaces : 5 a 8 cm de paille ou de feuilles broyees. Le paillis conserve l'humidite et inhibe les adventices.
- **soil** : Preparez les futures planches sureleurees en no-dig : superposez carton, compost, paille. Elles seront pretes a planter en mai.
- **green-manure** : Semez de la phacelie entre les rangs des planches encore vides : elle attire les pollinisateurs et peut etre fauchee au stade bouton floral avant repiquage.
- **planning** : Confirmez vos associations de la saison : tomates-basilic, carottes-poireaux, courges-haricots-mais (les trois soeurs). Notez-les dans l'app avant de semer.

#### Periode 7 — Premiere quinzaine d'avril

- **mulching** : Paillez les planches de pommes de terre apres la plantation. Un paillis epais (10 cm) remplace en grande partie le buttage traditionnel.
- **frost** : Preparez des protections (cloches, voiles P17) pour les repiquages precoces : le Plateau suisse connait des gelees nocturnes jusque debut mai.
- **water** : Installez ou verifiez les goutteurs et tuyaux de goutte-a-goutte avant la montee en temperature. L'irrigation localisee reduit la consommation d'eau de 50 %.
- **biodiversity** : Installez un abri a herissons dans un coin peu frequente du jardin : ils consomment limaces et escargots sans traitement chimique.
- **green-manure** : Semez trefle blanc nain dans les alles : il fixe l'azote atmospherique et fournit un couvert permanent benefique aux auxiliaires.

#### Periode 8 — Deuxieme quinzaine d'avril

- **soil** : Avant tout repiquage, verifiez la temperature du sol : les tomates et courges ne se plantent pas en dessous de 12 °C. Utilisez un thermometre de sol.
- **compost** : Le compost produit au printemps precedent est normalement mur : cribler et stocker a l'abri pour les usages d'ete.
- **mulching** : Paillez immediatement apres chaque repiquage : le paillis reduit le stress hydrique des jeunes plants et stabilise la temperature racinaire.
- **biodiversity** : Semez des fleurs melliferes dans les espaces entre planches : bourrache, coriandre, souci, aneth. Elles attirent les auxiliaires et favorisent la pollinisation.
- **planning** : Mettez a jour le plan de rotation dans l'app apres chaque plantation : une trace fiable de ce qui est plante ou evite les erreurs de rotation les annees suivantes.

#### Periode 9 — Premiere quinzaine de mai

- **frost** : Derniere fenetre de risque de gelee sur le Plateau (saints de glace : 11-13 mai). Ne repiquez les tomates, courges et basilic qu'apres le 15 mai.
- **mulching** : C'est le moment cle du paillage massif : couvrez toutes les planches productives avec 8 a 10 cm de matieres organiques. Ce geste est le plus rentable de l'annee en termes d'economie d'eau et de travail.
- **water** : Verifiez la capacite de stockage de vos cuves de recuperation d'eau : les pluies de mai remplissent les reserves pour l'ete.
- **biodiversity** : Installez des confusions sexuelles ou des pieges a pheromones pour le carpocapse (ver des pommes) si vous avez des arbres fruitiers.

#### Periode 10 — Deuxieme quinzaine de mai

- **soil** : Apres les saints de glace, le sol se rechauffe rapidement. C'est le meilleur moment pour un semis direct de haricots, courges et concombres.
- **mulching** : Completez le paillis autour des tomates tuteurees et des cucurbitacees : empechex les eclaboussures de sol sur le feuillage (prevention du mildiou).
- **green-manure** : Dans les espaces entre plants, semez de l'aneth ou de la coriandre : ils se sement et attirent les syrphes et les parasitoïdes des pucerons.
- **compost** : Enrichissez le pied des grandes cultures (tomates, courges) avec une poignee de compost mur et une couche de paillis fraiche. Cette association « couche lasagne » nourrit progressivement.
- **water** : Posez des soucoups d'eau peu profondes pour les insectes auxiliaires et les oiseaux : une mare de jardin, meme miniature, multiplie la biodiversite.

#### Periode 11 — Premiere quinzaine de juin

- **water** : La saison seche commence. Adoptez l'arrosage en profondeur et peu frequent (2 fois par semaine) plutot que quotidien et superficiel : les racines plongent plus profond et la plante devient plus resistante a la secheresse.
- **mulching** : Verifiez l'epaisseur des paillis : ils s'effritent et s'incorporent au sol. Rajoutez une couche si l'epaisseur est descendue sous 5 cm.
- **biodiversity** : Ne tondes pas les alles fleuries : laissez les ombelliferes (carotte sauvage, fenouil) monter en graine pour les insectes.
- **compost** : Demarrez un nouveau tas de compost estival avec les tontes, les restes de cuisine et les mauvaises herbes avant grenaison.

#### Periode 12 — Deuxieme quinzaine de juin

- **water** : Paillez le sol autour des arbres fruitiers : un cercle de 1 m de diametre avec 10 cm de bois raméal fragmente (BRF) ameliore la retention et la vie microbienne.
- **planning** : Planifiez les semis de succession pour l'automne : salades, epinards, radis, maiche. Ils se sement de juillet a septembre selon les varietes.
- **biodiversity** : Laissez quelques pieds de plantes « adventices » utiles : ortie (pucerons-appat pour auxiliaires), achillee (attire les syrphes), consoude (accumulatrice de potasse).
- **soil** : En cas de forte chaleur, le sol se craquelle sous le paillis. Arrosez lentement et profondement pour reamorcer la capillarite.

#### Periode 13 — Premiere quinzaine de juillet

- **water** : Installez un ombrage sur les salades et les epinards : a plus de 28 °C, ils montent en graine rapidement. Un voile d'ombrage 30 % ou une guilde ombragee suffisent.
- **compost** : Le compost chaud de l'ete monte vite en temperature si bien constitue : retournez toutes les 2 semaines et maintenez l'humidite (doit ressembler a une eponge essoree).
- **biodiversity** : Maintenez les mares et points d'eau remplis : les carabes, grenouilles et rainettes chassent efficacement les limaces la nuit.
- **planning** : C'est le moment de semer les engrais verts d'ete pour les planches liberes apres les recoltes precoces (petits pois, epinards) : sarrasin, phacelie.

#### Periode 14 — Deuxieme quinzaine de juillet

- **soil** : Apres recolte d'une planche, ne laissez jamais le sol nu : couvrez immediatement avec un engrais vert, du paillis ou un carton humidifie. Un sol nu perd sa structure et s'epuise.
- **green-manure** : Semez du sarrasin sur les planches libres : il pousse en 6 semaines, attire les pollinisateurs, etouffe les adventices et se fauche avant grenaison.
- **water** : Recuperez l'eau de lavage des legumes pour arroser : evitez le gaspillage en periode de restriction. Orientez les eaux grises non saponnees vers le jardin.
- **compost** : Ajoutez les fanes de tomates saines et les restes de cucurbitacees au compost. Eliminez les tiges malades (mildiou, oïdium) a la poubelle, pas au compost.

#### Periode 15 — Premiere quinzaine d'aout

- **planning** : Preparez le planning des semis d'automne : comptez 8 semaines avant les premieres gelees (fin octobre sur le Plateau) pour les derniers repiquages de salades et choux.
- **mulching** : Renforcez le paillis autour des courges et des tomates : la chaleur d'aout assecherait le sol en quelques jours sans protection.
- **biodiversity** : Observez les auxiliaires presents : syrphes, chrysopes, coccinelles. Leur presence indique un ecosysteme equilibre. Notez vos observations pour evaluer la biodiversite d'une annee sur l'autre.
- **soil** : Plantez des bulbes d'ail a partir de mi-aout pour une recolte precoce l'annee suivante. L'ail est un excellent compagnon de sante du sol.

#### Periode 16 — Deuxieme quinzaine d'aout

- **green-manure** : Semez les engrais verts d'hiver sur les planches qui se liberent : seigle d'hiver, vesce velue, trefle alexandrin. Ils protegent le sol pendant 6 mois.
- **soil** : Incorporez en surface (sans labourer) une couche de compost mur sur les planches qui accueilleront les cruciferes d'automne.
- **water** : Verifiez vos cuves de recup : les premieres pluies de fin aout remplissent les reserves. Nettoyez les filtres apres l'ete.
- **compost** : Le compost estival est souvent trop sec : arrosez et melangez. Un compost bien humide en automne donne un produit fini de qualite pour le printemps.

#### Periode 17 — Premiere quinzaine de septembre

- **planning** : Etablissez le bilan des associations de la saison : quelles combinaisons ont bien fonctionne ? Quelles ont echoue ? Mettez a jour l'app pendant que c'est encore frais.
- **mulching** : Paillez les planches de fraises apres la renovation (suppression des stolons). Une couche de paille protegera les couronnes en hiver.
- **biodiversity** : Laissez quelques plantes monter a graine (basilic, aneth, coriandre, fenouil) : les graines nourriront les oiseaux granivores en hiver et se ressemeront spontanement.
- **soil** : Plantez des bulbes de lumieres d'automne (perce-neige, crocus) dans les alles et sous les arbres : ils fleurissent avant les legumes et nourrissent les premiers pollinisateurs.

#### Periode 18 — Deuxieme quinzaine de septembre

- **green-manure** : Derniere fenetre pour les semis d'engrais verts : apres fin septembre, les temperatures sont trop basses pour une levee rapide sur le Plateau.
- **compost** : Collectez les premieres feuilles d'automne pour enrichir le compost : elles sont tres carbonees, equilibrez avec des matieres azotees (tontes, restes de cuisine).
- **soil** : Testez la structure de vos sols apres la saison : un sol sain s'effrite entre les doigts sans coller ni tomber en poussieres. Si compact, ajoutez du compost et du BRF en surface.
- **planning** : Evaluez les volumes recolt s et comparez aux previsions : l'ecart aide a calibrer les semis de l'annee suivante (quantites et varietes).

#### Periode 19 — Premiere quinzaine d'octobre

- **mulching** : C'est le grand paillage d'automne : couvrez toutes les planches vides avec 10 cm de paille, feuilles broyees ou BRF. Cette couche protegera la vie du sol pendant tout l'hiver.
- **compost** : Formez le grand tas de compost d'automne avec les fanes, les restes de potager et les feuilles. C'est la plus grande collecte de l'annee.
- **biodiversity** : Laissez les tiges creuses des plantes (fenouil, artichaut, rudbeckia) en place jusqu'au printemps : elles abritent les larves d'insectes solitaires.
- **frost** : Rentrez les plantes geleophiles en pots (agrumes, figuiers en caisses) avant les premieres gelees. Sur le Plateau suisse, elles peuvent survenir des mi-octobre.

#### Periode 20 — Deuxieme quinzaine d'octobre

- **planning** : Etablissez la carte definitive des rotations pour l'annee suivante : notez par planche la famille botanique de la culture sortante et la famille prevue pour l'entree.
- **soil** : Plantez les bulbes de printemps (tulipes, narcisses, alliums ornementaux) dans les espaces entre arbres et en bordure de planches : ils ameliorent la biodiversite printaniere.
- **biodiversity** : Construisez ou installez un hotel a insectes avant l'hiver : les chrysopes et coccinelles y passeront l'hiver et seront disponibles au printemps pour lutter contre les pucerons.
- **water** : Vidangez partiellement les cuves de recuperation d'eau pour prevenir les dommages du gel. Laissez un volume tampon ou isolez les cuves exterieures.

#### Periode 21 — Premiere quinzaine de novembre

- **mulching** : Completez le paillage des pieds d'arbres fruitiers et des vivaces sensibles : 15 cm de paille ou de feuilles broyees autour du collet.
- **compost** : Fermez le tas de compost avec une couche de paille ou de carton pour maintenir la chaleur et eviter le lessivage par les pluies hivernales.
- **biodiversity** : Installez des mangeoires a oiseaux : les merles, rouges-gorges et mesanges chasseront les insectes du sol tout l'hiver si vous les fidilisez pres du jardin.
- **soil** : Sur les planches en no-dig, le carton pose a l'automne se degrade pendant l'hiver : verifiez que les bordures sont bien pesees pour que le vent ne les emporte pas.

#### Periode 22 — Deuxieme quinzaine de novembre

- **planning** : Commencez le catalogue de vos semences : listez les varietes en stock, leur date de recolte ou d'achat, et leur taux de germination estime. Eliminez les stocks trop anciens.
- **compost** : Preparez un lombricomposteur d'interieur si vous ne l'avez pas encore : il transforme les restes de cuisine en compost de qualite durant l'hiver, quand le tas exterieur est inactif.
- **biodiversity** : Laissez les feuilles tomber naturellement sous les haies et dans les zones sauvages du jardin : elles constituent un habitat crucial pour les herissons en hibernation.
- **soil** : Par temps sec, observez la couleur du sol : un sol fonce et grumeleux indique une bonne vie microbienne ; un sol gris et compact signal un manque de matiere organique a corriger au printemps.

#### Periode 23 — Premiere quinzaine de decembre

- **planning** : Redigez votre bilan annuel complet : surfaces cultivees, rendements, incidents phytosanitaires, observations ecologiques. Ce document est votre outil de progression d'une annee sur l'autre.
- **biodiversity** : Ne taillez pas les arbres et arbustes maintenant si des graines sont encore presentes : attendez fevrier. Les graines nourrissent les oiseaux jusqu'a la fin de l'hiver.
- **compost** : Le compost hivernal est en dormance : ne le retournez pas, laissez les micro-organismes travailler lentement. Contentez-vous d'ajouter les restes de cuisine.
- **frost** : Verifiez les protections des plantes en pots : un pot gele se fissure. Isolez avec du geotextile ou rentrez en local non chauffé (cave, garage froid).

#### Periode 24 — Deuxieme quinzaine de decembre

- **planning** : C'est le meilleur moment pour lire, se former et s'inspirer : livres de permaculture, retours d'experience de jardiniers, ressources en ligne. Les projets germent en hiver.
- **biodiversity** : Maintenez les mangeoires a oiseaux regulierement approvisionnees : les periodes de grand froid sont critiques pour la survie des auxiliaires hivernants.
- **soil** : Si le sol n'est pas gele, faites un ultime apport de compost en surface des planches les plus epuisees. La pluie et le gel l'integreront naturellement d'ici le printemps.
- **planning** : Faites le point sur vos outils : nettoyez, huiler et affutez les lames. Un outil bien entretenu reduit l'effort et le risque de blesser les plantes.

---

### Notes & contraintes

- **Donnees statiques uniquement** : le contenu est un fichier JSON embarque dans le frontend (`assets/data/seasonal-tips.json`). Aucun endpoint backend, aucune table de base de donnees.
- **Contenu monolingue** : les textes des conseils sont en francais directement dans le JSON. Aucune cle i18n pour le contenu des conseils dans cette version.
- **Logique de periode** : la periode courante est calculee cote client selon la meme logique que le calendrier existant (demi-mois 1–24 base sur `Date.now()`). Ne pas dupliquer la logique — extraire en service partage si ce n'est pas deja le cas.
- **Categories d'icones** : les 8 categories (`mulching`, `compost`, `green-manure`, `soil`, `water`, `frost`, `biodiversity`, `planning`) doivent avoir chacune une icone Material Icons distincte. Pas d'icone generique pour tout.
- **Performance** : le JSON est charge une seule fois au demarrage de l'app (singleton via service Angular) — pas de rechargement a chaque navigation.
- **Pas de personnalisation utilisateur** dans cette version : les conseils sont identiques pour tous les utilisateurs. La personnalisation par climat ou region est hors scope.
- **Contenu expert** : le contenu ci-dessus a ete valide par l'agent `plant-expert` pour le climat du Plateau suisse (USDA zone 6b–7a, precipitations ~1000 mm/an, gelees de mi-octobre a mi-avril).

---

### Estimation

- **Priorite :** Should
- **Points :** 5
- **Statut :** A faire

---

### Dependances

| Story | Raison |
|-------|--------|
| US-200 (E18) | La section « Actions du moment » de la page d'accueil doit exister pour y integrer les conseils |
| US-061 (E10) | Le widget calendrier « En ce moment » est le deuxieme point d'affichage des conseils |
