using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace WombatLab.Editor
{
    public static class CharacterImportReview
    {
        static PlayerMotor player;
        static double started;
        static int step;
        static Vector3 spawn, walkStart;
        static Vector3 initialFoot;
        static float distance, rise, footMotion, facingAngle, idleMinY;
        static bool jumpWasAirborne, landed;
        public static string Start()
        {
            if (!EditorApplication.isPlaying || UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "CharacterImportLab")
                throw new InvalidOperationException("Play CharacterImportLab first.");
            EditorApplication.update -= Tick;
            player = Object.FindAnyObjectByType<PlayerMotor>();
            player.ResetToSpawn(); player.SetTestInput(Vector2.zero);
            spawn = player.transform.position;
            distance = rise = footMotion = facingAngle = 0; jumpWasAirborne = landed = false;
            started = EditorApplication.timeSinceStartup; step = 0;
            EditorApplication.update += Tick;
            return "Imported character: idle, walk/turn, jump/land and reset; then restore real input.";
        }
        static void Tick()
        {
            if (!EditorApplication.isPlaying || player == null)
            { EditorApplication.update -= Tick; if (player != null) player.ReleaseTestInput(); return; }
            double elapsed = EditorApplication.timeSinceStartup - started;
            if (step == 1)
                footMotion = Mathf.Max(footMotion, Vector3.Distance(initialFoot,
                    player.animator.transform.InverseTransformPoint(player.animator.GetBoneTransform(HumanBodyBones.LeftFoot).position)));
            if (step == 0 && elapsed > .2)
            {
                LabVisualReview.Capture("b2-import-idle");
                idleMinY = float.MaxValue;
                foreach (var renderer in player.GetComponentsInChildren<SkinnedMeshRenderer>())
                {
                    var baked = new Mesh(); renderer.BakeMesh(baked);
                    foreach (var vertex in baked.vertices)
                        idleMinY = Mathf.Min(idleMinY, renderer.transform.TransformPoint(vertex).y);
                    Object.DestroyImmediate(baked);
                }
                walkStart = player.transform.position;
                initialFoot = player.animator.transform.InverseTransformPoint(player.animator.GetBoneTransform(HumanBodyBones.LeftFoot).position);
                player.SetTestInput(Vector2.right); step++;
            }
            else if (step == 1 && elapsed > .55)
            {
                distance = Vector3.Distance(player.transform.position, walkStart);
                facingAngle = Vector3.Angle(player.visual.forward, Vector3.right);
                LabVisualReview.Capture("b2-import-walk"); player.SetTestInput(Vector2.zero, true); step++;
            }
            else if (step == 2 && elapsed > .78)
            {
                rise = player.transform.position.y - spawn.y; jumpWasAirborne = !player.Grounded;
                LabVisualReview.Capture("b2-import-jump"); step++;
            }
            else if (step == 3 && elapsed > 1.7)
            {
                landed = player.Grounded; player.ResetToSpawn(); step++;
            }
            else if (step == 4 && elapsed > 1.9)
            {
                var avatar = player.animator.avatar;
                bool reset = Vector3.ProjectOnPlane(player.transform.position - spawn, Vector3.up).magnitude < .01f;
                bool passed = avatar.isValid && avatar.isHuman && distance > .5f && footMotion > .03f
                    && facingAngle < 15 && rise > .5f && jumpWasAirborne && landed && reset && Mathf.Abs(idleMinY) < .15f;
                string result = JsonUtility.ToJson(new Result {
                    passed = passed, validHumanoid = avatar.isValid && avatar.isHuman, walkDistance = distance,
                    footMotion = footMotion, facingErrorDegrees = facingAngle, jumpRise = rise,
                    jumpWasAirborne = jumpWasAirborne, landed = landed, reset = reset,
                    idleLowestVertexY = idleMinY, rootMotion = player.animator.applyRootMotion
                }, true);
                Directory.CreateDirectory(Path.Combine(Application.dataPath, "QA"));
                File.WriteAllText(Path.Combine(Application.dataPath, "QA/b2-import-result.json"), result);
                player.ReleaseTestInput(); EditorApplication.update -= Tick;
                if (!passed) Debug.LogError("B2 import review failed: " + result);
                else Debug.Log("B2 import review passed: " + result);
            }
        }
        [Serializable] sealed class Result
        {
            public bool passed, validHumanoid, jumpWasAirborne, landed, reset, rootMotion;
            public float walkDistance, footMotion, facingErrorDegrees, jumpRise, idleLowestVertexY;
        }
    }
}
