# B3a — Bewegung, Kernaktionen und Trefferreaktionen

Stand: 4. Oktober 2026. Spielbare Lieferung im `HumanoidCombatLab`. Technischer Human und bestehende Robot-Gegner; Wombat-Look bleibt offen. B3b/B3c sind noch nicht begonnen.

## Direkt spielen

`UnityProject/Assets/Game/Scenes/HumanoidCombatLab.unity` öffnen und Play drücken. WASD/Stick bewegt; Ctrl oder linker Trigger gehalten schaltet am Boden auf Rennen. J dreimal rhythmisch für Jab → Cross → Haken, K für Overhand-Heavy, L für Kick. Im Sprung J/L für den Kick und K für den zweihändigen Smash; letzterer erreicht den stehenden Gegner besonders beim Abstieg. Shift bleibt Ausweichen. R setzt alles zurück; 1/2 schaltet Gegnerzahl.

## Konkrete Änderungen

| Bereich | Lieferung / Wirkung |
| --- | --- |
| Gehen und Rennen | Eigene Character.asset mit 3 m/s Gehen und 6,2 m/s Rennen; Run nutzt vorhandenes Sprint_Loop. Loslassen schaltet sofort zurück. Animationsgeschwindigkeit berücksichtigt tatsächlich zurückgelegte Strecke und Körperblockaden. |
| Angriff und Luft | Run umgeht keine Startup-/Active-Sperre. Freie Bewegung bleibt ab bestehender später Recovery erlaubt. In der Luft gilt weiterhin moveSpeed × airControl, ohne Sprint-Bonus. |
| Light 3 | Gebeugter rechter Arm und Rumpfdrehung als Haken-Finisher statt identischer Cross-Wiederholung. |
| Heavy | Vorhandene Sword_Attack-Bewegung ohne Waffe als Schlag von oben adaptiert und auf bestehende Heavy-Phasen retimt. |
| Air-Heavy | Eigene symmetrische Arm-/Rumpfmuskelkurven für Ausholen über Kopf und zweihändiges Niederschlagen mit Vorwärtsneigung des Körpers; angewinkelte Beine, Flugbahn bleibt beim Motor. Kontaktmoment bewusst an der niedrigen Schlagpose statt am Maximum der Vorwärtsreichweite. |
| Boden-/Sprung-Kick | Nach Nutzerfeedback als Kampfsport-Frontkick überarbeitet: Knie anziehen, gezielt strecken, zurücknehmen und Armdeckung. Im Sprung bleibt das freie Bein angewinkelt. |
| Spielerreaktion | Hit_Chest/Hit_Head als Hit/Stagger: unterbricht den laufenden Angriff und bleibt während vorhandener Treffer-Starre sichtbar. |
| Gegnerreaktion | Eigene kurze Robot-Hit-/Stagger-Clips mit Oberkörper-, Kopf- und Schwerpunktreaktion. KI überschreibt die Reaktion nicht; Hitstop friert Pose und Restdauer ein. |
| Reset / Vergleich | R räumt Reaktionen und Angriff auf. Ältere Definitions ohne runSpeed behalten die bisherige Bewegung; B1-Szenen ihren bisherigen Robot-Controller. |

Die Aktionszeiten bleiben unverändert: Finisher 0,17/0,11/0,28 s, Heavy 0,29/0,13/0,44 s, Air-Smash 0,13/0,16/0,27 s, Kick 0,18/0,13/0,34 s und Air-Kick 0,08/0,16/0,24 s für Startup/Active/Recovery. Die geänderten Kontaktbahnen sind am echten Avatar neu gebacken, mit je 161 Punkten. Animator-Zeit bleibt die einzige Angriffsphase; Runtime-Sweeps verändern die sichtbare Pose nicht.

## Quellen und Erzeugung

Keine neuen Packs oder Downloads. Run, Hit, Stagger und Overhand stammen aus dem bereits vorhandenen Quaternius-Standardarchiv mit CC0-Beleg. Haken und Air-Smash sind lokale Muskelkurven-Ableitungen, Robot-Reaktionen eigene Transformkurven. Einzelquellen stehen in [THIRD_PARTY_LICENSES.md](THIRD_PARTY_LICENSES.md).

