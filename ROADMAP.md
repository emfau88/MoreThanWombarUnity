# More Than Wombat — Roadmap zur ersten LF2-inspirierten Stage

Stand: 9. Oktober 2026, B4–B8 integriert und gepusht; das B8-Kapitel ist über GitHub Pages veröffentlicht. Diese Fassung ersetzt die bisherige Vorwärtsplanung zu eigenem Modell-/Rig-Bau und spätem Einzelduell. Verbindliche Gestaltung/Arbeitsweise: [PRODUCTION_GUIDELINES.md](PRODUCTION_GUIDELINES.md). Konkrete nächste Aufgaben: [NEXT_STEPS.md](NEXT_STEPS.md). Tatsächliche Nachweise: [TEST_SLICE_STATUS.md](TEST_SLICE_STATUS.md).

## Ziel und Schwerpunkt

Ein charakterstarker 3D-/2.5D-Brawler im illustrativen Cartoon-/Comic-Stil der 2D-Referenz: runde kräftige Figuren, expressive Gesichter, glatte Formen, gemalte Materialdetails und klar lesbare Treffer. Sichtbare Low-Poly-Facetten sind kein Stilziel.

Aktuelles Ziel: ein vollständiger spielbarer Charakter mit drei unterschiedlichen Spezialfähigkeiten und eine zusammenhängende Stage mit etwa 30 Gegnern aus höchstens drei Typen. Massengefühl, klare Mehrfachtreffer und Charaktertiefe nach Little-Fighter-2-Vorbild stehen im Mittelpunkt. Desktop und die bereits integrierte Android-Touch-Steuerung gehören zur Übergabe. Acht bis zehn gleichzeitig aktive Gegner sind zunächst ein zu prüfendes Designziel. Das frühere 10–15-Minuten-Ziel entfällt; die Stage wird nicht durch zusätzliche Wiederholungen gestreckt.

Bestehendes Gameplay weiterentwickeln. Standardmodelle, Rigs, Animationen und Props zunächst aus geeigneten fertigen Grundlagen wählen. Individuelle Arbeit konzentriert sich auf Identität, Bewegung, Kampfregeln, Reaktionen und Balance. Verbindlicher Umfang, Gegnerverteilung, Abnahmekriterien und detaillierte nächste Aufgaben: [LF2_STAGE_PLAN.md](LF2_STAGE_PLAN.md). Die bisherige reine Balance-Planung ab B9 wird durch B9–B15 ersetzt. Ein KI-Verbündeter bleibt eine vorbereitete, gesonderte Erweiterung.

## Stand und nächster Schritt

S0–S3 bilden das vorhandene Fundament. B1 ist technisch abgeschlossen: Startup-Drehgrenze korrigiert, fünf gezielte Polish-Fälle bestanden, aktuelle Spielposen kontrolliert und Combat-/Architekturbeschreibung aktualisiert. Nutzerfeedback zum Spielgefühl bleibt für weiteres Tuning willkommen. Ein eigener Blender-Entwurf wurde begonnen, ist aber noch nicht in Unity integriert. Dieser Pfad ist zurückgestellt, bis passende fertige Grundlagen geprüft sind.

