# More Than Wombat — Gestaltung und effiziente Produktion

Stand: 3. Oktober 2026. Projektvorgabe aus der Richtungsänderung und der anschließenden Stilkorrektur des Nutzers. Diese Vorgabe ersetzt die bisherige Bevorzugung eigener Modelle/Rigs und eines sichtbaren Low-Poly-Stils. Konkrete Umsetzung: [NEXT_STEPS.md](NEXT_STEPS.md); Gesamtablauf: [ROADMAP.md](ROADMAP.md).

## 1. Ziel und Priorität

Ein kompakter 3D-/2.5D-Arcade-Brawler, dessen Bewegung und Treffer mit einem Spieler und einem einfachen Gegner überzeugend Spaß machen. Die 2D-Fassung liefert Identität, Charakterrollen, Humor, Kampfrichtung und visuelle Referenz. Little Fighter 2 dient als Referenz für direkte Eingaben, schnellen Rhythmus und klare Kampfentscheidungen; keine seiner Figuren, Namen, Grafiken oder Sounds übernehmen.

**Spielgefühl vor Architektur vor Contentmenge.** Zuerst das vollständige Charakterduell ausarbeiten, danach zusätzliche Gegner, Maps und Content. Die bereits vorhandene Zwei-Gegner-Option bleibt eine nützliche Probe; sie begründet keinen vorgezogenen Ausbau der Gegnerfamilie.

## 2. Visuelles Ziel: illustrativer Cartoon-/Comic-Stil in 3D

Die tatsächlich angesehenen Wombat-Sprites und der Junkyard-Spielscreen der alten Fassung zeigen runde, kräftige Figuren, expressive Gesichter, dunkle Konturen, warme Fellfarben und gemalte Materialdetails. Die Umgebung besitzt verwitterte Oberflächen und dramatisches warmes/kühles Licht. Das ist eine illustrative Cartoon-/Comic-Richtung, keine fotorealistische oder sichtbar polygonale Gestaltung.

Lesend verwendete Referenzen im alten Projekt:

- `public/assets/characters/wombat/wombat_spritesheet.png`
- `docs/qa/ui-r4-2026-09-11/refresh/wide-landscape.png`
- `public/assets/arenas/scrapyard/scrapyard_background.png`

### Umsetzung in 3D

- Glatte, kräftige Silhouetten: kompakter Körper, breite Schnauze, kleine Ohren, kurze stabile Beine, klar erkennbare Pfoten und ausdrucksstarke Augen/Brauen.
- Warme braune Fell-/Bauchfarben, lesbare Hell-Dunkel-Flächen und stilisierte Fellakzente. Gemalte Texturen oder kontrollierte Farb-/Schattenflächen; kein dichtes simuliertes Fell nötig.
- Gegner durch Form, Haltung und Farbflächen unterscheiden. Konturen können die Comicwirkung unterstützen; zunächst mit Material und Licht prüfen, ob ein zusätzlicher Outline-Effekt überhaupt nötig ist.
- Junkyard mit stilisiertem Rost, Schrott und kräftiger Lichtstimmung. Spielrelevante Figuren müssen vor der detailreicheren Umgebung lesbar bleiben.
- Anatomie und Gesicht tragen den Wombat. Teal-Boxhandschuhe, Arbeitskleidung und neue Kostüme sind keine Vorgabe aus der Referenz; nur einsetzen, wenn sie bewusst zum Charakter passen.
- Modulare technische Wiederverwendung darf sichtbar zu einer einheitlichen Welt führen. Ein Anbietername oder ein günstiger Pack ersetzt keine Stilprüfung.

**Technisch sparsame Geometrie ist erwünscht; sichtbare Facetten sind kein Stilziel.** WebGL-Verträglichkeit bedeutet kontrollierte Geometrie, Materialien, Texturgrößen, Schatten und Effekte. Sie verlangt keinen Low-Poly-Look. Ein konkretes Budget wird am ersten spielbaren Charakter gemessen und danach für den Slice festgelegt.

## 3. Fertige Grundlagen zuerst prüfen

Vor neuer Eigenproduktion kurz prüfen, ob ein vorhandenes geeignetes Asset die Aufgabe übernimmt. Kleine Auswahl statt endloser Katalogsuche: höchstens drei ernsthafte Kandidaten für Modell/Rig und eine primäre Animationsbibliothek plus eine Ergänzungsquelle.

