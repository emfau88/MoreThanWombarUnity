# More Than Wombat — Combat-Polish und Weg zum hochwertigen Brawler

Stand: 3. Oktober 2026. Historischer Implementierungsplan auf Basis des Anhangs, der lokalen Unity-Implementierung und der lesend geprüften 2D-Referenz. B1 ist inzwischen implementiert und gezielt geprüft; der aktuelle Iststand steht in [COMBAT_SYSTEM.md](COMBAT_SYSTEM.md) und [TEST_SLICE_STATUS.md](TEST_SLICE_STATUS.md). Die folgenden Ausgangsbefunde und Startwerte beschreiben die Planung vor B1, nicht heutige offene Fehler.

## 1. Zielbild

Ein charakterstarker Arcade-Brawler: kompakter Wombat, kräftige und gut lesbare Bewegungen, direkte Steuerung, rhythmische Schlagfolgen und fair angekündigter Gegnerdruck. Echte 3D-Körper und Sprunghöhe, präsentiert durch eine kontrollierte 2.5D-Kamera. Der Spieler soll Abstand, Richtung und Timing intuitiv verstehen können.

Die Browserfassung liefert Identität, Humor und Kampfrhythmus. Unity soll diese Stärken mit räumlich glaubwürdigen Kontakten, deutlich besseren Animationen und konsistenter Präsentation weiterentwickeln. Dieses Dokument beschreibt den unmittelbaren Combat-Polish (B1). Die übergeordnete Planung der nächsten neun Bulks bis zum spielbaren ersten Junkyard-Kapitel steht in [ROADMAP.md](ROADMAP.md).

## 2. Was tatsächlich geprüft wurde

- Alle sechs Einstiegsdokumente, Runtime-Komponenten für Combat, Bewegung, Eingaben, Gegner und Schaden, die drei Szenen-Builder, Angriffsdaten, relevante Tests sowie Teile der gespeicherten Sparring-Szene und des Walk-Clips. Das vorhandene S3-QA-Bild wurde ebenfalls angesehen; es ersetzt keine aktuelle Bewegungsprüfung.
- Im lokalen Browser-Repo: README, BasicChainContract, MovementContract, relevante Fighter-Abschnitte, Angriffsdaten, EncounterDirector und Gegnerrollen. Dies ist eine fokussierte Designanalyse, kein vollständiger Content-Audit oder neuer Spieltest.
- Im geprüften Unity-Projektbereich und den geprüften übergeordneten Verzeichnissen wurden keine AGENTS.md-Anweisungen gefunden.
- Das Unity-Repo besitzt weiterhin keinen ersten Commit; vorhandene Projektdateien sind untracked. Sie bleiben erhalten. Die gepinnten Unity-/Paketversionen entsprechen dem Anhang.

### Konkrete Befunde

