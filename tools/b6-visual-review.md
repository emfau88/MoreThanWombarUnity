# B6 — Spielkameraprüfung

7. Oktober 2026, Unity Editor 6000.4.0f1 / Pipeline 0.8.0-exp.1 / CLI 1.0.0-beta.13.

Ein begrenzter tatsächlicher Durchlauf über `JunkyardChapterReview`: Bewegungs-/Combo-Befehle an bestehendem PlayerMotor/CombatController, unveränderte HP/Schaden/KI, keine Teleports oder direkten Schadensaufrufe. Der Helfer wählt den nächsten lebenden Gegner und richtet die Figur aus; das ist kein menschlicher Erstspieler-Lauf. Endzustand `Complete`, drei abgeschlossene Bereiche, 58 HP, 101,7 s. Vollständiges Wellen-/HP-Protokoll: [b6-combat-run.txt](b6-combat-run.txt).

Angesehen wurden die tatsächlichen Kamerabilder unter `UnityProject/Assets/QA/`:

- `b6-arrival.png`: Einstieg, cyanfarbener Checkpoint, lesbare Anlieferungskennzeichnung.
- `b6-area-1-wave-3.png`: Standard-Gruppe im ersten Kampfbereich.
- `b6-switch.png`: Laufabschnitt, gelber Torschalter, geschlossenes Verbindungstor; nahe Interaktion öffnet es im selben Durchlauf.
- `b6-area-2-wave-1.png`: folgende Kameraeinfassung, grüne Schalterlampe, Standard und Agile bei Weltposition x=24.
- `b6-area-3-wave-3.png`: Presse, vierte Rollen-Ableitung VORARBEITER, Standard/Agile-Begleiter und geschlossene Kampftore.
- `b6-run-end.png` und `b6-touch-complete.png`: geöffnete Tore, gewonnener Abschnitt; Touch-HUD mit CHECKPOINT/VON VORN, allen bisherigen Kampfaktionen, 58/100 HP und Kapitelabschluss.

Kameracaptures zeigen den aktuellen Spielzustand. Für das Touch-HUD wurde Canvas ausschließlich während der Aufnahme von ScreenSpaceOverlay auf ScreenSpaceCamera umgestellt, bei 1280×600 gerendert und anschließend vollständig zurückgestellt. Dieser technische Aufnahmeweg ersetzt den teilweise veralteten/fehlerhaften Pipeline-Screen-Frame; er ist keine Änderung an den gespeicherten UI-Einstellungen.

Erkenntnis: Das Kapitel funktioniert als kurzer zusammenhängender Kampf. Die drei Abschnitte verwenden erkennbar dieselbe Environment-Basis; Cartoon-Materialien, stärkere Ortsidentität und Audio bleiben B7. Das geplante 10–15-Minuten-Kapitel ist noch nicht erreicht. Umfang und abwechslungsreiche Situationen anhand von Spielerfeedback erweitern, keine reine HP-Verlängerung.

Gezielte Regelnprüfung: 11/11 Fälle bestanden, vier Ergebnisdateien `b6-chapter-results.json`, `b6-input-results.json`, `b6-duel-regression-results.json`, `b6-group-regression-results.json`. Kein erneuter Gesamt-Testlauf, kein neuer Windows-/WebGL-Build, kein physischer Android-/Gamepad- oder FPS-/Speicher-Nachweis.
