# LF2-inspirierte Stage — verbindliches Ausbauziel

Stand: 9. Oktober 2026. **B9–B12 sind implementiert und als WebGL-Stage veröffentlicht**, B13–B15 stehen aus. B12-Ablauf und Nachweise: [B12_LF2_STAGE.md](B12_LF2_STAGE.md). Nachweise und offene Geräte-/Spielgefühlabnahmen: [B9_B11_LF2_COMBAT.md](B9_B11_LF2_COMBAT.md). Der öffentliche Play-Link enthält B12; [Release-Nachweis](B12_WEBGL_RELEASE.md). Grundlage sind die Nutzerentscheidung für Little-Fighter-2-Spielgefühl und der vorhandene B8-Spielstand. Dieser Plan ersetzt die bisherige Vorwärtsplanung „B9 = nur Balance/Performance/Demo“. B1–B8 bleiben das nutzbare Fundament.

## 1. Ziel und Einschätzung

**Ein vollständig spielbarer, ausgearbeiteter Charakter und eine abgeschlossene Stage mit ungefähr 30 Gegnern aus höchstens drei Gegnercharakteren.** Kämpfer arbeiten sich durch eine zusammenhängende Strecke, können mehrere Gegner wirksam treffen und gewinnen durch Positionierung, Fähigkeiten und Kombinationen. Humor und illustrativer Cartoon-/Comic-Stil der ursprünglichen 2D-Referenz bleiben erhalten. Keine sichtbaren Low-Poly-Facetten als Stilziel.

Der Umfang ist angemessen: groß genug, um das gewünschte Spielgefühl zu beurteilen, und klein genug, um einen Charakter wirklich auszuarbeiten. Er ist eine substanzielle Erweiterung des Prototyps, kein kleiner Balance-Patch. Die größten Aufgaben sind Fähigkeiten/Animationen, Gruppenkampf und gute Gegnerunterscheidung; sieben zusätzliche Gegner allein würden die bisherige Wiederholung nicht beheben.

**30 bedeutet Gegner insgesamt pro regulärem Durchlauf, nicht 30 gleichzeitig.** Zunächst acht gleichzeitig in einer repräsentativen Kampfsituation aufbauen, anschließend auf acht bis zehn als Zielbereich abstimmen. Zwölf sind allenfalls ein späterer Versuch nach Geräteprüfung. Diese Zahlen sind Designziele, keine nachgewiesene Android-Leistung. Bei Engpässen kleinere überlappende Verstärkungen verwenden; die Gesamtzahl bleibt erhalten.

Eine kompakte Stage von einigen Minuten ist dafür sinnvoll. Das frühere Ziel von 10–15 Minuten ist für diesen Umfang aufgehoben: keine zusätzlichen Wellen oder zähen Lebensbalken nur zur Spielzeitverlängerung. Dauer und Schwierigkeit erst anhand tatsächlicher menschlicher Durchläufe festlegen.

## 2. Lieferumfang und Grenzen

| Bestandteil | Ziel dieser Ausbaustufe |
| --- | --- |
| Spieler | Genau ein auswählbarer Charakter mit vollständigem Grundkampf, drei unterschiedlichen Spezialfähigkeiten, Ressourcenregeln, passenden Animationen, Ton, Effekten und verständlicher Bedienung |
| Stage | Ein zusammenhängender Schrotthof mit drei unterscheidbaren Bereichen, abwechslungsreichem Ablauf, Checkpoints, Finale und klar erkennbarem Abschluss |
| Gegner | Planungswert exakt 30: 21 leichte Nahkämpfer, fünf Werfer, vier schwere Gegner; Anpassungen um wenige Gegner nur bei begründetem Spielgefühlbedarf dokumentieren |
| Gleichzeitige Gegner | Zunächst acht, Ziel acht bis zehn; höchstens zwei aktive Werfer gleichzeitig, keine unübersichtliche Ansammlung schwerer Gegner |
| Finale | Vorarbeiter als erkennbare Elitevariante des schweren Gegners; zählt zu den 30 und erzeugt keinen vierten Gegnertyp |
| Plattformen | Desktop und vorhandene Android-Touch-Steuerung im WebGL-Build; Windows-Build als lokale Übergabe |
| Verbündete | Team-/Zielwahl vorbereiten. Ein einfacher KI-Begleiter ist die bevorzugte Erweiterung nach dem Kernumfang, keine Voraussetzung für die 30-Gegner-Stage |
| Nicht enthalten | Weiterer spielbarer Charakter, Online-/lokaler Koop, große Kampagne, Inventar-/Loot-System, Skilltree, prozedurale Levels, eigener mehrphasiger Boss |

