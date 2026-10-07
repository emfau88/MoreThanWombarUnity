using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace WombatLab.Tests
{
    public class EnemyRolesPlayTests
    {
        EngagementCoordinator encounter;
        PlayerMotor player;
        PlayerDefense defense;
        [UnitySetUp] public IEnumerator Setup()
        {
            SceneManager.LoadScene("HumanoidCombatLab"); yield return null;
            encounter = Object.FindAnyObjectByType<EngagementCoordinator>(); player = encounter.player;
            defense = player.GetComponent<PlayerDefense>(); player.SetTestInput(Vector2.zero);
            encounter.SetMode(4); foreach (var enemy in encounter.enemies) enemy.enabled = false;
            yield return null;
        }
        void Place(EnemyBrain enemy, float distance)
        {
            var cc = player.GetComponent<CharacterController>(); cc.enabled = false;
            player.transform.position = new Vector3(0,.03f,-1); cc.enabled = true;
            enemy.transform.position = player.transform.position + Vector3.forward * distance;
            enemy.visual.rotation = Quaternion.LookRotation(Vector3.back);
            Physics.SyncTransforms();
        }
        IEnumerator WaitFor(EnemyBrain enemy, string state, float limit = 5)
        {
            float until = Time.time + limit;
            while (enemy.State != state && Time.time < until) yield return null;
            Assert.That(enemy.State, Is.EqualTo(state), enemy.RoleName);
        }
        [UnityTest] public IEnumerator RolesWarnThenDealTheirOwnSingleHitAndHeavyKnocksDown()
        {
            foreach (int index in new[] {1,2,0})
            {
                encounter.ResetEncounter(); var enemy = encounter.enemies[index];
                Place(enemy, enemy.role.role == EnemyRole.Agile ? 2.6f : 1.15f); enemy.enabled = true;
                yield return WaitFor(enemy,"TELEGRAPH");
                Assert.That(defense.Health,Is.EqualTo(defense.maxHealth));
                yield return new WaitForSeconds(enemy.role.telegraphSeconds * .7f);
                Assert.That(defense.Health,Is.EqualTo(defense.maxHealth), "Warning cannot damage");
                float until = Time.time + 2;
                while (defense.Health == defense.maxHealth && Time.time < until) yield return null;
                Assert.That(defense.Health,Is.EqualTo(defense.maxHealth - enemy.role.damage), enemy.RoleName + " must connect");
                Assert.That(player.GetComponent<BodyRecovery>().Busy,Is.EqualTo(enemy.role.role == EnemyRole.Heavy));
                yield return new WaitForSeconds(.3f);
                Assert.That(defense.Health,Is.EqualTo(defense.maxHealth - enemy.role.damage), "One hit per attack");
                enemy.enabled = false;
            }
        }
        [UnityTest] public IEnumerator RushLocksWarningLaneCanBeSidesteppedAndStopsAtWall()
        {
            var enemy = encounter.enemies[2]; Place(enemy,2.6f); enemy.enabled = true;
            yield return WaitFor(enemy,"TELEGRAPH");
            var direction = enemy.visual.forward; var start = enemy.transform.position;
            Assert.That(enemy.chargeWarning.gameObject.activeSelf,Is.True);
            player.SetTestInput(Vector2.right); yield return new WaitForSeconds(.45f); player.SetTestInput(Vector2.zero);
            Assert.That(Vector3.Angle(direction,enemy.visual.forward),Is.LessThan(.1f));
            yield return WaitFor(enemy,"RECOVERY");
            Assert.That(defense.Health,Is.EqualTo(defense.maxHealth));
            Assert.That(Vector3.ProjectOnPlane(enemy.transform.position-start,Vector3.up).magnitude,Is.EqualTo(enemy.attack.chargeDistance).Within(.06f));
            encounter.ResetEncounter(); Place(enemy,2.6f); yield return WaitFor(enemy,"TELEGRAPH");
            var wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wall.transform.position = enemy.transform.position + enemy.visual.forward * .6f + Vector3.up;
            wall.transform.localScale = new Vector3(3,2,.15f); Physics.SyncTransforms(); start = enemy.transform.position;
            try
            {
                yield return WaitFor(enemy,"RECOVERY");
                Assert.That(enemy.ChargeStopped,Is.True);
                Assert.That(Vector3.Distance(enemy.transform.position,start),Is.LessThan(.6f));
                Assert.That(defense.Health,Is.EqualTo(defense.maxHealth));
            }
            finally { Object.Destroy(wall); }
        }
        [UnityTest] public IEnumerator FourEnemiesShareOneTokenAndAllGetTurns()
        {
            encounter.ResetEncounter(); foreach (var enemy in encounter.enemies) enemy.enabled = true;
            var seen = new HashSet<EnemyBrain>(); float until = Time.time + 20;
            while (seen.Count < 4 && Time.time < until)
            {
                int committed = 0;
                foreach(var enemy in encounter.enemies) if (enemy.Committed) committed++;
                Assert.That(committed,Is.LessThanOrEqualTo(1));
                if (encounter.Owner != null) seen.Add(encounter.Owner);
                defense.ResetDefense(); yield return null;
            }
            Assert.That(seen.Count,Is.EqualTo(4),"Every role must get a turn without a stuck token");
            var owner = encounter.Owner;
            Assert.That(owner,Is.Not.Null); owner.target.ReceiveHit(player.GetComponent<CombatController>().shoulderCharge,Vector3.zero);
            Assert.That(encounter.Owner,Is.Null,"A shoulder hit interrupts the warning and releases permission");
            encounter.ResetEncounter(); Assert.That(encounter.LivingCount,Is.EqualTo(4));
            Assert.That(defense.Health,Is.EqualTo(defense.maxHealth));
            foreach(var enemy in encounter.enemies) Assert.That(enemy.Committed,Is.False);
            encounter.SetMode(3); Assert.That(encounter.LivingCount,Is.EqualTo(3));
            encounter.SetMode(1); Assert.That(encounter.LivingCount,Is.EqualTo(1));
            foreach(var button in player.GetComponentsInChildren<UnityEngine.UI.Button>(true))
                if(button.name == "Opponent count") { button.onClick.Invoke(); break; }
            yield return null;
            Assert.That(encounter.Mode,Is.EqualTo(2),"Touch group selector uses the same reset/mode flow");
        }
        [UnityTest] public IEnumerator OffscreenAndGetUpProtectionRefusePermissionAndRushCanBeInterrupted()
        {
            var enemy = encounter.enemies[2]; Place(enemy,2.6f); enemy.enabled = true;
            yield return WaitFor(enemy,"TELEGRAPH");
            enemy.target.ReceiveHit(player.GetComponent<CombatController>().kick,Vector3.zero);
            Assert.That(encounter.Owner,Is.Null); Assert.That(enemy.State,Is.EqualTo("STAGGER"));
            Assert.That(enemy.chargeWarning.gameObject.activeSelf,Is.False);
            enemy.transform.position = new Vector3(20,0,0); enemy.enabled = false;
            Assert.That(encounter.IsInView(enemy),Is.False); Assert.That(encounter.TryAcquire(enemy),Is.False);
            encounter.ResetEncounter(); Place(encounter.enemies[0],1.15f); encounter.enemies[0].enabled = true;
            Assert.That(defense.ReceiveDamage(12,Vector3.zero,true),Is.True);
            float until = Time.time + 4;
            while(player.GetComponent<BodyRecovery>().Protected && Time.time < until)
            { Assert.That(encounter.Owner,Is.Null); yield return null; }
            Assert.That(player.GetComponent<BodyRecovery>().Protected,Is.False);
            yield return WaitFor(encounter.enemies[0],"TELEGRAPH");
        }
    }
}
