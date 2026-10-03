# More Than Wombat — Roadmap mit neun Bulks

Stand: 4. Oktober 2026, nach B2-Schritt 3. Diese Fassung ersetzt die bisherige Vorwärtsplanung zu eigenem Modell-/Rig-Bau und spätem Einzelduell. Verbindliche Gestaltung/Arbeitsweise: [PRODUCTION_GUIDELINES.md](PRODUCTION_GUIDELINES.md). Konkrete nächste Aufgaben: [NEXT_STEPS.md](NEXT_STEPS.md). Tatsächliche Nachweise: [TEST_SLICE_STATUS.md](TEST_SLICE_STATUS.md).

## Ziel und Schwerpunkt

Ein charakterstarker 3D-/2.5D-Brawler im illustrativen Cartoon-/Comic-Stil der 2D-Referenz: runde kräftige Figuren, expressive Gesichter, glatte Formen, gemalte Materialdetails und klar lesbare Treffer. Sichtbare Low-Poly-Facetten sind kein Stilziel.

Zuerst ein vollständiges überzeugendes Duell mit einem Spieler und einem einfachen Gegner. Danach Ausbau zum ersten Junkyard-Kapitel mit etwa 10–15 Minuten Spielzeit als Designziel. Windows dient der direkten lokalen Übergabe; WebGL-Verträglichkeit wird bereits am kleinen Duell praktisch geprüft. Zusätzliche Plattformen/Touch-Steuerung sind daraus nicht automatisch beauftragt.

Bestehendes Gameplay weiterentwickeln. Standardmodelle, Rigs, Animationen und Props zunächst aus geeigneten fertigen Grundlagen wählen. Individuelle Arbeit konzentriert sich auf Wombat-Identität, Bewegung, Kampfregeln, Reaktionen und Balance. Die neun Bulks sind unterschiedlich groß; keine Zeit- oder Aufwandsgarantie.

## Stand und nächster Schritt

S0–S3 bilden das vorhandene Fundament. B1 ist technisch abgeschlossen: Startup-Drehgrenze korrigiert, fünf gezielte Polish-Fälle bestanden, aktuelle Spielposen kontrolliert und Combat-/Architekturbeschreibung aktualisiert. Nutzerfeedback zum Spielgefühl bleibt für weiteres Tuning willkommen. Ein eigener Blender-Entwurf wurde begonnen, ist aber noch nicht in Unity integriert. Dieser Pfad ist zurückgestellt, bis passende fertige Grundlagen geprüft sind.

Mit „go b1“ wurde der B1-Abschluss erledigt. B2-Schritte 1–3 liefern drei dokumentierte Kandidaten, regulären Humanoid-Import und ein separates spielbares HumanoidCombatLab mit konkreten Attack-Zeiten und retargeteten Kontaktbahnen. [B2_COMBAT_INTEGRATION.md](B2_COMBAT_INTEGRATION.md) beschreibt den aktuellen Stand. Wombat-Look bleibt offen; die stilistische B2-Abnahme ist damit nicht abgeschlossen. Nächste technische Arbeit ist B3: Run, Kernclips und sichtbare Trefferreaktionen, danach Knockdown/GetUp und Duell-Nachweis. Die eigene JSON-Mesh-Pipeline bleibt zurückgestellt.

## Übersicht