**Was „vollständiger Charakter“ bedeutet:** Kein Sammelsurium unfertiger Testaktionen. Alle benötigten Zustände vom Einstieg bis zum Tod/Neustart funktionieren; Angriffe haben erkennbare Zwecke und Grenzen; Bewegung, Treffer, Verteidigung und Spezialaktionen passen visuell zusammen. Der Charakter hat eine konsistente Erscheinung und verständliche Einführung.

Die vorhandene menschliche Figur ist die technische Ausgangsbasis und kann als bewusst gestalteter menschlicher Stage-Charakter abgeschlossen werden. Die endgültige Wombat-Figur ist weiterhin eine offene Gestaltungsentscheidung. Diese Entscheidung wird spätestens in B10 für diese Stage festgehalten: menschliche Figur bewusst ausarbeiten oder ein geeignetes fertiges Rig zur Wombat-Figur anpassen. Ein unveränderter Test-Humanoid wird nicht als fertiger Wombat ausgegeben. Ein kompletter eigener Modell-/Rig-Neubau wäre ein zusätzlicher Art-Bulk und wird als Umfangszuwachs gemeldet.

## 3. Kampfdesign des einen Charakters

Die Identität ist ein beweglicher Nahkämpfer mit kräftigen Flächen- und Durchbruchaktionen. Tiefe entsteht durch unterschiedliche Einsatzzwecke, Timing, Positionierung und Anschlüsse; nicht durch möglichst viele Tasten.

| Aktion | Aufgabe und gewünschte Entscheidung |
| --- | --- |
| Light-Kette | Schneller Druck und günstiger Anschluss gegen einzelne Gegner; bestehende Jab/Cross/Haken-Basis behalten |
| Kick | Abstand schaffen und Gegner für Folgeaktionen gruppieren; breiter Kontakt darf nahe Ziele mitnehmen, bleibt räumlich begrenzt |
| Heavy | Langsamer, klar angekündigter starker Nahkampftreffer mit Unterbrechung/Niederwerfen; Fehlschlag ist angreifbar |
| Sprung und Luftkick | Position wechseln, über Angriffe kommen und gezielt in den Kampf einsteigen |
| Ausweichen | Verlässliche aktive Verteidigung mit vorhandenen Zeitfenstern; kein zusätzlicher Block-/Parry-Komplex in diesem Umfang |
| Spezial 1: Boden-Smash | Vorhandenen Luft-Smash zu einem Bodenaufprall mit radialem AOE entwickeln. Trifft mehrere nahe Gegner, leichte Gegner fallen schnell; schwere werden nicht automatisch beseitigt |
| Spezial 2: Durchbruch | Schulterstoß zu einer kurzen gerichteten Gruppenaktion ausbauen. Durch leichte Gegner hindurch, pro Ziel einmal; Wände und schwere Blockierer beenden den Lauf. Erholung bleibt verwundbar |
| Spezial 3: Druckwelle | Ein gerichtetes, begrenzt durchdringendes Projektil. Erreicht Werfer und Gegner in einer Linie; schmaler als der Smash, keine automatisch suchende Attacke |

**Ressource:** Ein einfacher MP-/Energiebalken für die drei Spezialaktionen. MP regeneriert langsam; erfolgreiche Grundangriffe unterstützen die Erholung. Trefferbonus pro Angriffsinstanz deckeln, damit ein AOE auf acht Gegner seinen eigenen Preis nicht beliebig zurückbezahlt. Normale Angriffe, Sprung und Ausweichen bleiben ohne MP nutzbar. Keine zusätzliche Combo-Währung und kein Lebenspunktepreis für Spezialaktionen.

