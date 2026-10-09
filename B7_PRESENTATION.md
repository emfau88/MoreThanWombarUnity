# B7 — Schrotthof-Präsentation und Audio

Stand: 8. Oktober 2026. B7 liegt lokal in der vorhandenen `JunkyardChapter`-Szene. Kein neues Figuren-Rig, keine Änderung an Wellen, HP, Schaden, Kontaktzeiten, Bewegung oder Checkpoint-Regeln. Das HumanoidCombatLab bleibt als gespeicherte Vergleichsszene erhalten. Öffentlicher Play-Link und vorhandene Builds bleiben bis zum nächsten Release auf dem früheren Touch-Stand.

## Sichtbare Änderungen

- **Anlieferung:** kräftiger Teal-/Rost-Kontrast, lesbarer Schildträger, Ladeplatz und gefasste gelbe Markierungen.
- **Sortierhof:** drei Regalreihen mit vorhandenen Blech-/Reifenmodellen und Sortierbeschriftung; kleinere Container an den Rändern geben den Regalen die Hauptsilhouette. Kühleres Arbeitslicht.
- **Presswerk:** zentrale Presse mit Sockel, Hydraulikdetails, Warnstreifen, „HÄNDE WEG“-Schild und langsam bewegtem Stempel. Warmer Lichtschwerpunkt; die Presse bleibt Hintergrunddekoration.
- **Durchgehender Boden und Tiefe:** eine neue gemalte Beton-Farbtextur statt der fotografischen Diffuse-/Normal-Kombination; eine gemeinsame Fläche entfernt die vorher überlappenden Raumquads. Bestehende Garagenmodelle als entfernte Lagerhallen geben Tiefe. Materialien und Farbkorrektur verbinden alle drei Bereiche.
- **Kampfsicht:** etwas höhere Kamera `(0, 9.2, -12.8)`, 42° FOV und ruhige Nachführung; kürzere vordere Torpfosten. Schmaler Spieler-Bodenring, kleinere Rollennamen und kompakte Gegner-HP-Balken. Namensmarker verschwinden bei Tod. Die vorhandene Warnung/Agile-Spur bleibt erhalten.
- **Tore:** orange Statuslampe bei geschlossenem Tor, grün bei offenem Tor. Keine zusätzliche kollidierende Dekoration.
- **Bärengesichter:** vorhandene glatte Grundformen behalten, bisher einheitliche Stahlflächen auf warme Körper-/Bauch-/Schnauzenfarben, helle Augen, dunkle Pupillen/Brauen/Nase und Innenohren aufteilen. Die drei Handschuhfarben erhalten die Rollenerkennung. Keine neue Figur oder Rig-Pipeline.

Die vorhandenen Kenney-Props sind weiterhin vereinfachte Modelle. Die Farb-, Material- und Lichtabstimmung führt die Welt in Richtung illustrativer Cartoon-/Comic-Präsentation; sie ist kein Austausch aller Modelle durch finale hochwertige Cartoon-Assets. Mensch-gegen-Bären bleibt die aktuelle Figurenbasis. Ein endgültiger Wombat und dessen Stilabnahme bleiben separat offen.

## Audio und Effekte

Zwölf kurze CC0-Kenney-Clips aus Impact Sounds und Interface Sounds, mit Original-Lizenzen und Archivhashes im [Lizenzregister](THIRD_PARTY_LICENSES.md). Drei wechselnde Beton-Schritte folgen zurückgelegter Strecke bei tatsächlichem Gehen/Rennen; keine Schritte in Luft, Angriff, Ausweichen oder Knockdown. Leichter/schwerer Punch ersetzt im Kapitel den synthetischen Kontaktton. Das Labor nutzt weiterhin den bisherigen Fallback.

Standard-, Agile- und Heavy-Warnungen unterscheiden Clip/Pitch und starten beim Übergang in TELEGRAPH. Fallen/Tod erhalten einen kurzen dumpfen Klang. Neue Welle, Bereichssieg, nahes Schalteröffnen und Kapitelabschluss haben kurze Signale. Nahe dem Presswerk bleibt ein leiser mechanischer Rhythmus hinter dem Kampf. Kein neuer Musiktrack oder breites Ambience-Pack. Kontaktblitz/Strahlenstern und Angriffswarnungen bleiben an ihre bestehenden Kampfereignisse gebunden; Bodenring, HP-Balken und Torlampen ergänzen die Lesbarkeit.

