# Mobile Touch — Kapitel und Duell

Stand: 9. Oktober 2026. Der öffentliche Play-Link enthält jetzt das B8-Kapitel: STOSS, drei Gegnerrollen, drei Bereiche, Checkpoints/Torinteraktion, B7-Präsentation und B8-Menü/Pause/Optionen. [Release-Nachweis](B8_WEBGL_RELEASE.md). Die Touch-Bindings verwenden dieselbe Eingabepipeline wie Tastatur/Gamepad.

## Bedienung

- Auf Touchgeräten automatisch sichtbar; **TOUCH** oben rechts schaltet die Oberfläche auch am Desktop ein/aus.
- Links Stick ziehen: Richtung und analoges Gehen. Am äußeren Rand wird gerannt; loslassen beendet Bewegung und Rennen.
- Rechts **COMBO**, **HEAVY**, **KICK**, **SPRUNG**, **AUSWEICHEN**. Antippen startet eine Aktion; gehaltene Attack-Buttons wiederholen nicht automatisch. Für die Light-Kette wiederholt tippen.
- **STOSS** (rechter Trigger): kurzer gerader Schulterstoß am Boden; im öffentlichen Kapitel enthalten.
- Nur im lokalen Rollenlabor: **GEGNER: 1–4** schaltet die Gruppe und setzt den Kampf zurück. Im Kapitel ersetzt **VON VORN** die Gegnerwahl; die Wellen bestimmen die Gruppen.
- Bewegung und Aktionsbuttons können gleichzeitig mit verschiedenen Fingern bedient werden. Sprung plus Combo/Kick verwendet weiterhin Air-Kick, Sprung plus Heavy Air-Smash.
- **NEUSTART** setzt das Duell auch nach Niederlage zurück. Querformat ist die vorgesehene Spielansicht; Hochformat zeigt einen Drehhinweis.
- Tastatur-/Gamepad-Regeln bleiben vorhanden. Bei eingeblendeter Touch-Oberfläche lösen Maus-/emulierte Touch-Mausklicks keine zusätzlichen Angriffe aus.

## Kapitel spielen

Im Hauptmenü **SPIELEN** wählen; die Einführung ist überspringbar. In `JunkyardChapter` ersetzt **CHECKPOINT** das Lab-NEUSTART und **VON VORN** die Gegnerwahl. Der kontextabhängige Button **TOR ÖFFNEN** erscheint nach Bereich 1 nahe dem Schalter. **PAUSE** öffnet Fortsetzen/Steuerung/Optionen/Startmenü. Diese Bedienung ist am öffentlichen Link verfügbar.

B7: Touch-HUD in einer gestellten finalen Presswerk-Ansicht bei 1280×600 kontrolliert, Kamera-/Canvas-Aufnahme-Einstellungen danach wiederhergestellt. Vorhandener tatsächlicher Mischkampf-/Touch-Tor-Fall besteht erneut. Dieser B7-Nachweis war noch kein physischer Android-Spieltest oder öffentlicher Kapitelbuild. Details: [B7_PRESENTATION.md](B7_PRESENTATION.md).

## Integration

B8 ergänzt PAUSE und die gemeinsame Start-/Einführungs-/Options-/Ergebnisoberfläche. Während Menüs sind Gameplay-Controls ausgeblendet und virtuelle gehaltene Eingaben freigegeben; Fortsetzen wartet zusätzlich auf neutralen Gameplay-Input. CHECKPOINT/VON VORN rufen im Kapitel vorhandene Session-/Checkpoint-Aktionen auf; der Duell-Neustart bleibt erhalten. Zwei gezielte Sitzungsfälle bestehen, darunter Pause mit gehaltenem Touch-Stick/Combo. Einführung und Pause im Touch-Layout angesehen; kein physischer Android-Nachweis. Das Kapitel ist inzwischen veröffentlicht. Details: [B8_SESSION.md](B8_SESSION.md).

`MobileTouchControls` im gespeicherten HumanoidCombatLab erstellt einen kleinen Canvas mit Unitys vorhandenen `OnScreenStick`/`OnScreenButton`, `GraphicRaycaster`, `EventSystem` und `InputSystemUIInputModule`. Alle Aktionsbuttons speisen bereits vorhandene Gamepad-Bindings. Keine zweite Phasen-/Schadenslogik. LabInput führt nur die Pointer-Sperre und den äußeren Stickbereich als Rennen hinzu. Fokusverlust, Pause und Ausblenden geben virtuelle Eingaben frei.

Der Canvas berücksichtigt Screen.safeArea und skaliert nach Höhe im Querformat. `Assets/WebGLTemplates/TouchDuel/index.html` passt das Spielfeld an den Browser an, berücksichtigt CSS-Safe-Area und verhindert Scroll-/Kontextmenügesten auf dem Spielfeld. Pixeldichte ist auf mobilen Browsern auf 1,5 begrenzt. Der bestehende Builder nutzt das Template vorübergehend und stellt die Projekteinstellung anschließend zurück.

Nach komplettem Szenen-Neuaufbau: HumanoidCombatBuilder → CharacterActionBuilder → BodyRecoveryBuilder → JunkyardPreviewBuilder → DuelSliceBuilder.Apply → **MobileTouchBuilder.Apply** → **ShoulderChargeBuilder.Apply** → **EnemyRolesBuilder.Apply**. Editor-Menü für Touch: **Wombat Lab → Apply Mobile Touch**.

