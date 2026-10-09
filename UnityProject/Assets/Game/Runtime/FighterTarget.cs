using System.Collections.Generic;
using UnityEngine;

namespace WombatLab
{
    // Small shared target/damage adapter; the existing recipients still own health and reactions.
    public sealed class FighterTarget : MonoBehaviour
    {
        public static readonly List<FighterTarget> Active = new List<FighterTarget>();
        PlayerDefense player;
        TrainingDummy enemy;
        CombatController combat;
        BodyRecovery body;
        EnemyBrain brain;
        public int Team => enemy != null ? enemy.team : combat != null ? combat.team : 1;
        public bool Alive => isActiveAndEnabled && (enemy != null ? enemy.Alive : player != null && player.Alive);
        public bool Protected => body != null && body.Protected || player != null && player.Invulnerable;
        public bool Heavy => brain != null && brain.role != null && brain.role.role == EnemyRole.Heavy;
        public Vector3 AimPoint => transform.position + Vector3.up * .9f;
        public float FreezeRemaining { get; private set; }
        public bool Frozen => FreezeRemaining > 0;
        void Awake()
        {
            player = GetComponent<PlayerDefense>(); enemy = GetComponent<TrainingDummy>();
            combat = GetComponent<CombatController>(); body = GetComponent<BodyRecovery>(); brain = GetComponent<EnemyBrain>();
        }
        void OnEnable() { if (!Active.Contains(this)) Active.Add(this); }
        void OnDisable() { Active.Remove(this); FreezeRemaining = 0; }
        void Update() { FreezeRemaining = Mathf.Max(0, FreezeRemaining - Time.deltaTime); }
        public bool Receive(AttackDefinition attack, Vector3 direction, int damage = -1)
        {
            if (!Alive || Protected) return false;
            bool accepted = enemy != null ? enemy.ReceiveHit(attack, direction, damage)
                : player.ReceiveDamage(damage >= 0 ? damage : attack.damage, direction, attack.knocksDown);
            if (accepted && enemy != null) FreezeRemaining = Mathf.Max(FreezeRemaining, attack.hitstop);
            return accepted;
        }
        public void ResetFreeze() { FreezeRemaining = 0; }
    }

    public static class CombatHit
    {
        static readonly RaycastHit[] walls = new RaycastHit[64];
        public static bool Opponent(int team, FighterTarget target)
            => target != null && target.Alive && target.Team != team && !target.Protected;
        public static bool ClearPath(Vector3 from, Vector3 to)
        {
            Vector3 delta = to - from;
            int count = Physics.RaycastNonAlloc(from, delta.normalized, walls, delta.magnitude, ~0, QueryTriggerInteraction.Ignore);
            for (int i = 0; i < count; i++)
                if (walls[i].collider.GetComponentInParent<FighterTarget>() == null) return false;
            return true;
        }
    }
}
