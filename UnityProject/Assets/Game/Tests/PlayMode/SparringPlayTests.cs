using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace WombatLab.Tests
{
    public class SparringPlayTests
    {
        EngagementCoordinator encounter;
        PlayerDefense defense;
        [UnitySetUp] public IEnumerator Setup()
        {
            SceneManager.LoadScene("SparringLab"); yield return null;
            encounter = Object.FindAnyObjectByType<EngagementCoordinator>(); defense = encounter.player.GetComponent<PlayerDefense>();
            encounter.player.SetTestInput(Vector2.zero); yield return new WaitForSeconds(.2f);
        }
        [UnityTest] public IEnumerator EvadeMovesAndHasOnlyTheDefinedInvulnerabilityWindow()
        {
            var start = encounter.player.transform.position;
            Assert.That(defense.TryEvade(Vector2.left), Is.True);
            Assert.That(defense.Invulnerable, Is.False); Assert.That(defense.TryEvade(Vector2.left), Is.False);
            yield return new WaitForSeconds(.08f);
            Assert.That(defense.Invulnerable, Is.True); Assert.That(defense.ReceiveDamage(12, Vector3.right), Is.False);
            yield return new WaitForSeconds(.19f);
            Assert.That(defense.Invulnerable, Is.False);
            Assert.That(encounter.player.transform.position.x, Is.LessThan(start.x - 1.5f));
            yield return new WaitForSeconds(.4f);
            Assert.That(defense.ReceiveDamage(12, Vector3.right), Is.True); Assert.That(defense.Health, Is.EqualTo(88));
        }
        [UnityTest] public IEnumerator EnemyWarnsBeforeDealingOneHitAndThenRecovers()
        {
            float deadline = Time.time + 5;
            var enemy = encounter.enemies[0];
            while (enemy.State != "TELEGRAPH" && Time.time < deadline) yield return null;
            Assert.That(enemy.State, Is.EqualTo("TELEGRAPH")); Assert.That(enemy.warning.enabled, Is.True);
            Assert.That(defense.Health, Is.EqualTo(100));
            yield return new WaitForSeconds(.40f); Assert.That(defense.Health, Is.EqualTo(100));
            yield return new WaitForSeconds(.65f); Assert.That(defense.Health, Is.EqualTo(88));
            yield return new WaitForSeconds(.25f); Assert.That(defense.Health, Is.EqualTo(88));
        }
        [UnityTest] public IEnumerator TimedEvadeAvoidsTheActualAnimatedEnemyStrike()
        {
            var enemy = encounter.enemies[0]; float deadline = Time.time + 5;
            while (enemy.State != "TELEGRAPH" && Time.time < deadline) yield return null;
            Assert.That(enemy.State, Is.EqualTo("TELEGRAPH"));
            yield return new WaitForSeconds(.69f);
            Assert.That(defense.TryEvade(Vector2.down), Is.True);
            yield return new WaitForSeconds(.65f);
            Assert.That(defense.Health, Is.EqualTo(100), "Dodging the locked-in strike should preserve health");
        }

        [UnityTest] public IEnumerator TwoEnemiesNeverAttackTogetherAndReleaseTokenOnHitDeathAndReset()
        {
            encounter.SetMode(2); Assert.That(encounter.LivingCount, Is.EqualTo(2));
            int owners = 0; EnemyBrain last = null;
            for (float until = Time.time + 7; Time.time < until;)
            {
                int committed = 0;
                foreach (var e in encounter.enemies) if (e.State == "TELEGRAPH" || e.State == "ATTACK" || e.State == "RECOVERY") committed++;
                Assert.That(committed, Is.LessThanOrEqualTo(1));
                if (encounter.Owner != null && encounter.Owner != last) { last = encounter.Owner; owners++; }
                defense.ResetDefense(); yield return null;
            }
            Assert.That(owners, Is.GreaterThanOrEqualTo(2), "Both opponents must get turns without a stuck token");
            var owner = encounter.Owner;
            if (owner != null) { owner.target.ReceiveHit(encounter.player.GetComponent<CombatController>().heavy, Vector3.zero); Assert.That(encounter.Owner, Is.Null); }
            var enemy = encounter.enemies[0];
            for (int i = 0; i < 3; i++) enemy.target.ReceiveHit(encounter.player.GetComponent<CombatController>().heavy, Vector3.zero);
            Assert.That(enemy.target.Alive, Is.False); Assert.That(encounter.Owner, Is.Not.SameAs(enemy));
            encounter.ResetEncounter(); Assert.That(encounter.Owner, Is.Null); Assert.That(encounter.LivingCount, Is.EqualTo(2)); Assert.That(defense.Health, Is.EqualTo(100));
        }
        [UnityTest] public IEnumerator DodgeCancelsPlayerAttackAndResetClearsAllDefenseState()
        {
            var combat = encounter.player.GetComponent<CombatController>(); combat.Queue(CombatIntent.Heavy); yield return null;
            Assert.That(combat.Attacking, Is.True); Assert.That(defense.TryEvade(Vector2.right), Is.True);
            Assert.That(combat.Attacking, Is.False);
            encounter.ResetEncounter(); Assert.That(defense.Evading, Is.False); Assert.That(defense.Cooldown, Is.Zero); Assert.That(combat.Frozen, Is.False);
        }
    }
}
