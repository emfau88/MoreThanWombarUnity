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
        public float forwardStep = .18f;
        [Range(0, 1)] public float moveRelease = .82f;
        [Range(0, 1)] public float hitChainStart = .48f;
        public int damage = 10;
        public float radius = .30f, knockback = .6f, hitstun = .22f, hitstop = .045f;
        [Range(0, 1)] public float activeStart = .25f, activeEnd = .46f;
        [Range(0, 1)] public float chainStart = .58f, chainEnd = .88f;
        [Range(0, 1)] public float heavyCancelStart = .80f;

        public bool ActiveCrossed(float previous, float current)
            => AttackRules.WindowCrossed(previous, current, activeStart, activeEnd);
    }

    public enum CombatIntent { None, Light, Heavy, Kick }

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
