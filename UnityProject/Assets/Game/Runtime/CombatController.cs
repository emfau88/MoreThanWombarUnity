using System.Collections.Generic;
using UnityEngine;

namespace WombatLab
{
    [DefaultExecutionOrder(-20)]
    [RequireComponent(typeof(PlayerMotor), typeof(LabInput))]
    public sealed class CombatController : MonoBehaviour
    {
        public AttackDefinition[] lights;
        public AttackDefinition heavy, kick, airKick, airHeavy, shoulderCharge, pressureWave;
        public float maxEnergy = 100, energyRegeneration = 5, basicHitEnergy = 6;
        public float Energy { get; private set; }
        public bool WaitingForImpact => Attacking && Attack.groundImpact && !impactLanded;
        public int LastGroupHits { get; private set; }
        public float GroupMessageUntil { get; private set; }
        public float EnergyWarningUntil { get; private set; }
        public void RestoreEnergy(float value) { Energy = Mathf.Clamp(value, 0, maxEnergy); }
        public Transform leftFist, rightFist, leftFoot, rightFoot, shoulder;
        public Transform ContactPoint => ChargeCommitted ? shoulder : Attack == null ? rightFist : Attack.foot
            ? (Attack.rightHand ? rightFoot : leftFoot) : (Attack.rightHand ? rightFist : leftFist);
        public bool ChargeCommitted => Attacking && Attack.chargeDistance > 0;
        public bool ChargeStopped { get; private set; }
        public bool MovementReleased => Attacking && !ChargeCommitted && !Attack.airborne && Progress >= Attack.moveRelease;
        public CombatFeedback feedback;
        public TrainingDummy dummy;
        public int team = 1;
        public AttackDefinition Attack { get; private set; }
        public bool Attacking => Attack != null;
        public bool Frozen => freezeRemaining > 0;
        public bool GameplayBlocked => input != null && input.GameplayBlocked;
        public void ClearBufferedInput() { buffer.Clear(); }
        public bool HitboxOpen { get; private set; }
        public int AttackInstance { get; private set; }
        public float Progress { get; private set; }
        public string Phase => !Attacking ? "READY" : Frozen ? "HITSTOP" :
            Progress < Attack.ActiveStart ? "STARTUP" : Progress <= Attack.ActiveEnd ? "ACTIVE" : "RECOVERY";

        readonly CombatBuffer buffer = new CombatBuffer();
        readonly HashSet<TrainingDummy> hitTargets = new HashSet<TrainingDummy>();
        readonly HashSet<FighterTarget> fighterHits = new HashSet<FighterTarget>();
        readonly List<Collider> chargeIgnored = new List<Collider>();
        readonly Collider[] contacts = new Collider[128];
        PlayerMotor motor;
        LabInput input;
        PlayerDefense defense;
        float freezeRemaining, previous;
        int lightIndex;
        Vector3 previousRoot;
        Quaternion startFacing;
        Vector3 chargeDirection;
        bool impactLanded, projectileFired, energyAwarded;

        void Awake() { motor = GetComponent<PlayerMotor>(); input = GetComponent<LabInput>(); defense = GetComponent<PlayerDefense>(); Energy = maxEnergy; }
        void Update()
        {
            if (GameplayBlocked) return;
            if (!Attacking && (defense == null || defense.Alive)) RestoreEnergy(Energy + energyRegeneration * Time.deltaTime);
            var frame = input.Read();
            if (frame.Restart) { ResetCombat(); return; }
            if (defense != null && (defense.Evading || defense.Locked)) { buffer.Clear(); return; }
            if (frame.Wave) Queue(CombatIntent.Wave); else if (frame.Charge) Queue(CombatIntent.Charge); else if (frame.Heavy) Queue(CombatIntent.Heavy); else if (frame.Kick) Queue(CombatIntent.Kick); else if (frame.Light) Queue(CombatIntent.Light);
            bool wasFrozen = Frozen;
            freezeRemaining = Mathf.Max(0, freezeRemaining - Time.unscaledDeltaTime);
            motor.animator.speed = Frozen || WaitingForImpact && Progress >= Attack.ActiveStart ? 0 : Attacking && Attack.HasAuthoredTiming ? Attack.clip.length / Attack.Duration : 1;
            buffer.Tick(wasFrozen ? 0 : Time.deltaTime);
            // A simultaneous Jump + attack starts after the motor takes off, not before it.
            if (!Frozen && !Attacking && !(frame.Jump && motor.Grounded) && buffer.Pending != CombatIntent.None)
            {
                var intent = buffer.Consume();
                if ((intent == CombatIntent.Charge || intent == CombatIntent.Wave) && !motor.Grounded) return;
                Begin(intent == CombatIntent.Wave ? pressureWave : intent == CombatIntent.Charge ? shoulderCharge : !motor.Grounded ? (intent == CombatIntent.Heavy ? airHeavy : airKick)
                    : intent == CombatIntent.Heavy ? heavy : intent == CombatIntent.Kick ? kick : lights[0],
                    intent == CombatIntent.Light && motor.Grounded ? 0 : -1);
            }
        }

