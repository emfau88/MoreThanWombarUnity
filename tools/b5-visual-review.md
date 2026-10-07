# B5 — kurze Sichtkontrolle, 7. Oktober 2026

Gespeicherte HumanoidCombatLab-Szene im echten Editor-Play gestartet. Vier Gegner mit eigenem Rollenlabel/Farbe sichtbar, gemeinsame Warnung und 82 Spieler-HP im tatsächlichen Gruppenkampf (`Assets/QA/b5-group-warning.png`).

Agile separat innerhalb der Szene mit bestehenden KI-/Motor-Komponenten gespielt. Eine zusammenhängende Sequenz erfasst die tatsächliche Warnung, Ansturm-Pose und Erholung über den vorhandenen `LabVisualReview.Capture`-Helfer. Beim Warnbeginn seitlichen Bewegungsinput gegeben; am Ende RECOVERY, Spieler 100 HP, `ChargeStopped=False` (Fehlschlag). `b5-actual-rush-warning.png`, `b5-actual-rush-active.png` und `b5-actual-rush-recovery.png` angesehen. Die Distanz im lokalen Rohbericht misst ab der manuell gesetzten Ausgangsposition und enthält den vorherigen KI-Anlauf; sie ist kein isolierter Rush-Reichweitennachweis. Diese Reichweite ist im bestandenen Rollen-PlayMode-Fall separat geprüft.

Touch **GEGNER: 4**, NEUSTART, Stick und die sechs bestehenden Aktionsbuttons bei 1280×600 sichtbar und lesbar (`Assets/QA/b5-touch-canvas.png`). Für die Aufnahme die tatsächlichen Runtime-Canvases vorübergehend als ScreenSpaceCamera gerendert und anschließend auf ihre ursprünglichen Modi/Kameras zurückgestellt. Keine gespeicherte Änderung des Canvas-Pfads. Die normale CLI-Screen-Aufnahme war teilweise veraltet beziehungsweise zeigte beschädigte Texte; frühe `b5-rush-active-miss.png`/`b5-touch-group*.png` deshalb nicht als Nachweis verwenden.

Temporäre Eingabehaken, Gegner-Deaktivierung, Pause und Slow Motion gehören ausschließlich zur Sichtkontrolle und werden vor Übergabe durch Stop und frisches Laden der gespeicherten Szene beendet. Keine neue Vollsuite, kein neuer Build, keine Hardware-/Performanceabnahme.
