# B3c — abgestimmtes Duell und Build-Übergabe

Stand: 5. Oktober 2026. Technisches Duell mit temporärem Menschen und Bären-Gegner; endgültiger Wombat-Look bleibt offen.

## Änderungen

- Der Bär begann seine Warnung bisher bei 1,65 m. Ein tatsächlicher Anlauf ergab 1,643 m Abstand und nur einen Treffer der Dreierkette. In HumanoidCombatLab beginnt die Warnung jetzt bei 1,30 m; beim Repositionieren hält er 1,20 m. Derselbe Check ergibt 1,288 m und drei Treffer: 80 → 37 Gegner-HP. Der Haken schafft anschließend wieder Abstand (1,363 m). Die bestehenden Kontaktbahnen, Radien, Schritte und Animationszeiten bleiben erhalten.
- Neue Angriffserlaubnisse warten auf das Ende des Spieler-Aufstehschutzes. Bestehende Warnrichtung, Ausweichen und Angriffspausen bleiben angebunden. Die KI darf beim Aufstehen herankommen, kündigt aber noch keinen neuen Schlag an.
- Leichte Kontaktblitze dauern 0,085 s bei Stärke 0,23; starke 0,15 s bei 0,36. Leichte Sounds spielen mit 78 % des Heavy-Pegels. Vorhandene Kontaktlinien und selbst synthetisierte Sounds werden wiederverwendet; keine neue Effekt-/Audiopipeline.
- Das Duell-HUD zeigt deutsche Hinweise, Spieler-HP, Bären-HP beziehungsweise verbleibende Gegner, Warnung, Aufstehschutz, Sieg/Niederlage und Neustart. H zeigt weiterhin technische Zustände. Die älteren Labs behalten ihre bisherige HUD-Darstellung.

Kick schafft Abstand und Stagger; Heavy ist langsamer und wirft nieder. Air-Kick beim Anflug und Air-Smash im Abstieg nutzen weiterhin die bereits abgestimmten Clips und bewahren die Flugbahn während Hitstop. Ein bloßes Ändern dieser Animationen ohne konkreten Befund ist nicht Teil dieses Bulks.

## Gezielter Nachweis

Die beiden ursprünglichen Probleme sind vor der Korrektur reproduziert (`tools/b3c-before-results.json`). Danach bestehen **3/3 Duell-Fälle**, 20,43 s: Dreierkette aus tatsächlichem KI-Abstand; Aufstehen/Schutzende/erneute Warnung; echter Ausweich-Fehlschlag sowie Sieg und Reset gegen aktive KI (`tools/b3c-duel-results.json`). Eingaben im Test werden über die vorhandenen Testeingabe-/Queue-Schnittstellen gegeben; das ist kein physischer Tastatur- oder Controller-Nachweis.

Der vorhandene Kontakt-/Luftblock besteht ebenfalls: **5/5 HumanoidCombatPlayTests**, 13,34 s (`tools/b3c-contact-results.json`). Geprüft sind unter anderem Animator-/Hitstop-Zeit, fortgesetzte Luftflugbahn, sieben gebackene Kontaktbahnen, genau ein Treffer pro Instanz und übersprungenes Active. Insgesamt acht gezielte bestandene Fälle; keine neue Vollsuite.

Tatsächlicher Browserkampf über fokussierte Canvas und Tastatur: R setzt zurück, D bewegt/richtet aus, J führt die Combo aus. Eine zusammenhängende Sequenz besiegt die aktive KI bei 100 Spieler-HP; die Sieganzeige und gefallene Gegnerfigur sind sichtbar. R nach dem Sieg stellt 100/80 HP und die Startpositionen wieder her. Spielbild: `UnityProject/Assets/QA/b3c-browser-victory.png`. Der Browser verwendet den tatsächlich erzeugten WebGL-Spielstand, keine Testeingabe-Schnittstelle.

Windows startet und rendert Map, KI-Angriffe, Knockdown und Niederlage. Sein physischer Tastaturcheck bleibt offen: Das UI-Werkzeug meldet `foreground window did not report a process id`, auch nach einmaliger Wiederherstellung. Nutzerantwort: noch nicht getestet. Der [Computer-Use-Skill](C:/Users/madde/.codex/plugins/cache/openai-bundled/computer-use/26.930.31730/skills/computer-use/SKILL.md) verweist verbindlich auf die Recovery-Anweisung: „Refresh the app/window selection and retry once; report the exact error if recovery fails.“ Deshalb keine weitere native Eingabeprüfung über dieses Werkzeug. Keine hörbare Audioabnahme oder physische Gamepad-Prüfung behaupten.

## Build- und Laufzeitbefund

Windows-Releasebuild: **137.047.172 Bytes**, 462,15 s, 0 Fehler, 486 Warnungen (`tools/b3c-windows-build.json`). Erstes WebGL-Release: **69.043.188 Bytes** unkomprimiert, 1.117,49 s, 0 Fehler, 372 Warnungen. Die vielen Warnungen stammen vor allem aus bestehenden Inference-/Sentis-Shadern; das ist kein warnungsfreier Build. Der nächste Build sollte die bestätigte Nichtnutzung dieses Pakets bereinigen, bevor Plattformwechsel/Release-Neukompilierung erneut viel Zeit kosten.

