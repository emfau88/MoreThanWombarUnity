using UnityEngine;
using System.Collections.Generic;

namespace WombatLab
{
    public enum ChapterPhase { Arrival, Fighting, BetweenWaves, Travel, Complete }

    [DefaultExecutionOrder(-30)]
    public sealed class JunkyardChapter : MonoBehaviour
    {
        public ChapterDefinition definition;
        public EngagementCoordinator encounter;
        public ArenaCamera cameraRig;
        public ChapterGate[] entries, exits;
        public ChapterGate switchGate;
        public Transform switchPosition, checkpointMarker;
        public Renderer switchLamp;
        public Transform spawnWarning;
        public ChapterSession session;
        public ChapterPhase Phase { get; private set; }
        public int AreaIndex { get; private set; }
        public int WaveIndex { get; private set; }
        public int CompletedAreas { get; private set; }
        public bool GateOpened { get; private set; }
        public int CheckpointArea { get; private set; }
        public int SpawnedInArea { get; private set; }
        public int PendingEnemies => pending.Count;
        public bool ReinforcementWarning => warnedSpawn != null;
        public Vector3 ReinforcementPosition => warningPosition;
        public int PlannedEnemies { get { int n = 0; foreach (var area in definition.areas) n += Count(area); return n; } }
        public int DefeatedEnemies
        {
            get { int n = 0; for (int i = 0; i < AreaIndex; i++) n += Count(definition.areas[i]); return n + SpawnedInArea - encounter.LivingCount; }
        }
        public bool CanInteract => Phase == ChapterPhase.Travel && AreaIndex == 0 && !GateOpened
            && defense.Alive && !body.Busy && !combat.Attacking && player.Grounded
            && Vector3.ProjectOnPlane(player.transform.position - switchPosition.position, Vector3.up).magnitude < 2.2f;

        PlayerMotor player;
        PlayerDefense defense;
        BodyRecovery body;
        CombatController combat;
        LabInput input;
        CharacterDefinition originalMovement, movement;
        ChapterPhase checkpointPhase;
        Vector3 checkpointSpawn;
        int checkpointHealth;
        float checkpointEnergy;
        bool checkpointGateOpened;
        readonly List<ChapterEnemySpawn> pending = new List<ChapterEnemySpawn>();
        readonly Collider[] spawnContacts = new Collider[24];
        ChapterEnemySpawn warnedSpawn;
        Vector3 warningPosition;
        float spawnTimer;
        int nextAccess;
        MaterialPropertyBlock lampProperties;

