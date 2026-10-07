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
    public static class ShoulderChargeBuilder
    {
        const string Root = HumanoidCombatBuilder.Root;
        [MenuItem("Wombat Lab/B4 Apply Shoulder Charge")]
        public static string Apply()
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling || EditorUtility.scriptCompilationFailed)
                throw new InvalidOperationException("Stop Play and finish compilation first.");
            var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            if (scene.path != HumanoidCombatBuilder.ScenePath || scene.isDirty)
                throw new InvalidOperationException("Open the saved HumanoidCombatLab first.");
            var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(Root + "HumanoidCombat.controller");
            var prefab = PrefabUtility.LoadPrefabContents(Root + "HumanoidCombat.prefab");
            try
            {
                var motor = prefab.GetComponent<PlayerMotor>(); var combat = prefab.GetComponent<CombatController>();
                var clip = CreateShoulderPose();
                combat.shoulder = motor.animator.GetBoneTransform(HumanBodyBones.RightUpperArm);
                var attack = HumanoidCombatBuilder.Attack(motor, controller, "Shoulder_Charge", "Heavy_Smash", clip,
                    (true, .38f), .16f, .20f, .40f, HumanBodyBones.RightUpperArm);
                attack.heavy = false; attack.knocksDown = false; attack.airborne = attack.foot = false;
                attack.forwardStep = 0; attack.chargeDistance = 2.0f; attack.moveRelease = 1;
                attack.damage = 16; attack.radius = .36f; attack.knockback = .35f;
                attack.hitstun = .28f; attack.hitstop = .065f;
                EditorUtility.SetDirty(attack); combat.shoulderCharge = attack;
                // Kick makes space; charge closes it. Keep the proven light chain and heavy timing.
                combat.kick.knockback = 1.05f; EditorUtility.SetDirty(combat.kick);
                PrefabUtility.SaveAsPrefabAsset(prefab, Root + "HumanoidCombat.prefab");
            }
            finally { PrefabUtility.UnloadPrefabContents(prefab); }
            var hud = Object.FindAnyObjectByType<LabHud>();
            foreach (var label in hud.GetComponentsInChildren<Text>(true))
                if (label.transform.parent.name == "Controls")
                    label.text = "WASD Gehen  CTRL Rennen  SPACE Sprung  J Combo  K Heavy  L Kick  E Stoß  SHIFT Ausweichen  1/2 Gegner  R Neustart";
            EditorSceneManager.MarkSceneDirty(scene); EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets();
            return "B4 shoulder charge: E/RT/Touch STOSS, 2m, .16/.20/.40s, 16 damage, stops on contact/wall; kick knockback 1.05.";
        }

        static AnimationClip CreateShoulderPose()
        {
            // Retain the imported sprint's leg rhythm; change only the shoulder/guard/body lean.
            var clip = Object.Instantiate(AssetDatabase.LoadAssetAtPath<AnimationClip>(Root + "Source_Run.anim"));
            float length = clip.length;
            float[] times = new[] { 0f, .12f, .28f, .48f, .65f, .82f, 1f }.Select(t => t * length).ToArray();
            HumanoidCombatBuilder.Muscle(clip, "Spine Front-Back", times, new[] { .1f, .28f, .45f, .45f, .35f, .15f, 0f });
            HumanoidCombatBuilder.Muscle(clip, "Chest Front-Back", times, new[] { 0f, .18f, .3f, .3f, .2f, .1f, 0f });
            HumanoidCombatBuilder.Muscle(clip, "Chest Twist Left-Right", times, new[] { 0f, -.28f, -.45f, -.45f, -.3f, -.12f, 0f });
            HumanoidCombatBuilder.Muscle(clip, "Right Shoulder Front-Back", times, new[] { 0f, -.2f, -.55f, -.55f, -.4f, -.15f, 0f });
            foreach (string side in new[] { "Left", "Right" })
            {
                HumanoidCombatBuilder.Muscle(clip, side + " Arm Down-Up", times, side == "Right"
                    ? new[] { -.35f, -.6f, -.85f, -.85f, -.65f, -.45f, -.35f }
                    : new[] { -.3f, -.15f, -.1f, -.1f, -.2f, -.3f, -.3f });
                HumanoidCombatBuilder.Muscle(clip, side + " Arm Front-Back", times, new[] { -.3f, -.45f, -.6f, -.6f, -.5f, -.35f, -.3f });
                HumanoidCombatBuilder.Muscle(clip, side + " Forearm Stretch", times, new[] { -.2f, -.65f, -.9f, -.9f, -.75f, -.4f, -.2f });
            }
            var settings = AnimationUtility.GetAnimationClipSettings(clip); settings.loopTime = false;
            AnimationUtility.SetAnimationClipSettings(clip, settings);
            return HumanoidCombatBuilder.SaveClip(clip, "Source_ShoulderCharge");
        }
    }
}
