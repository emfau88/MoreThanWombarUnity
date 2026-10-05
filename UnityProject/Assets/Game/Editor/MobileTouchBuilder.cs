using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace WombatLab.Editor
{
    public static class MobileTouchBuilder
    {
        [MenuItem("Wombat Lab/Apply Mobile Touch")]
        public static void Apply()
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling || EditorUtility.scriptCompilationFailed)
                throw new InvalidOperationException("Stop Play and finish compilation first.");
            var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            if (scene.path != HumanoidCombatBuilder.ScenePath || scene.isDirty)
                throw new InvalidOperationException("Open the saved HumanoidCombatLab first.");
            var player = UnityEngine.Object.FindAnyObjectByType<PlayerMotor>();
            if (player.GetComponent<MobileTouchControls>() == null) Undo.AddComponent<MobileTouchControls>(player.gameObject);
            EditorSceneManager.MarkSceneDirty(scene); EditorSceneManager.SaveScene(scene);
        }
    }
}
