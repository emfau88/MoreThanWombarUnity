# Konkreter Arbeitsplan — LF2-inspirierte Stage

**Aktuelle Übergabe:** B4–B8 sind am öffentlichen Play-Link verfügbar. **B9–B11 sind lokal implementiert:** Boden-AOE, MP/Durchbruch/Druckwelle, drei Gegnerrollen und dichter Gruppenkampf. Sechs unterschiedliche gezielte PlayMode-Fälle bestehen; Geräte-/Spielgefühlgrenzen und Spielanleitung: [B9_B11_LF2_COMBAT.md](B9_B11_LF2_COMBAT.md). Verbindlicher Ausbauplan: [LF2_STAGE_PLAN.md](LF2_STAGE_PLAN.md). Ziel bleibt ein vollständiger Charakter, eine Stage, ungefähr 30 Gegner aus höchstens drei Typen. B12 ergänzt lokal den vollständigen Ablauf mit fünf Begegnungen und genau 30 Gegnern; [Übergabe](B12_LF2_STAGE.md).

## Als Nächstes: B13 — gemeinsame Präsentationspolitur

1. Den menschlichen Kämpfer und die drei Bärenrollen im tatsächlichen Gruppenbild prüfen; Rollen über Haltung, Silhouette und Accessoires klarer machen. Kein neuer Rig-Bau.
2. Die bestehende Stage an konkreten Sichtproblemen verbessern: Torpfosten, vordere Verdeckungen, Übergänge und Spawnzugänge. Bestehende Kenney-/Quaternius-Assets weiterverwenden.
3. Smash, Durchbruch und Druckwelle hinsichtlich Bewegungslesbarkeit, Wirkung und Erholung abstimmen. Warnungen und AOE-Effekte auch bei mehreren Treffern unterscheidbar halten.
4. Treffer-, Warn-, Schritt- und Umgebungston gemeinsam anhören und abstimmen. Einen kurzen integrierten Spielstand zur Beurteilung übergeben.

B14 folgt mit menschlichem Spielgefühl und tatsächlichem Android-Gerätecheck; B15 mit finaler Übergabe/Veröffentlichung.

## B12 — lokal integrierte 30-Gegner-Stage

1. Drei bestehende Bereiche zu fünf Begegnungen mit 21 Raufbolden, fünf Werfern und vier schweren Gegnern umstellen; einer davon ist der Vorarbeiter.
2. Begegnungen beim Vorankommen und endliche Verstärkungen statt dreimal derselben Drei-Wellen-Folge auslösen. Acht aktive Gegner bleiben bis zum Gerätefeedback der Standard.
3. Ausreichend freie Kampfwege, sichtbare Spawnzugänge und klaren Weiterweg vorsehen; höchstens zwei Werfer gleichzeitig.
4. Checkpoints inklusive HP/MP und ausstehender Gegner, Tor und eindeutigen Abschluss anpassen. Einen zusammenhängenden Durchlauf und gezielten Retry prüfen.

Umsetzung und Nachweise: [B12_LF2_STAGE.md](B12_LF2_STAGE.md). Zwei gezielte Ablauf-/Retry-Fälle bestehen. Ein tatsächlicher automatischer Kampf erreichte 19 besiegte Gegner; menschliche Balance bleibt offen.

## B9–B11 — umgesetzter Arbeitsumfang

1. Bestehende Trefferregeln für Team, Schutz und genau einen Treffer pro Ziel gemeinsam für Nahkampf und Flächentreffer verwenden.
2. Luft-Smash an den tatsächlichen Bodenaufprall binden; einmalige radiale Schadensauslösung, saubere Unterbrechung und Landungs-Recovery.
3. Passenden Bodenimpuls, einzelne Gegnerreaktionen und gebündelten Ton ergänzen; kein mit der Gegnerzahl gestapelter Hitstop/Shake.
4. Mit drei nahen und einem außerhalb stehenden Gegner sichtbar prüfen; gezielt Landung, Unterbrechung und Schutzfälle absichern.

**B10** ergänzt MP, Durchbruch, Druckwelle, Anschlüsse und Touch-Bedienung. Menschlicher Kämpfer bleibt die Basis dieser Stage; abschließende Art-Politur folgt in B13. **B11** ergänzt drei Gegnercharaktere und ein dichtes Gruppenlabor. Details und Abschlusskriterien stehen im [Gesamtplan](LF2_STAGE_PLAN.md); tatsächliche Nachweise in der [B9–B11-Übergabe](B9_B11_LF2_COMBAT.md).

## Historischer Arbeitsstand B1–B8

Die folgenden Abschnitte dokumentieren frühere Aufgaben und Nachweise. Frühere nächste Schritte, Release-Hinweise und das 10–15-Minuten-Ziel sind durch die aktuelle Übergabe und den LF2-Stage-Plan ersetzt.