## Handy im selben WLAN

Kapitel-WebGL-Build über **Wombat Lab → B8 Build Chapter WebGL** erstellen. `tools/Serve-Duel.ps1 -Build B8 -Lan -Port 8766` starten und am Handy `http://<IPv4-Adresse dieses PCs>:8766` öffnen. Beide Geräte müssen denselben PC erreichen können; falls Windows eine Firewallabfrage zeigt, den Zugriff im privaten Netzwerk erlauben. Der normale Serverstart ohne `-Lan` bleibt nur lokal erreichbar. Für den Nutzercheck ist der öffentliche Link einfacher.

## Öffentlicher Play-Link und Updates

[Schrotthof-Kapitel spielen](https://emfau88.github.io/MoreThanWombarUnity/). Der Link steht auch oben im README. Android im Querformat ist das erste Ziel; es ist eine Browserfassung.

GitHub Pages verwendet die bereits gewählte Quelle **GitHub Actions**. `.github/workflows/play.yml` lädt die Teile `webgl-duel.zip.part-*` aus einem veröffentlichten Release, setzt das Archiv zusammen, entpackt den Spielstand und veröffentlicht ihn mit den offiziellen Pages-Actions. Die 5-MiB-Teile umgehen die hier beobachteten Timeouts beim großen Upload. Unity wird lokal mit der vorhandenen Installation gebaut; keine Unity-Lizenz oder neue Build-Infrastruktur auf GitHub erforderlich. Builds bleiben außerhalb der Git-Historie.

Für ein Kapitelupdate: Spiel-/Publishing-Änderungen zuerst committen und pushen, WebGL mit diesem Commit über `DuelSliceBuilder.Start("WebGL", true, "<Commit-SHA>")` bauen und kurz prüfen. Danach `tools/Publish-Duel.ps1 -Build B8 -Tag <neue-version>` ausführen. Das Skript prüft Szene/Spielcommit, verpackt die WebGL-Ausgabe, ergänzt `version.json` und veröffentlicht einen Release. Anschließend startet es den Pages-Workflow auf `main`; dieser versioniert die Asset-Adressen gegen alte Browser-Loader. Bestehende Releases können über **Actions → Publish playable WebGL game → Run workflow** mit dem Release-Tag erneut veröffentlicht werden. Ohne `-Build B8` wählen die Werkzeuge weiterhin den früheren B3c-Duellpfad.

## Frühere Nachweise und aktueller Release

Aktuell: `chapter-b8-2026-10-09`, etwa 19,8 MB, erfolgreicher Pages-Lauf, Hauptmenü/Einführung/erste Welle/Touch-Oberfläche/Pause/Optionen/Fortsetzen im öffentlichen Browser geprüft. Einzelheiten und Grenzen: [B8_WEBGL_RELEASE.md](B8_WEBGL_RELEASE.md). Die folgenden Duell-/B7-Nachweise beschreiben ihre jeweiligen früheren Stände.

Drei gezielte PlayMode-Prüfungen bestehen: Stick und separater Angriffs-Pointer funktionieren gleichzeitig; alle fünf Aktionen samt Neustart benutzen die bestehenden Bindings und erzeugen keinen zusätzlichen Mausschlag; Fokusabbruch und Ausblenden lösen gehaltene Controls. Ergebnis: `tools/mobile-touch-results.json`. Tastatur-Heavy und Desktop-Mausangriff nach Ausblenden sind darin ebenfalls geprüft.

Der neue WebGL-Build ist erfolgreich (66.424.982 Bytes); Bericht: `tools/mobile-touch-webgl-results.json`. Der Bericht enthält 354 Warnungen und eine Fehlermeldung; der tatsächliche Buildstatus lautet Succeeded. Unity meldete unter anderem einen beim Shutdown beendeten Worker. Im geladenen Browser treten die bekannten AppUI-/URP-FSR-Initialisierungswarnungen auf, keine Konsolenfehler. Die 900×420-Querformatansicht zeigt das Duell samt Controls; Neustart stellt 100/80 HP her und der Sprung-Button hebt die Figur sichtbar ab. Der bestehende Windows-Build bleibt die B3c-Fassung vor Touch.

Erstveröffentlichung am 5. Oktober 2026 erfolgreich: Release `duel-touch-2026-10-05`, [Pages-Workflow](https://github.com/emfau88/MoreThanWombarUnity/actions/runs/37365892811) abgeschlossen. Der öffentliche Play-Link lädt das Duell samt Touch-Controls, ohne Konsolenfehler. `version.json` nennt Spielcommit `5b2a273`; spätere Commits betreffen nur Upload/Workflow und Dokumentation. Screenshot lokal unter `UnityProject/Assets/QA/mobile-touch-public.png`. Die Release-Teile wurden gegen das vollständige Archiv mit SHA-256 geprüft; der Workflow entpackt das Archiv erfolgreich.

Desktopbrowser-/simulierte Pointer-Prüfung ist kein physischer Android-/iPhone-Nachweis. Der tatsächliche Handytest bleibt offen: gleichzeitig bewegen und angreifen, springen, ausweichen, Neustart und Wechsel zurück in den Browser prüfen. Eine mobile Steuerung allein garantiert keine bestimmte Handy-Framerate.
