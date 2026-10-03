using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace WombatLab.Editor
{
    public static class HumanoidCombatReview
    {
        static EngagementCoordinator encounter;
        static PlayerMotor player;
        static CombatController combat;
        static double started;
        static int step;
        public static string Start()
        {
            if (!EditorApplication.isPlaying || UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "HumanoidCombatLab")
                throw new InvalidOperationException("Play HumanoidCombatLab first.");
            encounter = Object.FindAnyObjectByType<EngagementCoordinator>(); player = encounter.player;
            combat = player.GetComponent<CombatController>();
            encounter.SetMode(1); foreach (var e in encounter.enemies) e.enabled = false;
            player.SetTestInput(Vector2.zero); Place(); step = 0; started = EditorApplication.timeSinceStartup;
            EditorApplication.update -= Tick; EditorApplication.update += Tick;
            return "Actual Humanoid gameplay capture: punch, kick, airborne heavy; then reset and restore input/AI.";
        }
        static void Place()
        {
            encounter.enemies[0].transform.position = new Vector3(.5f, 0, 0);
            var cc = player.GetComponent<CharacterController>(); cc.enabled = false;
            player.transform.position = new Vector3(-.5f, .03f, 0); cc.enabled = true;
            player.visual.rotation = Quaternion.LookRotation(Vector3.right); Physics.SyncTransforms();
        }
        static void Tick()
        {
            if (!EditorApplication.isPlaying || player == null)
            { EditorApplication.update -= Tick; return; }
            double time = EditorApplication.timeSinceStartup - started;
            if (step == 0 && time > .2) { combat.Queue(CombatIntent.Light); step++; }
            else if (step == 1 && combat.Attacking && combat.Progress >= combat.Attack.ActiveStart)
            { LabVisualReview.Capture("b2-combat-punch"); step++; }
            else if (step == 2 && time > 1)
            { encounter.ResetEncounter(); Place(); combat.Queue(CombatIntent.Kick); step++; }
            else if (step == 3 && combat.Attacking && combat.Progress >= (combat.Attack.ActiveStart + combat.Attack.ActiveEnd) / 2)
            { LabVisualReview.Capture("b2-combat-kick"); step++; }
            else if (step == 4 && time > 2)
            { encounter.ResetEncounter(); Place(); player.SetTestInput(Vector2.zero, true); step++; }
            else if (step == 5 && time > 2.2) { combat.Queue(CombatIntent.Heavy); step++; }
            else if (step == 6 && combat.Attacking && combat.Progress > .25f)
            { LabVisualReview.Capture("b2-combat-air"); step++; }
            else if (step == 7 && time > 3.3 || time > 6)
            {
                encounter.ResetEncounter(); player.ReleaseTestInput();
                foreach (var e in encounter.enemies) e.enabled = true;
                EditorApplication.update -= Tick;
            }
        }
    }
}