Stand: 9. Oktober 2026. B1 und B2-Schritte 1–3 sind technisch geliefert. B3a/B3b, die Map-Aufwertung M1 und B3c sind integriert. B3c korrigiert den KI-Abstand und Angriffsschutz nach GetUp, stimmt Kontaktfeedback ab und liefert Windows-/WebGL-Spielstände. Acht gezielte PlayMode-Fälle bestehen; der Browserkampf wurde mit Tastatureingaben bis zum Sieg gespielt. Windows startet und rendert; sein physischer Tastaturcheck bleibt offen. Details: [B3C_DUEL_HANDOFF.md](B3C_DUEL_HANDOFF.md). Wombat-Look bleibt eine separate Entscheidung. B4–B6 sind lokal integriert. JunkyardChapter liefert drei verbundene Bereiche, neun Wellen, Torschalter, Checkpoints und Vorarbeiter-Abschluss; B7 ergänzt lokal Bereichsgestaltung, gemalten Boden, warme Bärengesichter, Kamera/Marker und Audio; B8 ergänzt lokal Menü/Pause/Einführung/Optionen/Ergebnis; nächster Entwicklungsbulk ist B9.

## M1 — Testmap aufgewertet

Auf Nutzerwunsch vorgezogen und integriert: Betonboden, neu gruppierte Container, fertige Schrott-/Reifen-/Zaun-/Werkstattmodelle, Hofschild und abgestimmtes Licht. Freie Kampfmitte, weiterhin bestehende Kollision und Bewegungsgrenzen. Kurzer tatsächlicher Durchlauf mit Randbewegung, Sprung, Warnung, Heavy, Knockdown/GetUp und Reset. Details und Quellen: [M1_MAP_PREVIEW.md](M1_MAP_PREVIEW.md).

M1 zieht einen kleinen Präsentationsanteil aus B7 vor; Levelstrecke und zusätzliche Kampfbereiche bleiben B6.

## B3c geliefert

1. **Duell-Abstände:** Jab/Cross/Haken aus normaler Spielentfernung verbinden; Vorwärtsschritt, Rückstoß und Fehlschlag-Recovery zusammen abstimmen. Heavy/Kick sollen einen klaren Zweck behalten.
2. **Luft und Aufstehen:** Sprung-Kick/Smash beim Anflug und Abstieg spielen; Kontaktmoment, Landung und erneuten Kampfbeginn nach GetUp abstimmen. Bestehende Flugbahn und Aufstehschutz erhalten.
3. **Treffergefühl:** Hitstop, Reaktion, Kontaktblitz und vorhandenen Sound auf leichte/starke Treffer abstimmen; Warnung und Ausweichen im Einzelduell beurteilen.
4. **Spielbare Übergabe:** Vorhandene Windows-/WebGL-Buildmodule prüfen, mit vorhandener Toolchain kleine Builds erzeugen. Windows kurz durchspielen; im Browser Fokus, Eingaben, Audio und Restart prüfen. Fehlende Module konkret festhalten, keine ungefragte Installation.

**Effekt:** Der vorhandene Mensch-gegen-Bären-Kampf wird als zusammenhängendes Duell abgestimmt und außerhalb des Editors überprüfbar. Größerer Contentausbau beginnt erst danach. Endgültiger Wombat-Look bleibt separat offen.

Die vier Punkte beschreiben den gelieferten Umfang. Tatsächliche Nachweise und verbleibende Audio-/Performance-/Windows-Eingabeprüfungen stehen in der B3c-Übergabe; diese werden nicht als bestanden vorausgesetzt.

## B4 — Schulterstoß lokal integriert

Auf „go b4“ umgesetzt: 2-m-Schulterstoß aus dem vorhandenen Sprint, gebackene Schulterbahn, feste Richtung, Stopp am ersten Kontakt/Kollision und volle verwundbare Erholung. E/RT/Touch STOSS. Kick schafft mit Rückstoß 1,05 mehr Platz; Combo und Heavy behalten ihre bewährten Rollen. Vier gezielte Stoß-Fälle bestehen. Details und abschließende Nachweise: [B4_SHOULDER_CHARGE.md](B4_SHOULDER_CHARGE.md). Öffentliche und eigenständige Builds bleiben bis zum nächsten Build-/Veröffentlichungsschritt auf dem vorherigen Stand.

Zusatzauftrag vor B4: Mobile Touch für Android im Querformat ist integriert; drei gezielte Eingabeprüfungen bestehen. GitHub Pages veröffentlicht die Browserfassung aus dem fertigen WebGL-Release; der Play-Link steht im README. Als Nutzercheck auf dem Android-Handy: bewegen und zugleich Combo/Kick auslösen, springen, ausweichen und nach Niederlage neu starten. Details und Prüfbefunde: [MOBILE_TOUCH.md](MOBILE_TOUCH.md). B4 bleibt separat.

