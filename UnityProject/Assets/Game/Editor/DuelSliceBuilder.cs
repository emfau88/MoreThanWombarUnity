using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace WombatLab.Editor
{
    [InitializeOnLoad]
    public static class DuelSliceBuilder
    {
        const string Pending = "WombatLab.DuelBuild";
        const string ChapterBuild = "WombatLab.ChapterBuild";
        static string Output => Path.GetFullPath(Path.Combine(Application.dataPath, SessionState.GetBool(ChapterBuild, false) ? "../../Builds/B8" : "../../Builds/B3c"));
        static DuelSliceBuilder() { EditorApplication.update += ResumeBuild; }

        [MenuItem("Wombat Lab/B3c Apply Duel Tuning")]
        public static string Apply()
        {
            if (EditorApplication.isPlaying || EditorApplication.isCompiling || EditorUtility.scriptCompilationFailed)
                throw new InvalidOperationException("Stop Play and finish compilation first.");
            var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
            if (scene.path != HumanoidCombatBuilder.ScenePath || scene.isDirty)
                throw new InvalidOperationException("Open the saved HumanoidCombatLab first.");
            var encounter = Object.FindAnyObjectByType<EngagementCoordinator>();
            foreach (var enemy in encounter.enemies)
            { enemy.engagementDistance = 1.30f; enemy.preferredDistance = 1.20f; EditorUtility.SetDirty(enemy); }
            var hud = Object.FindAnyObjectByType<LabHud>(); hud.duelPresentation = true;
            foreach (var label in hud.GetComponentsInChildren<Text>())
            {
                if (label.transform.parent.name == "Header" && label != hud.stateText)
                    label.text = "MORE THAN WOMBAT / SCHROTTHOF-DUELL";
                if (label.transform.parent.name == "Controls")
                    label.text = "WASD Gehen   CTRL Rennen   SPACE Sprung   J Combo   K Heavy   L Kick   SHIFT Ausweichen   1/2 Gegner   R Neustart";
            }
            hud.stateText.text = "BÄR 80/80 · Orange Warnung: ausweichen oder unterbrechen!";
            hud.healthText.text = "DU   100 / 100";
            EditorSceneManager.MarkSceneDirty(scene); EditorSceneManager.SaveScene(scene);
            return "Duel distance 1.30/1.20m, German duel HUD saved.";
        }

        [MenuItem("Wombat Lab/B3c Build Windows")]
        public static void Windows() => Start("Windows");
        [MenuItem("Wombat Lab/B3c Build WebGL")]
        public static void WebGL() => Start("WebGL");
        [MenuItem("Wombat Lab/B8 Build Chapter Windows")]
        public static void ChapterWindows() => Start("Windows", true);
        [MenuItem("Wombat Lab/B8 Build Chapter WebGL")]
        public static void ChapterWebGL() => Start("WebGL", true);
        public static string Start(string platform, bool chapterBuild = false)
        {
            if (platform != "Windows" && platform != "WebGL") throw new ArgumentException(platform);
            if (EditorApplication.isPlaying || EditorApplication.isCompiling || EditorUtility.scriptCompilationFailed || BuildPipeline.isBuildingPlayer)
                throw new InvalidOperationException("Editor must be idle and compiled.");
            if (SessionState.GetString(Pending, "") != "") throw new InvalidOperationException("A duel build is already queued.");
            var target = Target(platform); var group = BuildPipeline.GetBuildTargetGroup(target);
            if (!BuildPipeline.IsBuildTargetSupported(group, target)) throw new InvalidOperationException(platform + " module missing.");
            SessionState.SetBool(ChapterBuild, chapterBuild);
            Directory.CreateDirectory(Output);
            Write(platform, new Result { status = "queued", platform = platform });
            SessionState.SetString(Pending, platform);
            if (EditorUserBuildSettings.activeBuildTarget != target && !EditorUserBuildSettings.SwitchActiveBuildTarget(group, target))
            { SessionState.EraseString(Pending); throw new InvalidOperationException("Could not switch build target."); }
            return "Queued " + platform + " build; status in " + Output;
        }
        static BuildTarget Target(string platform) => platform == "Windows" ? BuildTarget.StandaloneWindows64 : BuildTarget.WebGL;
        static void ResumeBuild()
        {
            var platform = SessionState.GetString(Pending, "");
            if (platform == "" || EditorApplication.isCompiling || EditorApplication.isUpdating || EditorApplication.isPlaying || BuildPipeline.isBuildingPlayer) return;
            // Consume only when execution starts; delayCall can be lost during a domain reload.
            SessionState.EraseString(Pending);
            Build(platform);
        }
        static void Build(string platform)
        {
            string product = PlayerSettings.productName;
            var compression = PlayerSettings.WebGL.compressionFormat;
            var fullscreen = PlayerSettings.fullScreenMode;
            string template = PlayerSettings.WebGL.template;
            var started = DateTime.UtcNow;
            var result = new Result { platform = platform, status = "building", utc = started.ToString("O") };
            Write(platform, result);
            try
            {
                if (EditorUtility.scriptCompilationFailed) throw new InvalidOperationException("Script compilation failed.");
                bool chapterBuild = SessionState.GetBool(ChapterBuild, false);
                PlayerSettings.productName = chapterBuild ? "More Than Wombat — Schrotthof" : "More Than Wombat — Duell";
                PlayerSettings.WebGL.compressionFormat = WebGLCompressionFormat.Disabled;
                PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
                PlayerSettings.WebGL.template = "PROJECT:TouchDuel";
                string location = platform == "Windows" ? Path.Combine(Output, platform, "MoreThanWombat.exe") : Path.Combine(Output, platform);
                var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions {
                    scenes = new[] { chapterBuild ? JunkyardChapterBuilder.ScenePath : HumanoidCombatBuilder.ScenePath }, locationPathName = location,
                    target = Target(platform), options = BuildOptions.None });
                result.status = report.summary.result.ToString(); result.bytes = (long)report.summary.totalSize;
                result.errors = report.summary.totalErrors; result.warnings = report.summary.totalWarnings;
                result.messages = report.steps.SelectMany(s => s.messages).Where(m => m.type == LogType.Error || m.type == LogType.Warning)
                    .Select(m => m.content).Take(12).ToArray();
            }
            catch (Exception e) { result.status = "Failed"; result.errors++; result.messages = new[] { e.ToString() }; Debug.LogException(e); }
            finally
            {
                PlayerSettings.productName = product; PlayerSettings.WebGL.compressionFormat = compression;
                PlayerSettings.fullScreenMode = fullscreen;
                PlayerSettings.WebGL.template = template;
                AssetDatabase.SaveAssets();
                result.seconds = (DateTime.UtcNow - started).TotalSeconds; Write(platform, result);
            }
        }
        static void Write(string platform, Result result) => File.WriteAllText(Path.Combine(Output, platform + "BuildStatus.json"), JsonUtility.ToJson(result, true));
        [Serializable] public sealed class Result
        {
            public string platform, status, utc;
            public double seconds;
            public long bytes;
            public int errors, warnings;
            public string[] messages;
        }
    }
}
