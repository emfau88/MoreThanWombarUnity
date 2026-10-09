# More Than Wombat — Unity Lab

Eigenständiger Versuch eines stilisierten 3D-/2.5D-Arcade-Beat-'em-ups.
Das bestehende [Browsergame](https://github.com/emfau88/MoreThanWombat) ist
Designreferenz, keine technische Portierungsvorlage.

**Aktueller Stand:** B3a/B3b sind im HumanoidCombatLab integriert: Gehen/Rennen,
Jab → Cross → Haken, Overhand-Heavy, Kampfsport-Kicks und zweihändiger Luft-Smash sowie
sichtbare Hit-/Stagger-Reaktionen, Niederwerfen, Aufstehen und Tod für beide Figuren. Vorhandenes
Sparring, Ausweichen, Angriffsschritte, Combo-/Recovery-Regeln, Hitstop und Reset
bleiben angebunden. Lokal wahlweise ein bis vier Gegner. Temporärer menschlicher
Charakter; Wombat-Gestaltung bleibt offen. M1 ergänzt eine erste Junkyard-Kampfumgebung
mit Betonboden, Schrott/Reifen, Zaun, Werkstatt-Hintergrund und abgestimmtem Licht.
B3c stimmt KI-Abstand, Aufstehschutz und Feedback ab und ergänzt ein deutsches Duell-HUD.
Eigenständige Windows-/WebGL-Spielstände sind vorhanden; der Browserkampf wurde bis zum Sieg gespielt.

B4 ergänzt lokal einen kurzen Schulterstoß auf E/RT/Touch STOSS und mehr Kick-Abstand.
B5 ergänzt lokal Standard, Agile und Heavy sowie Mischkämpfe mit drei/vier Gegnern. B6 liefert eine eigene JunkyardChapter-Szene mit drei verbundenen Bereichen, neun Wellen, Torschalter, Checkpoints und Vorarbeiter-Finale. B7 ergänzt lokal unterschiedliche Bereichsgestaltung, gemalten Boden, Material-/Licht-/Kamerapass, warme Bärengesichter, kompakte HP-/Boden-/Tor-Marker und zwölf CC0-Audiosignale. Nächster Bulk: B8 für Menü, Pause, Einführung und Optionen. Der Play-Link enthält vorerst die vorige Touch-Fassung.

## Prototyp spielen

**[▶ Jetzt spielen — Schrotthof-Duell](https://emfau88.github.io/MoreThanWombarUnity/)**

Auf Android im Querformat öffnen: links bewegen, rechts kämpfen; oben **NEUSTART**.
Am Desktop stehen Tastatur und Gamepad zur Verfügung; **TOUCH** blendet die Bildschirmsteuerung ein.
Beim ersten Aufruf wird der Spielbuild heruntergeladen.

Direkt unter Windows: `Builds/B3c/Windows/MoreThanWombat.exe` starten.
Im Browser: `tools/Serve-Duel.ps1` ausführen und [lokales Duell](http://127.0.0.1:8765) öffnen.
Ins Spielfeld klicken. Builddetails und konkrete Prüfergebnisse: [B3c-Übergabe](B3C_DUEL_HANDOFF.md).

Die Browserfassung wird über GitHub Pages veröffentlicht. Der Workflow verwendet den fertigen
WebGL-Build aus einem GitHub-Release; Anleitung: [Mobile Touch und Veröffentlichung](MOBILE_TOUCH.md).

`UnityProject` mit Unity **6000.4.0f1** öffnen, die Szene
`Assets/Game/Scenes/HumanoidCombatLab.unity` laden und Play drücken. In die Game-Ansicht
klicken, damit sie die Tastatureingaben erhält.

**Kapitel lokal spielen:** Statt der Lab-Szene `Assets/Game/Scenes/JunkyardChapter.unity` öffnen. Nach rechts spielen; F/Gamepad LB/Touch TOR ÖFFNEN bedient den nahen Schalter nach Bereich 1. R/Start/Touch CHECKPOINT wiederholt den Abschnitt; Backspace/Select/Touch VON VORN startet das Kapitel neu. [B6: Ablauf und Checkpoints](B6_JUNKYARD_CHAPTER.md).

Die gespeicherte Kapitel-Szene enthält auch B7. [B7: Gestaltung, Audio und Nachweise](B7_PRESENTATION.md). Bestehendes Modell/Rig des Menschen und der Bären erhalten; endgültiger Wombat-Stil und längeres Kapitel bleiben offen.

- WASD oder Pfeiltasten: auf der X/Z-Bodenfläche bewegen.
- Ctrl / linker Gamepad-Trigger gehalten: Rennen am Boden. Loslassen: Gehen.
- Leertaste: springen; R: zurücksetzen; H: Debuganzeige umschalten.
- J / linke Maustaste: Light; K / rechte Maustaste: Heavy; L / Gamepad RB: Kick.
  Im Sprung starten J/L einen diagonalen Air-Kick und K einen Air-Smash.
  J rhythmisch für Jab → Cross → Finisher drücken. Die letzte Bewegungsrichtung
  bestimmt die Schlagrichtung. Früher Startup erlaubt eine kleine eingabegesteuerte
  Richtungskorrektur; Angriffe setzen kontrollierte Schritte nach vorne.
- Shift / Gamepad East: Ausweichen in Bewegungsrichtung (ohne Eingabe nach vorne).
- E / rechter Gamepad-Trigger / Touch **STOSS**: kurzer Schulterstoß am Boden.
  Schließt bis zu 2 m Distanz, stoppt am ersten Treffer oder an einer Wand und
  hat eine feste Erholung. Richtung beim Start wählen; Gegner können dich unterbrechen.
  B4 ist lokal in Unity enthalten; der öffentliche Play-Link und die bisherigen
  Builds enthalten zunächst die vorherige Touch-Fassung.
- Lokal 1–4: Gegnerzahl wählen; 3/4 starten den Mischkampf mit Standard, Agile und Heavy.
  Gamepad D-Pad oben oder Touch **GEGNER** schaltet weiter. Orange/roter Bodenmarker
  kündigt einen Schlag an; Agile zeigt zusätzlich seine feste Ansturmspur. R setzt die Gruppe zurück.
- Heavy und Luft-Smash werfen Gegner nieder; der Bären-Heavy kann dich niederwerfen.
  Nach Fall/Boden/Aufstehen erhältst du die Steuerung mit 0,45 s Schutz zurück.
  Bei 0 HP bleibt die Figur liegen; R startet das Duell neu.
- `CombatLab.unity` bleibt als Training ohne Gegenangriffe verfügbar.
- `CharacterImportLab.unity` bleibt der separate B2-Importnachweis für Bewegung und Sprung.
- `SparringLab.unity` bleibt die ursprüngliche B1-Vergleichsszene mit Grundform-Figur.
- Gamepad-Bindings sind implementiert und mit simuliertem Gerät geprüft;
  ein echter Hardware-Spieltest steht noch aus.
- Mobile Touch: links Bewegungsstick (außen Rennen), rechts Combo/Heavy/Kick/Sprung/Ausweichen/Stoß,
  oben Neustart und lokal ab B5 Gegnerzahl. Automatisch auf Touchgeräten; **TOUCH** schaltet die Anzeige auch am Desktop.
  Android im Querformat ist das erste Nutzungsziel. Anleitung und Nachweise: [Mobile Touch](MOBILE_TOUCH.md).

Der Humanoid ist eine importierte technische Basis, die Robot-Gegner sind eigene
Grundformen. Beide bleiben Platzhalter für die endgültige Cartoon-Gestaltung.

## Einstieg

- [Gestaltung und effiziente Produktion — aktuelle Vorgaben](PRODUCTION_GUIDELINES.md)
- [Konkrete nächste Schritte: Asset-Basis und Charakterduell](NEXT_STEPS.md)
- [Roadmap und Qualitätsgates](ROADMAP.md)
- [Technischer Ansatz](ARCHITECTURE.md)
- [Combat-Regeln und Testanleitung](COMBAT_SYSTEM.md)
- [Status und nächster Auftrag](TEST_SLICE_STATUS.md)
- [Externe Assets und Lizenznachweise](THIRD_PARTY_LICENSES.md)
- [B2: drei Kandidaten und konkrete Importbasis](B2_ASSET_SELECTION.md)
- [B2: Humanoid-Combat, Timing und Kontaktbahnen](B2_COMBAT_INTEGRATION.md)
- [B3a: Rennen, Kernaktionen und Trefferreaktionen](B3A_ACTIONS.md)
- [B3b: Niederwerfen, Aufstehen, Tod und Reset](B3B_BODY_RECOVERY.md)
- [B3c: abgestimmtes Duell und Windows-/WebGL-Übergabe](B3C_DUEL_HANDOFF.md)
- [Mobile Touch: Steuerung und Test im selben WLAN](MOBILE_TOUCH.md)
- [B4: Schulterstoß und Move-Rollen](B4_SHOULDER_CHARGE.md)
- [B5: Gegnerrollen und Mischkampf](B5_ENEMY_ROLES.md)
- [B6: Junkyard-Kapitel, Schalter und Checkpoints](B6_JUNKYARD_CHAPTER.md)
- [M1: vorgezogene Junkyard-Testmap](M1_MAP_PREVIEW.md)

Gestaltungsziel ist der illustrative Cartoon-/Comic-Stil der 2D-Referenz:
runde kräftige Formen, expressive Gesichter und gemalte Materialdetails.
Passende fertige Rigs, Animationen und Props werden zuerst geprüft. Der
begonnene eigene Blender-/Rig-Pfad ist zurückgestellt. Das lokale Junkyard-Kapitel
erweitert das überprüfte Duell um drei Bereiche und mehrere Gegnerrollen.
Der nächste Ausbau ergänzt den Ablauf vom Startmenü bis zum Kapitelabschluss.

Werkzeuganbindung, Bewegung und Combat-Polish sind umgesetzt; Nutzerfeedback
dient dem gezielten Tuning. B3a–B3c liefern Kernaktionen, Reaktionen, Fall/Aufstehen/Tod und das abgestimmte Duell samt Builds. B4 ergänzt Schulterstoß und mehr Kick-Abstand, B5 drei Gegnerrollen und Mischkampf. B6 ergänzt das durchspielbare Junkyard-Kapitel. B7 liefert lokal Welt-/Kamerapräsentation und Audio. Nächster Entwicklungsbulk ist B8 mit Startmenü, Pause, kurzem Intro und Optionen; Wombat-Gestaltung bleibt separat offen.
Keine Cloud-Dienste, kein Multiplayer, keine Asset-Käufe und keine Änderungen
am Browsergame als implizite Arbeitsschritte.

Der Repository-Name `MoreThanWombarUnity` bleibt unverändert. Der Schreibfehler
ist für den Test technisch unerheblich; eine Umbenennung ist ein eigener Auftrag.