## Wiederverwenden und bearbeiten

`Wombat Lab/B7 Dress Chapter and Audio` wendet den B7-Pass auf die gespeicherte `JunkyardChapter` an. B7-Dekoration und lokale Material-/Volume-Ableitungen werden wiederhergestellt; bewusst ausführen, wenn manuelle Änderungen darin ersetzt werden dürfen. Ein erneuter B6-Neuaufbau erzeugt wieder die Grundfassung; anschließend B7 anwenden. Eigene Assets unter `Assets/Game/Environment/JunkyardChapter/`, Original-Audio unter `Assets/ThirdParty/Kenney/`.

`ChapterPresentation` beobachtet Kapitel-/Motorzustände für Schritte, Signalsounds, Torlampen, Spieler-Bodenring und Presse. `EnemyPresentation` hängt an den drei vorhandenen Rollen-Templates und wird lokal mit jeder Welle geklont; es ergänzt HP-Balken, Namenssichtbarkeit und Warn-/Fallklang. `CombatFeedback` besitzt optionale externe Kontaktclips und erhält den synthetischen Fallback für andere Szenen. Keine zusätzliche Kampfzustandsmaschine.

Gemalter Boden: [ConcretePainted.png](UnityProject/Assets/Game/Environment/JunkyardChapter/Textures/ConcretePainted.png), erzeugt mit dem eingebauten ImageGen-Tool. Finaler Prompt und Herkunft stehen vollständig in [Textures/SOURCE.md](UnityProject/Assets/Game/Environment/JunkyardChapter/Textures/SOURCE.md). Die Datei wurde unverändert in das Projekt kopiert; Unity importiert sie mit höchstens 1024 Pixeln, Repeat und Mipmaps.

## Übergabe

**2/2 vorhandene gezielte PlayMode-Fälle bestanden:** tatsächlicher Standard-/Agile-Kontaktkampf im zweiten Bereich samt Touch-Torbedienung (28,36 s) und Checkpoint-Retry nach Tod mit HP/Fortschritt/Tor/frischer Gruppe (14,80 s). Ergebnisse unter `tools/b7-contact-regression-results.json` und `tools/b7-checkpoint-regression-results.json`. Keine neue Testsuite für Farben/Dekoration.

Ein tatsächlicher Durchlauf mit unveränderten Kampfregeln gewinnt alle neun Wellen in 94,8 s mit 90 HP (`tools/b7-first-combat-run.txt`). Die abschließenden Boden-/Hintergrund-/Bärenfarbänderungen betreffen nur Darstellung. Finale Spielkamerabilder für alle drei Bereiche und Touch bei 1280×600 angesehen; diese letzten Bilder sind ausdrücklich gestellte Layoutansichten, kein zweiter vollständiger Kampf. Details: [tools/b7-visual-review.md](tools/b7-visual-review.md).

Kurzer tatsächlicher Unity-Audioausgabefragment-Nachweis: 0,512 s Stereo, Peak 0,516, RMS 0,0853, keine geclippten Samples. Die begrenzte 12-s-Aufnahmesitzung liefert in diesem Editor nur diesen kurzen fragmentarischen Output; keine durchgehende 12-s-Aufnahme und keine hörbare Qualitätsabnahme daraus ableiten. `tools/b7-audio-output.txt`, lokaler WAV unter `UnityProject/Assets/QA/b7-actual-audio.wav`.

Finale gespeicherte Kapitel-Szene frisch geladen: Play/Compile/Dirty false, scriptCompilationFailed false, null fehlende Materialien, null neue Dressing-Collider, sieben Torlampen und drei Rollen-Presentation-Templates. Synthetische Labor-Fallbacks erhalten. Keine neue Build-/Android-/Gamepad-/FPS-/Speicherabnahme. Das 10–15-Minuten-Ziel bleibt offen; mehr abwechslungsreiche Situationen/Spielerfeedback gehören zum weiteren Umfangstuning.

B8 ist als Nächstes Menü/Pause, Einführung, HUD, Optionen und Ergebnisablauf. Neue Windows-/WebGL-Veröffentlichung und echte Geräte-/Performancebeurteilung bleiben separate Übergabeschritte.
