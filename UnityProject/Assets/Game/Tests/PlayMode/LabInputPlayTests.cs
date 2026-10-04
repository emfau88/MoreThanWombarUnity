using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;

namespace WombatLab.Tests
{
    // Official fixture isolates tests from OS events, focus and real devices, and
    // restores the original input backend afterward. No production setting changes.
    public class LabInputPlayTests : InputTestFixture
    {
        GameObject inputObject;
        LabInput input;
        Keyboard keyboard;
        Gamepad pad;

        public override void Setup()
        {
            base.Setup();
            keyboard = InputSystem.AddDevice<Keyboard>(); pad = InputSystem.AddDevice<Gamepad>();
            inputObject = new GameObject("Isolated production LabInput under test");
            input = inputObject.AddComponent<LabInput>();
            InputSystem.Update();
        }

        public override void TearDown()
        {
            // Dispose the scene's action map while its isolated input system is still
            // active, before the fixture restores the user's native input backend.
            if (inputObject != null) Object.DestroyImmediate(inputObject);
            base.TearDown();
        }

        // Synchronous PlayMode tests exercise the real MonoBehaviour bindings without
        // reloading live scenes while the fixture's global input backend is swapped.
        [Test] public void KeyboardBindingProducesGameplayIntent()
        {
            Press(keyboard.dKey);
            Assert.That(input.Read().Move.x, Is.GreaterThan(.9f));
            Release(keyboard.dKey); Assert.That(input.Read().Move, Is.EqualTo(Vector2.zero));
        }

        [Test] public void GamepadBindingProducesGameplayIntent()
        {
            Set(pad.leftStick, Vector2.right);
            Assert.That(input.Read().Move.x, Is.GreaterThan(.9f));
            Set(pad.leftStick, Vector2.zero); Assert.That(input.Read().Move, Is.EqualTo(Vector2.zero));
        }

        [Test] public void KeyboardAndGamepadExposeCombatJumpAndResetIntents()
        {
            Press(keyboard.jKey); Assert.That(input.Read().Light, Is.True); Release(keyboard.jKey);
            Press(keyboard.kKey); Assert.That(input.Read().Heavy, Is.True); Release(keyboard.kKey);
            Press(keyboard.lKey); Assert.That(input.Read().Kick, Is.True); Release(keyboard.lKey);
            Press(keyboard.spaceKey); Assert.That(input.Read().Jump, Is.True); Release(keyboard.spaceKey);
            Press(keyboard.rKey); Assert.That(input.Read().Restart, Is.True); Release(keyboard.rKey);
            Press(keyboard.leftShiftKey); Assert.That(input.Read().Evade, Is.True); Release(keyboard.leftShiftKey);
            Press(pad.buttonWest); Assert.That(input.Read().Light, Is.True); Release(pad.buttonWest);
            Press(pad.buttonNorth); Assert.That(input.Read().Heavy, Is.True); Release(pad.buttonNorth);
            Press(pad.rightShoulder); Assert.That(input.Read().Kick, Is.True); Release(pad.rightShoulder);
            Press(pad.buttonSouth); Assert.That(input.Read().Jump, Is.True); Release(pad.buttonSouth);
            Press(pad.startButton); Assert.That(input.Read().Restart, Is.True); Release(pad.startButton);
            Press(pad.buttonEast); Assert.That(input.Read().Evade, Is.True); Release(pad.buttonEast);
        }
        [Test] public void RunIsHeldOnControlOrLeftTriggerWithoutChangingEvade()
        {
            Press(keyboard.leftCtrlKey); Assert.That(input.Read().Run, Is.True); Assert.That(input.Read().Evade, Is.False);
            InputSystem.Update(); Assert.That(input.Read().Run, Is.True);
            Release(keyboard.leftCtrlKey); Assert.That(input.Read().Run, Is.False);
            Set(pad.leftTrigger, 1); Assert.That(input.Read().Run, Is.True);
            Set(pad.leftTrigger, 0); Assert.That(input.Read().Run, Is.False);
            Press(keyboard.leftShiftKey); Assert.That(input.Read().Evade, Is.True); Assert.That(input.Read().Run, Is.False);
        }
    }
}
