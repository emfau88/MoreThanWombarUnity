using System;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace WombatLab.Editor
{
    // Upgrade existing scenes and assets in place; never rebuild the S1 arena.
    public static class CombatPolishBuilder
    {
        const string Root = "Assets/Game/";
        [MenuItem("Wombat Lab/B1 Polish Combat")]
        public static string Build()
        {
            if (EditorApplication.isPlaying || EditorUtility.scriptCompilationFailed)
                throw new InvalidOperationException("Stop Play and fix compilation first.");
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().isDirty)
                throw new InvalidOperationException("Save the current scene first.");
            var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(Root + "Art/Animations/Wombat.controller");
            // Keep enemy timings and sweep geometry separate from the player's tuning.
            var originalHeavy = AssetDatabase.LoadAssetAtPath<AttackDefinition>(Root + "Data/Heavy_Smash.asset");
            var enemyHeavy = AssetDatabase.LoadAssetAtPath<AttackDefinition>(Root + "Data/Enemy_Heavy.asset");
            if (enemyHeavy == null)
            {
                enemyHeavy = Object.Instantiate(originalHeavy);
                var enemyClip = Object.Instantiate(originalHeavy.clip); enemyClip.name = "Enemy_Heavy";
                AssetDatabase.CreateAsset(enemyClip, Root + "Art/Animations/Enemy_Heavy.anim");
                enemyHeavy.clip = enemyClip; enemyHeavy.stateName = "Enemy_Heavy";
                enemyHeavy.forwardStep = 0;
                AssetDatabase.CreateAsset(enemyHeavy, Root + "Data/Enemy_Heavy.asset");
            }
            State(controller, enemyHeavy.clip);
            foreach (var name in new[] { "Idle", "Walk", "Jump", "Fall", "Land" }) Locomotion(controller, name);
            foreach (var name in new[] { "Light1_Jab", "Light2_Cross", "Light3_Finisher", "Heavy_Smash" })
            {
                var a = AssetDatabase.LoadAssetAtPath<AttackDefinition>(Root + "Data/" + name + ".asset");
                a.forwardStep = a.heavy ? .22f : .23f;
                a.hitChainStart = .48f; a.chainStart = .64f;
                a.knockback = name == "Light1_Jab" ? .17f : name == "Light2_Cross" ? .22f : a.knockback;
                PolishPunch(a); EditorUtility.SetDirty(a);
            }
            var kick = NewAttack(controller, "Kick", .60f, 18, true, false, .30f, .50f);
            var airKick = NewAttack(controller, "Air_Kick", .44f, 14, true, true, .20f, .65f);
            var airHeavy = NewAttack(controller, "Air_Smash", .56f, 26, false, true, .22f, .62f);
            var prefab = PrefabUtility.LoadPrefabContents(Root + "Prefabs/Wombat.prefab");
            Bind(prefab.GetComponent<CombatController>(), kick, airKick, airHeavy);
            PrefabUtility.SaveAsPrefabAsset(prefab, Root + "Prefabs/Wombat.prefab");
            PrefabUtility.UnloadPrefabContents(prefab);
            foreach (var sceneName in new[] { "CombatLab", "SparringLab" })
            {
                var scene = EditorSceneManager.OpenScene(Root + "Scenes/" + sceneName + ".unity");
                var combat = Object.FindAnyObjectByType<CombatController>();
                Bind(combat, kick, airKick, airHeavy);
                PrefabUtility.RecordPrefabInstancePropertyModifications(combat);
                foreach (var enemy in Object.FindObjectsByType<EnemyBrain>(FindObjectsInactive.Include, FindObjectsSortMode.None))
                    enemy.attack = enemyHeavy;
                foreach (var label in Object.FindAnyObjectByType<LabHud>().GetComponentsInChildren<Text>())
                    if (label.transform.parent.name == "Controls")
                    {
                        label.text = "WASD Move   SPACE Jump   J Light   K Heavy   L / RB Kick   SHIFT Evade   1 / 2 Enemies   R Reset";
                        label.fontSize = 14;
                    }
                EditorSceneManager.MarkSceneDirty(scene); EditorSceneManager.SaveScene(scene);
            }
            AssetDatabase.SaveAssets();
            return "B1: upgraded clips, kick/air attacks, foot contacts and both saved lab scenes.";
        }
        static void Bind(CombatController c, AttackDefinition kick, AttackDefinition airKick, AttackDefinition airHeavy)
        {
            c.kick = kick; c.airKick = airKick; c.airHeavy = airHeavy;
            var rig = c.GetComponent<PlayerMotor>().visual.Find("Rig");
            c.leftFoot = rig.Find("LeftLeg/Foot"); c.rightFoot = rig.Find("RightLeg/Foot");
            EditorUtility.SetDirty(c);
        }
        static void PolishPunch(AttackDefinition a)
        {
            float[] t = Times(a.clip.length);
            string side = a.rightHand ? "Right" : "Left";
            Position(a.clip, "Rig/" + side + "Arm", t,
                new[] { a.rightHand ? .65f : -.65f, a.rightHand ? .65f : -.65f, a.rightHand ? .20f : -.20f, a.rightHand ? .20f : -.20f, a.rightHand ? .65f : -.65f, a.rightHand ? .65f : -.65f },
                new[] { 1.25f, 1.25f, 1.25f, 1.25f, 1.25f, 1.25f },
                new[] { .06f, -.14f, a.heavy ? .66f : .54f, a.heavy ? .66f : .54f, .12f, .06f });
            // Contact extension begins at the authored Active boundary, including Heavy.
            var z = a.rightHand ? "Rig/RightArm" : "Rig/LeftArm";
            Curve(a.clip, z, "m_LocalPosition.z", new[] { 0f, a.clip.length * (a.activeStart - .10f), a.clip.length * (a.activeStart + .02f), a.clip.length * a.activeEnd, a.clip.length * .80f, a.clip.length },
                new[] { .06f, -.14f, a.heavy ? .66f : .54f, a.heavy ? .66f : .54f, .12f, .06f });
            Curve(a.clip, "Rig/Torso", "localEulerAnglesRaw.y", t, new[] { 0f, a.rightHand ? 12f : -12f, a.rightHand ? -16f : 16f, a.rightHand ? -10f : 10f, 0f, 0f });
            EditorUtility.SetDirty(a.clip);
        }
        static AttackDefinition NewAttack(AnimatorController c, string name, float length, int damage, bool foot, bool air, float start, float end)
        {
            var clip = Clip(name); clip.ClearCurves();
            float[] t = Times(length);
            Rest(clip, t);
            Position(clip, "Rig", t, new float[6], new[] { 0f, -.08f, .02f, .02f, -.04f, 0f }, new float[6]);
            if (foot)
            {
                Curve(clip, "Rig/RightLeg", "localEulerAnglesRaw.x", t,
                    air ? new[] { -20f, -80f, -50f, -40f, -25f, 0f } : new[] { 0f, -35f, -94f, -88f, -15f, 0f });
                Position(clip, "Rig/RightLeg", t, new[] { .32f, .24f, .16f, .16f, .32f, .32f },
                    air ? new[] { .40f, .65f, .55f, .50f, .52f, .40f } : new[] { .40f, .65f, .85f, .85f, .52f, .40f }, new[] { -.015f, .12f, .60f, .60f, .12f, -.015f });
                Position(clip, "Rig/RightLeg/Foot", t, new float[6],
                    new[] { -.27f, -.35f, -.72f, -.72f, -.32f, -.27f }, new[] { .09f, .09f, .09f, .09f, .09f, .09f });
                Position(clip, "Rig/RightLeg/Leg", t, new float[6], new[] { -.12f, -.20f, -.36f, -.36f, -.16f, -.12f }, new float[6]);
                Curve(clip, "Rig/RightLeg/Leg", "m_LocalScale.y", t, new[] { .47f, .65f, 1.05f, 1.05f, .55f, .47f });
                Curve(clip, "Rig/LeftLeg", "localEulerAnglesRaw.x", t, air ? new[] { -20f, -45f, -55f, -50f, -15f, 0f } : new float[6]);
                Curve(clip, "Rig/Torso", "localEulerAnglesRaw.x", t, new[] { 0f, -8f, -14f, -12f, 0f, 0f });
            }
            else
            {
                Curve(clip, "Rig/RightArm", "localEulerAnglesRaw.x", t, new[] { -12f, -155f, -35f, -20f, -12f, -12f });
                Position(clip, "Rig/RightArm", t, new[] { .65f, .45f, .15f, .15f, .65f, .65f },
                    new[] { 1.25f, 1.32f, .15f, .05f, 1.25f, 1.25f }, new[] { .06f, .02f, .82f, .88f, .06f, .06f });
                Curve(clip, "Rig/Torso", "localEulerAnglesRaw.x", t, new[] { 0f, -12f, 24f, 20f, 0f, 0f });
            }
            State(c, clip);
            string path = Root + "Data/" + name + ".asset";
            var a = AssetDatabase.LoadAssetAtPath<AttackDefinition>(path);
            if (a == null) { a = ScriptableObject.CreateInstance<AttackDefinition>(); AssetDatabase.CreateAsset(a, path); }
            a.clip = clip; a.stateName = name; a.rightHand = true; a.foot = foot; a.airborne = air;
            a.heavy = !foot; a.damage = damage; a.activeStart = start; a.activeEnd = end;
            a.forwardStep = air ? 0 : .27f; a.knockback = air ? .48f : .65f;
            a.radius = .30f; a.hitstop = air ? .055f : .060f; a.hitstun = .30f;
            EditorUtility.SetDirty(a); EditorUtility.SetDirty(clip); return a;
        }
        static void Locomotion(AnimatorController c, string name)
        {
            var clip = Clip(name); clip.ClearCurves();
            float length = name == "Walk" ? .46f : name == "Land" ? .14f : name == "Jump" ? .35f : .70f;
            float[] t = { 0, length * .25f, length * .50f, length * .75f, length };
            Rest(clip, t);
            bool walk = name == "Walk";
            foreach (var side in new[] { "Left", "Right" })
            {
                float sign = side == "Left" ? 1 : -1;
                Curve(clip, "Rig/" + side + "Leg", "localEulerAnglesRaw.x", t,
                    walk ? new[] { -38 * sign, 0, 38 * sign, 0, -38 * sign }
                    : name == "Jump" ? new[] { -8f, -34f, -42f, -38f, -30f }
                    : name == "Fall" ? new[] { -22f, -16f, -10f, 0f, 8f } : new float[5]);
                Curve(clip, "Rig/" + side + "Arm", "localEulerAnglesRaw.x", t,
                    walk ? new[] { 28 * sign, 0, -28 * sign, 0, 28 * sign }
                    : name == "Jump" ? new[] { -15f, -58f, -72f, -65f, -52f }
                    : name == "Fall" ? new[] { -50f, -42f, -35f, -32f, -28f } : new[] { -12f, -14f, -12f, -10f, -12f });
                if (walk) Position(clip, "Rig/" + side + "Leg", t,
                    Repeat(side == "Left" ? -.32f : .32f, 5),
                    side == "Left" ? new[] { .40f, .52f, .40f, .40f, .40f } : new[] { .40f, .40f, .40f, .52f, .40f }, Repeat(-.015f, 5));
            }
            Position(clip, "Rig", t, new float[5], name == "Land" ? new[] { 0f, -.16f, -.11f, -.04f, 0f }
                : walk ? new[] { 0f, .075f, 0f, .075f, 0f } : new[] { 0f, .015f, .025f, .015f, 0f }, new float[5]);
            Curve(clip, "Rig/Torso", "localEulerAnglesRaw.z", t, walk ? new[] { 5f, 0f, -5f, 0f, 5f } : new float[5]);
            Curve(clip, "Rig/Torso", "localEulerAnglesRaw.x", t, Repeat(walk ? 8 : name == "Land" ? 12 : 0, 5));
            var settings = AnimationUtility.GetAnimationClipSettings(clip);
            settings.loopTime = name == "Walk" || name == "Idle"; AnimationUtility.SetAnimationClipSettings(clip, settings);
            State(c, clip); EditorUtility.SetDirty(clip);
        }
        static void Rest(AnimationClip clip, float[] t)
        {
            Position(clip, "Rig", t, Repeat(0, t.Length), Repeat(0, t.Length), Repeat(0, t.Length));
            foreach (var side in new[] { "Left", "Right" })
            {
                Position(clip, "Rig/" + side + "Arm", t, Repeat(side == "Left" ? -.65f : .65f, t.Length), Repeat(1.25f, t.Length), Repeat(.06f, t.Length));
                Position(clip, "Rig/" + side + "Leg", t, Repeat(side == "Left" ? -.32f : .32f, t.Length), Repeat(.40f, t.Length), Repeat(-.015f, t.Length));
                Position(clip, "Rig/" + side + "Leg/Foot", t, Repeat(0, t.Length), Repeat(-.27f, t.Length), Repeat(.09f, t.Length));
                Position(clip, "Rig/" + side + "Leg/Leg", t, Repeat(0, t.Length), Repeat(-.12f, t.Length), Repeat(0, t.Length));
                Curve(clip, "Rig/" + side + "Leg/Leg", "m_LocalScale.y", t, Repeat(.47f, t.Length));
                Curve(clip, "Rig/" + side + "Leg/Leg", "m_LocalScale.x", t, Repeat(.38f, t.Length));
                Curve(clip, "Rig/" + side + "Leg/Leg", "m_LocalScale.z", t, Repeat(.38f, t.Length));
            }
        }
        static float[] Times(float length) => new[] { 0f, length * .15f, length * .30f, length * .48f, length * .76f, length };
        static float[] Repeat(float value, int count) { var a = new float[count]; for (int i = 0; i < count; i++) a[i] = value; return a; }
        static AnimationClip Clip(string name)
        {
            var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(Root + "Art/Animations/" + name + ".anim");
            if (clip == null) { clip = new AnimationClip(); AssetDatabase.CreateAsset(clip, Root + "Art/Animations/" + name + ".anim"); }
            clip.name = name; clip.frameRate = 60; return clip;
        }
        static void State(AnimatorController c, AnimationClip clip)
        {
            AnimatorState state = null;
            foreach (var s in c.layers[0].stateMachine.states) if (s.state.name == clip.name) state = s.state;
            if (state == null) state = c.layers[0].stateMachine.AddState(clip.name);
            state.motion = clip; state.writeDefaultValues = true; EditorUtility.SetDirty(c);
        }
        static void Position(AnimationClip c, string path, float[] t, float[] x, float[] y, float[] z)
        { Curve(c, path, "m_LocalPosition.x", t, x); Curve(c, path, "m_LocalPosition.y", t, y); Curve(c, path, "m_LocalPosition.z", t, z); }
        static void Curve(AnimationClip c, string path, string property, float[] t, float[] values)
        {
            var keys = new Keyframe[t.Length]; for (int i = 0; i < t.Length; i++) keys[i] = new Keyframe(t[i], values[i]);
            AnimationUtility.SetEditorCurve(c, EditorCurveBinding.FloatCurve(path, typeof(Transform), property), new AnimationCurve(keys));
        }
    }
}
