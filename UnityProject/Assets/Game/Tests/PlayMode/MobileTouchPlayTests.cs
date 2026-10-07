using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.OnScreen;

namespace WombatLab.Tests
{
    public class MobileTouchPlayTests : InputTestFixture
    {
        GameObject root;
        LabInput input;
        MobileTouchControls hud;
        Mouse mouse;
        Keyboard keyboard;
        public override void Setup()
        {
            base.Setup(); mouse = InputSystem.AddDevice<Mouse>(); keyboard = InputSystem.AddDevice<Keyboard>();
            root = new GameObject("Touch input integration"); input = root.AddComponent<LabInput>();
            hud = root.AddComponent<MobileTouchControls>(); hud.SetVisible(true);
            Canvas.ForceUpdateCanvases(); InputSystem.Update();
        }
        public override void TearDown() { Object.DestroyImmediate(root); base.TearDown(); }
        OnScreenButton Action(string name)
        {
            foreach (var control in root.GetComponentsInChildren<OnScreenButton>()) if (control.name == name) return control;
            Assert.Fail("Missing touch action: " + name); return null;
        }
        PointerEventData Pointer(int id, Vector2 position) => new PointerEventData(EventSystem.current) { pointerId = id, position = position };

        [Test] public void StickAndSeparateAttackPointerProduceMovementAndOnePress()
        {
            var center = RectTransformUtility.WorldToScreenPoint(null, hud.Stick.transform.position);
            var drag = Pointer(11, center); hud.Stick.OnPointerDown(drag);
            drag.position += Vector2.right * 200; hud.Stick.OnDrag(drag);
            var attack = Action("Light"); var finger = Pointer(22, Vector2.zero); attack.OnPointerDown(finger);
            InputSystem.Update(); var frame = input.Read();
            Assert.That(frame.Move.x, Is.GreaterThan(.9f)); Assert.That(frame.Run, Is.True); Assert.That(frame.Light, Is.True);
            Assert.That(input.Read().Light, Is.True, "All gameplay consumers see the same press in this input update");
            InputSystem.Update(); Assert.That(input.Read().Light, Is.False, "Holding does not repeatedly enqueue attacks");
            attack.OnPointerUp(finger); hud.Stick.OnPointerUp(drag); InputSystem.Update();
            Assert.That(input.Read().Move, Is.EqualTo(Vector2.zero)); Assert.That(input.Read().Run, Is.False);
        }
        [Test] public void OtherActionsDoNotAlsoTriggerMouseLightAndDesktopClicksReturnWhenHidden()
        {
            Press(mouse.leftButton); var finger = Pointer(33, Vector2.zero);
            foreach (string name in new[] { "Heavy", "Kick", "Jump", "Evade", "Restart", "Charge" })
            {
                var button = Action(name); button.OnPointerDown(finger); InputSystem.Update(); var frame = input.Read();
                Assert.That(frame.Light, Is.False, "UI/emulated mouse must not also enqueue a light attack");
                Assert.That(name == "Heavy" ? frame.Heavy : name == "Kick" ? frame.Kick : name == "Jump" ? frame.Jump : name == "Evade" ? frame.Evade : name == "Charge" ? frame.Charge : frame.Restart, Is.True, name);
                button.OnPointerUp(finger); InputSystem.Update();
            }
            Release(mouse.leftButton); hud.SetVisible(false); Set(mouse.position, Vector2.zero);
            Press(mouse.leftButton); Assert.That(input.Read().Light, Is.True);
            Release(mouse.leftButton); Press(keyboard.kKey); Assert.That(input.Read().Heavy, Is.True);
        }
        [Test] public void FocusCancellationAndHidingReleaseHeldVirtualControls()
        {
            var center = RectTransformUtility.WorldToScreenPoint(null, hud.Stick.transform.position);
            var drag = Pointer(44, center); hud.Stick.OnPointerDown(drag); drag.position += Vector2.right * 200; hud.Stick.OnDrag(drag);
            Action("Light").OnPointerDown(Pointer(55, Vector2.zero)); InputSystem.Update();
            Assert.That(input.Read().Move.x, Is.GreaterThan(.9f));
            hud.ReleasePointers(); InputSystem.Update();
            Assert.That(input.Read().Move, Is.EqualTo(Vector2.zero)); Assert.That(input.Read().Light, Is.False); Assert.That(input.Read().Run, Is.False);
            hud.Stick.OnPointerDown(drag); drag.position += Vector2.up * 200; hud.Stick.OnDrag(drag); InputSystem.Update();
            hud.SetVisible(false); InputSystem.Update(); Assert.That(input.Read().Move, Is.EqualTo(Vector2.zero));
        }
    }
}