1. **Ein Move festlegen:** geradliniger kurzer Schulterstoß, Eingabe zunächst E/RT als Vorschlag. Vorhandene Clips als Rohmaterial prüfen; nur nötige Schulter-/Körperpose ergänzen. Keine neue komplette Animationsbibliothek.
2. **Bewegung integrieren:** feste Richtung beim Start, kurzer Anlauf, begrenzte Strecke über PlayerMotor und bestehende Kollision. Root Motion bleibt aus. Eine Wand beendet die Bewegung zuverlässig.
3. **Kontakt anbinden:** eine AttackDefinition mit Startup/Active/Recovery, vorhandenen Team-/Frontfiltern, genau einem Treffer pro Ziel und bestehenden Stagger-/BodyRecovery-Regeln. Dash-Bewegung und Angriffsschritt nicht doppelt anwenden.
4. **Spielentscheidung abstimmen:** Schulterstoß schließt Distanz, Light hält Druck, Kick schafft Platz, Heavy wirft nieder. Fehlschlag/Ende des Stoßes erhalten eine spürbare Erholung; kein unbegrenzt sicheres Durchrennen.
5. **Kurze Übergabe:** tatsächliche Treffer-/Fehlschlagsequenz, Wandkontakt und Unterbrechung prüfen; dann Werte und Steuerung dokumentieren. Nur betroffene Fälle, keine neue Vollsuite und kein automatischer Plattformwechsel für jeden Tuningwert.

**Vorbereitung des Kapitel-Releases erledigt:** `com.unity.ai.inference` nach bestätigter Nichtnutzung entfernt; App-UI entfällt als transitive Abhängigkeit. WebGL wird mit Gzip und Unity-Dekompressionsfallback gebaut; Pages versioniert die Asset-Adressen gegen alte Loader im Browsercache. Der Kapitelbuild umfasst 19,8 MB. Ein tatsächliches Framezeit-/Speicherprofil bleibt B9.

**Effekt:** eine neue, erkennbare Kampfoption auf der bestehenden technischen Basis. B4 ist beauftragt und lokal integriert.

## B5 — Gegnerrollen und gemischte Gruppen geliefert

1. **Drei Rollen auf der bestehenden Basis:** Standard hält kurze Nahkampfschläge; Agile kündigt einen geraden Ansturm an; Heavy greift langsamer und kräftiger an. Vorhandene Assets/Animationen wiederverwenden, Haltung und gut lesbare Markierungen zunächst zur Unterscheidung nutzen.
2. **Kleine Encounter-Konfiguration:** Werte/Attack-Zuordnung der Rollen als einfache Daten hinterlegen; denselben EnemyBrain, Schadensempfänger und BodyRecovery verwenden. Kein neues paralleles KI-System.
3. **Gruppendruck:** EngagementCoordinator für drei bis vier Gegner erweitern; Positionen verteilen, Angriffsfreigaben begrenzen, Kamerabereich berücksichtigen. Ruhige Gegner dürfen den Spieler nicht einkesseln, ohne auszuweichen.
4. **Spielbarer Mischkampf:** Zwei repräsentative Gruppen abstimmen. Schulterstoß zum Annähern, Kick zum Befreien und Heavy zum Unterbrechen praktisch nutzen; Warnungen und Erholung fair halten. Nur neue Rollen-/Token-/Unterbrechungsfälle gezielt prüfen.

**Effekt:** unterschiedliche Gegner verlangen andere Prioritäten und Positionierung. Danach baut B6 daraus eine zusammenhängende Junkyard-Strecke mit drei Kampfbereichen und Checkpoints. B5 ist lokal geliefert; Details und Nachweise: [B5_ENEMY_ROLES.md](B5_ENEMY_ROLES.md).

## B6 — zusammenhängende Junkyard-Strecke geliefert

