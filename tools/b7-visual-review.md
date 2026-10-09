# B7 — Darstellung und gezielte Integration

8. Oktober 2026, Unity 6000.4.0f1 / Pipeline 0.8.0-exp.1 / Unity CLI 1.0.0-beta.13. Keine neue Toolchain oder Rig-/Animationspipeline.

## Tatsächlicher Kampf und Regeln

Ein zusammenhängender echter Motor-/Combo-Durchlauf über dieselben B6-Regeln gewinnt neun Wellen/23 Gegner: Complete, drei Bereiche, 90 HP, 94,8 s. Keine HP-/Schadensänderungen, Teleports oder direkten Schadensaufrufe im Kampf. Bericht: [b7-first-combat-run.txt](b7-first-combat-run.txt). Die anschließenden gemalten Boden-/Hintergrund- und Bärenmaterialänderungen sind ausschließlich Darstellung.

Zwei vorhandene Fälle, beide bestanden: tatsächlicher Mischkampf mit Touch-Torinteraktion im Sortierhof (28,36 s) und Checkpoint-Retry nach Tod (14,80 s). Ergebnisdateien [b7-contact-regression-results.json](b7-contact-regression-results.json), [b7-checkpoint-regression-results.json](b7-checkpoint-regression-results.json). Keine Vollsuite und keine neuen Farb-/Deko-Unit-Tests.

## Angesehene Kameraansichten

Lokale Dateien unter `UnityProject/Assets/QA/`:

- `b7-before.png`: Ausgangsfassung am Einstieg.
- `b7-after.png`: erste Farb-/Kamera-/Schildfassung; Boden noch fotografisch, Hintergrundkante zu abrupt. Beides gezielt nachgearbeitet.
- `b7-area-2-wave-2.png`, `b7-area-3-wave-3.png`: tatsächlicher Kampf mit Regalen/Presse, Bodenring und HP-Balken während der ersten Fassung.
- `b7-final-arrival.png`: abschließender gemalter Boden und tiefere Lagerhallenstaffelung.
- `b7-final-area-1.png`, `b7-final-area-2.png`, `b7-final-area-3.png`: finale gestellte Layoutansichten aus der echten Spielkamera, einschließlich warmer Bärenkörper, unterscheidbarer Gesichtsflächen, Handschuhfarben und kompakter Marker. Für diese Sichtprüfung wurden Bereich/Welle und Spielerposition bewusst zur Laufzeit gesetzt; kein daraus behaupteter zweiter Sieg.
- `b7-final-touch.png`: gestellte Presswerk-/Finale-Ansicht bei 1280×600 mit Touch-Buttons, HP und Welle 3/3. Alle Canvas-Kamera-/RenderMode-Einstellungen nach der Aufnahme wiederhergestellt. Kamera-Canvas-Capture kann Textkanten/Glow anders darstellen als das native Overlay; keine physische Android-Abnahme.

Vorhandene Kenney-/M1-Props und glatte Bärengrundformen weiterverwendet. Boden ist ein einzelnes neues illustratives ImageGen-Asset, kein Verfahren zum Neumodellieren der Welt. Container, Regale und Presse haben unterschiedliche Hauptsilhouetten; endgültige hochwertigere Cartoon-Modelle und Wombat bleiben offen. Kein sichtbarer Bodentile-Rand in den final angesehenen Kampfansichten.

## Audioausgabe und Grenzen

Zwölf lizenzgeklärte CC0-Originalclips. Unitys tatsächliche Stereoausgabe enthält einen nicht stillen kurzen Ausschnitt: 49.152 Samples bei 48.000 Hz/2 Kanälen = 0,512 s; Peak 0,516, RMS 0,0853, null geclippte Samples. Die 12-s-begrenzte Editor-Aufnahmesitzung ist kein durchgängiger 12-s-Mitschnitt. Bericht: [b7-audio-output.txt](b7-audio-output.txt), WAV lokal `Assets/QA/b7-actual-audio.wav`. AudioRenderer danach gestoppt; normale Ausgabe wiederhergestellt. Keine hörbare Klang-/Lautstärkenabnahme daraus behaupten.

Finale Übergabe: gespeicherte JunkyardChapter frisch geladen, Play/Compile/Dirty false, scriptCompilationFailed false; null fehlende Materialien, null Collider in B7-Dressing, sieben Torlampen, drei Rollen-Presentation-Templates und zwölf Audio-Originale. Das Humanoid-Labor, Charakter-/Attack-/Wellen-/HP-/Checkpoint-Daten unverändert. Kein neuer Build, kein physischer Geräte- oder Performance-Nachweis; Dauer/Umfang zum 10–15-Minuten-Ziel weiter offen.
