using System;
using System.Collections.Generic;
using UnityEngine;

namespace WombatLab
{
    // Opt-in local measurement only: add ?crowdprofile=1 (or =10) to the crowd build URL.
    // Uses real AI/animation/effects; inflated temporary HP keeps the requested workload alive.
    public sealed class CrowdPerformanceProbe : MonoBehaviour
    {
        readonly List<float> frames = new List<float>(2400);
        EngagementCoordinator encounter;
        PlayerMotor player;
        CombatController combat;
        float elapsed, nextAction;
        int count, action;
        string report;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void EnableRequestedProbe()
        {
            if (!Application.absoluteURL.Contains("crowdprofile=")) return;
            var encounter = FindAnyObjectByType<EngagementCoordinator>();
            if (encounter == null || encounter.chapter != null) return;
            encounter.gameObject.AddComponent<CrowdPerformanceProbe>();
        }
        void Start()
        {
            encounter = GetComponent<EngagementCoordinator>(); count = Application.absoluteURL.Contains("crowdprofile=10") ? 10 : 8;
            encounter.SetMode(count); player = encounter.player; combat = player.GetComponent<CombatController>();
            foreach (var e in encounter.enemies) { e.target.maxHealth = 5000; e.target.ResetTraining(); }
        }
        void Update()
        {
            if (report != null) return;
            elapsed += Time.unscaledDeltaTime;
            player.GetComponent<PlayerDefense>().RestoreHealth(100); combat.RestoreEnergy(100);
            player.SetTestInput(new Vector2(Mathf.Sin(elapsed * 1.6f), Mathf.Cos(elapsed * 1.6f)) * .4f);
            if (elapsed >= nextAction && !combat.Attacking)
            {
                nextAction = elapsed + .8f; action++;
                if (action % 3 == 0) { player.SetTestInput(Vector2.zero, true); }
                else combat.Queue(action % 3 == 1 ? CombatIntent.Wave : CombatIntent.Charge);
            }
            if (!player.Grounded && !combat.Attacking) combat.Queue(CombatIntent.Heavy);
            if (elapsed > 3) frames.Add(Time.unscaledDeltaTime * 1000);
            if (elapsed < 18) return;
            frames.Sort();
            var result = new Result { enemies = count, living = encounter.LivingCount, samples = frames.Count, medianMs = frames[frames.Count / 2], p95Ms = frames[Mathf.Min(frames.Count - 1, Mathf.FloorToInt(frames.Count * .95f))],
                memoryMB = UnityEngine.Profiling.Profiler.GetTotalAllocatedMemoryLong() / 1048576f, gpu = SystemInfo.graphicsDeviceName, platform = Application.platform.ToString(),
                width = Screen.width, height = Screen.height };
            report = JsonUtility.ToJson(result, true); Debug.Log("CROWD_PROFILE " + report);
            player.ReleaseTestInput(); player.GetComponent<LabInput>().enabled = false; Time.timeScale = 0;
        }
        void OnGUI()
        {
            var style = new GUIStyle(GUI.skin.box) { fontSize = 20, alignment = TextAnchor.UpperLeft };
            GUI.Box(new Rect(18, 160, 630, 325), report ?? "Lokaler Gruppenkampf-Probelauf: " + count + " Gegner\n" + elapsed.ToString("0.0") + " / 18 s (temporär erhöhte HP)", style);
        }
        void OnDestroy() { if (report != null) Time.timeScale = 1; }
        [Serializable] sealed class Result
        { public int enemies, living, samples, width, height; public float medianMs, p95Ms, memoryMB; public string gpu, platform; }
    }
}
