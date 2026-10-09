using UnityEngine;

namespace WombatLab
{
    public enum RecoveryState { Standing, Falling, Down, GettingUp, Dead }

    // Owns knockdown/get-up/death only; ordinary hitstun remains with the damage recipient.
    [DefaultExecutionOrder(20)]
    public sealed class BodyRecovery : MonoBehaviour
    {
        public Animator animator;
        public CombatController clock;
        public AnimationClip fallClip, getUpClip;
        public float fallSeconds = .42f, downSeconds = .4f, getUpSeconds = .8f, protectionSeconds = .45f;
        public RecoveryState State { get; private set; }
        public bool Busy => State != RecoveryState.Standing;
        public bool Protected => Busy || protection > 0;
        public float ProtectionRemaining => protection;
        float elapsed, protection;
        bool deathHeld;
        Collider[] colliders;
        bool[] colliderEnabled;
        CharacterController capsule;
        Vector3 capsuleCenter;
        float capsuleHeight;
        FighterTarget fighter;

        void Awake() { Cache(); ResetBody(); }
        void Cache()
        {
            if (colliders != null) return;
            capsule = GetComponent<CharacterController>();
            fighter = GetComponent<FighterTarget>();
            if (capsule != null) { capsuleCenter = capsule.center; capsuleHeight = capsule.height; }
            colliders = GetComponentsInChildren<Collider>(true);
            colliderEnabled = new bool[colliders.Length];
            for (int i = 0; i < colliders.Length; i++) colliderEnabled[i] = colliders[i].enabled;
        }
        public void KnockDown()
        {
            if (Protected) return;
            Begin(RecoveryState.Falling);
        }
        public void Die() { if (State != RecoveryState.Dead) Begin(RecoveryState.Dead); }
        void Begin(RecoveryState state)
        {
            Cache(); GetComponent<CombatController>()?.Cancel();
            GetComponent<PlayerMotor>()?.ClearJumpBuffer();
            GetComponent<AnimationReaction>()?.Clear();
            State = state; elapsed = protection = 0; deathHeld = false;
            foreach (var collider in colliders) if (collider != capsule) collider.enabled = false;
            // Keep floor/arena resolution and gravity, with a low body instead of a standing capsule.
            if (capsule != null) { capsule.height = capsule.radius * 2; capsule.center = new Vector3(capsuleCenter.x, capsule.radius, capsuleCenter.z); }
            animator.enabled = true; Play(state == RecoveryState.Dead ? "Death" : "Knockdown", fallClip, fallSeconds);
        }
        void Update()
        {
            if (clock != null && clock.GameplayBlocked) return;
            bool frozen = State != RecoveryState.Dead && (capsule == null && fighter != null ? fighter.Frozen : clock != null && clock.Frozen);
            float dt = frozen ? 0 : Time.deltaTime;
            if (!Busy) { protection = Mathf.Max(0, protection - dt); return; }
            elapsed += dt;
            float duration = State == RecoveryState.GettingUp ? getUpSeconds : fallSeconds;
            animator.speed = frozen || State == RecoveryState.Down ? 0
                : State == RecoveryState.Dead && elapsed >= fallSeconds ? 0
                : (State == RecoveryState.GettingUp ? getUpClip.length : fallClip.length) / duration;
            if (State == RecoveryState.Falling && elapsed >= fallSeconds)
            {
                State = RecoveryState.Down; elapsed = 0; Hold("Knockdown");
            }
            else if (State == RecoveryState.Down && elapsed >= downSeconds)
            {
                State = RecoveryState.GettingUp; elapsed = 0; Play("GetUp", getUpClip, getUpSeconds);
            }
            else if (State == RecoveryState.GettingUp && elapsed >= getUpSeconds)
            {
                State = RecoveryState.Standing; elapsed = 0; protection = protectionSeconds;
                RestoreColliders(); animator.speed = 1; animator.Play("Idle", 0, 0); animator.Update(0);
                GetComponent<PlayerMotor>()?.ResumeLocomotion();
            }
            else if (State == RecoveryState.Dead && elapsed >= fallSeconds && !deathHeld) { Hold("Death"); deathHeld = true; }
        }
        void Play(string name, AnimationClip clip, float seconds)
        { animator.speed = clip.length / seconds; animator.Play(name, 0, 0); animator.Update(0); }
        void Hold(string name)
        { animator.Play(name, 0, 1); animator.Update(0); animator.speed = 0; }
        void RestoreColliders()
        {
            for (int i = 0; i < colliders.Length; i++) colliders[i].enabled = colliderEnabled[i];
            if (capsule != null) { capsule.height = capsuleHeight; capsule.center = capsuleCenter; }
        }
        public void ResetBody()
        {
            Cache(); State = RecoveryState.Standing; elapsed = protection = 0; deathHeld = false;
            RestoreColliders(); GetComponent<AnimationReaction>()?.Clear();
            if (animator != null) { animator.enabled = true; animator.speed = 1; if (gameObject.activeInHierarchy) { animator.Play("Idle", 0, 0); animator.Update(0); } }
            GetComponent<PlayerMotor>()?.ResumeLocomotion();
        }
        void OnDisable() { if (colliders != null) ResetBody(); }
    }
}
