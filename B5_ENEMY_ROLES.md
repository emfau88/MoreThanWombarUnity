# B5 — Gegnerrollen und Mischkampf

Stand: 7. Oktober 2026. Lokal im gespeicherten `HumanoidCombatLab` integriert. Die bisherigen Windows-/WebGL-Builds und der öffentliche Play-Link enthalten weiterhin die vorige Touch-Fassung. B4 und B5 benötigen einen neuen Build-/Veröffentlichungsschritt.

## Spielen und Wirkung

Unity öffnen, `Assets/Game/Scenes/HumanoidCombatLab.unity` laden und Play drücken. Start bleibt das Heavy-Einzelduell. **1–4** wählt die Gegnerzahl und setzt den Kampf zurück; **Gamepad D-Pad oben** und Touch **GEGNER** schalten zyklisch weiter. **R/Start/NEUSTART** wiederholt die aktuelle Gruppe.

- **1:** Heavy allein, wie die bisherige Mensch-gegen-Bären-Situation.
- **2:** Heavy und Standard; der kürzere Standardschlag wechselt mit dem stärkeren Heavy.
- **3:** Heavy, Standard und Agile — erster Mischkampf mit angekündigtem Ansturm.
- **4:** dieselbe Gruppe plus ein zweiter Standard für mehr Positionsdruck.

Orange Handschuhe/große Silhouette kennzeichnen Heavy, türkis Standard und violett die schmalere Agile-Figur. Namensmarkierungen über den Gegnern ergänzen die Unterscheidung. Der Ansturm besitzt eine sichtbare feste Bodenspur. Die endgültige Wombat-/Cartoon-Gestaltung bleibt separat offen.

## Rollenwerte

| Rolle | HP | Schaden | Geschwindigkeit | Warnung | Zusätzliche Erholung | Zweck |
| --- | --- | --- | --- | --- | --- | --- |
| Standard | 65 | 8 | 2,2 m/s | 0,45 s | 0,35 s | Kurzer Nahkampfschlag, unterbrechbar |
| Agile | 55 | 10 | 3,3 m/s | 0,80 s | 0,60 s | Bis zu 2,35 m gerader Ansturm, seitlich ausweichbar |
| Heavy | 80 | 18 | 1,6 m/s | 0,75 s | 0,60 s | Langsamer kräftiger Schlag mit Knockdown |

Standard/Heavy beginnen innerhalb von 1,30 m. Agile sucht etwa 2,50 m Abstand und darf zwischen 1,60 und 3,20 m warnen. Richtung und Spur werden beim Warnstart festgelegt; der Lauf verfolgt den Spieler danach nicht. Treffer, Körper-/Wandkollision oder Arenarand beenden den Weg. Startup/Active/Recovery der Rush-Animation: 0,10/0,24/0,18 s. Nach der zusätzlichen Erholung folgen rollenabhängiger Cooldown und eine gemeinsame kurze Pause.

## Gemeinsame Regeln und vorhandene Technik

`EnemyRoleDefinition` hält die kleinen Rollenwerte und Attack-Zuordnungen als bearbeitbare Unity-Assets. Alle Rollen verwenden denselben `EnemyBrain`, `TrainingDummy`, Animator-Pfad und `BodyRecovery`. Standard/Heavy verwenden vorhandene Cross-/Heavy-Grundlagen, Rush leitet den vorhandenen Gehclip mit begrenzten Arm-/Torsoanpassungen ab. Eigene Bären-Grundform und Materialien werden weiterverwendet; keine neue Asset-Bibliothek oder parallele KI.

Der `EngagementCoordinator` vergibt weiterhin **genau eine** Freigabe für Warnung/Angriff/Erholung. Er berücksichtigt alle vier bereiten Gegner der Reihe nach, statt den ersten Update-Aufruf zu bevorzugen. Treffer, Tod, Disable, Abbruch und Reset geben die Freigabe frei. Außerhalb des Kamerabilds beginnt kein Angriff; ein bereits warnender/angreifender Gegner bricht dort ab. Während Knockdown/GetUp/Aufstehschutz erhält kein Gegner eine neue Freigabe.