| Bulk | Konkrete Maßnahmen | Ergebnis / Voraussetzung |
| --- | --- | --- |
| B1 | Abgeschlossen: Drehgrenze korrigiert, gezielte Polish-Prüfung/Sichtsequenz und Ist-Dokumentation | Spielbares Sparring zur Nutzerbeurteilung |
| B2 | Schritte 1–3 technisch geliefert: Auswahl, Humanoid-Import, Kontakte und Attack-Timing; Wombat-Gestaltung zurückgestellt | Spielbare technische Combat-Basis; stilistische Abnahme offen |
| B3 | Als Nächstes: Run/Kernclips/Reaktionen → Knockdown/GetUp/Death → Duell-Tuning und kleiner Buildnachweis | Überzeugender Character Slice vor Contentausbau |
| B4 | Einen charakteristischen Dash-/Schulterstoß und Moveset-Balance ausarbeiten | Mehr Kampftiefe auf Basis des fertigen Duells |
| B5 | Standard, Agile und Heavy; gemischten Gruppendruck und gemeinsame Asset-Nutzung | Lesbare Gegnerfamilie auf Basis B3/B4 |
| B6 | Junkyard-Strecke, drei Kampfbereiche, Interaktion, Checkpoints und Abschlusskampf | Vollständig durchspielbares Graybox-Kapitel |
| B7 | Passende Environment-Packs ableiten; Comic-Materialien, Kamera, Licht, Sound/VFX abstimmen | Zusammenhängende Präsentation im Referenzstil |
| B8 | Einführung, HUD, Menü/Pause, Optionen, Retry und Ergebnis | Vollständige selbst erklärende Spielschleife |
| B9 | Balance, tatsächliche Performance, Windows-Demo und WebGL-Slice prüfen | Spielbare Demo mit konkreten Laufzeit-/Buildnachweisen |

Jeder Bulk endet integriert und spielbar. Grundlegendes Feedback, Kamera und Bedienbarkeit wachsen ab dem Duell mit; B7/B8 arbeiten sie aus. Fertige Pakete werden nur für die tatsächlich benötigten Teile übernommen.

## B1 — Combat-Polish abgeschlossen

Bereits umgesetzt: kontrollierte Angriffsschritte, frühere Light-Anschlüsse nach Treffer, begrenzte Startup-Ausrichtung, Bewegungsfreigabe in später Recovery, Kick auf L/RB, Air-Kick/Air-Smash, deutlichere Locomotion und Ganzkörper-Tod mit Collider-Abschaltung/Reset.

Abschluss:

- Doppelte Startup-Drehkorrektur auf einen gemeinsamen 25°-Rahmen begrenzt; Richtung bis zur späten Recovery fest.
- Fünf gezielte PlayMode-Fälle bestanden, einschließlich Drehgrenze und Bewegungsfreigabe vor Attack-Ende. Keine neue Vollsuite.
- Echte Kamerasequenz mit Walk, Kick, Jump-Hit und Ganzkörper-Tod angesehen; Reset/Input-Rückgabe geprüft.
- COMBAT_SYSTEM und ARCHITECTURE beschreiben aktuelle Boden-/Luftangriffe, Fußkontakte, Schritt-/Recovery-Regeln und Gegner. B2-Timing/Retargeting klar als geplant abgegrenzt.

**Übergabe:** B1 ist technisch geprüft und korrekt dokumentiert. Nutzer-Spielgefühl wird daraus nicht als abgenommen behauptet; konkrete Rückmeldung kann weitere Werteanpassungen begründen. Der [Combat-Polish-Plan](COMBAT_POLISH_PLAN.md) beschreibt den ursprünglichen Implementierungsumfang; der Status dokumentiert die tatsächlichen Nachweise. Nächste Umsetzung ist B2.

## B2 — Asset-Auswahl und spielbare Charakterbasis

Detaillierter Ablauf und technische Arbeitspakete stehen in [NEXT_STEPS.md](NEXT_STEPS.md).

Schritte 1–3 geliefert: `CharacterImportLab` bleibt als Bewegungstest erhalten; `HumanoidCombatLab` ergänzt bestehendes Sparring, eigene Timingdaten und echte Avatar-Kontaktbahnen. Der fehlende Kick/Luftkick wurde lokal am Humanoid ergänzt. Finisher/Heavy/Air-Heavy verwenden vorläufige Cross-Varianten. Der temporäre Mensch ist keine endgültige Charakterentscheidung; Wombat-Anpassung bleibt offen.