        public void Queue(CombatIntent intent)
        {
            // A shoulder charge neither chains out of another attack nor becomes an air kick.
            if (intent == CombatIntent.Charge && (shoulderCharge == null || Attacking || !motor.Grounded)) return;
            if (intent == CombatIntent.Wave && (pressureWave == null || Attacking || !motor.Grounded)) return;
            buffer.Submit(intent);
        }

        void Begin(AttackDefinition attack, int index)
        {
            if (attack == null) return;
            if (!attack.projectile && !attack.groundImpact && attack.clip.isHumanMotion && (!attack.HasHumanoidContact || attack.contactAvatar != motor.animator.avatar))
            { Debug.LogError("Humanoid attack needs a contact path baked for its current clip and avatar", this); return; }
            if (Energy < attack.energyCost) { EnergyWarningUntil = Time.time + 1; return; }
            RestoreEnergy(Energy - attack.energyCost);
            Attack = attack; lightIndex = index; AttackInstance++;
            hitTargets.Clear(); fighterHits.Clear(); previous = Progress = 0; HitboxOpen = false;
            impactLanded = projectileFired = energyAwarded = false;
            ChargeStopped = false;
            previousRoot = transform.position;
            // Begin and early startup share one turn budget, measured from
            // the original facing rather than an already corrected pose.
            startFacing = motor.visual.rotation;
            var move = MotorMath.PlanarInput(motor.MovementIntent);
            if (move.sqrMagnitude > .01f)
                motor.visual.rotation = attack.chargeDistance > 0 ? Quaternion.LookRotation(move)
                    : Quaternion.RotateTowards(motor.visual.rotation, Quaternion.LookRotation(move), 25);
            chargeDirection = motor.visual.forward;
            if (attack.breakthrough)
            {
                var controller = GetComponent<CharacterController>();
                foreach (var target in FighterTarget.Active)
                {
                    if (!target.Alive || target.Team == team || target.Heavy) continue;
                    var collider = target.GetComponent<CapsuleCollider>();
                    if (collider == null || Physics.GetIgnoreCollision(controller, collider)) continue;
                    Physics.IgnoreCollision(controller, collider, true); chargeIgnored.Add(collider);
                }
            }
            motor.ClearJumpBuffer();
            motor.animator.Play(attack.stateName, 0, 0);
            motor.animator.Update(0);
        }

