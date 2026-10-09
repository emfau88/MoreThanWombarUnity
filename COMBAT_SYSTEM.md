# Combat und Sparring — B9–B12 lokal

## Aktueller Ausbau

Der lokale Stand ergänzt MP (100 Maximum), Boden-Smash bei tatsächlicher Landung (22 MP), Durchbruch durch leichte Gegner (18 MP) und Druckwelle gegen bis zu drei Ziele (26 MP). Sprung + K, E und Q lösen die Aktionen aus; Touch und Gamepad sind angebunden. Mehrfachtreffer erhalten einzelne Trefferreaktionen und kompakte Effekte. CrowdCombatLab startet mit acht Gegnern aus Raufbold, Werfer und Schläger; zehn sind optional. Zwei Nahkämpfer können gleichzeitig angreifen. B12 nutzt diese Regeln in fünf Begegnungen mit genau 30 Gegnern und endlichem Nachschub; [Stage-Ablauf](B12_LF2_STAGE.md).

Aktuelle Werte, Steuerung und Nachweise: [B9_B11_LF2_COMBAT.md](B9_B11_LF2_COMBAT.md). Die folgenden Abschnitte dokumentieren die bisherige Grundlage; insbesondere die frühere Stoßreichweite, Einzelkontakt-Regel und Vier-Gegner-Begrenzung gelten nicht für den neuen Crowd-Ausbau.

## Bisherige Grundlage bis B8

Stand: 9. Oktober 2026. Erhaltene B1-Regeln, B2-Humanoid-Anbindung, B3a-Aktionen, B3b-Körpererholung, B3c-Duell-Tuning, B4-Schulterstoß/B5-Gegnerrollen, B6-Kapitelablauf und B7-Präsentation. B7 verändert keine Kampfwerte, Kontaktzeiten, Wellen oder Checkpoint-Regeln; Kapitel ergänzt lesbarere Gesichtsfarben/HP-Marker und importierte Kontakt-/Warn-/Schritt-/Signalsounds. Figuren-Rigs bleiben erhalten, Wombat-Gestaltung bleibt offen. Siehe [NEXT_STEPS.md](NEXT_STEPS.md) und [B7_PRESENTATION.md](B7_PRESENTATION.md).

B2-Schritte 1–3 liefern zusätzlich CharacterImportLab und HumanoidCombatLab. Letztere verbindet den gültigen Humanoid mit denselben Kampfregeln, konkreten Angriffszeiten und retargeteten Faust-/Fußbahnen. Die Tabelle unten beschreibt die erhaltenen B1-Werte; Humanoid-Zeiten stehen in [B2_COMBAT_INTEGRATION.md](B2_COMBAT_INTEGRATION.md), aktuelle Aktionen und Reaktionen in [B3A_ACTIONS.md](B3A_ACTIONS.md).

## Spielen

Ab B8 startet `JunkyardChapter` im Startmenü. ESC/P, Gamepad Start oder PAUSE frieren Sitzung, Angriff und Gegner ein; beim Fortsetzen sind vorgemerkte Eingaben bereinigt. Ergebnisanzeige verwendet vorhandenes Checkpoint-Retry. Im separaten Labor bleiben R/Start und die bisherigen Bindings erhalten. Kampfwerte, Kontakte und Wellenfolge sind unverändert. Details: [B8_SESSION.md](B8_SESSION.md).

`UnityProject/Assets/Game/Scenes/HumanoidCombatLab.unity` öffnen, Play drücken und die Game-Ansicht fokussieren. `SparringLab.unity` bleibt B1-Vergleich, `CombatLab.unity` Training ohne Gegenangriffe.

