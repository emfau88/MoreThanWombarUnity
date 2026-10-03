using UnityEngine;
using UnityEngine.UI;

namespace WombatLab
{
    public sealed class LabHud : MonoBehaviour
    {
        public PlayerMotor player;
        public Text stateText;
        public TrainingDummy dummy;
        public Text healthText;
        public RectTransform healthFill;
        public EngagementCoordinator encounter;
        CombatController combat;
        float nextRefresh;
        void Awake() { combat = player.GetComponent<CombatController>(); }
        void Update()
        {
            if (player == null || stateText == null || Time.unscaledTime < nextRefresh) return;
            nextRefresh = Time.unscaledTime + .1f;
            var p = player.transform.position;
            string status = combat != null && combat.Attacking ? $"{combat.Attack.stateName} / {combat.Phase}" : player.State.ToUpperInvariant();
            if (encounter != null)
            {
                var defense = player.GetComponent<PlayerDefense>();
                stateText.text = !defense.Alive ? "PLAYER DOWN — R TO RETRY" : encounter.LivingCount == 0 ? "ARENA CLEAR — R TO RETRY / 2 FOR TWO OPPONENTS" :
                    $"S3 / {encounter.Mode} OPPONENT(S)   ·   {encounter.LivingCount} LEFT   ·   {(defense.Evading ? "EVADE" : status)}   ·   {(encounter.Owner != null ? encounter.Owner.State : "WATCH THE ORANGE WARNING")}";
                healthText.text = $"YOU {defense.Health}/{defense.maxHealth} HP   ·   {(defense.Cooldown > 0 ? "EVADE WAIT" : "EVADE READY")}";
                healthFill.anchorMax = new Vector2(defense.Health / (float)defense.maxHealth, 1);
                return;
            }
            stateText.text = player.ShowDebug
                ? $"{status}    X {p.x:0.00} Z {p.z:0.00} Y {p.y:0.00}  GROUND {player.Grounded}  ATTACK #{combat?.AttackInstance ?? 0}"
                : dummy != null ? $"{status}   /   S2 — FACE TARGET, TAP J FOR THE LIGHT CHAIN" : $"{status} / S1 MOVEMENT LAB";
            if (dummy != null && healthText != null && healthFill != null)
            {
                healthText.text = dummy.Alive ? $"TARGET   {dummy.Health} / {dummy.maxHealth} HP   ·   {dummy.HitCount} HITS" : "TARGET DOWN — R TO RESET";
                healthFill.anchorMax = new Vector2(dummy.Health / (float)dummy.maxHealth, 1);
            }
        }
    }
}