1. **Strecke und Bereiche:** Bestehende Schrotthof-Assets/Materialien und Kamera weiterverwenden. Drei verbundene Kampfbereiche mit kurzen Lauf-/Erholungsabschnitten und klar lesbaren Ausgängen aufbauen. Erst Layout und Ablauf, anschließend gezielte Dekoration.
2. **Encounter-Daten und Übergänge:** Kleine bearbeitbare Konfiguration für Rollen, Spawnpunkte und Wellenfolge. Bereich nach Eintritt starten, nach Sieg freigeben; Gegner sichtbar innerhalb sinnvoller Kampfpositionen auftreten lassen. Bestehenden Coordinator und BodyRecovery benutzen.
3. **Steigerung:** Einstieg mit Standard, anschließend Agile/Mischgruppe; Abschluss als inszenierter Heavy-Elitekampf aus derselben Technik. Schwierigkeit über Zusammensetzung/Positionen statt bloß zusätzliche HP abstimmen.
4. **Interaktion und Checkpoint:** Eine klare Environment-Aktion integrieren, etwa ein zu öffnender Durchgang. Nach jedem gewonnenen Bereich Checkpoint; Retry stellt nur den aktuellen Abschnitt und seinen Gegnerzustand wieder her. Vollständiger Neustart bleibt möglich.
5. **Spielbare Übergabe:** Einen vollständigen Durchlauf sowie Niederlage/Checkpoint-Retry prüfen. Wege, Pausen und Gegnerfolge anschließend auf etwa 10–15 Minuten Designziel abstimmen. Menüausbau bleibt B8, finale Art-/Audioabstimmung B7.

**Effekt:** Aus der Testarena ist erstmals eine eigene zusammenhängende Kapitel-Szene entstanden: neun Wellen/23 Gegner auf drei Bereichen, ein Schaltertor, Checkpoint-Retry und Vorarbeiter-Finale. Ablauf und Übergabe sind lokal integriert/geprüft; das vollständige automatische Combat-Spiel gewann in 101,7 s mit 58 HP. Details: [B6_JUNKYARD_CHAPTER.md](B6_JUNKYARD_CHAPTER.md). Damit zunächst ein Kurzkapitel. Das 10–15-Minuten-Ziel ist offen und braucht anhand von Spielerfeedback zusätzliche abwechslungsreiche Situationen, keine reine HP-Verlängerung. B4–B6 benötigen für den öffentlichen Play-Link einen neuen Build-/Veröffentlichungsschritt.

## B7 — Präsentation lokal geliefert

1. **Kapitel spielen und konkrete Engstellen wählen:** Lesbarkeit der Gruppen, Kamera an Toren/Übergängen, störende Verdeckungen und gleichförmige Abschnitte am vorhandenen Kapitel beurteilen. Daten/Platzierung gezielt verbessern; keine neue KI-/Levelarchitektur.
2. **Welt angleichen:** Vorhandene Props/Materialien zu konsistenter Schrotthof-Präsentation ausarbeiten. Bereichssilhouetten, gemalte Rost-/Schmutzdetails, warme Lichtinseln und Bodenkontakt verbessern. Bei einer konkreten Asset-Lücke eine kleine geeignete Auswahl statt breitem Neuimport.
3. **Audio/VFX:** Vorhandene Kontakte erhalten und passende lizenzgeklärte Grundlagen für Schritte, Warnungen, Tor, Presse und Umgebung wählen. Heavy/Agile/Standard hörbar und sichtbar unterscheiden, ohne Kampfsignale zu überladen.
4. **Figurenentscheidung separat halten:** Mensch-gegen-Bären bleibt spielbare Basis. Wombat-/Cartoon-Look gezielt aus geeigneten Rigs/Modellen ableiten, sobald die konkrete Gestaltung entschieden ist.
5. **Kurze Übergabe:** Eine dichte Spielsequenz mit Audio und Kamera beurteilen; konkrete Performanceprobleme messen. Der nächste Release kann dann erstmals B4–B6 außerhalb des Editors liefern. Menüs/Einführung/Pause folgen B8.

**Effekt:** Das Kurzkapitel besitzt unterschiedliche Bereichssilhouetten und eine erste zusammenhängende illustrative Bild-/Audiofassung. B7 ist lokal integriert; Details und zwei gezielte Prüfergebnisse: [B7_PRESENTATION.md](B7_PRESENTATION.md). Finale Figuren-/Modelle, hörbare Audioabnahme, Geräte-/Performanceprofil und längerer abwechslungsreicher Umfang bleiben offen.

## B8 — vollständige Spielsitzung lokal geliefert

1. **Start und Ergebnis:** Kleines Startmenü mit Spielen/Steuerung/Optionen; nach Sieg oder Niederlage klare Checkpoint-/Von-vorn-Aktionen. Die bestehende Kapitel-Szene und ihre Zustände verwenden.
2. **Pause sauber integrieren:** Tastatur, Gamepad und Touch; Spielzeit/Kampf einfrieren, Menüs weiter bedienen und beim Fortsetzen Eingabepuffer/virtuelle gehaltene Controls bereinigen.
3. **Kurze Einführung:** Wenige überspringbare Hinweise für Bewegung, Combo, Ausweichen und Schalter; Hinweise passend zur aktiven Tastatur-/Gamepad-/Touch-Bedienung.
4. **HUD und Optionen:** Bereich/Welle/HP kompakt halten, Lautstärke und Bildschirm-/Kameradarstellung einstellen und speichern. Bestehendes Input System und Unity-UI verwenden.
5. **Sitzungsnachweis:** Start → Kampf → Pause/Fortsetzen → Niederlage/Checkpoint → Abschluss durchspielen. Ein neuer Build muss die Kapitel-/Menü-Einstiegsszene verwenden; der bisherige Duell-Buildpfad startet noch das Labor. Veröffentlichungsworkflow weiterverwenden.

