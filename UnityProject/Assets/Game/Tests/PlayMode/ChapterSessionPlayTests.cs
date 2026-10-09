using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace WombatLab.Tests
{
    public class ChapterSessionPlayTests
    {
        ChapterSession session;
        PlayerMotor player;
        CombatController combat;
        PlayerDefense defense;
        float savedVolume, savedFov;
        [UnitySetUp] public IEnumerator Setup()
        {
            savedVolume = PlayerPrefs.GetFloat(ChapterSession.PrefPrefix + "Volume", 1);
            savedFov = PlayerPrefs.GetFloat(ChapterSession.PrefPrefix + "Fov", 42);
            SceneManager.LoadScene("JunkyardChapter"); yield return null; yield return null;
            session = Object.FindAnyObjectByType<ChapterSession>();
            player = session.chapter.encounter.player; combat = player.GetComponent<CombatController>(); defense = player.GetComponent<PlayerDefense>();
        }
        [UnityTearDown] public IEnumerator Cleanup()
        {
            player.ReleaseTestInput();
            PlayerPrefs.SetFloat(ChapterSession.PrefPrefix + "Volume", savedVolume);
            PlayerPrefs.SetFloat(ChapterSession.PrefPrefix + "Fov", savedFov); PlayerPrefs.Save();
            SceneManager.LoadScene("HumanoidCombatLab"); yield return null;
            Assert.That(Time.timeScale, Is.EqualTo(1)); Assert.That(AudioListener.pause, Is.False);
        }
        [UnityTest] public IEnumerator MenuPauseFreezesRealAttackAndReleasesHeldTouch()
        {
            Assert.That(session.Page, Is.EqualTo(SessionPage.Home)); Assert.That(Time.timeScale, Is.Zero);
            Assert.That(session.chapter.encounter.LivingCount, Is.Zero);
            session.StartRun(false); yield return new WaitForSeconds(.15f);
            player.SetTestInput(Vector2.right, run:true);
            while (session.chapter.Phase == ChapterPhase.Arrival) yield return null;
            player.SetTestInput(Vector2.zero);
            while (session.chapter.encounter.LivingCount == 0) yield return null;
            combat.Queue(CombatIntent.Light); yield return new WaitForSeconds(.08f);
            var touch = player.GetComponent<MobileTouchControls>(); touch.SetVisible(true);
            var virtualPad = (Gamepad)touch.Stick.control.device;
            InputSystem.QueueStateEvent(virtualPad, new GamepadState { leftStick = Vector2.right }.WithButton(GamepadButton.West));
            yield return null;
            session.Pause();
            var position = player.transform.position; float progress = combat.Progress, clock = Time.time;
            int health = defense.Health, instance = combat.AttackInstance;
            var enemy = session.chapter.encounter.enemies[0]; var enemyPosition = enemy.transform.position; string enemyState = enemy.State;
            yield return new WaitForSecondsRealtime(.35f);
            Assert.That(Time.time, Is.EqualTo(clock)); Assert.That(player.transform.position, Is.EqualTo(position));
            Assert.That(combat.Progress, Is.EqualTo(progress)); Assert.That(defense.Health, Is.EqualTo(health));
            Assert.That(enemy.transform.position, Is.EqualTo(enemyPosition)); Assert.That(enemy.State, Is.EqualTo(enemyState));
            Assert.That(AudioListener.pause, Is.True); Assert.That(player.GetComponent<LabInput>().Read().Move, Is.EqualTo(Vector2.zero));
            session.Resume(); yield return new WaitForSeconds(.7f);
            Assert.That(combat.AttackInstance, Is.EqualTo(instance), "Held touch/menu click must not chain another attack");
            Assert.That(player.GetComponent<LabInput>().Read().Move, Is.EqualTo(Vector2.zero)); Assert.That(AudioListener.pause, Is.False);
        }
        [UnityTest] public IEnumerator DefeatRetryOptionsAndGamepadPauseUseSameSession()
        {
            session.StartRun(false); yield return new WaitForSeconds(.1f);
            var pad = InputSystem.AddDevice<Gamepad>();
            try
            {
                InputSystem.QueueStateEvent(pad, new GamepadState().WithButton(GamepadButton.Start)); yield return null; yield return null;
                Assert.That(session.Page, Is.EqualTo(SessionPage.Pause), "Start pauses instead of resetting the chapter");
                InputSystem.QueueStateEvent(pad, new GamepadState()); yield return null;
                session.OpenOptions(); session.SetVolume(.35f); session.SetCameraSize(46);
                Assert.That(AudioListener.volume, Is.EqualTo(.35f)); Assert.That(PlayerPrefs.GetFloat(ChapterSession.PrefPrefix + "Fov"), Is.EqualTo(46));
                session.GoHome(); session.Resume(); yield return new WaitForSeconds(.1f);
                defense.ReceiveDamage(1000, Vector3.zero, true); yield return null; yield return null;
                Assert.That(session.Page, Is.EqualTo(SessionPage.Result)); Assert.That(Time.timeScale, Is.Zero);
                var retry = GameObject.Find("B8 Chapter Session UI").transform.Find("Safe area/Menu overlay/Menu card/Primary").GetComponent<Button>();
                retry.onClick.Invoke(); yield return null;
                Assert.That(session.Page, Is.EqualTo(SessionPage.Playing)); Assert.That(defense.Health, Is.EqualTo(100));
                Assert.That(session.chapter.Phase, Is.EqualTo(ChapterPhase.Arrival)); Assert.That(combat.Attacking, Is.False);
            }
            finally { InputSystem.RemoveDevice(pad); }
        }
    }
}
