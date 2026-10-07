# B6 — zusammenhängendes Schrotthof-Kapitel

Stand: 7. Oktober 2026. Lokaler Kapitelstand; vorhandenes `HumanoidCombatLab` bleibt als kurzes Duell-/Rollenlabor erhalten. Öffentlicher Play-Link und bisherige Windows-/WebGL-Builds enthalten weiterhin die frühere Touch-Fassung. Das Kapitel benötigt einen eigenen neuen Build-/Veröffentlichungsschritt.

## Spielen

`UnityProject/Assets/Game/Scenes/JunkyardChapter.unity` in Unity öffnen und Play drücken. Die bisherigen Kampfaktionen bleiben unverändert. Nach rechts zur ersten gelben Fläche laufen; während eines Kampfes schließen die Bereichstore. Alle Wellen besiegen, danach weiter nach rechts. Nach dem ersten Bereich am gelben Schalter das Verbindungstor öffnen.

- **F / Gamepad LB / Touch TOR ÖFFNEN:** den nahen Torschalter bedienen; erst nach dem ersten Bereich möglich.
- **R / Gamepad Start / Touch CHECKPOINT:** aktuellen Checkpoint wiederholen, auch nach Niederlage.
- **Backspace / Gamepad Select / Touch VON VORN:** gesamtes Kapitel von vorne.
- WASD/Stick, Ctrl/äußerer Stick fürs Rennen, Space/South, J/West, K/North, L/RB, E/RT und Shift/East bleiben die vorhandenen Bewegungs-/Kampfaktionen.
- 1–4/D-Pad oben und Touch-Gegnerwahl gehören weiterhin zum Rollenlabor; im Kapitel bestimmt die Wellenfolge die Gegner.

## Strecke und Steigerung

| Bereich | Drei Wellen | Wirkung |
| --- | --- | --- |
| Anlieferung, Mitte x=0 | 1 Standard → 2 Standard → 2 Standard | Einstieg mit kurzer lesbarer Nahkampfaktion |
| Sortierhof, Mitte x=24 | Standard/Agile → Agile/2 Standard → Heavy/Agile/Standard | Ansturmspur und unterschiedliche Prioritäten |
| Presswerk, Mitte x=48 | Heavy/2 Standard → Heavy/Agile/2 Standard → Vorarbeiter/Agile/Standard | Höherer Gruppendruck und sichtbarer Abschlussgegner |

Neun Wellen mit insgesamt 23 Gegnern, maximal vier gleichzeitig. Wellenpause 2,2 s; nach jedem gewonnenen Bereich 30 HP Heilung, höchstens 100. Der Vorarbeiter verwendet dieselbe Heavy-Technik mit 100 HP, 20 Schaden, 1,9 m/s und 0,70 s Warnung. Kein neuer mehrphasiger Boss und keine zusätzliche Rüstung.

Die 62 m lange begehbare Spur verbindet drei jeweils etwa 14 m breite Kampfbereiche durch kurze Laufabschnitte. Sortierracks, Presse, Bereichsschilder, Torpfosten, Markierungen und Checkpointfläche geben Orientierung. Vorhandene Kenney-/Poly-Haven-/Junkyard-Grundlagen werden wiederverwendet; keine neue externe Asset-Suche oder neue Rig-/Animationspipeline.

Diese erste Fassung ist ein **Kurzkapitel**: Der automatisierte Kampf-Durchlauf mit kontinuierlichen Combo-Eingaben gewann alle neun Wellen in 101,7 s mit 58 HP. Das ist kein menschlicher Erstspieler-Zeitwert. Das ursprüngliche 10–15-Minuten-Ziel ist noch nicht erreicht; dafür müssen anhand von Spielerfeedback zusätzliche abwechslungsreiche Situationen und Übergänge entstehen. Bloß mehr Gegner-HP wären kein sinnvoller Ersatz. Diese Fassung liefert die durchspielbare Strecke und ihre Regeln.

## Checkpoints und Kampfregeln

