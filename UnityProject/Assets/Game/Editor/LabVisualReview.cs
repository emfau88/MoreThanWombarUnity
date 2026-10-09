using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace WombatLab.Editor
{
    // Captures actual play-mode poses from the lab camera; no fabricated gameplay.
    public static class LabVisualReview
    {
        static double started;
        static int shot;
        static string prefix;
        static EngagementCoordinator encounter;
        static PlayerMotor player;
        static CombatController combat;
        public static string Start(string name)
        {
            if (!EditorApplication.isPlaying) return "Start Play first";
            Cancel(); prefix = name;
            encounter = Object.FindAnyObjectByType<EngagementCoordinator>(); player = encounter.player;
            combat = player.GetComponent<CombatController>();
            encounter.SetMode(2);
            foreach (var e in encounter.enemies) e.enabled = false;
            player.SetTestInput(Vector2.right); started = EditorApplication.timeSinceStartup;
            shot = 0; EditorApplication.update += Tick;
            return "Visual sequence started: walk, kick, jump hit, KO; then reset and restore real input.";
        }
        static void Place(float distance)
        {
            var cc = player.GetComponent<CharacterController>(); cc.enabled = false;
            player.transform.position = encounter.enemies[0].transform.position + Vector3.back * distance + Vector3.up * .03f;
            cc.enabled = true; player.visual.rotation = Quaternion.identity; player.SetTestInput(Vector2.zero);
        }
        static void Tick()
        {
            if (!EditorApplication.isPlaying || player == null) { Cancel(); return; }
            double t = EditorApplication.timeSinceStartup - started;
            if (shot == 0 && t > .35) { Capture(prefix + "-walk"); Place(1.8f); shot++; }
            else if (shot == 1 && t > .55) { combat.Queue(CombatIntent.Kick); shot++; }
            else if (shot == 2 && t > .79) { Capture(prefix + "-kick"); shot++; }
            else if (shot == 3 && t > 1.45) { encounter.ResetEncounter(); Place(1.4f); player.SetTestInput(Vector2.zero, true); shot++; }
            else if (shot == 4 && t > 1.69) { combat.Queue(CombatIntent.Light); shot++; }
            else if (shot == 5 && t > 1.85) { Capture(prefix + "-jump"); shot++; }
            else if (shot == 6 && t > 2.55)
            {
                var enemy = encounter.enemies[0];
                for (int i = 0; i < 3; i++) enemy.target.ReceiveHit(combat.heavy, Vector3.zero);
                shot++;
            }
            else if (shot == 7 && t > 3.15) { Capture(prefix + "-down"); shot++; }
            else if (shot == 8 && t > 3.45) Cancel();
        }
        public static void Cancel()
        {
            EditorApplication.update -= Tick;
            if (encounter != null && EditorApplication.isPlaying)
            {
                foreach (var e in encounter.enemies) e.enabled = true;
                encounter.SetMode(1); player.ReleaseTestInput();
            }
        }
        public static string Capture(string name)
        {
            string folder = Path.Combine(Application.dataPath, "QA"); Directory.CreateDirectory(folder);
            var camera = Camera.main;
            var previous = camera.targetTexture; var active = RenderTexture.active;
            var rt = new RenderTexture(1280, 720, 24); var tex = new Texture2D(1280, 720, TextureFormat.RGB24, false);
            try
            {
                camera.targetTexture = rt; camera.Render(); RenderTexture.active = rt;
                tex.ReadPixels(new Rect(0, 0, 1280, 720), 0, 0); tex.Apply();
                string path = Path.Combine(folder, name + ".png"); File.WriteAllBytes(path, tex.EncodeToPNG()); return path;
            }
            finally
            {
                camera.targetTexture = previous; RenderTexture.active = active;
                Object.DestroyImmediate(tex); rt.Release(); Object.DestroyImmediate(rt);
            }
        }
        public static string CaptureHud(string name)
        {
            // The Pipeline screen source can contain stale GameView UI. Render current
            // canvases once through the play camera and restore all runtime settings.
            var camera = Camera.main;
            var canvases = Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None);
            var modes = new RenderMode[canvases.Length]; var cameras = new Camera[canvases.Length]; var distances = new float[canvases.Length];
            var previous = camera.targetTexture; var active = RenderTexture.active;
            var rt = new RenderTexture(1280, 600, 24); var tex = new Texture2D(1280, 600, TextureFormat.RGB24, false);
            try
            {
                camera.targetTexture = rt;
                for (int i = 0; i < canvases.Length; i++)
                {
                    modes[i] = canvases[i].renderMode; cameras[i] = canvases[i].worldCamera; distances[i] = canvases[i].planeDistance;
                    canvases[i].renderMode = RenderMode.ScreenSpaceCamera; canvases[i].worldCamera = camera; canvases[i].planeDistance = 1;
                    foreach (var text in canvases[i].GetComponentsInChildren<UnityEngine.UI.Text>(true)) text.SetAllDirty();
                }
                Canvas.ForceUpdateCanvases(); camera.Render(); RenderTexture.active = rt;
                tex.ReadPixels(new Rect(0, 0, 1280, 600), 0, 0); tex.Apply();
                string folder = Path.Combine(Application.dataPath, "QA"); Directory.CreateDirectory(folder);
                string path = Path.Combine(folder, name + ".png"); File.WriteAllBytes(path, tex.EncodeToPNG()); return path;
            }
            finally
            {
                for (int i = 0; i < canvases.Length; i++)
                { canvases[i].renderMode = modes[i]; canvases[i].worldCamera = cameras[i]; canvases[i].planeDistance = distances[i]; }
                camera.targetTexture = previous; RenderTexture.active = active;
                Object.DestroyImmediate(tex); rt.Release(); Object.DestroyImmediate(rt);
            }
        }
    }
}