`CharacterActionBuilder.Build()` beziehungsweise Menü **Wombat Lab/B3a Upgrade Character Actions** ergänzt die vorhandenen Assets, das Prefab und HumanoidCombatLab. `RefreshActions()` erneuert eigene Kurven und Kontaktbahnen ohne erneuten FBX-Import. Eigene Kopien liegen unter `Assets/Game/Characters/HumanoidCombat/`. Bei bewusstem Neuaufbau B2-Builder zuerst, B3a-Builder danach: der B2-Builder allein stellt die ältere Variante her. Clip-/Avatar-/Timingänderungen erfordern erneuten Kontakt-Bake.

`AnimationReaction` besitzt nur die sichtbare Reaktion innerhalb der bestehenden Treffer-Starre. HP, Treffer, Unterbrechung, Körperbewegung und Angriffssperre bleiben bei PlayerDefense/TrainingDummy/Combat. Starke Gegnerreaktion bei Heavy oder Schaden ≥ 18, starke Spielerreaktion bei Schaden ≥ 20; beide kehren danach zu bestehender Bewegung/KI zurück.

## Gezielte Nachweise

- **3/3 neue Action-PlayMode-Fälle bestanden**, 6,25 s: Walk/Run/Idle, Geschwindigkeit, Angriffssperre, Luftkontrolle, Treffer-Unterbrechung, Pose gegen Locomotion, starke Reaktion, Hitstop, KI-Rückkehr und Reset. `tools/b3a-action-results.json`.
- **5/5 vorhandene Humanoid-Combat-Fälle bestanden**, 12,88 s nach Kick-Überarbeitung: Avatar-/Kontaktgenauigkeit aller sieben Attacken, Startup/Einzeltreffer/Fehlschlag, übersprungenes Active ohne Umposen, gemeinsame Timing-/Hitstop-Uhr, Drei-Schlag-Kette und Luftkick. `tools/b3a-contact-regression-results.json`.
- **1/1 gezielter Input-Fall bestanden**, 6,13 s: gehaltenes Ctrl/LT und unverändertes Shift-Ausweichen mit simuliertem Gamepad. `tools/b3a-input-results.json`.
- **1/1 Air-Smash-Fall bestanden**, 3,96 s nach der letzten Pose-Korrektur: trifft im Abstieg genau einmal, Flugbahn bleibt während Hitstop aktiv, Landung/Reset und Fehlschlag aus Distanz. `tools/b3a-air-smash-results.json`.
- **1/1 Kontaktgenauigkeitsfall erneut bestanden**, 1,94 s nach der finalen Körperneigung; sichtbare Avatar-Pose gegen die gebackene Bahn aller sieben Attacken. `tools/b3a-final-contact-results.json`.

Keine Vollsuite, kein Performanceprofil und kein physischer Gamepad-Nachweis. Diese Prüfung belegt die Regeln; subjektives Spielgefühl und weitere Posenfeinheit werden am spielbaren Stand beurteilt.

Tatsächliche Kameraposen unter `UnityProject/Assets/QA/` angesehen: Run, Heavy, Haken, Spieler-Hit, Robot-Stagger sowie überarbeitete Boden-/Sprung-Kicks und finaler Smash. Boden- und Sprung-Kick treffen in der Sequenz jeweils einmal; der finale Smash beim Abstieg ebenfalls. Finale Bilder: `b3a-martial-kick.png`, `b3a-martial-airkick.png`, `b3a-final-smash-windup.png`, `b3a-final-smash-contact.png`. Mensch gegen großen Bären-Dummy wird nach Nutzerfeedback für die technische Weiterentwicklung beibehalten. Szene frisch geladen, Play für die Übergabe gestoppt.

## Danach

**B3b:** lebendes Knockdown, Bodenphase, GetUp mit begrenztem Schutz und vollständige Spieler-/Gegner-Tod-/Reset-Folge. **B3c:** Duell-Abstände, Combo/Luftkontakte/Recovery/Feedback zusammen tunen und kleinen Windows-/WebGL-Nachweis ausführen. Die endgültige Cartoon-Tierdarstellung bleibt separat offen.
