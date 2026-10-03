using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace WombatLab.Tests
{
    public class HumanoidCombatPlayTests
    {
        PlayerMotor player;
        CombatController combat;
        EngagementCoordinator encounter;
        TrainingDummy target;
        [UnitySetUp] public IEnumerator Setup()
        {
            SceneManager.LoadScene("HumanoidCombatLab"); yield return null;
            encounter = Object.FindAnyObjectByType<EngagementCoordinator>(); player = encounter.player;
            combat = player.GetComponent<CombatController>(); target = encounter.enemies[0].target;
            foreach (var enemy in encounter.enemies) enemy.enabled = false;
            player.SetTestInput(Vector2.zero); Place(1.0f); yield return new WaitForSeconds(.1f);
        }
        void Place(float distance)
        {
            target.transform.position = new Vector3(0, 0, 1);
            var capsule = player.GetComponent<CharacterController>(); capsule.enabled = false;
            player.transform.position = target.transform.position + Vector3.back * distance + Vector3.up * .03f;
            capsule.enabled = true; player.visual.rotation = Quaternion.identity; Physics.SyncTransforms();
        }
        [UnityTest] public IEnumerator PunchAndKickHaveStartupOneHitMissAndReset()
        {
            foreach (var intent in new[] { CombatIntent.Light, CombatIntent.Kick })
            {
                encounter.ResetEncounter(); Place(1.0f); yield return new WaitForSeconds(.1f);
                var attack = intent == CombatIntent.Light ? combat.lights[0] : combat.kick;
                Assert.That(attack.clip.isHumanMotion, Is.True);
                Assert.That(attack.clip.length, Is.EqualTo(attack.Duration).Within(.002f));
                combat.Queue(intent); yield return new WaitForSeconds(attack.startupSeconds * .5f);
                Assert.That(target.HitCount, Is.Zero, "Startup cannot deal damage");
                yield return new WaitForSeconds(attack.Duration + .15f);
                Assert.That(target.HitCount, Is.EqualTo(1), intent + " must hit only once");
                Assert.That(target.Health, Is.EqualTo(target.maxHealth - attack.damage));
                Assert.That(combat.Attacking, Is.False);
                encounter.ResetEncounter(); Place(2.8f); yield return new WaitForSeconds(.1f);
                combat.Queue(intent); yield return new WaitForSeconds(attack.Duration + .15f);
                Assert.That(target.HitCount, Is.Zero, "No reach beyond the avatar's contact path");
            }
            encounter.ResetEncounter(); yield return null;
            Assert.That(target.Health, Is.EqualTo(target.maxHealth)); Assert.That(combat.Frozen, Is.False);
            Assert.That(player.animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"), Is.True);
        }
        [UnityTest] public IEnumerator SkippedActiveHitsWithoutReposingVisibleHumanoid()
        {
            var original = combat.lights[0]; var attack = Object.Instantiate(original);
            attack.hitstop = 0; combat.lights[0] = attack;
            try
            {
                combat.Queue(CombatIntent.Light); yield return null;
                Assert.That(target.HitCount, Is.Zero);
                player.animator.Play(attack.stateName, 0, .90f); player.animator.Update(0);
                player.animator.speed = 0; // Hold this exact pose through the native animation update.
                Vector3 visible = player.visual.InverseTransformPoint(combat.ContactPoint.position);
                yield return new WaitForEndOfFrame();
                Assert.That(target.HitCount, Is.EqualTo(1), "Entire active interval was skipped");
                Assert.That(Vector3.Distance(visible, player.visual.InverseTransformPoint(combat.ContactPoint.position)), Is.LessThan(.0001f), "Sweep must not sample onto the visible skeleton");
                yield return new WaitForSeconds(.2f); Assert.That(target.HitCount, Is.EqualTo(1));
            }
            finally { combat.Cancel(); combat.lights[0] = original; Object.Destroy(attack); }
        }
        [UnityTest] public IEnumerator ContactPathsMatchRetargetedPoseForEveryAttack()
        {
            foreach (var attack in combat.lights.Concat(new[] { combat.heavy, combat.kick, combat.airKick, combat.airHeavy }))
            {
                Assert.That(attack.HasHumanoidContact, Is.True);
                Assert.That(attack.contactAvatar, Is.SameAs(player.animator.avatar));
                var contact = attack.foot ? (attack.rightHand ? combat.rightFoot : combat.leftFoot) : (attack.rightHand ? combat.rightFist : combat.leftFist);
                foreach (float phase in new[] { .15f, attack.ActiveStart, (attack.ActiveStart + attack.ActiveEnd) / 2, attack.ActiveEnd, .90f })
                {
                    player.animator.Play(attack.stateName, 0, phase); player.animator.Update(0);
                    Vector3 visible = player.visual.InverseTransformPoint(contact.position);
                    Assert.That(Vector3.Distance(visible, attack.LocalContact(phase)), Is.LessThan(.025f), attack.name + " pose/path mismatch at " + phase);
                }
            }
            player.animator.Play("Idle", 0, 0); yield return null;
        }
        [UnityTest] public IEnumerator AuthoredDurationAndHitstopKeepOneAnimationClock()
        {
            var original = combat.lights[0]; var attack = Object.Instantiate(original);
            attack.hitstop = 0; combat.lights[0] = attack;
            try
            {
                Place(2.8f); combat.Queue(CombatIntent.Light); yield return null;
                float start = Time.time, deadline = start + 2;
                while (combat.Attacking && Time.time < deadline) yield return null;
                Assert.That(Time.time - start, Is.EqualTo(attack.Duration).Within(.065f), "Imported source duration must not define gameplay duration");
                encounter.ResetEncounter(); Place(1.0f); yield return new WaitForSeconds(.1f);
                attack.hitstop = .16f; combat.Queue(CombatIntent.Light);
                deadline = Time.time + 1;
                while (!combat.Frozen && Time.time < deadline) yield return null;
                Assert.That(combat.Frozen, Is.True);
                float phase = combat.Progress;
                yield return new WaitForSeconds(.05f);
                Assert.That(combat.Progress, Is.EqualTo(phase).Within(.001f));
                yield return new WaitForSeconds(.6f); Assert.That(combat.Attacking, Is.False);
            }
            finally { combat.Cancel(); combat.lights[0] = original; Object.Destroy(attack); }
        }
        [UnityTest] public IEnumerator ComboUsesThreeInstancesAndAirKickHitsWithoutStoppingFlight()
        {
            combat.Queue(CombatIntent.Light);
            float deadline = Time.time + 3;
            for (int stage = 0; stage < 2; stage++)
            {
                while ((combat.Attack != combat.lights[stage] || combat.Progress < .36f) && Time.time < deadline) yield return null;
                Assert.That(combat.Attack, Is.SameAs(combat.lights[stage]));
                combat.Queue(CombatIntent.Light);
                while (combat.Attack == combat.lights[stage] && Time.time < deadline) yield return null;
            }
            yield return new WaitForSeconds(.7f);
            Assert.That(target.HitCount, Is.EqualTo(3)); Assert.That(combat.AttackInstance, Is.EqualTo(3));
            encounter.ResetEncounter(); Place(.95f); yield return new WaitForSeconds(.1f);
            player.SetTestInput(Vector2.zero, true); yield return new WaitForSeconds(.22f);
            Assert.That(player.Grounded, Is.False); float height = player.transform.position.y;
            combat.Queue(CombatIntent.Light); yield return new WaitForSeconds(.15f);
            Assert.That(target.HitCount, Is.EqualTo(1));
            Assert.That(player.transform.position.y, Is.GreaterThan(height), "Air contact must preserve the jump arc");
            yield return new WaitForSeconds(.7f); Assert.That(player.Grounded, Is.True); Assert.That(combat.Attacking, Is.False);
        }
    }
}
