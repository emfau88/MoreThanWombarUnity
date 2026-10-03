using System;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;
using Object = UnityEngine.Object;

namespace WombatLab.Editor
{
    public static class CombatLabBuilder
    {
        const string Root = "Assets/Game/";
        [MenuItem("Wombat Lab/Upgrade to S2 Combat Lab")]
        public static string Build()
        {
            if (EditorApplication.isPlaying || EditorUtility.scriptCompilationFailed)
                throw new InvalidOperationException("Stop Play and fix compilation before upgrading.");
            if (!Application.dataPath.Replace('\\', '/').EndsWith("MoreThanWombarUnity/UnityProject/Assets"))
                throw new InvalidOperationException("Only the isolated lab may be upgraded.");
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().isDirty)
                throw new InvalidOperationException("Unsaved scene: save before upgrading.");
            var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(Root + "Art/Animations/Wombat.controller");
            if (controller == null) throw new InvalidOperationException("S1 must exist first.");
            // Transform position curves must contain every axis. Sampling a partial
            // vector can default the unbound Y channel to zero (hands at floor height).
            foreach (var name in new[] { "Idle", "Walk", "Jump", "Fall", "Land" })
            {
                var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(Root + "Art/Animations/" + name + ".anim");
                foreach (var side in new[] { "Left", "Right" })
                {
                    var times = new[] { 0f, clip.length };
                    float x = side == "Right" ? .65f : -.65f;
                    Curve(clip, "Rig/" + side + "Arm", "m_LocalPosition.x", times, new[] { x, x });
                    Curve(clip, "Rig/" + side + "Arm", "m_LocalPosition.y", times, new[] { 1.25f, 1.25f });
                    Curve(clip, "Rig/" + side + "Arm", "m_LocalPosition.z", times, new[] { .06f, .06f });
                }
                EditorUtility.SetDirty(clip);
            }
            var lights = new AttackDefinition[3];
            lights[0] = Attack(controller, "Light1_Jab", false, false, .38f, 10, .38f, .045f);
            lights[1] = Attack(controller, "Light2_Cross", true, false, .43f, 13, .48f, .05f);
            lights[2] = Attack(controller, "Light3_Finisher", false, false, .56f, 20, .75f, .065f);
            var heavy = Attack(controller, "Heavy_Smash", true, true, .86f, 32, 1.25f, .085f);

            string prefabPath = Root + "Prefabs/Wombat.prefab";
            var prefab = PrefabUtility.LoadPrefabContents(prefabPath);
            try
            {
                var combat = prefab.GetComponent<CombatController>();
                if (combat == null) combat = prefab.AddComponent<CombatController>();
                combat.lights = lights; combat.heavy = heavy;
                combat.leftFist = prefab.transform.Find("Visual/Rig/LeftArm/Fist");
                combat.rightFist = prefab.transform.Find("Visual/Rig/RightArm/Fist");
                if (combat.leftFist == null || combat.rightFist == null) throw new InvalidOperationException("S1 rig is missing fists.");
                PrefabUtility.SaveAsPrefabAsset(prefab, prefabPath);
            }
            finally { PrefabUtility.UnloadPrefabContents(prefab); }

            var scene = EditorSceneManager.OpenScene(Root + "Scenes/CombatLab.unity");
            var tags = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            var hurtLayer = tags.FindProperty("layers").GetArrayElementAtIndex(8);
            if (!string.IsNullOrEmpty(hurtLayer.stringValue) && hurtLayer.stringValue != "CombatHurtbox")
                throw new InvalidOperationException("Layer 8 is already in use; do not overwrite it.");
            hurtLayer.stringValue = "CombatHurtbox"; tags.ApplyModifiedProperties();
            var motor = Object.FindAnyObjectByType<PlayerMotor>();
            var owner = motor.GetComponent<CombatController>();
            var marker = GameObject.Find("Training marker — combat arrives in S2");
            if (marker != null) Object.DestroyImmediate(marker);
            var oldDummy = Object.FindAnyObjectByType<TrainingDummy>();
            if (oldDummy != null) Object.DestroyImmediate(oldDummy.gameObject);
            var oldFeedback = Object.FindAnyObjectByType<CombatFeedback>();
            if (oldFeedback != null) Object.DestroyImmediate(oldFeedback.gameObject);