Mit „go b1“ wurde der B1-Abschluss erledigt. B2-Schritte 1–3 liefern drei dokumentierte Kandidaten, regulären Humanoid-Import und ein separates spielbares HumanoidCombatLab mit konkreten Attack-Zeiten und retargeteten Kontaktbahnen. B3a ergänzt gehaltenes Rennen, einen Haken-Finisher, Overhand-Heavy, zweihändigen Luft-Smash und sichtbare Hit-/Stagger-Reaktionen für Spieler und Robot-Gegner. Details: [B3A_ACTIONS.md](B3A_ACTIONS.md). Wombat-Look bleibt offen; die stilistische B2-Abnahme ist damit nicht abgeschlossen. B3b ergänzt lebendes Knockdown, GetUp, Aufstehschutz und Tod/Reset für beide Figuren; Details in [B3B_BODY_RECOVERY.md](B3B_BODY_RECOVERY.md). Die vorgezogene kleine Map-Aufwertung M1 und B3c sind integriert. Das technische Duell ist als Windows-/WebGL-Spielstand verfügbar; Browserkampf bis zum Sieg und Neustart sind nachgewiesen. B4–B6 sind lokal integriert. JunkyardChapter liefert drei verbundene Kampfbereiche, neun Wellen, Torschalter, Checkpoints und Vorarbeiter-Abschluss. B7 ergänzt lokal unterschiedliche Bereichssilhouetten, gemalten Boden, Kamera/Marker, warme Bärengesichter und zwölf CC0-Audiosignale. B8 ergänzt Menü, Pause, Einführung, Optionen und Ergebnis und ist öffentlich spielbar. B9–B12 sind inzwischen lokal integriert: Kampfvertiefung und 30-Gegner-Ablauf gemäß LF2_STAGE_PLAN. Als Nächstes folgt B13 mit der gemeinsamen Präsentationspolitur. Laufzeit und Balance werden anhand von Spielerfeedback weiter abgestimmt. Die eigene JSON-Mesh-Pipeline bleibt zurückgestellt.

## Übersicht

**Zusatzauftrag nach B3c:** Mobile Touch für Android im Querformat ist integriert: vorhandene Unity-Bildschirmcontrols, responsive WebGL-Ansicht und WLAN-Testanleitung. Drei gezielte Eingabeprüfungen bestehen. GitHub Pages veröffentlicht den fertigen WebGL-Spielstand über einen Release-Workflow; öffentlicher Play-Link im README. Ein physischer Android-Spieltest bleibt offen. B4–B8 sind integriert und als Kapitel am öffentlichen Play-Link verfügbar. [Release-Nachweis](B8_WEBGL_RELEASE.md). Details: [MOBILE_TOUCH.md](MOBILE_TOUCH.md).

**Vorgezogen auf Nutzerwunsch und geliefert:** **M1 — kleine Testmap-Aufwertung** nach B3b. Betonboden, wenige fertige Junkyard-Props, Zaun/Werkstatt-Hintergrund und Licht sind integriert. B3c ergänzt inzwischen das abgestimmte Duell und Buildnachweise. Details: [M1_MAP_PREVIEW.md](M1_MAP_PREVIEW.md). M1 übernahm einen begrenzten Präsentationsanteil aus B7, die damalige Levelstrecke blieb B6. Die neue Vorwärtsplanung umfasst B9–B15.

