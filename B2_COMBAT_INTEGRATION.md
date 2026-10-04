# B2 Schritt 3 — Humanoid-Combat

Dieser Abschnitt dokumentiert den B2-Zwischenstand. B3a hat die hier beschriebenen vorläufigen Cross-Varianten für Finisher/Heavy/Air-Heavy ersetzt und Run sowie Hit/Stagger ergänzt; aktueller Stand in [B3A_ACTIONS.md](B3A_ACTIONS.md).

Stand: 3. Oktober 2026. Der Nutzer hat nach dem Commit von Schritten 1/2 den nächsten Schritt beauftragt. Die temporäre menschliche Figur bleibt unverändert; Wombat-Gestaltung ist weiterhin offen.

## Spielbarer Stand

`UnityProject/Assets/Game/Scenes/HumanoidCombatLab.unity` verbindet den regulär importierten Avatar mit vorhandenem Motor, Input, CombatController, Defense, Robot-Gegnern, EngagementCoordinator, Kamera, HUD, Hitstop, Trefferfeedback und Reset. Tastatur-/Gamepad-Bindings entsprechen dem B1-Sparring. Die ursprünglichen CombatLab/SparringLab und die reine CharacterImportLab bleiben erhalten.

Eigenes Prefab, Animator-Controller, AttackDefinitions und Clip-Ableitungen liegen unter `Assets/Game/Characters/HumanoidCombat/`. Kein weiteres Content-Pack oder Dienst wurde benötigt.

## Timing und Darstellung

| Attacke | Startup | Active | Recovery | Gesamt | Clipbasis |
| --- | --- | --- | --- | --- | --- |
| Light 1 | 0,10 s | 0,09 s | 0,19 s | 0,38 s | Quaternius Jab |
| Light 2 | 0,13 s | 0,10 s | 0,20 s | 0,43 s | Quaternius Cross |
| Light 3 | 0,17 s | 0,11 s | 0,28 s | 0,56 s | Cross-Ableitung als vorläufiger Finisher |
| Heavy | 0,29 s | 0,13 s | 0,44 s | 0,86 s | Cross-Ableitung als vorläufige Heavy-Pose |
| Kick | 0,18 s | 0,13 s | 0,34 s | 0,65 s | Eigene kurze Humanoid-Kick-Ableitung |
| Air-Kick | 0,08 s | 0,16 s | 0,24 s | 0,48 s | Eigene tiefere Luftkick-Ableitung |
| Air-Heavy | 0,13 s | 0,16 s | 0,27 s | 0,56 s | Cross-Ableitung, endgültige Smash-Pose offen |

AttackDefinition speichert die drei gewünschten Zeiten; ActiveStart/End werden aus ihrem Verhältnis berechnet. HumanoidCombatBuilder retimt die importierten Kurven abschnittsweise auf diese Zeiten, statt die Rohcliplänge als Gameplay-Dauer zu übernehmen. Der sichtbare Vorwärtskontakt wird am Avatar ermittelt und in Active gelegt. Lineare Kurventangenten verhindern Überschwingen an den neu eingefügten Phasengrenzen.

Animator.normalizedTime bleibt die einzige laufende Phasenquelle. Combat setzt die Geschwindigkeit aus Clipdauer/Gesamtdauer, Hitstop hält diesen Animator an. Schaden, Sweeps, Anschlüsse und Angriffsschritte verwenden dieselbe Phase. Motor/CharacterController bleiben für Bewegung verantwortlich; Root Motion bleibt aus. Ein externer State-Wechsel bricht die Attacke weiterhin ab.

Zeit-/Clip-/Rigänderungen werden über den Builder neu abgeleitet und gebacken. Individuelle Änderungen nur an erzeugten Kurven sind keine automatisch nachgezogene Kontaktbahn. Der Builder regeneriert die aktuell codierten Vorgaben; eigene Änderungen daran vor Neuaufbau im Builder übernehmen.

## Tatsächliche Kontaktbahnen

Der Builder evaluiert jede fertige Attacke mit Animator am gültigen importierten Avatar. Je Attacke speichert er 161 Hand-/Fußpunkte im lokalen Raum der Facing-Wurzel. Laufzeitabfragen interpolieren diese konkrete Modell-/Clipbahn; sie wenden weder SampleAnimation noch Play auf den sichtbaren Humanoid an.

Die vorhandene Capsule-Abfrage bleibt auf das zwischen zwei Frames tatsächlich gekreuzte Active-Intervall begrenzt. Rootbewegung fließt wie zuvor ein; Front-/Teamfilter, ein Treffer pro Ziel/Attack-Instanz und getrennte Hurtboxes bleiben erhalten. Ein ganz übersprungenes Active-Fenster wird weiterhin abgefragt. Avatar und Clipreferenz müssen zum Bake passen, andernfalls startet die Humanoid-Attacke mit einer klaren Fehlermeldung nicht.

Für B1-Transformclips bleibt der bestehende SampleAnimation-Pfad erhalten. Kein Wechsel des Robot-Gegner-Rigs in diesem Schritt.

## Prüfung und verbleibende Arbeit

Gezielte Humanoid-PlayMode-Fälle prüfen Startup/Treffer/Fehlschlag/Reset, Cliplänge gegen gewünschte Dauer, sichtbare Pose gegen Kontaktbahn für alle sieben Attacken, übersprungenes Active ohne Änderung der sichtbaren Endpose sowie Hitstop. Ein zusätzlicher Integrationsfall deckt die Drei-Schlag-Kette und realen Luftkick mit erhaltenem Sprungflug ab. Ergebnisse in `tools/b2-combat-results.json`; bestehender B1-Polish-Regressionsblock in `tools/b2-b1-regression-results.json`. Keine neue Vollsuite oder Performance-Matrix.

Dieser Schritt liefert eine spielbare technische Combat-Basis. Finisher/Heavy/Air-Heavy verwenden vorläufige Cross-Varianten; sie sind keine fertigen individuellen Move-Animationen. Nächste konkrete Arbeit: B3-Kernclips/Run und sichtbare Trefferreaktionen, dann lebendes Knockdown/GetUp und das vollständige Duell. Wombat-Identität bleibt eine separate Modell-/Stilentscheidung. Build- und Performance-Nachweis folgen am kleinen vollständigen Duell.

Abschluss: fünf Humanoid-Fälle bestanden (11,80 s), fünf vorhandene B1-Polish-Fälle bestanden (11,32 s). Tatsächliche Kamerasequenz Punch/Kick/Luftangriff unter `UnityProject/Assets/QA/b2-combat-*.png`. Diese Bildkontrolle und die technischen Fälle ersetzen kein Nutzerurteil über das Spielgefühl.
