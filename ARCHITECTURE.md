# Architektur — B9–B12 lokal implementiert

## Aktueller Ausbau

Die vorhandenen Motor-, Animator-, Input- und Schadenempfänger bleiben die Basis. B9–B12 ergänzen folgende begrenzte Verantwortlichkeiten:

| Baustein | Ergänzung |
| --- | --- |
| CombatController / AttackDefinition | MP, Landeschaden, Durchbruch und einmalige Projektilfreigabe in der Angriffsphase |
| FighterTarget | Gemeinsamer Team-/Ziel-/Schadensadapter um PlayerDefense und TrainingDummy; lokaler Gegner-Hitstop |
| CombatProjectile / SpecialEffects | Geordnete Projektilkontakte mit Wänden/Teams und begrenzte, reichweitengerechte Effekte |
| PlayerMotor | Bodenlandung trotz Gegnerkörpern; temporäre Körper-Kollisionsfreigaben mit Cleanup |
| EnemyBrain / EngagementCoordinator | Auswahl lebender gegnerischer Ziele, Werferverhalten und zwei Nahkampffreigaben pro Ziel; AppendEnemy ergänzt Nachschub ohne Reset der Überlebenden |
| Lf2CombatBuilder / Lf2StageBuilder | Vorhandene Assets erweitern, CrowdCombatLab erzeugen und Kapitel mit fünf Begegnungen konfigurieren |
| JunkyardChapter / ChapterWave | Positionsauslöser, endlicher Nachschubvorrat, Zugangswarnung und Checkpoint-Rekonstruktion des Bereichseinstiegs |
| CrowdPerformanceProbe | Nur ausdrücklich aktivierte technische Belastungsprobe im separaten Crowd-Labor |

Die Teamgrundlage liefert noch keinen KI-Begleiter. Aktuelle Details und Nachweise: [B9_B11_LF2_COMBAT.md](B9_B11_LF2_COMBAT.md), [B12_LF2_STAGE.md](B12_LF2_STAGE.md). Die folgende Tabelle beschreibt die historische Grundlage; die dortige einzelne Angriffsfreigabe und Vier-Gegner-Grenze werden im neuen Crowd-Labor und Kapitel durch den obigen Ausbau ersetzt.

## Grundlage bis B8

Stand: 9. Oktober 2026. S1–S3, B1-Polish, B2-Humanoid-Integration, B3a-Aktionen/Reaktionen und B3b-Knockdown/GetUp/Tod, B3c-Duell, Mobile Touch, B4-Schulterstoß, B5-Gegnerrollen, B6-Junkyard-Kapitel und B7-Präsentation sind implementiert. Humanoid und Grundform-Bären bleiben die technische Figurenbasis; B7 trennt ihre Gesichts-/Körperfarben. Wombat und finale Modelle bleiben offen. Die Produktionsrichtung steht in [PRODUCTION_GUIDELINES.md](PRODUCTION_GUIDELINES.md), Regeln und Steuerung in [COMBAT_SYSTEM.md](COMBAT_SYSTEM.md).

B2-Schritte 1/2 ergänzen CharacterImportLab mit regulärem Quaternius-Humanoid und vorhandenem PlayerMotor/LabInput. Schritt 3 ergänzt separat HumanoidCombatLab mit CombatController/Defense, bestehenden Robot-Gegnern und Feedback. HumanoidCombatBuilder erzeugt eigene AttackDefinitions, retimte Clips und je 161 Avatar-Kontaktpunkte im lokalen Facing-Raum; CombatController interpoliert sie ohne SampleAnimation auf dem sichtbaren Humanoid. AttackDefinition enthält gewünschte Phasendauern sowie Avatar-/Clipreferenz für die konkrete Bahn. Animator-Zeit bleibt die einzige laufende Phase; B1-Transformclips behalten ihren alten SampleAnimation-Pfad. Root Motion bleibt aus. Die ursprünglichen Combat-Szenen sind erhalten. Details in B2_COMBAT_INTEGRATION.md.

## Verantwortlichkeiten

