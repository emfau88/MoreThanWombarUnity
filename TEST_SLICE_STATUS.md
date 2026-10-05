# Test-Slice-Status

Stand: 5. Oktober 2026. Die folgenden älteren Abschnitte dokumentieren historische Nachweise.

## B3c — technisches Duell und Build-Übergabe

KI-Abstand, Combo-Anschluss aus tatsächlichem Anlauf, Angriffsschutz während GetUp, Kontaktfeedback und deutsches Duell-HUD sind integriert. Zwei vorher reproduzierte Probleme sind korrigiert: Warnung bei 1,643 m mit nur einem Combo-Treffer; neue KI-Angriffserlaubnis während Aufstehschutz. Nach Tuning: 1,288 m Warnabstand, drei Treffer (80 → 37 HP), neue Warnung erst nach Schutzende. Details: [B3C_DUEL_HANDOFF.md](B3C_DUEL_HANDOFF.md).

**3/3 DuelPlayTests**, 20,43 s und **5/5 HumanoidCombatPlayTests**, 13,34 s bestehen. Resultate: `tools/b3c-duel-results.json` und `tools/b3c-contact-results.json`. Der Kontaktblock prüft unter anderem Hitstop/Luftflugbahn, alle sieben Kontaktbahnen und übersprungenes Active. Kein erneuter Gesamt-Testlauf.

Windows- und WebGL-Releasebuild erfolgreich; bestehende Paket-/Shaderwarnungen sind in den Berichten erfasst. Windows meldet 0 Fehler; der letzte WebGL-Bericht meldet Succeeded und 1 Fehler des CLI-Wartezeitlimits während des Builds, keinen fehlgeschlagenen Playerbau. Details einschließlich Ladegrößen und Browser-Filterkorrektur stehen in der B3c-Übergabe. Windows startet und zeigt Map, KI, Knockdown und Niederlage. Der native Fokuszugriff des UI-Werkzeugs scheitert mit `foreground window did not report a process id`; nach einmaliger Wiederherstellung keine weitere native Tastaturprüfung. Nutzer hat diesen Spielstand noch nicht getestet. Im echten Browser funktionieren R, Bewegung/Richtung und die J-Combo; eine zusammenhängende Tastatursequenz gewinnt bei 100 Spieler-HP und R stellt 100/80 HP wieder her. Screenshot: `UnityProject/Assets/QA/b3c-browser-victory.png`. Keine hörbare Audioabnahme, kein physischer Gamepad-Nachweis und kein FPS-/WebGL-Speicherprofil daraus ableiten.

B3c ist integriert; B3b, M1 und B3c werden auf Nutzerauftrag gemeinsam committed und nach origin/main gepusht. B4 (Schulterstoß/Moveset-Balance) ist der nächste Entwicklungsbulk und wurde noch nicht begonnen. Wombat-Look bleibt separat offen.

## M1 — vorgezogene Junkyard-Testmap geliefert

HumanoidCombatLab besitzt jetzt eine erste gestaltete Kampfumgebung: durchgehender Beton, dezente Arbeitsflächen-Markierungen, neu gruppierte Container, drei Schrottgruppen mit Reifen/Fahrzeugteilen, Zaun, Werkstatt-Hintergrund, Hofschild und abgestimmtes warmes/kühles Licht. Zehn gezielt ausgewählte Kenney-FBX und eine Poly-Haven-Betonbasis statt eigener kompletter Modell-/Texturproduktion. Originale/Lizenzen unter ThirdParty, eigene URP-Materialien und wiederverwendbares Environment-Prefab unter Game/Environment. Details: [M1_MAP_PREVIEW.md](M1_MAP_PREVIEW.md).

Vorher/Nachher unter `Assets/QA/m1-before.png` und `m1-after.png` aus gleicher Kameraposition angesehen. Die erste zu sandfarbene Bodenfassung und zu große Hintergrundgebäude wurden korrigiert. Bodenmarkierungen berücksichtigen die tatsächlich gespeicherten Motorgrenzen und die Körperkapsel. Die Anfangs-Randmessung verglich irrtümlich mit älteren angenommenen Grenzen; abschließend gegen die gespeicherte CharacterDefinition kontrolliert.

