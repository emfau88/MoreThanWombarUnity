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
        public Text energyText;
        public RectTransform energyFill;
        public EngagementCoordinator encounter;
        public bool duelPresentation;
        public JunkyardChapter chapter;
        CombatController combat;
        float nextRefresh;
        void Awake() { combat = player.GetComponent<CombatController>(); }
        void Update()
        {
            if (player == null || stateText == null || Time.unscaledTime < nextRefresh) return;
            nextRefresh = Time.unscaledTime + .1f;
            if (energyText != null)
            {
                energyText.text = Time.time < combat.EnergyWarningUntil ? "ZU WENIG MP — COMBO AUFLADEN"
                    : $"MP  {Mathf.FloorToInt(combat.Energy)} / {combat.maxEnergy:0}" + (combat.LastGroupHits > 1 && Time.time < combat.GroupMessageUntil ? $"  ·  {combat.LastGroupHits} TREFFER" : "");
                if (energyFill != null) energyFill.anchorMax = new Vector2(combat.Energy / combat.maxEnergy, 1);
            }
            var p = player.transform.position;
            string status = combat != null && combat.Attacking ? $"{combat.Attack.stateName} / {combat.Phase}" : player.State.ToUpperInvariant();
            var body = player.GetComponent<BodyRecovery>();
            if (body != null) status = body.Busy ? body.State.ToString().ToUpperInvariant() : body.Protected ? "GETUP PROTECTION" : status;
            if (encounter != null)
            {
                var defense = player.GetComponent<PlayerDefense>();
                if (chapter != null)
                {
                    bool touch = player.GetComponent<LabInput>().TouchControls?.Visible == true;
                    stateText.text = chapter.Hint(touch, chapter.session?.UsingGamepad == true);
                    healthText.text = $"DU   {defense.Health} / {defense.maxHealth}";
                    healthFill.anchorMax = new Vector2(defense.Health / (float)defense.maxHealth, 1);
                    return;
                }
                if (duelPresentation)
                {
                    bool touch = player.GetComponent<LabInput>().TouchControls?.Visible == true;
                    string hint = body != null && body.Busy ? "Aufstehen ..." : body != null && body.Protected ? "Aufstehschutz" :
                        encounter.Owner != null && encounter.Owner.State == "TELEGRAPH" ?
                            (encounter.Owner.role?.role == EnemyRole.Agile ? "AGILE: aus der Spur!" : encounter.Owner.role?.role == EnemyRole.Heavy ? "HEAVY: ausweichen oder unterbrechen!" : "Warnung: ausweichen oder unterbrechen!") :
                        combat.ChargeCommitted ? (combat.Progress <= combat.Attack.ActiveEnd && !combat.ChargeStopped ? "Schulterstoß" : "Stoß erholt sich ...") :
                        defense.Evading ? "Ausweichen" : defense.Cooldown > 0 ? "Ausweichen erholt sich ..." : touch ? "Ausweichen bereit" : "SHIFT: Ausweichen bereit";
                    string opponent = encounter.Mode == 1 ? $"{encounter.enemies[0].RoleName} {encounter.enemies[0].target.Health}/{encounter.enemies[0].target.maxHealth}" : $"{encounter.LivingCount} Gegner übrig";
                    stateText.text = !defense.Alive ? (touch ? "Besiegt · NEUSTART: Neuer Versuch" : "Besiegt · R: Neuer Versuch") : encounter.LivingCount == 0 ? (touch ? "Gewonnen! · NEUSTART: Nochmal" : "Gewonnen! · R: Nochmal · 3/4: Mischkampf") :
                        player.ShowDebug ? $"{status} · {opponent} · {hint}" : $"{opponent} · {hint}";
                    healthText.text = $"DU   {defense.Health} / {defense.maxHealth}";
                    healthFill.anchorMax = new Vector2(defense.Health / (float)defense.maxHealth, 1);
                    return;
                }
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
