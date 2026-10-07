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
    public static class HumanoidCombatBuilder
    {
        public const string Root = "Assets/Game/Characters/HumanoidCombat/";
        public const string ScenePath = "Assets/Game/Scenes/HumanoidCombatLab.unity";
        const string Probe = "Assets/Game/Characters/ImportProbe/";

        [MenuItem("Wombat Lab/B2 Build Humanoid Combat Lab")]
        public static string Build()
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling || EditorUtility.scriptCompilationFailed)
                throw new InvalidOperationException("Stop Play and finish compilation first.");
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().isDirty)
                throw new InvalidOperationException("Save current scene first.");
            if (!AssetDatabase.IsValidFolder(Root.TrimEnd('/')))
                AssetDatabase.CreateFolder("Assets/Game/Characters", "HumanoidCombat");
            ImportPunches();
            var jab = AssetDatabase.LoadAssetAtPath<AnimationClip>(Root + "Source_Jab.anim");
            var cross = AssetDatabase.LoadAssetAtPath<AnimationClip>(Root + "Source_Cross.anim");
            var kick = CreateKick();
            var airKick = CreateKick(true);
            string controllerPath = Root + "HumanoidCombat.controller";
            if (AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath) == null)
                AssetDatabase.CopyAsset(Probe + "ImportProbe.controller", controllerPath);
            var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath);
            var prefab = PrefabUtility.LoadPrefabContents(Probe + "HumanoidProbe.prefab");
            try
            {
                prefab.name = "Humanoid combat — temporary human";
                var motor = prefab.GetComponent<PlayerMotor>();
                motor.animator.runtimeAnimatorController = controller;
                motor.animator.Rebind(); motor.animator.Update(0);
                var combat = prefab.AddComponent<CombatController>();
                prefab.AddComponent<PlayerDefense>();
                combat.leftFist = Contact(motor, HumanBodyBones.LeftHand);
                combat.rightFist = Contact(motor, HumanBodyBones.RightHand);
                combat.leftFoot = Contact(motor, HumanBodyBones.LeftFoot);
                combat.rightFoot = Contact(motor, HumanBodyBones.RightFoot);
                var jabPeak = Peak(motor, controller, jab, false);
                var crossPeak = Peak(motor, controller, cross, false);
                var kickPeak = Peak(motor, controller, kick, true);
                var airKickPeak = Peak(motor, controller, airKick, true);
                combat.lights = new[] {
                    Attack(motor, controller, "Light1_Jab", "Light1_Jab", jab, jabPeak, .10f, .09f, .19f),
                    Attack(motor, controller, "Light2_Cross", "Light2_Cross", cross, crossPeak, .13f, .10f, .20f),
                    Attack(motor, controller, "Light3_Finisher", "Light3_Finisher", cross, crossPeak, .17f, .11f, .28f)
                };
                combat.heavy = Attack(motor, controller, "Heavy_Smash", "Heavy_Smash", cross, crossPeak, .29f, .13f, .44f);
                combat.kick = Attack(motor, controller, "Kick", "Kick", kick, kickPeak, .18f, .13f, .34f);
                combat.airKick = Attack(motor, controller, "Air_Kick", "Air_Kick", airKick, airKickPeak, .08f, .16f, .24f);
                combat.airHeavy = Attack(motor, controller, "Air_Smash", "Air_Smash", cross, crossPeak, .13f, .16f, .27f);
                var machine = controller.layers[0].stateMachine;
                machine.RemoveState(machine.states.Select(s => s.state).First(s => s.name == "ContactBake"));
                PrefabUtility.SaveAsPrefabAsset(prefab, Root + "HumanoidCombat.prefab");
            }
            finally { PrefabUtility.UnloadPrefabContents(prefab); }
            AssetDatabase.SaveAssets();

            var scene = EditorSceneManager.OpenScene("Assets/Game/Scenes/SparringLab.unity");
            EditorSceneManager.SaveScene(scene, ScenePath);
            var oldPlayer = Object.FindAnyObjectByType<PlayerMotor>();
            Vector3 spawn = oldPlayer.transform.position;
            Quaternion facing = oldPlayer.visual.localRotation;
            Object.DestroyImmediate(oldPlayer.gameObject);
            var player = ((GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(Root + "HumanoidCombat.prefab"))).GetComponent<PlayerMotor>();
            player.transform.position = spawn; player.visual.localRotation = facing;
            var owner = player.GetComponent<CombatController>();
            var encounter = Object.FindAnyObjectByType<EngagementCoordinator>(); encounter.player = player;
            foreach (var enemy in encounter.enemies) { enemy.player = player; enemy.target.clock = owner; }
            var feedback = Object.FindAnyObjectByType<CombatFeedback>(); feedback.combat = owner; owner.feedback = feedback;
            Object.FindAnyObjectByType<ArenaCamera>().target = player.transform;
            var hud = Object.FindAnyObjectByType<LabHud>(); hud.player = player;
            foreach (var text in hud.GetComponentsInChildren<Text>())
                if (text.transform.parent.name == "Header" && text != hud.stateText)
                    text.text = "MORE THAN WOMBAT / B2 HUMANOID COMBAT — TEMPORARY HUMAN";
            PrefabUtility.RecordPrefabInstancePropertyModifications(owner);
            PrefabUtility.RecordPrefabInstancePropertyModifications(player);
            EditorSceneManager.MarkSceneDirty(scene); EditorSceneManager.SaveScene(scene);
            var scenes = EditorBuildSettings.scenes.ToList();
            if (!scenes.Any(s => s.path == ScenePath)) scenes.Add(new EditorBuildSettingsScene(ScenePath, true));
            EditorBuildSettings.scenes = scenes.ToArray();
            AssetDatabase.SaveAssets(); Selection.activeGameObject = player.gameObject;
            return "Humanoid Combat Lab: seven attacks, authored timing, avatar contact paths, existing defense/opponents/feedback. Wombat art remains open.";
        }

        static Transform Contact(PlayerMotor motor, HumanBodyBones bone)
            => motor.animator.GetBoneTransform(bone).Find(bone + "Contact");

        static void ImportPunches()
        {
            string source = Probe + "MovementSource.fbx";
            var importer = (ModelImporter)AssetImporter.GetAtPath(source);
            var clips = importer.clipAnimations.Where(c => c.name != "Source_Jab" && c.name != "Source_Cross").ToList();
            foreach (var pair in new[] { ("Source_Jab", "Punch_Jab"), ("Source_Cross", "Punch_Cross") })
            {
                var take = importer.defaultClipAnimations.First(c => c.name == "Armature|" + pair.Item2);
                take.name = pair.Item1; take.loopTime = false;
                take.lockRootRotation = take.lockRootHeightY = take.lockRootPositionXZ = true;
                take.heightFromFeet = true; clips.Add(take);
            }
            importer.clipAnimations = clips.ToArray(); importer.SaveAndReimport();
            foreach (string name in new[] { "Source_Jab", "Source_Cross" })
                SaveClip(Object.Instantiate(AssetDatabase.LoadAllAssetsAtPath(source).OfType<AnimationClip>().First(c => c.name == name)), name);
        }

        // Only the missing kick is authored locally, using the imported idle's human-muscle curves.
        static AnimationClip CreateKick(bool airborne = false)
        {
            var idle = AssetDatabase.LoadAssetAtPath<AnimationClip>(Probe + "Idle.anim");
            var clip = Object.Instantiate(idle);
            foreach (var binding in AnimationUtility.GetCurveBindings(clip))
            {
                float value = AnimationUtility.GetEditorCurve(clip, binding).Evaluate(0);
                AnimationUtility.SetEditorCurve(clip, binding, AnimationCurve.Constant(0, 1, value));
            }
            float[] times = { 0, .18f, .34f, .43f, .70f, 1 };
            Muscle(clip, "Right Upper Leg Front-Back", times, airborne
                ? new[] { 0f, -.15f, -.35f, -.35f, -.15f, 0 }
                : new[] { 0f, -.70f, -.92f, -.92f, -.20f, 0 });
            Muscle(clip, "Right Upper Leg In-Out", times, new[] { 0f, -.12f, -.12f, -.12f, 0, 0 });
            Muscle(clip, "Right Lower Leg Stretch", times, airborne
                ? new[] { .7f, .1f, .95f, .95f, .5f, .7f }
                : new[] { .7f, -.85f, .95f, .95f, -.4f, .7f });
            Muscle(clip, "Right Foot Up-Down", times, new[] { 0f, .1f, -.25f, -.25f, .1f, 0 });
            Muscle(clip, "Spine Front-Back", times, new[] { 0f, -.12f, -.25f, -.25f, 0, 0 });
            var settings = AnimationUtility.GetAnimationClipSettings(clip); settings.loopTime = false;
            AnimationUtility.SetAnimationClipSettings(clip, settings);
            return SaveClip(clip, airborne ? "Source_AirKick" : "Source_Kick");
        }

        internal static void Muscle(AnimationClip clip, string name, float[] times, float[] values)
        {
            var keys = times.Select((t, i) => new Keyframe(t, values[i])).ToArray();
            var curve = new AnimationCurve(keys);
            for (int i = 0; i < keys.Length; i++)
            { AnimationUtility.SetKeyLeftTangentMode(curve, i, AnimationUtility.TangentMode.ClampedAuto); AnimationUtility.SetKeyRightTangentMode(curve, i, AnimationUtility.TangentMode.ClampedAuto); }
            AnimationUtility.SetEditorCurve(clip, EditorCurveBinding.FloatCurve("", typeof(Animator), name), curve);
        }

        internal static AnimatorState State(AnimatorController controller, string name, AnimationClip clip)
        {
            var machine = controller.layers[0].stateMachine;
            var state = machine.states.Select(s => s.state).FirstOrDefault(s => s.name == name) ?? machine.AddState(name);
            state.motion = clip; state.writeDefaultValues = false; state.iKOnFeet = false; state.speed = 1;
            EditorUtility.SetDirty(state); EditorUtility.SetDirty(controller); return state;
        }

        internal static (bool right, float phase) Peak(PlayerMotor motor, AnimatorController controller, AnimationClip source, bool foot)
        {
            State(controller, "ContactBake", source);
            motor.animator.Rebind(); motor.animator.Update(0);
            float best = float.MinValue, phase = .3f; bool right = true;
            foreach (bool side in new[] { false, true })
                for (int i = 0; i <= 80; i++)
                {
                    float p = i / 80f;
                    motor.animator.Play("ContactBake", 0, p); motor.animator.Update(0);
                    var bone = foot ? (side ? HumanBodyBones.RightFoot : HumanBodyBones.LeftFoot) : (side ? HumanBodyBones.RightHand : HumanBodyBones.LeftHand);
                    float z = motor.visual.InverseTransformPoint(Contact(motor, bone).position).z;
                    if (z > best) { best = z; phase = p; right = side; }
                }
            return (right, Mathf.Clamp(phase, .08f, .85f));
        }

        internal static AttackDefinition Attack(PlayerMotor motor, AnimatorController controller, string name, string template,
            AnimationClip source, (bool right, float phase) peak, float startup, float active, float recovery, HumanBodyBones? contactBone = null)
        {
            string path = Root + name + ".asset";
            var definition = AssetDatabase.LoadAssetAtPath<AttackDefinition>(path);
            var original = AssetDatabase.LoadAssetAtPath<AttackDefinition>("Assets/Game/Data/" + template + ".asset");
            if (definition == null) { definition = Object.Instantiate(original); AssetDatabase.CreateAsset(definition, path); }
            else EditorUtility.CopySerialized(original, definition);
            definition.name = name; definition.stateName = name; definition.rightHand = peak.right;
            definition.startupSeconds = startup; definition.activeSeconds = active; definition.recoverySeconds = recovery;
            float sourceStart = Mathf.Max(.01f, peak.phase - .06f), sourceEnd = Mathf.Min(.99f, peak.phase + .06f);
            definition.clip = Retime(source, name, sourceStart, sourceEnd, startup, active, recovery);
            State(controller, name, definition.clip);
            motor.animator.Rebind(); motor.animator.Update(0);
            definition.contactAvatar = motor.animator.avatar; definition.contactClip = definition.clip;
            definition.contactPoints = new Vector3[161];
            var contact = definition.foot ? (peak.right ? HumanBodyBones.RightFoot : HumanBodyBones.LeftFoot) : (peak.right ? HumanBodyBones.RightHand : HumanBodyBones.LeftHand);
            for (int i = 0; i < definition.contactPoints.Length; i++)
            {
                motor.animator.Play(name, 0, i / (float)(definition.contactPoints.Length - 1)); motor.animator.Update(0);
                var point = contactBone.HasValue ? motor.animator.GetBoneTransform(contactBone.Value) : Contact(motor, contact);
                definition.contactPoints[i] = motor.visual.InverseTransformPoint(point.position);
            }
            motor.animator.Play("Idle", 0, 0); motor.animator.Update(0);
            EditorUtility.SetDirty(definition); return definition;
        }

        static AnimationClip Retime(AnimationClip source, string name, float start, float end, float startup, float active, float recovery)
        {
            var clip = Object.Instantiate(source);
            float length = source.length;
            float Map(float time)
            {
                float phase = time / length;
                return phase < start ? phase / start * startup
                    : phase <= end ? startup + (phase - start) / (end - start) * active
                    : startup + active + (phase - end) / (1 - end) * recovery;
            }
            foreach (var binding in AnimationUtility.GetCurveBindings(source))
            {
                var oldCurve = AnimationUtility.GetEditorCurve(source, binding);
                // Resample also at section boundaries; linear tangents prevent time-warp overshoot.
                var times = oldCurve.keys.Select(k => k.time).Concat(new[] { 0f, start * length, end * length, length }).Distinct().OrderBy(t => t);
                var curve = new AnimationCurve(times.Select(t => new Keyframe(Map(t), oldCurve.Evaluate(t))).ToArray());
                for (int i = 0; i < curve.length; i++)
                { AnimationUtility.SetKeyLeftTangentMode(curve, i, AnimationUtility.TangentMode.Linear); AnimationUtility.SetKeyRightTangentMode(curve, i, AnimationUtility.TangentMode.Linear); }
                AnimationUtility.SetEditorCurve(clip, binding, curve);
            }
            var settings = AnimationUtility.GetAnimationClipSettings(clip);
            settings.loopTime = false; settings.startTime = 0; settings.stopTime = startup + active + recovery;
            AnimationUtility.SetAnimationClipSettings(clip, settings);
            return SaveClip(clip, name);
        }

        internal static AnimationClip SaveClip(AnimationClip clip, string name)
        {
            clip.name = name;
            var old = AssetDatabase.LoadAssetAtPath<AnimationClip>(Root + name + ".anim");
            if (old == null) { AssetDatabase.CreateAsset(clip, Root + name + ".anim"); return clip; }
            EditorUtility.CopySerialized(clip, old); Object.DestroyImmediate(clip); EditorUtility.SetDirty(old); return old;
        }
    }
}