Kurzer tatsächlicher Play-Durchlauf: alle vier Ecken bei x = ±7 / z = ±2,35 erreichen, jeweils grounded und innerhalb der Grenzen; Sprung mit 1,17 m Höhe und Landung; sichtbare Gegnerwarnung; echter Gegner-Heavy (100 → 88 HP) mit Falling; GetUp mit Schutz; Spieler-Heavy trifft genau einmal und wirft Gegner nieder; Reset zu 100 HP und Standing bei beiden. Neue Environment-Collider: **0**, originale Boden-/Randkollision erhalten. Messungen in `tools/m1-map-results.txt`, Spielbilder unter `Assets/QA/m1-play-*.png` angesehen. Keine neue Unit-/Combat-Vollsuite, da Gameplay-Regeln unverändert.

M1 ist eine aufgewertete kleine Testarena, kein finales Kapitel. Weitere Bereiche/Encounter/Checkpoints bleiben B6, finale Weltgestaltung B7. Als Nächstes B3c mit Duell-Tuning und Buildnachweis. M1 liegt lokal vor; kein neuer Commit/Push in diesem Auftrag.

Finale Übergabe: HumanoidCombatLab frisch geladen, Szene gespeichert/clean, Play gestoppt, keine fehlgeschlagene Script-Kompilierung. Environment-Prefab vorhanden, 0 fehlende Materialien, ursprünglicher Floor-Collider aktiv und 0 neue Environment-Collider.

## B3b — Knockdown, GetUp, Schutz und Tod geliefert

B3a als `405bfb5` committed und nach origin/main gepusht. B3b liegt als neuer lokaler Spielstand vor: Spieler-Heavy/Luft-Smash und Bären-Heavy werfen nieder; Fall 0,42 s, Boden 0,40 s, GetUp 0,80 s, danach 0,45 s Schutz bei freier Steuerung. Tod hält beide Ganzkörper-Posen am Boden. R und Gegnerwechsel räumen den Ablauf samt Collider/Kapselhöhe, HP, Schutz und Angriff auf. Details: [B3B_BODY_RECOVERY.md](B3B_BODY_RECOVERY.md).

Gezielte B3b-Prüfung: **4/4 BodyRecovery-Fälle bestanden** (`tools/b3b-body-results.json`), für Unterbrechung während Warnung, Aktionssperre, Schutz und Schutzende, beide Tode sowie Reset in jeder Phase und Gegnerwechsel. Zwei feste Zeitannahmen im Test wurden durch Warten auf tatsächliche Zustandswechsel ersetzt. Eine bestehende Stagger-Prüfung verwendet nun Kick, weil Heavy ausdrücklich Knockdown auslöst.

Einmalige relevante Regression: **4/4 CharacterAction-Fälle**, 8,82 s (`tools/b3b-action-regression-results.json`); **5/5 HumanoidCombat-Fälle**, 11,75 s (`tools/b3b-contact-regression-results.json`). Combo, Run, Hit/Stagger, Hitstop, sieben Kontaktbahnen, übersprungenes Active und Luft-Smash mit fortgesetzter Flugbahn bestehen. Keine neue Vollsuite.

Tatsächliche Kamerasequenzen unter `Assets/QA/b3b-*.png` und `b3b-final-*.png`: Fall, Liegen, Seitstütz/Knien, Stehen mit Schutz, Tod und Reset angesehen. Ein echter KI-Heavy trifft den Spieler für 12 HP (100 → 88) und startet Falling; danach Reset zu Standing bei beiden Figuren (`Assets/QA/b3b-final-visual-report.txt`). Die GetUp-Zwischenpose wurde anhand des Bodenkontakts angepasst. Reset des inaktiven zweiten Gegners spielt keinen Animator-State mehr ab und erzeugt dadurch keine entsprechende Warnung.

Nächster Bulk: B3c, zusammenhängendes Duell-Tuning und kleiner Windows-/WebGL-Buildnachweis. Noch nicht begonnen; Wombat-Look bleibt offen.

Übergabe: HumanoidCombatLab frisch geladen und gespeichert, Play gestoppt, Script-Kompilierung ohne Fehler. Drei BodyRecovery-Komponenten und beide niederwerfenden Heavy-Definitionen vorhanden. Nach letzter reiner GetUp-Posekorrektur tatsächlichen Seitstütz erneut angesehen; Fuß-Bone-Höhen 0,162/0,120 m statt unter dem Boden (`Assets/QA/b3b-final-side-report.txt`). Phasenlänge bleibt 0,80 s; keine weitere Voll-/Regressionssuite für diese Poseänderung.

