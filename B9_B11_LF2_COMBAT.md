# B9–B11 — AOE, Fähigkeiten und Gruppenkampf

Stand: 9. Oktober 2026. Lokale Implementierung auf der vorhandenen Unity-Basis. Der öffentliche GitHub-Play-Link enthält weiterhin B8; eine Veröffentlichung ist mit diesem Arbeitsauftrag nicht erfolgt.

## Spielbarer Umfang

- **B9:** Luft-Heavy wird zum Boden-Smash. Er wartet auf die tatsächliche Landung, trifft einmal pro Ziel im Radius von 2,1 m und wird bei Unterbrechung verworfen. Bodenring, Einzelimpulse, Gegnerreaktionen und kurze Mehrfachtrefferanzeige machen die Wirkung sichtbar. Massive Hindernisse blockieren den Schaden.
- **B10:** 100 MP, langsame Regeneration außerhalb von Angriffen und sechs MP für einen erfolgreichen normalen Angriff, unabhängig von der getroffenen Gegnerzahl. Smash kostet 22 MP, Durchbruch 18, Druckwelle 26. Grundkampf und Ausweichen bleiben kostenlos. MP wird beim Checkpoint gespeichert und bei Retry wiederhergestellt; Pause hält die Regeneration an.
- **Durchbruch:** 3,2 m gerichteter Vorstoß, einmaliger Schaden je Gegner. Passiert leichte Gegner, stoppt an schweren Gegnern und Wänden. Körperkollision wird nur für den aktiven Stoß gezielt freigegeben und danach wiederhergestellt.
- **Druckwelle:** Gerichtetes Projektil, bis zu drei Gegner, 8 m Reichweite. Es stoppt an massiven Hindernissen; eigene Teammitglieder erhalten keinen Schaden. Eigene sichtbare Flugbahn statt eines zweiten AOE-Kreises.
- **B11:** Leichte Raufbolde (32 HP), Werfer (44 HP, angekündigter gerader Wurf) und schwere Schläger (85 HP). Eigene Größen, Farben und Accessoires auf vorhandenen Rigs. Werfer ersetzen Agile im Kapitel; das alte Labor bleibt als Vergleich bestehen.
- **Gruppendruck:** Zwei Nahkampfangreifer je Ziel sind möglich; neue Warnungen beginnen zeitversetzt. Fernkampf wird zusätzlich begrenzt. Wartepositionen verteilen sich über die Gruppengröße. Lokaler Gegner-Hitstop hält nicht mehr die komplette Gruppe an. Kurzer Schutz nach Spielertreffern gibt ein Zeitfenster zum Entkommen.
- **Teams:** Kleiner gemeinsamer Ziel-/Schadensadapter um die vorhandenen Empfänger; KI wählt gegnerische lebende Kämpfer statt ausschließlich eines fest verdrahteten Spielerziels. Noch kein KI-Begleiter und kein Multiplayer.

## Ausprobieren

**Dichter Gruppenkampf:** `UnityProject/Assets/Game/Scenes/CrowdCombatLab.unity` öffnen und Play starten. Standardmäßig acht Gegner; `8` wählt acht, `0` zehn, `1–4` kleine Gruppen. Der Touch-Gegnerbutton wechselt ebenfalls durch die Gruppengrößen. `R` setzt den Kampf zurück.