            var dummyMaterial = AssetDatabase.LoadAssetAtPath<Material>(Root + "Art/Materials/Safety.mat");
            var steel = AssetDatabase.LoadAssetAtPath<Material>(Root + "Art/Materials/Steel.mat");
            var target = new GameObject("Training Dummy — S2 (R resets health)");
            target.transform.position = new Vector3(2.7f, 0, -.35f);
            Shape(target.transform, PrimitiveType.Cylinder, "Base", new Vector3(0, .07f, 0), new Vector3(.85f, .07f, .85f), steel);
            Shape(target.transform, PrimitiveType.Cylinder, "Post", new Vector3(0, .72f, 0), new Vector3(.12f, .64f, .12f), steel);
            var pad = Shape(target.transform, PrimitiveType.Capsule, "Pad", new Vector3(0, 1.2f, 0), new Vector3(.70f, .56f, .70f), dummyMaterial);
            var dummy = target.AddComponent<TrainingDummy>(); dummy.pad = pad; dummy.padRenderer = pad.GetComponent<Renderer>();
            dummy.clock = owner; owner.dummy = dummy;
            var body = target.AddComponent<CapsuleCollider>(); body.center = new Vector3(0, .85f, 0); body.height = 1.7f; body.radius = .30f;
            var hurt = new GameObject("Hurtbox — team 2, separate from body");
            hurt.transform.SetParent(target.transform, false); hurt.layer = 8;
            var box = hurt.AddComponent<CapsuleCollider>(); box.center = new Vector3(0, 1.14f, 0); box.height = 1.22f;
            box.radius = .37f; box.isTrigger = true;
            // Static trigger query needs no Rigidbody; knockback is explicitly kinematic.
            dummy.clock = null; // A prefab cannot own a reference to a scene-specific player.
            PrefabUtility.SaveAsPrefabAsset(target, Root + "Prefabs/TrainingDummy.prefab");
            dummy.clock = owner;

            var material = AssetDatabase.LoadAssetAtPath<Material>(Root + "Art/Materials/Contact.mat");
            if (material == null)
            {
                material = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
                material.SetColor("_BaseColor", new Color(1, .82f, .31f));
                AssetDatabase.CreateAsset(material, Root + "Art/Materials/Contact.mat");
            }
            var feedback = new GameObject("Contact feedback — original synthesized audio").AddComponent<CombatFeedback>();
            feedback.combat = owner; owner.feedback = feedback;
            feedback.flash = Shape(feedback.transform, PrimitiveType.Sphere, "Contact flash", Vector3.zero, Vector3.one * .2f, material).GetComponent<Renderer>();
            feedback.spark = Line(feedback.transform, "Contact sparks", material);
            feedback.debugSphere = Line(feedback.transform, "H debug active fist", material);
            feedback.flash.enabled = feedback.spark.enabled = feedback.debugSphere.enabled = false;
            var oldHud = Object.FindAnyObjectByType<LabHud>();
            if (oldHud != null) Object.DestroyImmediate(oldHud.gameObject);
            LabSceneBuilder.Hud(motor, dummy);
            // Explicitly persist scene references on the player prefab instance.
            PrefabUtility.RecordPrefabInstancePropertyModifications(owner);

            PlayerSettings.productName = "More Than Wombat — Combat Lab";
            EditorSceneManager.MarkSceneDirty(scene); EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets(); Selection.activeGameObject = motor.gameObject;
            return "S2 saved: 4 attack clips/definitions, Wombat combat, training dummy, contact feedback and HUD.";
        }