        void LateUpdate()
        {
            if (GameplayBlocked) return;
            if (!Attacking || Frozen) { HitboxOpen = false; return; }
            var state = motor.animator.GetCurrentAnimatorStateInfo(0);
            if (!state.IsName(Attack.stateName)) { Cancel(); return; }
            Progress = Mathf.Clamp01(state.normalizedTime);
            if (WaitingForImpact)
            {
                HitboxOpen = false;
                if (Progress >= Attack.ActiveStart)
                { Progress = Attack.ActiveStart; motor.animator.Play(Attack.stateName, 0, Progress); motor.animator.Update(0); motor.animator.speed = 0; }
                previous = Progress; previousRoot = transform.position; return;
            }
            if (!ChargeCommitted && !Attack.airborne && motor.Grounded)
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
            HitboxOpen = Progress >= Attack.ActiveStart && Progress <= Attack.ActiveEnd;
            if (ChargeCommitted) TickCharge(previous, Progress);
            else if (Attack.projectile && !projectileFired && Progress >= Attack.ActiveStart)
            {
                projectileFired = true;
                CombatProjectile.Launch(Attack, team, transform.position + Vector3.up * .9f + motor.visual.forward * .55f, motor.visual.forward, feedback);
            }
            else if (!Attack.groundImpact && !Attack.projectile && Attack.ActiveCrossed(previous, Progress)) SweepActivePoses(previous, Progress);
            if (ChargeStopped) HitboxOpen = false;
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

        void TickCharge(float from, float to)
        {
            if (ChargeStopped || !motor.Grounded || !Attack.ActiveCrossed(from, to)) return;
            float begin = Mathf.Max(from, Attack.ActiveStart), end = Mathf.Min(to, Attack.ActiveEnd);
            float interval = Attack.ActiveEnd - Attack.ActiveStart;
            float distance = Attack.chargeDistance * (end - begin) / interval;
            // Short spatial steps stop at the first valid contact even if a frame skips Active.
            // Pose, motion and damage still use the single Animator phase clock.
            int steps = Mathf.Max(1, Mathf.CeilToInt(distance / .09f));
            for (int i = 0; i < steps && !ChargeStopped; i++)
            {
                float a = Mathf.Lerp(begin, end, i / (float)steps);
                float b = Mathf.Lerp(begin, end, (i + 1) / (float)steps);
                previousRoot = transform.position;
                bool blocked = motor.MoveAttackStep(chargeDirection * (distance / steps));
                SweepActivePoses(a, b);
                if (blocked) ChargeStopped = true;
            }
            if (Attack.breakthrough && distance > .01f) SpecialEffects.Trail(transform.position - chargeDirection * distance, transform.position);
        }

        void SweepActivePoses(float from, float to)
        {
            Physics.SyncTransforms(); // The dummy's controlled knockback is transform-driven.
            float begin = Mathf.Max(from, Attack.ActiveStart), end = Mathf.Min(to, Attack.ActiveEnd);
            // Animator normalized time is the ONLY phase clock. Evaluate exact authored
            // poses at the clipped active interval, sweep between them, then restore the
            // visible current pose. This also handles a low-FPS frame skipping Active.
            var fist = ContactPoint;
            Vector3 rootNow = transform.position;
            Vector3 rootOffset = previousRoot - rootNow;
            Vector3 last = ContactAt(begin, fist) + rootOffset * (1 - Mathf.InverseLerp(from, to, begin));
            int steps = Mathf.Clamp(Mathf.CeilToInt((end - begin) / .025f), 1, 16);
            for (int step = 0; step <= steps; step++)
            {
                float phase = Mathf.Lerp(begin, end, step / (float)steps);
                Vector3 next = ContactAt(phase, fist) + rootOffset * (1 - Mathf.InverseLerp(from, to, phase));
                int count = Physics.OverlapCapsuleNonAlloc(last, next, Attack.radius, contacts, 1 << 8, QueryTriggerInteraction.Collide);
                for (int i = 0; i < count; i++)
                {
                    var fighter = contacts[i].GetComponentInParent<FighterTarget>();
                    if (fighter != null)
                    {
                        Vector3 delta = Vector3.ProjectOnPlane(fighter.transform.position - transform.position, Vector3.up);
                        if (Vector3.Dot(motor.visual.forward, delta.normalized) <= .15f) continue;
                        if (!TryHit(fighter, motor.visual.forward)) continue;
                        feedback?.Contact(next, Attack.heavy);
                        if (ChargeCommitted && (!Attack.breakthrough || fighter.Heavy)) { ChargeStopped = true; break; }
                        continue;
                    }
                    var target = contacts[i].GetComponentInParent<TrainingDummy>();
                    if (target == null || hitTargets.Contains(target)) continue;
                    Vector3 toward = Vector3.ProjectOnPlane(target.transform.position - transform.position, Vector3.up).normalized;
                    if (!AttackRules.ValidTarget(team, target.team, target.Alive, Vector3.Dot(motor.visual.forward, toward))) continue;
                    if (!target.ReceiveHit(Attack, motor.visual.forward)) continue;
                    hitTargets.Add(target);
                    feedback?.Contact(next, Attack.heavy);
                    freezeRemaining = Mathf.Max(freezeRemaining, Attack.hitstop);
                    motor.animator.speed = 0;
                    if (ChargeCommitted) { ChargeStopped = true; break; }
                }
                last = next;
                if (ChargeStopped) break;
            }
            if (!Attack.clip.isHumanMotion)
                Attack.clip.SampleAnimation(motor.visual.gameObject, Progress * Attack.clip.length);
            if (Frozen) HitboxOpen = false;
        }

        bool TryHit(FighterTarget target, Vector3 direction)
        {
            if (fighterHits.Contains(target) || !CombatHit.Opponent(team, target)) return false;
            if (!CombatHit.ClearPath(transform.position + Vector3.up * .8f, target.AimPoint)) return false;
            if (!target.Receive(Attack, direction)) return false;
            fighterHits.Add(target);
            var dummyTarget = target.GetComponent<TrainingDummy>(); if (dummyTarget != null) hitTargets.Add(dummyTarget);
            SpecialEffects.Impact(target.AimPoint, true);
            freezeRemaining = Mathf.Max(freezeRemaining, Attack.hitstop); motor.animator.speed = 0;
            if (!energyAwarded && Attack.energyCost <= 0) { RestoreEnergy(Energy + basicHitEnergy); energyAwarded = true; }
            LastGroupHits = fighterHits.Count; GroupMessageUntil = Time.time + .8f;
            return true;
        }

        public void OnLanded()
        {
            if (!WaitingForImpact) { if (Attacking && Attack.airborne) Cancel(); return; }
            impactLanded = true; Progress = previous = Attack.ActiveEnd;
            Physics.SyncTransforms();
            foreach (var target in FighterTarget.Active)
            {
                Vector3 delta = target.transform.position - transform.position;
                if (Mathf.Abs(delta.y) > 1.2f || Vector3.ProjectOnPlane(delta, Vector3.up).sqrMagnitude > Attack.impactRadius * Attack.impactRadius) continue;
                TryHit(target, delta.sqrMagnitude > .001f ? delta.normalized : motor.visual.forward);
            }
            SpecialEffects.Ring(transform.position, Attack.impactRadius);
            feedback?.Contact(transform.position + Vector3.up * .12f, true);
            motor.animator.Play(Attack.stateName, 0, Progress); motor.animator.Update(0);
            motor.animator.speed = Frozen ? 0 : Attack.clip.length / Attack.Duration;
        }

        Vector3 ContactAt(float phase, Transform contact)
        {
            // Humanoid paths were evaluated by Animator on this exact avatar.
            // Querying them never rewrites the visible skeleton or its Animator state.
            if (Attack.clip.isHumanMotion) return motor.visual.TransformPoint(Attack.LocalContact(phase));
            Attack.clip.SampleAnimation(motor.visual.gameObject, phase * Attack.clip.length);
            return contact.position;
        }

        public void Cancel()
        {
            var controller = GetComponent<CharacterController>();
            foreach (var collider in chargeIgnored) if (collider != null && controller != null) Physics.IgnoreCollision(controller, collider, false);
            chargeIgnored.Clear(); fighterHits.Clear(); impactLanded = projectileFired = false;
            Attack = null; Progress = previous = 0; HitboxOpen = false;
            hitTargets.Clear(); freezeRemaining = 0; buffer.Clear();
            ChargeStopped = false; chargeDirection = Vector3.zero;
            if (motor != null && motor.animator != null) { motor.animator.speed = 1; motor.ResumeLocomotion(); }
        }
        public void ResetCombat() { Cancel(); buffer.Clear(); Energy = maxEnergy; LastGroupHits = 0; GroupMessageUntil = EnergyWarningUntil = 0; CombatProjectile.ClearAll(); GetComponent<FighterTarget>()?.ResetFreeze(); dummy?.ResetTraining(); feedback?.Clear(); }
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