| Individuell entwickeln und abstimmen | Vorzugsweise übernehmen und anpassen |
| --- | --- |
| Eingabereaktion, Bewegung, Facing und Luftkontrolle | Geeignete Basis-Modelle und Rigs |
| Angriffsphasen, Combo-/Cancel-Fenster und Angriffsschritte | Idle, Walk, Run, Sprung, generische Punch-/Kick-Clips |
| Hit-/Hurt-/Pushboxes, Trefferfilter und Kontaktabstände | Grundanimationen für Treffer, Fall, Aufstehen und Tod |
| Hitstop, Knockback, Gegnerreaktionen und Balance | Props, modulare Umgebung, einfache Audio-/VFX-Grundlagen |
| Wombat-Identität und besondere Moves | Texturen/Materialgrundlagen bei passender Gestaltung |

Eigenarbeit gezielt auf Lücken beschränken: Wombat-Gesicht/Proportionen, ein besonderer Move oder die Anpassung einer geeigneten Vorlage. Ein humanoides Zweibeiner-Rig mit Unity-Humanoid-Avatar und Retargeting ist bevorzugt, sofern die kurzen Gliedmaßen und die Kontaktposen damit gut funktionieren. Humanoid-Kompatibilität ist eine technische Eigenschaft, keine Festlegung auf einen Menschen als endgültige Spielfigur.

Ein menschliches Testmodell darf einen kurzen Retargeting-/Importnachweis liefern. Es erfüllt nicht das Gestaltungsziel eines Wombats. Bei fehlender passender fertiger Figur zunächst eine begrenzte Anpassung eines rigged Cartoon-Modells bewerten, bevor ein kompletter eigener Rig-/Animationspfad begonnen wird.

### Quellen als Kandidaten, nicht als fertige Auswahl