Kosten, Schaden und Radien zunächst als editierbare Werte anlegen. Startziel für leichte Gegner: wenige normale Treffer oder ein gut platzierter voller Smash, ohne dass jede Spezialaktion jede Gegnerrolle sofort entfernt. Schwere Gegner erhalten erkennbare Widerstands-/Unterbrechungsregeln statt permanenter Unverwundbarkeit. Keine endlose Stunlock-Kette und keine unentrinnbare Spieler-Trefferkette.

**Bedienung:** Vorhandene Aktionen beibehalten. Luft-Heavy löst den Boden-Smash aus, STOSS den Durchbruch; für die Druckwelle kommt höchstens ein zusätzlicher direkt erreichbarer Spezialbutton hinzu. Tastatur-/Gamepad-Belegung passend zu den freien bestehenden Actions ergänzen. Keine verpflichtenden komplizierten LF2-Tastensequenzen auf dem Handy. Touch-Erreichbarkeit schon in B10 prüfen, nicht erst beim Release.

**Kombinationen als Qualitätsprobe:** Kick → gruppierte Gegner → Smash; Ausweichen → Light-Anschluss; Durchbruch zum Werfer → Nahkampf. Jede Spezialaktion braucht außerdem eine Situation, in der sie weniger geeignet ist. Eingabepuffer und erlaubte Anschlüsse gezielt nutzen; kein universelles Abbrechen jeder Erholung.

## 4. AOE und Mehrfachtreffer verständlich zeigen

- Smash: kurzer Bodenring/Staubimpuls im tatsächlichen Schadensradius, ausgelöst am sichtbaren Bodenaufprall. Keine Schadensauslösung mitten in der Luft und kein Effektkreis außerhalb der Trefferfläche.
- Durchbruch: schmale Bewegungsspur und einzelne Kontaktimpulse entlang des Wegs; kein großer Kreis, der radialen Schaden suggeriert.
- Druckwelle: klar sichtbares Projektil mit passender Breite, Reisegeschwindigkeit und Ende an Hindernissen oder Reichweitengrenze.
- Jedes getroffene Ziel reagiert sichtbar durch Trefferpose, kurzen Blitz und passenden Rückstoß. Ungetroffene Figuren reagieren nicht wie Treffer. Optional eine kurze Anzeige „3 Treffer“, nur bei Mehrfachtreffern und nur wenn sie die Lesbarkeit verbessert.
- Pro Angriff und Ziel höchstens ein Treffer, sofern kein ausdrücklich entworfener Mehrfachschlag vorliegt. Effekte und Ton bündeln; kein achtfach gestapelter Bildschirmshake oder globaler Hitstop bei acht Treffern.
- Spieler, Gegnerwarnung und Schaden unterscheiden sich über Form, Bewegung und Ton, nicht ausschließlich über Farbe. Effekte lassen Boden, Charaktere und anlaufende Angriffe sichtbar.

## 5. Drei Gegnercharaktere mit unterschiedlichen Aufgaben

| Gegner | Verhalten | Lesbarkeit und Umgang |
| --- | --- | --- |
| Leichter Raufbold, 21 Stück | Einfacher Nahkämpfer mit wenig Lebenspunkten; erzeugt Masse und lässt sich wirksam verdrängen/besiegen | Kleine/leichte Silhouette, kurzer Angriff, gut sichtbarer Knockdown; befriedigende AOE-Ziele, aber zusammen gefährlich |
| Werfer, 5 Stück | Sucht Abstand und kündigt einen geraden Wurf an; begrenzte Feuerrate, kein zielsuchendes Dauerfeuer | Sichtbares Wurfobjekt und Ausholpose; Bewegung, Deckung oder gezielter Durchbruch beantworten den Druck |
| Schwerer Schläger, 4 Stück | Langsamer, gefährlicher Nahkampf; hält mehr aus und kontrolliert Raum | Breite Silhouette, längere Warnung und deutliche Erholung; unterbrechbar nach definierten Regeln, kein bloßer HP-Sack |