| Bulk | Konkrete Maßnahmen | Ergebnis / Voraussetzung |
| --- | --- | --- |
| B1 | Abgeschlossen: Drehgrenze korrigiert, gezielte Polish-Prüfung/Sichtsequenz und Ist-Dokumentation | Spielbares Sparring zur Nutzerbeurteilung |
| B2 | Schritte 1–3 technisch geliefert: Auswahl, Humanoid-Import, Kontakte und Attack-Timing; Wombat-Gestaltung zurückgestellt | Spielbare technische Combat-Basis; stilistische Abnahme offen |
| B3 | B3a–B3c und M1 technisch geliefert: Duell-Tuning, Schutz/Feedback/HUD, Windows/WebGL, Browserkampf bis zum Sieg | Spielbarer technischer Character Slice; Wombat-Stil, Windows-Tastatur, hörbare Audioabnahme und Performanceprofil offen |
| B4 | Lokal integriert: Schulterstoß aus Sprint-Ableitung, feste Richtung, Kontakt-/Wandstopp, verwundbare Erholung; Kick schafft mehr Platz; E/RT/Touch STOSS | Vier gezielte Stoß-Fälle bestehen; im öffentlichen Kapitel enthalten |
| B5 | Lokal integriert: Standard/Agile/Heavy, feste Rush-Spur, 1–4 Gegner, rotierende Freigabe, Wartepositionen/Körperkollision und Offscreen-Regeln | Mischkampf auf vorhandenen Figuren/Clips; im öffentlichen Kapitel enthalten |
| B6 | Lokal integriert: JunkyardChapter mit drei verbundenen Bereichen, neun Wellen/23 Gegnern, Schaltertor, Checkpoint-Retry und Vorarbeiter-Finale | Durchspielbares Kurzkapitel; automatischer Kampf 101,7 s, kein Nachweis menschlicher Spielzeit |
| B7 | Lokal integriert: Bereichssilhouetten, gemalter Boden, Material-/Licht-/Farbpass, höhere Kamera, Bodenring/HP/Torstatus, warme Bärengesichter und zwölf CC0-Signale | Erste gemeinsame illustrative Präsentation; finale Figuren-/Modelle und hörbare Audioabnahme offen |
| B8 | Integriert und als WebGL-Kapitel veröffentlicht: Start/Fortsetzen, Pause für Tastatur/Gamepad/Touch, drei überspringbare Einführungskarten, kompakteres HUD, gespeicherte Lautstärke/Kamera/Touch/Hinweise, Checkpoint-/Replay-Ergebnis | Vollständige Sitzungsoberfläche online; physischer Android-/Gamepad- und Performance-Nachweis offen |
| B9 | Lokal integriert: Boden-Smash mit einmaligem AOE bei Landung, Hindernis-/Teamfilter, sichtbarer Radius und Einzelreaktionen | Mehrfachtreffer und Unterbrechung gezielt geprüft |
| B10 | Lokal integriert: MP, Durchbruch, Druckwelle, bestehende Anschlüsse, neue Touch-Action und MP-Checkpoint | Drei Spezialaktionen auf menschlicher Charakterbasis; finale Art-Politur B13 |
| B11 | Lokal integriert: Raufbold/Werfer/Schläger, zwei Nahkampffreigaben, Teams/Zielwahl und Acht-/Zehn-Gegner-Labor | Gezielte Kampfprüfungen bestanden; tatsächliches Android-/Spielgefühlfeedback offen |
| B12 | Lokal integriert: fünf Begegnungen, drei Bereiche, genau 30 Gegner, endliche angekündigte Verstärkungen, Checkpoints und Finale | [Umsetzung und Nachweise](B12_LF2_STAGE.md) |
| B13 | Geplant: Figuren, Kampfwege, Map, Audio und Effekte ausarbeiten | Konsistente illustrative Präsentation |
| B14 | Geplant: Spielgefühl, MP/Schaden/Dichte und tatsächliche Geräteleistung abstimmen | Abgestimmte Desktop-/Android-Fassung mit dokumentierten Grenzen |
| B15 | Geplant: finaler WebGL-/Windows-Kandidat und beauftragte Veröffentlichung | Vollständige Stage lokal und über den Play-Link |

Jeder Bulk endet integriert und spielbar. Grundlegendes Feedback, Kamera und Bedienbarkeit wachsen ab dem Duell mit; B7/B8 arbeiten sie aus. Fertige Pakete werden nur für die tatsächlich benötigten Teile übernommen.

## Historische Detailplanung B1–B8

Die folgenden Detailabschnitte halten den damaligen Arbeitsablauf fest. Frühere Angaben zu „als Nächstes“, zum Duellfokus oder zu 10–15 Minuten sind historische Ziele. Für die weitere Entwicklung gelten ausschließlich die Übersicht oben und der [LF2-Stage-Plan](LF2_STAGE_PLAN.md).

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

Schritte 1–3 geliefert: `CharacterImportLab` bleibt als Bewegungstest erhalten; `HumanoidCombatLab` ergänzt bestehendes Sparring, eigene Timingdaten und echte Avatar-Kontaktbahnen. Der fehlende Kick/Luftkick wurde lokal am Humanoid ergänzt. Die ursprünglich vorläufigen Cross-Varianten für Finisher/Heavy/Air-Heavy wurden in B3a ersetzt. Der temporäre Mensch ist keine endgültige Charakterentscheidung; Wombat-Anpassung bleibt offen.

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