- Höchstens drei rigged Cartoon-Kandidaten vergleichen: Wombat-Eignung, glatte Silhouette, Gesicht, Avatar/Retargeting, Clipumfang, Lizenz, Anpassungsaufwand und Laufzeitkosten.
- Eine primäre Animationsbasis und höchstens eine Ergänzungsquelle wählen. Kostenlose Inhalte konkret prüfen; kostenlose Pack-Versionen nicht mit kompletter Bibliothek gleichsetzen.
- Die empfohlene Kombination importieren, Originale in ThirdParty erhalten, Lizenznachweise registrieren und eigene Prefabs/Materialien/Controller ableiten.
- Metermaßstab, Forward-Achse, Avatar, Bodenhöhe und Hand-/Fußkontakte korrekt zuordnen. Idle/Walk/Punch/Kick im echten Charakter prüfen.
- AttackDefinition um gewünschte Phasendauern/Clip-Mapping ergänzen. Eine gemeinsame Phasenquelle halten; vorhandene Regeln beibehalten.
- Direkte Transform-Clip-Samples nicht ungeprüft für Humanoid-Retargeting weiterverwenden. Retargetete Kontaktbahn am Avatar prüfen und die kleine nötige Pose-Anbindung bauen.
- Importierte Root Motion und bestehende Motorbewegung nicht doppelt anwenden.
- Eigene Anpassungen auf Wombat-Merkmale und klar identifizierte Lücken begrenzen. Ein technisches Human-Testmodell ist keine fertig gestaltete Wombat-Figur.

**Fertig:** passende Charakterbasis ist im Sparring spielbar, Import/Avatar funktionieren, vier Kernclips und Kontakte sind integriert, Reimport erhält Referenzen, Quellen/Lizenzen sind dokumentiert. Falls passende Eigenanpassung einen Kauf/externen Auftrag erfordert, konkrete Auswahl und Lieferumfang vorlegen.

## B3 — vollständiges überzeugendes Charakterduell

Run, animierte Trefferreaktion und lebendes Knockdown/GetUp werden gegenüber der früheren Planung vorgezogen.

Konkrete Ausführungsreihenfolge, jeweils als spielbarer Zwischenstand:

1. **B3a — Bewegung und Aktionen:** Run samt Eingabe/Übergängen integrieren; Walk-/Run-Tempo abstimmen; vorläufige Finisher-/Heavy-/Air-Heavy-Cross-Varianten durch unterscheidbare passende Aktionen ersetzen. Sichtbare Treffer-/Stagger-Reaktionen für Spieler und einen Gegner anbinden. Ergebnis: Bewegung und Schlagstärke sind klar erkennbar.
2. **B3b — Fallen und Aufstehen:** Lebendes Knockdown, GetUp und endgültige Death/Reset-Darstellung integrieren; Unterbrechung, Körperkollision und Schutz beim Aufstehen klar regeln. Ergebnis: kräftige Treffer erzeugen einen vollständigen verständlichen Ablauf statt bloßer HP-Abzüge.
3. **B3c — Duell und Übergabe:** Abstand, Combo-Anschlüsse, Luftkontakte, Recovery und Feedback gemeinsam tunen; kleinen Windows- und WebGL-Nachweis mit vorhandener Toolchain ausführen. Ergebnis: vollständig spielbarer Einzelkampf mit konkretem Build-/Laufzeitbefund.

Wombat-Look bleibt eine separate offene Entscheidung. Die technische B3-Arbeit kann am temporären Humanoid fortgesetzt werden; die endgültige stilistische Character-Slice-Abnahme benötigt später die passende Tierdarstellung.

