using UnityEngine;

namespace WombatLab
{
    public sealed class EnemyBrain : MonoBehaviour
    {
        public TrainingDummy target;
        public EngagementCoordinator coordinator;
        public PlayerMotor player;
        public Transform visual, fist;
        public Animator animator;
        public AttackDefinition attack;
        public EnemyRoleDefinition role;
        public Renderer warning;
        public Transform chargeWarning, roleLabel;
        public int slot;
        public float engagementDistance = 1.65f, preferredDistance = 1.25f;
        public string State { get; private set; } = "APPROACH";
        public string RoleName => role != null ? role.displayName : "BÄR";
        public bool ChargeStopped { get; private set; }
        public bool Committed => State == "TELEGRAPH" || State == "ATTACK" || State == "RECOVERY";
        public bool ReadyForAttack
        {
            get
            {
                float distance = Vector3.ProjectOnPlane(player.transform.position - transform.position, Vector3.up).magnitude;
                return isActiveAndEnabled && target.Alive && target.Hitstun <= 0 && body?.Busy != true && cooldown <= 0
                    && !Committed && distance < engagementDistance && distance >= (role != null ? role.minimumAttackDistance : 0)
                    && coordinator.IsInView(this);
            }
        }
        float elapsed, cooldown, previous;
        bool hit, changeSide;
        Vector3 chargeDirection;
        readonly Collider[] contacts = new Collider[12];
        MaterialPropertyBlock properties;
        CombatController clock;
        PlayerDefense defense;
        BodyRecovery body;
        AnimationReaction reaction;
        float Telegraph => role != null ? role.telegraphSeconds : .55f;
        float Recovery => role != null ? role.recoverySeconds : .45f;
        bool Charging => attack.chargeDistance > 0;

