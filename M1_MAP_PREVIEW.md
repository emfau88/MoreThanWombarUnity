# M1 — Junkyard-Testmap

Stand: 4. Oktober 2026. Nach B3b in HumanoidCombatLab integriert und aus der Spielkamera kontrolliert. Geplanter Umfang: [MAP_PREVIEW_PLAN.md](MAP_PREVIEW_PLAN.md).

## Gelieferte Umgebung

Durchgehender abgenutzter Beton statt des Plattenrasters, zurückhaltende gelbe Arbeitsflächen-Markierungen, niedrige Randsteine. Vorhandene Container neu gruppiert und lokal eingefärbt. Drei kleine Schrottgruppen aus importierten Reifen, Autotür, Blech, Stoßstange und Antrieb; ein abgestellter Wagen. Dahinter Kettenzaun, kleine Werkstattgebäude und das Schild „SCHROTT & SÖHNE“. Warmes Hauptlicht, kühles Fülllicht und ein einzelnes schattenloses Arbeitslicht.

Der Boden bleibt flach, die Mitte frei. Die neuen Dekorationen besitzen keine Gameplay-Collider. Boden und Randkollision der bisherigen Arena bleiben erhalten; deren alte Darstellung ist ausgeblendet. Bewegungsgrenzen, Spawns, Gegner, Angriffe und Eingaben wurden nicht geändert. Eigene Materialien verhindern Änderungen an den älteren Vergleichsszenen.

## Übernommene Grundlagen

| Quelle | Tatsächlich übernommen | Anpassung |
| --- | --- | --- |
| [Kenney Car Kit](https://kenney.nl/assets/car-kit), Archiv 3.1 | Sechs FBX: debris-tire, debris-door, debris-plate-a, debris-bumper, debris-drivetrain, sedan; gemeinsame colormap.png | Normale FBX-Importe, lokale URP-Materialien, Skalierung und Platzierung; Reifen rund und glatt dargestellt |
| [Kenney Racing Kit](https://kenney.nl/assets/racing-kit), Archiv 2.0 | Vier FBX: fenceStraight, pitsOffice, pitsGarageClosed, lightPostLarge; net.png | Gemeinsame gedämpfte Materialien, kleine Hintergrundgebäude, Alpha-Cutout-Netz |
| [Poly Haven Concrete Floor](https://polyhaven.com/a/concrete_floor), eye-candy.xyz | Diffuse und OpenGL-Normal als 1K-JPG | Farbstimmung zurückgenommen, Normalstärke 0,13; normales URP/Lit ohne neuen Shader |

Alle drei Grundlagen CC0. Original-Lizenzbelege/Quellennotiz stehen neben den importierten Dateien; Einträge in THIRD_PARTY_LICENSES.md. Drei konkrete Grundlagen, nur benötigte Modelle und Texturen importiert. Keine Käufe, Plugins oder neue Modellpipeline. Paletten/Fässer wurden für diesen kleinen Pass weggelassen, da die ausgewählten Schrottteile den Ort bereits erklären.

## Wiederverwendung und Pflege

`Assets/Game/Environment/JunkyardPreview/JunkyardEnvironment.prefab` enthält die neue Darstellung. Die alten physischen Arenaobjekte bleiben separat in der Szene. Eigene Materialien liegen im zugehörigen Materials-Ordner; importierte Originale unter ThirdParty/Kenney beziehungsweise ThirdParty/PolyHaven.

`JunkyardPreviewBuilder.Build()` / **Wombat Lab/M1 Dress Junkyard Preview** erneuert nur diesen begrenzten Dekorationspass in der gespeicherten HumanoidCombatLab-Szene. Er ist ein Editor-Werkzeug für diesen Zwischenstand, kein Laufzeit-Mapgenerator. Nach bewusstem vollständigem Szenenneuaufbau M1 zuletzt nach B2 → B3a → B3b anwenden. Manuelle spätere Änderungen am Environment vorher bewahren, da dieser Menüpunkt die Dekoration ersetzt.

## Übergabe und nächste Arbeit

Vorher/Nachher aus gleicher Kameraposition unter `Assets/QA/m1-before.png` und `m1-after.png`. Kurzer tatsächlicher Durchlauf: vier Arenaecken mit Grounding, Sprung/Landung, sichtbare KI-Warnung, Gegner-Heavy (100 → 88 HP), Spieler-Heavy mit einem Treffer, Knockdown/GetUp mit Schutz und vollständiger Reset. Neue Environment-Collider: 0. Nachweise in TEST_SLICE_STATUS.md und `tools/m1-map-results.txt`. Keine neue Combat-Vollsuite für reine Umgebungsgestaltung.

Danach B3c: Duell in dieser Umgebung abstimmen und kleinen Windows-/WebGL-Nachweis erstellen. Weitere Bereiche, Encounter-Strecke und Checkpoints bleiben B6; finale Comic-Weltgestaltung bleibt B7. Der Mensch/Bär-Stand und der offene Wombat-Look bleiben erhalten.
