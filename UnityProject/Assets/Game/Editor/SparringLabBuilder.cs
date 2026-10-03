using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace WombatLab.Editor
{
    public static class SparringLabBuilder
    {
        const string Root = "Assets/Game/";
        [MenuItem("Wombat Lab/Build S3 Sparring Lab")]
        public static string Build()
        {
            if (EditorApplication.isPlaying || EditorUtility.scriptCompilationFailed) throw new InvalidOperationException("Stop Play first.");
            if (!Application.dataPath.Replace('\\', '/').EndsWith("MoreThanWombarUnity/UnityProject/Assets")) throw new InvalidOperationException("Isolated lab only.");
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().isDirty) throw new InvalidOperationException("Save the current scene first.");
            // Save as a separate scene: S2's stationary training target remains available.
            var scene = EditorSceneManager.OpenScene(Root + "Scenes/CombatLab.unity");
            var player = Object.FindAnyObjectByType<PlayerMotor>();
            var combat = player.GetComponent<CombatController>();
            player.gameObject.AddComponent<PlayerDefense>();
            var dummy = Object.FindAnyObjectByType<TrainingDummy>(); Object.DestroyImmediate(dummy.gameObject); combat.dummy = null;
            var encounter = new GameObject("S3 — one attack token, 1 / 2 opponents").AddComponent<EngagementCoordinator>();
            encounter.player = player; encounter.enemies = new EnemyBrain[2];
            var steel = AssetDatabase.LoadAssetAtPath<Material>(Root + "Art/Materials/Steel.mat");
            var safety = AssetDatabase.LoadAssetAtPath<Material>(Root + "Art/Materials/Safety.mat");
            var warningMaterial = AssetDatabase.LoadAssetAtPath<Material>(Root + "Art/Materials/Contact.mat");
            var heavy = AssetDatabase.LoadAssetAtPath<AttackDefinition>(Root + "Data/Heavy_Smash.asset");
            for (int i = 0; i < 2; i++)
            {
                var go = new GameObject("Sparring Robot " + (i + 1)); go.transform.position = new Vector3(i == 0 ? 2.7f : 4.6f, 0, i == 0 ? -.35f : 1.3f);
                var visual = Object.Instantiate(player.visual.gameObject, go.transform).transform; visual.name = "Visual";
                visual.localPosition = Vector3.zero; visual.localRotation = Quaternion.Euler(0, -90, 0);
                foreach (var renderer in visual.GetComponentsInChildren<Renderer>()) renderer.sharedMaterial = renderer.name == "Fist" ? safety : steel;
                var target = go.AddComponent<TrainingDummy>(); target.maxHealth = 80; target.clock = combat;
                target.pad = visual.Find("Rig/Torso"); target.padRenderer = target.pad.Find("Body").GetComponent<Renderer>();
                var body = go.AddComponent<CapsuleCollider>(); body.center = new Vector3(0, .85f, 0); body.height = 1.7f; body.radius = .30f;
                var hurt = new GameObject("Combat Hurtbox"); hurt.layer = 8; hurt.transform.SetParent(go.transform, false);
                var collider = hurt.AddComponent<CapsuleCollider>(); collider.center = new Vector3(0, 1.14f, 0); collider.height = 1.22f; collider.radius = .37f; collider.isTrigger = true;
                var warning = GameObject.CreatePrimitive(PrimitiveType.Cylinder); warning.name = "Telegraph disc"; warning.transform.SetParent(go.transform, false);
                warning.transform.localPosition = Vector3.up * .025f; warning.transform.localScale = new Vector3(1.65f, .009f, 1.65f);
                Object.DestroyImmediate(warning.GetComponent<Collider>()); var rendererWarning = warning.GetComponent<Renderer>(); rendererWarning.sharedMaterial = warningMaterial; rendererWarning.enabled = false;
                var brain = go.AddComponent<EnemyBrain>(); brain.target = target; brain.coordinator = encounter; brain.player = player;
                brain.visual = visual; brain.animator = visual.GetComponent<Animator>(); brain.fist = visual.Find("Rig/RightArm/Fist");
                brain.attack = heavy; brain.warning = rendererWarning; brain.slot = i; encounter.enemies[i] = brain;
            }
            var oldHud = Object.FindAnyObjectByType<LabHud>(); Object.DestroyImmediate(oldHud.gameObject);
            LabSceneBuilder.Hud(player, encounter.enemies[0].target);
            var hud = Object.FindAnyObjectByType<LabHud>(); hud.encounter = encounter;
            foreach (var label in hud.GetComponentsInChildren<Text>())
            {
                if (label.transform.parent.name == "Header" && label != hud.stateText) label.text = "MORE THAN WOMBAT   /   S3 SPARRING LAB";
                if (label.transform.parent.name == "Controls") { label.text = "WASD Move   SPACE Jump   J / K Attack   SHIFT Evade   1 / 2 Opponents   R Reset"; label.fontSize = 16; }
            }
            PrefabUtility.RecordPrefabInstancePropertyModifications(combat);
            EditorSceneManager.SaveScene(scene, Root + "Scenes/SparringLab.unity");
            EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(Root + "Scenes/SparringLab.unity", true), new EditorBuildSettingsScene(Root + "Scenes/CombatLab.unity", true) };
            AssetDatabase.SaveAssets(); Selection.activeGameObject = player.gameObject;
            return "S3 saved separately: air attacks, evade, telegraphed robot opponent, 1/2 mode and shared attack token.";
        }
    }
}