B8 ergänzt `ChapterSession`: bestehende Kapitel-Szene als Einstieg, Start-/Pause-/Steuerungs-/Options-/Intro-/Ergebnisseiten, skalierte Spielzeit und gespeicherte Optionen. Kapitelaktionen nutzen weiterhin JunkyardChapter. Input-/Sweep-/KI-Guards erhalten den pausierten Angriff, bereinigen Eingabepuffer und verhindern Menü-Klicks im Combat; beim Szenenwechsel werden Zeit/Audio restauriert. Vorhandene Unity-UI/Input-System-Komponenten, keine weitere Gameplay- oder Savegame-Architektur. Details: [B8_SESSION.md](B8_SESSION.md).

| Komponente | Tatsächliche Aufgabe |
| --- | --- |
| LabInput / InputFrame | Unity Input System für Move, gehaltenes Run, Jump, Light, Heavy, Kick, Charge, Evade, Reset und Debug; gemeinsame Intents für Tastatur/Maus/Gamepad/Touch |
| PlayerMotor / MotorMath | CharacterController, X/Z-Bewegung, Gravitation, Grounding, Coyote-/Jump-Buffer, Landung, Arenagrenzen und kontrollierte Angriffsschritte |
| CombatController | Spieler-Attack-Instanzen, einzelner Input-Buffer, Combo/Cancel, begrenzte Startup-Ausrichtung, Animator-Phase, Hand-/Fuß-/Schulter-Sweeps, begrenzter Stoßweg, Trefferfilter und Hitstop |
| AttackDefinition / CharacterDefinition | Gespeicherte Angriffs-/Kontakt-/Bewegungsdaten als ScriptableObjects; keine allgemeine Definition für jedes zukünftige System |
| TrainingDummy | Gemeinsamer Schadens-/HP-/Hitstun-/Rückstoßempfänger für Trainingspuppe und Sparring-Gegner; Ganzkörper-Tod beim Gegner und Reset |
| PlayerDefense | Spieler-HP, Treffer-Starre, Rückstoß, Bodenausweichen samt Zeitfenster und Cooldown |
| AnimationReaction | Optionale Hit-/Stagger-Pose innerhalb vorhandener Treffer-Starre, passende Clip-Geschwindigkeit, Hitstop und Rückgabe an Locomotion/KI; kein zusätzlicher Gameplay-Lock |
| BodyRecovery | Optionaler Fall/Boden/GetUp/Tod mit Pose, Aufstehschutz, niedriger Spieler-Kapsel und Wiederherstellung der Collider; nur in HumanoidCombatLab angebunden |
| EnemyBrain / EnemyRoleDefinition | Gemeinsame KI mit kleinen Rollendaten, Warnung/geradem Ansturm, Animator-Phase, Recovery und Unterbrechung bei Treffer/Tod |
| EngagementCoordinator | Eine rotierende Angriffsfreigabe für bis zu vier Gegner, Kamera-/Aufstehschutz, Wartepositionen, Körperkapsel-Bewegung und Encounter-Reset |
| CombatFeedback / LabHud / ArenaCamera | Kontakt-Audio/VFX, Kapitel-/HP-/Phasenanzeige; im Kapitel scrollende Wege und rahmende Kampfkamera |
| JunkyardChapter / ChapterDefinition / ChapterGate | Kleiner Ablauf für drei Bereiche/Wellen, sichtbare Tor-Interaktion, Checkpoint mit HP/Position und Neustart; vorhandene Combat-Komponenten weiterverwendet |
| ChapterPresentation / EnemyPresentation | Beobachten Kapitel-/Motor-/KI-/Körperzustände für Schritt-/Signal-/Warnklänge, Torlampen, Presse, Bodenring, HP-Balken und Namenssichtbarkeit; keine neue Kampfzustandsmaschine |

Animationsbrücke und Spieler-Trefferauflösung sind in CombatController integriert. Es gibt keine eigenständigen Klassen namens AnimationBridge oder HitResolver. Weitere Trennung erfolgt erst bei echtem Bedarf.

B7 wird durch `ChapterPresentationBuilder` auf die gespeicherte Kapitel-Szene angewendet. Lokale Material-/Volume-Ableitungen und gemalter Boden unter `Environment/JunkyardChapter`; M1-Originale und Labor bleiben erhalten. Standard-URP-Lit/Particles-Unlit und ColorAdjustments/ACES/kleiner Bloom, keine eigene Renderpipeline. Optionale Kontaktclips in CombatFeedback erhalten den synthetischen Fallback. Die zwölf ausgewählten Kenney-Originale liegen mit CC0-Lizenzen unter ThirdParty. Details: [B7_PRESENTATION.md](B7_PRESENTATION.md).