1. **B3a — Bewegung und Aktionen, geliefert:** Run über Ctrl/LT samt Übergängen und eigenem Clip; Walk-/Run-Tempo nach tatsächlicher Bewegung; Haken-Finisher, Overhand-Heavy und zweihändiger Luft-Smash mit neu gebackenen Kontaktbahnen. Sichtbare Hit-/Stagger-Reaktionen für Spieler und Robot-Gegner. Boden-/Sprung-Kick und Smash nach Nutzerfeedback in Richtung klarer Kampfsport-/Abwärtsschlagposen überarbeitet. Mensch gegen Bären-Dummy bleibt vorerst. Ergebnis: Bewegung und Schlagstärke sind unterscheidbar. Weiteres Pose-/Tempo-Tuning erfolgt am spielbaren Stand.
2. **B3b — Fallen und Aufstehen, geliefert:** Heavy/Luft-Smash und Gegner-Heavy werfen nieder. Fall, Bodenphase und GetUp sperren Aktionen/Treffer; anschließend 0,45 s Aufstehschutz bei freier Steuerung. Tod hält die ganze Figur unten; Reset stellt Pose, Collider und Kampfzustand wieder her. Details: [B3B_BODY_RECOVERY.md](B3B_BODY_RECOVERY.md).
3. **B3c — Duell und Übergabe, technisch geliefert:** KI-Warnabstand 1,30 m statt 1,65 m, Repositionierung 1,20 m; drei Combo-Treffer aus tatsächlichem KI-Abstand. Neue Gegnerangriffe warten bis zum Ende des Aufstehschutzes. Kontaktfeedback und deutsches Duell-HUD abgestimmt. 3/3 Duell- und 5/5 Kontakt-/Luftfälle bestehen; Windows- und WebGL-Builds erfolgreich. Browserbewegung, Combo, Sieg und Neustart über Tastatur geprüft. Details und offene praktische Prüfungen: [B3C_DUEL_HANDOFF.md](B3C_DUEL_HANDOFF.md).

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

Lokal integriert: Schulterstoß bis zu 2 m, Startup/Active/Recovery 0,16/0,20/0,40 s, 16 Schaden, Stagger ohne Knockdown; feste Startrichtung, erster Kontakt/Kollision stoppt den Weg, keine freie Bewegung oder Ausweich-Cancel bis Attack-Ende. Kick-Rückstoß 1,05 schafft mehr Abstand. Importierter Sprint als Rohmaterial, bestehender Avatar-Bake und CombatController weiterverwendet. E/RT/Touch STOSS. Details: [B4_SHOULDER_CHARGE.md](B4_SHOULDER_CHARGE.md). B5 ist ebenfalls lokal integriert; der öffentliche Play-Link enthält vorerst die bisherige Touch-Fassung.

- Einen kontrollierten Dash-Angriff beziehungsweise Schulter-/Kopfstoß aus einer passenden Basis ableiten. Klare Reichweite, Kollisionsbegrenzung und bestrafbare Recovery.
- Rollen abstimmen: Light für Druck/Combo, Heavy für kräftigen Abschluss, Kick für Abstand, Air-Attack für Sprungangriff, Evade für Positionierung.
- Trefferstärke/Knockdown-Wirkung differenzieren. Dauerstun und endlose Cancel-Schleifen vermeiden.
- Special-/Launcher-/Guard-Systeme nur ergänzen, wenn eine konkrete Spielentscheidung fehlt; nicht mehrere neue Systeme gleichzeitig.

**Ergebnis:** ein charakteristischer eigener Move und ein Moveset mit mehreren praktisch nützlichen Entscheidungen. Dash-/Wandkontakte und Unterbrechung gezielt prüfen.

## B5 — Gegnerrollen und Gruppenkampf

Lokal geliefert: drei Rollenwerte/Attack-Zuordnungen, Standard-Nahkampf, angekündigter gerader Agile-Ansturm und Heavy-Knockdown; Auswahl 1–4/D-Pad oben/Touch GEGNER. Vier Gegner teilen eine rotierende Angriffsfreigabe, verteilen Wartepositionen und berücksichtigen Körperkollision sowie Kamera-/Aufstehschutz. Vorhandene Figuren und Clips wiederverwendet. Details: [B5_ENEMY_ROLES.md](B5_ENEMY_ROLES.md). Vier neue gezielte Fälle bestehen; Übergabenachweise stehen im Status. Darauf baut das inzwischen integrierte B6-Kapitel auf.

