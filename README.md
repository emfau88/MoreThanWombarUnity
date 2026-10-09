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

B4 ergänzt einen kurzen Schulterstoß auf E/RT/Touch STOSS und mehr Kick-Abstand.
B5 ergänzt Standard, Agile und Heavy sowie Mischkämpfe mit drei/vier Gegnern. B6 liefert eine eigene JunkyardChapter-Szene mit drei verbundenen Bereichen, neun Wellen, Torschalter, Checkpoints und Vorarbeiter-Finale. B7 ergänzt unterschiedliche Bereichsgestaltung, gemalten Boden, Material-/Licht-/Kamerapass, warme Bärengesichter, kompakte HP-/Boden-/Tor-Marker und zwölf CC0-Audiosignale. B8 ergänzt Startmenü/Fortsetzen, Pause, Einführung, gespeicherte Optionen und Niederlage-/Sieg-/Replay-Anzeige. **B9–B11 sind zusätzlich lokal integriert:** Boden-AOE, MP mit drei Spezialaktionen, Raufbold/Werfer/Schläger und ein Acht-/Zehn-Gegner-Labor. [Spielanleitung und Nachweise](B9_B11_LF2_COMBAT.md). **B12 ist ebenfalls lokal integriert:** fünf Begegnungen mit genau 30 Gegnern, angekündigten Verstärkungen und überarbeiteten Checkpoints. [Stage und Nachweise](B12_LF2_STAGE.md). Nächster Bulk ist B13: Präsentationspolitur. Diese lokalen Änderungen sind noch nicht im öffentlichen Play-Link enthalten. Der Play-Link enthält jetzt das B8-Kapitel mit Hauptmenü, Anlieferung, Sortierhof und Presswerk. [Release-Nachweis](B8_WEBGL_RELEASE.md).

## Prototyp spielen

