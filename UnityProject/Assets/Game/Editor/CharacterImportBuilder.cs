using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace WombatLab.Editor
{
    // Narrow B2 import proof. B1 scenes/prefab and combat data stay usable.
    public static class CharacterImportBuilder
    {
        const string Root = "Assets/Game/Characters/ImportProbe/";
        public const string ScenePath = "Assets/Game/Scenes/CharacterImportLab.unity";

        public static string ImportMovement()
        {
            const string source = Root + "MovementSource.fbx";
            var importer = (ModelImporter)AssetImporter.GetAtPath(source);
            importer.animationType = ModelImporterAnimationType.Human;
            importer.avatarSetup = ModelImporterAvatarSetup.CreateFromThisModel;
            importer.importAnimation = true;
            importer.materialImportMode = ModelImporterMaterialImportMode.None;
            string[] states = { "Idle", "Walk", "Jump", "Fall", "Land" };
            string[] takes = { "Idle_Loop", "Walk_Loop", "Jump_Start", "Jump_Loop", "Jump_Land" };
            var clips = states.Select((state, index) => {
                var clip = importer.defaultClipAnimations.First(c => c.name == "Armature|" + takes[index]);
                clip.name = state;
                clip.loopTime = state == "Idle" || state == "Walk" || state == "Fall";
                clip.lockRootRotation = clip.lockRootHeightY = clip.lockRootPositionXZ = true;
                clip.heightFromFeet = true;
                return clip;
            }).ToArray();
            importer.clipAnimations = clips;
            importer.SaveAndReimport();
            foreach (string state in states)
            {
                var clip = AssetDatabase.LoadAllAssetsAtPath(source).OfType<AnimationClip>().First(c => c.name == state);
                if (!clip.isHumanMotion) throw new InvalidOperationException("Movement clip is not Humanoid: " + state);
                var existing = AssetDatabase.LoadAssetAtPath<AnimationClip>(Root + state + ".anim");
                if (existing == null) AssetDatabase.CreateAsset(Object.Instantiate(clip), Root + state + ".anim");
                else EditorUtility.CopySerialized(clip, existing);
            }
            AssetDatabase.SaveAssets();
            return "Five Humanoid movement clips imported; originals preserved.";
        }

        [MenuItem("Wombat Lab/B2 Build Character Import Lab")]
        public static string Build()
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling || EditorUtility.scriptCompilationFailed)
                throw new InvalidOperationException("Stop Play and finish compilation first.");
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().isDirty)
                throw new InvalidOperationException("Save the current scene first.");
            var model = AssetDatabase.LoadAssetAtPath<GameObject>(Root + "HumanoidProbe.fbx");
            if (model == null) throw new InvalidOperationException("Import the selected model first.");
            var avatar = model.GetComponent<Animator>()?.avatar;
            if (avatar == null || !avatar.isValid || !avatar.isHuman)
                throw new InvalidOperationException("Selected model needs a valid Humanoid avatar.");
            var controller = Controller();
            var body = Material("Body", "T_Superhero_Male_Ligh.png", Color.white);
            var eyes = Material("Eyes", "T_Eye_Brown.png", Color.white);
            var brows = Material("Brows", null, new Color(.08f, .05f, .04f));
            var player = new GameObject("Humanoid import probe — temporary character");
            try
            {
                var capsule = player.AddComponent<CharacterController>();
                capsule.height = 1.85f; capsule.radius = .30f; capsule.center = new Vector3(0, .95f, 0);
                capsule.skinWidth = .035f; capsule.stepOffset = .18f; capsule.slopeLimit = 45;
                player.AddComponent<LabInput>();
                var facing = new GameObject("Visual").transform;
                facing.SetParent(player.transform, false);
                var imported = (GameObject)PrefabUtility.InstantiatePrefab(model);
                imported.transform.SetParent(facing, false);
                // Keep the motor/facing scale at one. Fit only the model instance.
                var renderers = imported.GetComponentsInChildren<SkinnedMeshRenderer>();
                var bounds = renderers[0].bounds;
                foreach (var r in renderers) bounds.Encapsulate(r.bounds);
                float scale = 1.85f / bounds.size.y;
                imported.transform.localScale = Vector3.one * scale;
                // Retargeted idle sole is ~7 cm above the rest-pose bound on this model.
                imported.transform.localPosition = Vector3.up * (-bounds.min.y * scale - .07f);
                foreach (var r in renderers)
                    r.sharedMaterial = r.name == "Eyes" ? eyes : r.name == "Eyebrows" ? brows : body;
                var animator = imported.GetComponent<Animator>();
                animator.runtimeAnimatorController = controller;
                animator.applyRootMotion = false;
                animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
                var motor = player.AddComponent<PlayerMotor>();
                motor.definition = AssetDatabase.LoadAssetAtPath<CharacterDefinition>("Assets/Game/Data/Wombat.asset");
                motor.visual = facing; motor.animator = animator;
                foreach (var bone in new[] { HumanBodyBones.LeftHand, HumanBodyBones.RightHand, HumanBodyBones.LeftFoot, HumanBodyBones.RightFoot })
                {
                    var anchor = animator.GetBoneTransform(bone);
                    if (anchor == null) throw new InvalidOperationException("Missing avatar contact bone: " + bone);
                    var contact = new GameObject(bone + "Contact").transform;
                    contact.SetParent(anchor, false);
                }
                PrefabUtility.SaveAsPrefabAsset(player, Root + "HumanoidProbe.prefab");
            }
            finally { Object.DestroyImmediate(player); }

            // Save the arena copy FIRST; never write back to CombatLab.
            var scene = EditorSceneManager.OpenScene("Assets/Game/Scenes/CombatLab.unity");
            EditorSceneManager.SaveScene(scene, ScenePath);
            foreach (var old in Object.FindObjectsByType<PlayerMotor>()) Object.DestroyImmediate(old.gameObject);
            foreach (var old in Object.FindObjectsByType<TrainingDummy>()) Object.DestroyImmediate(old.gameObject);
            foreach (var old in Object.FindObjectsByType<CombatFeedback>()) Object.DestroyImmediate(old.gameObject);
            var instance = (GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(Root + "HumanoidProbe.prefab"));
            instance.transform.position = new Vector3(-2.5f, .05f, -.35f);
            instance.GetComponent<PlayerMotor>().visual.localRotation = Quaternion.Euler(0, 135, 0);
            Object.FindAnyObjectByType<ArenaCamera>().target = instance.transform;
            var hud = Object.FindAnyObjectByType<LabHud>();
            hud.player = instance.GetComponent<PlayerMotor>(); hud.dummy = null; hud.encounter = null;
            foreach (var text in hud.GetComponentsInChildren<Text>())
            {
                if (text == hud.healthText) text.text = "B2 IMPORT PROBE — TEMPORARY HUMAN BASIS";
                if (text.transform.parent.name == "Controls")
                { text.text = "WASD / Stick Move   SPACE / A Jump   R / Start Reset   H Debug   Combat remains in SparringLab"; text.fontSize = 14; }
            }
            EditorSceneManager.MarkSceneDirty(scene); EditorSceneManager.SaveScene(scene);
            AssetDatabase.SaveAssets(); Selection.activeGameObject = instance;
            return "CharacterImportLab saved: valid imported Humanoid, original motor/input, mapped contacts, no root motion. Wombat art and combat integration pending.";
        }

        static Material Material(string name, string texture, Color color)
        {
            string path = Root + name + ".mat";
            var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat == null)
            { mat = new Material(Shader.Find("Universal Render Pipeline/Lit")); AssetDatabase.CreateAsset(mat, path); }
            mat.SetColor("_BaseColor", color); mat.SetFloat("_Smoothness", .2f);
            if (texture != null)
                mat.SetTexture("_BaseMap", AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/ThirdParty/Quaternius/BaseCharacters/" + texture));
            EditorUtility.SetDirty(mat); return mat;
        }

        static AnimatorController Controller()
        {
            string path = Root + "ImportProbe.controller";
            var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(path);
            if (controller == null) controller = AnimatorController.CreateAnimatorControllerAtPath(path);
            var machine = controller.layers[0].stateMachine;
            foreach (string stateName in new[] { "Idle", "Walk", "Jump", "Fall", "Land" })
            {
                var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(Root + stateName + ".anim");
                if (clip == null) throw new InvalidOperationException("Imported movement clip missing: " + stateName);
                var state = machine.states.Select(s => s.state).FirstOrDefault(s => s.name == stateName) ?? machine.AddState(stateName);
                state.motion = clip; state.writeDefaultValues = false;
                state.speed = stateName == "Jump" ? 3.8f : stateName == "Land" ? 9f : 1f;
                if (stateName == "Idle") machine.defaultState = state;
                EditorUtility.SetDirty(state);
            }
            EditorUtility.SetDirty(controller); return controller;
        }
    }
}