B4 ergänzt genau eine AttackDefinition mit chargeDistance. ShoulderChargeBuilder leitet den vorhandenen Sprint ab und benutzt den bestehenden Avatar-Bake mit RightUpperArm als Schulterkontakt. CombatController verteilt den begrenzten Weg auf kurze räumliche Schritte innerhalb Active; PlayerMotor meldet Wand-/Körperkollision oder Arenaclamp zurück. Erster gültiger Kontakt stoppt den Weg, vollständige Recovery bleibt. Keine zusätzliche Root Motion, Phasenuhr oder Schadensauflösung. PlayerDefense lässt den committed Stoß nicht ausweichen, Gegentreffer unterbrechen ihn regulär. MobileTouchControls speist E/RT/Charge über vorhandene OnScreenButton/Gamepad-Bindings.

## Phasenautorität und Kontakt

Die einzige aktuelle Attack-Phase ist die normalisierte Zeit des aktiven Animator-States. AttackDefinition referenziert Clip/State und enthält Active-, Chain-, Hit-Chain-, Heavy-Cancel- und Movement-Release-Fenster. Separate Telegraph-/Recovery-Zeiten der Gegner und Evade-/Hitstun-Zeiten beschreiben andere Zustände, keine zweite Uhr für denselben Schlag.

Startup/Recovery verursachen keinen Schaden. Der Controller begrenzt das seit dem letzten Frame durchlaufene Intervall auf Active und wertet direkte Transform-Clippose in kleinen Schritten aus. Capsules zwischen dem jeweiligen Faust-/Fußkontakt erfassen schnelle Bewegung und übersprungene Fenster. Root-Bewegung des Spielers wird berücksichtigt; danach wird die sichtbare aktuelle Clippose wiederhergestellt. `rightHand` wählt die rechte/linke Seite auch bei einem Fußangriff, `foot` bestimmt Hand gegenüber Fuß.

Getrennte Trigger-Hurtboxes auf Layer 8, Körpercollider und explizite Kontaktpunkte erhalten unterschiedliche Aufgaben. Team-/Front-/Alive-Filter und ein Zielset begrenzen Schaden auf einmal pro Ziel und Attack-Instanz. Gegner lösen ihren Faustkontakt gegen PlayerMotor/PlayerDefense auf und verursachen B5-Rollenschaden 8/10/18; ältere Gegner ohne Rolle behalten 12 Schaden. Der Spieler-Sweep hat 32, der Gegner-Sweep 12 vorab reservierte Kontaktplätze; das ist kein unbegrenztes Crowd-System.

B1 besitzt unterschiedliche Boden- und Luftclips. Am Boden verschiebt der Motor die Figur kontrolliert aus dem Fortschritt der Clipphase; freie Bewegung/Drehung kommt bei Phase 0,82 zurück. Luftangriffe erhalten Motor-Gravitation und Luftkontrolle, halten ihre Angriffsrichtung und werden bei Landung beendet. Keine parallele Root Motion oder Zielhöhenkorrektur.

Die Ausgangsrotation vor dem Angriffsbeginn begrenzt die gesamte frühe Ausrichtung auf 25°. Nach Active bleibt die Richtung bis zur späten Recovery fest. Folgeschläge starten eine neue Instanz und dürfen sich erneut begrenzt ausrichten.

Der einzelne Buffer altert in Combat-Zeit und ruht bei Hitstop. Treffer erlauben früheren Light-Anschluss als Fehlschläge. Attack-Ende, externer Animator-Wechsel, Disable, Defense und Reset bereinigen laufenden Angriff und Buffer. Am Boden friert Hitstop Motor/Angriff ein, in der Luft bleibt die Flugbahn aktiv. UI und Eingaben benutzen keine globale Zeitpause.

## Bewegung, Pose und Tod

PlayerMotor führt die freie Locomotion und setzt Walk-/Run-Abspieltempo nach gemessener Bewegung. Gehaltenes Run benutzt nur am Boden die eigene runSpeed; ältere Definitions ohne Run behalten ihre bisherigen Werte. Combat besitzt während eines Angriffs die Pose, AnimationReaction während Hit/Stagger. Der Motor wechselt erst danach zurück zu Idle/Walk/Run/Jump/Fall/Land; Landung beendet eine Luftattacke und stellt diese Zuständigkeit wieder her.