- Standard mit kurzer Nahkampfaktion und offener Erholung; Agile mit angekündigtem geradlinigem Ansturm nach Seitenwechsel; Heavy mit größerer Silhouette und langsamem Knockdown-Schlag. Keine zusätzliche Rüstung in diesem Bulk.
- Vorhandene Bären-Grundform und denselben geprüften Animationspfad nutzen, Rollen über Silhouette/Handschuhe/Markierung/Aktion unterscheiden. Finale Cartoon-Modelle separat auswählen.
- EngagementCoordinator um sinnvolle Positionen und begrenzte Angriffserlaubnisse für drei bis vier sichtbare Gegner erweitern.
- Klare Eintritte, Abstand, Repositionierung und faire Offscreen-Regeln.

**Ergebnis:** Einzelduelle sind verständlich, gemischte Gruppen verändern Prioritäten und Positionierung. Wenige repräsentative Kombinationen und Token-/Unterbrechungsfälle genügen.

## B6 — Junkyard-Kapitel

Lokal geliefert in eigener Szene `JunkyardChapter`: Anlieferung → Sortierhof → Presswerk, neun Wellen/23 Gegner, vorhandene Rollen/Assets, Bereichsgrenzen und Kamerawechsel, Schaltertor, 30 HP Bereichsheilung, Checkpoint-Retry und vollständiger Neustart. Vorarbeiter als Heavy-Ableitung mit Begleitern, keine neue Bossarchitektur. Elf gezielte Kapitel-/Input-/Regressionsfälle bestehen; ein kompletter tatsächlicher Combat-Durchlauf gewinnt in 101,7 s mit 58 HP. Details: [B6_JUNKYARD_CHAPTER.md](B6_JUNKYARD_CHAPTER.md). Zunächst ein Kurzkapitel: Das 10–15-Minuten-Ziel erfordert noch weitere abwechslungsreiche Situationen und Spielerfeedback. B7 ist inzwischen integriert; als Nächstes B8, neue Veröffentlichung bleibt separat.

- Kurze zusammenhängende Graybox-Strecke mit drei Kampfbereichen, Bewegungs-/Erholungsabschnitten und klaren Übergängen.
- Standardgegner erklären den Einstieg; Agile/Gruppen steigern Druck; abschließend ein besonders inszenierter Heavy-Elitegegner.
- Encounter-Zustände, Spawnpunkte und Wellenfolge als einfache bearbeitbare Daten.
- Eine Environment-Interaktion, Checkpoint-Retry, Niederlage und Abschluss integrieren.
- Auf etwa 10–15 Minuten abwechslungsreichen Ablauf abstimmen; zusätzliche HP ersetzen keine interessanten Situationen.

**Ergebnis:** ein kompletter Durchlauf. Einen Durchlauf plus gezielten Checkpoint-Retry prüfen. Ein mehrphasiger eigener Boss gehört zur späteren Erweiterung.

## B7 — Cartoon-Präsentation der Welt lokal geliefert

Kapitelgestaltung, gemalter Boden, Kamera/Marker, Material-/Licht-/Volume-Ableitungen, getrennte warme Bärengesichtsfarben und zwölf CC0-Audioclips integriert. Zwei gezielte Kontakt-/Checkpoint-Fälle bestehen, tatsächlicher Kampf bis zum Sieg und finale Layout-/Touch-Ansichten kontrolliert. Details: [B7_PRESENTATION.md](B7_PRESENTATION.md). Bestehende Meshes/Rigs weiterverwendet; finale Cartoon-Modelle/Wombat, hörbare Audioabnahme und Performanceprofil bleiben offen. B8 ist der nächste Bulk.

