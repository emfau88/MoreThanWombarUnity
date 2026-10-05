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
    public static class BodyRecoveryBuilder
    {
        const string Root = HumanoidCombatBuilder.Root;
        [MenuItem("Wombat Lab/B3b Upgrade Knockdown and GetUp")]
        public static string Build()
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling || EditorUtility.scriptCompilationFailed)
                throw new InvalidOperationException("Stop Play and finish compilation first.");
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().isDirty)
                throw new InvalidOperationException("Save current scene first.");
            ImportSources();
            var fall = Fall(); var getUp = GetUp();
            var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(Root + "HumanoidCombat.controller");
            States(controller, fall, getUp);
            var prefab = PrefabUtility.LoadPrefabContents(Root + "HumanoidCombat.prefab");
            try
            {
                var motor = prefab.GetComponent<PlayerMotor>(); var combat = prefab.GetComponent<CombatController>();
                Configure(prefab, motor.animator, combat, fall, getUp);
                foreach (var attack in new[] { combat.heavy, combat.airHeavy })
                { attack.knocksDown = true; EditorUtility.SetDirty(attack); }
                PrefabUtility.SaveAsPrefabAsset(prefab, Root + "HumanoidCombat.prefab");
            }
            finally { PrefabUtility.UnloadPrefabContents(prefab); }
            var scene = EditorSceneManager.OpenScene(HumanoidCombatBuilder.ScenePath);
            var encounter = Object.FindAnyObjectByType<EngagementCoordinator>();
            var clock = encounter.player.GetComponent<CombatController>();
            var robotController = AssetDatabase.LoadAssetAtPath<AnimatorController>(Root + "RobotReactions.controller");
            var robotFall = RobotFall(); var robotGetUp = RobotGetUp(robotFall);
            States(robotController, robotFall, robotGetUp, true);
            var enemyAttack = AssetDatabase.LoadAssetAtPath<AttackDefinition>(Root + "Enemy_Heavy.asset");
            if (enemyAttack == null)
            { enemyAttack = Object.Instantiate(encounter.enemies[0].attack); AssetDatabase.CreateAsset(enemyAttack, Root + "Enemy_Heavy.asset"); }
            enemyAttack.knocksDown = true; EditorUtility.SetDirty(enemyAttack);
            foreach (var enemy in encounter.enemies)
            { enemy.attack = enemyAttack; Configure(enemy.gameObject, enemy.animator, clock, robotFall, robotGetUp); }
            foreach (var text in Object.FindAnyObjectByType<LabHud>().GetComponentsInChildren<Text>())
                if (text.transform.parent.name == "Header" && text != Object.FindAnyObjectByType<LabHud>().stateText)
                    text.text = "MORE THAN WOMBAT / B3b — KNOCKDOWN AND GETUP";
            EditorSceneManager.MarkSceneDirty(scene); EditorSceneManager.SaveScene(scene); AssetDatabase.SaveAssets();
            Selection.activeGameObject = encounter.player.gameObject;
            return "B3b: heavy/air-smash knockdown, protected down/get-up, player/robot death and full reset.";
        }
        static void Configure(GameObject target, Animator animator, CombatController clock, AnimationClip fall, AnimationClip getUp)
        {
            var body = target.GetComponent<BodyRecovery>() ?? target.AddComponent<BodyRecovery>();
            body.animator = animator; body.clock = clock; body.fallClip = fall; body.getUpClip = getUp;
            body.fallSeconds = .42f; body.downSeconds = .4f; body.getUpSeconds = .8f; body.protectionSeconds = .45f;
        }
        static void States(AnimatorController controller, AnimationClip fall, AnimationClip getUp, bool transformRig = false)
        {
            foreach (var pair in new[] { ("Knockdown", fall), ("GetUp", getUp), ("Death", fall) })
                HumanoidCombatBuilder.State(controller, pair.Item1, pair.Item2).writeDefaultValues = transformRig;
        }
        static AnimationClip Clip(string name) => AssetDatabase.LoadAssetAtPath<AnimationClip>(Root + name + ".anim");
        static void ImportSources()
        {
            const string source = "Assets/Game/Characters/ImportProbe/MovementSource.fbx";
            var importer = (ModelImporter)AssetImporter.GetAtPath(source);
            var names = new[] { ("Source_Death", "Death01"), ("Source_Kneel", "Fixing_Kneeling"), ("Source_Crouch", "Crouch_Idle_Loop") };
            if (names.Any(n => !importer.clipAnimations.Any(c => c.name == n.Item1)))
            {
                var clips = importer.clipAnimations.Where(c => !names.Any(n => n.Item1 == c.name)).ToList();
                foreach (var pair in names)
                {
                    var take = importer.defaultClipAnimations.First(c => c.name == "Armature|" + pair.Item2);
                    take.name = pair.Item1; take.loopTime = false;
                    take.lockRootRotation = take.lockRootHeightY = take.lockRootPositionXZ = true; take.heightFromFeet = true;
                    clips.Add(take);
                }
                importer.clipAnimations = clips.ToArray(); importer.SaveAndReimport();
            }
            foreach (var pair in names)
                HumanoidCombatBuilder.SaveClip(Object.Instantiate(AssetDatabase.LoadAllAssetsAtPath(source).OfType<AnimationClip>().First(c => c.name == pair.Item1)), pair.Item1);
        }
        static AnimationClip Fall()
        {
            var source = Clip("Source_Death"); var clip = Object.Instantiate(source);
            float sourceEnd = source.length * .6f;
            foreach (var binding in AnimationUtility.GetCurveBindings(source))
            {
                var curve = AnimationUtility.GetEditorCurve(source, binding);
                var keys = Enumerable.Range(0, 61).Select(i => new Keyframe(i / 60f * .42f, curve.Evaluate(i / 60f * sourceEnd))).ToArray();
                AnimationUtility.SetEditorCurve(clip, binding, Smooth(keys));
            }
            SetLength(clip, .42f); return HumanoidCombatBuilder.SaveClip(clip, "Knockdown");
        }
        static AnimationClip GetUp()
        {
            var fall = Clip("Knockdown"); var kneel = Clip("Source_Kneel"); var crouch = Clip("Source_Crouch");
            var idle = AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/Game/Characters/ImportProbe/Idle.anim");
            var clip = Object.Instantiate(fall);
            // Back -> side/support -> kneel -> crouch -> stand. Not a reversed death playback.
            float[] times = { 0, .15f, .4f, .62f, .8f };
            foreach (var binding in AnimationUtility.GetCurveBindings(fall))
            {
                float Read(AnimationClip source, float time) => AnimationUtility.GetEditorCurve(source, binding)?.Evaluate(time)
                    ?? AnimationUtility.GetEditorCurve(idle, binding)?.Evaluate(0) ?? 0;
                float lying = Read(fall, fall.length);
                float[] values = { lying, lying, Read(kneel, kneel.length * .3f), Read(crouch, 0), Read(idle, 0) };
                if (binding.propertyName == "Spine Front-Back") values[1] = .6f;
                if (binding.propertyName == "RootT.y") values[1] += .15f; // Lift into side support before tucking the knees.
                // Keep the lying leg shape through the side roll; tuck during the rise to kneeling.
                AnimationUtility.SetEditorCurve(clip, binding, Smooth(times.Select((t,i) => new Keyframe(t,values[i])).ToArray()));
            }
            // Roll onto the side before placing hands/knees beneath the body.
            var rotation = new Quaternion(Value(fall,"RootQ.x"), Value(fall,"RootQ.y"), Value(fall,"RootQ.z"), Value(fall,"RootQ.w"));
            var sideRotation = Quaternion.AngleAxis(20, Vector3.forward) * rotation;
            foreach (var pair in new[] { ("RootQ.x",sideRotation.x), ("RootQ.y",sideRotation.y), ("RootQ.z",sideRotation.z), ("RootQ.w",sideRotation.w) })
            {
                var binding = EditorCurveBinding.FloatCurve("", typeof(Animator), pair.Item1);
                var curve = AnimationUtility.GetEditorCurve(clip,binding); var keys=curve.keys;
                keys[1].value=pair.Item2; AnimationUtility.SetEditorCurve(clip,binding,Smooth(keys));
            }
            SetLength(clip, .8f); return HumanoidCombatBuilder.SaveClip(clip, "GetUp");
        }
        static float Value(AnimationClip clip, string property) => AnimationUtility.GetEditorCurve(clip, EditorCurveBinding.FloatCurve("",typeof(Animator),property)).Evaluate(clip.length);
        static AnimationClip RobotFall()
        {
            var clip = Object.Instantiate(AssetDatabase.LoadAssetAtPath<AnimationClip>("Assets/Game/Art/Animations/Idle.anim"));
            foreach (var binding in AnimationUtility.GetCurveBindings(clip))
                AnimationUtility.SetEditorCurve(clip,binding,AnimationCurve.Constant(0,.42f,AnimationUtility.GetEditorCurve(clip,binding).Evaluate(0)));
            float[] times={0,.1f,.28f,.42f};
            Curve(clip,"Rig","localEulerAnglesRaw.x",times,new[]{0f,-12f,-72f,-90f});
            Curve(clip,"Rig","localEulerAnglesRaw.z",times,new[]{0f,8f,5f,0f});
            Curve(clip,"Rig","m_LocalPosition.y",times,new[]{0f,-.06f,.35f,.52f});
            Curve(clip,"Rig/Torso/Head","localEulerAnglesRaw.x",times,new[]{0f,-16f,-8f,0f});
            SetLength(clip,.42f); return HumanoidCombatBuilder.SaveClip(clip,"Robot_Knockdown");
        }
        static AnimationClip RobotGetUp(AnimationClip fall)
        {
            var clip = Object.Instantiate(fall);
            foreach (var binding in AnimationUtility.GetCurveBindings(fall))
            {
                var curve=AnimationUtility.GetEditorCurve(fall,binding);
                var keys=Enumerable.Range(0,41).Select(i=>new Keyframe(i/40f*.8f,curve.Evaluate((1-i/40f)*fall.length))).ToArray();
                AnimationUtility.SetEditorCurve(clip,binding,Smooth(keys));
            }
            Curve(clip,"Rig/Torso","localEulerAnglesRaw.x",new[]{0f,.3f,.6f,.8f},new[]{0f,20f,12f,0f});
            SetLength(clip,.8f); return HumanoidCombatBuilder.SaveClip(clip,"Robot_GetUp");
        }
        static void SetLength(AnimationClip clip,float duration)
        { var settings=AnimationUtility.GetAnimationClipSettings(clip); settings.loopTime=false;settings.startTime=0;settings.stopTime=duration;AnimationUtility.SetAnimationClipSettings(clip,settings); }
        static AnimationCurve Smooth(Keyframe[] keys)
        {
            var curve=new AnimationCurve(keys);
            for(int i=0;i<keys.Length;i++) { AnimationUtility.SetKeyLeftTangentMode(curve,i,AnimationUtility.TangentMode.ClampedAuto);AnimationUtility.SetKeyRightTangentMode(curve,i,AnimationUtility.TangentMode.ClampedAuto); }
            return curve;
        }
        static void Curve(AnimationClip clip,string path,string property,float[] times,float[] values)
            => AnimationUtility.SetEditorCurve(clip,EditorCurveBinding.FloatCurve(path,typeof(Transform),property),Smooth(times.Select((t,i)=>new Keyframe(t,values[i])).ToArray()));
    }
}
