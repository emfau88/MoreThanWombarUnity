using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace WombatLab.Tests
{
    public class BodyRecoveryPlayTests
    {
        EngagementCoordinator encounter;
        PlayerMotor player;
        CombatController combat;
        PlayerDefense defense;
        EnemyBrain enemy;
        BodyRecovery playerBody, enemyBody;
        [UnitySetUp] public IEnumerator Setup()
        {
            SceneManager.LoadScene("HumanoidCombatLab"); yield return null;
            encounter=Object.FindAnyObjectByType<EngagementCoordinator>(); player=encounter.player;
            combat=player.GetComponent<CombatController>(); defense=player.GetComponent<PlayerDefense>();
            enemy=encounter.enemies[0]; playerBody=player.GetComponent<BodyRecovery>(); enemyBody=enemy.GetComponent<BodyRecovery>();
            foreach(var e in encounter.enemies)e.enabled=false;
            player.SetTestInput(Vector2.zero); yield return new WaitForSeconds(.1f);
        }
        void Place(float distance)
        {
            enemy.transform.position=new Vector3(0,0,1);
            var cc=player.GetComponent<CharacterController>();cc.enabled=false;
            player.transform.position=enemy.transform.position+Vector3.back*distance+Vector3.up*.03f;cc.enabled=true;
            player.visual.rotation=Quaternion.identity;Physics.SyncTransforms();
        }
        [UnityTest] public IEnumerator HeavyInterruptsEnemyFallsGetsUpAndCannotBeStunLocked()
        {
            Place(1); enemy.enabled=true;
            float deadline=Time.time+1.2f;
            while(encounter.Owner!=enemy&&Time.time<deadline)yield return null;
            Assert.That(encounter.Owner,Is.SameAs(enemy));
            Assert.That(enemy.warning.enabled,Is.True);
            combat.Queue(CombatIntent.Heavy);
            deadline=Time.time+1.2f;
            while(enemyBody.State==RecoveryState.Standing&&Time.time<deadline)yield return null;
            Assert.That(enemyBody.State,Is.EqualTo(RecoveryState.Falling)); Assert.That(enemy.target.Alive,Is.True);
            Assert.That(encounter.Owner,Is.Null); Assert.That(enemy.warning.enabled,Is.False);
            foreach(var collider in enemy.GetComponentsInChildren<Collider>())Assert.That(collider.enabled,Is.False);
            int health=enemy.target.Health,hits=enemy.target.HitCount;
            Assert.That(enemy.target.ReceiveHit(combat.heavy,Vector3.forward),Is.False);
            yield return new WaitForSeconds(.55f); Assert.That(enemyBody.State,Is.EqualTo(RecoveryState.Down));
            Assert.That(enemy.target.ReceiveHit(combat.lights[0],Vector3.forward),Is.False);
            yield return new WaitForSeconds(.45f); Assert.That(enemyBody.State,Is.EqualTo(RecoveryState.GettingUp));
            deadline=Time.time+1.2f;
            while(enemyBody.Busy&&Time.time<deadline)yield return null;
            Assert.That(enemyBody.State,Is.EqualTo(RecoveryState.Standing));
            Assert.That(enemyBody.Protected,Is.True); Assert.That(enemy.target.ReceiveHit(combat.heavy,Vector3.forward),Is.False);
            Assert.That(enemy.target.Health,Is.EqualTo(health));Assert.That(enemy.target.HitCount,Is.EqualTo(hits));
            foreach(var collider in enemy.GetComponentsInChildren<Collider>())Assert.That(collider.enabled,Is.True);
            yield return new WaitForSeconds(.5f); Assert.That(enemyBody.Protected,Is.False);
            Assert.That(enemy.target.ReceiveHit(combat.lights[0],Vector3.forward),Is.True);
        }
        [UnityTest] public IEnumerator PlayerKnockdownCancelsAttackBlocksInputAndReturnsControlWithProtection()
        {
            var cc=player.GetComponent<CharacterController>();float height=cc.height;Vector3 center=cc.center;
            combat.Queue(CombatIntent.Light);yield return new WaitForSeconds(.04f);
            Assert.That(defense.ReceiveDamage(12,Vector3.zero,true),Is.True);
            Assert.That(combat.Attacking,Is.False); Assert.That(playerBody.State,Is.EqualTo(RecoveryState.Falling));
            Assert.That(cc.enabled,Is.True);Assert.That(cc.height,Is.LessThan(height));Assert.That(defense.TryEvade(Vector2.right),Is.False);
            Vector3 start=player.transform.position;player.SetTestInput(Vector2.right,true,true);combat.Queue(CombatIntent.Heavy);
            yield return new WaitForSeconds(.2f);
            Assert.That(combat.Attacking,Is.False);Assert.That(player.transform.position.x,Is.EqualTo(start.x).Within(.02f));
            Assert.That(player.Grounded,Is.True);Assert.That(defense.ReceiveDamage(12,Vector3.zero,true),Is.False);
            float deadline=Time.time+2;
            while(playerBody.Busy&&Time.time<deadline)yield return null;
            yield return null;
            Assert.That(playerBody.State,Is.EqualTo(RecoveryState.Standing));Assert.That(defense.Locked,Is.False);
            Assert.That(playerBody.Protected,Is.True);Assert.That(player.State,Is.EqualTo("Run"));
            Assert.That(cc.height,Is.EqualTo(height));Assert.That(cc.center,Is.EqualTo(center));
            Assert.That(defense.ReceiveDamage(12,Vector3.zero,true),Is.False);
            yield return new WaitForSeconds(.5f);Assert.That(defense.ReceiveDamage(12,Vector3.zero),Is.True);
        }
        [UnityTest] public IEnumerator LethalHitKeepsWholeBodiesDownAndResetRestoresCollisionAndInput()
        {
            Assert.That(defense.ReceiveDamage(100,Vector3.zero),Is.True);
            var lethal=Object.Instantiate(combat.lights[0]);lethal.damage=200;
            Assert.That(enemy.target.ReceiveHit(lethal,Vector3.zero),Is.True);Object.Destroy(lethal);
            yield return new WaitForSeconds(.7f);
            Assert.That(playerBody.State,Is.EqualTo(RecoveryState.Dead));Assert.That(enemyBody.State,Is.EqualTo(RecoveryState.Dead));
            Assert.That(player.animator.GetCurrentAnimatorStateInfo(0).IsName("Death"),Is.True);
            Assert.That(player.animator.GetBoneTransform(HumanBodyBones.Head).position.y,Is.LessThan(.5f));
            Assert.That(Quaternion.Angle(enemy.visual.Find("Rig").localRotation,Quaternion.identity),Is.GreaterThan(85));
            foreach(var collider in enemy.GetComponentsInChildren<Collider>())Assert.That(collider.enabled,Is.False);
            combat.Queue(CombatIntent.Light);player.SetTestInput(Vector2.right,true,true);yield return new WaitForSeconds(.6f);
            Assert.That(combat.Attacking,Is.False);Assert.That(defense.TryEvade(Vector2.right),Is.False);
            encounter.ResetEncounter();yield return null;
            Assert.That(defense.Health,Is.EqualTo(defense.maxHealth));Assert.That(enemy.target.Health,Is.EqualTo(enemy.target.maxHealth));
            Assert.That(playerBody.Busy||enemyBody.Busy||playerBody.Protected||enemyBody.Protected,Is.False);
            foreach(var collider in enemy.GetComponentsInChildren<Collider>())Assert.That(collider.enabled,Is.True);
            Assert.That(player.animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"),Is.True);
            player.SetTestInput(Vector2.right);yield return new WaitForSeconds(.15f);Assert.That(player.State,Is.EqualTo("Walk"));
        }
        [UnityTest] public IEnumerator ResetDuringEveryRecoveryPhaseRestoresBodiesAndChangingOpponentCountClearsState()
        {
            foreach(float delay in new[]{.1f,.55f,1.05f})
            {
                encounter.ResetEncounter();Assert.That(enemy.target.ReceiveHit(combat.heavy,Vector3.zero),Is.True);
                Assert.That(defense.ReceiveDamage(12,Vector3.zero,true),Is.True);
                yield return new WaitForSeconds(delay); Assert.That(enemyBody.Busy&&playerBody.Busy,Is.True);
                encounter.ResetEncounter();yield return null;
                Assert.That(enemyBody.State,Is.EqualTo(RecoveryState.Standing));Assert.That(playerBody.State,Is.EqualTo(RecoveryState.Standing));
                Assert.That(enemy.target.Hitstun,Is.Zero);Assert.That(defense.Locked,Is.False);Assert.That(combat.Attacking,Is.False);
            }
            encounter.SetMode(2);var other=encounter.enemies[1];other.enabled=false;
            other.target.ReceiveHit(combat.heavy,Vector3.zero);yield return new WaitForSeconds(.1f);
            encounter.SetMode(1);encounter.SetMode(2);yield return null;
            Assert.That(other.GetComponent<BodyRecovery>().State,Is.EqualTo(RecoveryState.Standing));
            foreach(var collider in other.GetComponentsInChildren<Collider>())Assert.That(collider.enabled,Is.True);
        }
    }
}