- Eine stilistisch passende modulare Environment-Basis auswählen. Rost/Schrott, gemalte Details und glatte Hauptformen über eigene Material-/Textur-Ableitungen angleichen; keine wahllose Packmischung.
- Warme Lichtinseln/kühle Tiefe, Bodenkontakt und lesbare Figuren aus der Referenz in 3D übertragen.
- Kamera an Bereichswechsel und Gegnerverteilung anpassen; Verdeckungen und unruhige Bewegung vermeiden.
- Passende lizenzgeklärte Audio-/VFX-Grundlagen für Schritte, Kontakte, Warnungen und Umgebung nutzen.
- Kleine koordinierte Signale für Light/Heavy/Kick/KO; humorvolle Reaktionen an vorhandenen Aktionen ausarbeiten.
- Optionen für günstige Schatten/Effekte anhand der frühen WebGL-Messung wählen.

**Ergebnis:** ein konsistentes Comic-Kapitel, lesbar auch im Gruppenkampf. Eine dichte Sequenz mit Ton/Präsentation ansehen und konkrete Engpässe messen.

## B8 — Bedienung und vollständige Sitzung lokal geliefert

- Startmenü, Pause, Optionen, Niederlage/Retry, Kapitelabschluss; klare Tastatur-/Gamepad-Navigation.
- Kompaktes HUD, passende Eingabehinweise, kurze überspringbare Einführung in Grundaktionen.
- Lautstärken, Bildschirmdarstellung und reduzierte Kamerabewegung; Einstellungen speichern.
- Checkpoint- und vollständigen Neustart anbieten; wenige verständliche Ergebniswerte.
- Browserfokus und Audiofreigabe in den WebGL-Ablauf einpassen, ohne die Spieloberfläche mit Implementierungsdetails zu belasten.

**Ergebnis:** Die vorhandene Kapitel-Szene besitzt Start-/Einführungs-/Pause-/Options-/Ergebnisoberfläche und benutzt weiterhin dieselben Kampf-/Checkpoint-Regeln. Zwei gezielte Sitzungsfälle und der bestehende Kapitel-Ablauffall mit Sieg/Replay bestehen. Ein einfacher tatsächlicher Combat-Bot wurde im zweiten Bereich besiegt; echte Niederlage und Retry erhalten Bereich/Tor/42 Einstieg-HP. Das ist keine menschliche Schwierigkeits- oder Geräteabnahme. Details: [B8_SESSION.md](B8_SESSION.md). Als Nächstes B9 für Balance/Umfang, tatsächliche Performance und Kapitel-Demo.

## Aktuelle Vorwärtsplanung B9–B15

Der [LF2-Stage-Plan](LF2_STAGE_PLAN.md) beschreibt den Ausbau. B9–B11 sind lokal implementiert; [Übergabe und Nachweise](B9_B11_LF2_COMBAT.md). B12 ergänzt lokal die 30-Gegner-Stage; [Übergabe](B12_LF2_STAGE.md). **Als Nächstes B13:** Präsentationspolitur. Präsentation, Balance und Übergabe folgen in B13–B15. Der öffentliche Play-Link bleibt bis zu einem neuen Release auf B8. Ein physischer Android-Check und menschliches Feedback zum dichteren Kampf sind weiterhin offen.

## Ausführung und Grenzen

Normale lokale Entscheidungen selbst treffen. Keine wiederholten Setup-Gates, Architektur für 20 Figuren oder Vollregression nach jedem Tuningwert. Fertige Grundlagen zuerst, Eigenarbeit auf notwendige Identität/Mechanik konzentrieren. Neue Ordner schrittweise nutzen; bestehende Assets/Referenzen nicht allein für Strukturarbeit verschieben.

Browsergame ausschließlich lesen. Vorhandene Unity-Änderungen erhalten. Kein Commit/Push, Asset-Kauf, neuer Dienst oder Editor-Upgrade ohne entsprechenden Auftrag. Ein Online-Account oder ungeklärte Lizenz ist eine konkrete Abhängigkeit; eine generische Vorsichtsannahme erzeugt keine neue Freigaberunde.

