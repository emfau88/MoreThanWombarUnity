# B2 — Auswahl und technischer Import, Schritte 1/2

Stand: 3. Oktober 2026. Beauftragt sind eine begrenzte Auswahl und der Unity-Import mit der vorhandenen Steuerung. Wombat-Gestaltung und vollständige Combat-/Animationsintegration bleiben offen.

## Drei Kandidaten

| Grundlage | Stil und Anpassung | Rig / Clips / Format | Lizenz / Kosten | Entscheidung |
| --- | --- | --- | --- | --- |
| [Free Cartoon Bear Rigged — QTP Media House](https://www.fab.com/listings/ff3c30bb-abce-4d4c-b65e-1e2e4e8a5390?lang=de) | Cartoon-Tier; näher an einer späteren Wombat-Anpassung als ein Mensch; keine Texturen | Eigenes Rig laut Anbieter, FBX; Humanoid-Kompatibilität nicht nachgewiesen, keine konkrete Clipliste | Kostenlos angeboten; Lizenzfeld im auslesbaren Angebot leer, konkreter Lizenz-/Archivnachweis fehlt | Als Tieroption erhalten, nicht importieren, solange Lizenz und Rig nicht konkret vorliegen |
| [Cute Cartoon Animals — FelzLab](https://felzlab.itch.io/cute-cartoon-animals-pack-8-rigged-3d-character-models-blender-fbx) | Rundes Cartoon-Tierpaket mit Bär; Wombat-Gesicht/Proportionen müssten angepasst werden | Rigged Blend/FBX, Anbieter nennt ca. 11.648 Dreiecke je Figur; keine Animationen; Humanoid nicht zugesichert | 12 USD Mindestpreis; kommerzielle/Rohdatei-Bedingungen vor Kauf klären | Stiloption für spätere Entscheidung; kein Kauf im aktuellen Auftrag |
| [Universal Base Characters — Quaternius](https://quaternius.itch.io/universal-base-characters) | Glatte stilisierte menschliche Grundmodelle; ausdrücklich nur technische Testbasis, kein fertiger Wombat | Humanoid-Rig, FBX/glTF; kostenlose Standardversion; kompatible Universal Animation Library | CC0 auf offizieller Produkt-/Downloadseite; Standard kostenlos, Source kostenpflichtig | Für den unmittelbaren Import-/Avatar-/Motor-Nachweis wählen; endgültiges Tiermodell offen |

Anbieterangaben sind keine bereits bestätigten Unity-Importresultate. Recherche endet mit diesen drei ernsthaften Kandidaten. Keine weiteren Packs allein wegen kostenloser Verfügbarkeit hinzufügen.

## Empfehlung und Umfang

Die Quaternius-Standardbasis liefert den klarsten kostenlosen Humanoid-Nachweis, während die endgültige Tiergestaltung ausdrücklich vertagt ist. Sie ist im separaten `CharacterImportLab` mit derselben PlayerMotor-/LabInput-Steuerung eingebunden. B1-Sparring und ursprüngliches Wombat-Prefab bleiben die vollständige Combat-Vergleichsbasis.

Für den Importnachweis genügen Idle/Walk und passende Jump-/Fall-/Land-Zustände aus einer geklärten Bibliothek. Diese kleine Bewegungseinbindung ist nötig, damit die Figur sich tatsächlich mit dem vorhandenen Motor bewegt; Punch/Kick-Timing und retargetete Combat-Sweeps gehören zum anschließenden B2-Schritt. Keine versteckte Attack-Hitbox am unveränderten alten Rig unter einem neuen sichtbaren Modell.

Größe, Vorwärtsrichtung, Avatar, Bodenhöhe, Hand-/Fußknochen, Material und `applyRootMotion = false` konkret am importierten FBX prüfen. Importierte Originale unter ThirdParty, eigene Import-/Controller-/Prefab-/Materialableitungen unter Game/Characters. Herkunft und konkrete Archivdatei in THIRD_PARTY_LICENSES führen.

## Übergabestand

Schritte 1/2 abgeschlossen. Beide Standardarchive kostenlos bezogen, enthaltene CC0-Belege erhalten und in THIRD_PARTY_LICENSES registriert. Die Standard-Modellversion enthält tatsächlich nur zwei Superhero-Grundmodelle (männlich/weiblich), keine fertige Tierfigur. Die tatsächliche Standard-Clipliste enthält unter anderem Punch_Jab/Cross, Hit und Death, aber keinen Kick.

- Importiertes `Superhero_Male_FullBody`: gültiger Humanoid-Avatar, 65 Skin-Bones, drei Meshes zusammen 14.318 Dreiecke. Eigene Körper-/Augen-/Brauenmaterialien, Modellhöhe 1,85 m; modelllokaler Bodenversatz anhand der retargeteten Idle-Pose korrigiert. Menschliche Form bewusst unverändert.
- Eigenes `Assets/Game/Characters/ImportProbe/HumanoidProbe.prefab` mit vorhandenem PlayerMotor/LabInput, eigener Visual-Wurzel und Animator. Hand-/Fußanker an vier echten Humanoid-Knochen. Keine versteckte alte Attack-Hitbox; Combat-Anbindung folgt separat.
- Fünf echte Humanoid-Clips aus `UAL1_Standard.fbx`: Idle_Loop → Idle, Walk_Loop → Walk, Jump_Start → Jump, Jump_Loop → Fall, Jump_Land → Land. Kopien/Controller in ImportProbe; Jump/Land beschleunigt auf die kurzen bestehenden Motorphasen. Motor bewegt den Charakter, `applyRootMotion = false`.
- Szene `Assets/Game/Scenes/CharacterImportLab.unity`: vorhandene Arena/Kamera, Bewegung, Sprung, Reset; eigenständige gespeicherte Kopie. CombatLab/SparringLab bleiben die Combat-Szenen. Steuerung: WASD/Stick, Space/A, R/Start.
- Gezielte echte Play-Sequenz bestanden: 1,48 m Laufweg, 0,24 m maximale modelllokale Fußbewegung, Blickrichtungsfehler 0,08°, 1,17 m Sprunghöhe, Landung und Reset. Unterster Idle-Vertex ca. 2,4 cm über Boden. Kamerabilder Idle/Walk/Jump unter `Assets/QA/b2-import-*.png` angesehen; Messung in `tools/b2-import-results.json`. Keine breite Testsuite für diesen isolierten Import.
- Ein regulärer FBX-Reimport erhält gültigen Avatar, Controller, Motor-Definition, Visual und vier Kontaktanker. Szene frisch geladen, Play gestoppt, keine fehlgeschlagene Kompilierung und keine ungespeicherten Szenenänderungen.

Der volle B2-Bulk ist noch nicht abgeschlossen. Wombat-Look nicht begonnen. Nächster Entwicklungsschritt: konkrete Humanoid-Combat-Pose/Kontaktbahn und Attack-Zeiten anbinden, zunächst Jab/Cross und eine gezielte Kick-Ergänzung. Die aktuelle Bewegungsbasis ist noch kein abgestimmtes finales Moveset; Schrittgeschwindigkeit und Übergänge werden am Combat-Slice weiter verbessert.
