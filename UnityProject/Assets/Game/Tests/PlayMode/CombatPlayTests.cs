using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace WombatLab.Tests
{
    public class CombatPlayTests
    {
        PlayerMotor player;
        CombatController combat;
        TrainingDummy dummy;

        [UnitySetUp] public IEnumerator Setup()
        {
            SceneManager.LoadScene("CombatLab"); yield return null;
            player = Object.FindAnyObjectByType<PlayerMotor>(); combat = player.GetComponent<CombatController>();
            dummy = Object.FindAnyObjectByType<TrainingDummy>();
            Assert.That(combat, Is.Not.Null); Assert.That(dummy, Is.Not.Null);
            Assert.That(combat.dummy, Is.SameAs(dummy)); Assert.That(combat.feedback, Is.Not.Null);
            player.SetTestInput(Vector2.zero);
            var controller = player.GetComponent<CharacterController>(); controller.enabled = false;
            player.transform.position = dummy.transform.position + Vector3.back * 1.05f;
            controller.enabled = true; player.visual.rotation = Quaternion.identity;
            Physics.SyncTransforms(); yield return new WaitForSeconds(.2f);
        }

        IEnumerator WaitUntilPhase(float progress)
        {
            float deadline = Time.realtimeSinceStartup + 3;
            while ((!combat.Attacking || combat.Progress < progress) && Time.realtimeSinceStartup < deadline) yield return null;
            Assert.That(combat.Attacking, Is.True, "Attack did not reach requested phase");
        }

        [UnityTest] public IEnumerator AirAttackStartsAndPreservesJumpTrajectoryAndAirControl()
        {
            player.SetTestInput(Vector2.right, true); yield return new WaitForSeconds(.16f);
            Assert.That(player.Grounded, Is.False); float y = player.transform.position.y, x = player.transform.position.x;
            combat.Queue(CombatIntent.Heavy); yield return null;
            Assert.That(combat.Attacking, Is.True); Assert.That(player.Grounded, Is.False);
            yield return new WaitForSeconds(.12f);
            Assert.That(player.transform.position.y, Is.GreaterThan(y + .1f));
            Assert.That(player.transform.position.x, Is.GreaterThan(x + .2f));
            yield return new WaitForSeconds(1);
            Assert.That(player.Grounded, Is.True); Assert.That(combat.Attacking, Is.False);
        }

        [UnityTest] public IEnumerator AirAttackDoesNotMagicallyHitADifferentLane()
        {
            var cc = player.GetComponent<CharacterController>(); cc.enabled = false;
            player.transform.position += Vector3.right * 2; cc.enabled = true;
            player.SetTestInput(Vector2.zero, true); yield return new WaitForSeconds(.14f);
            combat.Queue(CombatIntent.Light); yield return new WaitForSeconds(.38f);
            Assert.That(dummy.HitCount, Is.Zero); Assert.That(player.Grounded, Is.False);
            yield return new WaitForSeconds(.6f); Assert.That(player.Grounded, Is.True);
        }

        [UnityTest] public IEnumerator LightOnlyHitsInActiveAndOncePerInstance()
        {
            combat.Queue(CombatIntent.Light); yield return WaitUntilPhase(.12f);
            Assert.That(dummy.Health, Is.EqualTo(dummy.maxHealth)); Assert.That(combat.HitboxOpen, Is.False);
            yield return new WaitForSeconds(.55f);
            Assert.That(dummy.Health, Is.EqualTo(dummy.maxHealth - combat.lights[0].damage));
            Assert.That(dummy.HitCount, Is.EqualTo(1)); Assert.That(combat.Attacking, Is.False);
            Assert.That(combat.HitboxOpen, Is.False);
            Assert.That(player.visual.Find("Rig/LeftArm").localPosition.z, Is.EqualTo(.06f).Within(.01f));
            Assert.That(player.visual.Find("Rig/LeftArm").localPosition.y, Is.EqualTo(1.25f).Within(.01f));
        }

        [UnityTest] public IEnumerator BufferedLightInputsChainAllThreeClips()
        {
            combat.Queue(CombatIntent.Light); yield return WaitUntilPhase(.33f);
            combat.Queue(CombatIntent.Light);
            float deadline = Time.realtimeSinceStartup + 3;
            while (combat.Attack != combat.lights[1] && Time.realtimeSinceStartup < deadline) yield return null;
            Assert.That(combat.Attack, Is.SameAs(combat.lights[1]));
            yield return WaitUntilPhase(.33f); combat.Queue(CombatIntent.Light);
            while (combat.Attack != combat.lights[2] && Time.realtimeSinceStartup < deadline) yield return null;
            Assert.That(combat.Attack, Is.SameAs(combat.lights[2]));
            yield return new WaitForSeconds(.7f);
            Assert.That(dummy.HitCount, Is.EqualTo(3));
            Assert.That(dummy.Health, Is.EqualTo(dummy.maxHealth - 43));
            Assert.That(combat.AttackInstance, Is.EqualTo(3)); Assert.That(combat.Attacking, Is.False);
        }

        [UnityTest] public IEnumerator HeavyHasLongerStartupMoreDamageAndKnockback()
        {
            var start = dummy.transform.position;
            combat.Queue(CombatIntent.Heavy); yield return new WaitForSeconds(.20f);
            Assert.That(dummy.HitCount, Is.Zero);
            yield return new WaitForSeconds(.9f);
            Assert.That(dummy.HitCount, Is.EqualTo(1));
            Assert.That(dummy.Health, Is.EqualTo(dummy.maxHealth - combat.heavy.damage));
            Assert.That(dummy.transform.position.z - start.z, Is.GreaterThan(.50f));
        }

        [UnityTest] public IEnumerator TargetBehindAttackerAndDifferentDepthAreNotHit()
        {
            player.visual.rotation = Quaternion.Euler(0, 180, 0);
            combat.Queue(CombatIntent.Light); yield return new WaitForSeconds(.6f);
            Assert.That(dummy.HitCount, Is.Zero);
            player.visual.rotation = Quaternion.identity;
            var cc = player.GetComponent<CharacterController>(); cc.enabled = false;
            player.transform.position += Vector3.right * 2; cc.enabled = true;
            combat.Queue(CombatIntent.Heavy); yield return new WaitForSeconds(1.1f);
            Assert.That(dummy.HitCount, Is.Zero);
        }

        [UnityTest] public IEnumerator ResetDuringActiveClearsAttackBufferHitstopAndHealth()
        {
            combat.Queue(CombatIntent.Light); yield return WaitUntilPhase(.30f);
            combat.Queue(CombatIntent.Heavy); player.ResetToSpawn();
            Assert.That(combat.Attacking, Is.False); Assert.That(combat.Frozen, Is.False);
            Assert.That(combat.HitboxOpen, Is.False); Assert.That(dummy.Health, Is.EqualTo(dummy.maxHealth));
            Assert.That(dummy.HitCount, Is.Zero); Assert.That(player.animator.speed, Is.EqualTo(1));
            yield return new WaitForSeconds(.5f); Assert.That(combat.Attacking, Is.False);
        }

        [UnityTest] public IEnumerator AttackStepIsBoundedAndJumpRemainsCommittedUntilRecovery()
        {
            combat.Queue(CombatIntent.Heavy); yield return WaitUntilPhase(.10f);
            var start = player.transform.position; var facing = player.visual.rotation;
            player.SetTestInput(Vector2.left, true); yield return new WaitForSeconds(.20f);
            Assert.That(Vector3.Distance(start, player.transform.position), Is.LessThan(.25f));
            Assert.That(player.Grounded, Is.True);
            Assert.That(Quaternion.Angle(facing, player.visual.rotation), Is.LessThanOrEqualTo(25.1f));
            yield return new WaitForSeconds(.9f);
            Assert.That(player.transform.position.x, Is.LessThan(start.x - .25f));
        }

        [UnityTest] public IEnumerator ExternalAnimatorInterruptionClosesAttack()
        {
            combat.Queue(CombatIntent.Light); yield return WaitUntilPhase(.12f);
            player.animator.Play("Idle", 0, 0); yield return null; yield return null;
            Assert.That(combat.Attacking, Is.False); Assert.That(combat.HitboxOpen, Is.False);
            yield return new WaitForSeconds(.3f); Assert.That(dummy.HitCount, Is.Zero);
        }

        [UnityTest] public IEnumerator RealAnimatedHitSweepIsStableAt30_60_120AndSkippedActive()
        {
            var original = combat.lights[0]; var testAttack = Object.Instantiate(original);
            testAttack.hitstop = 0; combat.lights[0] = testAttack;
            try
            {
                foreach (int fps in new[] { 30, 60, 120, 3 })
                {
                    player.ResetToSpawn();
                    var cc = player.GetComponent<CharacterController>(); cc.enabled = false;
                    player.transform.position = dummy.transform.position + Vector3.back * 1.05f;
                    cc.enabled = true; player.visual.rotation = Quaternion.identity;
                    yield return new WaitForSeconds(.1f);
                    combat.Queue(CombatIntent.Light); yield return null;
                    Assert.That(combat.Attacking, Is.True);
                    // Drive the actual Animator and production sweep with explicit frame
                    // intervals. No wall-clock FPS claim; motor/knockback aren't simulated here.
                    for (float time = 1f / fps; time < testAttack.clip.length + 1f / fps; time += 1f / fps)
                    {
                        player.animator.Play(testAttack.stateName, 0, Mathf.Clamp01(time / testAttack.clip.length));
                        player.animator.Update(0); combat.SendMessage("LateUpdate");
                    }
                    Assert.That(dummy.HitCount, Is.EqualTo(1), $"Animated sweep at {fps} FPS");
                    Assert.That(dummy.Health, Is.EqualTo(dummy.maxHealth - 10));
                    combat.ResetCombat();
                }
            }
            finally { combat.lights[0] = original; Object.Destroy(testAttack); }
        }

        [UnityTest] public IEnumerator DuplicateHurtboxesStillOnlyReceiveOneHit()
        {
            var hurt = dummy.transform.Find("Hurtbox — team 2, separate from body");
            var extra = hurt.gameObject.AddComponent<BoxCollider>(); extra.center = new Vector3(0, 1.2f, 0);
            extra.size = Vector3.one * .65f; extra.isTrigger = true;
            combat.Queue(CombatIntent.Light); yield return new WaitForSeconds(.65f);
            Assert.That(dummy.HitCount, Is.EqualTo(1));
        }

        [UnityTest] public IEnumerator OldBufferedInputDoesNotReplayAfterHeavy()
        {
            combat.Queue(CombatIntent.Heavy); yield return WaitUntilPhase(.10f);
            combat.Queue(CombatIntent.Light); yield return new WaitForSeconds(1.15f);
            Assert.That(combat.AttackInstance, Is.EqualTo(1)); Assert.That(combat.Attacking, Is.False);
        }

        [UnityTest] public IEnumerator TargetDeathPreventsMoreHitsAndResetRestoresIt()
        {
            for (int i = 0; i < 5; i++) dummy.ReceiveHit(combat.heavy, Vector3.zero);
            Assert.That(dummy.Alive, Is.False);
            combat.Queue(CombatIntent.Heavy); yield return new WaitForSeconds(1.1f);
            Assert.That(dummy.Health, Is.Zero); Assert.That(dummy.HitCount, Is.EqualTo(5));
            player.ResetToSpawn(); Assert.That(dummy.Health, Is.EqualTo(150)); Assert.That(dummy.HitCount, Is.Zero);
        }

    }
}
