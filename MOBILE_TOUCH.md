# Mobile Touch — Duell-Steuerung

Stand: 7. Oktober 2026. Die veröffentlichte Touch-Fassung stammt vom 5. Oktober; B4 ergänzt lokal STOSS über dieselbe Eingabepipeline; B5 ergänzt die Gegnerzahlwahl.

## Bedienung

- Auf Touchgeräten automatisch sichtbar; **TOUCH** oben rechts schaltet die Oberfläche auch am Desktop ein/aus.
- Links Stick ziehen: Richtung und analoges Gehen. Am äußeren Rand wird gerannt; loslassen beendet Bewegung und Rennen.
- Rechts **COMBO**, **HEAVY**, **KICK**, **SPRUNG**, **AUSWEICHEN**. Antippen startet eine Aktion; gehaltene Attack-Buttons wiederholen nicht automatisch. Für die Light-Kette wiederholt tippen.
- Lokal ab B4 zusätzlich **STOSS** (rechter Trigger): kurzer gerader Schulterstoß am Boden. Ein neuer WebGL-Build mit Veröffentlichung ist nötig, damit die Erweiterung am öffentlichen Play-Link erscheint.
- Lokal ab B5 **GEGNER: 1–4** oben: schaltet Gruppe weiter und setzt den Kampf zurück. Der Modus bleibt auch bei Tastaturwahl synchron; 3/4 aktivieren den Mischkampf. Der öffentliche Play-Link benötigt dafür ebenfalls einen neuen Build.
- Bewegung und Aktionsbuttons können gleichzeitig mit verschiedenen Fingern bedient werden. Sprung plus Combo/Kick verwendet weiterhin Air-Kick, Sprung plus Heavy Air-Smash.
- **NEUSTART** setzt das Duell auch nach Niederlage zurück. Querformat ist die vorgesehene Spielansicht; Hochformat zeigt einen Drehhinweis.
- Tastatur-/Gamepad-Regeln bleiben vorhanden. Bei eingeblendeter Touch-Oberfläche lösen Maus-/emulierte Touch-Mausklicks keine zusätzlichen Angriffe aus.

## Integration

`MobileTouchControls` im gespeicherten HumanoidCombatLab erstellt einen kleinen Canvas mit Unitys vorhandenen `OnScreenStick`/`OnScreenButton`, `GraphicRaycaster`, `EventSystem` und `InputSystemUIInputModule`. Alle Aktionsbuttons speisen bereits vorhandene Gamepad-Bindings. Keine zweite Phasen-/Schadenslogik. LabInput führt nur die Pointer-Sperre und den äußeren Stickbereich als Rennen hinzu. Fokusverlust, Pause und Ausblenden geben virtuelle Eingaben frei.

Der Canvas berücksichtigt Screen.safeArea und skaliert nach Höhe im Querformat. `Assets/WebGLTemplates/TouchDuel/index.html` passt das Spielfeld an den Browser an, berücksichtigt CSS-Safe-Area und verhindert Scroll-/Kontextmenügesten auf dem Spielfeld. Pixeldichte ist auf mobilen Browsern auf 1,5 begrenzt. Der bestehende Builder nutzt das Template vorübergehend und stellt die Projekteinstellung anschließend zurück.

Nach komplettem Szenen-Neuaufbau: HumanoidCombatBuilder → CharacterActionBuilder → BodyRecoveryBuilder → JunkyardPreviewBuilder → DuelSliceBuilder.Apply → **MobileTouchBuilder.Apply** → **ShoulderChargeBuilder.Apply** → **EnemyRolesBuilder.Apply**. Editor-Menü für Touch: **Wombat Lab → Apply Mobile Touch**.

## Handy im selben WLAN

WebGL-Build über **Wombat Lab → B3c Build WebGL** erstellen. `tools/Serve-Duel.ps1 -Lan -Port 8766` starten und am Handy `http://<IPv4-Adresse dieses PCs>:8766` öffnen. Beide Geräte müssen denselben PC erreichen können; falls Windows eine Firewallabfrage zeigt, den Zugriff im privaten Netzwerk erlauben. Der normale Serverstart ohne `-Lan` bleibt nur lokal erreichbar.

