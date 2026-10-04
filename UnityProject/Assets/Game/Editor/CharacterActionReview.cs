using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace WombatLab.Editor
{
    // Actual movement/combat/reaction calls; captures never pose the Animator manually.
    public static class CharacterActionReview
    {
        static EngagementCoordinator encounter;
        static PlayerMotor player;
        static CombatController combat;
        static double started, actionStarted;
        static int step;
        public static string Start()
        {
            if (!EditorApplication.isPlaying || UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "HumanoidCombatLab")
                throw new InvalidOperationException("Play HumanoidCombatLab first.");
            encounter = Object.FindAnyObjectByType<EngagementCoordinator>(); player = encounter.player;
            combat = player.GetComponent<CombatController>();
            encounter.SetMode(1); foreach (var enemy in encounter.enemies) enemy.enabled = false;
            Place(-3); player.SetTestInput(Vector2.right, run: true);
            step = 0; started = EditorApplication.timeSinceStartup;
            EditorApplication.update -= Tick; EditorApplication.update += Tick;
            return "Capturing B3a run, heavy, combo finisher, airborne smash, player hit and robot stagger; then restoring input/AI.";
        }
        static void Place(float x = -.5f)
        {
            encounter.ResetEncounter(); player.SetTestInput(Vector2.zero);
            var cc = player.GetComponent<CharacterController>(); cc.enabled = false;
            player.transform.position = new Vector3(x, .03f, 0); cc.enabled = true;
            player.visual.rotation = Quaternion.LookRotation(Vector3.right);
            encounter.enemies[0].transform.position = new Vector3(.5f, 0, 0);
            encounter.enemies[0].visual.rotation = Quaternion.LookRotation(Vector3.left);
            Physics.SyncTransforms();
        }
        static void Tick()
        {
            if (!EditorApplication.isPlaying || player == null) { EditorApplication.update -= Tick; return; }
            double time = EditorApplication.timeSinceStartup - started;
            if (step == 0 && time > .26) { LabVisualReview.Capture("b3a-run"); Place(); combat.Queue(CombatIntent.Heavy); step++; }
            else if (step == 1 && combat.Attacking && combat.Progress >= combat.Attack.ActiveStart)
            { LabVisualReview.Capture("b3a-heavy"); step++; }
            else if (step == 2 && time > 1.6) { Place(); combat.Queue(CombatIntent.Light); step++; }
            else if (step == 3 && combat.Attack == combat.lights[0] && combat.Progress > .36f)
            { combat.Queue(CombatIntent.Light); step++; }
            else if (step == 4 && combat.Attack == combat.lights[1] && combat.Progress > .36f)
            { combat.Queue(CombatIntent.Light); step++; }
            else if (step == 5 && combat.Attack == combat.lights[2] && combat.Progress >= combat.Attack.ActiveStart)
            { LabVisualReview.Capture("b3a-finisher"); step++; }
            else if (step == 6 && time > 3.6)
            { Place(); player.SetTestInput(Vector2.zero, true); actionStarted = EditorApplication.timeSinceStartup; step++; }
            else if (step == 7 && EditorApplication.timeSinceStartup - actionStarted > .1)
            { combat.Queue(CombatIntent.Heavy); step++; }
            else if (step == 8 && combat.Attacking && combat.Progress >= .12f)
            { LabVisualReview.Capture("b3a-air-windup"); step++; }
            else if (step == 9 && combat.Attacking && combat.Progress >= (combat.Attack.ActiveStart + combat.Attack.ActiveEnd) / 2)
            { LabVisualReview.Capture("b3a-air-contact"); step++; }
            else if (step == 10 && time > 5.0)
            { Place(); player.GetComponent<PlayerDefense>().ReceiveDamage(12, Vector3.left); actionStarted = EditorApplication.timeSinceStartup; step++; }
            else if (step == 11 && EditorApplication.timeSinceStartup - actionStarted > .08)
            { LabVisualReview.Capture("b3a-player-hit"); step++; }
            else if (step == 12 && time > 5.6)
            { Place(); encounter.enemies[0].target.ReceiveHit(combat.heavy, Vector3.zero); actionStarted = EditorApplication.timeSinceStartup; step++; }
            else if (step == 13 && EditorApplication.timeSinceStartup - actionStarted > .1)
            { LabVisualReview.Capture("b3a-robot-stagger"); step++; }
            else if (step == 14 && time > 6.2 || time > 9)
            {
                encounter.ResetEncounter(); player.ReleaseTestInput();
                foreach (var enemy in encounter.enemies) enemy.enabled = true;
                EditorApplication.update -= Tick;
            }
        }
    }
}