Beim Bereichseintritt wird ein Checkpoint mit Einstiegspunkt und den damaligen HP gespeichert. Retry setzt diesen Bereich auf seine erste Welle zurück. Bereits abgeschlossene Bereiche bleiben erledigt. Nach einem Sieg wird zusätzlich der sichere Verbindungsabschnitt gespeichert; das geöffnete Schaltertor bleibt bei Retry offen. Vollständiger Neustart schließt das Tor und setzt Fortschritt und HP zurück.

Retry entfernt die alte Gruppe, räumt Token, Puffer, Hitstop, Knockdown/GetUp/Tod und Rückstoß auf und erzeugt frische Gegner an den aktuellen Spawnpunkten. Keine Rückkehr zu alten Laborpositionen. Tod zeigt den Checkpoint-/Neustarthinweis. Nach der letzten Welle erscheint der Kapitelabschluss.

Kampfgruppen verwenden weiterhin `EnemyBrain`, `TrainingDummy`, `BodyRecovery` und die gemeinsame Angriffsfreigabe. `EngagementCoordinator` und Schadensempfänger erhalten lokale Bereichsgrenzen statt fest verdrahteter Laborgrenzen. Die Kamera folgt auf den Wegen nach rechts und rahmt im Kampf den aktuellen Bereich ein. Der Spieler erhält zur Laufzeit eine eigene Kopie seiner Bewegungsdefinition; das gemeinsame Humanoid-Asset bleibt unverändert.

## Bearbeiten und Nachweise

`ChapterDefinition` unter `Assets/Game/Chapters/Junkyard/JunkyardChapter.asset` enthält Bereichsnamen, Mittelpunkte, Rollen/Spawnpunkte, Wellenpause und Bereichsheilung. `JunkyardChapter` führt den kleinen Ablauf; `ChapterGate` schaltet die tatsächliche Torplatte/Kollision. `Wombat Lab/B6 Build Junkyard Chapter` baut die eigene Szene aus dem gespeicherten B5-Labor und bestehenden Assets neu auf. Einen solchen Neuaufbau bewusst ausführen: Er ersetzt die Kapitel-Szene, keine manuell erhaltene Änderung darin.

**11/11 gezielte PlayMode-Fälle bestanden:** drei neue Kapitel-Fälle (85,01 s), vier gemeinsame Eingabefälle (1,21 s), drei bisherige Duellfälle (17,08 s) und ein Vier-Gegner-/Token-Fall (16,40 s). Die Kapitelprüfungen decken alle Wellen/Wege, Tor/Neustart, Checkpoint nach Tod samt HP/Fortschritt und einen tatsächlich gekämpften Standard-/Agile-Kontaktblock im zweiten Bereich ab. Die Ablaufprüfung nutzt den Schadensempfänger; sie ist keine vollständige Kampf-Simulation. Keine Vollsuite.

Zusätzlich ein zusammenhängender tatsächlicher Kampf mit unveränderten HP, Schaden und KI: normale Bewegungs-/Combo-Befehle an Motor/Combat, keine Teleports und keine direkten Schadensaufrufe. Drei Bereiche abgeschlossen, Tor regulär geöffnet, alle 23 Gegner besiegt. Bericht: [tools/b6-combat-run.txt](tools/b6-combat-run.txt). Spielkamerabilder für Einstieg, Tor, Sortierhof, Presswerk/Vorarbeiter und Abschluss angesehen; Touch-HUD bei 1280×600 einschließlich Checkpoint, Von vorn und Abschluss kontrolliert. Die Canvas-Aufnahme verwendet vorübergehend die Spielkamera und stellt ihre Einstellungen wieder her. Details: [tools/b6-visual-review.md](tools/b6-visual-review.md). Kein neuer Build, keine physische Android-/Gamepad- oder Performanceabnahme.

B7 ist die nächste Ausbauphase für Cartoon-Materialien, Welt-/Kamerapräsentation und Audio; Benutzerführung/Menü folgt in B8. Kapitelumfang/Balance bleiben bis zum 10–15-Minuten-Ziel weiter abzustimmen.