## B3a — Run, Kernaktionen und Hit/Stagger geliefert

HumanoidCombatLab ergänzt Ctrl/LT-Run, Haken-Finisher, Overhand-Heavy und zweihändigen Air-Smash statt der bisherigen Cross-Platzhalter. Die drei neuen Kontaktbahnen sind am Avatar neu gebacken. Sichtbare Hit-/Stagger-Reaktionen für Spieler und Robot-Gegner halten innerhalb bestehender Treffer-Starre, respektieren Hitstop und werden durch Reset beendet. Details in [B3A_ACTIONS.md](B3A_ACTIONS.md).

Gezielt geprüft: **3/3 Action-Fälle**, 6,25 s (`tools/b3a-action-results.json`); **5/5 vorhandene Humanoid-Combat-Fälle**, 12,88 s nach Kick-Überarbeitung (`tools/b3a-contact-regression-results.json`); **1/1 Input-Fall**, 6,13 s (`tools/b3a-input-results.json`). Alle bestanden, keine übersprungenen Fälle. Keine Vollsuite; Gamepad simuliert.

Nutzerfeedback: Mensch gegen den großen Bären-Dummy wird vorerst beibehalten. Boden-/Sprung-Kick und Luftschlag wurden als zu schwach dargestellt beurteilt und innerhalb B3a gezielt überarbeitet: Kammer-/Streck-/Rücknahmebewegung, Armdeckung, angewinkeltes freies Bein und zweihändiger Abwärtsschlag mit Körperneigung.

Nach der letzten Luft-Smash-Korrektur **1/1 gezielter PlayMode-Fall bestanden**, 3,96 s: genau ein Treffer im Abstieg, weiterlaufende Flugbahn, Landung/Reset und Fehlschlag aus Distanz (`tools/b3a-air-smash-results.json`). **1/1 Kontaktgenauigkeitsfall erneut bestanden**, 1,94 s für alle sieben Attacken nach finaler Körperneigung (`tools/b3a-final-contact-results.json`). Die geänderten Kick-Posen wurden in echter Spielkamerasequenz angesehen; Boden- und Sprung-Kick treffen darin jeweils einmal.

Finale Smash-Kameraposen mit Ausholen und Kontakt angesehen (`Assets/QA/b3a-final-smash-*.png`); tatsächliche Sequenz bestätigt einen Treffer. Run, Heavy, Haken und beide Hit-Reaktionen ebenfalls kontrolliert. HumanoidCombatLab frisch geladen; Play gestoppt und Script-Kompilierung ohne Fehler für die Übergabe.

Zum damaligen B3a-Abschluss war B3b der nächste Bulk. Aktueller B3b-Stand und nächste Arbeit stehen oben; Wombat-Gestaltung bleibt offen.

## B1 — Combat-Polish (technisch abgeschlossen)

Kontrollierte kollisionsbegrenzte Angriffsschritte, frühere Light-Anschlüsse nach
Treffer, begrenzte eingabegesteuerte Startup-Ausrichtung und Bewegungsfreigabe in
später Recovery. L/RB-Kick mit gesweeptem Fuß; eigene Air-Kick-/Air-Smash-Clips.
Deutlichere Schritt-/Flug-/Landeposen, Walk-Tempo nach tatsächlicher Bewegung.
Gegner fällt mit dem gesamten Rig, deaktiviert Hurtbox/Körperkollision und
Animator; Reset stellt ihn wieder her. Gegner-Heavy ist jetzt ein eigenes Asset.

Prüfung: 24/24 EditMode bestanden. Vollständiger PlayMode-Lauf: 28/29 bestanden;
ein Reichweitenfall war durch die Arena-Clamp falsch positioniert. Nach Korrektur
und aktualisierten Clips: 4/4 gezielte Polish-Fälle bestanden (Jump-Hit,
Kick-Reichweite samt Fehlschlag, Ganzkörper-Tod/Reset, Angriffsschritt am Rand).
Die 25 bestehenden PlayMode-Fälle bestanden im vollständigen Lauf. Visuelle
Spielsequenz mit Lauf, Kick, Jump-Hit und KO unter Assets/QA/b1-*.png kontrolliert.
Kein Performanceprofil oder physischer Gamepad-Spieltest daraus ableiten.

