# B8 — öffentliches Schrotthof-Kapitel

Stand: 9. Oktober 2026. **[Kapitel spielen](https://emfau88.github.io/MoreThanWombarUnity/)**, Android im Querformat. Im Hauptmenü SPIELEN wählen; das Kapitel enthält Anlieferung, Sortierhof und Presswerk, neun Wellen, Schaltertor/Checkpoints, Vorarbeiter-Finale sowie B7-Präsentation und B8-Menüs.

## Ausgelieferter Stand

- Spielcommit: `1869b0601b5c405c9a3c249c2c3a2defb21da12b`.
- Szene: `Assets/Game/Scenes/JunkyardChapter.unity`; Ausgabe `Builds/B8/WebGL`.
- [Release chapter-b8-2026-10-09](https://github.com/emfau88/MoreThanWombarUnity/releases/tag/chapter-b8-2026-10-09).
- [Erfolgreicher Pages-Lauf](https://github.com/emfau88/MoreThanWombarUnity/actions/runs/37926174430), Workflowcommit `82fb760`.
- Öffentliche `version.json` bestätigt Spielcommit, Release, B8 und Kapitel-Szene. Die spätere Workflow-/Dokumentationsänderung verändert den gebauten Spielcode nicht.

WebGL: **Succeeded**, **19.818.574 Bytes**, **367,52 s**, **0 Fehler, 2 Warnungen**; `tools/b8-webgl-build.json`. Warnungen: Runtime-Pipeline ohne Config ist im Player deaktiviert; Unity meldet unkompilierte Änderungen für Import-/Postprocessing-Code. Vor und nach dem Build war `scriptCompilationFailed` false; der veröffentlichte Player lädt die aktuellen Kapitel-/Menüfunktionen. Nicht als warnungsfreien Build ausgeben.

Das nachweislich ungenutzte Inference-Paket samt transitivem App-UI wurde entfernt. Gzip mit Unity-Dekompressionsfallback reduziert den Download gegenüber dem alten 66,4-MB-Duell auf etwa 19,8 MB. Beim ersten Laden traf ein alter gecachter Loader auf die neuen komprimierten Dateien. Der Workflow versieht jetzt alle vier Build-Asset-Adressen mit dem Spielcommit; derselbe Build wurde ohne erneute Unity-Kompilierung erfolgreich veröffentlicht. SHA-256 aller vier Release-Teile entspricht den lokalen Dateien (`tools/b8-release-parts.json`).

## Kurzer tatsächlicher Browsernachweis

Am öffentlichen Link: Hauptmenü → SPIELEN → Einführung → ÜBERSPRINGEN → Anlieferung; D bewegt den Spieler ins Kampffeld und startet Welle 1 mit Standard-Bär. Touch-Controls inklusive STOSS sind sichtbar. PAUSE → Optionen → Zurück → Weiterkämpfen → P → Startmenü funktioniert. Seit dem korrigierten Laden keine Konsolenfehler; Warnungen betreffen den vorhandenen URP-FSR-Pass und Unitys alte Persistenz-Synchronisierung. Rohbericht: `tools/b8-public-browser-results.json`. Bilder lokal: `UnityProject/Assets/QA/b8-public-home.png`, `b8-public-pause.png`.

Der lokale Python-Servercheck scheiterte am nicht antwortenden Server; die endgültige Kontrolle erfolgte direkt auf GitHub Pages. Beide gestarteten Serversitzungen wurden beendet. Unity bleibt gespeichert/idle, Buildqueue leer und Kompilierung erfolgreich. Automatisch synchronisierte Legacy-Materialfelder wurden zurückgestellt.

Das ist ein kurzer Veröffentlichungstest. Die bestehenden B6/B8-Ablauf-/Combat-Nachweise bleiben erhalten; keine weitere vollständige Testsuite oder öffentlicher Neun-Wellen-Durchlauf. Physisches Android/Gamepad, hörbare Audioabnahme, Framezeiten/Speicher, neuer Windows-Kapitelbuild und Ausbau zum 10–15-Minuten-Ziel bleiben B9/offen.

## Nächstes Update

Spiel-/Publishing-Änderungen committen und pushen, dann `DuelSliceBuilder.Start("WebGL", true, "<Commit-SHA>")` über die Unity CLI ausführen. Nach erfolgreichem Build kurz prüfen und `tools/Publish-Duel.ps1 -Build B8 -Tag <neuer-tag>` starten. Der Publisher prüft Szene und Spielcommit; der Pages-Workflow ergänzt versionierte Asset-Adressen. Lokal: `tools/Serve-Duel.ps1 -Build B8`, für Android im selben WLAN zusätzlich `-Lan`.