| Befund aus dem Code | Bedeutung für den nächsten Bulk |
| --- | --- |
| Bodenangriffe setzen Bewegung auf null und sperren das Drehen während der gesamten Attacke. | Angriffsbeginn, kontrollierten Schritt und Erholung gemeinsam abstimmen. |
| Light-Treffer bewegen das Ziel zurück, während der Angreifer stehen bleibt. | Knockback und Folgeschlag-Reichweite können die Kette räumlich auseinanderziehen; im Spiel prüfen und zusammen tunen. |
| Luftangriffe starten dieselben Light-/Heavy-Clips wie Bodenangriffe. | Ein eigener abwärts gerichteter Luftangriff ist erforderlich. |
| Jump-Speed 8 und Gravitation 23 ergeben rechnerisch etwa 1,39 m Sprunghöhe über dem Startpunkt. Die Gegner-Hurtbox reicht ungefähr von 0,53 bis 1,75 m über den Boden. | Horizontale Fäuste geraten im Sprung leicht über das Ziel. Kontaktbahn und Timing lösen das Problem, nicht pauschal größere Höhen-Toleranzen. |
| Der Walk-Clip hat einen festen Zyklus von 0,65 s, kurze Beine mit ±26° Ausschlag und nur 3,5 cm vertikales Wippen; Bewegung läuft mit 4,2 m/s. | Plausible Ursache für schwach sichtbare Schritte und Gleiten. Sichtprüfung muss tatsächliche Bewegung und Animation zusammen bewerten. |
| Tod kippt `TrainingDummy.pad`; beim Gegner verweist das auf `Rig/Torso`. Körpercollider bleiben aktiviert. `EnemyBrain.Interrupt` spielt auch bei Tod Idle. | Ganzkörper-Tod, eigene Animationszuständigkeit und Abschalten der blockierenden Kollision fehlen. |
| Gegner und Spieler nutzen dasselbe Heavy-Asset. | Spieler-Polish darf Gegnerreichweite und Telegraph-Timing nicht unbeabsichtigt mitverändern. |
| Die vorhandenen Lufttests prüfen Start/Flugbahn und ausdrücklich einen Fehlschlag. | Erfolgreichen Luftkontakt ergänzen; den bisherigen Negativfall anhand der neuen Move-Geometrie sinnvoll neu definieren. |

Die genaue komfortable Trefferentfernung und das aktuelle Laufbild sind noch nicht live vermessen. Die erste kurze Spielsequenz der Umsetzung klärt diese beiden Punkte; dafür ist keine neue allgemeine Diagnosephase nötig.

## 3. Lehren aus der 2D-Fassung

- **Rhythmus:** Die Wombat-Kette besitzt eigene Folgeangriffe und unterscheidet zwischen früherem Anschluss nach Treffer und späterem Anschluss nach Fehlschlag. Unity soll erfolgreiche Treffer belohnen und Fehlschläge spürbar binden.
- **Raumgewinn:** Mehrere Referenzangriffe besitzen `forwardTravelSpeed`. Ein kleiner sichtbarer Schritt hilft, Druck aufzubauen und Rückstoß innerhalb der Combo auszugleichen.
- **Luftangriff:** `air_bonk` ist eine eigene Aktion mit eigenem Timing und Luftsteuerung. Seine großzügige 2D-Höhenprüfung wird durch passende 3D-Posen und echte Kontakte ersetzt.
- **Gegnerdruck:** Rollen, Angriffstokens, klare Ankündigung und verwundbare Erholung sind gute Bausteine. Der nächste Bulk bleibt bei einem Gegnertyp und dem vorhandenen Zwei-Gegner-Modus.
- **Identität:** Humor entsteht später auch aus Bewegung und Reaktionen: überdeutliche Vorbereitung, kräftige Treffer, charaktervolle Fehlschläge. Breiter Roster, Specials und Bosscontent sind spätere Ausbauschritte.

## 4. Nächster Bulk: S3 Combat- und Animations-Polish

Die folgenden Arbeitspakete werden zusammenhängend umgesetzt und anschließend als spielbarer Stand übergeben. Normale Tuning- und Implementierungsentscheidungen werden direkt getroffen.

### A. Nahkampf, Abstand und Combo als Einheit verbessern