Der Vorarbeiter ist einer der vier schweren Gegner. Er erhält eine erkennbare äußere Variante und ein stärkeres vorhandenes Angriffsmuster, aber kein separates Boss-System.

Standard/Agile/Heavy aus B5 sind vorhandene technische Rollen. Für diese Stage ersetzt der Werfer die bisherige Agile-Rush-Rolle; nicht zusätzlich einen vierten Gegnertyp einführen. Bereits vorhandene Laborvarianten können erhalten bleiben. Rollen dürfen Rigs, Clips und Basismodelle teilen, müssen im tatsächlichen Spielbild durch Silhouette/Accessoire, Haltung und Verhalten unterscheidbar sein. Drei Farben auf demselben Kämpfer genügen nicht.

## 6. Stage mit 30 Gegnern und wechselndem Rhythmus

Die bestehende Strecke Anlieferung → Sortierhof → Presswerk wird überarbeitet. Drei Bereiche bedeuten nicht dreimal dieselbe Folge von drei vollständig abzuräumenden Wellen. Encounter werden beim Vorankommen oder durch begrenzte Verstärkungen ausgelöst; nur an sinnvollen Bereichsabschlüssen muss alles besiegt sein.

| Abschnitt / Begegnung | Leicht | Werfer | Schwer | Gesamt | Zweck |
| --- | ---: | ---: | ---: | ---: | --- |
| Anlieferung: Einstieg | 5 | 0 | 0 | 5 | Bewegung, Combo und erster gut lesbarer AOE-Erfolg |
| Anlieferung: Vorstoß zum Tor | 3 | 0 | 0 | 3 | Beim Weitergehen Verstärkung; keine zusätzliche lange Arenasperre |
| Sortierhof: gemischter Hauptkampf | 5 | 2 | 1 | 8 | Werfer priorisieren, leichte Gegner bündeln, Heavy umgehen |
| Sortierhof: Durchgang | 4 | 1 | 1 | 6 | Bewegter Kampf statt erneuter vollständiger Wellenfolge |
| Presswerk: Finale | 4 | 2 | 2 | 8 | Bekanntes Können zusammenführen; einer der beiden schweren Gegner ist der Vorarbeiter |
| **Summe** | **21** | **5** | **4** | **30** | **Drei Gegnertypen, fünf Begegnungen, eine Stage** |

Bei Verstärkungen dürfen Restgegner der vorigen Begegnung bestehen bleiben. Ein gemeinsames Limit lässt zunächst höchstens acht, später gegebenenfalls zehn gleichzeitig zu; weitere geplante Gegner warten an sinnvollen Zugängen. Maximal zwei Werfer gleichzeitig. Keine Spawns unmittelbar auf dem Spieler oder unvorhersehbar hinter ihm ohne lesbaren Zugang. Große Kampfstellen brauchen ausreichend freie Breite und Tiefe zum Gruppieren und Ausweichen.

Jeder Gegner gehört genau einer Begegnung. Das Zurückkehren in einen Trigger erzeugt keine zusätzlichen Gegner. Checkpoint-Retry rekonstruiert den betreffenden Abschnitt einschließlich noch ausstehender Verstärkung, HP/MP und Torzustand; bereits abgeschlossene Bereiche bleiben abgeschlossen. Die 30 beziehen sich auf einen Durchlauf ohne Wiederholungen durch Niederlagen. Sieg erst nach allen vorgesehenen Begegnungen und dem Finale, nicht allein nach Tod des Vorarbeiters.

Kurze Lauf-/Erholungspausen, klarer Ausgang und die vorhandene Torinteraktion bleiben. Heilung an Checkpoints und MP-Wiederherstellung gemeinsam abstimmen. Keine neue Loot-/Pick-up-Architektur nötig. Die Map wird gezielt an Kampfflächen, Sichtlinien, Spawnzugängen und drei Bereichssilhouetten verbessert; kein zweites Level bauen.

## 7. Technische Umsetzung auf dem Bestand