**Lokaler Browserbuild:** `tools/Serve-Duel.ps1 -Build B11 -Port 8766`, dann [Gruppenkampf](http://127.0.0.1:8766/) öffnen. Build liegt in `Builds/B11/WebGL`. Für den Handycheck im gleichen WLAN den Server mit `-Lan` starten und die IPv4-Adresse des PCs statt `127.0.0.1` verwenden.

**Vorhandenes Kapitel:** `UnityProject/Assets/Game/Scenes/JunkyardChapter.unity` enthält ebenfalls die neuen Fähigkeiten, MP und Gegnerrollen. Sein bisheriger Ablauf mit neun Wellen/23 Gegnern bleibt bis B12 erhalten. Dieser Auftrag liefert noch nicht die geplante 30-Gegner-Stage.

| Aktion | Tastatur | Gamepad | Touch |
| --- | --- | --- | --- |
| Boden-Smash | Springen, dann K | A, dann Y | SPRUNG, dann HEAVY |
| Durchbruch | E | RT | STOSS |
| Druckwelle | Q | Steuerkreuz rechts | WELLE |
| Grundkampf | J / K / L | X / Y / RB | COMBO / HEAVY / KICK |
| Ausweichen | SHIFT | B | AUSWEICHEN |

Die MP-Leiste zeigt verbleibende Energie und kurz die Anzahl von Mehrfachtreffern. Bei zu wenig MP erscheint ein Hinweis; normale Angriffe bleiben verfügbar. Die Druckwelle benötigt einen zusätzlichen Touch-Button. Bestehende Unity-OnScreen-Controls speisen dieselben Input-Actions wie das Gamepad.

## Charakter- und Assetentscheidung

Für diese Stage bleibt der menschliche Nahkämpfer die gewählte Basis. Retargetete vorhandene Animationen, Luft-Smash und Schulterstoß werden weiterverwendet; die Druckwelle erhält eine abgeleitete beidarmige Ausstoßpose. Kampfbandagen ergänzen die Figur. Das ist keine fertig gestaltete Wombat-Figur. Die abschließende gemeinsame Art-/Animationspolitur bleibt B13; ein eigenes Wombat-Modell wird daraus nicht stillschweigend abgeleitet.

Vorhandene Quaternius-/Kenney-Grundlagen, B7-Bärenmaterialien, Warn-/Treffertöne, Unity Input System, Animator, Physik und bestehender CLI-/Buildworkflow bleiben im Einsatz. Kein neues Paket, keine neue Rig-/KI-Architektur und keine zusätzliche Asset-Lizenz. Kleine Effekte entstehen mit gemeinsamem Material und begrenzter Lebensdauer aus Unity-LineRenderern.

## Gezielte Nachweise

Fünf neue PlayMode-Fälle bestanden, **5/5 in 19,49 s**. Rohbericht: [tools/b9-b11-combat-results.json](tools/b9-b11-combat-results.json). Die anschließende Browserprüfung fand eine Landung auf Gegneroberseiten. Nach der Korrektur bestehen die beiden betroffenen Smash-Fälle **2/2 in 5,95 s**, einschließlich eines neuen Falls direkt über besetztem Boden: [tools/b9-landing-results.json](tools/b9-landing-results.json). Insgesamt sechs unterschiedliche gezielte Fälle, keine neue Vollsuite.

1. Boden-Smash verursacht vor der Landung keinen AOE-Schaden, trifft drei nahe Gegner einmal, lässt ein äußeres Ziel ungetroffen und bleibt nach Unterbrechung aus.
2. Ohne MP kein Spezialangriff, normale Angriffe weiter möglich; Durchbruch trifft zwei leichte Gegner einmal, passiert sie und stoppt am schweren Gegner. Kollisionsfreigabe wird zurückgesetzt.
3. Druckwelle respektiert Wand und Team; ohne Wand werden genau die ersten drei Ziele getroffen.
4. Zwei Nahkämpfer erhalten gleichzeitig Angriffsfreigaben; ein angekündigter Werferangriff kann durch seitliches Versetzen vermieden werden.
5. Neue Gamepad-Action und Touch-Binding sind vorhanden; Checkpoint stellt MP wieder her, Pause stoppt MP-Zeit.

6. Beim Sprung bleiben Gegner-Hurtboxes aktiv, ihre festen Körper werden vorübergehend passiert. Der Smash landet dadurch auf dem Boden statt auf Köpfen. Körperkollision kehrt nach Landung/Trennung zurück; Reset räumt die Freigaben auf.

Zusätzliche kurze Laufzeitprobe für die Teamgrundlage: Ein Raufbold besiegte einen zweiten Kämpfer des Spieler-Teams mit fünf Treffern und wählte anschließend wieder ein lebendes Ziel. Lokaler Hitstop war am direkt getroffenen Gegner aktiv und an einem unbeteiligten Gegner inaktiv. Dies ist ein technischer Zielwahl-Nachweis, noch kein gelieferter KI-Begleiter.

Tatsächliche Unity-Spielbilder wurden für Touch-Anordnung, AOE-Aufprall, Druckwelle und acht Gegner aufgenommen. Erste Sichtprüfung führte zu weniger dauerhaftem Gegnertext und zur Übernahme der vorhandenen warmen Bärenmaterialien. Lokale Bilder liegen unter `UnityProject/Assets/QA/` und gehören nicht zu den versionierten Assets.

**Lokaler WebGL-Build mit Landekorrektur erfolgreich:** 188,74 s, 21.267.258 Bytes, null Fehler. Ein Hinweis betrifft die bewusst nicht konfigurierte Pipeline-Steuerung im ausgelieferten Player; die CLI bleibt ein Editor-Werkzeug. Buildbericht: [tools/b11-webgl-build.json](tools/b11-webgl-build.json).

Der optionale Messmodus `?crowdprofile=1` hält acht Gegner mit vorübergehend erhöhten HP am Leben und spielt Bewegung/Spezialaktionen automatisch ab. Nach drei Sekunden Anlauf sammelt er 15 Sekunden Frameintervalle; er misst eine reproduzierbare technische Belastung, keine menschliche Schwierigkeit. `?crowdprofile=10` ist die separate Zehn-Gegner-Probe. Angegebener Speicher ist die von Unity erfasste Allokation, nicht der gesamte Browser-/GPU-Speicher. Der Messmodus ist nur über diesen ausdrücklichen URL-Parameter aktiv; die normale Spieladresse verwendet normale HP/MP und Spielereingaben.

**PC-Browserbefund des korrigierten Builds:** Ryzen 7 6800H; der Browser meldet die integrierte AMD Radeon über ANGLE/D3D11, nicht die zusätzlich vorhandene RTX 3060. Tatsächliche Canvasauflösung 1281×721 bei angefordertem 1280×720-Viewport. Acht Gegner: 3.473 Samples, Median 4 ms, 95. Perzentil 5 ms, 60,62 MB erfasste Unity-Allokation. Zehn Gegner: 3.524 Samples, ebenfalls 4/5 ms, 60,79 MB. Alle vorgesehenen Gegner blieben für die Messung lebend; keine Browser-Konsolenfehler beobachtet. [Rohwerte](tools/b11-browser-profile.json).

Damit zeigt diese kurze PC-Probe keinen Anlass, acht Gegner aus Leistungsgründen zu reduzieren. Die Frameintervalle sind keine separat gemessenen CPU-/GPU-Zeiten und keine Garantie für lange Spielsitzungen, andere Rechner oder Android. Zehn bleiben ein optionaler Versuch; der normale Einstieg nutzt acht.

Im normalen Browserplayer wurden zusätzlich NEUSTART und der neue WELLE-Touchbutton betätigt: frischer Kampf mit acht Gegnern und 100 HP, danach 74 MP entsprechend den 26 MP Aktionskosten. Dies bestätigt den UI-Eingabepfad auf dem PC, nicht die Bedienbarkeit auf einem physischen Handy.

## Offene Abnahmen und nächster Bulk

Ein physischer Android-Test ist weiterhin offen. Weder ein virtueller Touch-Button noch eine PC-Browsermessung bestätigt die Leistung oder Finger-Erreichbarkeit auf dem Handy. Acht Gegner bleiben deshalb der Standard; zehn sind eine ausdrücklich wählbare Probe. Hörbare Gesamtbalance und menschliches Spielgefühl brauchen eine kurze Spielrunde, keine weitere automatische Vollsuite.

**Als Nächstes B12:** Die vorhandene Strecke zu fünf abwechslungsreichen Begegnungen mit insgesamt 30 Gegnern umbauen, Verstärkungen beim Vorankommen zulassen und Checkpoints/Finale auf diesen Ablauf abstimmen. B13 folgt mit der abschließenden Figuren-/Map-/Effektpräsentation.
