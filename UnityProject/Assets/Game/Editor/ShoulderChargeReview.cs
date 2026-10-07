using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace WombatLab.Editor
{
    public static class ShoulderChargeReview
    {
        static EngagementCoordinator encounter;
        static PlayerMotor player;
        static CombatController combat;
        static double started;
        static int stage;
        public static string Start()
        {
            if (!EditorApplication.isPlaying) throw new InvalidOperationException("Play HumanoidCombatLab first.");
            encounter = Object.FindAnyObjectByType<EngagementCoordinator>(); player = encounter.player;
            combat = player.GetComponent<CombatController>();
            encounter.SetMode(1); foreach (var enemy in encounter.enemies) enemy.enabled = false;
            Place(2.3f); combat.Queue(CombatIntent.Charge);
            started = EditorApplication.timeSinceStartup; stage = 0;
            EditorApplication.update -= Tick; EditorApplication.update += Tick;
            return "Capturing actual B4 windup, contact and miss; restoring encounter afterwards.";
        }
        static void Place(float distance)
        {
            encounter.ResetEncounter(); player.SetTestInput(Vector2.zero);
            var cc = player.GetComponent<CharacterController>(); cc.enabled = false;
            player.transform.position = new Vector3(-1.8f, .03f, 0); cc.enabled = true;
            player.visual.rotation = Quaternion.LookRotation(Vector3.right);
            encounter.enemies[0].transform.position = new Vector3(-1.8f + distance, 0, 0);
            encounter.enemies[0].visual.rotation = Quaternion.LookRotation(Vector3.left);
            Physics.SyncTransforms();
        }
        static void Tick()
        {
            if (!EditorApplication.isPlaying || player == null) { EditorApplication.update -= Tick; return; }
            try
            {
                double elapsed = EditorApplication.timeSinceStartup - started;
                if (stage == 0 && combat.ChargeCommitted && combat.Progress > .08f)
                { LabVisualReview.Capture("b4-charge-windup"); stage++; }
                else if (stage == 1 && combat.ChargeStopped)
                { LabVisualReview.Capture("b4-charge-contact"); stage++; }
                else if (stage == 2 && elapsed > 1.4)
                { Place(5); combat.Queue(CombatIntent.Charge); stage++; }
                else if (stage == 3 && combat.ChargeCommitted && combat.Progress > combat.Attack.ActiveEnd)
                { LabVisualReview.Capture("b4-charge-miss-recovery"); stage++; }
                else if (stage == 4 && !combat.Attacking || elapsed > 5) Stop();
            }
            catch { Stop(); throw; }
        }
        static void Stop()
        {
            EditorApplication.update -= Tick;
            encounter.ResetEncounter(); player.ReleaseTestInput();
            foreach (var enemy in encounter.enemies) enemy.enabled = true;
        }
    }
}