**Effekt:** Start/Fortsetzen, Pause, drei überspringbare Einführungskarten, passende Steuerung, gespeicherte Lautstärke/Kamerablick/Nachführung/Touch/Hinweise und Niederlage-/Sieg-/Replay-Aktionen sind lokal integriert. Eingabepuffer und gehaltene virtuelle Controls werden an Menüübergängen bereinigt. Zwei gezielte Sitzungsfälle bestehen; vorhandener Kapitel-Ablauffall ergänzt Sieg/Replay. Der neue Buildpfad startet JunkyardChapter, die B3c-Buildmenüs behalten das Labor. Details: [B8_SESSION.md](B8_SESSION.md).

## Frühere B9-Planung — durch den LF2-Stage-Plan ersetzt

1. **Kurzes Spielerfeedback:** Kapitel mit Einführung spielen, konkrete Stellen für unfaire Gruppen, dominante Moves, leere Wege und Checkpoint-Frust sammeln. Der einfache Light-Bot wurde beim B8-Batchdurchlauf im zweiten Bereich besiegt; kein Anlass, pauschal HP/Schaden zu ändern.
2. **Umfang entscheiden:** Das vorhandene Kurzkapitel gezielt abwechslungsreicher machen oder zunächst als kurze Demo ausliefern. Für 10–15 Minuten zusätzliche Situationen statt bloß HP-Verlängerung planen.
3. **Tatsächliche Leistung messen:** Framezeiten/Speicher im Kapitel auf Test-PC und Android-WebGL; nur nach konkreten Engstellen Schatten, Effekte, Texturen und Importkosten optimieren. Ungenutztes Inference-Paket bereits vor dem Kapitelbuild entfernt.
4. **Kapitel-Build beurteilen:** Der öffentliche Play-Link enthält jetzt B8 einschließlich Hauptmenü und drei Bereichen. Veröffentlichung erledigt; als Nächstes tatsächliche Android-Bedienung/Leistung und hörbare Audioqualität beurteilen. Ein neuer Windows-Kapitelbuild bleibt separat offen.
5. **Demo übergeben:** kurze deutsche Anleitung, Quellen/Lizenzen, konkrete Gerätegrenzen und priorisierte nächste Inhalte. Finale Wombat-Figur/hochwertige Modelle und hörbare Audioabstimmung bleiben eigenständige konkrete Entscheidungen.

## 1. Bestand und Entscheidung

| Vorhandener Stand | Entscheidung / konkrete Folge |
| --- | --- |
| LabInput, PlayerMotor, CharacterDefinition und MotorMath | Behalten; B3a ergänzt gehaltenes Rennen mit separater Bodengeschwindigkeit, ohne Luft-Sprint |
| CombatController, AttackDefinition, Buffer, Attack-Instanzen, Front-/Teamfilter | Behalten; gewünschte Angriffszeiten und Humanoid-Clip-Mapping sind in B2 Schritt 3 integriert |
| Animierte Faust-/Fuß-Sweeps auf getrennten Hurtboxes | Regel behalten; Humanoid-Kontaktbahnen sind am echten Avatar gebacken und gegen sichtbare Posen geprüft |
| B1: Angriffsschritte, Kick, Air-Kick/Air-Smash, spätes Movement-Release | Behalten; einmal anhand echter Nutzersequenz fein abstimmen |
| TrainingDummy, PlayerDefense, EnemyBrain, EngagementCoordinator | Wiederverwendet; BodyRecovery ergänzt B3b mit Knockdown/GetUp/Tod und Schutz |
| Kamera, Testarena, gespeicherte Trainings-/Sparring-Szenen, HUD, Reset | Behalten als kurze Integrationsumgebung; Arena genügt für B2/B3 |
| Grundform-Figur und selbst erzeugte Standardclips | Spielbare Vergleichsbasis; durch passende importierte Basis ersetzen, kein weiterer großer Standardanimationsbau |
| Blender-Wombat, eigener Skeleton-Entwurf und JSON-Mesh-Bibliothek | Erhalten und zurückstellen; noch nicht in die Spielfigur integriert, kein Grund für zusätzliche Importarchitektur |
| Geplanter kompletter eigener Modell-/Rig-/Animationspfad | Ersetzen durch Asset-Auswahl, Retargeting und begrenzte Anpassung für die Wombat-Identität |
| Run, Flinch/Stagger und Knockdown/GetUp | In B3a/B3b integriert; vor weiteren Gegnerrollen das vollständige Duell abstimmen |
| Build/Browserleistung | Windows-/WebGL-Builds vorhanden, Browser-Tastaturkampf nachgewiesen; Ladegrößen erfasst, Framezeit/WebGL-Speicher und hörbare Audioabnahme noch offen |