Wartende Gegner verteilen sich seitlich, schaffen in unmittelbarer Nähe Platz und stoßen sich leicht voneinander ab. B5-Bewegung prüft eine Körperkapsel gegen bestehende physische Kollision; Hurtbox-Trigger blockieren keinen Lauf. Kein neues NavMesh-/Crowd-System. Die inaktive Startgruppe wird beim Reset ebenfalls korrekt initialisiert.

Schulterstoß schließt Distanz und unterbricht eine Warnung, Kick schafft Raum, Heavy wirft nieder. Es gibt keine zusätzliche Heavy-Rüstung; klare Warnung, langsamere Bewegung und Knockdown bilden seine Rolle bereits ab.

## Gezielte Prüfung

**4/4 neue PlayMode-Fälle bestanden**, 30,17 s: Warnung vor Schaden, eigener einzelner Schaden aller Rollen, Heavy-Knockdown, feste Rush-Spur/seitlicher Fehlschlag, Wandstopp, Freigabe-Unterbrechung, Offscreen-/Aufstehschutz, alle vier Angriffswechsel und Reset/Touch-Gruppenwahl. Resultat: `tools/b5-role-results.json`. Keine Vollsuite.

Zusätzlich bestehen **3/3 Duellfälle** (16,21 s), **4/4 Schulterstoßfälle** (7,31 s) und **1/1 bisheriger Zwei-Gegner-Fall** (8,65 s): insgesamt zwölf gezielte Fälle, keine Vollsuite. Ergebnisse: `tools/b5-duel-regression-results.json`, `b5-charge-regression-results.json` und `b5-group-regression-results.json`.

Echte Gruppenwarnung sowie Warn-/Lauf-/Erholungspose des Agile aus der Spielkamera angesehen. Die tatsächliche seitliche Bewegung vermeidet den Ansturm bei 100 Spieler-HP. Touch-Canvas mit **GEGNER: 4**, allen vorhandenen Aktionen und lesbaren Beschriftungen bei 1280×600 kontrolliert. Für diese Aufnahme wurden die Runtime-Canvases vorübergehend durch die Spielkamera gerendert und danach zurückgestellt, da die normale CLI-Screen-Aufnahme teilweise alte beziehungsweise beschädigte UI-Bilder lieferte. Bilder liegen lokal unter `UnityProject/Assets/QA/b5-group-warning.png`, `b5-actual-rush-*.png` und `b5-touch-canvas.png`; genauer Nachweis: `tools/b5-visual-review.md`. Hardware-Spielgefühl und Android-Geräteprüfung bleiben Nutzerfeedback.

## Erneut anwenden und nächster Bulk

`Wombat Lab/B5 Apply Enemy Roles` aktualisiert Rollen, Controller, vier Gegner und HUD in der gespeicherten Szene. Beim bewussten vollständigen Neuaufbau: HumanoidCombatBuilder → CharacterActionBuilder → BodyRecoveryBuilder → JunkyardPreviewBuilder → DuelSliceBuilder.Apply → MobileTouchBuilder.Apply → ShoulderChargeBuilder.Apply → EnemyRolesBuilder.Apply. Frühere Builder allein stellen ältere Stufen her.

**B6** baut daraus die zusammenhängende Junkyard-Strecke: drei Kampfbereiche, einfache Encounter-/Wellenwerte, eine Interaktion, Checkpoint-Retry und Abschlusskampf. Die 10–15 Minuten bleiben ein Designziel für den abgestimmten Ablauf; die kleine Testarena ist noch kein Kapitel.

Übergabe: HumanoidCombatLab frisch geladen, Play gestoppt, Szene clean, keine fehlgeschlagene Script-Kompilierung. Vier Gegner mit vollständigen Rollen-/Attack-/Spieler-/Zielreferenzen; ein Gegner aktiv als Startduell. Normale Zeit und Eingaben wiederhergestellt.