        static AttackDefinition Attack(AnimatorController controller, string name, bool right, bool heavy,
            float length, int damage, float knockback, float hitstop)
        {
            string clipPath = Root + "Art/Animations/" + name + ".anim";
            var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(clipPath);
            if (clip == null) { clip = new AnimationClip(); AssetDatabase.CreateAsset(clip, clipPath); }
            clip.name = name; clip.frameRate = 60;
            float[] phases = heavy ? new[] { 0f, .29f, .40f, .52f, .76f, 1f } : new[] { 0f, .16f, .27f, .44f, .68f, 1f };
            var times = new float[phases.Length]; for (int i = 0; i < times.Length; i++) times[i] = phases[i] * length;
            foreach (var side in new[] { "Left", "Right" })
            {
                bool strike = right == (side == "Right");
                float extension = heavy ? .62f : name == "Light3_Finisher" ? .60f : .46f;
                Curve(clip, "Rig/" + side + "Arm", "localEulerAnglesRaw.x", times,
                    strike ? heavy ? new[] { -12f, 35, -108, -95, -30, -12 } : new[] { -12f, 18, -100, -92, -25, -12 }
                        : new[] { -12f, -25, -35, -35, -20, -12 });
                Curve(clip, "Rig/" + side + "Arm", "m_LocalPosition.z", times,
                    strike ? new[] { .06f, -.12f, extension, extension, .14f, .06f }
                        : new[] { .06f, .06f, .06f, .06f, .06f, .06f });
                Curve(clip, "Rig/" + side + "Arm", "m_LocalPosition.y", times, new[] { 1.25f, 1.25f, 1.25f, 1.25f, 1.25f, 1.25f });
                float x = side == "Right" ? .65f : -.65f;
                Curve(clip, "Rig/" + side + "Arm", "m_LocalPosition.x", times,
                    strike ? new[] { x, x, x * .38f, x * .38f, x, x } : new[] { x, x, x, x, x, x });
                Curve(clip, "Rig/" + side + "Leg", "localEulerAnglesRaw.x", times, new[] { 0f, 0, 0, 0, 0, 0 });
            }
            Curve(clip, "Rig/Torso", "localEulerAnglesRaw.z", times, new[] { 0f, right ? -6f : 6, right ? 7f : -7, right ? 4f : -4, 0, 0 });
            Curve(clip, "Rig", "m_LocalPosition.y", times, heavy ? new[] { 0f, -.12f, .055f, .02f, -.06f, 0 } : new[] { 0f, -.03f, .03f, .01f, 0, 0 });
            var settings = AnimationUtility.GetAnimationClipSettings(clip); settings.loopTime = false;
            AnimationUtility.SetAnimationClipSettings(clip, settings);
            var machine = controller.layers[0].stateMachine; AnimatorState state = null;
            foreach (var candidate in machine.states) if (candidate.state.name == name) state = candidate.state;
            if (state == null) state = machine.AddState(name);
            state.motion = clip; state.writeDefaultValues = true;
            string dataPath = Root + "Data/" + name + ".asset";
            var definition = AssetDatabase.LoadAssetAtPath<AttackDefinition>(dataPath);
            if (definition == null) { definition = ScriptableObject.CreateInstance<AttackDefinition>(); AssetDatabase.CreateAsset(definition, dataPath); }
            definition.clip = clip; definition.stateName = name; definition.rightHand = right; definition.heavy = heavy;
            definition.damage = damage; definition.knockback = knockback; definition.hitstop = hitstop;
            definition.hitstun = heavy ? .40f : .22f; definition.radius = heavy ? .38f : .30f;
            definition.activeStart = heavy ? .38f : .25f; definition.activeEnd = heavy ? .53f : .46f;
            definition.chainStart = .58f; definition.chainEnd = .88f; definition.heavyCancelStart = .80f;
            EditorUtility.SetDirty(clip); EditorUtility.SetDirty(definition); EditorUtility.SetDirty(controller);
            return definition;
        }
        static void Curve(AnimationClip clip, string path, string property, float[] times, float[] values)
        {
            var keys = new Keyframe[times.Length];
            for (int i = 0; i < keys.Length; i++) keys[i] = new Keyframe(times[i], values[i]);
            AnimationUtility.SetEditorCurve(clip, EditorCurveBinding.FloatCurve(path, typeof(Transform), property), new AnimationCurve(keys));
        }
        static Transform Shape(Transform parent, PrimitiveType primitive, string name, Vector3 position, Vector3 scale, Material material)
        {
            var go = GameObject.CreatePrimitive(primitive); go.name = name; go.transform.SetParent(parent, false);
            go.transform.localPosition = position; go.transform.localScale = scale;
            go.GetComponent<Renderer>().sharedMaterial = material; Object.DestroyImmediate(go.GetComponent<Collider>());
            return go.transform;
        }
        static LineRenderer Line(Transform parent, string name, Material material)
        {
            var go = new GameObject(name); go.transform.SetParent(parent, false);
            var line = go.AddComponent<LineRenderer>(); line.sharedMaterial = material;
            line.useWorldSpace = false; line.widthMultiplier = .025f;
            line.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off; line.receiveShadows = false;
            return line;
        }
    }
}
