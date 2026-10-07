# B4 — Schulterstoß und klare Move-Rollen

Stand: 7. Oktober 2026. Lokale Erweiterung von HumanoidCombatLab. Der bisher veröffentlichte WebGL-Spielstand und die B3c-Builds enthalten B4 noch nicht.

## Spielen

E / rechter Gamepad-Trigger / Touch **STOSS** startet einen geraden Schulterstoß am Boden. Bewegungsrichtung beim Start wählen; ohne Richtungseingabe gilt die Blickrichtung. Danach bleibt die Richtung fest. Kein Luftstoß, kein Anschluss aus einer anderen Attacke. Der Stoß endet am ersten gültigen Gegnerkontakt, einer Körper-/Wandkollision oder am Arenarand. Seine Erholung bleibt bestehen. Treffer können ihn unterbrechen; er gibt keinen Unverwundbarkeitsschutz und lässt sich nicht durch Sprung, Bewegung oder Ausweichen abbrechen.

| Move | Rolle | Konkreter B4-Stand |
| --- | --- | --- |
| Schulterstoß | Distanz schließen | Maximal 2 m, 16 Schaden, kurzer leichter Stagger, kein Knockdown |
| Light-Kette | Nahkampfdruck | Bewährte Jab/Cross/Haken-Kette beibehalten |
| Kick | Platz schaffen | Rückstoßwert von 0,65 auf 1,05 erhöht; 18 Schaden unverändert |
| Heavy / Air-Smash | Niederwerfen | Vorhandene Zeiten, Schaden und BodyRecovery beibehalten |

Schulterstoß: Startup 0,16 s, Active 0,20 s, Recovery 0,40 s; insgesamt 0,76 s ohne Hitstop. Hitstop 0,065 s, Radius 0,36 m, Hitstun 0,28 s, Rückstoßwert 0,35. Rückstoßwerte steuern einen gedämpften Geschwindigkeitsimpuls, keine garantierte Wegstrecke.

## Vorhandene Basis verwenden

Der importierte Sprint liefert die Beinbewegung; nur Schulter, Oberkörper und Armdeckung wurden angepasst. Das bestehende Retiming und der Avatar-Bake erzeugen den Schulterkontakt am RightUpperArm-Gelenk. Keine neue Animationsbibliothek und keine zweite Kampf-/Phasenuhr. Root Motion bleibt aus.

CombatController verteilt den Stoßweg innerhalb des tatsächlich gekreuzten Active-Fensters auf kurze räumliche Schritte über PlayerMotor/CharacterController. Bestehende Hurtboxes, Front-/Teamfilter, Trefferfeedback und Unterbrechung gelten weiter. Auch ein Frame, der das ganze Active-Fenster überspringt, prüft entlang des Weges und stoppt am ersten gültigen Kontakt. Der normale Angriffsschritt wird dabei nicht zusätzlich angewendet.

Touch **STOSS** speist denselben rechten Trigger wie das Gamepad. Die Unity-Bildschirmcontrols und die vorhandene Pointer-Freigabe bleiben erhalten.

## Integration und Übergabe

Gespeicherte Szene: `UnityProject/Assets/Game/Scenes/HumanoidCombatLab.unity`. Bei bestehender Szene genügt **Wombat Lab → B4 Apply Shoulder Charge**. Vollständiger Neuaufbau: HumanoidCombatBuilder → CharacterActionBuilder → BodyRecoveryBuilder → JunkyardPreviewBuilder → DuelSliceBuilder.Apply → MobileTouchBuilder.Apply → **ShoulderChargeBuilder.Apply**. Die letzte Stufe stellt Stoß und Kick-Abstimmung her; frühere Builder überschreiben sonst Teile dieser Werte.

Gezielt bestanden: **4/4 Stoß-Fälle** (11,92 s), **4/4 Eingabefälle** (0,36 s), **3/3 Touch-Fälle** (0,70 s), **3/3 bestehende Duell-Fälle** (15,40 s). Resultate: `tools/b4-charge-results.json`, `b4-input-results.json`, `b4-touch-results.json`, `b4-duel-regression-results.json`. Kein Gesamt-Testlauf. Geprüft sind Startup ohne Schaden/Bewegung, genau ein Kontakt samt Stopp, feste Richtung und 2-m-Fehlschlag, verwundbare gesperrte Erholung, Wandstopp, Gegentreffer-Unterbrechung, Luftsperre und ein übersprungenes Active-Fenster ohne Veränderung der sichtbaren lokalen Schulterpose. Die bestehenden Duell-Fälle bestätigen Combo-Reichweite, GetUp-Schutz und Sieg/Reset.

Tatsächliche Spielkamerasequenz mit Ausholen, Schulterkontakt samt Effekt und Fehlschlag-Erholung angesehen; lokale Bilder unter `UnityProject/Assets/QA/b4-charge-*.png`. Touch-Canvas samt zusätzlichem STOSS unter `b4-touch-layout.png` angesehen; Buttons überlappen nicht. Dabei den bestehenden Zeilenumbruch in SPRUNG durch kleinere Schrift behoben und BEWEGEN am unteren Rand nach innen verschoben. Die Kamerabilder sind reguläre Gameplay-Phasen, keine manuell umgestellten Beweisposen.

Nächster Entwicklungsbulk ist B5: lesbare Gegnerrollen und gemischte Gruppen. Wombat-Gestaltung bleibt separat offen. B4 ist lokal und noch nicht committed, gepusht oder als neuer Player veröffentlicht. Die Smartphone-Spielgefühl-Abnahme folgt am neuen Build.

Abschluss: HumanoidCombatLab geöffnet und gespeichert, Szene clean, Play gestoppt, Script-Kompilierung ohne Fehler. Die abschließende reine SPRUNG-Schriftkorrektur kompiliert; gemessene Textbreite 71 bei 80 verfügbaren UI-Einheiten, ohne erneute Combat-Suite.