| Vorhanden | Gezielte Anpassung |
| --- | --- |
| PlayerMotor, LabInput, bestehende Unity-UI/Touch-Controls | Weiterverwenden; eine neue Fähigkeit, MP-Anzeige und passende Eingabe ergänzen |
| CombatController, AttackDefinition, Kontaktbahnen, Hit-Set | Nahkampf erhalten; klar getrennte Trefferformen für Sweep, Aufprallfläche und Projektil ergänzen. Gemeinsame Team-/Schutz-/Einmal-pro-Ziel-Regeln |
| PlayerDefense, TrainingDummy, BodyRecovery | Vorhandene Reaktionen/Schutzfenster behalten; kleinen gemeinsamen Zugang zu Team, Position, Lebensstatus und Schaden schaffen |
| EnemyBrain und EngagementCoordinator | Direkte Bindung an nur einen Player auflösen, wo für Zielwahl nötig; globale Einzel-Angriffsfreigabe durch begrenzten lokalen Gruppendruck ersetzen |
| Wartepositionen/Körperkollision | Bisherige Vierer-Aufstellung für acht bis zehn Figuren erweitern; keine blockierenden Trauben an einem einzigen Zielpunkt |
| Hitstop/Combat-Uhr | Prüfen und begrenzen, wo der Spieler-Hitstop derzeit alle Gegner anhält; Trefferreaktionen passend zur betroffenen Figur steuern |
| ChapterDefinition und Kapitelablauf | Begegnungen mit Trigger, endlichem Spawnvorrat und Verstärkungslimit ergänzen; Szene, Checkpoints und Menüs erhalten |
| Quaternius-/Kenney-Grundlagen und vorhandene Import-/Buildskripte | Rigs, Clips, Props, Audio und Unity-Batch-/CLI-Workflow verwenden; Eigenarbeit auf fehlende Aktion und Stilangleichung begrenzen |

Wichtigster technischer Unterschied zu B8: Dort sind maximal vier Gegner und eine globale Angriffsfreigabe üblich. Einfach mehr Figuren zu spawnen erzeugt eine Warteschlange oder unlesbaren Druck. Gruppenkampf braucht verteilte Positionen, begrenzte gleichzeitig beginnende Angriffe und verständliche Fernkampfwarnungen. Startwert: zwei gleichzeitig aktive Nahkampfangreifer am Spieler, Fernangriffe zusätzlich zeitlich abstimmen; nach Spielgefühl anpassen.

Teamregeln: Spieler und spätere Verbündete verursachen untereinander standardmäßig keinen Schaden. Gegner können den jeweils relevanten gegnerischen Kämpfer wählen. Noch keine universelle Ability-Architektur, ECS-Umstellung, neue KI-Bibliothek oder eigene Physiklösung bauen. Pooling für kurzlebige Projektile/Effekte nutzen, falls deren häufige Erzeugung es rechtfertigt; Gegnerpool nicht vorsorglich für 30 einmalige Spawns entwickeln.

## 8. Nächste sieben Bulks

B9–B11 sind lokal integriert: AOE, MP und drei Spezialaktionen, drei Gegnerrollen, gemeinsamer Zielzugang sowie ein Acht-/Zehn-Gegner-Labor. Sechs unterschiedliche gezielte Kampfprüfungen bestehen. Ein physischer Android- und menschlicher Spielgefühlcheck bleiben offen. B12 liefert lokal fünf Begegnungen mit genau 30 Gegnern und endlichem Nachschub; Details in [B12_LF2_STAGE.md](B12_LF2_STAGE.md). B13–B15 sind noch geplant. Nach jedem Bulk ein integrierter spielbarer Stand; keine automatische Veröffentlichung nach jedem Tuningwert.