        void Awake()
        {
            properties = new MaterialPropertyBlock(); clock = player.GetComponent<CombatController>();
            defense = player.GetComponent<PlayerDefense>(); body = GetComponent<BodyRecovery>(); reaction = GetComponent<AnimationReaction>();
            if (role != null)
            {
                attack = role.attack; target.maxHealth = role.health;
                engagementDistance = role.engagementDistance; preferredDistance = role.preferredDistance;
            }
        }
        void Update()
        {
            float dt = clock.Frozen ? 0 : Time.deltaTime;
            animator.speed = dt > 0 ? State == "ATTACK" && attack.HasAuthoredTiming ? attack.clip.length / attack.Duration : 1 : 0;
            cooldown = Mathf.Max(0, cooldown - dt);
            if (roleLabel != null && Camera.main != null) roleLabel.rotation = Camera.main.transform.rotation;
            if (!target.Alive) { coordinator.Release(this); SetState("DOWN"); return; }
            if (body != null && body.Busy) { coordinator.Release(this); SetState(body.State.ToString().ToUpperInvariant()); return; }
            if (!defense.Alive) { Interrupt(); return; }
            if (target.Hitstun > 0 || reaction?.Active == true) { SetState("STAGGER"); return; }
            if ((State == "TELEGRAPH" || State == "ATTACK") && !coordinator.IsInView(this)) { Interrupt(); return; }
            elapsed += dt;
            var toward = Vector3.ProjectOnPlane(player.transform.position - transform.position, Vector3.up);
            if (State == "TELEGRAPH")
            {
                properties.SetColor("_BaseColor", Color.Lerp(new Color(1, .65f, .1f), Color.red, elapsed / Telegraph));
                warning.SetPropertyBlock(properties);
                if (Charging) animator.speed = 0;
                if (elapsed >= Telegraph)
                {
                    SetState("ATTACK"); animator.speed = 1; animator.Play(attack.stateName, 0, 0);
                    previous = 0; hit = ChargeStopped = false;
                }
                return;
            }
            if (State == "ATTACK") return;
            if (State == "RECOVERY")
            {
                if (elapsed > Recovery)
                {
                    coordinator.Release(this); cooldown = role != null ? role.cooldownSeconds : .65f;
                    changeSide = !changeSide; SetState("REPOSITION");
                }
                return;
            }
            if (ReadyForAttack && coordinator.TryAcquire(this))
            {
                visual.rotation = Quaternion.LookRotation(toward); chargeDirection = toward.normalized;
                SetState("TELEGRAPH"); animator.Play(Charging ? attack.stateName : "Idle", 0, 0); animator.Update(0);
                if (chargeWarning != null)
                {
                    chargeWarning.rotation = Quaternion.LookRotation(chargeDirection);
                    chargeWarning.position = transform.position + chargeDirection * attack.chargeDistance * .5f + Vector3.up * .035f;
                }
                return;
            }
            bool waiting = coordinator.Owner != null && coordinator.Owner != this || cooldown > 0;
            SetState(waiting ? "REPOSITION" : "APPROACH");
            Vector3 desired = waiting ? coordinator.WaitingPosition(this, changeSide)
                : player.transform.position - toward.normalized * preferredDistance;
            if (waiting && toward.magnitude < 1.15f) desired = transform.position - toward.normalized * 1.2f;
            var direction = Vector3.ProjectOnPlane(desired - transform.position, Vector3.up);
            float speed = role != null ? role.moveSpeed : 2.1f;
            Vector3 motion = direction.normalized * Mathf.Min(speed * dt, direction.magnitude);
            foreach (var other in coordinator.enemies)
            {
                if (other == this || !other.gameObject.activeSelf || !other.target.Alive) continue;
                Vector3 apart = Vector3.ProjectOnPlane(transform.position - other.transform.position, Vector3.up);
                if (apart.magnitude < 1.0f) motion += (apart.sqrMagnitude < .001f ? (slot % 2 == 0 ? Vector3.left : Vector3.right) : apart.normalized) * 2.4f * dt;
            }
            if (role != null) coordinator.MoveEnemy(this, motion);
            else transform.position = MotorMath.ClampGround(transform.position + motion, new Vector2(-6.5f, -2), new Vector2(6.5f, 2));
            if (toward.sqrMagnitude > .01f) visual.rotation = Quaternion.Slerp(visual.rotation, Quaternion.LookRotation(toward), 1 - Mathf.Exp(-12 * dt));
            string animation = motion.sqrMagnitude > .00001f ? "Walk" : "Idle";
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName(animation)) animator.CrossFadeInFixedTime(animation, .08f);
        }
        void LateUpdate()
        {
            if (State != "ATTACK" || clock.Frozen) return;
            var state = animator.GetCurrentAnimatorStateInfo(0);
            if (!state.IsName(attack.stateName)) { Interrupt(); return; }
            float progress = Mathf.Clamp01(state.normalizedTime);
            if (attack.ActiveCrossed(previous, progress) && (!Charging || !ChargeStopped))
            {
                float start = Mathf.Max(previous, attack.ActiveStart), end = Mathf.Min(progress, attack.ActiveEnd);
                float distance = Charging ? attack.chargeDistance * (end - start) / (attack.ActiveEnd - attack.ActiveStart) : 0;
                int steps = Mathf.Max(1, Mathf.CeilToInt((end - start) / .025f), Mathf.CeilToInt(distance / .08f));
                for (int i = 0; i < steps && (!Charging || !ChargeStopped); i++)
                {
                    float a = Mathf.Lerp(start, end, i / (float)steps), b = Mathf.Lerp(start, end, (i + 1) / (float)steps);
                    attack.clip.SampleAnimation(visual.gameObject, a * attack.clip.length); Vector3 last = fist.position;
                    bool blocked = Charging && coordinator.MoveEnemy(this, chargeDirection * (distance / steps));
                    attack.clip.SampleAnimation(visual.gameObject, b * attack.clip.length);
                    Physics.SyncTransforms(); if (!hit) Strike(last, fist.position);
                    if (Charging && (blocked || hit)) ChargeStopped = true;
                }
                attack.clip.SampleAnimation(visual.gameObject, progress * attack.clip.length);
            }
            previous = progress;
            if (progress >= .999f) SetState("RECOVERY");
            if (chargeWarning != null && ChargeStopped) chargeWarning.gameObject.SetActive(false);
        }
        void Strike(Vector3 from, Vector3 to)
        {
            int count = Physics.OverlapCapsuleNonAlloc(from, to, attack.radius, contacts, ~0, QueryTriggerInteraction.Ignore);
            for (int c = 0; c < count; c++)
            {
                if (contacts[c].GetComponent<PlayerMotor>() != player) continue;
                var toward = Vector3.ProjectOnPlane(player.transform.position - transform.position, Vector3.up).normalized;
                if (Vector3.Dot(visual.forward, toward) <= .15f) continue;
                hit = true;
                if (defense.ReceiveDamage(role != null ? role.damage : 12, visual.forward, attack.knocksDown))
                    clock.feedback?.Contact(to, attack.heavy);
                break;
            }
        }
        void SetState(string state)
        {
            if (State == state) return;
            State = state; elapsed = 0; warning.enabled = state == "TELEGRAPH";
            if (chargeWarning != null) chargeWarning.gameObject.SetActive(state == "TELEGRAPH" || state == "ATTACK" && !ChargeStopped);
            if (state == "TELEGRAPH")
            { properties.SetColor("_BaseColor", new Color(1, .65f, .1f)); warning.SetPropertyBlock(properties); }
        }
        public void Interrupt()
        {
            coordinator.Release(this); cooldown = .6f; ChargeStopped = true;
            SetState(target.Alive ? "STAGGER" : "DOWN");
            if (animator != null && target.Alive) animator.Play("Idle");
        }
        public void ResetEnemy()
        {
            if (role != null) target.maxHealth = role.health;
            target.ResetTraining(); coordinator.Release(this); cooldown = .3f + slot * .2f;
            hit = ChargeStopped = changeSide = false; previous = 0;
            SetState("APPROACH"); warning.enabled = false; if (chargeWarning != null) chargeWarning.gameObject.SetActive(false);
            animator.speed = 1; if (gameObject.activeInHierarchy) animator.Play("Idle");
        }
        void OnDisable() { if (coordinator != null) coordinator.Release(this); }
    }
}
