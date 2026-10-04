# Architektur — implementierter B3a-Stand

Stand: 4. Oktober 2026. S1–S3, B1-Polish, B2-Humanoid-Integration und B3a-Aktionen/Reaktionen sind implementiert. Humanoid und Robot-Gegner bleiben Platzhalter für den späteren Wombat-/Cartoon-Look. Die Produktionsrichtung steht in [PRODUCTION_GUIDELINES.md](PRODUCTION_GUIDELINES.md), Regeln und Steuerung in [COMBAT_SYSTEM.md](COMBAT_SYSTEM.md).

B2-Schritte 1/2 ergänzen CharacterImportLab mit regulärem Quaternius-Humanoid und vorhandenem PlayerMotor/LabInput. Schritt 3 ergänzt separat HumanoidCombatLab mit CombatController/Defense, bestehenden Robot-Gegnern und Feedback. HumanoidCombatBuilder erzeugt eigene AttackDefinitions, retimte Clips und je 161 Avatar-Kontaktpunkte im lokalen Facing-Raum; CombatController interpoliert sie ohne SampleAnimation auf dem sichtbaren Humanoid. AttackDefinition enthält gewünschte Phasendauern sowie Avatar-/Clipreferenz für die konkrete Bahn. Animator-Zeit bleibt die einzige laufende Phase; B1-Transformclips behalten ihren alten SampleAnimation-Pfad. Root Motion bleibt aus. Die ursprünglichen Combat-Szenen sind erhalten. Details in B2_COMBAT_INTEGRATION.md.

## Verantwortlichkeiten

| Komponente | Tatsächliche Aufgabe |
| --- | --- |
| LabInput / InputFrame | Unity Input System für Move, gehaltenes Run, Jump, Light, Heavy, Kick, Evade, Reset und Debug; gemeinsame Intents für Tastatur/Maus/Gamepad |
| PlayerMotor / MotorMath | CharacterController, X/Z-Bewegung, Gravitation, Grounding, Coyote-/Jump-Buffer, Landung, Arenagrenzen und kontrollierte Angriffsschritte |
| CombatController | Spieler-Attack-Instanzen, einzelner Input-Buffer, Combo/Cancel, begrenzte Startup-Ausrichtung, Animator-Phase, Hand-/Fuß-Sweeps, Trefferfilter und Hitstop |
| AttackDefinition / CharacterDefinition | Gespeicherte Angriffs-/Kontakt-/Bewegungsdaten als ScriptableObjects; keine allgemeine Definition für jedes zukünftige System |
| TrainingDummy | Gemeinsamer Schadens-/HP-/Hitstun-/Rückstoßempfänger für Trainingspuppe und Sparring-Gegner; Ganzkörper-Tod beim Gegner und Reset |
| PlayerDefense | Spieler-HP, Treffer-Starre, Rückstoß, Bodenausweichen samt Zeitfenster und Cooldown |
| AnimationReaction | Optionale Hit-/Stagger-Pose innerhalb vorhandener Treffer-Starre, passende Clip-Geschwindigkeit, Hitstop und Rückgabe an Locomotion/KI; kein zusätzlicher Gameplay-Lock |
| EnemyBrain | Gegnerbewegung, Warnung, Animator-basierter Schlag, Recovery und unmittelbare Unterbrechung bei Treffer/Tod |
| EngagementCoordinator | Eine Angriffsfreigabe, Wechsel zwischen einem/zwei Gegnern und vollständiger Encounter-Reset |
| CombatFeedback / LabHud / ArenaCamera | Kontakt-Audio/VFX, HP-/Phasen-/Steuerungsanzeige und kontrollierte 2.5D-Perspektive |

Animationsbrücke und Spieler-Trefferauflösung sind in CombatController integriert. Es gibt keine eigenständigen Klassen namens AnimationBridge oder HitResolver. Weitere Trennung erfolgt erst bei echtem Bedarf.

## Phasenautorität und Kontakt

Die einzige aktuelle Attack-Phase ist die normalisierte Zeit des aktiven Animator-States. AttackDefinition referenziert Clip/State und enthält Active-, Chain-, Hit-Chain-, Heavy-Cancel- und Movement-Release-Fenster. Separate Telegraph-/Recovery-Zeiten der Gegner und Evade-/Hitstun-Zeiten beschreiben andere Zustände, keine zweite Uhr für denselben Schlag.

Startup/Recovery verursachen keinen Schaden. Der Controller begrenzt das seit dem letzten Frame durchlaufene Intervall auf Active und wertet direkte Transform-Clippose in kleinen Schritten aus. Capsules zwischen dem jeweiligen Faust-/Fußkontakt erfassen schnelle Bewegung und übersprungene Fenster. Root-Bewegung des Spielers wird berücksichtigt; danach wird die sichtbare aktuelle Clippose wiederhergestellt. `rightHand` wählt die rechte/linke Seite auch bei einem Fußangriff, `foot` bestimmt Hand gegenüber Fuß.

Getrennte Trigger-Hurtboxes auf Layer 8, Körpercollider und explizite Kontaktpunkte erhalten unterschiedliche Aufgaben. Team-/Front-/Alive-Filter und ein Zielset begrenzen Schaden auf einmal pro Ziel und Attack-Instanz. Gegner lösen ihren Faustkontakt gegen PlayerMotor/PlayerDefense auf und verursachen derzeit 12 Schaden. Der Spieler-Sweep hat 32, der Gegner-Sweep 12 vorab reservierte Kontaktplätze; das ist kein unbegrenztes Crowd-System.