| Bulk | Arbeit | Ergebnis / Abschlussbedingung |
| --- | --- | --- |
| **B9 — Mehrfachtreffer und erster AOE** | Gemeinsame Trefferregeln, Boden-Smash mit echtem Aufprallradius, individuelle Trefferreaktionen und begrenztes Gesamtfeedback | Spieler trifft mehrere Gegner klar sichtbar; außerhalb stehende Gegner bleiben ungetroffen |
| **B10 — Einen Charakter vervollständigen** | MP, Durchbruch, Druckwelle, Anschlüsse, klare Schwächen, Animationen und direkte Touch-Bedienung; Figurenentscheidung festhalten | Drei unterschiedliche Spezialaktionen und Grundkampf ergeben ein vollständiges, verständliches Repertoire |
| **B11 — Drei Gegner und dichter Gruppenkampf** | Schwache Raufbolde, Werfer und Heavy; Zielwahl/Teamgrundlage, Positionierung und Angriffsfreigaben; erster dichter Gerätecheck | Acht Gegner sind spielerisch lesbar und fair; belastbare Entscheidung über zehn gleichzeitig und Android-Budget |
| **B12 — Eine vollständige 30-Gegner-Stage** | Fünf Begegnungen auf drei Bereichen, Vorwärtsbewegung, begrenzte Verstärkungen, Tor, Checkpoints und Finale | Vom Menü bis zum Sieg durchspielbar; genau 30 geplante Gegner, keine Wiederholungsschleifen oder steckengebliebenen Spawns |
| **B13 — Figuren, Map, Audio und Effekte abschließen** | Konsistente Spielerfigur, drei erkennbare Gegner, Kampfwege/Sichtlinien, abgestimmte AOE- und Treffereffekte, hörbare Rollen | Zusammenhängende illustrative Stage ohne unfertige Kernanimation oder austauschbare Gegnerdarstellung |
| **B14 — Spielgefühl und Gerätebudget** | Kurze menschliche Durchläufe; Schaden, MP, Dichte und Erholung gemeinsam abstimmen; tatsächliches Android-Touch-/Framezeitprofil | Spezialaktionen nützlich ohne Dauer-Spam, keine zähen Restgegner, dokumentierte Gerätequalität |
| **B15 — Spielbare Übergabe** | Finalen WebGL-/Windows-Kandidaten bauen, Anleitung/Status/Lizenzen aktualisieren; Pages-Veröffentlichung im beauftragten Release-Schritt | Vollständige Stage über den Play-Link erreichbar und lokal startbar; bekannte Grenzen ehrlich dokumentiert |

### B9 im Detail — lokal umgesetzt

1. **Trefferpfad gezielt erweitern.** Bestehendes Hit-Set weiterverwenden. Ein gemeinsamer Aufruf verarbeitet Team, lebendig/geschützt, Schaden und Reaktion; Sweep und Flächenabfrage liefern ihre jeweiligen Kandidaten. Radiale Treffer benötigen Distanz-/Höhenprüfung statt des bisherigen reinen Vorwärtsfilters. Keine Treffer durch trennende massive Hindernisse. Keine Projektile oder große Abstraktionsschicht vorziehen.
2. **Boden-Smash spielbar machen.** Luft-Heavy startet den vorhandenen Smash mit angepasstem Ablauf. Schaden am bestätigten Bodenkontakt genau einmal auslösen; bei Unterbrechung/Tod vor Landung entfällt der Angriff. Spätere normale Landungen dürfen keinen gespeicherten Smash nachholen. Bodenhaltung und Recovery passen zum Einschlag.
3. **Wirkung zeigen.** Kurzer passender Bodenimpuls, klare Einzelreaktionen und ein gebündelter Einschlagton. Radius, Schadenszeitpunkt und Effekt stimmen überein. Rückstoß wirkt vom Einschlag nach außen. Hitstop/Shake nicht mit Trefferzahl multiplizieren.
4. **Kurze Kampfsituation.** Drei nahe und ein außerhalb stehender Gegner; anschließend vorhandene Mischgruppe. Light/Kick/Heavy bleiben funktionsfähig. MP folgt in B10; B9 hat bis dahin die bestehende Aktions-Recovery, keinen zweiten provisorischen Ressourcenmechanismus.
5. **Gezielt abschließen.** Prüfen: Innen-/Außentreffer und einmaliger Schaden; Landung/Unterbrechung; Schutz-/Teamregeln. Ein sichtbarer Kampfclip bzw. kurze tatsächliche Spielsequenz genügt für Effekt-/Animationskontrolle. Nur betroffene automatisierte Fälle ergänzen, keine Plattform-Buildserie.

