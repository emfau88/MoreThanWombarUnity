# Combat und Sparring — aktueller B3a-Stand

Stand: 4. Oktober 2026. Diese Beschreibung enthält die erhaltenen B1-Regeln, B2-Humanoid-Anbindung und B3a mit Run, unterscheidbaren Kernaktionen und Hit/Stagger. Grundform-Figuren und temporärer Human bleiben Platzhalter. Lebendes Knockdown/GetUp folgt in B3b; Wombat-Gestaltung bleibt eine spätere Entscheidung. Siehe [NEXT_STEPS.md](NEXT_STEPS.md).

B2-Schritte 1–3 liefern zusätzlich CharacterImportLab und HumanoidCombatLab. Letztere verbindet den gültigen Humanoid mit denselben Kampfregeln, konkreten Angriffszeiten und retargeteten Faust-/Fußbahnen. Die Tabelle unten beschreibt die erhaltenen B1-Werte; Humanoid-Zeiten stehen in [B2_COMBAT_INTEGRATION.md](B2_COMBAT_INTEGRATION.md), aktuelle Aktionen und Reaktionen in [B3A_ACTIONS.md](B3A_ACTIONS.md).

## Spielen

`UnityProject/Assets/Game/Scenes/HumanoidCombatLab.unity` öffnen, Play drücken und die Game-Ansicht fokussieren. `SparringLab.unity` bleibt B1-Vergleich, `CombatLab.unity` Training ohne Gegenangriffe.

- WASD / Pfeiltasten / linker Gamepad-Stick: bewegen.
- Ctrl / linker Gamepad-Trigger gehalten: Rennen im HumanoidCombatLab; beim Loslassen wieder Gehen.
- Leertaste / Gamepad South (A bzw. Kreuz): springen.
- J / linke Maustaste / Gamepad West (X bzw. Quadrat): Light; rhythmisch neu drücken für Jab → Cross → Finisher. Gedrückthalten erzeugt keine automatische Combo.
- K / rechte Maustaste / Gamepad North (Y bzw. Dreieck): Heavy.
- L / rechte Schultertaste (RB / R1): Kick; aus neutral oder als Abschluss nach Light 2.
- Im Sprung starten J und L einen Air-Kick, K einen Air-Smash.
- Shift / Gamepad East (B bzw. Kreis): am Boden ausweichen.
- 1 / 2: im Sparring einen oder zwei Gegner aktivieren.
- R / Gamepad Start: vollständig zurücksetzen. H / rechter Stick-Klick: Debuganzeige mit Phase, Attack-Instanz und Faust-/Fußkontakt.

Angriffe folgen der aktuellen Blickrichtung; keine automatische Zielerfassung. Bewegungsinput darf sie beim Start und im frühen Startup insgesamt um höchstens 25° korrigieren. Ab Active bleibt die Richtung bis zur Bewegungsfreigabe in später Recovery fest. Ein Folgeschlag erhält eine neue begrenzte Ausrichtung.

## Angriffe und Bewegung

Die Werte stammen aus den gespeicherten AttackDefinition-Assets. Clipzeit ist ohne Hitstop angegeben; Fenster sind normalisierte Clippositionen, keine unabhängig laufenden Timer.

| Angriff | Cliplänge | Active | Schaden | Hitstop | Vorwärtsschritt |
| --- | --- | --- | --- | --- | --- |
| Light 1 / Jab | 0,38 s | 0,25–0,46 | 10 | 0,045 s | 0,23 m |
| Light 2 / Cross | 0,43 s | 0,25–0,46 | 13 | 0,050 s | 0,23 m |
| Light 3 / Finisher | 0,56 s | 0,25–0,46 | 20 | 0,065 s | 0,23 m |
| Heavy / Smash | 0,86 s | 0,38–0,53 | 32 | 0,085 s | 0,22 m |
| Kick | 0,60 s | 0,30–0,50 | 18 | 0,060 s | 0,27 m |
| Air-Kick | 0,44 s | 0,20–0,65 | 14 | 0,055 s | 0 |
| Air-Smash | 0,56 s | 0,22–0,62 | 26 | 0,055 s | 0 |

Der CharacterController begrenzt Angriffsschritte an Körperkollision und Arenarand. Der Weg wird einmalig aus der fortgeschrittenen Clipphase zwischen 0,08 und 0,40 berechnet, ohne importierte Root Motion. Hitstop vervielfacht den Weg nicht.

