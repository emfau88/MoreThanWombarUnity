using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace WombatLab.Editor
{
    public static class Lf2CombatReview
    {
        static EngagementCoordinator encounter;
        static PlayerMotor player;
        static CombatController combat;
        static double start;
        static int step;
        public static string Start()
        {
            if (!EditorApplication.isPlaying) throw new InvalidOperationException("Start CrowdCombatLab Play first.");
            EditorApplication.update -= Tick;
            encounter = Object.FindAnyObjectByType<EngagementCoordinator>(); player = encounter.player; combat = player.GetComponent<CombatController>();
            encounter.SetMode(8); foreach (var enemy in encounter.enemies) enemy.enabled = false;
            var controller = player.GetComponent<CharacterController>(); controller.enabled = false; player.transform.position = new Vector3(0,.03f,0); controller.enabled = true;
            player.visual.rotation = Quaternion.LookRotation(Vector3.right); player.SetTestInput(Vector2.zero);
            for (int i = 0; i < encounter.enemies.Length; i++) encounter.enemies[i].transform.position = new Vector3(4.3f,0,-1.5f + i * .35f);
            for (int i = 0; i < 3; i++) encounter.enemies[i].transform.position = new Vector3(i - 1,0,1.2f);
            player.GetComponent<MobileTouchControls>().SetVisible(true);
            start = EditorApplication.timeSinceStartup; step = 0; EditorApplication.update += Tick;
            return "Capturing actual touch, landing AOE, pressure wave and eight-enemy gameplay.";
        }
        static void Tick()
        {
            if (!EditorApplication.isPlaying || player == null) { EditorApplication.update -= Tick; return; }
            double elapsed = EditorApplication.timeSinceStartup - start;
            if (step == 0 && elapsed > .3) { LabVisualReview.CaptureHud("b9-b11-touch"); player.SetTestInput(Vector2.zero, true); step++; }
            else if (step == 1 && elapsed > .5) { combat.Queue(CombatIntent.Heavy); step++; }
            else if (step == 2 && combat.LastGroupHits > 1) { LabVisualReview.CaptureHud("b9-smash-impact"); step++; }
            else if (step == 3 && !combat.Attacking) { combat.Queue(CombatIntent.Wave); step++; }
            else if (step == 4 && CombatProjectile.ActiveCount > 0) { LabVisualReview.CaptureHud("b10-pressure-wave"); step++; }
            else if (step == 5 && elapsed > 3)
            { encounter.SetMode(8); foreach (var enemy in encounter.enemies) enemy.enabled = true; player.ReleaseTestInput(); step++; }
            else if (step == 6 && elapsed > 4.1)
            { LabVisualReview.CaptureHud("b11-crowd"); EditorApplication.update -= Tick; }
            if (elapsed > 12) { EditorApplication.update -= Tick; Debug.LogWarning("Review timed out at step " + step); }
        }
    }
}
