using UnityEngine;

namespace WombatLab
{
    [CreateAssetMenu(menuName = "Wombat Lab/Attack")]
    public sealed class AttackDefinition : ScriptableObject
    {
        public AnimationClip clip;
        public string stateName;
        public bool rightHand, heavy;
        public bool foot, airborne;
        public bool knocksDown;
        public float forwardStep = .18f;
        [Tooltip("Ground-only shoulder charge distance. Zero retains normal attack steps.")]
        public float chargeDistance;
        [Range(0, 1)] public float moveRelease = .82f;
        [Range(0, 1)] public float hitChainStart = .48f;
        public int damage = 10;
        public float radius = .30f, knockback = .6f, hitstun = .22f, hitstop = .045f;
        [Range(0, 1)] public float activeStart = .25f, activeEnd = .46f;
        [Range(0, 1)] public float chainStart = .58f, chainEnd = .88f;
        [Range(0, 1)] public float heavyCancelStart = .80f;

        [Header("Authored timing (zero keeps legacy clip timing)")]
        public float startupSeconds, activeSeconds, recoverySeconds;
        public float Duration => startupSeconds + activeSeconds + recoverySeconds;
        public bool HasAuthoredTiming => startupSeconds > 0 && activeSeconds > 0 && recoverySeconds > 0;
        public float ActiveStart => HasAuthoredTiming ? startupSeconds / Duration : activeStart;
        public float ActiveEnd => HasAuthoredTiming ? (startupSeconds + activeSeconds) / Duration : activeEnd;

        [Header("Contact path baked on this avatar and clip, in facing-root coordinates")]
        public Avatar contactAvatar;
        public AnimationClip contactClip;
        public Vector3[] contactPoints;
        public bool HasHumanoidContact => contactAvatar != null && contactClip == clip && contactPoints != null && contactPoints.Length > 1;
        public Vector3 LocalContact(float phase)
        {
            float index = Mathf.Clamp01(phase) * (contactPoints.Length - 1);
            int lower = Mathf.Min(Mathf.FloorToInt(index), contactPoints.Length - 2);
            return Vector3.Lerp(contactPoints[lower], contactPoints[lower + 1], index - lower);
        }

        public bool ActiveCrossed(float previous, float current)
            => AttackRules.WindowCrossed(previous, current, ActiveStart, ActiveEnd);
    }

    public enum CombatIntent { None, Light, Heavy, Kick, Charge }

    public static class AttackRules
    {
        // Closed interval overlap, including a frame that skips the entire window.
        public static bool WindowCrossed(float previous, float current, float start, float end)
            => current >= previous && current >= start && previous <= end && start <= end;
        public static bool ValidTarget(int attackerTeam, int targetTeam, bool alive, float forwardDot)
            => attackerTeam != targetTeam && alive && forwardDot > .15f;
    }

    public sealed class CombatBuffer
    {
        public CombatIntent Pending { get; private set; }
        public float Remaining { get; private set; }
        public void Submit(CombatIntent intent)
        { if (intent != CombatIntent.None) { Pending = intent; Remaining = .30f; } }
        public void Tick(float combatDelta)
        { Remaining = Mathf.Max(0, Remaining - Mathf.Max(0, combatDelta)); if (Remaining <= 0) Clear(); }
        public CombatIntent Consume() { var intent = Pending; Clear(); return intent; }
        public void Clear() { Pending = CombatIntent.None; Remaining = 0; }
    }
}