Am Boden bleiben freie Bewegung und ein neuer Sprung zunächst gesperrt. Ab Phase 0,82 erlaubt der laufende Bodenangriff wieder Bewegung und Drehen; seine Pose bleibt bis zum Ende unter Combat-Kontrolle. Ein Sprung wird während des Angriffs weiterhin nicht angenommen. Walk-/Run-Tempo richtet sich in freier Bewegung nach der tatsächlich zurückgelegten Strecke, einschließlich Blockade durch einen Körper. HumanoidCombatLab nutzt 3 m/s Gehen und 6,2 m/s Rennen. Luftkontrolle bleibt moveSpeed × airControl; Run erzeugt keinen Luft-Sprint. Ältere CharacterDefinitions mit runSpeed = 0 behalten ihre bisherige Bewegung.

Luftangriffe besitzen eigene Clips und Fuß-/Faustkontakte. Gravitation und X/Z-Luftkontrolle laufen weiter; die Schlagrichtung bleibt während der Luftattacke fest. Gleichzeitiges Jump + Attack startet zuerst den Sprung. Keine künstliche Zielhöhe, kein Doppelsprung und kein Schweben bei Hitstop. Landung beendet eine noch laufende Luftattacke und führt in die kurze Landepose. Ohne reale räumliche Überschneidung trifft ein zu hoher oder entfernter Angriff nicht.

## Phasen, Kontakte und Verkettung

CombatController liest in LateUpdate den aktiven Animator-State und dessen normalisierte Zeit als einzige Angriffsphase. Startup und Recovery verursachen keinen Schaden. Ein externer State-Wechsel beendet den Angriff.

Die B1-Graybox besitzt direkt animierte Transformkurven. Für das auf Active begrenzte Intervall wertet `AnimationClip.SampleAnimation` Zwischenposen aus; anschließend wird die sichtbare Clippose wiederhergestellt. Der Humanoid nutzt stattdessen je Attacke 161 am konkreten Avatar ausgewertete Hand-/Fußpunkte im lokalen Facing-Raum. Er interpoliert diese Bahn ohne Umposen des sichtbaren Rigs. Beide Pfade verwenden dieselben Capsules im tatsächlich gekreuzten Active-Intervall und berücksichtigen Rootbewegung; auch ein übersprungenes Active-Fenster wird abgefragt.

Für neue Humanoid-Attacken enthält AttackDefinition gewünschte Startup-/Active-/Recovery-Sekunden. Der Builder retimt die eigenen Clipkurven abschnittsweise und richtet den tatsächlichen Kontakt darauf aus; die Animator-Geschwindigkeit berücksichtigt Gesamtdauer und Hitstop. Die normalisierte Animator-Zeit bleibt die einzige laufende Phasenquelle. Neue Clip-/Rigkombinationen benötigen einen neuen Bake. Falscher Avatar oder eine abweichende Clipreferenz verhindern den Start der Humanoid-Attacke.

- Eine gepufferte Absicht bleibt höchstens 0,30 Combat-Sekunden erhalten. Eine neue Absicht ersetzt sie; bei gleichzeitigem Input gilt Heavy vor Kick vor Light.
- Light 1 → Light 2 → Light 3: Anschluss ab 0,48 nach bestätigtem Treffer, ab 0,64 nach Fehlschlag, jeweils bis 0,88. Jede Stufe hat eine neue Attack-Instanz.
- Nach Light 2 ist Kick im gleichen Treffer-/Fehlschlagfenster ein alternativer Abschluss. Kick setzt die Light-Kette nicht fort.
- Heavy darf eine Light-Recovery ab 0,80 abbrechen. Kein entsprechender Heavy-Cancel aus Kick oder Luftangriff.
- Angriffsende, Abbruch, Reset, Disable und aktive Defense-Sperre räumen den Puffer auf. Während Hitstop altert er nicht.

## Treffer, Reaktion und Tod

Die Trainingspuppe hat 150 HP, der Sparring-Gegner 80 HP. Die frühen Lights haben weniger Rückstoß (0,17 / 0,22), der Finisher (0,75) und Heavy (1,25) schaffen Abstand. Diese Daten steuern einen kinematischen Geschwindigkeitsimpuls mit Dämpfung, keine exakt garantierte Rückstoßstrecke.

Körperkollision und Trigger-Hurtbox auf Layer 8 `CombatHurtbox` sind getrennt. Faust oder Fuß sind explizite Kontaktpunkte mit Angriffsradius; keine aus Mesh-Bounds abgeleiteten Schadenszonen. Team, Lebensstatus, räumlicher Kontakt und Front-Richtung filtern Treffer. Jeder Gegner erhält pro Attack-Instanz höchstens einen Treffer, auch mit mehreren Hurtboxes. Der Spieler-Sweep verwendet einen festen Puffer mit 32 Kontakten.

