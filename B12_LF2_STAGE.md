# B12 — zusammenhängende 30-Gegner-Stage

Stand: 9. Oktober 2026. Ausbau der vorhandenen `JunkyardChapter`-Szene; keine neue Map und kein neues KI-System. B9–B11-Kampfregeln, Menschenfigur, drei Bärenrollen und bestehende B7/B8-Präsentation/Sitzungsoberfläche werden weiterverwendet. Anschließender Commit, Push und Pages-Release sind ausdrücklich beauftragt; der Veröffentlichungsschritt folgt auf den lokalen Abschluss.

## Ablauf

| Bereich / Begegnung | Raufbold | Werfer | Schwer | Auslösung |
| --- | ---: | ---: | ---: | --- |
| Anlieferung: Einstieg | 5 | 0 | 0 | Eintritt in die Kampffläche |
| Anlieferung: Vorstoß zum Tor | 3 | 0 | 0 | Vorrücken über die Bereichsmitte |
| Sortierhof: Hauptkampf | 5 | 2 | 1 | Bereichseintritt |
| Sortierhof: Durchgang | 4 | 1 | 1 | Vorrücken über die Bereichsmitte |
| Presswerk: Finale | 4 | 2 | 2 | Bereichseintritt; einer der schweren Gegner ist der Vorarbeiter |
| **Summe** | **21** | **5** | **4** | **30 Gegner in fünf Begegnungen** |

Restgegner bleiben beim nächsten Begegnungstrigger erhalten. Neue Gegner warten in einem endlichen Vorrat, solange acht aktive Gegner oder zwei aktive Werfer erreicht sind. Zurücklaufen erzeugt keine weiteren Gegner. Nur der jeweilige Bereichsabschluss verlangt, alle vorgesehenen Begegnungen auszulösen und alle Gegner zu besiegen. Es gibt keine festen Drei-Wellen-Folgen und keine künstliche Zwischenwellenpause mehr.

Vier markierte Zugänge je Bereich verteilen den Nachschub. Ein Bodenwinkel und der HUD-Hinweis kündigen den gewählten Zugang 1,1 Sekunden vorher an. Vor der Erzeugung werden freier Raum und mindestens 2,8 Meter Abstand zum Spieler erneut geprüft; ein besetzter Zugang wird verworfen und ein freier gesucht. Ein einzelner blockierter Werfer hält zulässige Nahkämpfer im Vorrat nicht auf. Vorhandene Kampfwege bleiben frei; neue Bodenmarkierungen und Schilder haben keine Collider. Kamera etwas höher für den Gruppenkampf.

Der HUD zeigt Bereich, Begegnung, aktive Gegner und besiegte Gegner von 30. „Vorstoßen“ weist auf die nächste noch nicht aktivierte Begegnung hin. Sieg gibt es erst nach dem vollständigen Finale, nicht schon nach dem Tod des Vorarbeiters.

## Checkpoints und Erholung

Checkpoint bei Bereichseintritt und nach dem Bereichsabschluss. Retry beginnt den aktuellen Bereich neu: erste Begegnung wieder ausstehend, spätere Verstärkung noch nicht ausgelöst, keine alten Gegner oder Projektile. Abgeschlossene Bereiche und das geöffnete Schaltertor bleiben erhalten. HP und MP werden auf die gespeicherten Werte zurückgesetzt. Der Zähler zählt den aktuellen erfolgreichen Durchlauf, nicht wiederholte Abschüsse vor einer Niederlage.

Bereichsabschluss stellt bis zum jeweiligen Maximum 30 HP und 30 MP wieder her. MP regeneriert weiterhin nach den B10-Regeln. Pause hält auch die Verstärkungswarnung und Spawnzeit an. Vollständiger Neustart setzt Gegnerplan, Tor und Checkpoint zurück.

## Technik und Bedienung

`ChapterWave` bleibt der vorhandene serialisierte Datenträger und erhält Begegnungsname/Positionsauslöser. `JunkyardChapter` verwaltet den kleinen Vorrat und den bereits aktivierten Begegnungsindex. `EngagementCoordinator.AppendEnemy` ergänzt Gegner, ohne Restgegner, Angriffserlaubnisse oder laufende Projektile zurückzusetzen. Kein vorsorglicher Pool oder zusätzlicher Encounter-Manager.