        void Awake()
        {
            player = encounter.player; player.chapter = this; encounter.chapter = this;
            defense = player.GetComponent<PlayerDefense>(); body = player.GetComponent<BodyRecovery>();
            combat = player.GetComponent<CombatController>(); input = player.GetComponent<LabInput>();
            originalMovement = player.definition;
            movement = Instantiate(originalMovement); player.definition = movement;
            lampProperties = new MaterialPropertyBlock();
        }
        void Start() { RestartChapter(); }
        void Update()
        {
            if (input.GameplayBlocked) return;
            var frame = input.Read();
            if (frame.ChapterRestart) { RestartChapter(); return; }
            if (frame.Restart) { RetryCheckpoint(); return; }
            if (!defense.Alive || Phase == ChapterPhase.Complete) return;
            if (frame.Interact) TryInteract();
            if (Phase == ChapterPhase.Arrival)
            {
                if (player.transform.position.x >= definition.areas[0].center - 4.8f) BeginArea(0);
            }
            else if (Phase == ChapterPhase.Fighting)
            {
                var area = definition.areas[AreaIndex];
                if (WaveIndex + 1 < area.waves.Length && player.transform.position.x >= area.center + area.waves[WaveIndex + 1].triggerOffset)
                    ActivateEncounter(WaveIndex + 1);
                PumpReinforcements();
                if (WaveIndex == area.waves.Length - 1 && pending.Count == 0 && encounter.LivingCount == 0) ClearArea();
            }
            else if (Phase == ChapterPhase.Travel && (AreaIndex != 0 || GateOpened)
                && player.transform.position.x >= definition.areas[AreaIndex + 1].center - 4.8f)
                BeginArea(AreaIndex + 1);
        }
        void BeginArea(int index)
        {
            encounter.ClearWave(); ClearPending();
            AreaIndex = index; WaveIndex = 0; SpawnedInArea = 0;
            float center = definition.areas[index].center;
            movement.arenaMin = new Vector2(center - 7, definition.minimum.y);
            movement.arenaMax = new Vector2(center + 7, definition.maximum.y);
            encounter.arenaMin = new Vector2(center - 6.5f, -2);
            encounter.arenaMax = new Vector2(center + 6.5f, 2);
            cameraRig.scrolling = false; cameraRig.center = center;
            SetCombatGates(index);
            SaveCheckpoint(ChapterPhase.Fighting, new Vector3(center - 5.6f, .05f, 0));
            Phase = ChapterPhase.Fighting; ActivateEncounter(0);
        }
        static int Count(ChapterArea area)
        {
            int n = 0; foreach (var wave in area.waves) n += wave.enemies.Length; return n;
        }
        void ActivateEncounter(int index)
        {
            WaveIndex = index;
            pending.AddRange(definition.areas[AreaIndex].waves[index].enemies);
        }
        int LivingThrowers()
        {
            int n = 0; foreach (var enemy in encounter.enemies)
                if (enemy.gameObject.activeSelf && enemy.target.Alive && enemy.role.role == EnemyRole.Thrower) n++;
            return n;
        }
        bool CanSpawn(ChapterEnemySpawn spawn)
        {
            return encounter.LivingCount < definition.activeLimit
                && (spawn.role.role != EnemyRole.Thrower || LivingThrowers() < definition.throwerLimit);
        }
        bool ClearAccess(Vector3 position)
        {
            if (Vector3.ProjectOnPlane(player.transform.position - position, Vector3.up).sqrMagnitude < 2.8f * 2.8f) return false;
            foreach (var enemy in encounter.enemies)
                if (enemy.target.Alive && (enemy.transform.position - position).sqrMagnitude < 1.1f * 1.1f) return false;
            return Physics.OverlapCapsuleNonAlloc(position + Vector3.up * .65f, position + Vector3.up * 1.4f,
                .42f, spawnContacts, ~0, QueryTriggerInteraction.Ignore) == 0;
        }
        void PumpReinforcements()
        {
            spawnTimer -= Time.deltaTime;
            if (warnedSpawn != null)
            {
                // Revalidate after the warning: never materialize on a player who moved into it.
                if (!CanSpawn(warnedSpawn) || !ClearAccess(warningPosition))
                { CancelWarning(); spawnTimer = .15f; return; }
                if (spawnTimer > 0) return;
                encounter.AppendEnemy(warnedSpawn, warningPosition);
                pending.Remove(warnedSpawn); SpawnedInArea++;
                CancelWarning(); spawnTimer = definition.spawnInterval; return;
            }
            if (spawnTimer > 0 || pending.Count == 0) return;
            var candidate = pending.Find(CanSpawn); if (candidate == null) return;
            float center = definition.areas[AreaIndex].center;
            for (int i = 0; i < 4; i++)
            {
                int access = (nextAccess + i) % 4;
                var position = new Vector3(center + (access < 2 ? 5.8f : -5.8f), 0, access % 2 == 0 ? 1.55f : -1.55f);
                if (!ClearAccess(position)) continue;
                nextAccess = (access + 1) % 4; warningPosition = position; warnedSpawn = candidate;
                spawnTimer = definition.spawnWarningSeconds;
                if (spawnWarning != null) { spawnWarning.position = position + Vector3.up * .04f; spawnWarning.gameObject.SetActive(true); }
                return;
            }
        }
        void CancelWarning()
        {
            warnedSpawn = null;
            if (spawnWarning != null) spawnWarning.gameObject.SetActive(false);
        }
        void ClearPending()
        {
            pending.Clear(); CancelWarning(); spawnTimer = 0; nextAccess = 0;
        }
        void ClearArea()
        {
            CompletedAreas = AreaIndex + 1;
            defense.RestoreHealth(definition.areaHeal);
            combat.RestoreEnergy(combat.Energy + definition.areaEnergy);
            SetCombatGates(-1); cameraRig.scrolling = true;
            if (CompletedAreas == definition.areas.Length)
            {
                Phase = ChapterPhase.Complete;
                movement.arenaMin = definition.minimum; movement.arenaMax = definition.maximum;
                SaveCheckpoint(ChapterPhase.Complete, player.transform.position);
            }
            else
            {
                Phase = ChapterPhase.Travel;
                SetTravelBounds();
                SaveCheckpoint(ChapterPhase.Travel, new Vector3(definition.areas[AreaIndex].center + 8.5f, .05f, 0));
            }
        }
        void SaveCheckpoint(ChapterPhase phase, Vector3 position)
        {
            CheckpointArea = AreaIndex; checkpointPhase = phase; checkpointSpawn = position;
            checkpointHealth = defense.Health; checkpointGateOpened = GateOpened;
            checkpointEnergy = combat.Energy;
            checkpointMarker.position = new Vector3(position.x, .012f, position.z);
        }
        void SetTravelBounds()
        {
            movement.arenaMin = definition.minimum;
            movement.arenaMax = new Vector2(definition.areas[AreaIndex + 1].center + 7, definition.maximum.y);
        }
        void SetCombatGates(int active)
        {
            for (int i = 0; i < entries.Length; i++)
            { entries[i].SetClosed(i == active); exits[i].SetClosed(i == active); }
        }
        void SetSwitch(bool opened)
        {
            GateOpened = opened; switchGate.SetClosed(!opened);
            lampProperties.SetColor("_BaseColor", opened ? new Color(.22f,.85f,.55f) : new Color(1,.7f,.12f));
            switchLamp.SetPropertyBlock(lampProperties);
        }
        public bool TryInteract()
        {
            if (!CanInteract) return false;
            SetSwitch(true); checkpointGateOpened = true; return true;
        }
        public void RetryCheckpoint()
        {
            encounter.ClearWave(); ClearPending();
            AreaIndex = CheckpointArea; WaveIndex = 0;
            SetSwitch(checkpointGateOpened); SetCombatGates(-1);
            player.ResetAt(checkpointSpawn, Quaternion.LookRotation(Vector3.right));
            defense.ResetDefense(checkpointHealth);
            combat.RestoreEnergy(checkpointEnergy);
            if (checkpointPhase == ChapterPhase.Fighting)
            { CompletedAreas = AreaIndex; BeginArea(AreaIndex); }
            else
            {
                Phase = checkpointPhase;
                SpawnedInArea = Phase == ChapterPhase.Arrival ? 0 : Count(definition.areas[AreaIndex]);
                CompletedAreas = Phase == ChapterPhase.Arrival ? 0 : AreaIndex + 1;
                cameraRig.scrolling = true;
                if (Phase == ChapterPhase.Travel) SetTravelBounds();
                else
                {
                    movement.arenaMin = definition.minimum;
                    movement.arenaMax = Phase == ChapterPhase.Arrival
                        ? new Vector2(definition.areas[0].center + 7, definition.maximum.y) : definition.maximum;
                }
            }
            cameraRig.Snap(); Physics.SyncTransforms();
        }
        public void RestartChapter()
        {
            encounter.ClearWave(); ClearPending(); AreaIndex = WaveIndex = CompletedAreas = CheckpointArea = SpawnedInArea = 0;
            SetCombatGates(-1); SetSwitch(false);
            movement.arenaMin = definition.minimum;
            movement.arenaMax = new Vector2(definition.areas[0].center + 7, definition.maximum.y);
            player.ResetAt(definition.start, Quaternion.LookRotation(Vector3.right));
            Phase = ChapterPhase.Arrival; cameraRig.scrolling = true;
            cameraRig.minimumX = definition.minimum.x + 2; cameraRig.maximumX = definition.maximum.x - 4;
            cameraRig.Snap(); SaveCheckpoint(ChapterPhase.Arrival, definition.start);
            Physics.SyncTransforms();
        }
        public string Hint(bool touch, bool gamepad = false)
        {
            string retry = touch ? "CHECKPOINT" : gamepad ? "Checkpoint im Menü" : "R: Checkpoint";
            if (!defense.Alive) return "Besiegt · " + retry + " · " + (touch ? "VON VORN" : "BACKSPACE: Von vorn");
            if (Phase == ChapterPhase.Complete) return "Schrotthof geschafft! · " + (touch ? "VON VORN: Nochmal" : "BACKSPACE: Nochmal");
            string prefix = (AreaIndex + 1) + "/" + definition.areas.Length + " " + definition.areas[AreaIndex].title + " · ";
            if (Phase == ChapterPhase.Arrival) return prefix + "Zum gelben Kampffeld →";
            if (Phase == ChapterPhase.Travel) return prefix + (AreaIndex == 0 && !GateOpened
                ? CanInteract ? (touch ? "TOR ÖFFNEN" : gamepad ? "LB: Schalter — Tor öffnen" : "F: Schalter — Tor öffnen") : "Zum gelben Schalter →"
                : "Checkpoint gesetzt · Weiter nach rechts →");
            var area = definition.areas[AreaIndex];
            string action = ReinforcementWarning ? "Verstärkung am markierten Zugang!"
                : WaveIndex + 1 < area.waves.Length ? "Vorstoßen →" : "Bereich freikämpfen";
            return prefix + area.waves[WaveIndex].title + " · " + encounter.LivingCount + " aktiv · "
                + DefeatedEnemies + "/" + PlannedEnemies + " besiegt\n" + action;
        }
        void OnDestroy()
        {
            if (player != null && player.definition == movement) player.definition = originalMovement;
            if (movement != null) Destroy(movement);
        }
    }
}