Der Bestand ist bereits fokussiert geprüft. Eine Fortsetzung beginnt an den geänderten Stellen. Quaternius liefert jetzt einen konkreten Humanoid-/Bewegungsnachweis; die Eignung einer tierischen Basis und die Wombat-Gestaltung bleiben offen.

## 2. B1 abgeschlossen: konkrete Übergabe

- Die doppelte Startup-Richtungskorrektur ist behoben: Angriffsbeginn und früher Startup teilen insgesamt höchstens 25° zur ursprünglichen Blickrichtung.
- Fünf gezielte Polish-PlayMode-Fälle bestanden. Der neue Fall belegt Drehgrenze, feste Richtung bis zur Bewegungsfreigabe und freie Bewegung vor Attack-Ende. Vor der Korrektur reproduzierte er die Überschreitung. Resultat unter tools/b1-close-results.json.
- Echte Kamerasequenz für Walk, Kick, Jump-Hit und KO unter Assets/QA/b1-close-*.png angesehen. Sie zeigt Spielposen; eine subjektive Nutzerabnahme ist damit nicht ersetzt.
- COMBAT_SYSTEM.md und ARCHITECTURE.md vollständig auf den tatsächlichen B1-Stand gebracht. B2-Retargeting/Attack-Zeiten bleiben geplante Erweiterungen.
- Vorhandene breitere Resultate bleiben gültige historische Nachweise: 24/24 EditMode, bestehende 25 PlayMode-Fälle sowie die früheren vier Polish-Fälle. Nach der kleinen Drehkorrektur nur die fünf betroffenen Polish-Fälle ausgeführt, keine neue Vollsuite.

**Lieferung:** B1-Spielstand samt korrekter Steuerung/Regeln. Kurz selbst spielen: Richtungswechsel, J-Kette aus angenehmem Abstand, K, L, Sprung + J/K, Shift, KO und R; mit 2 den Gruppendruck ansehen. Nutzerfeedback dient gezieltem Tuning. B2 kann bei Beauftragung direkt mit der begrenzten Asset-Auswahl beginnen.

## 3. B2: passende Asset-Basis auswählen und integrieren

### Schritt 1 — höchstens drei ernsthafte Kandidaten

- Nach rigged Cartoon-Zweibeinern beziehungsweise anpassbaren tierischen Figuren suchen: runde Silhouette, Schnauze/Ohren, gute Augen-/Brauenform, glatte Oberflächen. Eine bloß braun gefärbte menschliche Figur zählt nicht als passender Wombat.
- Vorhandene Humanoid-Animationen zuerst prüfen, etwa die konkrete kostenlose Quaternius-Version; Mixamo ergänzend für fehlende Bewegungen. Idle/Walk/Run/Punch/Kick/Hit/Death tatsächlich im gewählten Archiv nachsehen.
- Je Kandidat kurz notieren: Quelle, Lizenz, Preis, Rig-/Avatar-Typ, enthaltene Clips, grober Anpassungsaufwand, Stilabweichungen, Materialien/Geometrie und Importformat.
- Eignung in dieser Reihenfolge: erkennbare Charakteridentität und Silhouette → brauchbare Bewegungs-/Kontaktposen → Import/Retargeting → Anpassungsaufwand und Laufzeitkosten. Kein Pack nur wegen kostenloser Verfügbarkeit auswählen.
- Ergebnis als kleine Vergleichstabelle mit Empfehlung im Projekt festhalten. Keine pauschale Zusage, dass ein fertiger rigged Wombat existiert.

### Schritt 2 — eng begrenzter Importnachweis

Aktuelle Lieferung: `CharacterImportLab.unity` mit temporärem Quaternius-Humanoid, eigenem Prefab/Controller, fünf retargeteten Bewegungskopien und unverändertem PlayerMotor/LabInput. Idle, Walk, Jump, Fall und Land sind angebunden. Skalierung, Bodenhöhe, Forward und vier Hand-/Fußanker sind eingerichtet; Root Motion ist aus. Auswahl/Lizenzen sind gespeichert. Das ist ein technischer Bewegungsnachweis; das eigentliche Combat bleibt im B1-Sparring. Wombat-Gestaltung wurde nicht begonnen.

