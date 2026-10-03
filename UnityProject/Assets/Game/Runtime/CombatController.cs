using System.Collections.Generic;
using UnityEngine;

namespace WombatLab
{
    [DefaultExecutionOrder(-20)]
    [RequireComponent(typeof(PlayerMotor), typeof(LabInput))]
    public sealed class CombatController : MonoBehaviour
    {
        public AttackDefinition[] lights;
        public AttackDefinition heavy, kick, airKick, airHeavy;
        public Transform leftFist, rightFist, leftFoot, rightFoot;
        public Transform ContactPoint => Attack == null ? rightFist : Attack.foot
            ? (Attack.rightHand ? rightFoot : leftFoot) : (Attack.rightHand ? rightFist : leftFist);
        public bool MovementReleased => Attacking && !Attack.airborne && Progress >= Attack.moveRelease;
        public CombatFeedback feedback;
        public TrainingDummy dummy;
        public int team = 1;
        public AttackDefinition Attack { get; private set; }
        public bool Attacking => Attack != null;
        public bool Frozen => freezeRemaining > 0;
        public bool HitboxOpen { get; private set; }
        public int AttackInstance { get; private set; }
        public float Progress { get; private set; }
        public string Phase => !Attacking ? "READY" : Frozen ? "HITSTOP" :
            Progress < Attack.activeStart ? "STARTUP" : Progress <= Attack.activeEnd ? "ACTIVE" : "RECOVERY";

        readonly CombatBuffer buffer = new CombatBuffer();
        readonly HashSet<TrainingDummy> hitTargets = new HashSet<TrainingDummy>();
        readonly Collider[] contacts = new Collider[32];
        PlayerMotor motor;
        LabInput input;
        PlayerDefense defense;
        float freezeRemaining, previous;
        int lightIndex;
        Vector3 previousRoot;
        Quaternion startFacing;

        void Awake() { motor = GetComponent<PlayerMotor>(); input = GetComponent<LabInput>(); defense = GetComponent<PlayerDefense>(); }
        void Update()
        {
            var frame = input.Read();
            if (frame.Restart) { ResetCombat(); return; }
            if (defense != null && (defense.Evading || defense.Locked)) { buffer.Clear(); return; }
            if (frame.Heavy) Queue(CombatIntent.Heavy); else if (frame.Kick) Queue(CombatIntent.Kick); else if (frame.Light) Queue(CombatIntent.Light);
            bool wasFrozen = Frozen;
            freezeRemaining = Mathf.Max(0, freezeRemaining - Time.unscaledDeltaTime);
            motor.animator.speed = Frozen ? 0 : 1;
            buffer.Tick(wasFrozen ? 0 : Time.deltaTime);
            // A simultaneous Jump + attack starts after the motor takes off, not before it.
            if (!Frozen && !Attacking && !(frame.Jump && motor.Grounded) && buffer.Pending != CombatIntent.None)
            {
                var intent = buffer.Consume();
                Begin(!motor.Grounded ? (intent == CombatIntent.Heavy ? airHeavy : airKick)
                    : intent == CombatIntent.Heavy ? heavy : intent == CombatIntent.Kick ? kick : lights[0],
                    intent == CombatIntent.Light && motor.Grounded ? 0 : -1);
            }
        }

        public void Queue(CombatIntent intent) => buffer.Submit(intent);

        void Begin(AttackDefinition attack, int index)
        {
            if (attack == null) return;
            Attack = attack; lightIndex = index; AttackInstance++;
            hitTargets.Clear(); previous = Progress = 0; HitboxOpen = false;
            previousRoot = transform.position;
            // Begin and early startup share one turn budget, measured from
            // the original facing rather than an already corrected pose.
            startFacing = motor.visual.rotation;
            var move = MotorMath.PlanarInput(motor.MovementIntent);
            if (move.sqrMagnitude > .01f)
                motor.visual.rotation = Quaternion.RotateTowards(motor.visual.rotation, Quaternion.LookRotation(move), 25);
            motor.ClearJumpBuffer();
            motor.animator.Play(attack.stateName, 0, 0);
            motor.animator.Update(0);
        }