Die Gegner-KI benutzt denselben bewährten Schadensempfänger wie das Training. Im HumanoidCombatLab übergibt ein ausgewählter kräftiger Treffer oder Tod an BodyRecovery. Die Komponente beendet Angriff, Sprungpuffer und normale Trefferreaktion; PlayerMotor/KI geben die Pose ab. Lebende Phasen halten mit Hitstop an; Tod läuft bis zur gehaltenen Endpose weiter. Collider des Gegners sind währenddessen aus; der Spieler behält eine niedrige Boden-/Arena-Kapsel. Nach GetUp kehren normale Collider und Steuerung mit 0,45 s Trefferschutz zurück. Reset beendet jede Phase einschließlich Tod und Schutz. Ältere Szenen ohne BodyRecovery behalten den bisherigen Animator-/Rig-Tod; der Trainingskörper seine lokale Kippreaktion.

Enemy_Heavy ist ein separates AttackDefinition-/Clip-Asset. Änderungen an Spieler-Heavy sollen weder gegnerische Pose noch Warn-/Trefferverhalten unbeabsichtigt ändern. EngagementCoordinator hält maximal eine Freigabe über Telegraph/Attack/Recovery und gibt sie bei Unterbrechung, Tod, Disable oder Reset frei. B5-Gegnerbewegung bleibt kinematisch mit seitlichen Wartepositionen, Repulsion und Körperkapsel-Casts; kein NavMesh. EnemyRolesBuilder leitet Standard/Heavy/Rush sowie drei EnemyRoleDefinition-Assets aus dem vorhandenen Material ab. TrainingDummy initialisiert seinen Reset-Cache auch für zunächst inaktive Gruppengegner.

## Projektstruktur und Werkzeuge

```text
UnityProject/
  Assets/Game/
    Runtime/          # Gameplay und Präsentation
    Editor/           # Szenen-/Clip-Aufbau und kurze Sichtprüfung
    Data/             # CharacterDefinition-/AttackDefinition-Assets
    Art/              # Aktuelle eigene Grundformen, Clips, Materialien, Audio
    Prefabs/
    Scenes/CombatLab.unity
    Scenes/SparringLab.unity
    Tests/EditMode/
    Tests/PlayMode/
  Assets/QA/          # Echte Kamerabilder der Spielsequenzen
  Packages/
  ProjectSettings/
art-source/wombat/    # Begonnener, zurückgestellter Blender-Entwurf
tools/               # Lokale CLI-Hilfe und Modell-Quellskript
```

Runtime, Editor und Tests verwenden getrennte Assemblies. Die vorhandenen Builder bleiben Werkzeuge für bestimmte Stufen. Ältere Arena-/Combat-Builder nicht als allgemeinen Neuaufbau ausführen: Sie können später integrierte Assets/Szenen überschreiben. CombatPolishBuilder arbeitet B1 gezielt in vorhandene Assets, Prefab und beide Szenen ein. LabVisualReview nimmt echte Kameraposen auf und stellt nach der Sequenz Encounter und normalen Input wieder her.

CharacterActionBuilder ergänzt B3a gezielt am vorhandenen HumanoidCombat-Prefab/Controller und an der Szene. Er nutzt die B2-Retiming-/Kontaktfunktionen, ersetzt die drei bisherigen Cross-Platzhalter und arbeitet Nutzerfeedback zu Boden-/Sprung-Kick ein. Der Air-Smash hat einen bewusst gewählten niedrigen Kontaktmoment statt des Maximums der Vorwärtsreichweite. RobotReactions.controller ist eine eigene Kopie für die Humanoid-Szene; B1-Szenen behalten ihren ursprünglichen Controller. Beim bewussten Neuaufbau zuerst HumanoidCombatBuilder, danach CharacterActionBuilder ausführen; der B2-Builder allein stellt den älteren B2-Stand her.

Neue Modelle/Clips werden über den normalen Unity-Import integriert. Quaternius-Originale stehen unter ThirdParty, eigene Bewegungs-/Combat-Ableitungen unter Game/Characters. Quellen und Lizenzbelege stehen in [THIRD_PARTY_LICENSES.md](THIRD_PARTY_LICENSES.md). Kein pauschaler Ordnerumbau.

