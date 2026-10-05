using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace WombatLab.Tests
{
    public class DuelPlayTests
    {
        [UnityTest] public IEnumerator LightChainConnectsAtEnemyEngagementDistance()
        {
            SceneManager.LoadScene("HumanoidCombatLab"); yield return null;
            var encounter = Object.FindAnyObjectByType<EngagementCoordinator>();
            var player = encounter.player; var combat = player.GetComponent<CombatController>();
            var enemy = encounter.enemies[0]; player.SetTestInput(Vector2.zero);
            float deadline = Time.time + 6;
            while (enemy.State != "TELEGRAPH" && Time.time < deadline) yield return null;
            Assert.That(enemy.State, Is.EqualTo("TELEGRAPH"));
            float distance = Vector3.Distance(player.transform.position, enemy.transform.position);
            player.visual.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(enemy.transform.position - player.transform.position, Vector3.up));
            enemy.enabled = false;
            combat.Queue(CombatIntent.Light);
            for (int stage = 0; stage < 2; stage++)
            {
                while ((combat.Attack != combat.lights[stage] || combat.Progress < .36f) && Time.time < deadline) yield return null;
                Assert.That(combat.Attack, Is.SameAs(combat.lights[stage]));
                combat.Queue(CombatIntent.Light);
                while (combat.Attack == combat.lights[stage] && Time.time < deadline) yield return null;
            }
            yield return new WaitForSeconds(.8f);
            Debug.Log($"DUEL: engagement distance {distance:F3}, hits {enemy.target.HitCount}, enemy HP {enemy.target.Health}, final distance {Vector3.Distance(player.transform.position, enemy.transform.position):F3}");
            Assert.That(enemy.target.HitCount, Is.EqualTo(3), "The normal AI engagement distance must support the light chain");
        }

        [UnityTest] public IEnumerator EnemyWaitsForGetUpProtectionBeforeStartingNextWarning()
        {
            SceneManager.LoadScene("HumanoidCombatLab"); yield return null;
            var encounter = Object.FindAnyObjectByType<EngagementCoordinator>();
            var player = encounter.player; var defense = player.GetComponent<PlayerDefense>();
            var body = player.GetComponent<BodyRecovery>(); player.SetTestInput(Vector2.zero);
            Assert.That(defense.ReceiveDamage(12, Vector3.forward, true), Is.True);
            float deadline = Time.time + 4;
            while (body.Protected && Time.time < deadline)
            {
                Assert.That(encounter.Owner, Is.Null, "Do not pre-load a new attack while the player is getting up");
                yield return null;
            }
            Assert.That(body.Protected, Is.False);
            while (encounter.Owner == null && Time.time < deadline) yield return null;
            Assert.That(encounter.Owner, Is.Not.Null, "The duel must resume after protection ends");
        }

        [UnityTest] public IEnumerator TelegraphCanBeDodgedAndActiveDuelCanBeWonAndRestarted()
        {
            SceneManager.LoadScene("HumanoidCombatLab"); yield return null;
            var encounter = Object.FindAnyObjectByType<EngagementCoordinator>();
            var player = encounter.player; var combat = player.GetComponent<CombatController>();
            var defense = player.GetComponent<PlayerDefense>(); var enemy = encounter.enemies[0];
            player.SetTestInput(Vector2.zero);
            float deadline = Time.time + 6;
            while (enemy.State != "TELEGRAPH" && Time.time < deadline) yield return null;
            Assert.That(enemy.State, Is.EqualTo("TELEGRAPH"));
            Assert.That(defense.TryEvade(Vector2.up), Is.True);
            yield return new WaitForSeconds(1.1f);
            Assert.That(defense.Health, Is.EqualTo(defense.maxHealth), "The direction-locked warning permits a real sidestep miss");
            encounter.ResetEncounter(); deadline = Time.time + 12;
            float nextTap = 0;
            while (enemy.target.Alive && defense.Alive && Time.time < deadline)
            {
                var toward = Vector3.ProjectOnPlane(enemy.transform.position - player.transform.position, Vector3.up);
                if (!combat.Attacking) player.visual.rotation = Quaternion.LookRotation(toward);
                player.SetTestInput(toward.magnitude > 1.2f ? new Vector2(toward.x, toward.z).normalized : Vector2.zero);
                if (Time.time >= nextTap) { combat.Queue(CombatIntent.Light); nextTap = Time.time + .15f; }
                yield return null;
            }
            Assert.That(defense.Alive, Is.True); Assert.That(encounter.LivingCount, Is.Zero);
            Assert.That(enemy.GetComponent<BodyRecovery>().State, Is.EqualTo(RecoveryState.Dead));
            encounter.ResetEncounter(); player.SetTestInput(Vector2.zero); yield return null;
            Assert.That(defense.Health, Is.EqualTo(defense.maxHealth)); Assert.That(encounter.LivingCount, Is.EqualTo(1));
            Assert.That(enemy.GetComponent<BodyRecovery>().Busy, Is.False); Assert.That(combat.Attacking, Is.False);
        }
    }
}
