using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace WombatLab.Tests
{
    public class Lf2CombatPlayTests
    {
        EngagementCoordinator encounter;
        PlayerMotor player;
        CombatController combat;
        PlayerDefense defense;
        [UnitySetUp] public IEnumerator Setup()
        {
            SceneManager.LoadScene("CrowdCombatLab"); yield return null;
            encounter = Object.FindAnyObjectByType<EngagementCoordinator>(); player = encounter.player;
            combat = player.GetComponent<CombatController>(); defense = player.GetComponent<PlayerDefense>();
            encounter.SetMode(8); foreach (var e in encounter.enemies) e.enabled = false;
            player.SetTestInput(Vector2.zero); PlacePlayer(Vector3.zero);
            for (int i = 0; i < encounter.enemies.Length; i++) encounter.enemies[i].transform.position = new Vector3(5,0,-1.5f + i * .3f);
            yield return new WaitForSeconds(.12f);
        }
        void PlacePlayer(Vector3 position)
        {
            var controller = player.GetComponent<CharacterController>(); controller.enabled = false;
            player.transform.position = position + Vector3.up * .03f; controller.enabled = true;
            player.visual.rotation = Quaternion.LookRotation(Vector3.right); Physics.SyncTransforms();
        }
        IEnumerator Finish(float limit = 3)
        { float until = Time.time + limit; while (combat.Attacking && Time.time < until) yield return null; Assert.That(combat.Attacking, Is.False); }
        [UnityTest] public IEnumerator SmashHitsOnLandingOnceInRadiusAndIsCancelledByDamage()
        {
            for (int i = 0; i < 3; i++) encounter.enemies[i].transform.position = new Vector3(i - 1,0,1.25f);
            encounter.enemies[3].transform.position = new Vector3(3.2f,0,0);
            player.SetTestInput(Vector2.zero, true); yield return new WaitForSeconds(.16f);
            combat.Queue(CombatIntent.Heavy); yield return new WaitForSeconds(.1f);
            Assert.That(combat.WaitingForImpact, Is.True);
            for (int i = 0; i < 3; i++) Assert.That(encounter.enemies[i].target.HitCount, Is.Zero, "No midair AOE");
            yield return Finish();
            for (int i = 0; i < 3; i++) { Assert.That(encounter.enemies[i].target.HitCount, Is.EqualTo(1)); Assert.That(encounter.enemies[i].target.Alive, Is.False); }
            Assert.That(encounter.enemies[3].target.HitCount, Is.Zero);
            Assert.That(combat.LastGroupHits, Is.EqualTo(3));
            encounter.enemies[0].ResetEnemy(); encounter.enemies[0].transform.position = new Vector3(0,0,1.2f);
            player.SetTestInput(Vector2.zero, true); yield return new WaitForSeconds(.16f);
            combat.Queue(CombatIntent.Heavy); yield return new WaitForSeconds(.08f);
            Assert.That(defense.ReceiveDamage(1, Vector3.zero), Is.True);
            yield return new WaitForSeconds(.8f);
            Assert.That(encounter.enemies[0].target.HitCount, Is.Zero, "Interrupted smash cannot fire on later landing");
        }
        [UnityTest] public IEnumerator SmashOverBodiesLandsOnFloorInsteadOfEnemyHeads()
        {
            player.SetTestInput(Vector2.zero, true); yield return new WaitForSeconds(.16f);
            encounter.enemies[0].transform.position = new Vector3(0,0,0);
            encounter.enemies[1].transform.position = new Vector3(.9f,0,.3f);
            Physics.SyncTransforms(); combat.Queue(CombatIntent.Heavy);
            float until = Time.time + 2;
            while (encounter.enemies[0].target.HitCount == 0 && Time.time < until) yield return null;
            Assert.That(encounter.enemies[0].target.HitCount, Is.EqualTo(1));
            Assert.That(encounter.enemies[1].target.HitCount, Is.EqualTo(1));
            Assert.That(player.transform.position.y, Is.LessThan(.15f), "Smash must resolve on the floor, never on another fighter");
            yield return Finish();
            Assert.That(Physics.GetIgnoreCollision(player.GetComponent<CharacterController>(), encounter.enemies[0].GetComponent<CapsuleCollider>()), Is.False);
        }
        [UnityTest] public IEnumerator SpecialsRespectMpAndBreakthroughPassesLightButStopsAtHeavy()
        {
            combat.RestoreEnergy(0); combat.Queue(CombatIntent.Wave); yield return null;
            Assert.That(combat.Attacking, Is.False); Assert.That(CombatProjectile.ActiveCount, Is.Zero);
            combat.Queue(CombatIntent.Light); yield return null; Assert.That(combat.Attacking, Is.True); yield return Finish();
            combat.RestoreEnergy(100);
            encounter.enemies[0].transform.position = new Vector3(.95f,0,0);
            encounter.enemies[1].transform.position = new Vector3(1.85f,0,0);
            encounter.enemies[6].transform.position = new Vector3(2.7f,0,0);
            Physics.SyncTransforms(); combat.Queue(CombatIntent.Charge); yield return null;
            Assert.That(combat.Energy, Is.EqualTo(82).Within(.1f));
            yield return Finish();
            Assert.That(encounter.enemies[0].target.HitCount, Is.EqualTo(1));
            Assert.That(encounter.enemies[1].target.HitCount, Is.EqualTo(1));
            Assert.That(player.transform.position.x, Is.GreaterThan(1.5f).And.LessThan(2.8f));
            Assert.That(Physics.GetIgnoreCollision(player.GetComponent<CharacterController>(), encounter.enemies[0].GetComponent<CapsuleCollider>()), Is.False);
        }
        [UnityTest] public IEnumerator WavePiercesThreeTargetsButCannotPassWallOrDamageTeam()
        {
            for (int i = 0; i < 4; i++) encounter.enemies[i].transform.position = new Vector3(1.5f + i,0,0);
            encounter.enemies[1].target.team = combat.team;
            var wall = GameObject.CreatePrimitive(PrimitiveType.Cube); wall.transform.position = new Vector3(3.1f,1,0); wall.transform.localScale = new Vector3(.15f,2,1);
            Physics.SyncTransforms(); combat.Queue(CombatIntent.Wave);
            yield return new WaitForSeconds(.9f);
            Assert.That(encounter.enemies[0].target.HitCount, Is.EqualTo(1)); Assert.That(encounter.enemies[1].target.HitCount, Is.Zero);
            Assert.That(encounter.enemies[2].target.HitCount, Is.Zero, "Wall blocks projectile");
            Object.Destroy(wall); encounter.ResetEncounter(); foreach (var e in encounter.enemies) e.enabled = false;
            encounter.enemies[1].target.team = 2; PlacePlayer(Vector3.zero);
            for (int i = 0; i < 4; i++) encounter.enemies[i].transform.position = new Vector3(1.5f + i,0,0);
            Physics.SyncTransforms(); combat.Queue(CombatIntent.Wave); yield return new WaitForSeconds(1.1f);
            for (int i = 0; i < 3; i++) Assert.That(encounter.enemies[i].target.HitCount, Is.EqualTo(1));
            Assert.That(encounter.enemies[3].target.HitCount, Is.Zero, "Finite piercing budget");
        }
        [UnityTest] public IEnumerator CrowdUsesTwoMeleeAttackersAndThrowerCanBeSidestepped()
        {
            encounter.enemies[0].transform.position = new Vector3(1.2f,0,.15f);
            encounter.enemies[1].transform.position = new Vector3(-1.2f,0,-.15f);
            encounter.enemies[0].enabled = encounter.enemies[1].enabled = true;
            float until = Time.time + 5; int peak = 0;
            while (Time.time < until)
            { peak = Mathf.Max(peak, encounter.ActiveAttackers); Assert.That(encounter.ActiveAttackers, Is.LessThanOrEqualTo(2)); defense.ResetDefense(); yield return null; }
            Assert.That(peak, Is.EqualTo(2), "Crowd must not queue behind a single global token");
            encounter.ResetEncounter(); foreach (var e in encounter.enemies) e.enabled = false; PlacePlayer(Vector3.zero);
            var thrower = encounter.enemies[5]; thrower.transform.position = new Vector3(4,0,0); thrower.enabled = true;
            until = Time.time + 5; while (thrower.State != "TELEGRAPH" && Time.time < until) yield return null;
            Assert.That(thrower.State, Is.EqualTo("TELEGRAPH")); Assert.That(thrower.chargeWarning.gameObject.activeSelf, Is.True);
            Assert.That(thrower.Opponent, Is.EqualTo(player.GetComponent<FighterTarget>()));
            PlacePlayer(new Vector3(0,0,1.5f)); yield return new WaitForSeconds(1.6f);
            Assert.That(defense.Health, Is.EqualTo(defense.maxHealth), "Locked announced throw can be avoided");
        }
        [UnityTest] public IEnumerator TouchWaveUsesSharedInputAndCheckpointRestoresMp()
        {
            var pad = InputSystem.AddDevice<Gamepad>();
            try
            {
                InputSystem.QueueStateEvent(pad, new GamepadState().WithButton(GamepadButton.DpadRight)); yield return null;
                Assert.That(player.GetComponent<LabInput>().Read().Wave, Is.True);
                InputSystem.QueueStateEvent(pad, new GamepadState()); yield return null;
            }
            finally { InputSystem.RemoveDevice(pad); }
            Assert.That(player.GetComponentsInChildren<UnityEngine.InputSystem.OnScreen.OnScreenButton>(true),
                Has.Some.Matches<UnityEngine.InputSystem.OnScreen.OnScreenButton>(b => b.controlPath == "<Gamepad>/dpad/right"));
            SceneManager.LoadScene("JunkyardChapter"); yield return null;
            var chapter = Object.FindAnyObjectByType<JunkyardChapter>(); chapter.session.StartRun(false); yield return null;
            var c = chapter.encounter.player.GetComponent<CombatController>(); c.RestoreEnergy(12);
            chapter.RetryCheckpoint(); Assert.That(c.Energy, Is.EqualTo(c.maxEnergy));
            chapter.session.Pause(); float mp = c.Energy; yield return new WaitForSecondsRealtime(.15f);
            Assert.That(c.Energy, Is.EqualTo(mp));
        }
        [UnityTearDown] public IEnumerator Cleanup()
        { Time.timeScale = 1; AudioListener.pause = false; CombatProjectile.ClearAll(); yield return null; }
    }
}
