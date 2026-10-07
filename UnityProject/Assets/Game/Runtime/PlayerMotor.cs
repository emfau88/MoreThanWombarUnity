using UnityEngine;

namespace WombatLab
{
    [RequireComponent(typeof(CharacterController), typeof(LabInput))]
    public sealed class PlayerMotor : MonoBehaviour
    {
        public CharacterDefinition definition;
        public Transform visual;
        public Animator animator;
        public JunkyardChapter chapter;
        public string State { get; private set; } = "Idle";
        public bool Grounded { get; private set; }
        public float VerticalVelocity { get; private set; }
        public bool ShowDebug { get; private set; }
        public Vector2 MovementIntent => testInputEnabled ? testInput.Move : input.Read().Move;

        CharacterController controller;
        LabInput input;
        CombatController combat;
        PlayerDefense defense;
        AnimationReaction reaction;
        BodyRecovery body;
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
            reaction = GetComponent<AnimationReaction>();
            body = GetComponent<BodyRecovery>();
            spawn = transform.position;
            if (definition == null || animator == null || visual == null)
            { Debug.LogError("PlayerMotor requires definition, visual and Animator", this); enabled = false; return; }
            spawnFacing = visual.localRotation;
        }

        void Update()
        {
            var frame = testInputEnabled ? testInput : input.Read();
            if (testInputEnabled) testInput = new InputFrame(testInput.Move, run: testInput.Run);
            if (frame.Restart) { if (chapter == null) ResetToSpawn(); return; }
            if (frame.Debug) ShowDebug = !ShowDebug;
            // Hitstop never suspends an airborne trajectory. Ground attacks still commit.
            if (combat != null && combat.Frozen && Grounded) return;
            if (combat != null && combat.Attacking)
                frame = new InputFrame(Grounded && !combat.MovementReleased ? Vector2.zero : frame.Move, run: frame.Run);
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
            bool running = frame.Run && definition.runSpeed > definition.moveSpeed && Grounded;
            float groundSpeed = running ? definition.runSpeed : definition.moveSpeed;
            float speed = Grounded ? groundSpeed : definition.moveSpeed * definition.airControl;
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
                : landingRemaining > 0 ? "Land" : actualSpeed > .1f ? (running ? "Run" : "Walk") : "Idle";
            bool locomotionOwnsPose = (combat == null || !combat.Attacking) && (reaction == null || !reaction.Active) && (body == null || !body.Busy);
            if (locomotionOwnsPose)
                animator.speed = State == "Walk" || State == "Run" ? Mathf.Clamp(actualSpeed / groundSpeed, .1f, 1.35f) : 1;
            if (locomotionOwnsPose && requestedAnimation != State)
            {
                animator.CrossFadeInFixedTime(State, .065f); requestedAnimation = State;
            }
        }

        public void ResetToSpawn()
        { ResetAt(spawn, visual.parent.rotation * spawnFacing); }

        public void ResetAt(Vector3 position, Quaternion facing)
        {
            combat?.ResetCombat();
            defense?.ResetDefense();
            reaction?.Clear();
            controller.enabled = false; transform.position = position; controller.enabled = true;
            visual.rotation = facing; VerticalVelocity = -2;
            coyoteRemaining = bufferRemaining = landingRemaining = 0;
            State = "Idle"; requestedAnimation = null;
            testInput = new InputFrame(Vector2.zero); animator.Play("Idle", 0, 0);
        }

        // Deterministic test hook: injected intent goes through the same motor,
        // grounding, animation and boundary logic as real input.
        public void SetTestInput(Vector2 move, bool jump = false, bool run = false)
        { testInputEnabled = true; testInput = new InputFrame(move, jump, run: run); }
        public void ReleaseTestInput() { testInputEnabled = false; }
        public void ClearJumpBuffer() { bufferRemaining = coyoteRemaining = 0; }
        public void ResumeLocomotion() { requestedAnimation = null; }
        public bool MoveAttackStep(Vector3 delta)
        {
            var desired = MotorMath.ClampGround(transform.position + delta, definition.arenaMin, definition.arenaMax);
            var limited = Vector3.ProjectOnPlane(desired - transform.position, Vector3.up);
            var flags = controller.Move(limited);
            return (flags & CollisionFlags.Sides) != 0 || (limited - Vector3.ProjectOnPlane(delta, Vector3.up)).sqrMagnitude > .000001f;
        }
    }
}