Treffer erzeugen Hitstun, Rückstoß, kurzen Flash, Kontaktblitz/Funken und eigene synthetisierte Kontaktklänge. Sie übernehmen keine Schadensentscheidung. Im HumanoidCombatLab spielt AnimationReaction sichtbares Hit/Stagger innerhalb der vorhandenen Treffer-Starre: beim Gegner starke Reaktion für Heavy oder Schaden ≥ 18, beim Spieler für Schaden ≥ 20. Locomotion/KI überschreiben die Pose währenddessen nicht. Hitstop friert Gegnerreaktion samt Restdauer ein; danach kehrt die Zuständigkeit zu Bewegung/KI zurück. Reset räumt die Reaktion auf. Der Trainingskörper kippt weiterhin lokal. Lebendes Knockdown/GetUp folgt in B3b.

Hitstop stoppt am Boden Spieler-Motor und Angriffspose sowie gegnerische Bewegung/Hitstun. Eine laufende Flugbahn bleibt aktiv. UI, Eingaben und Reset laufen weiter; kein globales `Time.timeScale`. Der kurze Ganzkörper-Fall eines bereits toten Gegners läuft nach Echtzeit weiter.

Bei 0 HP verliert der Gegner sofort seine Angriffsfreigabe und Warnung, geht in DOWN, schaltet den Animator und seine Collider ab. Das gesamte Rig fällt in etwa 0,42 s um und bleibt liegen. Die separate Trainingspuppe behält ihre eigene Kippreaktion. Reset stellt HP, Position, Rig-Pose, Animator, ursprüngliche Collider-Aktivierung, Farbe, Warnung, Angriff, Puffer und Freigaben wieder her.

## Sparring und Ausweichen

EnemyBrain verwendet Approach, Telegraph, Attack, Recovery, Reposition, Stagger und Down. TrainingDummy bleibt der gemeinsame Schadens-/Rückstoßempfänger. Der Gegner besitzt ein eigenes `Enemy_Heavy`-Asset; Spielertuning verändert seine Animation und Daten nicht automatisch.

Ein orange/roter Bodenmarker kündigt 0,55 s lang den Schlag an. Die Richtung wird beim Warnstart festgelegt. Die anschließende Attack-Phase kommt aus dem Animator, mit Active 0,38–0,53. Pro Angriff kann der Spieler einmal 12 Schaden erhalten; der Gegner verwendet dafür seine eigene Damage-Auflösung, nicht den Spieler-Schadenswert 32 im gemeinsamen Asset-Typ.

Genau ein Gegner besitzt die Freigabe für Telegraph/Attack/Recovery. Nach dem Clip folgen 0,45 s Recovery, anschließend 0,30 s Gruppenpause. Treffer, Tod, Disable, Animator-Abbruch und Encounter-Reset geben die Freigabe frei. Wartende Gegner bewegen sich seitlich; kurze Repulsion verhindert Stapelung. Kein NavMesh oder vollständiges Crowd-System.

PlayerDefense verwaltet 100 HP, Treffer-Starre, Rückstoß und Bodenausweichen. Evade dauert 0,32 s bei 8 m/s, mit 0,65 s Cooldown und Unverwundbarkeit nur zwischen 0,05 und 0,24 s. Es cancelt den eigenen Angriff und nutzt ohne Richtungsinput die Blickrichtung. In der Luft ist es gesperrt. Spieler-Tod sperrt Aktionen; eine eigene animierte Spieler-Todesfolge gehört zum vollständigen Duell in B3. R startet auch nach Tod oder Sieg neu.

## Kurze Übergabe und Nachweise

Zum Spielgefühl: anlaufen und stoppen, Richtungswechsel beim Schlag, J-Kette nach Treffer und Fehlschlag, K, L, Sprung + J/K, Shift, KO und R; anschließend mit 2 den wechselnden Gegnerdruck ansehen. Feintuning folgt konkretem Feedback an dieser spielbaren Basis.

Vorhandene Tests prüfen Phasen, Buffer, Team-/Frontfilter, echte Animator-/Physik-Sweeps, Angriffsschritte, Luftkontakt, Kick-Reichweite, Ganzkörper-Tod/Reset, Defense und Eingabebindings. Die gezielte B1-Abschlussprüfung und ihre Resultate stehen in [TEST_SLICE_STATUS.md](TEST_SLICE_STATUS.md). Simulierte Gamepad-Bindings belegen keinen physischen Controller-Test; deterministische Zeitschritte kein Performanceprofil. Die Kamerabilder unter `UnityProject/Assets/QA/` zeigen echte Spielposen, nicht das HUD oder subjektives Nutzer-Spielgefühl.