**B9 fertig, wenn:** Der neue Smash trifft eine kleine Gruppe zuverlässig und erkennbar, und seine Schadensfläche lässt sich im Spielbild verstehen. Dies ist noch kein fertiger 30-Gegner-Kampf.

### B10 im Detail — lokal umgesetzt

1. Einfache MP-Daten, Anzeige, Kosten und Wiederherstellung ergänzen; Reset, Pause und Checkpointzustand mitführen.
2. Vorhandenen Schulterstoß zum Durchbruch erweitern; Treffer-Set über die ganze Bewegung halten, leichte Gegner passieren, schwere Gegner/Wände stoppen. Kein mehrfacher Schaden durch Collider-Überlappung.
3. Eine Druckwelle mit klarer Richtung, Reichweite und Lebensdauer ergänzen. Nahkampfregeln für Team, Schutz und Einmaltreffer wiederverwenden; Hindernisse blockieren das Projektil.
4. Die drei Aktionen über wenige direkte Eingaben erreichbar machen. Touch gleichzeitig mit Bewegung nutzbar, Schaltflächen ausreichend groß und ohne verdeckte Kampffläche. Reale Handybedienung mit dem Nutzer abgleichen, soweit das Gerät verfügbar ist.
5. Vorhandene Clips/Posen zuerst verwenden, nur erkennbare Lücken ergänzen. Figurenentscheidung für die Stage dokumentieren; größere Wombat-Modellarbeit vor Einplanung als zusätzlichen Aufwand benennen.
6. MP-Spam, zwei sinnvolle Anschlusskombinationen und Verhalten ohne MP praktisch prüfen. Daten/Steuerung aktualisieren; keine vollständige Stage-Balance vor B12 behaupten.

**B10 fertig, wenn:** Ein Spieler kann erklären und zeigen, wann Smash, Durchbruch oder Druckwelle sinnvoll sind. Keine Aktion ist nur eine anders eingefärbte Kopie.

## 9. Verbündete und spätere Tiefe

Verbündete gehören weiterhin zur gewünschten LF2-Richtung. Der ausdrücklich gesetzte erste Umfang bleibt ein spielbarer Charakter und eine Stage. Deshalb bereitet B11 Teams und Zielwahl vor, ohne den Abschluss an eine weitere vollständige KI-Figur zu binden.

Als nächster begrenzter Ausbau bietet sich **ein KI-Begleiter** an: einmal an festem Stage-Punkt befreien, folgt automatisch, verwendet vorhandene Kämpferaktionen, greift Gegner an, kein Friendly Fire und kein Kommandomenü. Er zählt nicht zu den 30 Gegnern und ist kein zweiter auswählbarer Charakter. Sein Zustand muss mit Checkpoints zurückgesetzt werden; Zielverteilung und Gegnerdruck müssen auch nach seinem Ausfall funktionieren. Multiplayer ist daraus nicht abgeleitet. Diesen Ausbau vorziehen, wenn der Teamkampf als Abnahmekriterium der ersten Stage gewünscht wird; dann als zusätzlichen Bulk sichtbar einplanen.

## 10. Effiziente Prüfung und Abschlusskriterien

**Früh prüfen, wo das Risiko liegt:** B9 prüft AOE/Animation, B10 Bedienung und Fähigkeiten, B11 einen dichten Mischkampf auf der Zieltechnik. Die Leistung von zehn animierten Kämpfern erst am Ende zu entdecken wäre unnötig teuer. Als Arbeitsziel 60 FPS auf dem dokumentierten Test-PC und stabile 30 FPS auf dem konkret benannten Android-Handy; Messwerte und Qualitätseinstellungen festhalten, keine allgemeine Handygarantie. Ohne verfügbares physisches Handy bleibt dieser Nachweis ausdrücklich offen.