- Nur die empfohlene Kombination beziehen, Lizenzbeleg erhalten und THIRD_PARTY_LICENSES.md aktualisieren. Originale unter ThirdParty; eigene Darstellung/Controller unter Game/Characters und Game/Animations.
- Avatar, Metermaßstab, Forward-Achse, Bodenhöhe und Hand-/Fußkontakte einstellen. Normale Unity-Modellimporte nutzen; bestehendes Rig vorerst als Vergleich erhalten.
- Im technischen Human-Nachweis zunächst Idle/Walk/Jump/Fall/Land anbinden. Punch/Kick werden zusammen mit der tatsächlichen Humanoid-Kontaktauflösung in Schritt 3 integriert. Das Standardarchiv enthält Jab/Cross, aber keinen Kick; dafür eine konkrete Ergänzung wählen. Die kurze spätere Wombat-Anatomie separat auf brauchbare Kontaktposen prüfen.
- Ist nur ein neutrales Humanoid-Testmodell verfügbar, damit ausschließlich die technische Animationseinbindung prüfen und es klar als temporär kennzeichnen. B2 gilt erst mit einer stilistisch passenden spielbaren Charakterbasis als abgeschlossen.

### Schritt 3 — Combat-Zeiten und importierte Pose verbinden

Umgesetzt in `HumanoidCombatLab`: eigene AttackDefinitions mit Startup-/Active-/Recovery-Sekunden, abschnittsweise retimte Humanoid-Clips und am Avatar ausgewertete Kontaktpunkte. Animator-Zeit bleibt die einzige Phasenquelle; Runtime-Sweeps verwenden die gespeicherte Bahn ohne Umposen des sichtbaren Humanoids. Jab/Cross, eigene Kick-/Luftkick-Ableitungen, bestehende Defense/Gegner/Feedback und Reset sind angebunden. B3a ersetzt die ursprünglichen Cross-Finisher/Heavy/Air-Heavy durch eigene unterscheidbare Ableitungen. Zeit-/Clip-/Avataränderungen benötigen Neuableitung/Bake: beim vollständigen Neuaufbau HumanoidCombatBuilder, anschließend CharacterActionBuilder und zuletzt BodyRecoveryBuilder. Details in den Integrationsdokumenten.

- AttackDefinition um gewünschte Startup-/Active-/Recovery-Dauern und notwendige Clip-Abspielbereiche ergänzen. Bestehende Schadens-/Buffer-/Cancel-/Filterregeln weiterverwenden.
- Clip-Zuordnung an diese Zeiten anbinden; den Kontaktmoment des Punch/Kick im Active-Fenster zeigen. Ein schnellerer oder längerer Ersatzclip darf Angriffstiming nicht automatisch verändern.
- Eine gemeinsame Phasenautorität bestimmen. Für gleichmäßig beschleunigbare Clips reicht eine passend konfigurierte State-Geschwindigkeit; wenn Kontaktpose und Timing damit nicht passen, Abspielbereich oder gezielte Ableitung korrigieren. Keine zwei unabhängig laufenden Phasenquellen hinzufügen.
- Der aktuelle Code verwendet `AnimationClip.SampleAnimation` für direkte Transformkurven. Nicht voraussetzen, dass dies für retargetete Humanoid-Clips dieselbe Pose ergibt. Am ausgewählten Avatar den korrekt retargeteten Hand-/Fußverlauf über Animator-Auswertung prüfen; Kontaktauflösung auf diesen Pfad anpassen.
- Zunächst einen konkreten Punch/Kick integrieren, einschließlich übersprungenem Active-Fenster. Bei Bedarf wenige Posepunkte offline am ausgewählten Avatar abtasten und als lokale Kontaktbahn zur Attacke speichern. Diese gehört zur Modell-/Clip-Kombination und wird nach Rig-/Clipwechsel erneuert. Die sichtbare Pose darf durch die Abfrage nicht hängen bleiben.
- Bewegung weiter durch den bestehenden Motor begrenzen. Importierte Root Motion nicht parallel zu den B1-Angriffsschritten anwenden.

### Schritt 4 — begrenzte Anpassung und Übergabe

- Nur nötige Form-, Material- und Gesichtsänderungen für die Referenzidentität ausarbeiten. Neue Kleidung/Handschuhe nicht als automatische Stilvorgabe übernehmen.
- Passende einfache Gegnerdarstellung ableiten; ein Gegnertyp genügt. Vorhandene KI und Schaden übernehmen.
- Sparring speichern, Figur aus der Spielkamera in Idle/Walk, Punch/Kick und Sprung ansehen. Einmal Reimport mit erhaltenen Referenzen prüfen.

**B2 fertig:** stilistisch passende Basis im vorhandenen Spiel, regulärer Import, brauchbares Rig/Avatar, vier integrierte Kernclips, verlässliche Hand-/Fußkontakte und nachgewiesene Herkunft. Die endgültige Material-/Animationsfeinheit wird an diesem integrierten Stand weiterentwickelt.