Der Browser meldet einen nicht unterstützten FSR-Shader. Die lokale URP-Quelle erklärt den Befund: `Runtime/PostProcess.cs` erzeugt den FSR-Pass immer beim Start; dessen Konstruktor ruft `PostProcessUtils.LoadShader` auf und warnt bereits bei `shader.isSupported == false`. Aus dieser Meldung folgt also kein Ausfall aller Postprocessing-Effekte. `UniversalRenderPipeline.ResolveUpscalingFilterSelection` wählt bei Auto nur Point oder Linear, nicht FSR. Die erste Annahme „Auto aktiviert FSR“ war falsch. `Mobile_RPAsset` verwendet jetzt ausdrücklich Linear/Bilinear für verlässliche Auswahl auch bei späteren Auflösungsänderungen; PC-Profil und Spielregeln bleiben gleich. Die FSR-Initialisierungswarnung bleibt dokumentiert; keine Änderung an PackageCache/URP-Code.

Abschließender WebGL-Bericht: **Succeeded**, **69.043.166 Bytes**, 72,98 s mit Buildcache, 371 Warnungen und 1 erfasstem Fehler. Dieser Fehler lautet ausschließlich `Failed to handle /api/exec request: Main thread operation timed out after 5000ms`: Der CLI-Aufruf wartet auf den mehr als fünf Sekunden langen Build, Unity baut dennoch erfolgreich zu Ende. Nicht als 0-Fehler-Bericht ausgeben und deshalb keinen weiteren kompletten Build wiederholen. Der aktuelle Bericht liegt in `tools/b3c-webgl-build.json`. Auch dieser endgültige Spielstand wurde per Tastatur bis zum Sieg und anschließendem R-Reset gespielt (`Assets/QA/b3c-browser-final.png`, `b3c-browser-final-reset.png`); Beobachtungen in `tools/b3c-browser-results.json`. Keine Browser-Fehlerlogs bei dieser Kontrolle; verbleibende Warnungen: AppUI-Defaultsettings und FSR-Initialisierung. Alte, vom ersten Buildnamen übrig gebliebene Dateien sind entfernt; das tatsächliche WebGL-Verzeichnis umfasst ebenfalls 69.043.166 Bytes.

Grobe native Prozessmessung beim tatsächlichen Lauf: etwa **694 MB Working Set**, Spitze etwa **702 MB**, gleichzeitig mit Unity-Buildarbeit. Das ist kein stabiler Speicherbudget-Nachweis. FPS/Framezeit, WebGL-Speicher und hörbare Audioqualität wurden noch nicht gemessen. Vor Contentwachstum kurz am laufenden Duell messen, dann konkrete Budgets setzen; keine Geräte-Vollmatrix. Finale Editorübergabe: HumanoidCombatLab gespeichert/clean, Play und Build gestoppt, keine fehlgeschlagene Script-Kompilierung, Queue leer. Produktname Combat Lab, FullScreenWindow und Brotli sind zurückgestellt; aktive Editorplattform bleibt WebGL, um keinen weiteren teuren Reimport auszulösen.

## Builds erstellen und spielen

Vorhandene Unity-Module: Windows x64 und WebGL, beide über BuildPipeline als unterstützt bestätigt. Keine zusätzliche Installation.

- Unity-Menü **Wombat Lab → B3c Build Windows**: `Builds/B3c/Windows/MoreThanWombat.exe`.
- Unity-Menü **Wombat Lab → B3c Build WebGL**: `Builds/B3c/WebGL/index.html`.
- Nur HumanoidCombatLab wird gebaut. Der Windows-Spielstand öffnet ein Fenster. WebGL wird für den lokalen Nachweis unkomprimiert gebaut; Veröffentlichung/Downloadoptimierung ist später separat zu entscheiden.
- Buildberichte stehen unter `Builds/B3c/WindowsBuildStatus.json` und `WebGLBuildStatus.json`. Die Queue überlebt den für einen Plattformwechsel nötigen Domain-Reload. Projektname, Vollbildmodus und WebGL-Kompression werden nach dem Build zurückgestellt.
- Browserstart: `powershell -ExecutionPolicy Bypass -File tools/Serve-Duel.ps1`, danach [lokales Duell](http://127.0.0.1:8765). Der Server bindet nur an localhost. Im Spielfeld klicken, damit Tastatur und Audio starten können. Nicht direkt per file:// öffnen.

Steuerung: WASD/Pfeile gehen, Ctrl rennen, Space springen, J Dreiercombo, K Heavy, L Kick, Shift ausweichen, 1/2 Gegnerzahl, R Neustart. Im Sprung J/L Air-Kick, K Air-Smash. Orange/rote Warnung: seitlich ausweichen oder mit einem rechtzeitigen Treffer unterbrechen.

## Danach

B4: einen charakteristischen Dash-/Schulterstoß mit vorhandenen Bewegung-/Combat-Regeln ausarbeiten, danach seine Distanz, Verpflichtung und Erholung gemeinsam mit dem Moveset abstimmen. Zusätzliche Gegnerrollen B5, längere Map mit Kampfbereichen/Checkpoints B6, finale Präsentation B7. Kein B4-Code in diesem Auftrag. Stilistische Character-Slice-Abnahme erst nach der separaten Wombat-Entscheidung.