Eine kurze menschliche Stage-Runde beurteilt Rhythmus, Nutzen der Fähigkeiten und Wiederholung besser als viele automatische Siege. Gezielte technische Fälle sichern Mehrfachtreffer, Ressourcen-/Retry-Zustand, endliche Spawns und Sieg. Vorhandene passende Prüfungen weiterverwenden. Vollständige Wiederholung erst am Release-Kandidaten oder nach einem konkreten Regressionsbefund; keine großen Testserien nach Farb-/Zahlenänderungen.

Abschluss der Ausbaustufe:

- Ein Charakter mit vollständigem Grundkampf und drei abgestimmten Spezialaktionen, konsistenter Erscheinung und verständlicher Touch-/Desktop-Bedienung.
- Eine Stage vom Hauptmenü bis zum Ergebnis, drei erkennbare Bereiche, ungefähr 30 Gegner aus höchstens drei Typen; geplante Verteilung und tatsächlicher Durchlauf stimmen überein.
- Gegnerzahl erzeugt zeitweise Massengefühl; Gegner stehen nicht überwiegend in einer starren Duell-Warteschlange. Spezialisten verändern Entscheidungen, schwache Gegner lassen AOE befriedigend wirken.
- Sichtbare Trefferfläche und tatsächlicher Schaden passen zusammen. Warnungen, Spielerposition und wichtige Gegner bleiben auch bei Mehrfachtreffern lesbar.
- Niederlage, Checkpoint, vollständiger Neustart und Pause funktionieren einschließlich HP/MP und ausstehender Gegner.
- Repräsentativer Desktop- und tatsächlicher Android-Check samt Einschränkungen; Build/Play-Link eindeutig dem übergebenen Stand zugeordnet.

Umfang früh neu bewerten, wenn ein komplett eigener Wombat-Rig, alle drei Gegner mit vollständig unabhängigen Animationsbibliotheken, 30 gleichzeitig aktive Gegner oder Multiplayer hinzukommen. Das wäre deutlich mehr als dieser Plan. Umgekehrt wäre es zu niedrig gegriffen, nur die vorhandenen 23 Gegner auf 30 zu erhöhen und einen Effektkreis hinzuzufügen: Charaktertiefe, Gruppendruck und Levelrhythmus sind die eigentlichen Ziele.

## 11. Bewährte Bezugspunkte

Die zuvor kurz geprüften Quellen dienen als Designorientierung, nicht als Zusage, dass eine bestimmte Gegnerzahl oder Balance automatisch funktioniert:

- [Little Fighter 2 — offizielle Einführung](https://lf2.net/en/intro.html): Spezialaktionen mit Eingabekombinationen/MP, Stage- und Teamkampf als Vorbild für Charaktertiefe und gemeinsames Kämpfen. Eigene Figuren, Namen, Assets und Sounds verwenden.
- [Little Fighter 2 — Steuerung von Verbündeten](https://lf2.net/control3.html): Anhaltspunkt für die spätere Teamdimension; ein Kommandosystem gehört nicht zur ersten Stage.
- [TMNT: Shredder’s Revenge — Interview mit Designer Frédéric Gémus](https://noisypixel.net/tmnt-shredders-revenge-interview-frederic-gemus/): Gruppen kontrollieren und zügig besiegen; Spezialaktionen unterstützen den Kampfrhythmus. Daraus hier schwache AOE-Ziele und unterschiedliche Einsatzformen ableiten.
- [Streets of Rage 4 — Entwickler über die Wiederbelebung der Serie](https://blog.playstation.com/2020/04/30/streets-of-rage-4-how-three-studios-revived-a-legendary-series/): Bewusste Kosten/Risiken von Spezialaktionen als Referenz. Dieser Plan wählt MP statt eines zusätzlichen Lebenspunkte-Rückgewinnsystems.

Die konkrete Gegnerverteilung, Fähigkeitenauswahl, sieben Bulks und Leistungsziele sind unsere Projektentscheidungen. Sie werden nicht als Zahlenvorgaben dieser Referenzspiele ausgegeben.
