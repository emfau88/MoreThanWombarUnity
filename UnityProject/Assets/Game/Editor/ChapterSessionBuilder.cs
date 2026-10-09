using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace WombatLab.Editor
{
    public static class ChapterSessionBuilder
    {
        [MenuItem("Wombat Lab/B8 Apply Chapter Session")]
        public static string Apply()
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling || EditorUtility.scriptCompilationFailed)
                throw new InvalidOperationException("Use idle compiled Editor.");
            var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            if (scene.path != JunkyardChapterBuilder.ScenePath || scene.isDirty)
                throw new InvalidOperationException("Open the saved JunkyardChapter first.");
            var chapter = Object.FindAnyObjectByType<JunkyardChapter>();
            var session = chapter.GetComponent<ChapterSession>() ?? chapter.gameObject.AddComponent<ChapterSession>();
            session.chapter = chapter; session.hud = Object.FindAnyObjectByType<LabHud>(); chapter.session = session;
            var header = session.hud.transform.Find("Header");
            foreach (var label in header.GetComponentsInChildren<Text>())
            { if (label != session.hud.stateText) { label.text = "SCHROTTHOF / KAPITEL 01"; label.fontSize = 22; } }
            session.hud.stateText.fontSize = 18;
            EditorSceneManager.MarkSceneDirty(scene); EditorSceneManager.SaveScene(scene);
            return "B8 session saved: chapter entry menu, pause, introduction, options and result.";
        }
    }
}
