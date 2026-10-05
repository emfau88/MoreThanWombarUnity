using UnityEngine;

namespace WombatLab
{
    [DefaultExecutionOrder(-10)]
    [RequireComponent(typeof(PlayerMotor), typeof(LabInput), typeof(CombatController))]
    public sealed class PlayerDefense : MonoBehaviour
    {
        public int maxHealth = 100;
        public int Health { get; private set; }
        public bool Alive => Health > 0;
        public bool Evading => dodgeRemaining > 0;
        public bool Invulnerable => (Evading && DodgeElapsed >= .05f && DodgeElapsed <= .24f) || body != null && body.Protected;
        public bool Locked => !Alive || stun > 0 || body != null && body.Busy;
        public float DodgeElapsed => .32f - dodgeRemaining;
        public Vector3 Motion => Evading ? dodgeDirection * 8 : knockback;
        public float Cooldown => cooldown;
        PlayerMotor motor;
        CombatController combat;
        LabInput input;
        BodyRecovery body;
        Vector3 dodgeDirection, knockback;
        float dodgeRemaining, cooldown, stun;

        void Awake()
        { motor = GetComponent<PlayerMotor>(); combat = GetComponent<CombatController>(); input = GetComponent<LabInput>(); body = GetComponent<BodyRecovery>(); ResetDefense(); }
        void Update()
        {
            float dt = Time.deltaTime;
            dodgeRemaining = Mathf.Max(0, dodgeRemaining - dt);
            cooldown = Mathf.Max(0, cooldown - dt); stun = Mathf.Max(0, stun - dt);
            knockback *= Mathf.Exp(-10 * dt);
            var frame = input.Read();
            if (frame.Evade) TryEvade(frame.Move);
        }
        public bool TryEvade(Vector2 direction)
        {
            if (Locked || cooldown > 0 || !motor.Grounded) return false;
            dodgeDirection = direction.sqrMagnitude > .01f ? MotorMath.PlanarInput(direction).normalized : motor.visual.forward;
            combat.Cancel(); motor.ClearJumpBuffer(); knockback = Vector3.zero;
            dodgeRemaining = .32f; cooldown = .65f;
            return true;
        }
        public bool ReceiveDamage(int damage, Vector3 direction, bool knocksDown = false)
        {
            if (!Alive || Invulnerable) return false;
            Health = Mathf.Max(0, Health - damage); stun = .28f; dodgeRemaining = 0;
            knockback = Vector3.ProjectOnPlane(direction, Vector3.up).normalized * 3;
            combat.Cancel(); motor.ClearJumpBuffer();
            if (!Alive && body != null) body.Die();
            else if (knocksDown && body != null) body.KnockDown();
            else if (Alive) GetComponent<AnimationReaction>()?.Play(damage >= 20, stun);
            return true;
        }
        public void ResetDefense()
        { Health = maxHealth; dodgeRemaining = cooldown = stun = 0; knockback = Vector3.zero; GetComponent<AnimationReaction>()?.Clear(); GetComponent<BodyRecovery>()?.ResetBody(); }
    }
}
