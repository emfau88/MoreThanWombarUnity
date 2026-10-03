using System;
using System.IO;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace WombatLab.Editor
{
    public static class LabSceneBuilder
    {
        const string Root = "Assets/Game/";

        [MenuItem("Wombat Lab/Build S1 Movement Lab")]
        public static string Build()
        {
            if (EditorApplication.isPlaying) throw new InvalidOperationException("Stop Play mode first.");
            if (AssetDatabase.LoadAssetAtPath<AttackDefinition>(Root + "Data/Heavy_Smash.asset") != null)
                throw new InvalidOperationException("S2 exists. Use Upgrade to S2 Combat Lab; do not reset the scene with the S1 builder.");
            if (!Application.dataPath.Replace('\\', '/').EndsWith("MoreThanWombarUnity/UnityProject/Assets"))
                throw new InvalidOperationException("Builder may only run in the isolated Unity lab.");
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().isDirty)
                throw new InvalidOperationException("Save the current scene before building the lab.");

            foreach (var dir in new[] { "Art/Materials", "Art/Animations", "Data", "Prefabs", "Scenes" })
                Directory.CreateDirectory(Root + dir);
            AssetDatabase.Refresh();

            var fur = Mat("Fur", new Color(.40f, .24f, .14f));
            var warmFur = Mat("WarmFur", new Color(.61f, .40f, .23f));
            var snout = Mat("Snout", new Color(.74f, .57f, .38f));
            var black = Mat("Nose", new Color(.055f, .043f, .035f));
            var white = Mat("Eyes", new Color(.96f, .95f, .84f));
            var gloves = Mat("Gloves", new Color(.08f, .42f, .40f));
            var ground = Mat("Floor", new Color(.17f, .22f, .24f));
            var seams = Mat("Seams", new Color(.11f, .15f, .18f));
            var dark = Mat("Steel", new Color(.13f, .18f, .21f));
            var teal = Mat("ContainerTeal", new Color(.12f, .34f, .36f));
            var rust = Mat("ContainerRust", new Color(.47f, .23f, .13f));
            var safety = Mat("Safety", new Color(.90f, .63f, .17f));
            var glow = Mat("Signal", new Color(.36f, .87f, .83f), true);

            var definition = LoadOrCreate<CharacterDefinition>("Data/Wombat.asset", () => ScriptableObject.CreateInstance<CharacterDefinition>());
            var controller = CreateAnimator();
            var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            scene.name = "CombatLab";
            RenderSettings.ambientMode = AmbientMode.Flat;
            RenderSettings.ambientLight = new Color(.50f, .57f, .63f);
            RenderSettings.skybox = null;
            RenderSettings.fog = true; RenderSettings.fogColor = new Color(.10f, .16f, .20f);
            RenderSettings.fogMode = FogMode.Linear; RenderSettings.fogStartDistance = 25; RenderSettings.fogEndDistance = 48;

            var arena = new GameObject("Arena — S1 graybox (not final environment)").transform;
            Box("Floor", arena, new Vector3(0, -.2f, 0), new Vector3(16, .4f, 7), ground, true);
            for (int x = -6; x <= 6; x += 2)
                Box("Floor seam", arena, new Vector3(x, .008f, 0), new Vector3(.018f, .012f, 7), seams);
            for (int z = -2; z <= 2; z += 2)
                Box("Floor seam", arena, new Vector3(0, .009f, z), new Vector3(16, .012f, .018f), seams);
            for (int x = -7; x <= 7; x++)
            {
                Box("Rear safety dash", arena, new Vector3(x, .018f, 2.85f), new Vector3(.48f, .025f, .12f), safety);
                Box("Front safety dash", arena, new Vector3(x, .018f, -2.85f), new Vector3(.48f, .025f, .12f), safety);
            }
            foreach (int side in new[] { -1, 1 })
            {
                Box("Side barrier", arena, new Vector3(side * 7.8f, .35f, 0), new Vector3(.3f, .7f, 7), dark, true);
                Box("Barrier accent", arena, new Vector3(side * 7.8f, .73f, 0), new Vector3(.32f, .08f, 7), safety);
                Box("End wall", arena, new Vector3(0, .12f, side * 3.45f), new Vector3(15.6f, .24f, .18f), dark, true);
            }
            Container(arena, new Vector3(-4, 1.1f, 4.5f), teal, dark, glow);
            Container(arena, new Vector3(2.3f, 1.1f, 4.5f), rust, dark, glow);
            for (int i = 0; i < 3; i++)
                Box("Stacked crate", arena, new Vector3(6.6f + (i % 2) * .8f, .45f + (i / 2) * .85f, 3.8f), new Vector3(.8f, .8f, .8f), dark);

            var marker = new GameObject("Training marker — combat arrives in S2").transform;
            Shape(PrimitiveType.Cylinder, "Base", marker, new Vector3(3, .08f, .5f), new Vector3(.9f, .08f, .9f), dark);
            Shape(PrimitiveType.Cylinder, "Post", marker, new Vector3(3, .75f, .5f), new Vector3(.13f, .68f, .13f), dark);
            Shape(PrimitiveType.Capsule, "Pad", marker, new Vector3(3, 1.12f, .5f), new Vector3(.65f, .60f, .65f), safety);
            Box("Pad band", marker, new Vector3(3, 1.14f, .16f), new Vector3(.35f, .10f, .025f), dark);

            var player = new GameObject("Wombat — original 3D placeholder");
            var cc = player.AddComponent<CharacterController>();
            cc.height = 1.85f; cc.radius = .43f; cc.center = new Vector3(0, .95f, 0);
            cc.skinWidth = .035f; cc.stepOffset = .18f; cc.slopeLimit = 45;
            player.AddComponent<LabInput>();
            var visual = new GameObject("Visual").transform; visual.SetParent(player.transform, false);
            var rig = Pivot("Rig", visual, Vector3.zero);
            var torso = Pivot("Torso", rig, new Vector3(0, 1.0f, 0));
            Shape(PrimitiveType.Sphere, "Body", torso, Vector3.zero, new Vector3(1.15f, 1.40f, .93f), fur);
            Shape(PrimitiveType.Sphere, "Belly", torso, new Vector3(0, -.13f, .30f), new Vector3(.84f, .97f, .5f), warmFur);
            var head = Pivot("Head", torso, new Vector3(0, .74f, .08f));
            Shape(PrimitiveType.Sphere, "Skull", head, Vector3.zero, new Vector3(.99f, .72f, .79f), fur);
            Shape(PrimitiveType.Sphere, "Muzzle", head, new Vector3(0, -.12f, .32f), new Vector3(.70f, .43f, .49f), snout);
            Shape(PrimitiveType.Sphere, "Nose", head, new Vector3(0, -.015f, .56f), new Vector3(.31f, .21f, .20f), black);
            for (int side = -1; side <= 1; side += 2)
            {
                Shape(PrimitiveType.Sphere, "Ear", head, new Vector3(side * .39f, .29f, -.08f), new Vector3(.29f, .30f, .18f), fur);
                Shape(PrimitiveType.Sphere, "Inner ear", head, new Vector3(side * .39f, .30f, .018f), new Vector3(.15f, .18f, .055f), snout);
                Shape(PrimitiveType.Sphere, "Eye", head, new Vector3(side * .25f, .085f, .31f), new Vector3(.18f, .16f, .095f), white);
                Shape(PrimitiveType.Sphere, "Pupil", head, new Vector3(side * .245f, .075f, .365f), new Vector3(.078f, .10f, .055f), black);
                var brow = Box("Brow", head, new Vector3(side * .25f, .19f, .33f), new Vector3(.24f, .06f, .11f), fur);
                brow.localRotation = Quaternion.Euler(0, 0, side * 12);
                string limb = side < 0 ? "Left" : "Right";
                var arm = Pivot(limb + "Arm", rig, new Vector3(side * .65f, 1.25f, .06f));
                Shape(PrimitiveType.Sphere, "Arm", arm, new Vector3(0, -.18f, 0), new Vector3(.43f, .67f, .45f), fur);
                Shape(PrimitiveType.Sphere, "Fist", arm, new Vector3(side * .03f, -.47f, .18f), new Vector3(.54f, .52f, .58f), gloves);
                var leg = Pivot(limb + "Leg", rig, new Vector3(side * .32f, .40f, -.015f));
                Shape(PrimitiveType.Sphere, "Leg", leg, new Vector3(0, -.12f, 0), new Vector3(.38f, .47f, .38f), fur);
                Shape(PrimitiveType.Sphere, "Foot", leg, new Vector3(0, -.27f, .09f), new Vector3(.46f, .24f, .55f), black);
            }
            var anim = visual.gameObject.AddComponent<Animator>(); anim.runtimeAnimatorController = controller;
            anim.applyRootMotion = false; anim.cullingMode = AnimatorCullingMode.AlwaysAnimate;
            var motor = player.AddComponent<PlayerMotor>(); motor.definition = definition;
            motor.visual = visual; motor.animator = anim;
            visual.localRotation = Quaternion.Euler(0, 135, 0);
            PrefabUtility.SaveAsPrefabAsset(player, Root + "Prefabs/Wombat.prefab");
            Object.DestroyImmediate(player);
            player = (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(Root + "Prefabs/Wombat.prefab"));
            player.transform.position = new Vector3(-2.5f, .05f, -.35f);
            motor = player.GetComponent<PlayerMotor>();

            var camera = new GameObject("Lab Camera").AddComponent<Camera>();
            camera.tag = "MainCamera"; camera.fieldOfView = 43; camera.nearClipPlane = .1f; camera.farClipPlane = 60;
            camera.clearFlags = CameraClearFlags.SolidColor; camera.backgroundColor = new Color(.08f, .13f, .17f);
            camera.gameObject.AddComponent<AudioListener>();
            var urp = camera.gameObject.AddComponent<UniversalAdditionalCameraData>();
            urp.renderPostProcessing = false; urp.antialiasing = AntialiasingMode.FastApproximateAntialiasing;
            var cameraRig = camera.gameObject.AddComponent<ArenaCamera>(); cameraRig.target = player.transform;
            camera.transform.position = new Vector3(0, 8.3f, -12);
            camera.transform.LookAt(new Vector3(0, .7f, 0));
            var sun = new GameObject("Warm key light").AddComponent<Light>();
            sun.type = LightType.Directional; sun.intensity = 1.4f; sun.color = new Color(1, .88f, .72f);
            sun.shadows = LightShadows.Soft; sun.transform.rotation = Quaternion.Euler(48, -32, 0);
            RenderSettings.sun = sun;
            var fill = new GameObject("Cool fill light").AddComponent<Light>();
            fill.type = LightType.Directional; fill.intensity = .35f; fill.color = new Color(.53f, .77f, 1);
            fill.transform.rotation = Quaternion.Euler(25, 140, 0);
            Hud(motor);

            EditorSceneManager.SaveScene(scene, Root + "Scenes/CombatLab.unity");
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(Root + "Scenes/CombatLab.unity", true) };
            PlayerSettings.companyName = "Wombat Lab"; PlayerSettings.productName = "More Than Wombat — Movement Lab";
            PlayerSettings.defaultScreenWidth = 1280; PlayerSettings.defaultScreenHeight = 720;
            PlayerSettings.runInBackground = true;
            AssetDatabase.SaveAssets();
            if (SceneView.lastActiveSceneView != null) SceneView.lastActiveSceneView.AlignViewToObject(camera.transform);
            Selection.activeGameObject = player;
            return "S1 CombatLab scene, Wombat prefab, 5 clips, definition and materials saved.";
        }

        static AnimatorController CreateAnimator()
        {
            string path = Root + "Art/Animations/Wombat.controller";
            var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(path);
            if (controller == null) controller = AnimatorController.CreateAnimatorControllerAtPath(path);
            var machine = controller.layers[0].stateMachine;
            foreach (var name in new[] { "Idle", "Walk", "Jump", "Fall", "Land" })
            {
                float length = name == "Walk" ? .65f : name == "Land" ? .16f : 1f;
                var clip = LoadOrCreate<AnimationClip>("Art/Animations/" + name + ".anim", () => new AnimationClip());
                clip.name = name; clip.frameRate = 60;
                float[] times = { 0, length * .25f, length * .5f, length * .75f, length };
                float[] leg = name == "Walk" ? new[] { 0f, 26, 0, -26, 0 } :
                    name == "Jump" ? new[] { -20f, -20, -20, -20, -20 } :
                    name == "Fall" ? new[] { 8f, 8, 8, 8, 8 } : new[] { 0f, 0, 0, 0, 0 };
                float[] arm = name == "Walk" ? new[] { 0f, -18, 0, 18, 0 } :
                    name == "Jump" ? new[] { -40f, -40, -40, -40, -40 } :
                    name == "Fall" ? new[] { -25f, -25, -25, -25, -25 } : new[] { -12f, -10, -12, -14, -12 };
                Curve(clip, "Rig/LeftLeg", "localEulerAnglesRaw.x", times, leg);
                Curve(clip, "Rig/RightLeg", "localEulerAnglesRaw.x", times, name == "Walk" ? Negate(leg) : leg);
                Curve(clip, "Rig/LeftArm", "localEulerAnglesRaw.x", times, arm);
                Curve(clip, "Rig/RightArm", "localEulerAnglesRaw.x", times, name == "Walk" ? Negate(arm) : arm);
                Curve(clip, "Rig", "m_LocalPosition.y", times, name == "Land" ? new[] { 0f, -.13f, -.10f, -.04f, 0 } :
                    name == "Walk" ? new[] { 0f, .035f, 0, .035f, 0 } : new[] { 0f, .01f, .025f, .01f, 0 });
                Curve(clip, "Rig/Torso", "localEulerAnglesRaw.z", times, name == "Walk" ? new[] { 0f, -3, 0, 3, 0 } : new[] { 0f, 0, 0, 0, 0 });
                var settings = AnimationUtility.GetAnimationClipSettings(clip);
                settings.loopTime = name != "Land"; AnimationUtility.SetAnimationClipSettings(clip, settings);
                AnimatorState state = null;
                foreach (var candidate in machine.states) if (candidate.state.name == name) state = candidate.state;
                if (state == null) state = machine.AddState(name);
                state.motion = clip; state.writeDefaultValues = true;
                if (name == "Idle") machine.defaultState = state;
                EditorUtility.SetDirty(clip); EditorUtility.SetDirty(controller);
            }
            return controller;
        }

        static float[] Negate(float[] values)
        { var result = new float[values.Length]; for (int i = 0; i < values.Length; i++) result[i] = -values[i]; return result; }
        static void Curve(AnimationClip clip, string path, string property, float[] times, float[] values)
        {
            var keys = new Keyframe[times.Length]; for (int i = 0; i < keys.Length; i++) keys[i] = new Keyframe(times[i], values[i]);
            AnimationUtility.SetEditorCurve(clip, EditorCurveBinding.FloatCurve(path, typeof(Transform), property), new AnimationCurve(keys));
        }
        static T LoadOrCreate<T>(string path, Func<T> create) where T : Object
        {
            var item = AssetDatabase.LoadAssetAtPath<T>(Root + path);
            if (item == null) { item = create(); AssetDatabase.CreateAsset(item, Root + path); }
            return item;
        }
        static Material Mat(string name, Color color, bool emissive = false)
        {
            var mat = LoadOrCreate<Material>("Art/Materials/" + name + ".mat", () => new Material(Shader.Find("Universal Render Pipeline/Lit")));
            mat.SetColor("_BaseColor", color); mat.SetFloat("_Smoothness", .18f);
            if (emissive) { mat.EnableKeyword("_EMISSION"); mat.SetColor("_EmissionColor", color * 1.5f); }
            EditorUtility.SetDirty(mat); return mat;
        }
        static Transform Pivot(string name, Transform parent, Vector3 position)
        { var t = new GameObject(name).transform; t.SetParent(parent, false); t.localPosition = position; return t; }
        static Transform Shape(PrimitiveType type, string name, Transform parent, Vector3 position, Vector3 scale, Material mat, bool collision = false)
        {
            var go = GameObject.CreatePrimitive(type); go.name = name; go.transform.SetParent(parent, false);
            go.transform.localPosition = position; go.transform.localScale = scale;
            go.GetComponent<Renderer>().sharedMaterial = mat;
            if (!collision) Object.DestroyImmediate(go.GetComponent<Collider>());
            return go.transform;
        }
        static Transform Box(string name, Transform parent, Vector3 position, Vector3 scale, Material mat, bool collision = false)
            => Shape(PrimitiveType.Cube, name, parent, position, scale, mat, collision);
        static void Container(Transform parent, Vector3 position, Material mat, Material dark, Material signal)
        {
            var root = Pivot("Graybox container", parent, position);
            Box("Shell", root, Vector3.zero, new Vector3(5.5f, 2.2f, 1.8f), mat);
            for (int i = -5; i <= 5; i++) Box("Rib", root, new Vector3(i * .48f, 0, -.93f), new Vector3(.08f, 2.1f, .08f), dark);
            Box("Signal strip", root, new Vector3(0, .96f, -.98f), new Vector3(4.8f, .05f, .05f), signal);
        }
        public static void Hud(PlayerMotor player, TrainingDummy dummy = null)
        {
            var go = new GameObject("Lab HUD", typeof(Canvas), typeof(CanvasScaler));
            go.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            var scaler = go.GetComponent<CanvasScaler>(); scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1280, 720); scaler.matchWidthOrHeight = .5f;
            var header = Panel(go.transform, "Header", new Vector2(.035f, .865f), new Vector2(.965f, .97f));
            Label(header, dummy == null ? "MORE THAN WOMBAT   /   UNITY MOVEMENT LAB" : "MORE THAN WOMBAT   /   COMBAT LAB", new Vector2(0, .38f), Vector2.one, 24, Color.white);
            var state = Label(header, "S1 — MOVEMENT ONLY", Vector2.zero, new Vector2(1, .43f), 14, new Color(.49f, .87f, .81f));
            var controls = Panel(go.transform, "Controls", new Vector2(.1f, .025f), new Vector2(.9f, .09f));
            Label(controls, dummy == null ? "WASD / ARROWS  Move     SPACE / A  Jump     R / START  Reset     H  Debug" :
                "WASD Move   SPACE Jump   J / LMB Light   K / RMB Heavy   R Reset   H Debug", Vector2.zero, Vector2.one, 17, Color.white);
            var hud = go.AddComponent<LabHud>(); hud.player = player; hud.stateText = state;
            hud.dummy = dummy;
            if (dummy != null)
            {
                var healthPanel = Panel(go.transform, "Training target", new Vector2(.70f, .71f), new Vector2(.965f, .84f));
                hud.healthText = Label(healthPanel, "TRAINING TARGET", new Vector2(0, .42f), Vector2.one, 16, Color.white);
                var track = Panel(healthPanel, "Health track", new Vector2(.06f, .17f), new Vector2(.94f, .36f));
                var fill = Panel(track, "Health fill", Vector2.zero, Vector2.one);
                fill.GetComponent<Image>().color = new Color(.26f, .78f, .65f);
                hud.healthFill = fill.GetComponent<RectTransform>();
            }
        }
        static Transform Panel(Transform parent, string name, Vector2 min, Vector2 max)
        {
            var go = new GameObject(name, typeof(RectTransform), typeof(Image)); go.transform.SetParent(parent, false);
            var rect = go.GetComponent<RectTransform>(); rect.anchorMin = min; rect.anchorMax = max; rect.offsetMin = rect.offsetMax = Vector2.zero;
            go.GetComponent<Image>().color = new Color(.035f, .065f, .09f, .94f); return go.transform;
        }
        static Text Label(Transform parent, string value, Vector2 min, Vector2 max, int size, Color color)
        {
            var go = new GameObject("Label", typeof(RectTransform), typeof(Text)); go.transform.SetParent(parent, false);
            var rect = go.GetComponent<RectTransform>(); rect.anchorMin = min; rect.anchorMax = max;
            rect.offsetMin = new Vector2(16, 0); rect.offsetMax = new Vector2(-16, 0);
            var text = go.GetComponent<Text>(); text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.text = value; text.fontSize = size; text.color = color; text.alignment = TextAnchor.MiddleLeft;
            text.raycastTarget = false; return text;
        }
    }
}
