using UnityEngine;

namespace WombatLab
{
    [RequireComponent(typeof(CharacterController), typeof(LabInput))]
    public sealed class PlayerMotor : MonoBehaviour
    {
        public CharacterDefinition definition;
        public Transform visual;
        public Animator animator;
        public string State { get; private set; } = "Idle";
        public bool Grounded { get; private set; }
        public float VerticalVelocity { get; private set; }
        public bool ShowDebug { get; private set; }
        public Vector2 MovementIntent => testInputEnabled ? testInput.Move : input.Read().Move;

        CharacterController controller;
        LabInput input;
        CombatController combat;
        PlayerDefense defense;
        Vector3 spawn;
        Quaternion spawnFacing;
        float coyoteRemaining, bufferRemaining, landingRemaining;
        string requestedAnimation;
        bool testInputEnabled;
        InputFrame testInput;

        void Awake()
        {
            controller = GetComponent<CharacterController>(); input = GetComponent<LabInput>();
            combat = GetComponent<CombatController>();
            defense = GetComponent<PlayerDefense>();
            spawn = transform.position;
            if (definition == null || animator == null || visual == null)
            { Debug.LogError("PlayerMotor requires definition, visual and Animator", this); enabled = false; return; }
            spawnFacing = visual.localRotation;
        }

        void Update()
        {
            var frame = testInputEnabled ? testInput : input.Read();
            if (testInputEnabled) testInput = new InputFrame(testInput.Move);
            if (frame.Restart) { ResetToSpawn(); return; }
            if (frame.Debug) ShowDebug = !ShowDebug;
            // Hitstop never suspends an airborne trajectory. Ground attacks still commit.
            if (combat != null && combat.Frozen && Grounded) return;
            if (combat != null && combat.Attacking)
                frame = new InputFrame(Grounded && !combat.MovementReleased ? Vector2.zero : frame.Move);
            if (defense != null && (defense.Evading || defense.Locked)) frame = new InputFrame(Vector2.zero);
            Tick(frame, Time.deltaTime);
        }

        void Tick(InputFrame frame, float dt)
        {
            if (dt <= 0) return;
            Grounded = controller.isGrounded;
            coyoteRemaining = Grounded ? definition.coyoteTime : Mathf.Max(0, coyoteRemaining - dt);
            bufferRemaining = frame.Jump ? definition.jumpBuffer : Mathf.Max(0, bufferRemaining - dt);
            landingRemaining = Mathf.Max(0, landingRemaining - dt);

            bool jumping = bufferRemaining > 0 && coyoteRemaining > 0 && landingRemaining <= 0;
            if (jumping)
            {
                VerticalVelocity = definition.jumpSpeed; Grounded = false;
                coyoteRemaining = 0; bufferRemaining = 0;
            }
            else if (Grounded && VerticalVelocity < 0) VerticalVelocity = -2;

            var move = MotorMath.PlanarInput(frame.Move);
            float speed = definition.moveSpeed * (Grounded ? 1 : definition.airControl);
            var delta = move * speed * dt;
            if (defense != null) delta += defense.Motion * dt;
            var desired = MotorMath.ClampGround(transform.position + delta, definition.arenaMin, definition.arenaMax);
            delta.x = desired.x - transform.position.x; delta.z = desired.z - transform.position.z;
            delta.y = MotorMath.VerticalDisplacement(VerticalVelocity, definition.gravity, dt);
            VerticalVelocity -= definition.gravity * dt;

            bool wasAirborne = !Grounded;
            var beforeMove = transform.position;
            var collision = controller.Move(delta);
            float actualSpeed = Vector3.ProjectOnPlane(transform.position - beforeMove, Vector3.up).magnitude / dt;
            Grounded = (collision & CollisionFlags.Below) != 0;
            if (Grounded && VerticalVelocity < 0)
            {
                VerticalVelocity = -2;
                if (wasAirborne && !jumping)
                {
                    landingRemaining = definition.landingRecovery;
                    if (combat != null && combat.Attacking && combat.Attack.airborne) combat.Cancel();
                }
            }
            if ((collision & CollisionFlags.Above) != 0 && VerticalVelocity > 0) VerticalVelocity = 0;

            if (move.sqrMagnitude > .001f && (combat == null || !combat.Attacking || combat.MovementReleased))
                visual.rotation = Quaternion.Slerp(visual.rotation, Quaternion.LookRotation(move),
                    1 - Mathf.Exp(-definition.turnSpeed * dt));

            State = !Grounded ? (VerticalVelocity > 0 ? "Jump" : "Fall")
                : landingRemaining > 0 ? "Land" : actualSpeed > .1f ? "Walk" : "Idle";
            if (combat == null || !combat.Attacking)
                animator.speed = State == "Walk" ? Mathf.Clamp(actualSpeed / definition.moveSpeed, .3f, 1.35f) : 1;
            if ((combat == null || !combat.Attacking) && requestedAnimation != State)
            {
                animator.CrossFadeInFixedTime(State, .065f); requestedAnimation = State;
            }
        }

        public void ResetToSpawn()
        {
            combat?.ResetCombat();
            defense?.ResetDefense();
            controller.enabled = false; transform.position = spawn; controller.enabled = true;
            visual.localRotation = spawnFacing; VerticalVelocity = -2;
            coyoteRemaining = bufferRemaining = landingRemaining = 0;
            State = "Idle"; requestedAnimation = null;
            testInput = new InputFrame(Vector2.zero); animator.Play("Idle", 0, 0);
        }

        // Deterministic test hook: injected intent goes through the same motor,
        // grounding, animation and boundary logic as real input.
        public void SetTestInput(Vector2 move, bool jump = false)
        { testInputEnabled = true; testInput = new InputFrame(move, jump); }
        public void ReleaseTestInput() { testInputEnabled = false; }
        public void ClearJumpBuffer() { bufferRemaining = coyoteRemaining = 0; }
        public void ResumeLocomotion() { requestedAnimation = null; }
        public void MoveAttackStep(Vector3 delta)
        {
            var desired = MotorMath.ClampGround(transform.position + delta, definition.arenaMin, definition.arenaMax);
            controller.Move(Vector3.ProjectOnPlane(desired - transform.position, Vector3.up));
        }
    }
}
