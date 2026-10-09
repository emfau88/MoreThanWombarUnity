# B12 — veröffentlichte LF2-Stage

Stand: 9. Oktober 2026. **[Stage spielen](https://emfau88.github.io/MoreThanWombarUnity/)**. Auf Android im Querformat öffnen und SPIELEN wählen; Bewegung links, Kampfaktionen rechts. TOUCH blendet die Bildschirmsteuerung bei Bedarf ein.

## Ausgelieferter Stand

- Spielcommit: `25b293fda4aa21b18cca93a10bfcfb79ae078cae` auf `main`.
- [Release chapter-b12-2026-10-09](https://github.com/emfau88/MoreThanWombarUnity/releases/tag/chapter-b12-2026-10-09).
- [Erfolgreiche Pages-Bereitstellung](https://github.com/emfau88/MoreThanWombarUnity/actions/runs/37958953239).
- Szene: `Assets/Game/Scenes/JunkyardChapter.unity`, Build: B12. Öffentliche `version.json` bestätigt dieselbe Revision und denselben Release: [gespeicherter Nachweis](tools/b12-public-version.json).
- B9–B12 enthalten: Boden-AOE, MP, Durchbruch, Druckwelle; Raufbold/Werfer/Schläger; fünf Begegnungen mit genau 30 Gegnern, angekündigte endliche Verstärkungen, Checkpoints und Finale. Anlieferung enthält nur leichte Raufbolde, gemischte Rollen erscheinen ab Sortierhof.

Der endgültige Build der gepushten Revision verwendet den unveränderten geprüften Buildcache: **Succeeded, 6,53 s, 19.853.081 Bytes, null Fehler/eine Warnung**. Der vorherige vollständige Kandidatenbuild nach den Menü-/Touchkorrekturen benötigte 256,28 s. Die Warnung betrifft die im Player bewusst deaktivierte Editor-Pipeline-Steuerung. [Release-Buildbericht](tools/b12-webgl-build.json). Die anschließende Dokumentationsaktualisierung verändert die gebauten Spieldateien nicht.

Der vorhandene Release-/Pages-Workflow bleibt erhalten: geteiltes ZIP, Pages-Deployment und mit dem Spielcommit versionierte Asset-Adressen. Der Publisher prüft Szene und Revision; keine Unity-Lizenz oder Kompilierung im GitHub-Runner erforderlich.

## Öffentlicher Browsernachweis

Im tatsächlichen öffentlichen Player bei 915×412 Browser-Viewport geprüft: Startmenü nennt 30 Gegner; SPIELEN öffnet die Stage; Touch-Tasten einschließlich WELLE sind erreichbar. Ein Klick auf WELLE reduziert MP von 100 auf 74. D bewegt den Spieler in die Anlieferung; zweizeiliger HUD-Hinweis und orange Zugangswarnung erscheinen. PAUSE zeigt Bereich und Fortschritt von 30, STARTMENÜ kehrt zurück. Keine Browser-Konsolenfehler im Check beobachtet. [Kurzbericht](tools/b12-public-browser-results.json).

Das ist ein PC-Browsercheck in einer Handy-Querformatgröße, kein physischer Android-Test und kein Nachweis echter Mehrfingerbedienung oder Handy-Leistung. Die bestehende Touch-Steuerung ist online verfügbar; dieser Gerätecheck bleibt mit dem Nutzer offen. Die vollständige technische Stage-/Retry-Prüfung und die reale Kampfrunde sind getrennt in [B12_LF2_STAGE.md](B12_LF2_STAGE.md) dokumentiert.

## Weiter

B13: deutlichere Rollen-/Figurenunterscheidung auf der vorhandenen Grundlage, Animationen, Map-Verdeckungen, Effekte und Audio. B14: menschliche Balance und tatsächliches Android-Gerätebudget. B15: abschließende Auslieferung einschließlich Windows-Kandidat. Die vorgezogene B12-WebGL-Veröffentlichung ersetzt diese Qualitätsarbeit nicht.