        void LateUpdate()
        {
            if (!Attacking || Frozen) { HitboxOpen = false; return; }
            var state = motor.animator.GetCurrentAnimatorStateInfo(0);
            if (!state.IsName(Attack.stateName)) { Cancel(); return; }
            Progress = Mathf.Clamp01(state.normalizedTime);
            if (!Attack.airborne && motor.Grounded)
            {
                float stepFrom = Mathf.SmoothStep(0, 1, Mathf.InverseLerp(.08f, .40f, previous));
                float stepTo = Mathf.SmoothStep(0, 1, Mathf.InverseLerp(.08f, .40f, Progress));
                motor.MoveAttackStep(motor.visual.forward * Attack.forwardStep * Mathf.Max(0, stepTo - stepFrom));
                if (Progress < .16f)
                {
                    var move = MotorMath.PlanarInput(motor.MovementIntent);
                    if (move.sqrMagnitude > .01f)
                        motor.visual.rotation = Quaternion.RotateTowards(motor.visual.rotation,
                            Quaternion.RotateTowards(startFacing, Quaternion.LookRotation(move), 25), 180 * Time.deltaTime);
                }
            }
            HitboxOpen = Progress >= Attack.activeStart && Progress <= Attack.activeEnd;
            if (Attack.ActiveCrossed(previous, Progress)) SweepActivePoses(previous, Progress);
            previous = Progress;
            previousRoot = transform.position;

            float chainStart = hitTargets.Count > 0 ? Attack.hitChainStart : Attack.chainStart;
            if (!Frozen && lightIndex >= 0 && buffer.Pending == CombatIntent.Light && lightIndex < lights.Length - 1
                && Progress >= chainStart && Progress <= Attack.chainEnd)
            { buffer.Consume(); Begin(lights[lightIndex + 1], lightIndex + 1); return; }
            // Only late light recovery permits a deliberate heavy cancel; no jump/move cancel.
            if (!Frozen && lightIndex >= 0 && buffer.Pending == CombatIntent.Heavy && Progress >= Attack.heavyCancelStart)
            { buffer.Consume(); Begin(heavy, -1); return; }
            if (!Frozen && lightIndex == 1 && buffer.Pending == CombatIntent.Kick && Progress >= chainStart && Progress <= Attack.chainEnd)
            { buffer.Consume(); Begin(kick, -1); return; }
            if (Progress >= .999f) Cancel();
        }

        void SweepActivePoses(float from, float to)
        {
            Physics.SyncTransforms(); // The dummy's controlled knockback is transform-driven.
            float begin = Mathf.Max(from, Attack.activeStart), end = Mathf.Min(to, Attack.activeEnd);
            // Animator normalized time is the ONLY phase clock. Evaluate exact authored
            // poses at the clipped active interval, sweep between them, then restore the
            // visible current pose. This also handles a low-FPS frame skipping Active.
            var fist = ContactPoint;
            Vector3 rootNow = transform.position;
            Vector3 rootOffset = previousRoot - rootNow;
            Attack.clip.SampleAnimation(motor.visual.gameObject, begin * Attack.clip.length);
            Vector3 last = fist.position + rootOffset * (1 - Mathf.InverseLerp(from, to, begin));
            int steps = Mathf.Clamp(Mathf.CeilToInt((end - begin) / .025f), 1, 16);
            for (int step = 0; step <= steps; step++)
            {
                float phase = Mathf.Lerp(begin, end, step / (float)steps);
                Attack.clip.SampleAnimation(motor.visual.gameObject, phase * Attack.clip.length);
                Vector3 next = fist.position + rootOffset * (1 - Mathf.InverseLerp(from, to, phase));
                int count = Physics.OverlapCapsuleNonAlloc(last, next, Attack.radius, contacts, 1 << 8, QueryTriggerInteraction.Collide);
                for (int i = 0; i < count; i++)
                {
                    var target = contacts[i].GetComponentInParent<TrainingDummy>();
                    if (target == null || hitTargets.Contains(target)) continue;
                    Vector3 toward = Vector3.ProjectOnPlane(target.transform.position - transform.position, Vector3.up).normalized;
                    if (!AttackRules.ValidTarget(team, target.team, target.Alive, Vector3.Dot(motor.visual.forward, toward))) continue;
                    if (!target.ReceiveHit(Attack, motor.visual.forward)) continue;
                    hitTargets.Add(target);
                    feedback?.Contact(next, Attack.heavy);
                    freezeRemaining = Mathf.Max(freezeRemaining, Attack.hitstop);
                    motor.animator.speed = 0;
                }
                last = next;
            }
            Attack.clip.SampleAnimation(motor.visual.gameObject, Progress * Attack.clip.length);
            if (Frozen) HitboxOpen = false;
        }

        public void Cancel()
        {
            Attack = null; Progress = previous = 0; HitboxOpen = false;
            hitTargets.Clear(); freezeRemaining = 0; buffer.Clear();
            if (motor != null && motor.animator != null) { motor.animator.speed = 1; motor.ResumeLocomotion(); }
        }
        public void ResetCombat() { Cancel(); buffer.Clear(); dummy?.ResetTraining(); feedback?.Clear(); }
        void OnDisable() { Cancel(); buffer.Clear(); }

        void OnDrawGizmos()
        {
            var owner = motor != null ? motor : GetComponent<PlayerMotor>();
            if (owner == null || !owner.ShowDebug || !Attacking) return;
            Gizmos.color = HitboxOpen ? Color.red : Color.yellow;
            var fist = ContactPoint;
            if (fist != null) Gizmos.DrawWireSphere(fist.position, Attack.radius);
        }
    }
}