Abschluss auf „go b1“: Die frühe Ausrichtung nutzte vor der Korrektur zwei
25°-Budgets (Begin und Startup). Ein neuer gezielter PlayMode-Test reproduzierte
bereits 27,13° und scheiterte korrekt. Begin/Startup beziehen sich jetzt gemeinsam
auf die Blickrichtung vor Begin. Nach Import/Kompilierung: **5/5 Polish-PlayMode-
Fälle bestanden**, 0 Fehler/übersprungen, 12,21 s. Der neue Fall prüft auch feste
Richtung während Active und Bewegung vor Attack-Ende. Ergebnis gespeichert unter
`tools/b1-close-results.json`. Kein neuer vollständiger Regressionlauf.

Echte neue Kamerasequenz mit Walk, Kick, Jump-Hit und Ganzkörper-KO unter
`UnityProject/Assets/QA/b1-close-*.png` angesehen. COMBAT_SYSTEM und ARCHITECTURE
sind vollständig auf die B1-Regeln aktualisiert. Physisches Gamepad, Performance
und subjektives Nutzer-Spielgefühl sind damit nicht als abgenommen behauptet.

Nutzerurteil zum neuen Spielgefühl bleibt für konkrete Werteanpassungen willkommen.

Übergabe: gespeichertes SparringLab frisch geladen, Play gestoppt und keine
fehlgeschlagene Script-Kompilierung. Hand-/Fußreferenzen und beide Luftclips
vorhanden, Enemy_Heavy getrennt vom Spieler-Heavy. Play startet das Sparring mit
einem Gegner; R reset, 2 aktiviert den Gruppentest.

## B2 — Import und Humanoid-Combat geliefert, Stil offen

B1 ist wie oben abgeschlossen. README, gespeicherte HUD-Steuerung,
COMBAT_SYSTEM und ARCHITECTURE enthalten den aktuellen Stand mit Kick,
eigenen Luftclips, Angriffsschritten und später Bewegungsfreigabe.

B2 ist begonnen, noch nicht abgeschlossen: tools/Build-WombatModel.py erzeugt
17 eigene Modellteile, eine editierbare Blender-Datei mit Quellrig unter
art-source/wombat/Wombat_B2.blend und die lokale Geometrie-Bibliothek unter
UnityProject/Assets/Game/Art/Models/WombatMeshLibrary.json. Der Blender-Lauf
wurde erfolgreich abgeschlossen. Diese neuen Modellteile sind noch nicht in
Unity-Mesh-Assets, Prefab oder Spielfigur integriert und noch nicht visuell
abgenommen. Das Spiel verwendet weiterhin das bisherige Grundform-Rig.

Der frühere nächste Schritt (eigene JSON-Mesh-Importpipeline und Rig-Integration)
ist nach der Richtungsänderung zurückgestellt. Modelle und Quelldateien bleiben
erhalten. Aktuelle Vorgaben stehen in PRODUCTION_GUIDELINES.md, die konkrete
Fortsetzung in NEXT_STEPS.md und die aktualisierten neun Bulks in ROADMAP.md.

Auftrag B2-Schritte 1/2: drei konkrete Kandidaten in B2_ASSET_SELECTION.md verglichen. Quaternius Base Characters Standard und Animation Library Standard mit enthaltenen CC0-Belegen importiert; konkrete Originale und Archivhashes stehen in THIRD_PARTY_LICENSES.md. Kostenloses Modellarchiv enthält zwei Superhero-Grundmodelle; Animation Standard hat Jab/Cross, aber keinen Kick.

CharacterImportLab verwendet ein eigenes HumanoidProbe-Prefab und denselben PlayerMotor/LabInput. Gültiger Humanoid-Avatar, Idle/Walk/Jump/Fall/Land, Modellmaßstab/-bodenversatz, Hand-/Fußanker und eigene URP-Materialien sind eingerichtet. Root Motion ist aus. Temporärer menschlicher Charakter, Wombat-Look bewusst offen. Combat-Integration und verbindliche Phasen-/Kontaktanbindung gehören zu Schritt 3; B1-Sparring bleibt vollständig spielbar.