- Ein Spieler, ein Gegnertyp; vorhandene Trainingsszene und Zwei-Gegner-Option erhalten.
- Idle, Walk, Run, Jump, Fall, Land, drei Lights, Heavy, Kick, Air-Attacks, Evade, Flinch/Stagger, Knockdown, GetUp und Death integrieren.
- Fertige Clips kürzen/abstimmen, Kontaktmoment an Active binden und Übergänge aus der Spielkamera beurteilen. Jede Combo-Stufe bleibt eine eigene Instanz.
- Direkte Eingaben und schnelle Richtungswechsel, Fußtempo, Luftkontrolle, Angriffsschritte, Recovery und Treffer-/Fehlschlag-Cancels gemeinsam tunen.
- Sichtbare Reaktion, Knockback und sauberes Aufstehen mit begrenztem Schutz; KO/Tod/Reset eindeutig.
- Einen kleinen Windows- und WebGL-Build des Duells anstreben; vorhandene Module prüfen. Browserfokus, Eingaben, Audio nach Interaktion und Restart kontrollieren; Ladegröße, Framezeit und Speicher grob messen.

**Fertig:** der Einzelkampf überzeugt praktisch in Steuerung, Treffergefühl, Darstellung und Reaktionen. Eine geeignete Asset-Basis allein ist kein Qualitätsnachweis. Danach sind Gegner-/Level-Erweiterungen sinnvoll.

**Prüfung:** konkrete neue Timing-/Retargeting-/Knockdown-Regeln, eine vorhandene Regression nach Integration und eine kurze tatsächliche Spiel-/Buildkontrolle. Keine große FPS-/Gerätematrix ohne Anlass.

## B4 — Kampftiefe und Wombat-Move

- Einen kontrollierten Dash-Angriff beziehungsweise Schulter-/Kopfstoß aus einer passenden Basis ableiten. Klare Reichweite, Kollisionsbegrenzung und bestrafbare Recovery.
- Rollen abstimmen: Light für Druck/Combo, Heavy für kräftigen Abschluss, Kick für Abstand, Air-Attack für Sprungangriff, Evade für Positionierung.
- Trefferstärke/Knockdown-Wirkung differenzieren. Dauerstun und endlose Cancel-Schleifen vermeiden.
- Special-/Launcher-/Guard-Systeme nur ergänzen, wenn eine konkrete Spielentscheidung fehlt; nicht mehrere neue Systeme gleichzeitig.

**Ergebnis:** ein charakteristischer eigener Move und ein Moveset mit mehreren praktisch nützlichen Entscheidungen. Dash-/Wandkontakte und Unterbrechung gezielt prüfen.

## B5 — Gegnerrollen und Gruppenkampf

- Standard mit kurzer Nahkampfaktion und offener Erholung; Agile mit angekündigtem geradlinigem Ansturm nach Seitenwechsel; Heavy mit breiter langsamer Attacke und gegebenenfalls sichtbarer brechbarer Rüstung.
- Passende fertige Modelle und denselben geprüften Rig-/Animationspfad nutzen, Rollen über Silhouette/Haltung/Aktion unterscheiden.
- EngagementCoordinator um sinnvolle Positionen und begrenzte Angriffserlaubnisse für drei bis vier sichtbare Gegner erweitern.
- Klare Eintritte, Abstand, Repositionierung und faire Offscreen-Regeln.

**Ergebnis:** Einzelduelle sind verständlich, gemischte Gruppen verändern Prioritäten und Positionierung. Wenige repräsentative Kombinationen und Token-/Unterbrechungsfälle genügen.

## B6 — Junkyard-Kapitel

- Kurze zusammenhängende Graybox-Strecke mit drei Kampfbereichen, Bewegungs-/Erholungsabschnitten und klaren Übergängen.
- Standardgegner erklären den Einstieg; Agile/Gruppen steigern Druck; abschließend ein besonders inszenierter Heavy-Elitegegner.
- Encounter-Zustände, Spawnpunkte und Wellenfolge als einfache bearbeitbare Daten.
- Eine Environment-Interaktion, Checkpoint-Retry, Niederlage und Abschluss integrieren.
- Auf etwa 10–15 Minuten abwechslungsreichen Ablauf abstimmen; zusätzliche HP ersetzen keine interessanten Situationen.

**Ergebnis:** ein kompletter Durchlauf. Einen Durchlauf plus gezielten Checkpoint-Retry prüfen. Ein mehrphasiger eigener Boss gehört zur späteren Erweiterung.

