using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace WombatLab.Tests
{
    public class CombatPolishPlayTests
    {
        EngagementCoordinator encounter;
        PlayerMotor player;
        CombatController combat;
        EnemyBrain enemy;
        [UnitySetUp] public IEnumerator Setup()
        {
            SceneManager.LoadScene("SparringLab"); yield return null;
            encounter = Object.FindAnyObjectByType<EngagementCoordinator>();
            player = encounter.player; combat = player.GetComponent<CombatController>(); enemy = encounter.enemies[0];
            // Isolate attack geometry from an opponent deliberately moving out of it.
            enemy.enabled = false; player.SetTestInput(Vector2.zero);
            Place(1.4f); yield return new WaitForSeconds(.15f);
        }
        void Place(float distance)
        {
            // Keep both endpoints inside the arena; otherwise its clamp silently
            // turns the intended long-range miss into a close-range contact.
            enemy.transform.position = new Vector3(0, 0, 1.0f);
            var cc = player.GetComponent<CharacterController>(); cc.enabled = false;
            player.transform.position = enemy.transform.position + Vector3.back * distance + Vector3.up * .03f;
            cc.enabled = true; player.visual.rotation = Quaternion.identity; Physics.SyncTransforms();
        }
        [UnityTest] public IEnumerator TimedJumpKickActuallyHitsGroundEnemyAndLands()
        {
            player.SetTestInput(Vector2.zero, true); yield return new WaitForSeconds(.24f);
            Assert.That(player.Grounded, Is.False);
            combat.Queue(CombatIntent.Light); yield return new WaitForSeconds(.30f);
            Assert.That(enemy.target.HitCount, Is.EqualTo(1), "A descending foot must contact the ground opponent");
            Assert.That(enemy.target.Health, Is.EqualTo(80 - combat.airKick.damage));
            yield return new WaitForSeconds(.45f);
            Assert.That(player.Grounded, Is.True); Assert.That(combat.Attacking, Is.False);
        }
        [UnityTest] public IEnumerator KickReachesBeyondJabButDoesNotHitOutsideItsVisibleReach()
        {
            Place(2.00f); combat.Queue(CombatIntent.Light); yield return new WaitForSeconds(.60f);
            Assert.That(enemy.target.HitCount, Is.Zero);
            encounter.ResetEncounter(); Place(2.00f); yield return new WaitForSeconds(.1f);
            combat.Queue(CombatIntent.Kick); yield return new WaitForSeconds(.80f);
            Assert.That(enemy.target.HitCount, Is.EqualTo(1));
            encounter.ResetEncounter(); Place(2.90f); yield return new WaitForSeconds(.1f);
            combat.Queue(CombatIntent.Kick); yield return new WaitForSeconds(.80f);
            Assert.That(enemy.target.HitCount, Is.Zero);
        }
        [UnityTest] public IEnumerator DeadEnemyFallsAsAWholeStopsBlockingAndFullyResets()
        {
            enemy.enabled = true;
            Assert.That(encounter.TryAcquire(enemy), Is.False); // Reset rest period is active.
            for (int i = 0; i < 3; i++) enemy.target.ReceiveHit(combat.heavy, Vector3.zero);
            yield return new WaitForSeconds(.55f);
            Assert.That(enemy.target.Alive, Is.False); Assert.That(enemy.State, Is.EqualTo("DOWN"));
            Assert.That(encounter.Owner, Is.Not.SameAs(enemy));
            var rig = enemy.visual.Find("Rig");
            Assert.That(Vector3.Dot(rig.up, Vector3.up), Is.LessThan(.15f));
            Assert.That(enemy.animator.enabled, Is.False);
            foreach (var collider in enemy.GetComponentsInChildren<Collider>()) Assert.That(collider.enabled, Is.False);
            encounter.ResetEncounter(); yield return null;
            Assert.That(enemy.target.Health, Is.EqualTo(80)); Assert.That(enemy.animator.enabled, Is.True);
            Assert.That(Quaternion.Angle(rig.localRotation, Quaternion.identity), Is.LessThan(1));
            foreach (var collider in enemy.GetComponentsInChildren<Collider>()) Assert.That(collider.enabled, Is.True);
        }
        [UnityTest] public IEnumerator AttackStepCannotCrossArenaBoundary()
        {
            var cc = player.GetComponent<CharacterController>(); cc.enabled = false;
            player.transform.position = new Vector3(6.94f, .03f, 0); cc.enabled = true;
            player.visual.rotation = Quaternion.LookRotation(Vector3.right);
            yield return new WaitForSeconds(.1f); combat.Queue(CombatIntent.Kick);
            yield return new WaitForSeconds(.8f);
            Assert.That(player.transform.position.x, Is.InRange(6.93f, 7.01f));
        }

        [UnityTest] public IEnumerator StartupTurnUsesOne25DegreeBudgetThenRecoveryReleasesMovement()
        {
            // No target contact/hitstop: measure the complete startup correction
            // relative to the facing BEFORE Begin, not its already rotated pose.
            Place(2.9f);
            var originalFacing = player.visual.rotation;
            player.SetTestInput(Vector2.right);
            combat.Queue(CombatIntent.Heavy);
            float deadline = Time.time + 2;
            while (!combat.Attacking && Time.time < deadline) yield return null;
            Assert.That(combat.Attacking, Is.True);
            while (combat.Attacking && combat.Progress < combat.heavy.activeStart && Time.time < deadline)
            {
                Assert.That(Quaternion.Angle(originalFacing, player.visual.rotation), Is.LessThanOrEqualTo(25.1f));
                yield return null;
            }
            Assert.That(combat.Progress, Is.GreaterThanOrEqualTo(combat.heavy.activeStart));
            var activeFacing = player.visual.rotation;
            player.SetTestInput(Vector2.left);
            while (combat.Attacking && combat.Progress < combat.heavy.moveRelease && Time.time < deadline)
            {
                Assert.That(Quaternion.Angle(activeFacing, player.visual.rotation), Is.LessThan(.1f));
                yield return null;
            }
            Assert.That(combat.Progress, Is.GreaterThanOrEqualTo(combat.heavy.moveRelease));
            var recoveryPosition = player.transform.position;
            yield return new WaitForSeconds(.05f);
            Assert.That(combat.Attacking, Is.True, "Movement must resume before the attack ends");
            Assert.That(player.transform.position.x, Is.LessThan(recoveryPosition.x - .05f));
            yield return new WaitForSeconds(.25f);
            Assert.That(combat.Attacking, Is.False);
            Assert.That(player.State, Is.EqualTo("Walk"));
        }
    }
}