## Öffentlicher Play-Link und Updates

[Schrotthof-Duell spielen](https://emfau88.github.io/MoreThanWombarUnity/). Der Link steht auch oben im README. Android im Querformat ist das erste Ziel; es ist eine Browserfassung.

GitHub Pages verwendet die bereits gewählte Quelle **GitHub Actions**. `.github/workflows/play.yml` lädt die Teile `webgl-duel.zip.part-*` aus einem veröffentlichten Release, setzt das Archiv zusammen, entpackt den Spielstand und veröffentlicht ihn mit den offiziellen Pages-Actions. Die 5-MiB-Teile umgehen die hier beobachteten Timeouts beim großen Upload. Unity wird lokal mit der vorhandenen Installation gebaut; keine Unity-Lizenz oder neue Build-Infrastruktur auf GitHub erforderlich. Builds bleiben außerhalb der Git-Historie.

Für ein Update: WebGL bauen, kurz prüfen, Änderungen committen und pushen. Danach `tools/Publish-Duel.ps1 -Tag <neue-version>` ausführen. Das Skript verpackt ausschließlich die WebGL-Ausgabe, ergänzt `version.json` mit Commit/Buildzeit und veröffentlicht einen Release samt Archiv. Anschließend startet es den Pages-Workflow auf `main` mit diesem Release-Tag. Bestehende Releases können über **Actions → Publish playable WebGL duel → Run workflow** mit dem Release-Tag erneut veröffentlicht werden.

## Nachweis

Drei gezielte PlayMode-Prüfungen bestehen: Stick und separater Angriffs-Pointer funktionieren gleichzeitig; alle fünf Aktionen samt Neustart benutzen die bestehenden Bindings und erzeugen keinen zusätzlichen Mausschlag; Fokusabbruch und Ausblenden lösen gehaltene Controls. Ergebnis: `tools/mobile-touch-results.json`. Tastatur-Heavy und Desktop-Mausangriff nach Ausblenden sind darin ebenfalls geprüft.

Der neue WebGL-Build ist erfolgreich (66.424.982 Bytes); Bericht: `tools/mobile-touch-webgl-results.json`. Der Bericht enthält 354 Warnungen und eine Fehlermeldung; der tatsächliche Buildstatus lautet Succeeded. Unity meldete unter anderem einen beim Shutdown beendeten Worker. Im geladenen Browser treten die bekannten AppUI-/URP-FSR-Initialisierungswarnungen auf, keine Konsolenfehler. Die 900×420-Querformatansicht zeigt das Duell samt Controls; Neustart stellt 100/80 HP her und der Sprung-Button hebt die Figur sichtbar ab. Der bestehende Windows-Build bleibt die B3c-Fassung vor Touch.

Erstveröffentlichung am 5. Oktober 2026 erfolgreich: Release `duel-touch-2026-10-05`, [Pages-Workflow](https://github.com/emfau88/MoreThanWombarUnity/actions/runs/37365892811) abgeschlossen. Der öffentliche Play-Link lädt das Duell samt Touch-Controls, ohne Konsolenfehler. `version.json` nennt Spielcommit `5b2a273`; spätere Commits betreffen nur Upload/Workflow und Dokumentation. Screenshot lokal unter `UnityProject/Assets/QA/mobile-touch-public.png`. Die Release-Teile wurden gegen das vollständige Archiv mit SHA-256 geprüft; der Workflow entpackt das Archiv erfolgreich.

Desktopbrowser-/simulierte Pointer-Prüfung ist kein physischer Android-/iPhone-Nachweis. Der tatsächliche Handytest bleibt offen: gleichzeitig bewegen und angreifen, springen, ausweichen, Neustart und Wechsel zurück in den Browser prüfen. Eine mobile Steuerung allein garantiert keine bestimmte Handy-Framerate.
