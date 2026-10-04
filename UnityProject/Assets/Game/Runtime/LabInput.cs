using UnityEngine;
using UnityEngine.InputSystem;

namespace WombatLab
{
    public readonly struct InputFrame
    {
        public readonly Vector2 Move;
        public readonly bool Jump, Restart, Debug, Light, Heavy, Evade, Kick, Run;
        public InputFrame(Vector2 move, bool jump = false, bool restart = false, bool debug = false, bool light = false, bool heavy = false, bool evade = false, bool kick = false, bool run = false)
        { Move = move; Jump = jump; Restart = restart; Debug = debug; Light = light; Heavy = heavy; Evade = evade; Kick = kick; Run = run; }
    }

    public sealed class LabInput : MonoBehaviour
    {
        InputActionMap map;
        InputAction move, jump, restart, debug, lightAction, heavyAction, evade, kick, run;

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
            lightAction.AddBinding("<Keyboard>/j"); lightAction.AddBinding("<Mouse>/leftButton"); lightAction.AddBinding("<Gamepad>/buttonWest");
            heavyAction = map.AddAction("Heavy", InputActionType.Button);
            heavyAction.AddBinding("<Keyboard>/k"); heavyAction.AddBinding("<Mouse>/rightButton"); heavyAction.AddBinding("<Gamepad>/buttonNorth");
            evade = map.AddAction("Evade", InputActionType.Button);
            evade.AddBinding("<Keyboard>/leftShift"); evade.AddBinding("<Keyboard>/rightShift"); evade.AddBinding("<Gamepad>/buttonEast");
            kick = map.AddAction("Kick", InputActionType.Button);
            kick.AddBinding("<Keyboard>/l"); kick.AddBinding("<Gamepad>/rightShoulder");
            run = map.AddAction("Run", InputActionType.Button);
            run.AddBinding("<Keyboard>/leftCtrl"); run.AddBinding("<Keyboard>/rightCtrl"); run.AddBinding("<Gamepad>/leftTrigger");
        }

        void OnEnable() { map?.Enable(); }
        void OnDisable() { map?.Disable(); }
        void OnDestroy() { map?.Dispose(); }
        public InputFrame Read() => new InputFrame(move.ReadValue<Vector2>(), jump.WasPressedThisFrame(),
            restart.WasPressedThisFrame(), debug.WasPressedThisFrame(), lightAction.WasPressedThisFrame(), heavyAction.WasPressedThisFrame(), evade.WasPressedThisFrame(), kick.WasPressedThisFrame(), run.IsPressed());
    }
}