- WASD / Pfeiltasten / linker Gamepad-Stick: bewegen.
- Ctrl / linker Gamepad-Trigger gehalten: Rennen im HumanoidCombatLab; beim Loslassen wieder Gehen.
- Leertaste / Gamepad South (A bzw. Kreuz): springen.
- J / linke Maustaste / Gamepad West (X bzw. Quadrat): Light; rhythmisch neu drücken für Jab → Cross → Finisher. Gedrückthalten erzeugt keine automatische Combo.
- K / rechte Maustaste / Gamepad North (Y bzw. Dreieck): Heavy.
- L / rechte Schultertaste (RB / R1): Kick; aus neutral oder als Abschluss nach Light 2.
- Im Sprung starten J und L einen Air-Kick, K einen Air-Smash.
- Shift / Gamepad East (B bzw. Kreis): am Boden ausweichen.
- E / rechter Gamepad-Trigger / Touch STOSS: Schulterstoß am Boden, bis zu 2 m in fester Startrichtung; stoppt am ersten gültigen Kontakt oder an Kollision. Details: [B4-Schulterstoß](B4_SHOULDER_CHARGE.md).
- HumanoidCombatLab lokal: 1–4 Gegner; 3/4 sind Mischgruppen. D-Pad oben/Touch GEGNER schaltet weiter. Älteres Sparring bleibt bei 1/2.
- R / Gamepad Start: vollständig zurücksetzen. H / rechter Stick-Klick: Debuganzeige mit Phase, Attack-Instanz und Faust-/Fußkontakt.

B6 liefert eine eigene `JunkyardChapter.unity` mit denselben Aktionen: neun Wellen in drei Bereichen. F/LB öffnet das nahe Schaltertor nach Bereich 1; Touch TOR ÖFFNEN erscheint dort. R/Start/CHECKPOINT wiederholt den aktuellen Abschnitt mit den gespeicherten HP, Backspace/Select/VON VORN beginnt das Kapitel neu. Die Lab-Gegnerwahl 1–4 ist im Kapitel deaktiviert. Nach Bereichssieg 30 HP Heilung bis maximal 100. Der Vorarbeiter ist eine eigene Heavy-Rolle (100 HP, 20 Schaden), kein neues Kampfsystem. Details: [B6_JUNKYARD_CHAPTER.md](B6_JUNKYARD_CHAPTER.md).

Normale Angriffe folgen der aktuellen Blickrichtung; keine automatische Zielerfassung. Bewegungsinput darf sie beim Start und im frühen Startup insgesamt um höchstens 25° korrigieren. Ab Active bleibt die Richtung bis zur Bewegungsfreigabe in später Recovery fest. Ein Folgeschlag erhält eine neue begrenzte Ausrichtung. Der Schulterstoß übernimmt die gewünschte Bewegungsrichtung vollständig beim Start und hält sie bis zum Ende fest; kein nachträgliches Lenken oder Ausweich-Cancel.

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

- Eine gepufferte Absicht bleibt höchstens 0,30 Combat-Sekunden erhalten. Eine neue Absicht ersetzt sie; bei gleichzeitigem Input gilt Stoß vor Heavy vor Kick vor Light. Stoß wird während einer laufenden Attacke und in der Luft verworfen.
- Light 1 → Light 2 → Light 3: Anschluss ab 0,48 nach bestätigtem Treffer, ab 0,64 nach Fehlschlag, jeweils bis 0,88. Jede Stufe hat eine neue Attack-Instanz.
- Nach Light 2 ist Kick im gleichen Treffer-/Fehlschlagfenster ein alternativer Abschluss. Kick setzt die Light-Kette nicht fort.
- Heavy darf eine Light-Recovery ab 0,80 abbrechen. Kein entsprechender Heavy-Cancel aus Kick oder Luftangriff.
- Angriffsende, Abbruch, Reset, Disable und aktive Defense-Sperre räumen den Puffer auf. Während Hitstop altert er nicht.

## Treffer, Reaktion und Tod

Die Trainingspuppe hat 150 HP; alte Sparring-Gegner haben 80 HP. B5-Rollen im HumanoidCombatLab: Standard 65, Agile 55, Heavy 80 HP. Die frühen Lights haben weniger Rückstoß (0,17 / 0,22), der Finisher (0,75) und Heavy (1,25) schaffen Abstand. Diese Daten steuern einen kinematischen Geschwindigkeitsimpuls mit Dämpfung, keine exakt garantierte Rückstoßstrecke.