Schritt 3 ist jetzt umgesetzt: HumanoidCombatLab nutzt bestehenden Combat/Defense, Gegner-KI und Feedback. Eigene AttackDefinitions enthalten gewünschte Phasendauern; HumanoidCombatBuilder retimt Jab/Cross und eigene Kick-/Luftkick-Ableitungen. Je Attacke 161 Avatar-Kontaktpunkte statt SampleAnimation auf dem sichtbaren Humanoid. Animator-Zeit bleibt die einzige laufende Phase. Finisher/Heavy/Air-Heavy sind vorläufige Cross-Varianten; eigene endgültige Kernclips und Reaktionen folgen in B3.

Prüfung Schritt 3: **5/5 gezielte Humanoid-PlayMode-Fälle bestanden**, 0 Fehler/übersprungen, 11,80 s (`tools/b2-combat-results.json`). Startup/Einzeltreffer/Fehlschlag/Reset, gewünschte Dauer/Hitstop, Kontaktgenauigkeit aller sieben Attacken, übersprungenes Active ohne Umposen sowie Drei-Schlag-Kette/Luftkontakt mit erhaltenem Sprung. Nach konkretem Luftkick-/Interpolationsbefund korrigiert und betroffene Fälle wiederholt. Zusätzlich **5/5 bestehende B1-Polish-Fälle bestanden**, 11,32 s (`tools/b2-b1-regression-results.json`). Keine neue Vollsuite.

Tatsächliche seitliche Kamerabilder Punch/Kick/Luftangriff unter `UnityProject/Assets/QA/b2-combat-*.png` angesehen. Nach regulärem Modell-Reimport bleiben gültiger Avatar, sieben zum Avatar/Clip passende Kontaktbahnen, vier Anker und Feedback erhalten. HumanoidCombatLab frisch geladen, Play gestoppt, Compile/Dirty false. Die Air-Heavy-Pose ist weiterhin ein vorläufiger Cross im Sprung; ein eigener überzeugender Air-Smash gehört zu den B3-Kernclips.

Gezielter Bewegungsnachweis bestanden: Laufweg 1,48 m, maximale lokale Fußbewegung 0,24 m, Richtungsfehler 0,08°, Sprunghöhe 1,17 m, Landung/Reset erfolgreich, Idle-Bodenkontakt ca. 2,4 cm. Drei tatsächliche Kamerabilder angesehen. Resultat `tools/b2-import-results.json`; keine neue Vollsuite. Ein regulärer Modell-Reimport erhält Avatar, Controller, Definition, Visual und vier Kontaktanker. CharacterImportLab frisch geladen, Play gestoppt, Compile/Dirty false.

Danach B3: vollständiges Einzelduell einschließlich Run, Trefferreaktion und lebendem Knockdown/GetUp. Gestaltungsziel bleibt illustrativer Cartoon-/Comic-Look mit glatten Formen.

Der Nutzer hat nach der Dokumentations-/Planänderung mit „go b1“ den B1-Abschluss
beauftragt; anschließend B2-Schritte 1/2 und danach Schritt 3. Schritte 1/2 samt Projektfundament wurden auf Nutzerauftrag als `c6a92ea` auf origin/main veröffentlicht. Schritt 3 ist aktuell lokal umgesetzt. Kein Kauf oder Editor-Upgrade erfolgt. Der folgende S0–S3-Verlauf dokumentiert historische Nachweise.

## Dieses Repository

- [x] Neues Repository geprüft: anfangs leer.
- [x] Eigene Arbeitskopie außerhalb des Browsergame-Repositories angelegt.
- [x] README, Roadmap, technischer Ansatz und Status dokumentiert.
- [x] S0: fokussierte Referenzanalyse für den Test und Live-Editor-Gate.
- [x] S1: echte 3D-Bewegung und kontrollierte Kamera; erster Nutzer-Spieltest positiv.
- [x] S2: implementiert, automatisiert geprüft und Nutzer-Spieltest vorläufig positiv.
- [ ] S3: implementiert und automatisiert geprüft; Nutzer-Spielgefühl-Abnahme offen.
- [ ] S4: Build und Auswertung — später freigeben.

Das separate Unity-Projekt ist unter `UnityProject` angelegt und im Editor
spielbar, einschließlich S2-Combat und separatem S3-Sparring. Kein eigenständiger Build. Alle Änderungen liegen
lokal; Commit und Push erfolgen erst auf Auftrag.

## Nachweise im neuen 3D-Test