## B7 — Cartoon-Präsentation der Welt

- Eine stilistisch passende modulare Environment-Basis auswählen. Rost/Schrott, gemalte Details und glatte Hauptformen über eigene Material-/Textur-Ableitungen angleichen; keine wahllose Packmischung.
- Warme Lichtinseln/kühle Tiefe, Bodenkontakt und lesbare Figuren aus der Referenz in 3D übertragen.
- Kamera an Bereichswechsel und Gegnerverteilung anpassen; Verdeckungen und unruhige Bewegung vermeiden.
- Passende lizenzgeklärte Audio-/VFX-Grundlagen für Schritte, Kontakte, Warnungen und Umgebung nutzen.
- Kleine koordinierte Signale für Light/Heavy/Kick/KO; humorvolle Reaktionen an vorhandenen Aktionen ausarbeiten.
- Optionen für günstige Schatten/Effekte anhand der frühen WebGL-Messung wählen.

**Ergebnis:** ein konsistentes Comic-Kapitel, lesbar auch im Gruppenkampf. Eine dichte Sequenz mit Ton/Präsentation ansehen und konkrete Engpässe messen.

## B8 — Bedienung und vollständige Sitzung

- Startmenü, Pause, Optionen, Niederlage/Retry, Kapitelabschluss; klare Tastatur-/Gamepad-Navigation.
- Kompaktes HUD, passende Eingabehinweise, kurze überspringbare Einführung in Grundaktionen.
- Lautstärken, Bildschirmdarstellung und reduzierte Kamerabewegung; Einstellungen speichern.
- Checkpoint- und vollständigen Neustart anbieten; wenige verständliche Ergebniswerte.
- Browserfokus und Audiofreigabe in den WebGL-Ablauf einpassen, ohne die Spieloberfläche mit Implementierungsdetails zu belasten.

**Ergebnis:** Start → Lernen → Kampf → Pause/Retry → Abschluss funktioniert ohne Entwicklererklärung. Ein Durchlauf aus Sicht eines neuen Spielers.

## B9 — Balance, Performance und Demo

- Wenige vollständige Durchläufe für häufige Kontaktprobleme, dominante Moves, Leerzeiten und frustrierende Checkpoints.
- Schaden, HP, Gegnerdruck, Spawnfolge und Recovery gemeinsam abstimmen.
- Ziel 60 FPS auf dem tatsächlichen Test-PC; Browserleistung separat messen. Schatten, Materialien, Textur-/Speicherbedarf und Effekte nach konkreten Befunden optimieren.
- Windows-Demo und WebGL-Fassung prüfen, soweit die Toolchain verfügbar ist. Einschränkungen konkret dokumentieren.
- Einmal vorhandene Regression für den Kandidaten und kurze tatsächliche Buildkontrolle; nach Änderungen nur betroffene Prüfungen wiederholen.
- Build, kurze deutsche Anleitung, Lizenznachweise und wenige konkrete nächste Erweiterungen übergeben.

**Ergebnis:** spielbare Demo und wiederverwendbare Produktionsbasis. Weitere Abschnitte, ein eigener Boss oder ein zweiter Charakter folgen aus dem Nutzerfeedback.

## Ausführung und Grenzen

Normale lokale Entscheidungen selbst treffen. Keine wiederholten Setup-Gates, Architektur für 20 Figuren oder Vollregression nach jedem Tuningwert. Fertige Grundlagen zuerst, Eigenarbeit auf notwendige Identität/Mechanik konzentrieren. Neue Ordner schrittweise nutzen; bestehende Assets/Referenzen nicht allein für Strukturarbeit verschieben.

Browsergame ausschließlich lesen. Vorhandene Unity-Änderungen erhalten. Kein Commit/Push, Asset-Kauf, neuer Dienst oder Editor-Upgrade ohne entsprechenden Auftrag. Ein Online-Account oder ungeklärte Lizenz ist eine konkrete Abhängigkeit; eine generische Vorsichtsannahme erzeugt keine neue Freigaberunde.

