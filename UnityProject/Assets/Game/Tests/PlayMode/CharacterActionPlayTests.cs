using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace WombatLab.Tests
{
    public class CharacterActionPlayTests
    {
        PlayerMotor player;
        CombatController combat;
        EngagementCoordinator encounter;
        EnemyBrain enemy;
        [UnitySetUp] public IEnumerator Setup()
        {
            SceneManager.LoadScene("HumanoidCombatLab"); yield return null;
            encounter = Object.FindAnyObjectByType<EngagementCoordinator>(); player = encounter.player;
            combat = player.GetComponent<CombatController>(); enemy = encounter.enemies[0];
            foreach (var e in encounter.enemies) e.enabled = false;
            player.SetTestInput(Vector2.zero); yield return new WaitForSeconds(.1f);
        }
        void Place(float distance)
        {
            enemy.transform.position = new Vector3(0, 0, 1);
            var cc = player.GetComponent<CharacterController>(); cc.enabled = false;
            player.transform.position = enemy.transform.position + Vector3.back * distance + Vector3.up * .03f;
            cc.enabled = true; player.visual.rotation = Quaternion.identity; Physics.SyncTransforms();
        }
        [UnityTest] public IEnumerator RunTransitionsHoldSpeedButCannotBypassAttackOrCreateAirSprint()
        {
            float x = player.transform.position.x;
            player.SetTestInput(Vector2.right); yield return new WaitForSeconds(.25f);
            float walk = player.transform.position.x - x; x = player.transform.position.x;
            player.SetTestInput(Vector2.right, run:true); yield return new WaitForSeconds(.25f);
            Assert.That(player.State, Is.EqualTo("Run")); Assert.That(player.transform.position.x - x, Is.GreaterThan(walk * 1.7f));
            player.SetTestInput(Vector2.right); yield return new WaitForSeconds(.1f); Assert.That(player.State, Is.EqualTo("Walk"));
            player.SetTestInput(Vector2.zero, run:true); yield return new WaitForSeconds(.1f); Assert.That(player.State, Is.EqualTo("Idle"));
            Place(2.8f); player.SetTestInput(Vector2.zero); combat.Queue(CombatIntent.Heavy); yield return null;
            player.SetTestInput(Vector2.right,run:true); x=player.transform.position.x;
            yield return new WaitForSeconds(.18f);
            Assert.That(Mathf.Abs(player.transform.position.x-x), Is.LessThan(.05f), "Run must not release committed startup");
            Assert.That(player.animator.GetCurrentAnimatorStateInfo(0).IsName(combat.heavy.stateName),Is.True);
            combat.Cancel(); player.SetTestInput(Vector2.right,true,true); yield return new WaitForSeconds(.15f);
            Assert.That(player.Grounded, Is.False); x=player.transform.position.x;
            yield return new WaitForSeconds(.12f);
            Assert.That(player.transform.position.x-x, Is.LessThan(.45f), "Air control uses walk speed, not sprint");
            player.ResetToSpawn(); yield return new WaitForSeconds(.1f); Assert.That(player.State, Is.EqualTo("Idle"));
        }
        [UnityTest] public IEnumerator PlayerReactionInterruptsAttackResistsLocomotionAndResets()
        {
            var defense=player.GetComponent<PlayerDefense>(); var reaction=player.GetComponent<AnimationReaction>();
            combat.Queue(CombatIntent.Heavy); yield return new WaitForSeconds(.06f);
            Assert.That(defense.ReceiveDamage(12,Vector3.back),Is.True); Assert.That(combat.Attacking,Is.False);
            player.SetTestInput(Vector2.right,run:true); yield return new WaitForSeconds(.08f);
            Assert.That(reaction.Active,Is.True); Assert.That(player.animator.GetCurrentAnimatorStateInfo(0).IsName("Hit"),Is.True);
            combat.Queue(CombatIntent.Light); yield return new WaitForSeconds(.06f); Assert.That(combat.Attacking,Is.False);
            yield return new WaitForSeconds(.3f); Assert.That(reaction.Active,Is.False); Assert.That(player.State,Is.EqualTo("Run"));
            defense.ReceiveDamage(25,Vector3.back); Assert.That(reaction.State,Is.EqualTo("Stagger"));
            player.ResetToSpawn(); yield return null;
            Assert.That(reaction.Active,Is.False); Assert.That(defense.Health,Is.EqualTo(defense.maxHealth));
            Assert.That(player.animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"),Is.True);
        }
        [UnityTest] public IEnumerator EnemyHitAndStaggerHoldThroughHitstopAndReturnToAI()
        {
            enemy.enabled=true; Place(2.8f);
            var reaction=enemy.GetComponent<AnimationReaction>();
            enemy.target.ReceiveHit(combat.lights[0],Vector3.forward); yield return new WaitForSeconds(.06f);
            Assert.That(reaction.Active,Is.True); Assert.That(enemy.animator.GetCurrentAnimatorStateInfo(0).IsName("Hit"),Is.True);
            Assert.That(enemy.State,Is.EqualTo("STAGGER"));
            yield return new WaitForSeconds(.5f); Assert.That(reaction.Active,Is.False); Assert.That(enemy.State,Is.Not.EqualTo("STAGGER"));
            enemy.enabled=false; encounter.ResetEncounter(); Place(1.0f); combat.Queue(CombatIntent.Heavy);
            float deadline=Time.time+1;
            while(!combat.Frozen && Time.time<deadline) yield return null;
            Assert.That(combat.Frozen,Is.True); Assert.That(reaction.State,Is.EqualTo("Stagger"));
            float pose=enemy.animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            yield return new WaitForSeconds(.03f);
            Assert.That(enemy.animator.GetCurrentAnimatorStateInfo(0).normalizedTime,Is.EqualTo(pose).Within(.003f));
            encounter.ResetEncounter(); Assert.That(reaction.Active,Is.False); Assert.That(enemy.target.Hitstun,Is.Zero);
        }
        [UnityTest] public IEnumerator AirSmashOnDescentHitsOnceAndKeepsFlight()
        {
            Place(.8f); player.SetTestInput(Vector2.zero, true);
            yield return new WaitForSeconds(.43f);
            Assert.That(player.Grounded, Is.False); Assert.That(player.VerticalVelocity, Is.LessThan(0));
            combat.Queue(CombatIntent.Heavy); yield return null;
            Assert.That(combat.Attack, Is.SameAs(combat.airHeavy));
            float height = player.transform.position.y;
            yield return new WaitForSeconds(.18f);
            Assert.That(enemy.target.HitCount, Is.EqualTo(1));
            Assert.That(player.transform.position.y, Is.LessThan(height), "Hitstop cannot stop descending flight");
            yield return new WaitForSeconds(.45f);
            Assert.That(enemy.target.HitCount, Is.EqualTo(1)); Assert.That(player.Grounded, Is.True);
            encounter.ResetEncounter(); Place(2.8f); player.SetTestInput(Vector2.zero,true);
            yield return new WaitForSeconds(.43f); combat.Queue(CombatIntent.Heavy);
            yield return new WaitForSeconds(.7f);
            Assert.That(enemy.target.HitCount, Is.Zero, "Smash must not acquire artificial forward reach");
        }
    }
}
