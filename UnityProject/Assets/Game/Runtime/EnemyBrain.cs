using UnityEngine;

namespace WombatLab
{
    public sealed class EnemyBrain : MonoBehaviour
    {
        public TrainingDummy target; // Reuse S2's proven damage/knockback recipient.
        public EngagementCoordinator coordinator;
        public PlayerMotor player;
        public Transform visual, fist;
        public Animator animator;
        public AttackDefinition attack;
        public Renderer warning;
        public int slot;
        public string State { get; private set; } = "APPROACH";
        public bool ReadyForAttack => target.Alive && target.Hitstun <= 0 && cooldown <= 0
            && State != "TELEGRAPH" && State != "ATTACK" && State != "RECOVERY"
            && Vector3.ProjectOnPlane(player.transform.position - transform.position, Vector3.up).magnitude < 1.65f;
        float elapsed, cooldown, previous;
        bool hit;
        Vector3 lastFist;
        readonly Collider[] contacts = new Collider[12];
        MaterialPropertyBlock properties;

        void Awake() { properties = new MaterialPropertyBlock(); }
        void Update()
        {
            float dt = player.GetComponent<CombatController>().Frozen ? 0 : Time.deltaTime;
            animator.speed = dt > 0 ? 1 : 0;
            cooldown = Mathf.Max(0, cooldown - dt);
            if (!target.Alive) { coordinator.Release(this); SetState("DOWN"); return; }
            if (!player.GetComponent<PlayerDefense>().Alive) { Interrupt(); return; }
            if (target.Hitstun > 0) { SetState("STAGGER"); return; }
            elapsed += dt;
            var toward = Vector3.ProjectOnPlane(player.transform.position - transform.position, Vector3.up);
            float distance = toward.magnitude;
            if (State == "TELEGRAPH")
            {
                // Lock direction when the warning begins: dodging away can genuinely make it miss.
                warning.enabled = true;
                properties.SetColor("_BaseColor", Color.Lerp(new Color(1, .65f, .1f), Color.red, elapsed / .55f));
                warning.SetPropertyBlock(properties);
                if (elapsed >= .55f) { SetState("ATTACK"); animator.Play(attack.stateName, 0, 0); previous = 0; hit = false; }
                return;
            }
            if (State == "ATTACK") return; // Active phase is driven by Animator in LateUpdate.
            if (State == "RECOVERY")
            { if (elapsed > .45f) { coordinator.Release(this); cooldown = .65f; SetState("REPOSITION"); } return; }
            if (ReadyForAttack && coordinator.TryAcquire(this))
            { visual.rotation = Quaternion.LookRotation(toward); SetState("TELEGRAPH"); animator.Play("Idle"); return; }
            SetState(coordinator.Owner != null && coordinator.Owner != this ? "REPOSITION" : "APPROACH");
            // Separate waiting positions and a short-range repulsion avoid stacked opponents.
            Vector3 desired = player.transform.position - toward.normalized * 1.25f;
            if (State == "REPOSITION") desired = player.transform.position + new Vector3(slot == 0 ? -1.8f : 1.8f, 0, .8f);
            Vector3 direction = Vector3.ProjectOnPlane(desired - transform.position, Vector3.up);
            Vector3 motion = direction.normalized * Mathf.Min(2.1f * dt, direction.magnitude);
            foreach (var other in coordinator.enemies)
            {
                if (other == this || !other.gameObject.activeSelf || !other.target.Alive) continue;
                Vector3 apart = Vector3.ProjectOnPlane(transform.position - other.transform.position, Vector3.up);
                if (apart.magnitude < 1.05f) motion += (apart.sqrMagnitude < .001f ? Vector3.right * (slot == 0 ? -1 : 1) : apart.normalized) * 2.4f * dt;
            }
            transform.position = MotorMath.ClampGround(transform.position + motion, new Vector2(-6.5f, -2), new Vector2(6.5f, 2));
            if (toward.sqrMagnitude > .01f) visual.rotation = Quaternion.Slerp(visual.rotation, Quaternion.LookRotation(toward), 1 - Mathf.Exp(-12 * dt));
            string animation = motion.sqrMagnitude > .00001f ? "Walk" : "Idle";
            if (!animator.GetCurrentAnimatorStateInfo(0).IsName(animation)) animator.CrossFadeInFixedTime(animation, .08f);
        }
        void LateUpdate()
        {
            if (State != "ATTACK" || player.GetComponent<CombatController>().Frozen) return;
            var state = animator.GetCurrentAnimatorStateInfo(0);
            if (!state.IsName(attack.stateName)) { Interrupt(); return; }
            float progress = Mathf.Clamp01(state.normalizedTime);
            if (!hit && attack.ActiveCrossed(previous, progress))
            {
                Physics.SyncTransforms();
                float start = Mathf.Max(previous, attack.activeStart), end = Mathf.Min(progress, attack.activeEnd);
                attack.clip.SampleAnimation(visual.gameObject, start * attack.clip.length); lastFist = fist.position;
                int steps = Mathf.Max(1, Mathf.CeilToInt((end - start) / .025f));
                for (int i = 0; i <= steps && !hit; i++)
                {
                    attack.clip.SampleAnimation(visual.gameObject, Mathf.Lerp(start, end, i / (float)steps) * attack.clip.length);
                    int count = Physics.OverlapCapsuleNonAlloc(lastFist, fist.position, attack.radius, contacts, ~0, QueryTriggerInteraction.Ignore);
                    for (int c = 0; c < count; c++)
                    {
                        if (contacts[c].GetComponent<PlayerMotor>() != player) continue;
                        hit = true;
                        if (player.GetComponent<PlayerDefense>().ReceiveDamage(12, visual.forward))
                            player.GetComponent<CombatController>().feedback?.Contact(fist.position, true);
                        break;
                    }
                    lastFist = fist.position;
                }
                attack.clip.SampleAnimation(visual.gameObject, progress * attack.clip.length);
            }
            previous = progress;
            if (progress >= .999f) SetState("RECOVERY");
        }
        void SetState(string state)
        {
            if (State == state) return;
            State = state; elapsed = 0; warning.enabled = state == "TELEGRAPH";
            if (state == "TELEGRAPH")
            { properties.SetColor("_BaseColor", new Color(1, .65f, .1f)); warning.SetPropertyBlock(properties); }
        }
        public void Interrupt()
        { coordinator.Release(this); cooldown = .6f; SetState(target.Alive ? "STAGGER" : "DOWN"); if (animator != null && target.Alive) animator.Play("Idle"); }
        public void ResetEnemy()
        { target.ResetTraining(); coordinator.Release(this); cooldown = .3f + slot * .2f; SetState("APPROACH"); warning.enabled = false; animator.speed = 1; animator.Play("Idle"); }
        void OnDisable() { if (coordinator != null) coordinator.Release(this); }
    }
}
