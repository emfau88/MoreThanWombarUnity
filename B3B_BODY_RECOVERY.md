# B3b — Niederwerfen, Aufstehen und Tod

Stand: 4. Oktober 2026. Im `HumanoidCombatLab` integriert und gezielt geprüft. B3a ist als `405bfb5` auf origin/main veröffentlicht; dieser Bulk baut darauf auf und liegt lokal zur spielbaren Übergabe vor.

## Spielablauf

- Spieler-Heavy K und Luft-Smash werfen einen lebenden Gegner nieder. Jab/Cross/Haken und Kicks behalten ihre Hit-/Stagger-Reaktion.
- Der angekündigte Heavy des Bären-Dummys kann den Spieler niederwerfen. Schaden bleibt zunächst 12 HP; Angriff bleibt ausweichbar.
- Fall 0,42 s → Bodenphase 0,40 s → Aufstehen 0,80 s. Währenddessen keine eigenen Aktionen oder zusätzlichen Treffer. Danach 0,45 s Schutz bei bereits zurückgegebener Steuerung.
- Bei 0 HP folgt Tod statt GetUp. Die ganze Figur bleibt liegen. R stellt Spieler und Gegner einschließlich Collider, Pose, HP, Puffer, KI und Schutz wieder her.

## Zuständigkeit und Körperkollision

`BodyRecovery` verwaltet Standing/Falling/Down/GettingUp/Dead, sichtbare Clips, Aufstehschutz und Collider-Wiederherstellung. `PlayerDefense`/`TrainingDummy` entscheiden weiterhin Schaden, Annahme und Unterbrechung. `AttackDefinition.knocksDown` wählt kräftige Angriffe explizit aus; Schaden allein löst keinen Knockdown aus. Komponenten und neue Angriffsflags sind nur in der aktuellen Humanoid-Szene aktiv; ältere B1-Szenen behalten ihren Ablauf.

Beim Gegner sind Körpercollider und Trigger-Hurtbox während Fall/Boden/GetUp/Tod aus; sie kehren nach GetUp zurück. Der Spieler behält einen niedrigen CharacterController, damit Boden/Schwerkraft/Arenagrenzen weiter funktionieren. Die normale Kapselhöhe kehrt beim Aufstehen zurück. Zusätzliche Treffer sind sowohl über Collider als auch den Schadensempfänger gesperrt; kein Dauerniederwerfen während Bodenphase oder Aufstehschutz. GetUp-Schutz sperrt die zurückgegebene Steuerung nicht.

Hitstop hält lebende Fall-/Boden-/GetUp-Phasen an. Tod bleibt sichtbar und läuft weiter. PlayerMotor und EnemyBrain überschreiben diese Posen nicht. Die bestehende freie Flugbahn bleibt unter dem Motor; Knockdown setzt keinen künstlichen Sprung-/Fallimpuls.

## Animation und Quellen

Vorhandenes Quaternius Death01 liefert den Humanoid-Fall. Der Clip wird bis zur liegenden Pose gekürzt und auf 0,42 s abgestimmt. Dieselbe Fallpose dient bei tödlichen Treffern als Death, dann bleibt das letzte Bild stehen. Im Standardarchiv fehlt GetUp. Die lokale Ergänzung verbindet Liegen/Seitstütz mit vorhandenen Fixing_Kneeling-/Crouch_Idle-Posen und Idle. Es ist keine rückwärts abgespielte Todesanimation. Robot-Fall/GetUp verwenden eigene Rig-/Rumpf-/Kopfkurven.

Keine neuen Downloads oder Käufe. Lizenzbelege der Quaternius-Originale bleiben CC0; Einzelquellen in THIRD_PARTY_LICENSES.md. Eigene Ableitungen und Controller liegen unter `Assets/Game/Characters/HumanoidCombat/`. Der Gegner erhält dort eine eigene Enemy_Heavy-Definition mit Knockdown, damit B1 unverändert bleibt.

`BodyRecoveryBuilder.Build()` / **Wombat Lab/B3b Upgrade Knockdown and GetUp** integriert diesen Bulk gezielt. Bei bewusstem vollständigen Neuaufbau Reihenfolge **B2 → B3a → B3b** einhalten; frühere Builder können neuere Definitionen/Komponenten überschreiben.

## Gezielt prüfen

Vier BodyRecovery-PlayMode-Fälle bestanden: Heavy während gegnerischer Warnung, lebender Fall/GetUp, Angriff-/Jump-/Evade-Sperre, Schutzende, beide Tode und Reset während jeder Phase einschließlich Gegnerwechsel. Vier vorhandene CharacterAction- und fünf HumanoidCombat-Fälle bestanden als relevante Regression. Keine neue Vollsuite.

Die tatsächliche Kamerasequenz zeigt Fall, Boden, Seitstütz/Knien, Stehen mit Schutz und beide Tode/Reset. Ein echter KI-Heavy verursacht 12 HP und startet Spieler-Falling. GetUp-Zwischenpose nach Bodenkontakt angepasst; die Beine werden erst beim Wechsel zum Knien angezogen. Der inaktive zweite Gegner erhält beim Reset keinen wirkungslosen Animator-Play-Aufruf. Nachweise in [TEST_SLICE_STATUS.md](TEST_SLICE_STATUS.md).

## Danach

B3c: Duell-Abstände, Combo/Luftkontakte/Recovery und Feedback zusammen abstimmen, kleinen Windows-/WebGL-Nachweis erstellen. Mensch gegen Bären-Dummy bleibt vorerst die spielbare Basis; endgültiger Wombat-/Cartoon-Look ist separat offen.