1. In der gespeicherten Sparring-Szene einmal Anlaufen, Light-Kette, Heavy und Richtungswechsel spielen. Sichtbare Faustkontakte, Körperabstand und Rückstoß vergleichen.
2. Jab, Cross und Finisher mit klaren Vorbereitungs-, Kontakt- und Erholungsposen versehen: Körperdrehung, Gewichtswechsel und sichtbare Armstreckung. Die kurze Wombat-Silhouette erhalten; keine künstlich teleskopierenden Arme.
3. Kontrollierten Vorwärtsschritt über den CharacterController einführen. Startwert: ungefähr 0,15–0,30 m pro Light, begrenzt durch Kollision und Arena. Die Bewegung folgt der Clipphase; bereits abgearbeitete Phasen erzeugen keinen zusätzlichen Weg.
4. Bewegungsrichtung beim Angriffsbeginn berücksichtigen; kleine eingabegesteuerte Richtungskorrektur nur im frühen Startup zulassen, als Startwert höchstens etwa 25°. Ab Active bleibt die Richtung fest. Neue Combo-Schläge dürfen sich erneut ausrichten.
5. Rückstoß der ersten beiden Lights so reduzieren beziehungsweise durch Schritte ausgleichen, dass eine gut getimte Kette am Gegner bleibt. Finisher und Heavy schaffen bewusst Abstand.
6. Combo-Anschluss nach bestätigtem Treffer früher erlauben als nach Fehlschlag. Den vorhandenen kurzen Buffer beibehalten und mit den tatsächlichen Clipfenstern abstimmen. Bewegung in der späten Recovery wieder freigeben, sobald die Pose dies trägt.

**Ergebnis:** Light ist die schnelle Druckaktion, Heavy der kräftige Abschluss. Gegner sind aus einem sichtbar plausiblen Abstand erreichbar, und die Kette scheitert nicht regelmäßig am eigenen Rückstoß.

### B. Kick und brauchbaren Jump-Hit gemeinsam bauen

Die vorhandene Kontaktauswahl von linker/rechter Faust auf einen kleinen expliziten Typ für Faust/Fuß erweitern. Alle Angriffe verwenden weiterhin dieselbe Animator-Phasenquelle und dieselben Trefferfilter.

- **Bodenkick:** Vorwärtskick mit sichtbarem Anheben, Strecken und Zurückziehen des Beins. Taste **L**, Gamepad **rechte Schultertaste / RB / R1**. Ziel: ungefähr 20–30 % mehr nutzbare Reichweite als Jab, getragen von Fußpose und Körpereinsatz.
- **Rolle:** Abstandskontrolle und bewusster Combo-Abschluss. Langsamer als Light, weniger Schaden als Heavy, erkennbare Erholung bei Fehlschlag. Kein unbegrenztes Light/Kick-Canceln.
- **Startwerte für den Bodenkick:** 0,18 s Startup, 0,12 s Active, 0,30 s Recovery, 18 Schaden. Einstieg aus neutral oder als Alternative nach Light 2; nach dem Kick endet die Kette. Dies sind Tuning-Ausgangswerte, keine bereits bestätigte Balance.
- **Luftangriff:** J und L starten airborne denselben klar diagonal abwärts gerichteten Air-Kick. K erhält eine dazu passende kräftigere Air-Smash-Pose. So bleiben Light/Heavy in der Luft verfügbar, und J liefert bereits den intuitiven Jump-Hit.
- **Startwerte Air-Kick:** ungefähr 0,10 s Startup, 0,18 s Active, 0,16 s Recovery, 14 Schaden. Ziel ist ein brauchbares Eingabefenster im späteren Aufstieg, um den Scheitelpunkt und beim Fallen; getroffen wird nur bei echter geometrischer Überschneidung.
- Gravitation und Luftsteuerung bleiben aktiv. Keine Zielhöhen-Korrektur, keine Teleports, kein Schweben bei Hitstop. Landung beendet eine noch laufende Luft-Trefferphase und führt in eine kurze Landepose; dieselbe Attacke darf am Boden nicht erneut treffen.

**Ergebnis:** Der Spieler kann auf einen Boden-Gegner zuspringen und mit einem nachvollziehbar getimten Tastendruck treffen. Ein weit entferntes oder zu hoch überflogenes Ziel bleibt ein Fehlschlag.

### C. Lauf-, Sprung- und Landebewegungen sichtbar machen

