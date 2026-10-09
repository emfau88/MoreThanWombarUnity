using UnityEngine;

namespace WombatLab
{
    public enum EnemyRole { Standard, Agile, Heavy, Thrower }
    [CreateAssetMenu(menuName = "Wombat Lab/Enemy Role")]
    public sealed class EnemyRoleDefinition : ScriptableObject
    {
        public EnemyRole role;
        public string displayName;
        public AttackDefinition attack;
        public int health = 65, damage = 8;
        public float moveSpeed = 2.2f, telegraphSeconds = .45f, recoverySeconds = .35f, cooldownSeconds = .7f;
        public float engagementDistance = 1.3f, preferredDistance = 1.2f, minimumAttackDistance;
        public Color color = Color.cyan;
    }
}