- Unity 6000.4.0f1, URP 17.4.0, Pipeline 0.8.0-exp.1, Input System 1.19.0.
- Hauptthread-Probes nach Script-Kompilierung sowie Play/Stop verifiziert.
  Die letzte Live-Probe bestätigt `CombatLab`, Play aktiv, keine Kompilierung
  und `scriptCompilationFailed = false`.
- 11 EditMode-Tests bestanden: Umgebung und Bewegungsregeln, darunter
  Diagonalnormalisierung und ballistische Integration bei 30/60/120 FPS.
- Async-PlayMode-Lauf abgeschlossen: **6/6 bestanden**, 0 Fehler, 0 übersprungen,
  Laufzeit 20,28 Sekunden. Ausgelesen über `unity command test_status` und
  `UnityProject/Temp/pipeline_test_status.json`.
- Geprüft: reale CharacterController-Bewegung samt Walk-Animation,
  Sprunghöhe und Landung ohne Tiefenversatz, Arenagrenze und Reset,
  Kamera am hinteren Rand, Tastatur- und simulierte Gamepad-Bindings.
- Nutzer hat im Editor per Tastatur Laufen und Springen gespielt und Bewegung/
  Kamera vorläufig positiv bewertet: „ist erstmal ganz gut“.
- Ein physisches Gamepad ist noch nicht abgenommen; Simulation ist kein
  Hardware-Nachweis. Die 30/60/120-FPS-Mathematiktests sind kein Performanceprofil.

### Verbleibende Werkzeug-Reibung

URP-Materialdialoge erforderten Nutzerbestätigung. Zeitweise waren Hauptthread-
Befehle nicht erreichbar; nach Wiederherstellen des minimierten Editorfensters
funktionierten Szenenöffnung und Tests wieder. Eine einzelne eindeutige Ursache
für alle Timeouts ist nicht nachgewiesen. Vollständig unbeaufsichtigte Bedienung
ist daher nicht zugesichert. Computer Use wurde während S1 per Escape gestoppt.
Das laufende Nutzer-Play wurde beim damaligen Auslesen der Ergebnisse nicht
verändert. Für den später freigegebenen S2-Auftrag wurde Play regulär beendet;
bei einem weiteren Timeout wurde das Editorfenster wiederhergestellt. Kein
blockierender Dialog war dabei sichtbar. S2-Tests werden aus dem gestoppten
Editor neu gestartet. Der abgebrochene letzte Testauftrag wurde aufgeräumt.

## Vorversuch: nicht mit dem neuen 3D-Test verwechseln

Ein früherer Universal-2D-Vorversuch existiert im Browsergame-Arbeitsverzeichnis
unter `experiments/unity-spike/MoreThanWombatUnityTest`. Er bleibt pausiert und
wird weder automatisch übernommen noch gelöscht.

Beobachtet: Unity CLI 1.0.0-beta.12 und Editor 6000.4.0f1 funktionieren für
Installationsdiagnose/Projekterstellung; Pipeline 0.8.0-exp.1 wurde installiert.
Der Editor kompilierte und meldete `ready`, aber Main-Thread-C#-Probe und
Testausführung liefen in Timeouts. Play/Stop und Smoke-Tests sind **nicht**
bestanden. Ein veralteter Status-Snapshot beweist keine funktionierende Steuerung.

Eine visuelle Untersuchung via Computer Use scheiterte am App-Freigabe-Timeout;
ein blockierender Startdialog ist eine Hypothese, keine nachgewiesene Ursache.
Es wurde kein Gameplay umgesetzt. Das frühere Setup-Gate ist daher offen.

## S2 — animierter Combat-Test (abgeschlossen)

Drei Light-Angriffe, ein Heavy und ein Trainingsgegner gemäß Roadmap.
Animation-synchronisierte Trefferfenster, Input-Buffer, einmaliger Treffer pro
Attack-Instanz sowie minimale Trefferreaktion, Lebensanzeige und Reset.
S2 ist implementiert: vier eigene Transform-Animationsclips, AttackDefinition-
Assets, animation-synchronisierte gesweepte Fausttreffer, einmaliger Treffer je
Attack-Instanz, kurzer Buffer, Combo-/Heavy-Cancel-Fenster, Hitstop, Rückstoß,
Flash/Funken, zwei originale synthetisierte Kontaktklänge, HP-Anzeige und Reset.

