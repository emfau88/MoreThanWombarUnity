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
    public static class CharacterActionBuilder
    {
        const string Root = HumanoidCombatBuilder.Root;
        [MenuItem("Wombat Lab/B3a Upgrade Character Actions")]
        public static string Build() => Apply(true);
        public static string RefreshActions() => Apply(false);
        static string Apply(bool importSources)
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling || EditorUtility.scriptCompilationFailed)
                throw new InvalidOperationException("Stop Play and finish compilation first.");
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().isDirty)
                throw new InvalidOperationException("Save current scene first.");
            if (importSources) ImportExtras();
            var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(Root + "HumanoidCombat.controller");
            var prefab = PrefabUtility.LoadPrefabContents(Root + "HumanoidCombat.prefab");
            try
            {
                var motor = prefab.GetComponent<PlayerMotor>(); var combat = prefab.GetComponent<CombatController>();
                var definition = AssetDatabase.LoadAssetAtPath<CharacterDefinition>(Root + "Character.asset");
                if (definition == null) { definition = Object.Instantiate(motor.definition); AssetDatabase.CreateAsset(definition, Root + "Character.asset"); }
                definition.moveSpeed = 3f; definition.runSpeed = 6.2f;
                EditorUtility.SetDirty(definition); motor.definition = definition;
                HumanoidCombatBuilder.State(controller, "Run", Clip("Source_Run")).speed = 1.7f;
                controller.layers[0].stateMachine.states.First(s => s.state.name == "Walk").state.speed = 2.0f;
                HumanoidCombatBuilder.State(controller, "Hit", Clip("Source_Hit"));
                HumanoidCombatBuilder.State(controller, "Stagger", Clip("Source_Stagger"));
                var reaction = prefab.GetComponent<AnimationReaction>() ?? prefab.AddComponent<AnimationReaction>();
                reaction.animator = motor.animator; reaction.clock = combat;
                reaction.hitClip = Clip("Source_Hit"); reaction.staggerClip = Clip("Source_Stagger");
                var hook = CreateHook(); var heavy = Clip("Source_Overhand"); var air = CreateAirSmash();
                combat.lights[2] = Make(motor, controller, "Light3_Finisher", hook, .17f, .11f, .28f);
                combat.heavy = Make(motor, controller, "Heavy_Smash", heavy, .29f, .13f, .44f);
                // A downward smash contacts on the authored low pose, not at maximum forward reach.
                combat.airHeavy = HumanoidCombatBuilder.Attack(motor, controller, "Air_Smash", "Air_Smash", air,
                    (true, .4f), .13f, .16f, .27f);
                var kick = CreateMartialKick();
                combat.kick = HumanoidCombatBuilder.Attack(motor, controller, "Kick", "Kick", kick,
                    HumanoidCombatBuilder.Peak(motor, controller, kick, true), .18f, .13f, .34f);
                var airKick = CreateMartialKick(true);
                combat.airKick = HumanoidCombatBuilder.Attack(motor, controller, "Air_Kick", "Air_Kick", airKick,
                    HumanoidCombatBuilder.Peak(motor, controller, airKick, true), .08f, .16f, .24f);
                var machine = controller.layers[0].stateMachine;
                var bake = machine.states.Select(s => s.state).FirstOrDefault(s => s.name == "ContactBake");
                if (bake != null) machine.RemoveState(bake);
                motor.animator.Play("Idle", 0, 0); motor.animator.Update(0);
                PrefabUtility.SaveAsPrefabAsset(prefab, Root + "HumanoidCombat.prefab");
            }
            finally { PrefabUtility.UnloadPrefabContents(prefab); }

            var scene = EditorSceneManager.OpenScene(HumanoidCombatBuilder.ScenePath);
            var player = Object.FindAnyObjectByType<PlayerMotor>();
            var owner = player.GetComponent<CombatController>();
            var encounter = Object.FindAnyObjectByType<EngagementCoordinator>();
            var robots = RobotController(encounter.enemies[0].animator.runtimeAnimatorController);
            foreach (var enemy in encounter.enemies)
            {
                enemy.animator.runtimeAnimatorController = robots;
                var reaction = enemy.GetComponent<AnimationReaction>() ?? enemy.gameObject.AddComponent<AnimationReaction>();
                reaction.animator = enemy.animator; reaction.clock = owner;
                reaction.hitClip = Clip("Robot_Hit"); reaction.staggerClip = Clip("Robot_Stagger");
            }
            foreach (var text in Object.FindAnyObjectByType<LabHud>().GetComponentsInChildren<Text>())
            {
                if (text.transform.parent.name == "Header" && text != Object.FindAnyObjectByType<LabHud>().stateText)
                    text.text = "MORE THAN WOMBAT / B3a ACTIONS — TEMPORARY HUMAN";
                if (text.transform.parent.name == "Controls")
                { text.text = "WASD Move  CTRL / LT Run  SPACE Jump  J Combo  K Heavy  L Kick  SHIFT Evade  1/2 Opponents  R Reset"; text.fontSize = 14; }
            }
            EditorSceneManager.MarkSceneDirty(scene); EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets(); Selection.activeGameObject = player.gameObject;
            return "B3a: walk/run, hook finisher, overhand heavy, two-arm air smash, martial front kick, player/robot Hit and Stagger, existing timing and contacts retained.";
        }
        static AnimationClip Clip(string name) => AssetDatabase.LoadAssetAtPath<AnimationClip>(Root + name + ".anim");
        static void ImportExtras()
        {
            string source = "Assets/Game/Characters/ImportProbe/MovementSource.fbx";
            var importer = (ModelImporter)AssetImporter.GetAtPath(source);
            var names = new[] { ("Source_Run", "Sprint_Loop"), ("Source_Hit", "Hit_Chest"), ("Source_Stagger", "Hit_Head"), ("Source_Overhand", "Sword_Attack") };
            var clips = importer.clipAnimations.Where(c => !names.Any(n => n.Item1 == c.name)).ToList();
            foreach (var pair in names)
            {
                var take = importer.defaultClipAnimations.First(c => c.name == "Armature|" + pair.Item2);
                take.name = pair.Item1; take.loopTime = pair.Item1 == "Source_Run";
                take.lockRootRotation = take.lockRootHeightY = take.lockRootPositionXZ = true; take.heightFromFeet = true;
                clips.Add(take);
            }
            importer.clipAnimations = clips.ToArray(); importer.SaveAndReimport();
            foreach (var pair in names)
                HumanoidCombatBuilder.SaveClip(Object.Instantiate(AssetDatabase.LoadAllAssetsAtPath(source).OfType<AnimationClip>().First(c => c.name == pair.Item1)), pair.Item1);
        }
        static AttackDefinition Make(PlayerMotor motor, AnimatorController controller, string name, AnimationClip source, float startup, float active, float recovery)
            => HumanoidCombatBuilder.Attack(motor, controller, name, name, source,
                HumanoidCombatBuilder.Peak(motor, controller, source, false), startup, active, recovery);

        static AnimationClip CreateHook()
        {
            var clip = Object.Instantiate(Clip("Source_Cross"));
            float[] times = { 0, .12f, .27f, .36f, .58f, clip.length };
            HumanoidCombatBuilder.Muscle(clip, "Right Forearm Stretch", times, new[] { .2f, -.2f, -.45f, -.2f, .1f, .2f });
            HumanoidCombatBuilder.Muscle(clip, "Chest Twist Left-Right", times, new[] { 0f, -.5f, .5f, .65f, .2f, 0 });
            HumanoidCombatBuilder.Muscle(clip, "Right Arm Down-Up", times, new[] { -.4f, -.2f, .15f, .1f, -.2f, -.4f });
            return HumanoidCombatBuilder.SaveClip(clip, "Source_Hook");
        }
        static AnimationClip CreateAirSmash()
        {
            var clip = Object.Instantiate(AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/Game/Characters/ImportProbe/Idle.anim"));
            foreach (var binding in AnimationUtility.GetCurveBindings(clip))
                AnimationUtility.SetEditorCurve(clip, binding, AnimationCurve.Constant(0, 1, AnimationUtility.GetEditorCurve(clip, binding).Evaluate(0)));
            float[] times = { 0, .18f, .30f, .40f, .64f, 1 };
            foreach (string side in new[] { "Left", "Right" })
            {
                HumanoidCombatBuilder.Muscle(clip, side + " Shoulder Down-Up", times, new[] { 0f, .65f, -.4f, -.6f, 0, 0 });
                HumanoidCombatBuilder.Muscle(clip, side + " Arm Down-Up", times, new[] { -.3f, 1.05f, -.7f, -.95f, -.5f, -.3f });
                HumanoidCombatBuilder.Muscle(clip, side + " Arm Front-Back", times, new[] { -.3f, -.15f, -.4f, -.2f, -.3f, -.3f });
                HumanoidCombatBuilder.Muscle(clip, side + " Forearm Stretch", times, new[] { 0f, -.2f, .85f, .9f, -.15f, 0 });
            }
            foreach (string joint in new[] { "Spine", "Chest", "UpperChest" })
                HumanoidCombatBuilder.Muscle(clip, joint + " Front-Back", times, new[] { 0f, -.3f, .65f, .95f, .15f, 0 });
            // Pitch the airborne body into the downstroke; position/gravity still belong to the motor.
            float[] pitch = { 0, -8, 35, 50, 12, 0 };
            HumanoidCombatBuilder.Muscle(clip, "RootQ.x", times, pitch.Select(a => Mathf.Sin(a * Mathf.Deg2Rad * .5f)).ToArray());
            HumanoidCombatBuilder.Muscle(clip, "RootQ.w", times, pitch.Select(a => Mathf.Cos(a * Mathf.Deg2Rad * .5f)).ToArray());
            foreach (string side in new[] { "Left", "Right" })
            {
                HumanoidCombatBuilder.Muscle(clip, side + " Upper Leg Front-Back", times, new[] { 0f, -.6f, -.45f, -.3f, -.1f, 0 });
                HumanoidCombatBuilder.Muscle(clip, side + " Lower Leg Stretch", times, new[] { .7f, -.75f, -.6f, -.35f, .4f, .7f });
            }
            var settings = AnimationUtility.GetAnimationClipSettings(clip); settings.loopTime = false; settings.startTime = 0; settings.stopTime = 1;
            AnimationUtility.SetAnimationClipSettings(clip, settings);
            return HumanoidCombatBuilder.SaveClip(clip, "Source_AirSmash");
        }

        static AnimationClip CreateMartialKick(bool airborne = false)
        {
            var clip = Object.Instantiate(AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/Game/Characters/ImportProbe/Idle.anim"));
            foreach (var binding in AnimationUtility.GetCurveBindings(clip))
                AnimationUtility.SetEditorCurve(clip, binding, AnimationCurve.Constant(0, 1, AnimationUtility.GetEditorCurve(clip, binding).Evaluate(0)));
            // Front kick: chamber, snap extension at torso height, re-chamber, settle.
            float[] times = { 0, .15f, .27f, .35f, .43f, .55f, .8f, 1 };
            HumanoidCombatBuilder.Muscle(clip, "Right Upper Leg Front-Back", times, airborne
                ? new[] { 0f, -.6f, -.35f, -.35f, -.35f, -.6f, -.15f, 0 }
                : new[] { 0f, -.68f, -.56f, -.56f, -.56f, -.7f, -.15f, 0 });
            HumanoidCombatBuilder.Muscle(clip, "Right Lower Leg Stretch", times, new[] { .7f, -.95f, -.65f, .9f, .9f, -.95f, .3f, .7f });
            HumanoidCombatBuilder.Muscle(clip, "Right Upper Leg In-Out", times, new[] { 0f, -.08f, -.08f, -.08f, -.08f, -.08f, 0, 0 });
            HumanoidCombatBuilder.Muscle(clip, "Right Foot Up-Down", times, new[] { 0f, .3f, .5f, .5f, .5f, .3f, 0, 0 });
            if (airborne)
                HumanoidCombatBuilder.Muscle(clip, "Left Upper Leg Front-Back", times, new[] { 0f, -.6f, -.6f, -.6f, -.6f, -.55f, -.15f, 0 });
            HumanoidCombatBuilder.Muscle(clip, "Left Lower Leg Stretch", times, airborne
                ? new[] { .7f, -.9f, -.9f, -.9f, -.9f, -.9f, .2f, .7f }
                : new[] { .7f, .55f, .55f, .55f, .55f, .55f, .65f, .7f });
            HumanoidCombatBuilder.Muscle(clip, "Spine Front-Back", times, new[] { 0f, -.12f, -.22f, -.22f, -.22f, -.15f, 0, 0 });
            HumanoidCombatBuilder.Muscle(clip, "Chest Twist Left-Right", times, new[] { 0f, -.12f, .1f, .14f, .14f, -.08f, 0, 0 });
            foreach (string side in new[] { "Left", "Right" })
            {
                HumanoidCombatBuilder.Muscle(clip, side + " Arm Down-Up", times, new[] { -.3f, -.12f, -.12f, -.12f, -.12f, -.12f, -.3f, -.3f });
                HumanoidCombatBuilder.Muscle(clip, side + " Arm Front-Back", times, new[] { -.3f, -.65f, -.65f, -.65f, -.65f, -.65f, -.3f, -.3f });
                HumanoidCombatBuilder.Muscle(clip, side + " Forearm Stretch", times, new[] { 0f, -.8f, -.8f, -.8f, -.8f, -.8f, -.25f, 0 });
            }
            var settings = AnimationUtility.GetAnimationClipSettings(clip); settings.loopTime = false; settings.startTime = 0; settings.stopTime = 1;
            AnimationUtility.SetAnimationClipSettings(clip, settings);
            return HumanoidCombatBuilder.SaveClip(clip, airborne ? "Source_AirKick" : "Source_Kick");
        }

        static AnimatorController RobotController(RuntimeAnimatorController original)
        {
            string path = Root + "RobotReactions.controller";
            if (AssetDatabase.LoadAssetAtPath<AnimatorController>(path) == null)
                AssetDatabase.CopyAsset(AssetDatabase.GetAssetPath(original), path);
            var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(path);
            foreach (bool strong in new[] { false, true })
            {
                string name = strong ? "Robot_Stagger" : "Robot_Hit";
                float duration = strong ? .4f : .22f;
                var clip = Object.Instantiate(AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/Game/Art/Animations/Idle.anim"));
                foreach (var binding in AnimationUtility.GetCurveBindings(clip))
                    AnimationUtility.SetEditorCurve(clip, binding, AnimationCurve.Constant(0, duration, AnimationUtility.GetEditorCurve(clip, binding).Evaluate(0)));
                float[] times = { 0, duration * .2f, duration * .6f, duration };
                Curve(clip, "Rig/Torso", "localEulerAnglesRaw.x", times, new[] { 0f, strong ? -28f : -12f, strong ? -16f : -6f, 0 });
                Curve(clip, "Rig", "m_LocalPosition.y", times, new[] { 0f, strong ? -.16f : -.05f, -.03f, 0 });
                Curve(clip, "Rig/Torso/Head", "localEulerAnglesRaw.x", times, new[] { 0f, strong ? -24f : -9f, -6f, 0 });
                var settings = AnimationUtility.GetAnimationClipSettings(clip); settings.loopTime = false; settings.startTime = 0; settings.stopTime = duration;
                AnimationUtility.SetAnimationClipSettings(clip, settings);
                clip = HumanoidCombatBuilder.SaveClip(clip, name);
                HumanoidCombatBuilder.State(controller, strong ? "Stagger" : "Hit", clip).writeDefaultValues = true;
            }
            EditorUtility.SetDirty(controller); return controller;
        }
        static void Curve(AnimationClip clip, string path, string name, float[] times, float[] values)
            => AnimationUtility.SetEditorCurve(clip, EditorCurveBinding.FloatCurve(path, typeof(Transform), name), new AnimationCurve(times.Select((t,i) => new Keyframe(t, values[i])).ToArray()));
    }
}