- Quaternius: [Universal Animation Library](https://quaternius.com/packs/universalanimationlibrary.html) und [Universal Base Characters](https://quaternius.com/packs/universalbasecharacters.html) als technische Retargeting-Kandidaten. Offizielle Seiten nennen CC0 und Unity-/Humanoid-Kompatibilität. Standard-/Pro-/Source-Umfang konkret prüfen; nicht alle genannten Inhalte sind automatisch im kostenlosen Download enthalten. Die Base Characters sind menschliche Figuren und kein bestätigter Wombat-Ersatz.
- Mixamo: generische Humanoid-Animationen als mögliche Ergänzung. [Adobe-FAQ](https://helpx.adobe.com/creative-cloud/faq/mixamo-faq.html) erlaubt kommerzielle Spieleverwendung und nennt einen Adobe-ID-Zugang sowie Grenzen des Auto-Riggers. Kompakte Wombat-Proportionen und getrennte Modellsegmente nicht ungeprüft an den Auto-Rigger übergeben.
- Kenney: einzelne Props/UI-/Audio-Grundlagen nach Stilprüfung. [Offizielle Lizenzinformation](https://kenney.nl/support) nennt CC0 für Assets auf den Asset-Seiten. Ein kantiger Pack muss nicht zu diesem Projekt passen.
- Poly Pizza, Unity Asset Store und Fab: ergänzende Suche nach tatsächlich passenden Cartoon-Figuren und Umgebungen. Lizenz und Urheber jedes konkreten Angebots prüfen; keine pauschale Plattformlizenz unterstellen.

Quaternius Universal Base Characters Standard und Universal Animation Library Standard sind jetzt für einen technischen Human-Bewegungsnachweis importiert. Die konkrete Drei-Kandidaten-Auswahl steht in B2_ASSET_SELECTION.md, Archiv-/Lizenznachweise in THIRD_PARTY_LICENSES.md. CharacterImportLab nutzt unsere vorhandene Steuerung. Eine passende Tierbasis ist damit weiterhin offen; Wombat-Gestaltung wurde vertagt.

## 4. Lizenz und Originale

- Für jedes verwendete externe Asset Quelle, Autor, Pack/Version, Lizenzbeleg, Download-/Prüfdatum, Projektpfad und eigene Ableitungen in [THIRD_PARTY_LICENSES.md](THIRD_PARTY_LICENSES.md) eintragen. Lizenzdateien mit dem Import aufbewahren.
- Nur Inhalte mit eindeutig erlaubter kommerzieller Nutzung verwenden. Kandidaten klar von tatsächlich verwendeten Assets unterscheiden. Bei Account-gebundenen Quellen zusätzlich Bedingungen für Speicherung/Weitergabe der Rohdateien prüfen.
- Originale unter `Assets/ThirdParty/<Anbieter>/<Pack>/` erhalten. Eigene Prefabs, Materials, Controller, Avatar-/Importkonfigurationen und bearbeitete Clipkopien außerhalb dieser Originale führen.
- Änderungen an fremden Quelldateien nur in einer eigenen Ableitung durchführen. Keine fremden Rohassets ungeprüft in ein öffentliches Repository veröffentlichen.
- Kostenpflichtige Assets, neue Dienste und Editor-Upgrades bleiben gesonderte Entscheidungen. Eine Recherche oder ein kostenloses, lokal nutzbares Pack erfordert keine Routinefreigabe.

## 5. Bestehendes Projekt weiterentwickeln

Motor, Input, Combat, Schaden, Defense, KI, Token und Kamera weiterverwenden. Neue Rigs benötigen klare Hand-/Fußreferenzen; alte Transformpfade nicht in ein allgemeines Charakterframework ausbauen.

Aktuelle Struktur `Runtime`, `Data`, `Art`, `Prefabs`, `Scenes` und `Tests` erhalten. Neue Ableitungen gezielt unter `Assets/Game/Characters`, `Animations`, `Combat` und `Environment` ablegen. Bestehende Assets nur bei konkretem Nutzen verschieben und dabei GUIDs/Referenzen bewahren. Kein Projektumbau allein für Ordnernamen.

Die begonnenen eigenen Blender-Modelle und die JSON-Mesh-Bibliothek bleiben erhalten, sind aber zurückgestellt. Eine zusätzliche eigene JSON-zu-Unity-Modellpipeline ist vorerst nicht der nächste Produktionsschritt. Fertige Assets bevorzugt über einen von Unity unterstützten regulären Import verwenden.

## 6. Combat bestimmt das Design, Animation bleibt synchron

Angriffsdaten bestimmen gewünschte Startup-/Active-/Recovery-Zeiten, Damage, Hitbox, Knockback, Hitstop, Cancels und Bewegung. Importierte Clipdauer darf das gewünschte Timing nicht unbemerkt verändern. Eine lange Einspielung darf keine ganze Combo als untrennbare Aktion ersetzen: jeder Schlag bleibt eine eigene Attack-Instanz.

Animation visualisiert diese Regeln und wird dazu passend zugeschnitten, beschleunigt oder in Abschnitten angepasst. Eine gemeinsame Phasenquelle verbindet Darstellung, Kontakte und Motorbewegung. Keine parallel driftenden Animation-/Schadenstimer und keine Treffer während bloßer Vorbereitung oder Erholung.

Die normalisierte Animator-Zeit bleibt auch im importierten Humanoid die einzige laufende Angriffsphase. B2 Schritt 3 speichert konkrete Startup-/Active-/Recovery-Zeiten, retimt die Clip-Ableitungen darauf und verwendet am Avatar gebackene Kontaktbahnen. Details in B2_COMBAT_INTEGRATION.md; endgültige individuelle Move-Animationen werden in B3 ausgebaut.

Hurtbox, Angriffskontakt und Körperblockade bleiben getrennt. Faust-/Fußpositionen sind gute Anker für abgestimmte Kontakte; Mesh-Bounds werden nicht automatisch zur Schadensreichweite. Sichtbare Aktion und erreichbare Kontaktzone müssen trotzdem zusammenpassen.

## 7. Ein vollständiges Duell vor weiterer Skalierung

Ein Spieler und ein einfacher Gegner müssen Idle, Walk, Run, Jump, Fall, Land, drei Normalangriffe, Trefferreaktion, Knockback, lebendes Knockdown/GetUp und Tod überzeugen. Vorhandene Heavy-, Kick-, Evade- und Air-Attacks bleiben integriert.

Erfolg wird an einer kurzen tatsächlichen Spielsequenz beurteilt: unmittelbare Reaktion, klarer Abstand, brauchbarer Jump-Hit, verständliche Trefferreaktionen und sauberer Neustart. Danach Gegnerrollen und Level ausbauen. Ein bestandenes Testpaket ersetzt dieses Urteil nicht.

## 8. Schlanke Ausführung

Kurzer Befund → konkreter Verbesserungs-Bulk → integrierte spielbare Übergabe. Keine wiederholten Setup-Audits, theoretischen Absicherungsrunden oder Abstraktionen für einen großen Roster. Normale lokale Tuning-/Implementierungsentscheidungen selbst treffen.

Gezielt die geänderte Fehlerklasse prüfen; eine Regression nach größeren Integrationen. Reversible Dokumentations-/Wertänderungen benötigen keine neuen Tests. Art und Animation visuell beurteilen. WebGL früh mit dem kleinen Slice praktisch prüfen, statt aus der Gestaltung auf Browserleistung zu schließen.

Der B1-Abschluss und B2-Schritte 1–3 sind beauftragt und umgesetzt. Wombat-Look bleibt nach Nutzerwunsch offen. B3-Kernclips/Reaktionen, Stil-Anpassung und spätere Bulks sind weitere Arbeitspakete; die alte Anweisung zum vollständigen B2-Eigenbau ist durch Stopp und Richtungsänderung überholt.
