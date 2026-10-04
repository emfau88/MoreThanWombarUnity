# Externe Assets — Herkunft und Lizenznachweise

Stand: 4. Oktober 2026. Dieses Register betrifft zusätzlich importierte Modelle, Rigs, Animationen, Props, Texturen, Audio und VFX. Unity-/Paketabhängigkeiten stehen separat in `UnityProject/Packages/manifest.json` und behalten ihre jeweiligen Bedingungen.

Die vollständigen Downloadarchive und entpackten Recherchekopien unter `art-source/third-party/` bleiben lokal und sind vom Git-Commit ausgenommen. Alle tatsächlich benötigten Unity-Originale, Ableitungen und Lizenzdateien werden mit dem Projekt versioniert; die Archivhashes dokumentieren den lokalen Bezug.

## Tatsächlich verwendete externe Content-Assets

Zwei Quaternius-Standardpakete sind für den separaten technischen B2-Importnachweis übernommen. Bestehende Grundformen, Transformclips, synthetische Trefferklänge und der begonnene Blender-Entwurf wurden lokal erzeugt. Die Bilder der alten Fassung bleiben reine Designreferenzen.

### Universal Base Characters — Standard

- Urheber: Quaternius; [offizielle Downloadquelle](https://quaternius.itch.io/universal-base-characters). Bezug und Lizenzprüfung: 03.10.2026.
- Archiv: `art-source/third-party/Quaternius/BaseCharacters-Standard.zip`; SHA256 `FDBF1804C90DFC1EA03E992BFF7DA2DFD1A79318E13270A660180F9308455F40`.
- Nachweis: enthaltene `License_Standard.txt`, auch unter `UnityProject/Assets/ThirdParty/Quaternius/BaseCharacters/`. CC0 1.0 Universal / Public Domain Dedication. Kommerzielle Verwendung und Weitergabe der Rohdateien erlaubt; keine verpflichtende Attribution. Urheber freiwillig hier genannt.
- Verwendet: `Superhero_Male_FullBody.fbx`, Körper-/Augentextur und Unity-Normaltextur (letztere vorgehalten, derzeit nicht im Material verwendet). Tatsächliches kostenloses Archiv enthält männliches und weibliches Superhero-Grundmodell; die größere Angebotsauswahl nicht als kostenlosen Inhalt behandeln.
- Originale: `UnityProject/Assets/ThirdParty/Quaternius/BaseCharacters/`. Ableitung: `Assets/Game/Characters/ImportProbe/HumanoidProbe.fbx`, eigene URP-Materialien, Animator-Controller und Prefab. Humanoid-Import, modelllokaler Maßstab/Bodenversatz; Rohgeometrie unverändert. Temporärer Mensch, keine Wombat-Gestaltung.

### Universal Animation Library — Standard

- Urheber: Quaternius; [offizielle Downloadquelle](https://quaternius.itch.io/universal-animation-library). Bezug und Lizenzprüfung: 03.10.2026.
- Archiv: `art-source/third-party/Quaternius/AnimationLibrary-Standard.zip`; SHA256 `CC73FC4E495B82958207316596317A3F40B9FA38065BDE1027937452DA537724`.
- Nachweis: enthaltene `License.txt` und `README.txt`, auch unter `UnityProject/Assets/ThirdParty/Quaternius/AnimationLibrary/`. CC0 1.0 Universal / Public Domain Dedication, kommerzielle Nutzung und Rohdateiweitergabe erlaubt; keine verpflichtende Attribution.
- Original: `UAL1_Standard.fbx`, ausdrücklich Variante ohne Root Motion. Unter anderem Idle, Walk, Jog, Sprint, Jump_Start/Loop/Land, Punch_Jab/Cross, Hit_Head/Chest, Roll und Death01. Kein Kick im Standardarchiv; fehlende Aktionen später konkret ergänzen.
- Ableitung: `Assets/Game/Characters/ImportProbe/MovementSource.fbx`; fünf Bewegungskopien und Jab/Cross, in B3a zusätzlich Sprint_Loop, Hit_Chest, Hit_Head und Sword_Attack als Humanoid importiert. Eigene Combat-Kopien unter `Assets/Game/Characters/HumanoidCombat/` mit retimten Kurven, AttackDefinitions und Avatar-Kontaktbahnen. Source_Hook ist eine Cross-Ableitung mit geänderten Arm-/Rumpfmuskelkurven; Source_Overhand nutzt Sword_Attack ohne Waffe als Heavy-Grundlage. Lokal ergänzte Source_Kick/Source_AirKick und Source_AirSmash verwenden eingefrorene Humanoid-Idle-Kurven mit selbst gesetzten Muskelkurven. Robot_Hit/Robot_Stagger sind eigene Ableitungen unserer Grundform-Idle-Clips. Kein zusätzlicher Download in B3a; dieselben CC0-Originale/Lizenzbelege. Root Motion bleibt aus; Motor ist Bewegungsautorität.

## Kandidaten — noch nicht verwendet

| Quelle / Angebot | Urheber | Öffentliche Lizenzinformation | Stand / offene Prüfung |
| --- | --- | --- | --- |
| [Mixamo](https://helpx.adobe.com/creative-cloud/faq/mixamo-faq.html) | Adobe / Urheber des konkret gewählten Inhalts prüfen | Adobe-FAQ erlaubt royalty-free Verwendung in kommerziellen Spielen | Account erforderlich; konkrete Clips und aktuelle Download-/Rohdatei-Bedingungen vor Verwendung dokumentieren; nicht als CC0 behandeln |
| [Kenney-Assets](https://kenney.nl/support) | Kenney | Supportseite nennt CC0 für Asset-Seiten | Noch kein konkreter stilistisch passender Pack ausgewählt |

Poly Pizza / Asset Store / Fab sind Recherchequellen ohne bisher ausgewähltes Angebot. Daraus wird keine Lizenzzusage für einzelne Assets abgeleitet.

## Eintrag je importiertem Pack

Für tatsächliche Importe diese Angaben vollständig führen:

- Pack und verwendete Einzelassets; Version beziehungsweise Archivname.
- Urheber/Publisher und direkte Angebots-/Downloadquelle.
- Datum des Bezugs und der Lizenzprüfung.
- Lizenzname sowie im Projekt gespeicherte Lizenzdatei oder Bedingungen als Nachweis.
- Kommerzielle Verwendung, erforderliche Attribution und Bedingungen für Weitergabe der Rohassets.
- Originalpfad unter `UnityProject/Assets/ThirdParty/<Anbieter>/<Pack>/`.
- Eigene Ableitungen unter `UnityProject/Assets/Game/...` und daran vorgenommene Anpassungen.
- Archivhash bei Bedarf zur eindeutigen Zuordnung.

Originaldateien erhalten; bearbeitete Modelle/Clips und eigene Prefabs außerhalb des Originals speichern. Vor Commit/Push zusätzlich prüfen, ob die gewählte Lizenz die Veröffentlichung der Rohdateien gestattet.