Körperkollision und Trigger-Hurtbox auf Layer 8 `CombatHurtbox` sind getrennt. Faust oder Fuß sind explizite Kontaktpunkte mit Angriffsradius; keine aus Mesh-Bounds abgeleiteten Schadenszonen. Team, Lebensstatus, räumlicher Kontakt und Front-Richtung filtern Treffer. Jeder Gegner erhält pro Attack-Instanz höchstens einen Treffer, auch mit mehreren Hurtboxes. Der Spieler-Sweep verwendet einen festen Puffer mit 32 Kontakten.

Treffer erzeugen Hitstun, Rückstoß, kurzen Flash, Kontaktblitz/Funken und eigene synthetisierte Kontaktklänge. Sie übernehmen keine Schadensentscheidung. Im HumanoidCombatLab spielt AnimationReaction sichtbares Hit/Stagger innerhalb der vorhandenen Treffer-Starre: beim Gegner starke Reaktion für Heavy oder Schaden ≥ 18, beim Spieler für Schaden ≥ 20. Knockdown oder Tod hat Vorrang. Locomotion/KI überschreiben die Pose währenddessen nicht. Hitstop friert Gegnerreaktion samt Restdauer ein; danach kehrt die Zuständigkeit zu Bewegung/KI zurück. Reset räumt die Reaktion auf. Der Trainingskörper kippt weiterhin lokal.

Hitstop stoppt am Boden Spieler-Motor und Angriffspose sowie gegnerische Bewegung/Hitstun. Eine laufende Flugbahn bleibt aktiv. UI, Eingaben und Reset laufen weiter; kein globales `Time.timeScale`. Der kurze Ganzkörper-Fall eines bereits toten Gegners läuft nach Echtzeit weiter.

Im HumanoidCombatLab werfen Spieler-Heavy/Luft-Smash und Gegner-Heavy ausdrücklich nieder (`knocksDown`), unabhängig vom Schadenswert. Fall 0,42 s → Boden 0,40 s → GetUp 0,80 s sperrt Aktionen und zusätzliche Treffer. Danach sind Steuerung und Collider wieder frei, mit 0,45 s weiterem Trefferschutz gegen sofortiges Dauerniederwerfen. Lebende Phasen respektieren Hitstop. Gegnercollider sind während Fall/Boden/GetUp aus; die Spieler-Kapsel bleibt niedrig für Boden, Gravitation und Arenagrenzen.

Bei 0 HP spielen beide Figuren den Ganzkörper-Fall als Death und halten die liegende Endpose. Gegnerfreigabe/Warnung enden sofort; kein Aufstehen. Ältere B1-Szenen ohne BodyRecovery behalten ihren bisherigen Rig-Tod mit abgeschaltetem Animator/Collidern. Die separate Trainingspuppe behält ihre eigene Kippreaktion. R stellt HP, Position, Pose, Animator, ursprüngliche Collider, normale Kapselhöhe, Schutz, Farbe, Warnung, Angriff, Puffer und Freigaben wieder her. Details in [B3B_BODY_RECOVERY.md](B3B_BODY_RECOVERY.md).

## Sparring und Ausweichen

EnemyBrain verwendet Approach, Telegraph, Attack, Recovery, Reposition, Stagger und Down. TrainingDummy bleibt der gemeinsame Schadens-/Rückstoßempfänger. Der Gegner besitzt ein eigenes `Enemy_Heavy`-Asset; Spielertuning verändert seine Animation und Daten nicht automatisch.

B3c setzt die Angriffsdistanz im HumanoidCombatLab auf 1,30 m und den gewünschten Repositionsabstand auf 1,20 m. Aus dem tatsächlichen KI-Anlauf trifft damit die gesamte Light-Kette statt nur einer Stufe. Die älteren Szenen behalten ihre 1,65/1,25-m-Vorgaben. EngagementCoordinator vergibt während Spieler-Knockdown/GetUp und anschließendem Aufstehschutz keine neue Angriffserlaubnis; danach läuft der Kampf weiter.

