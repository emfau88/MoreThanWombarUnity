using System;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace WombatLab.Editor
{
    public static class Lf2CombatBuilder
    {
        const string Root = HumanoidCombatBuilder.Root;
        public const string CrowdScene = "Assets/Game/Scenes/CrowdCombatLab.unity";
        static EnemyRoleDefinition light, thrower, heavy;
        [MenuItem("Wombat Lab/B9-B11 Apply LF2 Combat")]
        public static string Apply()
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling || EditorUtility.scriptCompilationFailed
                || UnityEngine.SceneManagement.SceneManager.GetActiveScene().isDirty)
                throw new InvalidOperationException("Use idle compiled Editor with saved scene.");
            ConfigurePlayer(); ConfigureRoles();
            EnsureEffectMaterial();
            var scene = EditorSceneManager.OpenScene(HumanoidCombatBuilder.ScenePath);
            // Keep the old lab as a comparison; the dedicated crowd scene is the B11 playground.
            EditorSceneManager.SaveScene(scene, CrowdScene);
            var encounter = Object.FindAnyObjectByType<EngagementCoordinator>();
            var enemies = encounter.enemies.ToList();
            while (enemies.Count < 10) enemies.Add(Object.Instantiate(enemies[1].gameObject, enemies[0].transform.parent).GetComponent<EnemyBrain>());
            encounter.enemies = enemies.ToArray(); encounter.maxMeleeAttackers = 2; encounter.maxOpponents = 10; encounter.initialOpponents = 8;
            for (int i = 0; i < enemies.Count; i++)
            {
                var role = i == 5 || i == 7 ? thrower : i == 6 || i == 9 ? heavy : light;
                ConfigureEnemy(enemies[i], role, encounter);
                enemies[i].slot = i; enemies[i].transform.position = new Vector3(-4.5f + i % 5 * 2.1f, 0, i < 5 ? 1.3f : -1.3f);
                enemies[i].gameObject.SetActive(i < 8);
            }
            ConfigureScene(encounter, true);
            encounter.player.transform.position = new Vector3(-5.8f, .05f, 0);
            EditorSceneManager.MarkSceneDirty(scene); EditorSceneManager.SaveScene(scene);
            var buildScenes = EditorBuildSettings.scenes.ToList();
            if (!buildScenes.Any(s => s.path == CrowdScene)) buildScenes.Add(new EditorBuildSettingsScene(CrowdScene, true));
            EditorBuildSettings.scenes = buildScenes.ToArray();

            scene = EditorSceneManager.OpenScene(JunkyardChapterBuilder.ScenePath);
            encounter = Object.FindAnyObjectByType<EngagementCoordinator>(); encounter.maxMeleeAttackers = 2;
            foreach (var enemy in encounter.chapterTemplates)
                ConfigureEnemy(enemy, enemy.role.role == EnemyRole.Heavy ? heavy : enemy.role.role == EnemyRole.Agile || enemy.role.role == EnemyRole.Thrower ? thrower : light, encounter);
            var chapter = Object.FindAnyObjectByType<JunkyardChapter>();
            var foreman = AssetDatabase.LoadAssetAtPath<EnemyRoleDefinition>("Assets/Game/Chapters/Junkyard/Foreman.asset");
            foreach (var area in chapter.definition.areas)
                foreach (var wave in area.waves)
                    foreach (var spawn in wave.enemies)
                        spawn.role = spawn.role == foreman ? foreman : spawn.role.role == EnemyRole.Heavy ? heavy
                            : spawn.role.role == EnemyRole.Agile || spawn.role.role == EnemyRole.Thrower ? thrower : light;
            foreman.name = "Foreman"; foreman.attack = heavy.attack; foreman.health = 105; foreman.displayName = "VORARBEITER";
            EditorUtility.SetDirty(foreman); EditorUtility.SetDirty(chapter.definition);
            ConfigureScene(encounter, false);
            EditorSceneManager.MarkSceneDirty(scene); EditorSceneManager.SaveScene(scene); AssetDatabase.SaveAssets();
            return "B9-B11 applied: landing AOE, MP / breakthrough / pressure wave, 3 roles, 8/10 crowd lab; chapter retains its B8 route until B12.";
        }
        public static void EnsureEffectMaterial()
        {
            if (!AssetDatabase.IsValidFolder("Assets/Game/Resources")) AssetDatabase.CreateFolder("Assets/Game", "Resources");
            const string path = "Assets/Game/Resources/CombatSignals.mat";
            if (AssetDatabase.LoadAssetAtPath<Material>(path) == null)
                AssetDatabase.CreateAsset(new Material(Shader.Find("Universal Render Pipeline/Unlit")), path);
            AssetDatabase.SaveAssets();
        }
        static void ConfigurePlayer()
        {
            var prefab = PrefabUtility.LoadPrefabContents(Root + "HumanoidCombat.prefab");
            try
            {
                var combat = prefab.GetComponent<CombatController>();
                if (prefab.GetComponent<FighterTarget>() == null) prefab.AddComponent<FighterTarget>();
                var smash = combat.airHeavy; smash.groundImpact = true; smash.impactRadius = 2.1f; smash.damage = 34;
                smash.knockback = .7f; smash.energyCost = 22; smash.hitstop = .065f; smash.recoverySeconds = .32f;
                var charge = combat.shoulderCharge; charge.breakthrough = true; charge.chargeDistance = 3.2f;
                charge.energyCost = 18; charge.damage = 22; charge.hitstop = .025f; charge.activeSeconds = .27f;
                var wave = Copy<AttackDefinition>("Player_PressureWave", combat.lights[1]);
                wave.projectile = true; wave.energyCost = 26; wave.damage = 20; wave.radius = .32f;
                wave.projectileSpeed = 9; wave.projectileRange = 8; wave.projectileTargets = 3;
                wave.forwardStep = 0; wave.knockback = .32f; wave.hitstun = .22f;
                wave.startupSeconds = .24f; wave.activeSeconds = .08f; wave.recoverySeconds = .38f;
                wave.stateName = "Pressure_Wave";
                var clip = Object.Instantiate(combat.lights[1].clip);
                float[] times = { 0, clip.length * .3f, clip.length * .45f, clip.length * .7f, clip.length };
                foreach (string side in new[] { "Left", "Right" })
                {
                    HumanoidCombatBuilder.Muscle(clip, side + " Arm Front-Back", times, new[] { -.2f, -.4f, -.8f, -.6f, -.2f });
                    HumanoidCombatBuilder.Muscle(clip, side + " Forearm Stretch", times, new[] { -.7f, -.8f, .65f, .3f, -.7f });
                }
                wave.clip = HumanoidCombatBuilder.SaveClip(clip, "Source_PressureWave");
                var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(Root + "HumanoidCombat.controller");
                HumanoidCombatBuilder.State(controller, wave.stateName, wave.clip);
                combat.pressureWave = wave;
                var motor = prefab.GetComponent<PlayerMotor>();
                foreach (var side in new[] { HumanBodyBones.LeftHand, HumanBodyBones.RightHand })
                {
                    var bone = motor.animator.GetBoneTransform(side);
                    var existing = bone.Find("Fighter hand wrap"); if (existing != null) Object.DestroyImmediate(existing.gameObject);
                    var wrap = Part(motor.visual, "Fighter hand wrap", PrimitiveType.Sphere, Vector3.zero, new Vector3(.15f,.13f,.17f), new Color(.14f,.48f,.43f));
                    wrap.position = bone.position; wrap.rotation = bone.rotation; wrap.SetParent(bone, true);
                }
                foreach (var asset in new Object[] { smash, charge, wave, controller }) EditorUtility.SetDirty(asset);
                PrefabUtility.SaveAsPrefabAsset(prefab, Root + "HumanoidCombat.prefab");
            }
            finally { PrefabUtility.UnloadPrefabContents(prefab); }
        }
        static void ConfigureRoles()
        {
            light = Copy<EnemyRoleDefinition>("Role_RAUFBOLD", AssetDatabase.LoadAssetAtPath<EnemyRoleDefinition>(Root + "Role_STANDARD.asset"));
            light.displayName = "RAUFBOLD"; light.health = 32; light.damage = 7; light.moveSpeed = 2.5f;
            light.telegraphSeconds = .52f; light.cooldownSeconds = .95f;
            heavy = Copy<EnemyRoleDefinition>("Role_SCHLAEGER", AssetDatabase.LoadAssetAtPath<EnemyRoleDefinition>(Root + "Role_HEAVY.asset"));
            heavy.displayName = "SCHLÄGER"; heavy.health = 85; heavy.telegraphSeconds = .85f; heavy.cooldownSeconds = 1.25f;
            thrower = Copy<EnemyRoleDefinition>("Role_WERFER", light); thrower.role = EnemyRole.Thrower;
            thrower.displayName = "WERFER"; thrower.health = 44; thrower.damage = 9; thrower.moveSpeed = 1.9f;
            thrower.telegraphSeconds = .85f; thrower.cooldownSeconds = 1.7f; thrower.recoverySeconds = .55f;
            thrower.engagementDistance = 6.5f; thrower.preferredDistance = 4; thrower.minimumAttackDistance = 2;
            thrower.color = new Color(.73f, .46f, .85f);
            var attack = Copy<AttackDefinition>("Enemy_Throw", light.attack);
            attack.stateName = "Enemy_Throw"; attack.projectile = true; attack.projectileTargets = 1;
            attack.damage = 9; attack.projectileSpeed = 5.5f; attack.projectileRange = 7; attack.radius = .18f;
            attack.startupSeconds = .22f; attack.activeSeconds = .1f; attack.recoverySeconds = .4f;
            thrower.attack = attack;
            var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(Root + "RobotReactions.controller");
            HumanoidCombatBuilder.State(controller, attack.stateName, attack.clip).writeDefaultValues = true;
            foreach (var asset in new Object[] { light, heavy, thrower, attack, controller }) EditorUtility.SetDirty(asset);
        }
        static T Copy<T>(string name, T original) where T : Object
        {
            var asset = AssetDatabase.LoadAssetAtPath<T>(Root + name + ".asset");
            if (asset == null) { asset = Object.Instantiate(original); AssetDatabase.CreateAsset(asset, Root + name + ".asset"); }
            else EditorUtility.CopySerialized(original, asset);
            asset.name = name; return asset;
        }
        static void ConfigureEnemy(EnemyBrain enemy, EnemyRoleDefinition role, EngagementCoordinator encounter)
        {
            enemy.role = role; enemy.attack = role.attack; enemy.player = encounter.player; enemy.coordinator = encounter;
            enemy.target.maxHealth = role.health; enemy.target.clock = encounter.player.GetComponent<CombatController>();
            if (enemy.GetComponent<FighterTarget>() == null) enemy.gameObject.AddComponent<FighterTarget>();
            float width = role == heavy ? 1.05f : role == thrower ? .73f : .82f;
            float height = role == heavy ? 1.04f : role == thrower ? .95f : .87f;
            enemy.visual.localScale = new Vector3(width, height, width);
            var capsule = enemy.GetComponent<CapsuleCollider>(); capsule.radius = .30f * width; capsule.height = 1.7f * height; capsule.center = Vector3.up * .85f * height;
            var hurt = enemy.GetComponentsInChildren<CapsuleCollider>(true).First(c => c.isTrigger);
            hurt.radius = .37f * width; hurt.height = 1.22f * height; hurt.center = Vector3.up * 1.14f * height;
            enemy.name = "LF2 " + role.displayName;
            if (enemy.roleLabel != null) { var label = enemy.roleLabel.GetComponent<TextMesh>(); label.text = role.displayName; label.color = role.color; label.characterSize = .038f; label.fontSize = 64; enemy.roleLabel.localPosition = Vector3.up * (2.55f * height); }
            var old = enemy.visual.GetComponentsInChildren<Transform>(true).FirstOrDefault(t => t.name == "Role accessories"); if (old != null) Object.DestroyImmediate(old.gameObject);
            const string presentation = "Assets/Game/Environment/JunkyardChapter/Materials/";
            string glovePath = Root + "LF2_Gloves_" + role.role + ".mat";
            var glove = AssetDatabase.LoadAssetAtPath<Material>(glovePath);
            if (glove == null) { glove = new Material(Shader.Find("Universal Render Pipeline/Lit")); AssetDatabase.CreateAsset(glove, glovePath); }
            glove.SetColor("_BaseColor", role.color); EditorUtility.SetDirty(glove);
            foreach (var renderer in enemy.visual.GetComponentsInChildren<Renderer>(true))
            {
                string name = renderer.name == "Belly" ? "Bear belly" : renderer.name == "Muzzle" ? "Bear snout" : renderer.name == "Eye" ? "Bear eyes"
                    : renderer.name == "Pupil" || renderer.name == "Nose" || renderer.name == "Brow" ? "Bear nose brows pupils" : renderer.name == "Inner ear" ? "Bear inner ears" : "Bear warm fur";
                renderer.sharedMaterial = renderer.name == "Fist" ? glove : AssetDatabase.LoadAssetAtPath<Material>(presentation + name + ".mat");
            }
            var cues = enemy.GetComponent<EnemyPresentation>() ?? enemy.gameObject.AddComponent<EnemyPresentation>();
            cues.enemy = enemy; cues.barMaterial = AssetDatabase.LoadAssetAtPath<Material>(presentation + "Markers.mat");
            cues.standardWarning = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/ThirdParty/Kenney/InterfaceSounds/question_001.ogg");
            cues.agileWarning = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/ThirdParty/Kenney/InterfaceSounds/select_001.ogg");
            cues.heavyWarning = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/ThirdParty/Kenney/ImpactSounds/impactMetal_medium_000.ogg");
            cues.knockdown = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/ThirdParty/Kenney/ImpactSounds/impactPunch_heavy_000.ogg");
            var accessories = new GameObject("Role accessories").transform; accessories.SetParent(enemy.visual, false);
            // Rounded, restrained additions distinguish silhouettes without a new rig/model pipeline.
            if (role == heavy)
                foreach (int side in new[] { -1, 1 }) Part(accessories, "Shoulder guard", PrimitiveType.Sphere, new Vector3(side * .56f, 1.60f, 0), new Vector3(.46f,.33f,.5f), new Color(.32f,.24f,.14f));
            else if (role == thrower)
            {
                Part(accessories, "Scrap satchel", PrimitiveType.Capsule, new Vector3(.5f, .95f, -.12f), new Vector3(.38f,.3f,.3f), new Color(.38f,.23f,.14f));
                Part(accessories, "Throwing scrap", PrimitiveType.Sphere, new Vector3(.55f, 1.45f, .28f), Vector3.one * .23f, new Color(.76f,.57f,.32f));
            }
            else Part(accessories, "Light fighter belt", PrimitiveType.Capsule, new Vector3(0,.82f,0), new Vector3(.56f,.075f,.35f), new Color(.2f,.45f,.39f));
            var rig = enemy.visual.Find("Rig"); if (rig != null) accessories.SetParent(rig, true);
            if (enemy.chargeWarning != null) Object.DestroyImmediate(enemy.chargeWarning.gameObject);
            enemy.chargeWarning = null;
            if (role == thrower)
            {
                var lane = Part(enemy.transform, "Throw warning lane", PrimitiveType.Cube, Vector3.zero, new Vector3(.12f,.012f,role.attack.projectileRange), new Color(1,.53f,.16f));
                enemy.chargeWarning = lane; lane.gameObject.SetActive(false);
            }
            EditorUtility.SetDirty(enemy); EditorUtility.SetDirty(enemy.target);
        }
        static Transform Part(Transform parent, string name, PrimitiveType primitive, Vector3 position, Vector3 scale, Color color)
        {
            var go = GameObject.CreatePrimitive(primitive); go.name = name; go.transform.SetParent(parent, false);
            go.transform.localPosition = position; go.transform.localScale = scale; Object.DestroyImmediate(go.GetComponent<Collider>());
            string path = Root + "LF2_" + name.Replace(" ", "_") + ".mat";
            var material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material == null) { material = new Material(Shader.Find("Universal Render Pipeline/Lit")); AssetDatabase.CreateAsset(material, path); }
            material.SetColor("_BaseColor", color); material.SetFloat("_Smoothness", .18f); EditorUtility.SetDirty(material);
            go.GetComponent<Renderer>().sharedMaterial = material; return go.transform;
        }
        static void ConfigureScene(EngagementCoordinator encounter, bool crowd)
        {
            var hud = Object.FindAnyObjectByType<LabHud>();
            if (encounter.player.GetComponent<FighterTarget>() == null) encounter.player.gameObject.AddComponent<FighterTarget>();
            encounter.player.GetComponent<CombatController>().pressureWave = AssetDatabase.LoadAssetAtPath<AttackDefinition>(Root + "Player_PressureWave.asset");
            var old = hud.transform.Find("Energy"); if (old != null) Object.DestroyImmediate(old.gameObject);
            var panel = new GameObject("Energy", typeof(RectTransform), typeof(Image)); panel.transform.SetParent(hud.transform, false);
            var rect = panel.GetComponent<RectTransform>(); rect.anchorMin = rect.anchorMax = new Vector2(.5f,1); rect.pivot = new Vector2(.5f,1);
            rect.anchoredPosition = new Vector2(0,-115); rect.sizeDelta = new Vector2(300,38);
            panel.GetComponent<Image>().color = new Color(.035f,.10f,.12f,.9f);
            var fill = new GameObject("MP fill", typeof(RectTransform), typeof(Image)); fill.transform.SetParent(panel.transform, false);
            var fillRect = fill.GetComponent<RectTransform>(); fillRect.anchorMin = Vector2.zero; fillRect.anchorMax = Vector2.one; fillRect.offsetMin = new Vector2(3,3); fillRect.offsetMax = new Vector2(-3,-28);
            fill.GetComponent<Image>().color = new Color(.2f,.85f,.78f); hud.energyFill = fillRect;
            var text = new GameObject("MP text", typeof(RectTransform), typeof(Text)); text.transform.SetParent(panel.transform,false);
            var textRect = text.GetComponent<RectTransform>(); textRect.anchorMin = Vector2.zero; textRect.anchorMax = Vector2.one; textRect.offsetMin = new Vector2(0,6); textRect.offsetMax = Vector2.zero;
            var label = text.GetComponent<Text>(); label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf"); label.fontSize = 16; label.alignment = TextAnchor.MiddleCenter; label.color = Color.white; label.raycastTarget = false; label.text = "MP 100 / 100"; hud.energyText = label;
            foreach (var child in hud.GetComponentsInChildren<Text>(true))
                if (child.transform.parent.name == "Controls") child.text = "J Combo  K Heavy/Luft-Smash  L Kick  E Durchbruch  Q Welle  SHIFT Ausweichen  1–4 / 8 / 0 Gegner  R Reset";
            if (crowd)
            {
                var camera = Object.FindAnyObjectByType<ArenaCamera>(); camera.offset = new Vector3(0,8.5f,-13); camera.center = 0;
                foreach (var child in hud.GetComponentsInChildren<Text>(true))
                    if (child.transform.parent.name == "Header" && child != hud.stateText) child.text = "SCHROTTHOF / GRUPPENKAMPF";
            }
            EditorUtility.SetDirty(hud); EditorUtility.SetDirty(encounter);
        }
    }
}
