using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace WombatLab.Tests
{
    // Stage progression checks intentionally use direct damage; they do not claim a combat victory.
    public class JunkyardChapterPlayTests
    {
        JunkyardChapter chapter;
        EngagementCoordinator encounter;
        PlayerMotor player;
        PlayerDefense defense;
        CombatController combat;
        readonly HashSet<EnemyBrain> parked = new HashSet<EnemyBrain>();
        [UnitySetUp] public IEnumerator Setup()
        {
            SceneManager.LoadScene("JunkyardChapter"); yield return null; yield return null;
            chapter = Object.FindAnyObjectByType<JunkyardChapter>(); encounter = chapter.encounter; player = encounter.player;
            parked.Clear();
            chapter.session.StartRun(false); defense = player.GetComponent<PlayerDefense>(); combat = player.GetComponent<CombatController>();
            player.SetTestInput(Vector2.zero); yield return null;
        }
        [UnityTearDown] public IEnumerator Cleanup()
        {
            player.ReleaseTestInput(); SceneManager.LoadScene("HumanoidCombatLab"); yield return null;
        }
        void Observe()
        {
            foreach (var e in encounter.enemies)
            {
                e.enabled = false;
                // Disabled AI cannot vacate a spawn access by itself. Park fixtures inside
                // the field while retaining their actual colliders, health and role limits.
                if (parked.Add(e)) e.transform.position = new Vector3(chapter.definition.areas[chapter.AreaIndex].center
                    - 2.5f + e.slot % 4 * 1.6f, 0, e.slot < 4 ? 1.3f : -1.3f);
            }
            Physics.SyncTransforms();
            Assert.That(encounter.LivingCount, Is.LessThanOrEqualTo(8));
            Assert.That(encounter.enemies.Count(e => e.target.Alive && e.role.role == EnemyRole.Thrower), Is.LessThanOrEqualTo(2));
        }
        IEnumerator Until(Func<bool> done, float seconds = 20)
        {
            float end = Time.unscaledTime + seconds;
            while (!done() && Time.unscaledTime < end) { Observe(); yield return null; }
            Observe(); Assert.That(done(), Is.True, "Stage condition timed out");
        }
        IEnumerator WalkTo(float x)
        {
            float end = Time.unscaledTime + 12;
            while (Mathf.Abs(player.transform.position.x - x) > .16f && Time.unscaledTime < end)
            { Observe(); player.SetTestInput(new Vector2(Mathf.Sign(x - player.transform.position.x), 0), run: true); yield return null; }
            player.SetTestInput(Vector2.zero); Assert.That(player.transform.position.x, Is.EqualTo(x).Within(.2f), "Physical walkable route");
        }
        void Defeat(EnemyBrain e) { if (e.target.Alive) e.target.ReceiveHit(combat.lights[0], Vector3.zero, 1000); }
        IEnumerator ClearArea()
        {
            int area = chapter.AreaIndex; float end = Time.unscaledTime + 30;
            while (chapter.Phase == ChapterPhase.Fighting && chapter.AreaIndex == area && Time.unscaledTime < end)
            {
                Observe(); foreach (var e in encounter.enemies) Defeat(e); yield return null;
            }
            Assert.That(chapter.CompletedAreas, Is.EqualTo(area + 1));
        }
        IEnumerator ReachSortierhof()
        {
            yield return WalkTo(-4); yield return WalkTo(2); yield return ClearArea();
            yield return WalkTo(10.5f); Assert.That(chapter.TryInteract(), Is.True); yield return WalkTo(20);
        }
        [UnityTest] public IEnumerator FullRouteFiniteReinforcementsAndFinale()
        {
            var roles = chapter.definition.areas.SelectMany(a => a.waves).SelectMany(w => w.enemies).Select(s => s.role).ToArray();
            Assert.That(roles.Length, Is.EqualTo(30)); Assert.That(roles.Count(r => r.role == EnemyRole.Standard), Is.EqualTo(21));
            Assert.That(roles.Count(r => r.role == EnemyRole.Thrower), Is.EqualTo(5)); Assert.That(roles.Count(r => r.role == EnemyRole.Heavy), Is.EqualTo(4));
            Assert.That(chapter.session.Page, Is.EqualTo(SessionPage.Playing));
            yield return WalkTo(-4);
            yield return Until(() => chapter.ReinforcementWarning);
            Assert.That(encounter.LivingCount, Is.Zero, "First spawn must be announced");
            yield return Until(() => chapter.SpawnedInArea == 5);
            var survivor = encounter.enemies[0];
            yield return WalkTo(2); Assert.That(chapter.WaveIndex, Is.EqualTo(1));
            Assert.That(encounter.enemies.Contains(survivor) && survivor.target.Alive, Is.True, "Progress trigger preserves survivors");
            yield return Until(() => chapter.SpawnedInArea == 8);
            yield return WalkTo(0); yield return WalkTo(2);
            Assert.That(chapter.SpawnedInArea + chapter.PendingEnemies, Is.EqualTo(8), "Re-entering trigger never duplicates enemies");
            yield return ClearArea(); yield return WalkTo(10.5f); Assert.That(chapter.TryInteract(), Is.True);
            yield return WalkTo(20); yield return Until(() => chapter.SpawnedInArea == 8);
            yield return WalkTo(26);
            Assert.That(chapter.PendingEnemies, Is.EqualTo(6)); Assert.That(chapter.ReinforcementWarning, Is.False, "Full field blocks further spawns");
            Defeat(encounter.enemies.First(e => e.role.role == EnemyRole.Standard));
            yield return Until(() => chapter.SpawnedInArea == 9);
            yield return ClearArea(); yield return WalkTo(44);
            yield return Until(() => chapter.SpawnedInArea == 8);
            Defeat(encounter.enemies.Single(e => e.role.displayName == "VORARBEITER")); yield return null;
            Assert.That(chapter.Phase, Is.EqualTo(ChapterPhase.Fighting), "Boss death alone is not victory");
            yield return ClearArea(); yield return null;
            Assert.That(chapter.DefeatedEnemies, Is.EqualTo(30)); Assert.That(chapter.PendingEnemies, Is.Zero);
            Assert.That(chapter.session.Page, Is.EqualTo(SessionPage.Result)); Assert.That(Time.timeScale, Is.Zero);
            var replay = GameObject.Find("B8 Chapter Session UI").transform.Find("Safe area/Menu overlay/Menu card/Primary").GetComponent<UnityEngine.UI.Button>();
            replay.onClick.Invoke(); yield return null;
            Assert.That(chapter.Phase, Is.EqualTo(ChapterPhase.Arrival)); Assert.That(chapter.DefeatedEnemies, Is.Zero);
            Assert.That(chapter.PendingEnemies, Is.Zero); Assert.That(chapter.GateOpened, Is.False);
        }
        [UnityTest] public IEnumerator RetryReconstructsPendingEnemiesHealthEnergyAndGate()
        {
            yield return ReachSortierhof();
            // Capture checkpoint values, not regenerated MP several seconds later.
            chapter.RetryCheckpoint(); float entryMP = combat.Energy; int entryHP = defense.Health;
            yield return Until(() => chapter.SpawnedInArea == 8); yield return WalkTo(26);
            Assert.That(chapter.PendingEnemies, Is.EqualTo(6));
            var abandoned = encounter.enemies[0]; Defeat(abandoned); combat.RestoreEnergy(0);
            defense.ReceiveDamage(1000, Vector3.zero, true); yield return null; yield return null;
            chapter.session.Retry();
            Assert.That(combat.Energy, Is.EqualTo(entryMP).Within(.1f)); Assert.That(defense.Health, Is.EqualTo(entryHP));
            Assert.That(chapter.PendingEnemies, Is.EqualTo(8), "Area starts fresh; second encounter remains untriggered");
            Assert.That(chapter.SpawnedInArea, Is.Zero); Assert.That(chapter.WaveIndex, Is.Zero);
            Assert.That(chapter.CompletedAreas, Is.EqualTo(1)); Assert.That(chapter.GateOpened, Is.True);
            Assert.That(chapter.DefeatedEnemies, Is.EqualTo(8)); Assert.That(encounter.Owner, Is.Null);
            yield return null; Assert.That(abandoned == null, Is.True);
            yield return Until(() => chapter.ReinforcementWarning);
            chapter.session.Pause(); int pending = chapter.PendingEnemies;
            yield return new WaitForSecondsRealtime(.25f);
            Assert.That(chapter.SpawnedInArea, Is.Zero); Assert.That(chapter.PendingEnemies, Is.EqualTo(pending));
            chapter.session.Resume();
            // Move into the warned access: it must cancel instead of spawning on the player.
            var warning = chapter.ReinforcementPosition;
            var controller = player.GetComponent<CharacterController>(); controller.enabled = false;
            player.transform.position = warning + Vector3.up * .05f; controller.enabled = true; Physics.SyncTransforms();
            yield return null; yield return null;
            Assert.That(chapter.SpawnedInArea, Is.Zero);
            yield return Until(() => chapter.SpawnedInArea > 0);
            Assert.That(Vector3.Distance(encounter.enemies[0].transform.position, player.transform.position), Is.GreaterThan(2.8f));
            yield return WalkTo(26); Assert.That(chapter.SpawnedInArea + chapter.PendingEnemies, Is.EqualTo(14));
        }
    }
}
