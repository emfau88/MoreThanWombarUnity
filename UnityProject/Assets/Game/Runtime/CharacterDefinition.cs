using UnityEngine;

namespace WombatLab
{
    [CreateAssetMenu(menuName = "Wombat Lab/Character")]
    public sealed class CharacterDefinition : ScriptableObject
    {
        [Min(0)] public float moveSpeed = 4.2f;
        [Min(0)] public float runSpeed; // Zero preserves movement-only/legacy scenes.
        [Range(0, 1)] public float airControl = .8f;
        [Min(0)] public float jumpSpeed = 8f;
        [Min(.1f)] public float gravity = 23f;
        [Min(0)] public float turnSpeed = 18f;
        [Min(0)] public float coyoteTime = .09f;
        [Min(0)] public float jumpBuffer = .12f;
        [Min(0)] public float landingRecovery = .08f;
        public Vector2 arenaMin = new Vector2(-7, -2.35f);
        public Vector2 arenaMax = new Vector2(7, 2.35f);
    }
}