M1 ergänzt eine Darstellungsschicht für HumanoidCombatLab: normales Environment-Prefab unter Game/Environment/JunkyardPreview, ausgewählte Kenney-FBX, Poly-Haven-Beton und lokale URP/Lit-Materialien. Alte Arena-Renderer sind ausgeblendet; bestehende physische Boden-/Randobjekte bleiben aktiv. Neue Dekoration besitzt keine Collider. JunkyardPreviewBuilder erneuert nur diese Dekoration in der gespeicherten Szene; nach komplettem Szenenneuaufbau zuletzt nach B2/B3a/B3b anwenden. Details: [M1_MAP_PREVIEW.md](M1_MAP_PREVIEW.md).

BodyRecoveryBuilder ergänzt B3b mit Death01-Fall, lokal verbundenem GetUp, Robot-Kurven und den ausdrücklich niederwerfenden AttackDefinitions. Bei vollständigem Neuaufbau Reihenfolge HumanoidCombatBuilder → CharacterActionBuilder → BodyRecoveryBuilder einhalten. Ein früherer Builder allein stellt den älteren Stand her. Details in [B3B_BODY_RECOVERY.md](B3B_BODY_RECOVERY.md).

DuelSliceBuilder.Apply ergänzt danach die B3c-Abstände und das deutsche Duell-HUD in der gespeicherten Humanoid-Szene. Beim vollständigen Neuaufbau nach BodyRecoveryBuilder und JunkyardPreviewBuilder anwenden, danach MobileTouchBuilder.Apply → ShoulderChargeBuilder.Apply → EnemyRolesBuilder.Apply. Der Builder erzeugt über die vorhandene BuildPipeline ausschließlich diesen Slice für Windows/WebGL, stellt temporäre Produkteinstellungen einschließlich WebGL-Template anschließend wieder her und schreibt einen kleinen Buildbericht. Eine SessionState-Queue hält den Auftrag über den Plattformwechsel; es gibt keine neue Build-/Importarchitektur. `tools/Serve-Duel.ps1` dient dem lokalen Browserstart, `-Lan` ermöglicht einen Handytest im selben WLAN. Details in [B3C_DUEL_HANDOFF.md](B3C_DUEL_HANDOFF.md) und [MOBILE_TOUCH.md](MOBILE_TOUCH.md).

MobileTouchControls erstellt ein kleines Runtime-Overlay aus Unity UI und den vorhandenen Input-System-OnScreenControls. Diese speisen dieselben Gamepad-Aktionen wie ein physisches Gerät. LabInput sperrt zusätzliche Pointer-Angriffe bei sichtbarer Touch-Oberfläche und liest Rennen aus dem äußeren Stickbereich. Fokusverlust/Ausblenden gibt virtuelle Controls frei. Canvas-Safe-Area und responsives TouchDuel-WebGL-Template ergänzen die Darstellung; Kampfzustände und Kontaktauflösung bleiben unverändert.

`tools/Publish-Duel.ps1` verpackt den lokal gebauten WebGL-Spielstand als Release-Asset und startet `.github/workflows/play.yml` auf `main`. Der Workflow lädt den angegebenen Release und veröffentlicht die statischen Dateien über GitHub Pages. Kein Unity-Build auf GitHub und keine Build-Binärdateien in der Git-Historie; `version.json` nennt den zugehörigen Commit und Buildzeitpunkt.

## B2-Anbindung — implementierter technischer Stand

Die Animator-Phasenautorität gilt für Transform-Graybox und Humanoid. B2 ergänzt definierte Startup-/Active-/Recovery-Zeiten und retimte eigene Clip-Ableitungen, damit die ursprüngliche Bibliotheksdauer nicht die Combat-Regeln bestimmt. Eine gemeinsame Phase verbindet sichtbare Kontaktpose und Trefferfenster; kein zweiter laufender Attack-Timer. Zeit-/Clipänderungen benötigen Neuableitung/Bake durch HumanoidCombatBuilder.

