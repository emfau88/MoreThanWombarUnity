# More Than Wombat — Unity Lab

Eigenständiger Versuch eines stilisierten 3D-/2.5D-Arcade-Beat-'em-ups.
Das bestehende [Browsergame](https://github.com/emfau88/MoreThanWombat) ist
Designreferenz, keine technische Portierungsvorlage.

**Aktueller Stand:** S0–S2 abgeschlossen und vorläufig positiv gespielt. S3/B1
ergänzen Sparring, Ausweichen, Kick, eigene Luftangriffe, kontrollierte Angriffsschritte,
frühere Light-Anschlüsse nach Treffer, begrenzte Startup-Ausrichtung, Bewegungsfreigabe
in später Recovery und Ganzkörper-Tod/Reset. Wahlweise ein oder zwei Gegner mit
abwechselnder Angriffsfreigabe. Die separate Trainingsszene bleibt erhalten.
Noch kein eigenständiger Build. Figur und Animationen sind funktionale Platzhalter.

## Prototyp spielen

`UnityProject` mit Unity **6000.4.0f1** öffnen, die Szene
`Assets/Game/Scenes/SparringLab.unity` laden und Play drücken. In die Game-Ansicht
klicken, damit sie die Tastatureingaben erhält.

- WASD oder Pfeiltasten: auf der X/Z-Bodenfläche bewegen.
- Leertaste: springen; R: zurücksetzen; H: Debuganzeige umschalten.
- J / linke Maustaste: Light; K / rechte Maustaste: Heavy; L / Gamepad RB: Kick.
  Im Sprung starten J/L einen diagonalen Air-Kick und K einen Air-Smash.
  J rhythmisch für Jab → Cross → Finisher drücken. Die letzte Bewegungsrichtung
  bestimmt die Schlagrichtung. Früher Startup erlaubt eine kleine eingabegesteuerte
  Richtungskorrektur; Angriffe setzen kontrollierte Schritte nach vorne.
- Shift / Gamepad East: Ausweichen in Bewegungsrichtung (ohne Eingabe nach vorne).
- 1 / 2: einen beziehungsweise zwei Gegner aktivieren. Orange/roter Bodenmarker
  kündigt einen Schlag an. R setzt Spieler und Gegner zurück.
- `CombatLab.unity` bleibt als Training ohne Gegenangriffe verfügbar.
- `CharacterImportLab.unity` zeigt den B2-Importnachweis: temporärer Quaternius-Humanoid mit WASD/Stick, Sprung und Reset. Bewegung ist angebunden; Combat spielt man weiterhin im SparringLab.
- Gamepad-Bindings sind implementiert und mit simuliertem Gerät geprüft;
  ein echter Hardware-Spieltest steht noch aus.

Die Figur besteht aus selbst erzeugten 3D-Grundformen und ist bewusst ein
animierbarer Funktionsplatzhalter, kein finales Charaktermodell.

## Einstieg

- [Gestaltung und effiziente Produktion — aktuelle Vorgaben](PRODUCTION_GUIDELINES.md)
- [Konkrete nächste Schritte: Asset-Basis und Charakterduell](NEXT_STEPS.md)
- [Roadmap und Qualitätsgates](ROADMAP.md)
- [Technischer Ansatz](ARCHITECTURE.md)
- [Combat-Regeln und Testanleitung](COMBAT_SYSTEM.md)
- [Status und nächster Auftrag](TEST_SLICE_STATUS.md)
- [Externe Assets und Lizenznachweise](THIRD_PARTY_LICENSES.md)
- [B2: drei Kandidaten und konkrete Importbasis](B2_ASSET_SELECTION.md)

Gestaltungsziel ist der illustrative Cartoon-/Comic-Stil der 2D-Referenz:
runde kräftige Formen, expressive Gesichter und gemalte Materialdetails.
Passende fertige Rigs, Animationen und Props werden zuerst geprüft. Der
begonnene eigene Blender-/Rig-Pfad ist zurückgestellt. Der nächste Ausbau
konzentriert sich auf ein vollständiges Duell mit einem Spieler und einem
Gegner; WebGL-Verträglichkeit wird an diesem kleinen Slice geprüft.

Werkzeuganbindung, Bewegung und Combat-Polish sind umgesetzt; Nutzerfeedback
dient dem gezielten Tuning. B2-Schritte 1/2 liefern jetzt einen regulären Humanoid-Import mit fünf Bewegungsclips. Combat-Retargeting und die Entscheidung zur Wombat-Gestaltung folgen gemäß NEXT_STEPS.
Keine Cloud-Dienste, kein Multiplayer, keine Asset-Käufe und keine Änderungen
am Browsergame als implizite Arbeitsschritte.

Der Repository-Name `MoreThanWombarUnity` bleibt unverändert. Der Schreibfehler
ist für den Test technisch unerheblich; eine Umbenennung ist ein eigener Auftrag.