B1 besitzt unterschiedliche Boden- und Luftclips. Am Boden verschiebt der Motor die Figur kontrolliert aus dem Fortschritt der Clipphase; freie Bewegung/Drehung kommt bei Phase 0,82 zurück. Luftangriffe erhalten Motor-Gravitation und Luftkontrolle, halten ihre Angriffsrichtung und werden bei Landung beendet. Keine parallele Root Motion oder Zielhöhenkorrektur.

Die Ausgangsrotation vor dem Angriffsbeginn begrenzt die gesamte frühe Ausrichtung auf 25°. Nach Active bleibt die Richtung bis zur späten Recovery fest. Folgeschläge starten eine neue Instanz und dürfen sich erneut begrenzt ausrichten.

Der einzelne Buffer altert in Combat-Zeit und ruht bei Hitstop. Treffer erlauben früheren Light-Anschluss als Fehlschläge. Attack-Ende, externer Animator-Wechsel, Disable, Defense und Reset bereinigen laufenden Angriff und Buffer. Am Boden friert Hitstop Motor/Angriff ein, in der Luft bleibt die Flugbahn aktiv. UI und Eingaben benutzen keine globale Zeitpause.

## Bewegung, Pose und Tod

PlayerMotor führt die freie Locomotion und setzt Walk-/Run-Abspieltempo nach gemessener Bewegung. Gehaltenes Run benutzt nur am Boden die eigene runSpeed; ältere Definitions ohne Run behalten ihre bisherigen Werte. Combat besitzt während eines Angriffs die Pose, AnimationReaction während Hit/Stagger. Der Motor wechselt erst danach zurück zu Idle/Walk/Run/Jump/Fall/Land; Landung beendet eine Luftattacke und stellt diese Zuständigkeit wieder her.

Die Gegner-KI benutzt denselben bewährten Schadensempfänger wie das Training. Bei 0 HP beendet sie Angriff/Warnung/Freigabe; TrainingDummy beendet die Reaktion, deaktiviert Animator/Collider und kippt das gesamte Rig. Reset stellt den gespeicherten Ausgangszustand wieder her. Der Trainingskörper ohne EnemyBrain behält seine eigene lokale Kippreaktion. Hit/Stagger ist in B3a angebunden; lebendes Knockdown/GetUp und eine eigene Spieler-Todesanimation folgen in B3b.

Enemy_Heavy ist ein separates AttackDefinition-/Clip-Asset. Änderungen an Spieler-Heavy sollen weder gegnerische Pose noch Warn-/Trefferverhalten unbeabsichtigt ändern. EngagementCoordinator hält maximal eine Freigabe über Telegraph/Attack/Recovery und gibt sie bei Unterbrechung, Tod, Disable oder Reset frei. Gegnerbewegung bleibt kinematisch mit einfachen Wartepositionen und Repulsion; kein NavMesh.

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

## B2-Anbindung — implementierter technischer Stand

Die Animator-Phasenautorität gilt für Transform-Graybox und Humanoid. B2 ergänzt definierte Startup-/Active-/Recovery-Zeiten und retimte eigene Clip-Ableitungen, damit die ursprüngliche Bibliotheksdauer nicht die Combat-Regeln bestimmt. Eine gemeinsame Phase verbindet sichtbare Kontaktpose und Trefferfenster; kein zweiter laufender Attack-Timer. Zeit-/Clipänderungen benötigen Neuableitung/Bake durch HumanoidCombatBuilder.

Humanoid-Retargeting wertet die Hand-/Fußpose am ausgewählten echten Avatar über Animator aus und speichert eine kleine konkrete Bahn pro Attacke. Die Laufzeit verwendet diese Bahn mit derselben Phase wie die sichtbare Pose. Kontaktgenauigkeit und übersprungenes Active sind gezielt geprüft. Rig-/Clipänderungen benötigen eine Neuableitung; keine allgemeine Animations-Importarchitektur.

Die eigene Blender-/JSON-Geometrie-Bibliothek ist erhalten, aber nicht in die Spielfigur integriert. Der frühere Plan, eine eigene JSON-Mesh-Importpipeline weiterzubauen, ist zurückgestellt. Passende vorhandene Cartoon-Modelle/Rigs und Animationen werden zuerst geprüft; vollständiges Duell folgt vor zusätzlichen Gegnertypen und Levelcontent.

## Angemessene Prüfung

EditMode prüft reine Regeln; PlayMode die echten gespeicherten Szenen, Animator/Physik, Kontakte, Defense und Reset. Geänderte gemeinsame Abläufe erhalten gezielte Regressionen. Keine neue Vollsuite allein für Dokumentation oder Materialfarbe. Tatsächliche Nachweise und verbleibendes Nutzerfeedback stehen in [TEST_SLICE_STATUS.md](TEST_SLICE_STATUS.md).

Echte Spielbilder ergänzen Regeltests, liefern aber weder subjektive Spielgefühl-Abnahme noch Hardware-/Performance-Nachweis. Windows- und WebGL-Buildprüfung gehört zum kleinen vollständigen Duell in B3. Weitere Architektur wächst aus beobachteten Anforderungen; kein globales Service-/DI-/ECS-Framework.