Humanoid-Retargeting wertet die Hand-/Fußpose am ausgewählten echten Avatar über Animator aus und speichert eine kleine konkrete Bahn pro Attacke. Die Laufzeit verwendet diese Bahn mit derselben Phase wie die sichtbare Pose. Kontaktgenauigkeit und übersprungenes Active sind gezielt geprüft. Rig-/Clipänderungen benötigen eine Neuableitung; keine allgemeine Animations-Importarchitektur.

Die eigene Blender-/JSON-Geometrie-Bibliothek ist erhalten, aber nicht in die Spielfigur integriert. Der frühere Plan, eine eigene JSON-Mesh-Importpipeline weiterzubauen, ist zurückgestellt. Passende vorhandene Cartoon-Modelle/Rigs und Animationen werden zuerst geprüft; vollständiges Duell folgt vor zusätzlichen Gegnertypen und Levelcontent.

## B6 — eigene Kapitel-Szene

JunkyardChapterBuilder kopiert das gespeicherte B5-Labor in die eigene Szene `JunkyardChapter.unity` und verbindet drei Ableitungen des vorhandenen Junkyard-Environment mit physischem Boden und Abschnittstoren. Die Lab-Szene bleibt erhalten. Keine neuen Downloads. ChapterDefinition hält Bereichsmittelpunkte, Wellen/Spawnpunkte, Heilung und Pause als kleine Daten; JunkyardChapter besitzt Fortschritt und den aktuellen Checkpoint. ChapterGate schaltet Torplatte/Kollision. Eintritt → Wellen → Bereichssieg → Weg/Schalter → nächster Eintritt → Finale.

EngagementCoordinator erzeugt pro Welle höchstens vier Gegner aus drei inaktiven vorhandenen Rollen-Vorlagen und entfernt die alte Gruppe beim Wechsel/Retry. Er erhält Weltgrenzen des aktuellen Bereichs; TrainingDummy begrenzt Rückstoß im selben Bereich. Gemeinsame Token-/Kamera-/Schutzregeln bleiben erhalten. PlayerMotor.ResetAt verbindet bestehenden vollständigen Reset mit Checkpointposition/Facing; PlayerDefense kann die damaligen Checkpoint-HP zurückstellen. Die Bewegungsdefinition wird nur als Runtime-Kopie verändert. LabInput ergänzt F/LB und Backspace/Select, Touch ergänzt kontextabhängiges TOR ÖFFNEN und CHECKPOINT/VON VORN. B5-Gegnerwahl bleibt ausschließlich im Labor.

Beim gezielten Neuaufbau B6 erst nach dem vollständigen B5-Labor anwenden. Der B6-Builder ersetzt die Kapitel-Szene; manuelle Kapiteländerungen vorher erhalten. Haupt-Builder bleiben keine allgemein beliebig wiederholbaren Migrationswerkzeuge. Nachweise/Steuerung: [B6_JUNKYARD_CHAPTER.md](B6_JUNKYARD_CHAPTER.md).

## Werkzeugstand 7. Oktober 2026

Unity CLI ist auf **1.0.0-beta.13** aktualisiert und als aktuelle Version geprüft; Editor **6000.4.0f1** und Pipeline **0.8.0-exp.1** bleiben unverändert. Recompile, C#-Eval, Tests, Play/Stop und Spielbildaufnahme laufen direkt über die CLI am vorhandenen Editor. Pipeline `wait_for` kann tatsächliche Zustände abwarten und im selben Frame aufnehmen/pausieren; GUI-Arbeit ist nur bei konkreten Dialogen nötig. Kein neues Editor-Upgrade oder paralleler Automatisierungspfad.

## Angemessene Prüfung

EditMode prüft reine Regeln; PlayMode die echten gespeicherten Szenen, Animator/Physik, Kontakte, Defense und Reset. Geänderte gemeinsame Abläufe erhalten gezielte Regressionen. Keine neue Vollsuite allein für Dokumentation oder Materialfarbe. Tatsächliche Nachweise und verbleibendes Nutzerfeedback stehen in [TEST_SLICE_STATUS.md](TEST_SLICE_STATUS.md).

Echte Spielbilder ergänzen Regeltests, liefern aber weder subjektive Spielgefühl-Abnahme noch Hardware-/Performance-Nachweis. Windows- und WebGL-Buildprüfung gehört zum kleinen vollständigen Duell in B3. Weitere Architektur wächst aus beobachteten Anforderungen; kein globales Service-/DI-/ECS-Framework.
