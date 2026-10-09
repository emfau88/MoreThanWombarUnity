# B8 — vollständige Spielsitzung

Stand: 9. Oktober 2026. Lokal in der gespeicherten `JunkyardChapter` integriert. B7 ist bereits auf `main` gepusht; B8 hat noch keinen eigenen Commit oder veröffentlichten Build.

## Spielen

`UnityProject/Assets/Game/Scenes/JunkyardChapter.unity` öffnen und Play starten. **SPIELEN** startet beim ersten Mal drei kurze, überspringbare Einführungskarten. Danach beginnt das Kapitel an derselben Anlieferung wie bisher. Der erste Kampf startet erst beim Betreten der gelben Fläche.

- **Pause:** ESC/P, Gamepad Start oder der sichtbare PAUSE-Button. Fortsetzen bringt denselben Kampf zurück. Fokusverlust/App-Unterbrechung pausiert ebenfalls; Rückkehr setzt nicht ungefragt fort.
- **Steuerung:** Menü zeigt Tastatur-, Gamepad- oder Touch-Hinweise. Gameplay-Bindings bleiben gleich; Gamepad Start pausiert im Kapitel und setzt weiterhin im separaten Duell-Labor zurück.
- **Niederlage:** Ergebnisanzeige mit **AB CHECKPOINT**, **VON VORN** und **STARTMENÜ**. Retry benutzt die vorhandenen Einstieg-HP, Bereichs-/Torinformationen und frischen Gegnergruppen.
- **Sieg:** Bereiche, aktive Spielzeit und verbleibende HP; **NOCHMAL SPIELEN** oder **STARTMENÜ**. Das Startmenü kann eine laufende, lebende Sitzung fortsetzen.
- **Optionen:** Lautstärke 0–100 %, Kamerablick 38–48°, direkte/ruhigere Nachführung, Touch an/aus und kurze Steuerungshinweise an/aus. Speicherung über vorhandenes `PlayerPrefs` unter `Wombat.Chapter.*`; kein Konto oder Cloud-Speicher. Die Spielzeit pausiert mit dem Spiel und zählt Retry-Versuche weiter; Von-vorn setzt sie zurück.
- **Direkt im Kapitel:** R/Touch CHECKPOINT wiederholt den Abschnitt; Backspace/Gamepad Select/Touch VON VORN startet die Sitzung neu. F/LB/Touch TOR ÖFFNEN bleibt die nahe Schalteraktion.

Touch-Steuerung bleibt für Android-Querformat vorgesehen. Menüs verwenden Safe-Area-Anker und skalieren mit Bildschirmgröße. In Menüs verschwinden die Kampfcontrols und geben gehaltene virtuelle Eingaben frei. Menübuttons und Schieberegler nutzen Unitys vorhandene UI/EventSystem-Navigation.

## Integration

`ChapterSession` steuert ausschließlich Seiten, Zeit-/Audio-Pause, Optionen und vorhandene Kapitelaktionen. `JunkyardChapter` bleibt die einzige Fortschritts-/Checkpoint-Instanz. Kein neues Kampf-, Savegame- oder UI-Paket. `ChapterSessionBuilder.Apply()` / **Wombat Lab/B8 Apply Chapter Session** ergänzt die gespeicherte Kapitel-Szene und das kompaktere Kapitel-HUD. Nach vollständigem B6-Neuaufbau zuerst B7, dann B8 anwenden.

Pause setzt `Time.timeScale` auf null und pausiert Audio. Explizite Guards halten auch unskalierte Hitstop-/Kontaktfeedback-Zeit, Angriffssweeps und KI-Entscheidungen an. Der laufende Angriff wird erhalten; nur vorgemerkte Combo-/Sprung-Eingaben werden entfernt. `LabInput` sperrt Gameplay und wartet beim Fortsetzen auf das Loslassen gehaltenen Inputs. Damit wird ein Menü-Klick oder festgehaltener Stick nicht in eine neue Aktion übernommen. Test-Injektion bleibt der bestehende Motorpfad; auch dieser Motor hält in Menüs an. Zeit/Audio werden beim Szenenwechsel wiederhergestellt.

## Gezielte Prüfung

**2/2 B8-PlayMode-Fälle bestanden (4,89 s):** Pause während echter Attack-Progression mit gehaltenem virtuellen Stick/Combo, eingefrorenem Spieler/Gegner/HP/Spielzeit und ohne zusätzliche Attack-Instanz nach Resume; Gamepad Start → Pause, Optionsspeicherung, tatsächlicher Spielertod → Ergebnisbutton → Checkpoint und Wiederherstellung von Zeit/Audio beim Szenenwechsel. Ergebnis: `tools/b8-session-results.json`. Keine neue Vollsuite.

Startmenü, Steuerung, Optionen, erste Einführung und letzte Touch-Einführung als aktuelle Unity-UI angesehen. Menütexte/Buttons bleiben bei 1280×600 vollständig innerhalb der Karte. `LabVisualReview.CaptureHud` berücksichtigt Zielauflösung und responsive Skalierung; Kamera-/Canvas-/Scaler-Einstellungen werden danach wiederhergestellt. Die Overlay-UI wird für die Aufnahme einmal ohne Kamera-Postprocessing gerendert; das ersetzt keine Geräteprüfung.

Zusätzlich besteht der vorhandene FullRoute-Fall (39,87 s): alle neun Wellen/Schalter, Sieg-Ergebnis mit eingefrorener Zeit und Replay-Button zurück zum frischen Start. Dieser Ablauf setzt bewusst scripted Damage-Empfänger und ausgeschaltete KI ein; er ist kein tatsächlicher Combat-Sieg. Ergebnis: `tools/b8-chapter-flow-results.json`. Insgesamt drei gezielte Fälle bestanden. Ein tatsächlicher einfacher Combo-Bot verlor im zweiten Bereich nach 44,2 s; echte Niederlage und UI-Retry stellen Bereich 2, geöffnetes Tor und 42 Einstieg-HP wieder her. Sichtprüfung, Rohbericht und Grenzen: `tools/b8-visual-review.md`, `tools/b8-combat-run.txt`.

## Nächster Build und B9

Zur Übergabe ist der normale GUI-Editor wieder mit der gespeicherten Kapitel-Szene geöffnet: Play/Compile/Dirty false, keine Kompilierungsfehler, normale Zeit und Audio-Pause aus. Die Wiederherstellungsdatei wurde separat erhalten; die Szene bleibt die integrierte B7/B8-Fassung.

Der bestehende `DuelSliceBuilder` hat zusätzlich **B8 Build Chapter Windows/WebGL**. Er verwendet dieselbe Build-/Status-/Template-Technik, startet aber `JunkyardChapter` und schreibt nach `Builds/B8`. Die bisherigen B3c-Menüs starten weiterhin das Labor. WebGL-Ladeüberschrift/Canvas-Beschreibung verwenden jetzt den jeweiligen Produktnamen. Kein Build oder Release wird durch Anwenden von B8 ausgelöst.

Als Nächstes B9: Spielerfeedback für Schwierigkeit, Gruppen und Wege; abwechslungsreichen Umfang entscheiden; tatsächliche Framezeiten/Speicher auf Windows und Android-WebGL messen; gezielt ungenutzte Build-Abhängigkeiten prüfen und einen Kapitel-Build über den vorhandenen Veröffentlichungsworkflow ausliefern. Das 10–15-Minuten-Ziel, finale Figuren-/Modelle und hörbare Audioabnahme bleiben offen.
