# Referenzprüfung — erste Combat-Testentscheidung

## Ergänzung: visuelle Referenz und aktuelle Produktionsrichtung

Am 3. Oktober 2026 wurden die tatsächliche Wombat-Spritesheet-Datei
`public/assets/characters/wombat/wombat_spritesheet.png` und der Junkyard-Screen
`docs/qa/ui-r4-2026-09-11/refresh/wide-landscape.png` im alten Projekt angesehen.
Sie zeigen illustrative Cartoon-/Comic-Figuren mit runden kräftigen Formen,
Gesichtsausdruck, dunklen Konturen und gemalten Fell-/Materialdetails. Die 3D-
Interpretation folgt diesem Look; sichtbare Low-Poly-Facetten sind kein Ziel.
Die Arena kombiniert verwitterte industrielle Details mit warmer/kühler
Lichtstimmung. Sie ist ein Gestaltungsbezug, kein automatisch übernehmbares Asset.

Passende externe Modell-/Rig-/Animationsgrundlagen zuerst prüfen, Eigenarbeit
auf Wombat-Identität und Combat-Anpassung konzentrieren. Einzelduell vor weiteren
Rollen/Levels. Konkrete Vorgaben in [PRODUCTION_GUIDELINES.md](PRODUCTION_GUIDELINES.md).

## Historische technische Referenzprüfung

3. Oktober 2026. Tatsächliche lokale Browser-Implementierung wurde lesend geprüft.
Dies ist eine fokussierte Analyse für S0–S2, kein abgeschlossener Content-Audit
aller Charaktere, Level, Audio-Assets und QA-Bilder.

| Gelesene Quelle | Befund | Konsequenz im neuen Projekt |
| --- | --- | --- |
| `combat/Fighter.ts`, `MovementContract.ts` | Normalisierte Bodenbewegung, getrennte Höhe/Gravitation, Landing und Attack-Locks; Rendercontainer hängen an Phaser | Absicht übernehmen; CharacterController auf echter X/Z-Fläche, Y als Höhe |
| `data/fighters.ts`, `data/attacks.ts`, `BasicChainContract.ts` | Wombat mit Jab/Backhand/Finisher, verschiedenen Schäden und kurzen Combo-Fenstern | Vier eigene 3D-Testclips und AttackDefinition-Assets; Werte neu balancierbar |
| `combat/MoveTimeline.ts` | Startup/Active/Recovery anhand verstrichener Zeit | Animator-Clip-Position ist die Phasenquelle; keine unabhängige Timer-Attacke |
| `combat/InputBuffer.ts` | Kurzer Action-Buffer; bei Combat-Freeze läuft seine Lebenszeit nicht ab | Gemeinsame Combat-Zeit; Input erfassen während Hitstop, einmal konsumieren |
| `combat/CombatResolver.ts`, `Fighter.hitTargets` | Hurtbox/Hitbox, Lane-/Höhenfilter, defensive Responses und bereits getroffene Ziele | Echte 3D-Hurtboxes, Team-Filter und Set je Attack-Instanz; sichtbarer Kontakt |
| `combat/DefenseContract.ts` | Guard/Evade, Knockdown/GetUp mit unterschiedlichen Schutzfenstern | Nicht in S0–S2 übernehmen; Evade/Gegenkampf erst nach Feedback |
| `core/EncounterDirector.ts`, `ai/EnemyController.ts` | Attack-Tokens und Spacing; Gegner mit Rollen, Telegraphen und Reposition | Erst S3: ein Gegnertyp, zwei Instanzen und ein Token statt kompletter Waves |
| `core/InputController.ts` | Browser-Tasten und Touch liefern Action-Flags | Neue Unity-Input-System-Actionmap mit Tastatur/Gamepad; kein Touch-HUD-Port |
| `combat/CombatPresentationController.ts` | Trefferklassen und kurze Kamera-/Kontaktreaktionen | Kleine unabhängige Feedback-Komponente; Effekte verursachen keinen Schaden |
| `core/DepthSort.ts`, `ui/Hud.ts`, `ui/HudLayout.ts` | 2D-Sortierung und assetabhängiger Browser-HUD | Echte 3D-Tiefe und minimaler Test-HUD; kein Rahmen-/Porträt-Port |
| `docs/qa/` Inventar | Umfangreiche historische QA zu Assets, VFX, UI und Wave-Fluss | Nicht als Nachweis für neues Unity-Gameplay verwenden; frische Tests nötig |

## Bewusst nicht übernehmen

Spriteatlanten, Phantom-Höhenprojektion, Browser-UI-Rahmen und Touch-Anordnung,
breiter Roster, Mana/Specials/Ultimate, Stage-Wechsel und Bosscontent.
Kein Asset aus dem Browsergame wird für den 3D-Platzhalter benötigt.

## Was der erste Test nicht beweist

Ein aus selbst erzeugten 3D-Grundformen gebauter und animierter Test-Wombat
ist ein funktionaler Platzhalter, kein professionell modellierter Charakter.
Die volle Art Direction, hochwertiges Rigging, Gesichtsanimation, Gegnergattung
und Junkyard-Produktion benötigen eigene spätere Entscheidungen und Nachweise.