`Lf2StageBuilder.Apply()` aktualisiert gezielt die bestehende Szene und Kapiteldefinition; es ersetzt nicht die gestaltete Umgebung. Das ältere B6-Neuaufbauskript bleibt historische Grundlage. Nach absichtlichem Neuaufbau des Kapitels B7/B8/B9–B11 und zuletzt B12 anwenden.

Im Editor `Assets/Game/Scenes/JunkyardChapter.unity` öffnen und Play → SPIELEN. Bewegung WASD, Grundkampf J/K/L, Boden-Smash Sprung + K, Durchbruch E, Druckwelle Q, Ausweichen SHIFT, Schalter F. R wiederholt den Checkpoint, BACKSPACE die Stage. Touch- und Gamepad-Bedienung bleiben angebunden.

Separater lokaler Buildpfad: Menü **Wombat Lab/B12 Build Stage WebGL**, Ausgabe `Builds/B12/WebGL`. Server: `tools/Serve-Duel.ps1 -Build B12 -Port 8767`. Keine Änderung am öffentlichen B8-Spielstand.

## Nachweise

**2/2 gezielte Kapiteltests bestanden in 79,65 s:** zusammenhängender Weg, endliche Gegnerzahl, Überlappung/Begrenzung der Verstärkungen, Finale und Wiederholung einschließlich HP/MP, Tor, Spawnwarnung und Pause. [Rohbericht](tools/b12-stage-results.json). Sie verwenden für Ablaufprüfungen direkten Schaden und stellen stillgelegte Gegner im Innenfeld auf, damit diese nicht künstlich die Spawnzugänge besetzen. Dies ist kein Nachweis eines menschlichen Kampfsiegs.

**Tatsächlicher automatischer Kampf:** normale Bewegungs-/Angriffseingaben, unveränderte HP, Schaden und KI, keine Teleports oder direkten Schadensaufrufe. Anlieferung gewonnen und Tor geöffnet; Niederlage im Sortierhof nach 61,5 Sekunden und 19 besiegten Gegnern. [Bericht](tools/b12-combat-run.txt). Das prüft die integrierten Kontakte und das Vorrücken, bestätigt aber keinen vollständigen Combat-Sieg oder menschliche Schwierigkeit. Ein abgeschnittener zweiter HUD-Hinweis wurde nach der Sichtprüfung korrigiert.

Der erste Browsercheck bestätigte Startmenü, überspringbare Einführung und tastaturgesteuerten Kampfeinstieg mit angekündigtem Nachschub. Dabei wurden noch der historische Menütext „9 Wellen“ und die größere Touch-Schrift korrigiert. Die abschließenden Editor-Spielbilder zeigen nun „30 Gegner“ im Menü und beide Zeilen des Kampf-HUD mit acht aktiven Gegnern. Lokale Aufnahmen: `UnityProject/Assets/QA/b12-final-menu.png`, `b12-final-touch.png`.

Lokaler WebGL-Kandidat nach diesen Korrekturen: **Succeeded, 256,28 s, 19.853.081 Bytes, null Fehler/eine Warnung**. Die Warnung betrifft die absichtlich nicht konfigurierte Pipeline-Steuerung im Player; die Unity-CLI bleibt ein Editor-Werkzeug. [Buildbericht](tools/b12-webgl-build.json). Der Publisher unterstützt B12 und verlangt wie beim bisherigen Kapitelrelease einen Build der veröffentlichten Git-Revision.

## Nächster Bulk

B13: gemeinsame Figuren-/Map-/Audio-/Effektpolitur. B14: menschliches Spielgefühl, Dichte und Ressourcen sowie tatsächlicher Android-Check. B15: abschließender Release-Kandidat und beauftragte Veröffentlichung. B12 führt weder einen Begleiter noch einen zweiten spielbaren Charakter ein.

Konkreter B13-Sichtbefund: Am rechten Kampffeldrand können Torpfosten und dicht stehende Bären den Menschen verdecken. Dort Kamerabild, Toransicht und Spielermarkierung gezielt verbessern; die Kampffläche selbst ist durchgehbar. B14 soll besonders den Druck der zweiten Sortierhof-Begegnung beurteilen, an der der einfache Kampf-Bot besiegt wurde.