**Lokale B12-Stage:** `Assets/Game/Scenes/JunkyardChapter.unity` öffnen → Play → SPIELEN. Browserbuild mit `tools/Serve-Duel.ps1 -Build B12 -Port 8767` starten und [Stage lokal spielen](http://127.0.0.1:8767/).

**Separates B9–B11-Kampflabor:** `Assets/Game/Scenes/CrowdCombatLab.unity` in Unity öffnen und Play starten: acht Gegner, `0` für zehn, `R` für Reset. Luft + K = AOE-Smash, E = Durchbruch, Q = Druckwelle. MP und neue Rollen sind auch in der Kapitel-Szene integriert. [Details](B9_B11_LF2_COMBAT.md).

Lokaler Browserbuild: `tools/Serve-Duel.ps1 -Build B11 -Port 8766` und [Gruppenkampf öffnen](http://127.0.0.1:8766/). Für ein Handy im selben WLAN den Server mit `-Lan` starten und die IPv4-Adresse des PCs verwenden; das ist ein eigener Gerätecheck, kein bereits bestätigter Android-Nachweis.

**[▶ Jetzt spielen — Schrotthof-Kapitel](https://emfau88.github.io/MoreThanWombarUnity/)**

Auf Android im Querformat öffnen und im Hauptmenü **SPIELEN** wählen: links bewegen, rechts kämpfen. **PAUSE** öffnet das Sitzungsmenü; **CHECKPOINT** wiederholt den Abschnitt.
Am Desktop stehen Tastatur und Gamepad zur Verfügung; **TOUCH** blendet die Bildschirmsteuerung ein.
Beim ersten Aufruf wird der Spielbuild heruntergeladen.

Früherer Windows-Duellbuild: `Builds/B3c/Windows/MoreThanWombat.exe`. Ein Windows-Kapitelbuild ist noch offen.
Aktuelles Kapitel im Browser: `tools/Serve-Duel.ps1 -Build B8` ausführen und [lokales Kapitel](http://127.0.0.1:8765) öffnen. Ohne `-Build B8` startet der Server den älteren Duell-Build.
Ins Spielfeld klicken. Builddetails und konkrete Prüfergebnisse: [B3c-Übergabe](B3C_DUEL_HANDOFF.md).

Die Browserfassung wird über GitHub Pages veröffentlicht. Der Workflow verwendet den fertigen
WebGL-Build aus einem GitHub-Release; Anleitung: [Mobile Touch und Veröffentlichung](MOBILE_TOUCH.md).

`UnityProject` mit Unity **6000.4.0f1** öffnen, die Szene
`Assets/Game/Scenes/HumanoidCombatLab.unity` laden und Play drücken. In die Game-Ansicht
klicken, damit sie die Tastatureingaben erhält.

**Kapitel im Editor spielen:** Statt der Lab-Szene `Assets/Game/Scenes/JunkyardChapter.unity` öffnen, Play und **SPIELEN** wählen. ESC/P/Gamepad Start/PAUSE pausiert. F/Gamepad LB/Touch TOR ÖFFNEN bedient den nahen Schalter nach Bereich 1. R/Touch CHECKPOINT wiederholt den Abschnitt; Backspace/Select/Touch VON VORN startet das Kapitel neu. Nach Niederlage/Sieg erscheinen Retry/Replay und Startmenü. [B8: Sitzung, Einführung und Optionen](B8_SESSION.md), [B6: Ablauf und Checkpoints](B6_JUNKYARD_CHAPTER.md).

Die gespeicherte Kapitel-Szene enthält auch B7. [B7: Gestaltung, Audio und Nachweise](B7_PRESENTATION.md). Bestehendes Modell/Rig des Menschen und der Bären erhalten; endgültiger Wombat-Stil bleibt offen. Der lokale B12-Ablauf umfasst die geplanten 30 Gegner.

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
  Der öffentliche Kapitelbuild enthält den Schulterstoß und die drei Gegnerrollen.
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
  im Kapitel oben Pause, Checkpoint und Von vorn; Gegnerzahlwahl bleibt im Labor. Automatisch auf Touchgeräten; **TOUCH** schaltet die Anzeige auch am Desktop.
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
- [B8: Menü, Pause, Einführung und Optionen](B8_SESSION.md)
- [M1: vorgezogene Junkyard-Testmap](M1_MAP_PREVIEW.md)

Gestaltungsziel ist der illustrative Cartoon-/Comic-Stil der 2D-Referenz:
runde kräftige Formen, expressive Gesichter und gemalte Materialdetails.
Passende fertige Rigs, Animationen und Props werden zuerst geprüft. Der
begonnene eigene Blender-/Rig-Pfad ist zurückgestellt. Das lokale Junkyard-Kapitel
erweitert das überprüfte Duell um drei Bereiche und mehrere Gegnerrollen.
B8 verbindet Startmenü und Kapitelabschluss; als Nächstes folgen Balance und Demo-Übergabe.

Werkzeuganbindung, Bewegung und Combat-Polish sind umgesetzt; Nutzerfeedback
dient dem gezielten Tuning. B3a–B3c liefern Kernaktionen, Reaktionen, Fall/Aufstehen/Tod und das abgestimmte Duell samt Builds. B4 ergänzt Schulterstoß und mehr Kick-Abstand, B5 drei Gegnerrollen und Mischkampf. B6 ergänzt das durchspielbare Junkyard-Kapitel. B7 liefert Welt-/Kamerapräsentation und Audio. B8 liefert Startmenü, Pause, Intro und Optionen; das Kapitel ist jetzt als WebGL-Release öffentlich spielbar. B9–B11 ergänzen lokal AOE, MP, drei Spezialaktionen und dichteren Gruppenkampf. Nächster Entwicklungsbulk ist B12 gemäß [LF2-Stage-Plan](LF2_STAGE_PLAN.md). Für diese Stage bleibt der menschliche Kämpfer die Basis; die endgültige Wombat-Gestaltung bleibt offen.
Keine Cloud-Dienste, kein Multiplayer, keine Asset-Käufe und keine Änderungen
am Browsergame als implizite Arbeitsschritte.

Der Repository-Name `MoreThanWombarUnity` bleibt unverändert. Der Schreibfehler
ist für den Test technisch unerheblich; eine Umbenennung ist ein eigener Auftrag.