- Füße deutlicher anheben und vorführen, Stütz- und Schwungphase trennen, Armschwung und Oberkörper gegenläufig bewegen. Größere lesbare Silhouettenwechsel aus der tatsächlichen Spielkamera beurteilen.
- Schritttempo an tatsächlich zurückgelegte Bodenstrecke koppeln. Gegen einen blockierenden Körper zu drücken darf keinen schnellen Lauf auf der Stelle erzeugen.
- Idle → Anlaufen → Laufen → Stoppen mit kurzen Übergängen verbinden. Dafür nur die wirklich benötigten Posen/Clips ergänzen; ein separates Sprint-System gehört noch nicht dazu.
- Absprung ohne zusätzliche Eingabeverzögerung zeigen, Beine in der Flugphase erkennbar anziehen, Luftangriffe gestreckt ausführen und Landung kurz abfedern.
- Motor und Animator erhalten eindeutige Zuständigkeiten: Combat besitzt Angriffsposen, Locomotion die freie Bewegung, Tod die endgültige Bodenpose. Kein zweites System überschreibt parallel dieselben Transformkurven.

**Ergebnis:** Bewegung wirkt aus normaler Spielentfernung wie Laufen und Springen; nach Angriff oder Landung bleibt keine alte Pose hängen.

### D. Ganzkörper-Tod und vollständigen Reset ergänzen

- Eigene Fallbewegung für das gesamte sichtbare Rig: Körper verliert Halt, kippt und kommt mit passendem Höhenversatz auf dem Boden zum Liegen. Die Pose bleibt bestehen; kein Ragdoll-System erforderlich.
- Bei 0 HP sofort Angriff, Warnmarker, KI-Aktionen und Token beenden. Hurtbox und blockierenden Körpercollider abschalten.
- Todespose darf weder von Idle/Walk noch vom bisherigen Torso-Hitstun überschrieben werden. Die stationäre Trainingspuppe behält eine passende eigene Reaktion.
- Reset stellt Position, Ausrichtung, Rig-Pose, Animator, Collider, HP, Farben, Warnmarker und Kampfzustände wieder her.

**Ergebnis:** Ein besiegter Gegner fällt sichtbar vollständig um, bleibt liegen und blockiert den Spieler nicht. R stellt einen sauber spielbaren Encounter wieder her.

### E. Integration und spielbare Übergabe

- Änderungen gezielt in die bestehenden Assets und gespeicherten Szenen einarbeiten. Den alten S1-Builder nicht als Neuaufbau ausführen; er würde spätere Arbeit überschreiben.
- Builder so nachziehen, dass sie den neuen Stand erhalten beziehungsweise reproduzieren. Prefab-Referenzen, neue Fußkontakte und Szenen-Overrides korrekt speichern.
- Gegner-Heavy bei Bedarf als eigenes AttackDefinition-Asset trennen, damit Spieler-Tuning keine unbeabsichtigte Änderung des Gegenkampfs auslöst.
- Fußkontakt-Debuganzeige sowie Kontakt-VFX und Sound aus der tatsächlichen Kontaktstelle speisen. Wenige klar unterscheidbare Effekte reichen.
- Steuerung, COMBAT_SYSTEM, ARCHITECTURE, ROADMAP und TEST_SLICE_STATUS knapp auf den tatsächlichen Stand bringen; veraltete Aussagen berichtigen.
- SparringLab frisch laden und direkt spielbar übergeben. Kurze deutsche Anleitung: J-Kette, K-Heavy, L-Kick, Sprung + J, Shift, 1/2 und R.

## 5. Schlanker Prüfplan

Während der Umsetzung gezielt kompilieren und die gerade betroffene Aktion ansehen. Neue automatisierte Prüfungen konzentrieren sich auf die neuen Fehlerklassen:

