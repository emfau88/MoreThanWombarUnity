# Konkreter Arbeitsplan — Asset-Basis und vollständiges Charakterduell

Stand: 4. Oktober 2026. B1 ist abgeschlossen. B2-Schritte 1–3 liefern Auswahl, technischen Import und Humanoid-Combat mit vorhandenen Spielregeln. [B2_ASSET_SELECTION.md](B2_ASSET_SELECTION.md) enthält die Auswahl, [B2_COMBAT_INTEGRATION.md](B2_COMBAT_INTEGRATION.md) die konkreten Zeiten und Kontaktanbindung. Wombat-Look bleibt ausdrücklich eine spätere Entscheidung. Nächste technische Arbeit ist B3a: Run, unterscheidbare Kernangriffe statt vorläufiger Cross-Varianten und sichtbare Trefferreaktionen. Danach B3b mit Knockdown/GetUp/Death und B3c mit Duell-Tuning/Buildnachweis; die Reihenfolge steht in ROADMAP.md. Diese weiteren Schritte wurden beim Commit nicht begonnen.

## 1. Bestand und Entscheidung

| Vorhandener Stand | Entscheidung / konkrete Folge |
| --- | --- |
| LabInput, PlayerMotor, CharacterDefinition und MotorMath | Behalten; direkte Eingaben, Richtungswechsel, Luftkontrolle und später Run gezielt abstimmen |
| CombatController, AttackDefinition, Buffer, Attack-Instanzen, Front-/Teamfilter | Behalten; gewünschte Angriffszeiten und Humanoid-Clip-Mapping sind in B2 Schritt 3 integriert |
| Animierte Faust-/Fuß-Sweeps auf getrennten Hurtboxes | Regel behalten; Humanoid-Kontaktbahnen sind am echten Avatar gebacken und gegen sichtbare Posen geprüft |
| B1: Angriffsschritte, Kick, Air-Kick/Air-Smash, spätes Movement-Release | Behalten; einmal anhand echter Nutzersequenz fein abstimmen |
| TrainingDummy, PlayerDefense, EnemyBrain, EngagementCoordinator | Wiederverwenden; animierte Trefferreaktion und lebendes Knockdown/GetUp fehlen noch |
| Kamera, Testarena, gespeicherte Trainings-/Sparring-Szenen, HUD, Reset | Behalten als kurze Integrationsumgebung; Arena genügt für B2/B3 |
| Grundform-Figur und selbst erzeugte Standardclips | Spielbare Vergleichsbasis; durch passende importierte Basis ersetzen, kein weiterer großer Standardanimationsbau |
| Blender-Wombat, eigener Skeleton-Entwurf und JSON-Mesh-Bibliothek | Erhalten und zurückstellen; noch nicht in die Spielfigur integriert, kein Grund für zusätzliche Importarchitektur |
| Geplanter kompletter eigener Modell-/Rig-/Animationspfad | Ersetzen durch Asset-Auswahl, Retargeting und begrenzte Anpassung für die Wombat-Identität |
| Run, animierter Flinch, lebendes Knockdown/GetUp | In B3 vorziehen; vor Ausbau auf weitere Rollen fertigstellen |
| Build/Browserleistung | Noch nicht nachgewiesen; kleinen WebGL-Nachweis im Character Slice einplanen |

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

Umgesetzt in `HumanoidCombatLab`: eigene AttackDefinitions mit Startup-/Active-/Recovery-Sekunden, abschnittsweise retimte Humanoid-Clips und am Avatar ausgewertete Kontaktpunkte. Animator-Zeit bleibt die einzige Phasenquelle; Runtime-Sweeps verwenden die gespeicherte Bahn ohne Umposen des sichtbaren Humanoids. Jab/Cross, eigene Kick-/Luftkick-Ableitungen, vorläufige Cross-Finisher/Heavy/Air-Heavy, bestehende Defense/Gegner/Feedback und Reset sind angebunden. Zeit-/Clip-/Avataränderungen benötigen Neuableitung/Bake über HumanoidCombatBuilder. Details im Integrationsdokument.

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