**Falls kein Kandidat passt:** kurze Recherche beenden und die kleinste konkrete Anpassungsroute wählen. Ein vorhandenes rigged Cartoon-Modell anpassen; eigene Arbeit auf Wombat-Identität beschränken. Bei notwendigem Kauf/externem Auftrag vorab einen konkreten Lieferumfang samt Kosten vorlegen. Unabhängiges Combat-Tuning kann mit dem vorhandenen Platzhalter weitergehen.

## 4. B3: ein vollständiges überzeugendes Duell

- Vollständige Basis integrieren: Idle, Walk, Run, Jump, Fall, Land, Light 1/2/3, Heavy, Kick, Air-Attacks, Evade, Flinch/Stagger, Knockdown, GetUp und Death. Fertige Clips als Rohmaterial nutzen; nur fehlende eigene Bewegungen selbst bauen.
- Walk/Run so schalten, dass Eingaben unmittelbar reagieren und Schrittgeschwindigkeit zur tatsächlich zurückgelegten Strecke passt. Run dient zunächst der Bewegung; ein neuer Dash-Move ist B4.
- Jede Light-Stufe bleibt eine einzelne Attack-Instanz. Startup, Trefferanschluss, Fehlschlag-Erholung, Knockback und Vorwärtsschritt gemeinsam abstimmen. Die Kette soll am Gegner bleiben, starke Abschlüsse sollen Abstand erzeugen.
- Gegner nach gültigem Treffer sichtbar reagieren lassen. Lebendes Knockdown besitzt eigene Boden-/Aufstehphase und definierten Schutz gegen sofortige Dauertreffer. Death beendet KI/Angriff/Kollision; Reset stellt alles wieder her.
- Hand-/Fußreichweite, Hurtbox und Körperabstand aus der Spielkamera prüfen. Keine automatische Schadenszone aus Mesh-Bounds ableiten.
- Hitstop, kleiner Kontakt-Effekt, Sound und Reaktion gemeinsam tunen; Luftflugbahn erhalten. Der vorhandene synthetische Sound genügt bis eine passende geklärte Quelle Mehrwert liefert.
- Einen kleinen Windows-Spielstand und einen WebGL-Smoke-Build des Duells anstreben. Vorhandene Buildmodule prüfen; fehlende Installation nicht ungefragt durchführen. Im Browser Start, Fokus, Eingaben, Audio nach Nutzerinteraktion und Restart prüfen. Ladegröße, Framezeit und Speicher grob messen; danach konkrete Budgets festlegen.

**B3 fertig:** ein Spieler und ein Gegner lassen sich in einer kurzen vollständigen Sitzung überzeugend steuern und bekämpfen. Kontakt, Fall/Aufstehen/Tod und Reset sind klar. Die Charakterdarstellung trifft die Cartoon-Richtung. Browserverträglichkeit ist geprüft oder eine konkrete fehlende Toolchain ist dokumentiert; sie wird nicht aus dem Art-Stil abgeleitet.

## 5. Gezielte Prüfung und Übergaben

| Änderung | Kurzer Nachweis |
| --- | --- |
| Neues Rig/importierte Clips | Idle/Walk/Punch/Kick im echten Avatar, Fußkontakt, Abbruch/Reset und einmal Reimport |
| Clip austauschen | Definierte Angriffszeiten bleiben erhalten; Active zeigt tatsächlichen Kontakt; kein Schaden in Startup/Recovery |
| Retargeteter Sweep | Übersprungenes Active-Fenster, einmal pro Ziel/Instanz, Front-/Teamfilter, keine Veränderung der sichtbaren Endpose |
| Run/Knockdown/GetUp | Bewegungswechsel, begrenzte Unterbrechung, Aufstehen/Schutz und Reset in der gespeicherten Szene |
| Build | Kurzer tatsächlicher Durchlauf auf Windows und kleiner Browser-Smoke-Test |

Ein relevanter bestehender Regressionblock nach Integration der Charakter-/Timingänderung; danach nur betroffene Fälle bei weiteren Änderungen. Für B2 sind das die fünf B1-Polish-Fälle zusätzlich zu gezielten Humanoid-Fällen. Keine Unit-Tests für Modellfarbe, Materialwahl oder Dokumentation. Visuelle Kontrolle und Nutzer-Spielgefühl ergänzen die Regeln-Tests.

B2 liefert einen direkt spielbaren Zwischenstand mit Lizenz-/Quellennachweis. B3 liefert das vollständige Duell samt knapper deutscher Spielanleitung. Weitere Rollen und Level folgen danach gemäß Roadmap.