Aktuelle Regression: **24/24 EditMode und 18/18 PlayMode bestanden**. Darunter
vollständige Light-Kette, Heavy-Damage/Knockback, keine Treffer hinter dem Spieler
oder in anderer Tiefe, doppelte Hurtboxes, Reset mitten im Angriff, Tod/Reset,
Animator-Unterbrechung, Movement-/Jump-Lock und auslaufende Eingabepuffer.
Ein realer Animator-/Physik-Sweep wird mit 30/60/120-FPS-Zeitschritten sowie einem
3-FPS-Schritt geprüft, der Active komplett überspringt. Kein Performanceprofil.
Der abschließende PlayMode-Lauf wurde nach Wiederaufnahme sauber gestartet und
mit 18 Passed / 0 Failed / 0 Skipped in 32,18 Sekunden abgeschlossen.

Vorübergehende Initialisierungs-, unvollständige Transformkurven- und
Prefab-Override-Fehler wurden korrigiert. S1-Grenzentest umgeht den nun soliden
Dummy; Eingabebindings nutzen Unitys offizielle isolierte Testfixture mit
synchronem Komponentenaufbau statt Gerätesimulation während Szenenwechseln.
Keine Tests ignoriert oder erwartete Schadenswerte abgeschwächt.

Visuelle QA: gespeicherte Game-View-Bilder unter `UnityProject/Assets/QA/` zeigen
Arena, Figur, Trainingsziel und HUD; ein tatsächlich ausgelöster Light-Kontakt
ergab 140 HP, einen Treffer, Hitstop und aktiven Kontaktblitz. Für die Übergabe
wurde der QA-Eingabehaken gelöst, Reset ausgeführt und Play unpausiert belassen.

Nutzerfeedback: „ja an sich gut“, mit dem Wunsch nach Angriffen während des Sprungs
und Fortsetzung. S3 damit freigegeben; S4 und professionelle Art bleiben nächste Schritte.
Regeln, Steuerung und Grenzen stehen in `COMBAT_SYSTEM.md`.

## S3 — Luftangriffe, Ausweichen und Gegenkampf

Light/Heavy starten auch während des Sprungs; Flugbahn und X/Z-Luftkontrolle
bleiben aktiv. Schlagrichtung bleibt fest, echte Höhenkontakte statt Auto-Hits.
Die bestehenden Boden-Combo-Regeln bleiben erhalten. S2-Training bleibt in
`CombatLab`, Gegenkampf in eigener `SparringLab`-Szene.

Ein eigener Grundform-Gegner mit sichtbarem 0,55-s-Warnfenster, animiertem
Angriff, Recovery, Reposition, Stagger und Tod. 1/2 schaltet Gegnerzahl um.
Ein gemeinsames Angriffstoken verhindert simultane Angriffe; Freigabe auch bei
Treffer, Tod, Abbruch, Disable und Reset. Shift/Gamepad East weicht aus, mit
klar begrenztem Schutzfenster. Spieler-HP, Sieg/Tod/Retry und gemeinsame Rücksetzung.

Die erste Regression bestätigte alle 20 Bewegung-/Combat-/Input-Tests inklusive
beider Luftangriffstests. Die vier neuen Sparringtests deckten eine fehlende
Renderer-Zuordnung im Szenenaufbau auf; diese wurde am Torso-Body korrigiert.
Abschließende Regression: **24/24 EditMode und 25/25 PlayMode bestanden**,
0 Fehler, 0 übersprungen; vollständiger PlayMode-Lauf 48,73 s. Zusätzlich geprüft:
rechtzeitiges Ausweichen vermeidet den tatsächlichen animierten Gegnerschlag.
Visuelle Kontrolle der Zwei-Gegner-Szene und Warnmarkierung; HP-Hinweis gekürzt,
damit er einzeilig bleibt. Warnfarbe startet bei jedem Telegraph neu orange.
Editor wird in `SparringLab` gestoppt und unpausiert übergeben: Play startet
mit einem Gegner, 2 aktiviert den Gruppentest. Kein Build/Commit/Push ausgeführt.

## Noch zu klären, bevor ein professioneller Slice geplant wird

- Verfügbare Gamepad-Hardware und Test-PC als Performance-Basis.
- Geeignetes 3D-Modell, Rig und Animationsquelle; Kosten/Herkunft/Lizenz.
- Wahl und Fixierung der Editor-Version für eine mögliche Produktion.
- Nutzerurteil zu Bewegung und Combat nach S1/S2.
