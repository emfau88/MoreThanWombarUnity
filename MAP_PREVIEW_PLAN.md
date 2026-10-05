# M1 — vorgezogene Aufwertung der Testmap

Stand: 4. Oktober 2026. Auf Nutzerwunsch nach B3b und vor B3c umgesetzt. Dieser Text hält den ursprünglichen Umfang fest; Lieferung/Quellen in [M1_MAP_PREVIEW.md](M1_MAP_PREVIEW.md). Ein kompakter Präsentations-Bulk, kein fertiges Kapitel.

## Zielbild

Eine kleine Junkyard-Kampffläche, die als Ort erkennbar ist: abgenutzter Beton/Asphalt, Container, Reifen und Schrott am Rand, Zaun und entfernte Werkstattformen. Warme Arbeitsbeleuchtung gegen einen kühleren Hintergrund. Illustrativer Cartoon-/Comic-Stil mit glatten Silhouetten und zurückhaltenden gemalten Details. Keine sichtbar facettierte Low-Poly-Richtung.

Der aktuelle Stand hat einen gleichmäßigen Plattenboden mit Raster, gelbe Randmarkierungen, zwei Container und einen leeren Hintergrund. Diese Basis aufwerten; der freie Kampfbereich bleibt der Mittelpunkt. Bereits brauchbare Container und vorhandene Kamera/Lichttechnik weiterverwenden.

## Ein Bulk mit fünf Arbeitsschritten

| Schritt | Konkrete Umsetzung | Sichtbarer Effekt |
| --- | --- | --- |
| 1. Kleine Asset-Auswahl | Höchstens drei passende Quellen prüfen, eine zusammenpassende Grundlage auswählen. Benötigt: Reifen, Fässer, Palette/Kisten, Zaun und wenige Schrottteile; passende Boden-/Metalltexturen. Lizenz, tatsächlichen Inhalt und Unity/URP-Materialien prüfen. Nur benötigte Dateien importieren. | Glaubwürdige Details ohne eigene komplette Modellproduktion |
| 2. Boden und Spielfeldrand | Raster optisch zurücknehmen; abgenutzten Beton/Asphalt mit wenigen Rissen, Flecken und verblassten Markierungen einsetzen. Gelbe Randstreifen sparsamer als echte Sicherheitsmarkierungen verwenden. Boden flach halten; Details als Material-/Mesh-Overlays nach vorhandener Technik. | Die Fläche wirkt wie ein Hof statt wie eine Testplattform |
| 3. Umgebung komponieren | Bestehende Container neu gruppieren; zwei bis drei kleine Objektgruppen am seitlichen/hinteren Rand. Dahinter Zaun und eine einfache entfernte Werkstatt-/Schrottsilhouette; Blick über die Arenakante schließen. Freie Mitte und niedriger Vordergrund, keine Sichtblockade. | Mehr Tiefe, ein erkennbarer Ort und gezielte statt beliebige Dekoration |
| 4. Materialien, Licht und Kamera | Rostrot, gedämpftes Petrol und warme Sand-/Betontöne gemeinsam abstimmen. Bestehendes warmes Hauptlicht/kühles Fülllicht verbessern, weiche Bodenschatten und dezente atmosphärische Tiefe. Höchstens ein kleines Arbeitslicht als Akzent. Kameraausschnitt nur soweit nötig anpassen, damit Figuren und Bodenmarker deutlich bleiben. | Zusammenhängende Cartoon-Stimmung und besserer Bodenkontakt |
| 5. Kurze Spielkontrolle und Übergabe | Vorher/Nachher aus derselben Spielkamera ansehen; ein kurzer Kampf mit Bewegung an den Rändern, Sprung, Warnmarker, Knockdown und R. Nur bei tatsächlicher Gameplay-Änderung betroffene Regelprüfung. | Sichtbar verbesserte und weiterhin direkt spielbare Map |

## Effiziente Umsetzung

- Fertige Props/Materialgrundlagen zuerst verwenden. Einfache Platzierung und Materialabstimmung im normalen Unity-/URP-Ablauf; kein neuer Terrain-, Shader-, Import- oder Mapgenerator.
- Die Asset-Auswahl ist abgeschlossen, sobald die wenigen benötigten Objekte stilistisch zusammenpassen. Keine Suche nach einem perfekten vollständigen Junkyard-Paket. Fehlt nur ein Detail, dieses weglassen oder eine vorhandene einfache Form weiterverwenden.
- Die aktuellen Combat-Assets, Spawns, Ebenenhöhe und Motorgrenzen als Grundlage erhalten. Randdekoration zunächst außerhalb des begehbaren Bereichs ohne zusätzliche Gameplay-Collider. Die sichtbare Begrenzung muss zu den tatsächlichen Grenzen passen.
- Änderungen in HumanoidCombatLab integrieren; neue Umgebung in einem eigenen Environment-Unterbaum/Prefab und eigene Materialableitungen unter Assets/Game/Environment. Gemeinsame alte Lab-Materialien nicht global überschreiben. Vorhandene frühe Szenenbuilder nicht neu ausführen.
- Normale URP-Materialien und wenige gemeinsame Texturen/Materialien, nur nötige Schattenlichter. Aufwendiges Wasser, Echtzeitreflexionen, Partikelwolken oder ein eigener Outline-Shader gehören nicht in diesen Bulk.
- Keine neuen Käufe als Voraussetzung einplanen. Eine passende kostenpflichtige Alternative nur mit konkretem Nutzen und Kosten vorlegen; vorhandene oder lizenzgeklärte kostenlose Grundlagen reichen als Ausgangspunkt.

## Fertig, wenn

Die Gesamtansicht vermittelt einen Junkyard-Hof; Boden, Rand und Hintergrund haben erkennbare Unterschiede. Die Mitte bleibt ruhig, Figuren und gegnerische Warnung sind deutlich sichtbar. Die Szene ist gespeichert, direkt spielbar und hat beim kurzen Durchlauf keine neuen Hindernis-/Boden-/Kameraprobleme. Vorher/Nachher-Bilder und verwendete Quellen sind dokumentiert.

Umfang bewusst klein: eine bessere Kampfumgebung. Weitere Kampfbereiche, Laufstrecke, Wellen, Interaktionen und Checkpoints bleiben B6; finale Weltgestaltung und größere Audio-/VFX-Ausarbeitung B7. Wombat-Look separat offen.

## Danach

B3c stimmt das Duell bereits in dieser Umgebung ab und prüft den kleinen Build. B6/B7 können die ausgewählten Props und Materialien später weiterverwenden. Falls Zeit/Aufwand wächst, zuerst zusätzliche Dekoration und Effekte streichen; Boden, Komposition und Licht bilden den Kern.