1. **Erfolgreicher Jump-Hit:** In der gespeicherten Sparring-Szene über normalen Sprung-/Angriffsinput einen Gegner tatsächlich verletzen, höchstens einmal je Attacke, danach natürlich landen. Wenige repräsentative Zeitpunkte prüfen; zusätzlich einen klaren Höhen-/Abstands-Fehlschlag erhalten.
2. **Kick-Reichweite:** Unter denselben Startbedingungen trifft der Kick außerhalb der Jab-Reichweite; außerhalb der sichtbaren Kick-Reichweite trifft auch er nicht.
3. **Tod/Reset:** Gegner liegt mit dem ganzen Rig, verursacht keinen Schaden und keine Blockade, gibt das Token frei und wird durch Reset vollständig wiederhergestellt.
4. **Neue Angriffsbewegung:** Schritt bleibt an Kollision und Arena begrenzt; Hitstop oder schwankende Framezeit vervielfacht ihn nicht. Bestehende Sweep-Tests um bewegten Angreifer ergänzen, falls die Implementierung diese Kontaktbahn verändert.

Bewusst geänderte Regeln erhalten passende Tests: Der bisherige vollständige Movement-Lock ist mit kontrolliertem Schritt nicht mehr die gewünschte Regel. Der alte Luft-Fehlschlag muss einen geometrisch echten Fehlschlag abbilden.

Nach der Integration ein vollständiger Lauf der vorhandenen EditMode-/PlayMode-Suite, weil Motor, Combat und gemeinsame Assets betroffen sind. Danach nur betroffene Tests wiederholen, wenn Änderungen oder Fehler dies rechtfertigen. Kein erneutes S0-Werkzeug-Gate und keine breite Geräte-/FPS-Matrix ohne konkreten Anlass.

Zusätzlich eine kurze Sicht- und Spielkontrolle: laufen/stoppen, Light-Kette, Heavy, Kick, Jump-Hit, Ausweichen, Tod/Reset und Zwei-Gegner-Modus. Bestehende Skripte verwenden, normales Play vor PlayMode-Tests beenden und asynchrone Resultate vollständig auslesen. Maßstab der Übergabe bleibt das tatsächliche Spielgefühl.

## 6. Einordnung in den Gesamtplan

Dieser Polish ist **B1** der aktuellen [Roadmap mit neun Bulks](ROADMAP.md). Danach folgen passende fertige Cartoon-Modell-/Rig-/Animationsbasis mit individueller Anpassung (B2), vollständiges Charakterduell einschließlich Run/Trefferreaktion/Knockdown/GetUp (B3), Kampftiefe (B4), Gegnerfamilie (B5), Junkyard-Kapitel (B6), Umgebung/Präsentation (B7), Bedienung/Spielschleife (B8) und Balance/Demo einschließlich WebGL-Prüfung (B9).

Der B1-Stand ist implementiert; Nachweise stehen in TEST_SLICE_STATUS.md. Dieser Plan bleibt die Beschreibung des ursprünglichen Polish-Umfangs. Die nächste konkrete Umsetzung steht in [NEXT_STEPS.md](NEXT_STEPS.md), Stil und effiziente Asset-Nutzung in [PRODUCTION_GUIDELINES.md](PRODUCTION_GUIDELINES.md). Der begonnene komplette eigene Modell-/Rig-Pfad ist zurückgestellt.

## 7. Arbeitsweise und Grenzen

- Ein kurzer Befund, ein zusammenhängender Implementierungs-Bulk, eine spielbare Übergabe. Keine Freigabe pro internem Arbeitspaket oder normaler Designentscheidung.
- Die bestehende Architektur weiterentwickeln; keinen allgemeinen Combat-Framework-Neubau beginnen.
- Browsergame ausschließlich lesen. Unity-Dateien erhalten; kein Commit/Push, Asset-Kauf, zusätzlicher Dienst oder Editor-Upgrade aus diesem Plan ableiten.
- „go b1“ beauftragt den aktuellen B1-Abschluss. Die folgenden Bulks richten sich nach ROADMAP und NEXT_STEPS und benötigen einen eigenen Fortsetzungsauftrag.

