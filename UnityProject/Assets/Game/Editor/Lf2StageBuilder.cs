using System;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Object = UnityEngine.Object;

namespace WombatLab.Editor
{
    // Modify the existing B7/B8 scene in place; do not regenerate its authored presentation.
    public static class Lf2StageBuilder
    {
        const string Combat = "Assets/Game/Characters/HumanoidCombat/";
        [MenuItem("Wombat Lab/B12 Apply 30 Enemy Stage")]
        public static string Apply()
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling || EditorUtility.scriptCompilationFailed
                || UnityEngine.SceneManagement.SceneManager.GetActiveScene().isDirty)
                throw new InvalidOperationException("Use idle compiled Editor with saved scene.");
            var scene = EditorSceneManager.OpenScene(JunkyardChapterBuilder.ScenePath);
            var chapter = Object.FindAnyObjectByType<JunkyardChapter>();
            var light = AssetDatabase.LoadAssetAtPath<EnemyRoleDefinition>(Combat + "Role_RAUFBOLD.asset");
            var thrower = AssetDatabase.LoadAssetAtPath<EnemyRoleDefinition>(Combat + "Role_WERFER.asset");
            var heavy = AssetDatabase.LoadAssetAtPath<EnemyRoleDefinition>(Combat + "Role_SCHLAEGER.asset");
            var foreman = AssetDatabase.LoadAssetAtPath<EnemyRoleDefinition>("Assets/Game/Chapters/Junkyard/Foreman.asset");
            var data = chapter.definition;
            data.areas = new[] {
                Area("ANLIEFERUNG", 0,
                    Encounter("Einstieg", -4.8f, light, light, light, light, light),
                    Encounter("Vorstoß zum Tor", 1.5f, light, light, light)),
                Area("SORTIERHOF", 24,
                    Encounter("Gemischter Hauptkampf", -4.8f, light, thrower, light, heavy, light, thrower, light, light),
                    Encounter("Durchgang", 1.5f, light, light, thrower, heavy, light, light)),
                Area("PRESSWERK", 48,
                    Encounter("Vorarbeiter und Gefolge", -4.8f, light, thrower, light, heavy, light, thrower, light, foreman))
            };
            data.activeLimit = 8; data.throwerLimit = 2; data.spawnWarningSeconds = 1.1f; data.spawnInterval = .25f;
            data.areaHeal = 30; data.areaEnergy = 30;
            chapter.encounter.maxOpponents = 8; chapter.encounter.maxMeleeAttackers = 2;
            chapter.cameraRig.offset = new Vector3(0, 8.5f, -13);
            var hud = Object.FindAnyObjectByType<LabHud>();
            hud.stateText.fontSize = 16;
            hud.stateText.rectTransform.anchorMin = Vector2.zero;
            hud.stateText.rectTransform.anchorMax = new Vector2(1,.58f);
            hud.stateText.rectTransform.offsetMin = new Vector2(16,0);
            hud.stateText.rectTransform.offsetMax = new Vector2(-16,0);
            hud.stateText.alignment = TextAnchor.MiddleLeft;
            EditorUtility.SetDirty(hud.stateText);

            var previous = GameObject.Find("B12 Stage access cues"); if (previous != null) Object.DestroyImmediate(previous);
            var cues = new GameObject("B12 Stage access cues").transform;
            var paint = AssetDatabase.LoadAssetAtPath<Material>("Assets/Game/Environment/JunkyardChapter/Materials/Worn yellow paint.mat");
            foreach (var area in data.areas)
            {
                foreach (int side in new[] { -1, 1 })
                {
                    float x = area.center + side * 5.8f;
                    // Low, non-colliding threshold stripes identify the four safe entry lanes.
                    foreach (float z in new[] { -1.55f, 1.55f })
                        Bar(cues, "Zugang — Bodenmarkierung", new Vector3(x, .022f, z), new Vector3(.12f,.014f,.8f), paint);
                    var sign = new GameObject("ZUGANG").AddComponent<TextMesh>(); sign.transform.SetParent(cues, false);
                    sign.transform.position = new Vector3(x, .65f, 3.1f); sign.text = "ZUGANG";
                    sign.characterSize = .055f; sign.fontSize = 64; sign.anchor = TextAnchor.MiddleCenter;
                    sign.color = new Color(.86f,.70f,.32f);
                }
                if (area.waves.Length > 1)
                {
                    var sign = new GameObject("VORSTOSS").AddComponent<TextMesh>(); sign.transform.SetParent(cues,false);
                    sign.transform.position = new Vector3(area.center + 1.5f,.12f,2.7f);
                    sign.text = "WEITER  >"; sign.characterSize = .06f; sign.fontSize = 64; sign.anchor = TextAnchor.MiddleCenter;
                    sign.color = new Color(.5f,.9f,.75f);
                }
            }
            var warning = new GameObject("Announced reinforcement").transform; warning.SetParent(cues,false);
            const string warningPath = "Assets/Game/Chapters/Junkyard/SpawnWarning.mat";
            var material = AssetDatabase.LoadAssetAtPath<Material>(warningPath);
            if (material == null)
            {
                material = new Material(AssetDatabase.LoadAssetAtPath<Material>("Assets/Game/Resources/CombatSignals.mat"));
                AssetDatabase.CreateAsset(material, warningPath);
            }
            material.SetColor("_BaseColor", new Color(1,.65f,.16f)); EditorUtility.SetDirty(material);
            var line = warning.gameObject.AddComponent<LineRenderer>(); line.sharedMaterial = material;
            line.useWorldSpace = false; line.widthMultiplier = .09f; line.positionCount = 3;
            line.SetPositions(new[] { new Vector3(-.45f,0,-.35f), new Vector3(0,0,.35f), new Vector3(.45f,0,-.35f) });
            line.startColor = line.endColor = new Color(1,.65f,.16f);
            line.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            chapter.spawnWarning = warning; warning.gameObject.SetActive(false);
            EditorUtility.SetDirty(data); EditorUtility.SetDirty(chapter); EditorUtility.SetDirty(chapter.encounter);
            EditorSceneManager.MarkSceneDirty(scene); EditorSceneManager.SaveScene(scene); AssetDatabase.SaveAssets();
            var roles = data.areas.SelectMany(a => a.waves).SelectMany(w => w.enemies).ToArray();
            return "Saved B12: " + roles.Length + " enemies, 5 encounters, 3 areas; 21 light / 5 thrower / 4 heavy, including foreman.";
        }
        static ChapterArea Area(string title, float center, params ChapterWave[] encounters)
            => new ChapterArea { title = title, center = center, waves = encounters };
        static ChapterWave Encounter(string title, float trigger, params EnemyRoleDefinition[] roles)
            => new ChapterWave { title = title, triggerOffset = trigger, enemies = roles.Select(r => new ChapterEnemySpawn { role = r }).ToArray() };
        static void Bar(Transform parent, string name, Vector3 position, Vector3 scale, Material material)
        {
            var go = GameObject.CreatePrimitive(PrimitiveType.Cube); go.name = name; go.transform.SetParent(parent,false);
            go.transform.position = position; go.transform.localScale = scale; go.GetComponent<Renderer>().sharedMaterial = material;
            Object.DestroyImmediate(go.GetComponent<Collider>());
        }
    }
}