B5-Rollen definieren eigene Warnung/Schaden: Standard 0,45 s/8 HP, Agile 0,80 s/10 HP, Heavy 0,75 s/18 HP mit Knockdown. Ein orange/roter Bodenmarker kündigt den Angriff an. Agile zeigt zusätzlich seine feste Ansturmspur und läuft im Active maximal 2,35 m; Kontakt, Körper-/Wandkollision und Arenarand stoppen den Weg. Die Richtung steht ab Warnstart fest. Die Attack-Phase kommt weiterhin ausschließlich aus dem Animator; pro Angriff höchstens ein Spielertreffer. Ältere Rollen ohne Definition behalten 0,55 s Warnung und 12 Schaden. Details/Werte: [B5_ENEMY_ROLES.md](B5_ENEMY_ROLES.md).

Genau ein sichtbarer bereiter Gegner besitzt die Freigabe für Telegraph/Attack/Recovery. Die Vergabe rotiert über alle aktiven Rollen. Nach dem Clip folgen rollenabhängig 0,35/0,60 s zusätzliche Recovery und 0,30 s Gruppenpause; alte Rollen behalten 0,45 s Recovery. Treffer, Tod, Disable, Animator-Abbruch und Encounter-Reset geben die Freigabe frei. Außerhalb der Kamera startet kein Angriff; laufende Warnung/Attack wird dort abgebrochen. Wartende Rollen verteilen sich seitlich, weichen in unmittelbarer Nähe zurück und nutzen Repulsion und Körperkapsel-Kollision. Kein NavMesh oder vollständiges Crowd-System.

PlayerDefense verwaltet 100 HP, Treffer-Starre, Rückstoß und Bodenausweichen. Evade dauert 0,32 s bei 8 m/s, mit 0,65 s Cooldown und Unverwundbarkeit nur zwischen 0,05 und 0,24 s. Es cancelt den eigenen Angriff und nutzt ohne Richtungsinput die Blickrichtung. In der Luft sowie während Fall/GetUp/Tod ist es gesperrt. Der Aufstehschutz verhindert Treffer bei bereits freier Steuerung. R startet auch nach Tod oder Sieg neu.

## Kurze Übergabe und Nachweise

B3c nutzt kürzere leichte/starke Kontaktblitze (0,085/0,15 s), Stärken 0,23/0,36 und 78 % Light-Soundpegel gegenüber Heavy. Hitstop-Zeiten und Clip-/Kontaktbahnen bleiben erhalten. Das Humanoid-Duell zeigt deutsche Kampfhinweise, beide HP-Stände, Aufstehschutz, Sieg/Niederlage und R-Retry; H ergänzt technische Zustände. Builds und tatsächliche Prüfumfänge stehen in [B3C_DUEL_HANDOFF.md](B3C_DUEL_HANDOFF.md).

Zum Spielgefühl: anlaufen und stoppen, Richtungswechsel beim Schlag, J-Kette nach Treffer und Fehlschlag, K, L, Sprung + J/K, Shift, KO und R; anschließend mit 2 den wechselnden Gegnerdruck ansehen. Feintuning folgt konkretem Feedback an dieser spielbaren Basis.

Vorhandene Tests prüfen Phasen, Buffer, Team-/Frontfilter, echte Animator-/Physik-Sweeps, Angriffsschritte, Luftkontakt, Kick-Reichweite, Ganzkörper-Tod/Reset, Defense und Eingabebindings. Die gezielte B1-Abschlussprüfung und ihre Resultate stehen in [TEST_SLICE_STATUS.md](TEST_SLICE_STATUS.md). Simulierte Gamepad-Bindings belegen keinen physischen Controller-Test; deterministische Zeitschritte kein Performanceprofil. Die Kamerabilder unter `UnityProject/Assets/QA/` zeigen echte Spielposen, nicht das HUD oder subjektives Nutzer-Spielgefühl.
