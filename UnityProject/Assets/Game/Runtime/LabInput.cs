using UnityEngine;
using UnityEngine.InputSystem;

namespace WombatLab
{
    public readonly struct InputFrame
    {
        public readonly Vector2 Move;
        public readonly bool Jump, Restart, Debug, Light, Heavy, Evade, Kick, Run, Charge, Interact, ChapterRestart;
        public InputFrame(Vector2 move, bool jump = false, bool restart = false, bool debug = false, bool light = false, bool heavy = false, bool evade = false, bool kick = false, bool run = false, bool charge = false, bool interact = false, bool chapterRestart = false)
        { Move = move; Jump = jump; Restart = restart; Debug = debug; Light = light; Heavy = heavy; Evade = evade; Kick = kick; Run = run; Charge = charge; Interact = interact; ChapterRestart = chapterRestart; }
    }

    public sealed class LabInput : MonoBehaviour
    {
        InputActionMap map;
        InputAction move, jump, restart, debug, lightAction, heavyAction, evade, kick, run, charge, interact, chapterRestart;
        public MobileTouchControls TouchControls { get; set; }
        public ChapterSession Session { get; set; }
        public bool GameplayBlocked => Session != null && Session.Blocked;
        bool awaitRelease;
        public void ClearForMenu() { awaitRelease = true; TouchControls?.ReleasePointers(); }

        void Awake()
        {
            map = new InputActionMap("CombatLab");
            move = map.AddAction("Move", InputActionType.Value);
            move.expectedControlType = "Vector2";
            move.AddCompositeBinding("2DVector").With("Up", "<Keyboard>/w")
                .With("Down", "<Keyboard>/s").With("Left", "<Keyboard>/a").With("Right", "<Keyboard>/d");
            move.AddCompositeBinding("2DVector").With("Up", "<Keyboard>/upArrow")
                .With("Down", "<Keyboard>/downArrow").With("Left", "<Keyboard>/leftArrow").With("Right", "<Keyboard>/rightArrow");
            move.AddBinding("<Gamepad>/leftStick");
            jump = map.AddAction("Jump", InputActionType.Button);
            jump.AddBinding("<Keyboard>/space"); jump.AddBinding("<Gamepad>/buttonSouth");
            restart = map.AddAction("Restart", InputActionType.Button);
            restart.AddBinding("<Keyboard>/r"); restart.AddBinding("<Gamepad>/start");
            debug = map.AddAction("Debug", InputActionType.Button);
            debug.AddBinding("<Keyboard>/h"); debug.AddBinding("<Gamepad>/rightStickPress");
            lightAction = map.AddAction("Light", InputActionType.Button);
            lightAction.AddBinding("<Keyboard>/j"); lightAction.AddBinding("<Gamepad>/buttonWest");
            heavyAction = map.AddAction("Heavy", InputActionType.Button);
            heavyAction.AddBinding("<Keyboard>/k"); heavyAction.AddBinding("<Gamepad>/buttonNorth");
            evade = map.AddAction("Evade", InputActionType.Button);
            evade.AddBinding("<Keyboard>/leftShift"); evade.AddBinding("<Keyboard>/rightShift"); evade.AddBinding("<Gamepad>/buttonEast");
            kick = map.AddAction("Kick", InputActionType.Button);
            kick.AddBinding("<Keyboard>/l"); kick.AddBinding("<Gamepad>/rightShoulder");
            run = map.AddAction("Run", InputActionType.Button);
            run.AddBinding("<Keyboard>/leftCtrl"); run.AddBinding("<Keyboard>/rightCtrl"); run.AddBinding("<Gamepad>/leftTrigger");
            charge = map.AddAction("Charge", InputActionType.Button);
            charge.AddBinding("<Keyboard>/e"); charge.AddBinding("<Gamepad>/rightTrigger");
            interact = map.AddAction("Interact", InputActionType.Button);
            interact.AddBinding("<Keyboard>/f"); interact.AddBinding("<Gamepad>/leftShoulder");
            chapterRestart = map.AddAction("ChapterRestart", InputActionType.Button);
            chapterRestart.AddBinding("<Keyboard>/backspace"); chapterRestart.AddBinding("<Gamepad>/select");
        }

        void OnEnable() { map?.Enable(); }
        void OnDisable() { map?.Disable(); }
        void OnDestroy() { map?.Dispose(); }
        public InputFrame Read()
        {
            if (GameplayBlocked) return new InputFrame(Vector2.zero);
            if (awaitRelease)
            {
                bool held = move.ReadValue<Vector2>().sqrMagnitude > .01f || jump.IsPressed() || restart.IsPressed()
                    || lightAction.IsPressed() || heavyAction.IsPressed() || evade.IsPressed() || kick.IsPressed()
                    || run.IsPressed() || charge.IsPressed() || interact.IsPressed() || chapterRestart.IsPressed()
                    || Mouse.current?.leftButton.isPressed == true || Mouse.current?.rightButton.isPressed == true;
                if (!held) awaitRelease = false;
                return new InputFrame(Vector2.zero);
            }
            // A UI touch can also arrive as a mouse click in WebGL. Only gameplay pointer
            // clicks may attack; keyboard and physical/virtual gamepad bindings stay active.
            bool pointerAllowed = (TouchControls == null || !TouchControls.BlocksPointerAttacks)
                && (Touchscreen.current == null || !Touchscreen.current.primaryTouch.press.isPressed);
            var mouse = Mouse.current;
            return new InputFrame(move.ReadValue<Vector2>(), jump.WasPressedThisFrame(),
                Session == null && restart.WasPressedThisFrame(), debug.WasPressedThisFrame(),
                lightAction.WasPressedThisFrame() || (pointerAllowed && mouse != null && mouse.leftButton.wasPressedThisFrame),
                heavyAction.WasPressedThisFrame() || (pointerAllowed && mouse != null && mouse.rightButton.wasPressedThisFrame),
                evade.WasPressedThisFrame(), kick.WasPressedThisFrame(),
                run.IsPressed() || (TouchControls != null && TouchControls.Running), charge.WasPressedThisFrame(),
                interact.WasPressedThisFrame(), Session == null && chapterRestart.WasPressedThisFrame());
        }
    }
}
