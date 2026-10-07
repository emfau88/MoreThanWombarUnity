using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace WombatLab.Tests
{
    public class ShoulderChargePlayTests
    {
        PlayerMotor player;
        CombatController combat;
        PlayerDefense defense;
        TrainingDummy target;

        [UnitySetUp] public IEnumerator Setup()
        {
            SceneManager.LoadScene("HumanoidCombatLab"); yield return null;
            var encounter = Object.FindAnyObjectByType<EngagementCoordinator>();
            player = encounter.player; combat = player.GetComponent<CombatController>();
            defense = player.GetComponent<PlayerDefense>(); target = encounter.enemies[0].target;
            foreach (var enemy in encounter.enemies) enemy.enabled = false;
            Place(2.3f); yield return new WaitForSeconds(.1f);
        }

        void Place(float distance)
        {
            player.SetTestInput(Vector2.zero);
            var cc = player.GetComponent<CharacterController>(); cc.enabled = false;
            player.transform.position = new Vector3(0, .03f, -1); cc.enabled = true;
            player.visual.rotation = Quaternion.identity;
            target.transform.position = new Vector3(0, 0, -1 + distance);
            Physics.SyncTransforms();
        }

        [UnityTest] public IEnumerator ContactStopsChargeAndKeepsVulnerableRecovery()
        {
            var attack = combat.shoulderCharge;
            Assert.That(attack.HasHumanoidContact, Is.True);
            Assert.That(combat.shoulder, Is.SameAs(player.animator.GetBoneTransform(HumanBodyBones.RightUpperArm)));
            Vector3 start = player.transform.position;
            combat.Queue(CombatIntent.Charge); yield return new WaitForSeconds(.08f);
            Assert.That(target.HitCount, Is.Zero);
            Assert.That(player.transform.position.z - start.z, Is.LessThan(.01f));
            Assert.That(defense.Invulnerable, Is.False);
            float deadline = Time.time + 2;
            while (!combat.ChargeStopped && Time.time < deadline) yield return null;
            Assert.That(combat.ChargeStopped, Is.True);
            Assert.That(target.HitCount, Is.EqualTo(1));
            Assert.That(target.Health, Is.EqualTo(target.maxHealth - attack.damage));
            Assert.That(target.GetComponent<BodyRecovery>().Busy, Is.False);
            Assert.That(defense.TryEvade(Vector2.left), Is.False, "The charge's recovery cannot be dodge-cancelled");
            Vector3 contactPosition = player.transform.position;
            player.SetTestInput(Vector2.left, jump: true);
            yield return new WaitForSeconds(.15f);
            Assert.That(combat.Attacking, Is.True);
            Assert.That(Vector3.ProjectOnPlane(player.transform.position - contactPosition, Vector3.up).magnitude, Is.LessThan(.02f));
            Assert.That(player.Grounded, Is.True);
            yield return new WaitForSeconds(.7f);
            Assert.That(target.HitCount, Is.EqualTo(1)); Assert.That(combat.Attacking, Is.False);
        }

        [UnityTest] public IEnumerator MissTravelsLimitedDistanceInLockedDirection()
        {
            Place(5); Vector3 start = player.transform.position;
            combat.Queue(CombatIntent.Charge); yield return new WaitForSeconds(.09f);
            player.SetTestInput(Vector2.left);
            yield return new WaitForSeconds(.4f); player.SetTestInput(Vector2.zero);
            float deadline = Time.time + 2;
            while (combat.Attacking && Time.time < deadline) yield return null;
            // Inspect before the following free-movement frame.
            Assert.That(player.transform.position.z - start.z, Is.EqualTo(combat.shoulderCharge.chargeDistance).Within(.03f));
            Assert.That(Mathf.Abs(player.transform.position.x - start.x), Is.LessThan(.03f));
            Assert.That(target.HitCount, Is.Zero);
            Assert.That(combat.Attacking, Is.False);
        }

        [UnityTest] public IEnumerator WallStopsMotionAndDamageInterruptsCharge()
        {
            Place(5);
            var wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.transform.position = new Vector3(0, 1, -.05f); wall.transform.localScale = new Vector3(3, 2, .15f);
            Physics.SyncTransforms();
            try
            {
                combat.Queue(CombatIntent.Charge); yield return new WaitForSeconds(.4f);
                Assert.That(combat.ChargeStopped, Is.True);
                Assert.That(player.transform.position.z, Is.LessThan(-.15f));
                Assert.That(target.HitCount, Is.Zero);
                Assert.That(combat.Attacking, Is.True, "A wall does not skip recovery");
                yield return new WaitForSeconds(.5f);
            }
            finally { Object.Destroy(wall); }
            yield return null;
            Place(5); combat.Queue(CombatIntent.Charge); yield return new WaitForSeconds(.08f);
            Assert.That(defense.ReceiveDamage(12, Vector3.back), Is.True);
            Assert.That(combat.Attacking, Is.False);
            float z = player.transform.position.z;
            yield return new WaitForSeconds(.5f);
            Assert.That(player.transform.position.z, Is.LessThanOrEqualTo(z + .01f));
            Assert.That(target.HitCount, Is.Zero);
            player.SetTestInput(Vector2.zero, jump: true); yield return new WaitForSeconds(.2f);
            Assert.That(player.Grounded, Is.False);
            combat.Queue(CombatIntent.Charge); yield return null;
            Assert.That(combat.Attacking, Is.False, "Charge must not turn into an airborne attack");
        }

        [UnityTest] public IEnumerator SkippedActiveStillStopsAtFirstContactWithoutReposing()
        {
            var original = combat.shoulderCharge; var attack = Object.Instantiate(original);
            attack.hitstop = 0; combat.shoulderCharge = attack;
            try
            {
                combat.Queue(CombatIntent.Charge); yield return null;
                player.animator.Play(attack.stateName, 0, .90f); player.animator.Update(0); player.animator.speed = 0;
                Vector3 pose = player.visual.InverseTransformPoint(combat.shoulder.position);
                yield return new WaitForEndOfFrame();
                Assert.That(target.HitCount, Is.EqualTo(1)); Assert.That(combat.ChargeStopped, Is.True);
                Assert.That(player.transform.position.z, Is.LessThan(.99f), "Contact must stop before the full two metres");
                Assert.That(Vector3.Distance(pose, player.visual.InverseTransformPoint(combat.shoulder.position)), Is.LessThan(.0001f));
                yield return new WaitForSeconds(.1f); Assert.That(target.HitCount, Is.EqualTo(1));
            }
            finally { combat.Cancel(); combat.shoulderCharge = original; Object.Destroy(attack); }
        }
    }
}
